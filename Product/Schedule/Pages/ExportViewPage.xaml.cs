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
using System.Drawing;

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

            ExportReportStartDatePicker.SelectedDate = DateTime.Today.Date.AddDays(-dayOfWeek + 1);
            ExportReportEndDatePicker.SelectedDate = ExportReportStartDatePicker.SelectedDate!.Value.AddDays(5);

            ScheduleExportStatusProgressBar.Maximum = 1.0;
            ReportExportStatusProgressBar.Maximum = 1.0;
        }

        private void ExportDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;

            if (datePicker.SelectedDate == null)
            {
                datePicker.SelectedDate = DateTime.Today.Date;
            }

            if (datePicker.Name.Contains("Start"))
            {
                DatePicker pairDatePicker = (FindName(datePicker.Name.Replace("Start", "End")) as DatePicker)!;

                if (pairDatePicker == null) return;

                if (pairDatePicker.SelectedDate < datePicker.SelectedDate)
                {
                    pairDatePicker.SelectedDate = datePicker.SelectedDate;
                }
            }
            else if (datePicker.Name.Contains("End"))
            {
                DatePicker pairDatePicker = (FindName(datePicker.Name.Replace("End", "Start")) as DatePicker)!;

                if (pairDatePicker == null) return;

                if (pairDatePicker.SelectedDate > datePicker.SelectedDate)
                {
                    pairDatePicker.SelectedDate = datePicker.SelectedDate;
                }
            }
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

            try
            {
                Excel.Application app = new();
            }
            catch
            {
                MessageBox.Show("Ошибка инициализации Excel документа. Проверьте наличие приложения Excel и права доступа к нему."
                    , "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

                Dispatcher.Invoke(() => ScheduleExportDefaultStyle());
            }

            if (cancellationToken.IsCancellationRequested) return;

            Excel.Application application = new();
            Excel.Workbook workbook = application.Workbooks.Add();
            Excel.Worksheet worksheet = workbook.Sheets[1];

            application.ScreenUpdating = true;

            List<DateTime> dateTimes = scheduleLessons.Select(x => x.Date).Distinct().OrderBy(x => x).ToList();
            List<string?> classes = context.Classes.Select(x => x.Name).OrderBy(x => x!.Length).ThenBy(x => x).ToList();

            Dispatcher.Invoke(() => SchduleExportStatusTextBlock.Text = "Экспортирование...");

            int exportedRecord = 0;
            int countRecord = scheduleLessons.Count();

            await Task.Run(() =>
            {
                Task.Run(async () =>
                {
                    if (countRecord <= 0) return;

                    while (exportedRecord < countRecord)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

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

                    await Task.Delay(10);
                }, cancellationToken);

                int rowMarginDate = 3;

                int rowEnd = 2 + (11 + rowMarginDate) * dateTimes.Count - 3;
                int columnEnd = 2 + 3 * classes.Count;

                // Change color borders of cells
                worksheet.Range[
                    worksheet.Cells[1, 1],
                    worksheet.Cells[rowEnd, columnEnd]
                    ].Borders.Color = ColorTranslator.ToOle(System.Drawing.Color.White);

                for (int date = 0; date < dateTimes.Count; ++date)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    for (int classItem = 0; classItem < classes.Count; ++classItem)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        int rowClass = 2 + (date * 11) + (rowMarginDate * date);
                        int columnClass = 2 + classItem * 3;

                        int rowLesson = rowClass + 2;
                        int columnLesson = columnClass;

                        async void SettingBordersAsync(int rowClassAsync, int columnClassAsync)
                        {
                            await Task.Run(() =>
                            {
                                // Change color borders of cells
                                worksheet.Range[
                                    worksheet.Cells[rowClassAsync, columnClassAsync],
                                    worksheet.Cells[rowClassAsync + 10, columnClassAsync + 2]
                                    ].Borders.Color = ColorTranslator.ToOle(System.Drawing.Color.Black);

                                // Changing size borders of cells
                                worksheet.Range[
                                    worksheet.Cells[rowClassAsync, columnClassAsync],
                                    worksheet.Cells[rowClassAsync + 10, columnClassAsync + 2]
                                    ].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            });
                        }
                        SettingBordersAsync(rowClass, columnClass);

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
                            if (cancellationToken.IsCancellationRequested) break;

                            ScheduleLesson? scheduleLesson = default!;

                            scheduleLesson = scheduleLessons
                                .Where(x => x.Date == dateTimes[date])
                                .Where(x => x.ClassLesson.Class!.Name == classes[classItem])
                                .Where(x => x.NumberLesson == numberLesson)
                                .FirstOrDefault();

                            if (scheduleLesson != null)
                            {
                                ScheduleLessonViewItemSource vis = new(scheduleLesson);

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
            }, cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                Dispatcher.Invoke(() => SchduleExportStatusTextBlock.Text = "Открытие документа...");

                await Task.Delay(500, cancellationToken);

                application.Visible = true;
                application.Interactive = true;
                application.ScreenUpdating = true;
                application.UserControl = true;
                application.ScreenUpdating = true;

                while (application.Visible == true)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    await Task.Delay(100, cancellationToken);
                }
            }

            application.Quit();

            Dispatcher.Invoke(() => ScheduleExportDefaultStyle());
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

        private CancellationTokenSource ReportExportDataToken = new();
        private Task ReportExportDataTask = null!;

        private void ReportExportDefaultStyle()
        {
            ReportExportStatusProgressBar.Value = 0;
            ReportExportStatusTextBlock.Text = null!;
            ReportExportButton.IsEnabled = true;
            ReportCancelButton.IsEnabled = false;
        }

        private async Task ReportExportAsync(CancellationToken cancellationToken = default!)
        {
            Dispatcher.Invoke(() =>
            {
                ReportExportButton.IsEnabled = false;
                ReportCancelButton.IsEnabled = true;
            });

            DateTime dateStart = ExportReportStartDatePicker.SelectedDate!.Value.Date;
            DateTime dateEnd = ExportReportEndDatePicker.SelectedDate!.Value.Date;
            string path = $"{Directory.GetCurrentDirectory()}";
            string fileName = $"Отчет_{DateTime.Now.ToString().Replace(":", ".")}";

            using DatabaseContext context = new();

            Dispatcher.Invoke(() => ReportExportStatusTextBlock.Text = "Получение расписания...");

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

            Dispatcher.Invoke(() => ReportExportStatusProgressBar.Value += 0.2);
            Dispatcher.Invoke(() => ReportExportStatusTextBlock.Text = "Инициализация документа...");

            try
            {
                Excel.Application app = new();
            }
            catch
            {
                MessageBox.Show("Ошибка инициализации Excel документа. Проверьте наличие приложения Excel и права доступа к нему."
                    , "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

                Dispatcher.Invoke(() => ScheduleExportDefaultStyle());
            }

            if (cancellationToken.IsCancellationRequested) return;

            Excel.Application application = new();
            Excel.Workbook workbook = application.Workbooks.Add();
            Excel.Worksheet worksheet = workbook.Sheets[1];

            application.ScreenUpdating = true;

            List<DateTime> dateTimes = scheduleLessons.Select(x => x.Date).Distinct().OrderBy(x => x).ToList();
            List<string?> classes = context.Classes.Select(x => x.Name).OrderBy(x => x!.Length).ThenBy(x => x).ToList();

            Dispatcher.Invoke(() => ReportExportStatusProgressBar.Value += 0.1);
            Dispatcher.Invoke(() => ReportExportStatusTextBlock.Text = "Расчет...");

            Dictionary<int, int> countConductedLessonClasses = new();
            Dictionary<int, int> countConductedLessonTeachers = new();
            int CountConductedLessons() => countConductedLessonClasses.Select(x => x.Value).Sum();

            foreach (Class classItem in context.Classes)
            {
                countConductedLessonClasses[classItem.ClassId] = scheduleLessons
                    .Where(x => x.ClassLesson.Class!.ClassId == classItem.ClassId)
                    .Count();
            }

            foreach (Teacher teacherItem in context.Teachers)
            {
                countConductedLessonTeachers[teacherItem.TeacherId] = scheduleLessons
                    .Where(x => x.ClassLesson.Teacher!.TeacherId == teacherItem.TeacherId || (x.ClassLesson.PairClassLesson != null && x.ClassLesson.PairClassLesson.Teacher!.TeacherId == teacherItem.TeacherId))
                    .Count();
            }

            Dispatcher.Invoke(() => ReportExportStatusProgressBar.Value += 0.1);
            Dispatcher.Invoke(() => ReportExportStatusTextBlock.Text = "Формирование...");

            // Filling excel document

            int CountRows() => 10 + (context.Teachers.Count() > context.Classes.Count() ? context.Teachers.Count() : context.Classes.Count()) + 1;

            // Clear color borders
            worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[CountRows(), 11]]
                .Borders.Color = ColorTranslator.ToOle(System.Drawing.Color.White);

            // Header
            worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 11]].Merge();
            worksheet.Cells[1, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            worksheet.Cells[1, 1] = "Отчет о проведенных занятиях";

            // Create report date
            worksheet.Range[worksheet.Cells[3, 1], worksheet.Cells[3, 11]].Merge();
            worksheet.Cells[3, 1] = $"Дата составления отчета: {DateTime.Now:dd.MM.yyyy HH.mm.ss}";

            // Period report
            worksheet.Range[worksheet.Cells[4, 1], worksheet.Cells[4, 11]].Merge();
            worksheet.Cells[4, 1] = $"Период отчетности: {dateStart.Date.ToShortDateString()} - {dateEnd.Date.ToShortDateString()}";

            // Count classes
            worksheet.Range[worksheet.Cells[5, 1], worksheet.Cells[5, 11]].Merge();
            worksheet.Cells[5, 1] = $"Всего классов: {context.Classes.Count()}";

            // Count teachers
            worksheet.Range[worksheet.Cells[6, 1], worksheet.Cells[6, 11]].Merge();
            worksheet.Cells[6, 1] = $"Всего педагогов: {context.Teachers.Count()}";

            // Count Lessons
            worksheet.Range[worksheet.Cells[7, 1], worksheet.Cells[7, 11]].Merge();
            worksheet.Cells[7, 1] = $"Всего проведено занятий: {CountConductedLessons()}";


            int startTables = 9;

            // Count conducted lessons by class
            worksheet.Range[worksheet.Cells[startTables, 2], worksheet.Cells[startTables, 5]].Merge();
            worksheet.Cells[startTables, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            worksheet.Cells[startTables, 2] = "Проведено занятий по классам";

            worksheet.Range[worksheet.Cells[startTables + 1, 2], worksheet.Cells[startTables + 1, 5]]
                .Borders.Color = ColorTranslator.ToOle(System.Drawing.Color.Black);

            worksheet.Range[worksheet.Cells[startTables + 1, 2], worksheet.Cells[startTables + 1, 5]]
                .Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            worksheet.Range[worksheet.Cells[startTables + 1, 2], worksheet.Cells[startTables + 1, 3]].Merge();
            worksheet.Cells[startTables + 1, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            worksheet.Cells[startTables + 1, 2] = "Класс";

            worksheet.Range[worksheet.Cells[startTables + 1, 4], worksheet.Cells[startTables + 1, 5]].Merge();
            worksheet.Cells[startTables + 1, 4].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            worksheet.Cells[startTables + 1, 4] = "Кол-во";

            int indexClass = 0;
            foreach (var classItem in countConductedLessonClasses)
            {
                int classId = classItem.Key;

                worksheet.Range[worksheet.Cells[startTables + 2 + indexClass, 2], worksheet.Cells[startTables + 2 + indexClass, 5]]
                    .Borders.Color = ColorTranslator.ToOle(System.Drawing.Color.Black);

                worksheet.Range[worksheet.Cells[startTables + 2 + indexClass, 2], worksheet.Cells[startTables + 2 + indexClass, 5]]
                    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                worksheet.Range[worksheet.Cells[startTables + 2 + indexClass, 2], worksheet.Cells[startTables + 2 + indexClass, 3]].Merge();
                worksheet.Cells[startTables + 2 + indexClass, 2] = context.Classes.First(x => x.ClassId == classId).Name;

                worksheet.Range[worksheet.Cells[startTables + 2 + indexClass, 4], worksheet.Cells[startTables + 2 + indexClass, 5]].Merge();
                worksheet.Cells[startTables + 2 + indexClass, 4] = classItem.Value;

                ++indexClass;
            }

            // Count conducted lessons by teachers
            worksheet.Range[worksheet.Cells[startTables, 7], worksheet.Cells[startTables, 10]].Merge();
            worksheet.Cells[startTables, 7].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            worksheet.Cells[startTables, 7] = "Проведено занятий по педагогам";

            worksheet.Range[worksheet.Cells[startTables + 1, 7], worksheet.Cells[startTables + 1, 10]]
                .Borders.Color = ColorTranslator.ToOle(System.Drawing.Color.Black);

            worksheet.Range[worksheet.Cells[startTables + 1, 7], worksheet.Cells[startTables + 1, 10]]
                .Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            worksheet.Range[worksheet.Cells[startTables + 1, 7], worksheet.Cells[startTables + 1, 8]].Merge();
            worksheet.Cells[startTables + 1, 7].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            worksheet.Cells[startTables + 1, 7] = "Педагог";

            worksheet.Range[worksheet.Cells[startTables + 1, 9], worksheet.Cells[startTables + 1, 10]].Merge();
            worksheet.Cells[startTables + 1, 9].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            worksheet.Cells[startTables + 1, 9] = "Кол-во";

            int indexTeacher = 0;
            foreach (var teacherItem in countConductedLessonTeachers)
            {
                int teacherId = teacherItem.Key;

                worksheet.Range[worksheet.Cells[startTables + 2 + indexTeacher, 7], worksheet.Cells[startTables + 2 + indexTeacher, 10]]
                    .Borders.Color = ColorTranslator.ToOle(System.Drawing.Color.Black);

                worksheet.Range[worksheet.Cells[startTables + 2 + indexTeacher, 7], worksheet.Cells[startTables + 2 + indexTeacher, 10]]
                    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                worksheet.Range[worksheet.Cells[startTables + 2 + indexTeacher, 7], worksheet.Cells[startTables + 2 + indexTeacher, 8]].Merge();
                worksheet.Cells[startTables + 2 + indexTeacher, 7] = context.Teachers.First(x => x.TeacherId == teacherId).ToShortString();

                worksheet.Range[worksheet.Cells[startTables + 2 + indexTeacher, 9], worksheet.Cells[startTables + 2 + indexTeacher, 10]].Merge();
                worksheet.Cells[startTables + 2 + indexTeacher, 9] = teacherItem.Value;

                ++indexTeacher;
            }

            worksheet.Columns.AutoFit();
            worksheet.Rows.AutoFit();

            // Openning excel document

            Dispatcher.Invoke(() => ReportExportStatusProgressBar.Value += 0.6);
            if (!cancellationToken.IsCancellationRequested)
            {
                Dispatcher.Invoke(() => ReportExportStatusTextBlock.Text = "Открытие документа...");

                await Task.Delay(500, cancellationToken);

                application.Visible = true;
                application.Interactive = true;
                application.ScreenUpdating = true;
                application.UserControl = true;
                application.ScreenUpdating = true;

                while (application.Visible == true)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    await Task.Delay(100, cancellationToken);
                }
            }

            application.Quit();

            Dispatcher.Invoke(() => ReportExportDefaultStyle());
        }

        private void ReportExportButton_Click(object sender, RoutedEventArgs e)
        {
            ReportExportDataTask = ReportExportAsync(ReportExportDataToken.Token);
        }

        private void ReportCancelButton_Click(object sender, RoutedEventArgs e)
        {
            ReportExportDataToken.Cancel();
            ReportExportDefaultStyle();
        }

        #endregion
    }
}
