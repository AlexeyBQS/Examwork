using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.EntityFrameworkCore;
using Schedule.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Schedule.ViewItemSources;
using Schedule.Services;

namespace Schedule.Pages
{
    /// <summary>
    /// Логика взаимодействия для ExportViewPage.xaml
    /// </summary>
    public partial class ExportViewPage : Page
    {
        public ExportViewPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)!.WindowTitle = "Расписание - Экспорт данных";

            int dayOfWeek = (int)DateTime.Today.DayOfWeek;

            ExportScheduleStartDatePicker.SelectedDate = DateTime.Today.Date.AddDays(-dayOfWeek + 1);
            ExportScheduleEndDatePicker.SelectedDate = ExportScheduleStartDatePicker.SelectedDate!.Value.AddDays(5);
            ScheduleExportStatusProgressBar.Maximum = 1.0;
        }

        #region ScheduleExport

        private CancellationTokenSource ScheduleExportDataToken = new();
        private Task ScheduleExportDataTask = null!;

        private void ScheduleExportDefaultStyle()
        {
            ScheduleExportStatusProgressBar.Value = 0;
            SchduleExportStatusTextBlock.Text = null!;
            ScheduleExportButton.IsEnabled = true;
            ScheduleCancelButton.IsEnabled = false;
        }

        private async Task ScheduleExportAsync(CancellationToken cancellationToken = default!)
        {
            Dispatcher.Invoke(() =>
            {
                ScheduleExportButton.IsEnabled = false;
                ScheduleCancelButton.IsEnabled = true;
            });

            DateTime dateStart = ExportScheduleStartDatePicker.SelectedDate!.Value.Date;
            DateTime dateEnd = ExportScheduleEndDatePicker.SelectedDate!.Value.Date;
            string path = $"{Directory.GetCurrentDirectory()}";
            string fileName = $"Расписание_{DateTime.Now.ToString().Replace(":", ".")}";

            using DatabaseContext context = new();

            Dispatcher.Invoke(() => SchduleExportStatusTextBlock.Text = "Получение расписания...");

            IQueryable<ScheduleLesson> scheduleLessons = default!;

            await Task.Run(() =>
            {
                scheduleLessons = context.ScheduleLessons
                   .Where(x => x.Date >= dateStart && x.Date <= dateEnd)
                   .Include(x => x.Cabinet)
                   .Include(x => x.PairCabinet)
                   .Include(x => x.ClassLesson)
                   .Include(x => x.ClassLesson.Class)
                   .Include(x => x.ClassLesson.Lesson)
                   .Include(x => x.ClassLesson.Teacher)
                   .Include(x => x.ClassLesson.PairClassLesson)
                   .Include(x => x.ClassLesson.PairClassLesson!.Class)
                   .Include(x => x.ClassLesson.PairClassLesson!.Lesson)
                   .Include(x => x.ClassLesson.PairClassLesson!.Teacher)
                   .OrderBy(x => x.Date)
                   .ThenBy(x => x.ClassLesson.Class!.Name);
            }, cancellationToken);

            Dispatcher.Invoke(() => SchduleExportStatusTextBlock.Text = "Инициализация документа...");

            if (Type.GetType("Microsoft.Office.Interop.Excel.Application") == null)
            {
                MessageBox.Show("Не удалось запустить приложение Microsoft Excel. Проверьте наличие приложения и права доступа.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                ScheduleExportDefaultStyle();
                ScheduleExportDataToken.Cancel();

                return;
            }

            Excel.Application application = new();
            Excel.Workbook workbook = application.Workbooks.Add();
            Excel.Worksheet worksheet = workbook.Sheets[1];

            List<DateTime> dateTimes = scheduleLessons.Select(x => x.Date).Distinct().OrderBy(x => x).ToList();
            List<string?> classes = context.Classes.Select(x => x.Name).OrderBy(x => x!.Length).ThenBy(x => x).ToList();

            Dispatcher.Invoke(() => SchduleExportStatusTextBlock.Text = "Экспортирование...");

            int exportedRecord = 0;
            int countRecord = scheduleLessons.Count();

            await Task.Run(() =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        while (exportedRecord < countRecord)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            Dispatcher.Invoke(() =>
                            {
                                SchduleExportStatusTextBlock.Text = $"Экспортирование {exportedRecord}/{countRecord}...";
                                ScheduleExportStatusProgressBar.Value = (double)exportedRecord / countRecord;
                            });
                        }

                        Dispatcher.Invoke(() =>
                        {
                            SchduleExportStatusTextBlock.Text = $"Экспортирование {exportedRecord}/{countRecord}...";
                            ScheduleExportStatusProgressBar.Value = (double)exportedRecord / countRecord;
                        });
                    }
                    catch { }
                }, cancellationToken);

                try
                {
                    for (int date = 0; date < dateTimes.Count; ++date)
                    {
                        for (int classItem = 0; classItem < classes.Count; ++classItem)
                        {
                            int rowClass = 1 + date * 11;
                            int columnClass = 1 + classItem * 3;

                            // Date
                            worksheet.Range[
                                worksheet.Cells[rowClass, columnClass],
                                worksheet.Cells[rowClass, columnClass + 2]
                                ].Merge();

                            worksheet.Cells[rowClass, columnClass].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            worksheet.Cells[rowClass, columnClass] =
                                $"{Service.ConvertDayOfWeekToShortString(dateTimes[date].DayOfWeek)} - {dateTimes[date]:dd.MM.yyyy}";

                            // ClassName
                            worksheet.Range[
                                worksheet.Cells[rowClass + 1, columnClass],
                                worksheet.Cells[rowClass + 1, columnClass + 2]
                                ].Merge();

                            worksheet.Cells[rowClass + 1, columnClass].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            worksheet.Cells[rowClass + 1, columnClass] = classes[classItem];

                            // Header ScheduleLesson
                            worksheet.Cells[rowClass + 2, columnClass] = "№";
                            worksheet.Cells[rowClass + 2, columnClass + 1] = "Предмет";
                            worksheet.Cells[rowClass + 2, columnClass + 2] = "Каб.";

                            // ScheduleLessons
                            for (int numberLesson = 1; numberLesson <= 8; ++numberLesson)
                            {
                                cancellationToken.ThrowIfCancellationRequested();

                                ScheduleLesson? scheduleLesson = scheduleLessons
                                    .Where(x => x.Date == dateTimes[date])
                                    .Where(x => x.ClassLesson.Class!.Name == classes[classItem])
                                    .Where(x => x.NumberLesson == numberLesson)
                                    .FirstOrDefault();

                                if (scheduleLesson != null)
                                {
                                    ScheduleLessonViewItemSource vis = new(scheduleLesson);

                                    int rowLesson = rowClass + 2;
                                    int columnLesson = columnClass;

                                    //NumberLesson
                                    worksheet.Cells[rowLesson + numberLesson, columnLesson] = numberLesson.ToString();

                                    //LessonName
                                    worksheet.Cells[rowLesson + numberLesson, columnLesson + 1].Font.Bold = vis.IsBold;
                                    worksheet.Cells[rowLesson + numberLesson, columnLesson + 1] = vis.ScheduleLesson_Name;

                                    //CabinetName
                                    worksheet.Cells[rowLesson + numberLesson, columnLesson + 2] = vis.ScheduleLesson_Cabinet;

                                    ++exportedRecord;
                                }
                            }
                        }
                    }

                    worksheet.Columns.AutoFit();
                }
                catch { }
            }, cancellationToken);


            try
            {
                Dispatcher.Invoke(() => SchduleExportStatusTextBlock.Text = "Открытие документа...");

                application.Visible = true;
                application.Interactive = true;
                application.ScreenUpdating = true;
                application.UserControl = true;

                while (application.Visible == true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Thread.Sleep(100);
                }
            }
            catch { }
            finally
            {
                application.Quit();
            }

            Dispatcher.Invoke(() => ScheduleExportDefaultStyle());
        }

        private void ExportScheduleDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;

            if (datePicker.SelectedDate == null)
            {
                datePicker.SelectedDate = DateTime.Today.Date;
            }

            if (datePicker.Name == "ExportScheduleStartDatePicker")
            {
                if (ExportScheduleEndDatePicker.SelectedDate < ExportScheduleStartDatePicker.SelectedDate)
                {
                    ExportScheduleEndDatePicker.SelectedDate = ExportScheduleStartDatePicker.SelectedDate;
                }
            }
            else if (datePicker.Name == "ExportScheduleEndDatePicker")
            {
                if (ExportScheduleStartDatePicker.SelectedDate > ExportScheduleEndDatePicker.SelectedDate)
                {
                    ExportScheduleStartDatePicker.SelectedDate = ExportScheduleEndDatePicker.SelectedDate;
                }
            }
        }

        private void ScheduleExportButton_Click(object sender, RoutedEventArgs e)
        {
            ScheduleExportDataTask = ScheduleExportAsync(ScheduleExportDataToken.Token);
        }

        private void ScheduleCancelButton_Click(object sender, RoutedEventArgs e)
        {
            ScheduleExportDataToken.Cancel();
            ScheduleExportDefaultStyle();
        }

        #endregion

        #region Report



        #endregion
    }
}
