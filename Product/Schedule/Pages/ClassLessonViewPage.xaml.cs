using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для ClassLessonViewPage.xaml
    /// </summary>
    public partial class ClassLessonViewPage : Page
    {
        public ClassLessonViewPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)!.WindowTitle = "Расписание - Дисциплины класса";

            UpdateDataClassListBox();
            UpdateDataLessonAndClassLessonListBox();
        }

        #region [Updaters]

        private void UpdateDataClassListBox()
        {
            using DatabaseContext context = new();

            IQueryable<Class> classes = context.Classes
                .Include(_class => _class.Teacher)
                .Include(_class => _class.Cabinet)
                .Include(_class => _class.Cabinet!.Teacher);

            IEnumerable<ClassViewItemSource> viewItemSources = classes.ToList().Select(x => new ClassViewItemSource(x));

            ClassListBox.ItemsSource = viewItemSources;
        }

        private void UpdateDataLessonAndClassLessonListBox()
        {
            if (ClassListBox.SelectedItems.Count > 0)
            {
                int? classId = (ClassListBox.SelectedItems[0] as ClassViewItemSource)?.ClassId ?? null!;

                if (classId != null)
                {
                    using DatabaseContext context = new();

                    Class? _class = context.Classes.FirstOrDefault(x => x.ClassId == classId);

                    if (_class != null)
                    {
                        IQueryable<Lesson> lessons = context.Lessons;

                        List<ClassLessonViewItemSource> viewItemSources = lessons
                            .ToList().Select(x => new ClassLessonViewItemSource(x)).ToList();

                        viewItemSources.ForEach(x =>
                            x.ClassLesson = context.ClassLessons
                                .Where(classLesson => classLesson.LessonId == x.LessonId)
                                .FirstOrDefault(classLesson => classLesson.ClassId == (int)classId)
                            );

                        LessonAndClassLessonListBox.ItemsSource = viewItemSources;
                    }
                }
            }
            else
            {
                LessonAndClassLessonListBox.ItemsSource = null!;
            }
        }

        private void DefaultViewTabControl()
        {
            ClassLessonIdTextBox.Text = null!;
            ClassIdComboBox.SelectedIndex = -1;
            LessonIdComboBox.SelectedIndex = -1;
            TeacherIdComboBox.SelectedIndex = -1;
            PairClassLessonIdComboBox.SelectedIndex = -1;
            DefaultCabinetIdComboBox.SelectedIndex = -1;
            CountLessonTextBox.Text = null!;
            LeftLessonsTextBox.Text = null!;
            DifficultyTextBox.Text = null!;

            ClassLessonIdTextBox.IsEnabled = false;
            ClassIdComboBox.IsEnabled = false;
            LessonIdComboBox.IsEnabled = false;
            TeacherIdComboBox.IsEnabled = false;
            PairClassLessonIdComboBox.IsEnabled = false;
            DefaultCabinetIdComboBox.IsEnabled = false;
            CountLessonTextBox.IsEnabled = false;
            LeftLessonsTextBox.IsEnabled = false;
            DifficultyTextBox.IsEnabled = false;

            //ClassIdComboBoxClearButton.IsEnabled = false;
            //LessonIdComboBoxClearButton.IsEnabled = false;
            TeacherIdComboBoxClearButton.IsEnabled = false;
            PairClassLessonIdComboBoxClearButton.IsEnabled = false;
            DefaultCabinetIdComboBoxClearButton.IsEnabled = false;

            AddClassLessonButton.IsEnabled = false;
            SaveChangeClassLessonButton.IsEnabled = false;
            DeleteClassLessonButton.IsEnabled = false;
        }

        private void UpdateViewTabControl()
        {
            if (LessonAndClassLessonListBox.SelectedItems.Count > 0)
            {
                ClassLessonViewItemSource lessonAndClassLesson =
                    (LessonAndClassLessonListBox.SelectedItems[0] as ClassLessonViewItemSource)!;

                if (lessonAndClassLesson.ClassLesson != null)
                {
                    ClassLessonIdTextBox.Text = lessonAndClassLesson.ClassLessonId.ToString();

                    using DatabaseContext context = new();

                    IEnumerable<Class> classes = context.Classes.ToList();

                    ClassIdComboBox.ItemsSource = classes;
                    ClassIdComboBox.SelectedIndex = lessonAndClassLesson.ClassId != null
                        ? ClassIdComboBox.Items.IndexOf(context.Classes.First(c => c.ClassId == lessonAndClassLesson.ClassId))
                        : -1;

                    IEnumerable<Lesson> lessons = context.Lessons.ToList();

                    LessonIdComboBox.ItemsSource = lessons;
                    LessonIdComboBox.SelectedIndex = LessonIdComboBox.Items
                        .IndexOf(context.Lessons.First(l => l.LessonId == lessonAndClassLesson.LessonId));

                    IEnumerable<Teacher> teachers = context.Teachers.ToList();

                    TeacherIdComboBox.ItemsSource = teachers;
                    TeacherIdComboBox.SelectedIndex = lessonAndClassLesson.TeacherId != null
                        ? TeacherIdComboBox.Items.IndexOf(context.Teachers.First(t => t.TeacherId == lessonAndClassLesson.TeacherId))
                        : -1;

                    IEnumerable<ClassLesson> classLessons = context.ClassLessons
                        .Include(cl => cl.Lesson)
                        .Where(classLesson => classLesson.ClassId == lessonAndClassLesson.ClassId &&
                                                (classLesson.PairClassLessonId == null
                                                || classLesson.PairClassLessonId == lessonAndClassLesson.ClassLessonId) &&
                                              classLesson.LessonId != lessonAndClassLesson.LessonId)
                        .ToList();

                    PairClassLessonIdComboBox.ItemsSource = classLessons;
                    PairClassLessonIdComboBox.SelectedIndex = lessonAndClassLesson.PairClassLessonId != null
                        ? PairClassLessonIdComboBox.Items.IndexOf(context.ClassLessons.First(cl => cl.ClassLessonId == lessonAndClassLesson.PairClassLessonId))
                        : -1;

                    IEnumerable<Cabinet> cabinets = context.Cabinets.ToList();

                    DefaultCabinetIdComboBox.ItemsSource = cabinets;
                    DefaultCabinetIdComboBox.SelectedIndex = lessonAndClassLesson.DefaultCabinetId != null
                        ? DefaultCabinetIdComboBox.Items.IndexOf(context.Cabinets.First(c => c.CabinetId == lessonAndClassLesson.DefaultCabinetId))
                        : -1;

                    
                    CountLessonTextBox.Text = lessonAndClassLesson.CountLesson.ToString();
                    LeftLessonsTextBox.Text = (lessonAndClassLesson.CountLesson - context.ScheduleLessons.Count(sl => sl.ClassLessonId == lessonAndClassLesson.ClassLessonId)).ToString();
                    DifficultyTextBox.Text = lessonAndClassLesson.Difficulty.ToString();

                    ClassLessonIdTextBox.IsEnabled = true;
                    ClassIdComboBox.IsEnabled = true;
                    LessonIdComboBox.IsEnabled = true;
                    TeacherIdComboBox.IsEnabled = true;
                    PairClassLessonIdComboBox.IsEnabled = true;
                    DefaultCabinetIdComboBox.IsEnabled = true;
                    CountLessonTextBox.IsEnabled = true;
                    LeftLessonsTextBox.IsEnabled = true;
                    DifficultyTextBox.IsEnabled = true;

                    TeacherIdComboBoxClearButton.IsEnabled = true;
                    PairClassLessonIdComboBoxClearButton.IsEnabled = true;
                    DefaultCabinetIdComboBoxClearButton.IsEnabled = true;

                    AddClassLessonButton.IsEnabled = false;
                    SaveChangeClassLessonButton.IsEnabled = false;
                    DeleteClassLessonButton.IsEnabled = true;
                }
                else
                {
                    DefaultViewTabControl();

                    AddClassLessonButton.IsEnabled = true;
                }
            }
            else
            {
                DefaultViewTabControl();
            }
        }

        #endregion

        #region [WorkWithListBox]

        private void ClassListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDataLessonAndClassLessonListBox();
        }

        private void LessonAndClassLessonLessonListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateViewTabControl();
        }

        #endregion

        #region [ViewRecord]

        private void ViewTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveChangeClassLessonButton.IsEnabled = true;
        }

        private void ViewComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveChangeClassLessonButton.IsEnabled = true;
        }

        #endregion

        #region [WorkWithRecord]

        private void AddClassLessonButton_Click(object sender, RoutedEventArgs e)
        {
            if (LessonAndClassLessonListBox.SelectedItems.Count > 0 &&
                !(LessonAndClassLessonListBox.SelectedItems[0] as ClassLessonViewItemSource)!.ClassLesson_IsEnabled)
            {
                int classId = (ClassListBox.SelectedItems[0] as ClassViewItemSource)!.ClassId;
                int lessonId = (LessonAndClassLessonListBox.SelectedItems[0] as ClassLessonViewItemSource)!.LessonId;

                using (DatabaseContext context = new())
                {
                    ClassLesson classLesson = new();

                    classLesson.ClassId = classId;
                    classLesson.LessonId = lessonId;

                    context.ClassLessons.Add(classLesson);
                    context.SaveChangesAsync();
                }

                UpdateDataLessonAndClassLessonListBox();

                using (DatabaseContext context = new())
                {
                    IEnumerable<ClassLessonViewItemSource> items =
                        (LessonAndClassLessonListBox.ItemsSource as IEnumerable<ClassLessonViewItemSource>) ?? null!;

                    ClassLessonViewItemSource? selectedItem = items?.FirstOrDefault(i => i.LessonId == lessonId);

                    if (selectedItem != null)
                    {
                        LessonAndClassLessonListBox.SelectedIndex = LessonAndClassLessonListBox.Items
                            .IndexOf(selectedItem);
                    }
                }
            }
        }

        private void SaveChangeClassLessonButton_Click(object sender, RoutedEventArgs e)
        {
            if (LessonAndClassLessonListBox.SelectedItems.Count > 0)
            {
                ClassLessonViewItemSource viewItemSource =
                    (LessonAndClassLessonListBox.SelectedItems[0] as ClassLessonViewItemSource) ?? null!;

                if (viewItemSource != null)
                {
                    int lessonId = viewItemSource.LessonId;
                    int? classLessonId = viewItemSource.ClassLessonId;

                    if (classLessonId != null)
                    {
                        using DatabaseContext context = new();

                        ClassLesson classLesson = context.ClassLessons.First(cl => cl.ClassLessonId == (int)classLessonId);

                        classLesson.TeacherId = TeacherIdComboBox.SelectedIndex >= 0
                            ? (TeacherIdComboBox.Items[TeacherIdComboBox.SelectedIndex] as Teacher)!.TeacherId
                            : null!;

                        int? previousPairClassLessonId = classLesson.PairClassLessonId;

                        classLesson.PairClassLessonId = PairClassLessonIdComboBox.SelectedIndex >= 0
                            ? (PairClassLessonIdComboBox.Items[PairClassLessonIdComboBox.SelectedIndex] as ClassLesson)!.ClassLessonId
                            : null!;

                        if (classLesson.PairClassLessonId != previousPairClassLessonId)
                        {
                            if (previousPairClassLessonId != null && classLesson.PairClassLessonId == null)
                            {
                                ClassLesson pairClassLesson = context.ClassLessons
                                    .First(cl => cl.ClassLessonId == (int)previousPairClassLessonId);

                                pairClassLesson.PairClassLessonId = null!;
                            }
                            else if (previousPairClassLessonId == null && classLesson.PairClassLessonId != null)
                            {
                                ClassLesson pairClassLesson = context.ClassLessons
                                    .First(cl => cl.ClassLessonId == (int)classLesson.PairClassLessonId);

                                pairClassLesson.PairClassLessonId = classLesson.ClassLessonId;
                            }
                            else if (previousPairClassLessonId != null && classLesson.PairClassLessonId != null)
                            {
                                ClassLesson oldPairClassLesson = context.ClassLessons
                                    .First(cl => cl.ClassLessonId == (int)previousPairClassLessonId);

                                oldPairClassLesson.PairClassLessonId = null!;

                                ClassLesson newPairClassLesson = context.ClassLessons
                                    .First(cl => cl.ClassLessonId == (int)classLesson.PairClassLessonId);

                                newPairClassLesson.PairClassLessonId = classLesson.ClassLessonId;
                            }
                        }

                        classLesson.DefaultCabinetId = DefaultCabinetIdComboBox.SelectedIndex >= 0
                            ? (DefaultCabinetIdComboBox.Items[DefaultCabinetIdComboBox.SelectedIndex] as Cabinet)!.CabinetId
                            : null!;

                        classLesson.CountLesson = CountLessonTextBox.Text != string.Empty ? int.Parse(CountLessonTextBox.Text) : null!;
                        classLesson.Difficulty = DifficultyTextBox.Text != string.Empty ? int.Parse(DifficultyTextBox.Text) : null!;

                        context.SaveChangesAsync();
                    }

                    UpdateDataLessonAndClassLessonListBox();

                    using (DatabaseContext context = new())
                    {
                        IEnumerable<ClassLessonViewItemSource> items =
                            (LessonAndClassLessonListBox.ItemsSource as IEnumerable<ClassLessonViewItemSource>) ?? null!;

                        ClassLessonViewItemSource? selectedItem = items?.FirstOrDefault(i => i.LessonId == lessonId);

                        if (selectedItem != null)
                        {
                            LessonAndClassLessonListBox.SelectedIndex = LessonAndClassLessonListBox.Items
                                .IndexOf(selectedItem);
                        }
                    }
                }
            }
        }

        private void DeleteClassLessonButton_Click(object sender, RoutedEventArgs e)
        {
            if (LessonAndClassLessonListBox.SelectedItems.Count > 0)
            {
                ClassLessonViewItemSource viewItemSource =
                    (LessonAndClassLessonListBox.SelectedItems[0] as ClassLessonViewItemSource) ?? null!;

                if (viewItemSource != null)
                {
                    using (DatabaseContext context = new())
                    {
                        ClassLesson classLesson = context.ClassLessons.First(cl => cl.ClassLessonId == viewItemSource.LessonId);

                        context.ClassLessons.Remove(classLesson);
                        context.SaveChangesAsync();
                    }

                    UpdateDataLessonAndClassLessonListBox();
                }
            }
        }

        private void TeacherIdComboBoxClearButton_Click(object sender, RoutedEventArgs e)
        {
            TeacherIdComboBox.SelectedIndex = -1;
        }

        private void DefaultCabinetIdComboBoxClearButton_Click(object sender, RoutedEventArgs e)
        {
            DefaultCabinetIdComboBox.SelectedIndex = -1;
        }

        private void PairClassLessonIdComboBoxClearButton_Click(object sender, RoutedEventArgs e)
        {
            PairClassLessonIdComboBox.SelectedIndex = -1;
        }

        #endregion

        private void OnlyDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Service.OnlyDigit_PreviewTextInput(sender, e);
            base.OnPreviewTextInput(e);
        }
    }
}
