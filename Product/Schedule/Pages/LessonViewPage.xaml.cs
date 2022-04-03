using Schedule.Database;
using Schedule.Services;
using Schedule.ViewItemSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Schedule.Pages
{
    /// <summary>
    /// Логика взаимодействия для LessonViewPage.xaml
    /// </summary>
    public partial class LessonViewPage : Page
    {
        public LessonViewPage()
        {
            InitializeComponent();
        }

        private int CountViewRecord { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)!.WindowTitle = "Расписание - Дисциплины";

            Service.FillViewCountRecordComboBox(ViewCountRecordComboBox);

            UpdateDataListBox();
            UpdateViewTabControl();
        }

        #region [Updaters]

        private void UpdateDataListBox()
        {
            using DatabaseContext context = new();

            IQueryable<Lesson> lessons = context.Lessons;

            if (LessonIdFilterTextBox.Text != null)
            {
                string lessonId = LessonIdFilterTextBox.Text;
                lessons = lessons.Where(lesson => lesson.LessonId.ToString().Contains(lessonId));
            }

            if (NameFilterTextBox.Text != null)
            {
                string name = NameFilterTextBox.Text;
                lessons = lessons.Where(lesson => lesson.Name!.Contains(name));
            }

            if (DescriptionFilterTextBox.Text != null)
            {
                string description = DescriptionFilterTextBox.Text;
                lessons = lessons.Where(lesson => lesson.Description!.Contains(description));
            }

            IEnumerable<LessonViewItemSource> viewItemSources = CountViewRecord > 0
                ? lessons.ToList().Take(CountViewRecord).Select(x => new LessonViewItemSource(x))
                : lessons.ToList().Select(x => new LessonViewItemSource(x));

            LessonListBox.ItemsSource = viewItemSources;

            StatusTextBlock.Text = $"Всего: {context.Lessons.Count()} | Всего с фильтрами: {lessons.Count()} | Отображается: {viewItemSources.Count()}";
        }

        private void DefaultViewTabControl()
        {
            LessonIdTextBox.Text = null!;
            NameTextBox.Text = null!;
            DescriptionTextBox.Text = null!;

            SaveChangeLessonButton.IsEnabled = false;
            DeleteLessonButton.IsEnabled = false;

            LessonIdTextBox.IsEnabled = false;
            NameTextBox.IsEnabled = false;
            DescriptionTextBox.IsEnabled = false;
        }

        private void UpdateViewTabControl()
        {
            if (LessonListBox.SelectedItems.Count > 0)
            {
                LessonViewItemSource lesson = LessonListBox.SelectedItems[0] as LessonViewItemSource ?? null!;

                if (lesson != null)
                {
                    LessonIdTextBox.Text = lesson.LessonId.ToString();
                    NameTextBox.Text = lesson.Name;
                    DescriptionTextBox.Text = lesson.Description;

                    SaveChangeLessonButton.IsEnabled = false;
                    DeleteLessonButton.IsEnabled = true;

                    LessonIdTextBox.IsEnabled = true;
                    NameTextBox.IsEnabled = true;
                    DescriptionTextBox.IsEnabled = true;
                }
                else
                {
                    DefaultViewTabControl();
                }
            }
            else
            {
                DefaultViewTabControl();
            }
        }

        #endregion

        #region [WorkWithListBox]

        private void LessonListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateViewTabControl();
        }

        private void ViewCountRecordComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Service.ViewCountRecord viewCountRecord = ((sender as ComboBox)!.SelectedItem as Service.ViewCountRecord)!;
            if (viewCountRecord != null)
            {
                CountViewRecord = viewCountRecord.Count;
            }

            UpdateDataListBox();
        }

        #endregion

        #region [ViewRecord]

        private void ViewTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveChangeLessonButton.IsEnabled = true;
        }

        #endregion

        #region [WorkWithRecord]

        private void AddLessonButton_Click(object sender, RoutedEventArgs e)
        {
            using (DatabaseContext context = new())
            {
                context.Lessons.Add(new Lesson());
                context.SaveChangesAsync();
            }

            UpdateDataListBox();

            LessonListBox.SelectedIndex = LessonListBox.Items.Count - 1;
        }

        private void SaveChangeLessonButton_Click(object sender, RoutedEventArgs e)
        {
            if (LessonListBox.SelectedItems.Count > 0)
            {
                int? lessonId = (LessonListBox.SelectedItems[0] as LessonViewItemSource)?.LessonId ?? null!;

                if (lessonId != null)
                {
                    using (DatabaseContext context = new())
                    {
                        Lesson? lesson = context.Lessons.FirstOrDefault(x => x.LessonId == (int)lessonId);

                        if (lesson != null)
                        {
                            lesson.Name = NameTextBox.Text;
                            lesson.Description = DescriptionTextBox.Text;
                        }

                        context.SaveChangesAsync();
                    }

                    UpdateDataListBox();
                }
            }
        }

        private void DeleteLessonButton_Click(object sender, RoutedEventArgs e)
        {
            if (LessonListBox.SelectedItems.Count > 0)
            {
                if (Message.Action_DeleteRecord() == MessageBoxResult.Yes)
                {
                    int? lessonid = (LessonListBox.SelectedItems[0] as LessonViewItemSource)?.LessonId ?? -1;

                    using (DatabaseContext context = new())
                    {
                        Lesson? lesson = context.Lessons.FirstOrDefault(l => l.LessonId == (int)lessonid);

                        if (lesson != null)
                        {
                            context.Lessons.Remove(lesson);

                            context.SaveChangesAsync();
                        }
                    }

                    UpdateDataListBox();
                }
            }
        }

        #endregion

        #region [WorkWithFiltres]

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDataListBox();
        }

        private void ClearFiltresButton_Click(object sender, RoutedEventArgs e)
        {
            LessonIdFilterTextBox.Text = null!;
            NameFilterTextBox.Text = null!;
            DescriptionFilterTextBox.Text = null!;
        }

        #endregion

        private void OnlyDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Service.OnlyDigit_PreviewTextInput(sender, e);
            base.OnPreviewTextInput(e);
        }
    }
}
