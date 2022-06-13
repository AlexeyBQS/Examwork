using Microsoft.EntityFrameworkCore;
using Schedule.Database;
using Schedule.Database.Models;
using Schedule.Services;
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
using System.Windows.Shapes;

namespace Schedule.Windows
{
    /// <summary>
    /// Логика взаимодействия для ScheduleLessonChangeWindow.xaml
    /// </summary>
    public partial class ScheduleLessonChangeWindow : Window
    {
        private ScheduleLessonChangeWindow()
        {
            InitializeComponent();
        }
        public ScheduleLessonChangeWindow(DateTime date, int classId) : this()
        {
            Date = date;
            ClassId = classId;

            MaxDifficultyClass = (new DatabaseContext()).Classes.First(x => x.ClassId == ClassId).MaxDifficulty ?? 0;
        }

        private DateTime Date { get; set; } = default!;
        private int ClassId { get; set; } = default!;

        private int MaxDifficultyClass { get; set; } = default!;
        private int CurrentDifficulty { get; set; } = default!;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using DatabaseContext context = new();

            Title = $"Изменение расписания";
            TitleTextBox.Text = $"Изменение расписания на {Date:dd.MM.yyyy} для класса {context.Classes.First(x => x.ClassId == ClassId).Name}";

            DefaultStyle();
            LoadDataComboBox();
            RecalculateDifficulty();
        }

        private void DefaultStyle()
        {
            for (int numberLesson = 1; numberLesson <= 8; ++numberLesson)
            {
                (FindName($"Cabinet_Lesson{numberLesson}_ComboBox") as ComboBox)!.IsEnabled = false;
                (FindName($"Cabinet_Lesson{numberLesson}_ComboBox_ClearButton") as Button)!.IsEnabled = false;
                (FindName($"PairCabinet_Lesson{numberLesson}_ComboBox") as ComboBox)!.IsEnabled = false;
                (FindName($"PairCabinet_Lesson{numberLesson}_ComboBox_ClearButton") as Button)!.IsEnabled = false;
                (FindName($"IsBold_Lesson{numberLesson}_CheckBox") as CheckBox)!.IsEnabled = false;
            }
        }

        private void LoadDataComboBox()
        {
            using DatabaseContext context = new();

            IQueryable<ClassLesson> classLessons = context.ClassLessons
                .Include(x => x.Lesson)
                .Include(x => x.Teacher)
                .Include(x => x.DefaultCabinet)
                .Include(x => x.PairClassLesson)
                .Include(x => x.PairClassLesson!.Lesson)
                .Include(x => x.PairClassLesson!.Teacher)
                .Include(x => x.PairClassLesson!.DefaultCabinet)
                .Where(x => x.ClassId == ClassId);

            IQueryable<Cabinet> cabinets = context.Cabinets;

            IQueryable<ScheduleLesson> scheduleLessons = context.ScheduleLessons
                .Include(x => x.ClassLesson)
                .Include(x => x.Cabinet)
                .Include(x => x.PairCabinet)
                .Where(x => x.Date == Date)
                .Where(x => classLessons.Contains(x.ClassLesson));

            for (int numberLesson = 1; numberLesson <= 8; ++numberLesson)
            {
                ComboBox lessonComboBox = (FindName($"Lesson_Lesson{numberLesson}_ComboBox") as ComboBox)!;
                ComboBox cabinetComboBox = (FindName($"Cabinet_Lesson{numberLesson}_ComboBox") as ComboBox)!;
                ComboBox pairCabinetComboBox = (FindName($"PairCabinet_Lesson{numberLesson}_ComboBox") as ComboBox)!;
                CheckBox isBoldCheckBox = (FindName($"IsBold_Lesson{numberLesson}_CheckBox") as CheckBox)!;

                lessonComboBox.ItemsSource = classLessons.ToList();
                cabinetComboBox.ItemsSource = cabinets.ToList();
                pairCabinetComboBox.ItemsSource = cabinets.ToList();

                lessonComboBox.SelectedIndex = scheduleLessons.FirstOrDefault(x => x.NumberLesson == numberLesson) != null
                    ? lessonComboBox.Items.IndexOf(classLessons.First(x => x.ClassLessonId == scheduleLessons.First(y => y.NumberLesson == numberLesson).ClassLessonId))
                    : -1;

                cabinetComboBox.SelectedIndex = scheduleLessons.FirstOrDefault(x => x.NumberLesson == numberLesson)?.CabinetId != null
                    ? cabinetComboBox.Items.IndexOf(cabinets.First(x => x.CabinetId == scheduleLessons.First(y => y.NumberLesson == numberLesson).CabinetId))
                    : -1;

                pairCabinetComboBox.SelectedIndex = scheduleLessons.FirstOrDefault(x => x.NumberLesson == numberLesson)?.PairCabinetId != null
                    ? pairCabinetComboBox.Items.IndexOf(cabinets.First(x => x.CabinetId == scheduleLessons.First(y => y.NumberLesson == numberLesson).PairCabinetId))
                    : -1;

                isBoldCheckBox.IsChecked = scheduleLessons.FirstOrDefault(x => x.NumberLesson == numberLesson)?.IsBold ?? false;
            }
        }

        private void RecalculateDifficulty()
        {
            using DatabaseContext context = new();

            CurrentDifficulty = 0;

            for (int numberLesson = 1; numberLesson <= 8; ++numberLesson)
            {
                ComboBox lessonComboBox = (FindName($"Lesson_Lesson{numberLesson}_ComboBox") as ComboBox)!;

                CurrentDifficulty += (lessonComboBox.SelectedItem as ClassLesson)?.Difficulty ?? 0;
            }

            StatusDifficultyTextBox.Text = $"Сложность: {CurrentDifficulty}/{MaxDifficultyClass}";

            if (CurrentDifficulty > MaxDifficultyClass)
            {
                StatusDifficultyTextBox.Foreground = Brushes.Red;
            }
            else
            {
                StatusDifficultyTextBox.Foreground = Brushes.Black;
            }
        }

        private bool ValidCheckBusynessTeachers()
        {
            using DatabaseContext context = new();

            for (int numberLesson = 1; numberLesson <= 8; ++numberLesson)
            {
                ComboBox lessonComboBox = (FindName($"Lesson_Lesson{numberLesson}_ComboBox") as ComboBox)!;
                ClassLesson classLesson = (lessonComboBox.SelectedItem as ClassLesson)!;

                int? teacherId = classLesson?.TeacherId;
                int? pairTeacherId = classLesson?.PairClassLesson?.TeacherId;

                if (classLesson != null)
                {
                    if (teacherId != null)
                    {
                        ScheduleLesson? busyScheduleLesson = context.ScheduleLessons
                            .Include(x => x.ClassLesson)
                            .Include(x => x.ClassLesson.Class)
                            .Include(x => x.ClassLesson.Lesson)
                            .Include(x => x.ClassLesson.Teacher)
                            .Where(x => x.Date == Date)
                            .Where(x => x.NumberLesson == numberLesson)
                            .Where(x => x.ClassLessonId != classLesson.ClassLessonId)
                            .Where(x => x.ClassLesson.TeacherId == teacherId)
                            .FirstOrDefault();

                        Teacher? teacher = teacherId != null ? context.Teachers.FirstOrDefault(x => x.TeacherId == teacherId) : null;

                        if (busyScheduleLesson != null)
                        {
                            Message.Message_BusyTeacher(numberLesson, classLesson, teacher!, busyScheduleLesson.ClassLesson);
                            return false;
                        }
                    }

                    if (pairTeacherId != null)
                    {
                        ScheduleLesson? busyScheduleLesson = context.ScheduleLessons
                            .Include(x => x.ClassLesson)
                            .Include(x => x.ClassLesson.PairClassLesson)
                            .Include(x => x.ClassLesson.PairClassLesson.Class)
                            .Include(x => x.ClassLesson.PairClassLesson.Lesson)
                            .Include(x => x.ClassLesson.PairClassLesson.Teacher)
                            .Where(x => x.Date == Date)
                            .Where(x => x.NumberLesson == numberLesson)
                            .Where(x => x.ClassLessonId != classLesson.ClassLessonId)
                            .Where(x => x.ClassLesson.PairClassLesson!.TeacherId == pairTeacherId)
                            .FirstOrDefault();

                        Teacher? pairTeacher = pairTeacherId != null ? context.Teachers.FirstOrDefault(x => x.TeacherId == pairTeacherId) : null;

                        if (busyScheduleLesson != null)
                        {
                            Message.Message_BusyTeacher(numberLesson, classLesson, pairTeacher!, busyScheduleLesson.ClassLesson!.PairClassLesson!);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool ValidCheckBusynessCabinets()
        {
            using DatabaseContext context = new();

            for (int numberLesson = 1; numberLesson <= 8; ++numberLesson)
            {
                ComboBox lessonComboBox = (FindName($"Lesson_Lesson{numberLesson}_ComboBox") as ComboBox)!;
                ComboBox cabinetComboBox = (FindName($"Cabinet_Lesson{numberLesson}_ComboBox") as ComboBox)!;
                ComboBox pairCabinetComboBox = (FindName($"PairCabinet_Lesson{numberLesson}_ComboBox") as ComboBox)!;

                ClassLesson classLesson = (lessonComboBox.SelectedItem as ClassLesson)!;
                int? cabinetId = (cabinetComboBox.SelectedItem as Cabinet)?.CabinetId;
                int? pairCabinetId = (pairCabinetComboBox.SelectedItem as Cabinet)?.CabinetId;

                if (cabinetId != null)
                {
                    ScheduleLesson? busyScheduleLesson = context.ScheduleLessons
                        .Include(x => x.ClassLesson)
                        .Include(x => x.ClassLesson.Class)
                        .Include(x => x.ClassLesson.Lesson)
                        .Include(x => x.Cabinet)
                        .Include(x => x.PairCabinet)
                        .Where(x => x.Date == Date)
                        .Where(x => x.NumberLesson == numberLesson)
                        .Where(x => x.ClassLessonId != classLesson.ClassLessonId)
                        .Where(x => x.CabinetId == cabinetId || x.PairCabinetId == cabinetId)
                        .FirstOrDefault();

                    Cabinet? cabinet = cabinetId != null ? context.Cabinets.FirstOrDefault(x => x.CabinetId == cabinetId) : null;

                    if (busyScheduleLesson != null && Message.Action_BusyCabinet(numberLesson, busyScheduleLesson.ClassLesson, cabinet!, busyScheduleLesson.ClassLesson) == MessageBoxResult.No)
                        return false;
                }

                if (pairCabinetId != null)
                {
                    ScheduleLesson? busyScheduleLesson = context.ScheduleLessons
                        .Include(x => x.ClassLesson)
                        .Include(x => x.ClassLesson.Class)
                        .Include(x => x.ClassLesson.Lesson)
                        .Include(x => x.Cabinet)
                        .Include(x => x.PairCabinet)
                        .Where(x => x.Date == Date)
                        .Where(x => x.NumberLesson == numberLesson)
                        .Where(x => x.CabinetId == pairCabinetId || x.PairCabinetId == pairCabinetId)
                        .FirstOrDefault();

                    Cabinet? pairCabinet = pairCabinetId != null ? context.Cabinets.FirstOrDefault(x => x.CabinetId == cabinetId) : null;

                    if (busyScheduleLesson != null && Message.Action_BusyCabinet(numberLesson, busyScheduleLesson.ClassLesson, pairCabinet!, busyScheduleLesson.ClassLesson) == MessageBoxResult.No)
                        return false;
                }
            }

            return true;
        }

        #region Buttons

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentDifficulty > MaxDifficultyClass)
                if (Message.Action_SaveChangeScheduleWithIncreasedDifficulty() == MessageBoxResult.No)
                    return;

            using DatabaseContext context = new();

            if (!ValidCheckBusynessTeachers() || !ValidCheckBusynessCabinets()) return;

            for (int numberLesson = 1; numberLesson <= 8; ++numberLesson)
            {

                ScheduleLesson? scheduleLesson = context.ScheduleLessons
                    .Include(x => x.ClassLesson)
                    .FirstOrDefault(x => x.Date == Date && x.NumberLesson == numberLesson && x.ClassLesson.ClassId == ClassId);

                ComboBox lessonComboBox = (FindName($"Lesson_Lesson{numberLesson}_ComboBox") as ComboBox)!;
                ComboBox cabinetComboBox = (FindName($"Cabinet_Lesson{numberLesson}_ComboBox") as ComboBox)!;
                ComboBox pairCabinetComboBox = (FindName($"PairCabinet_Lesson{numberLesson}_ComboBox") as ComboBox)!;
                CheckBox isBoldCheckBox = (FindName($"IsBold_Lesson{numberLesson}_CheckBox") as CheckBox)!;

                int? classLessonId = (lessonComboBox.SelectedItem as ClassLesson)?.ClassLessonId;
                int? cabinetId = (cabinetComboBox.SelectedItem as Cabinet)?.CabinetId;
                int? pairCabinetId = (pairCabinetComboBox.SelectedItem as Cabinet)?.CabinetId;
                bool isBold = isBoldCheckBox.IsChecked ?? false;

                if (scheduleLesson != null)
                {
                    if (classLessonId != null)
                    {
                        if (scheduleLesson.ClassLessonId == classLessonId)
                        {
                            scheduleLesson.CabinetId = cabinetId;
                            scheduleLesson.PairCabinetId = pairCabinetId;
                            scheduleLesson.IsBold = isBold;
                        }
                        else
                        {
                            context.ScheduleLessons.Remove(scheduleLesson);
                            context.ScheduleLessons.Add(new ScheduleLesson
                            {
                                ClassLessonId = classLessonId.Value,
                                Date = Date,
                                NumberLesson = numberLesson,
                                CabinetId = cabinetId,
                                PairCabinetId = pairCabinetId,
                                IsBold = isBold
                            });
                        }
                    }
                    else
                    {
                        context.ScheduleLessons.Remove(scheduleLesson);
                    }
                }
                else
                {
                    if (classLessonId != null)
                    {
                        context.ScheduleLessons.Add(new ScheduleLesson
                        {
                            Date = Date,
                            NumberLesson = numberLesson,
                            ClassLessonId = classLessonId!.Value,
                            CabinetId = cabinetId,
                            PairCabinetId = pairCabinetId,
                            IsBold = isBold,
                        });
                    }
                }

                context.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion

        #region Lesson_ComboBoxes

        private void Lesson_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox lessonComboBox = (sender as ComboBox)!;

            ComboBox cabinetComboBox = (FindName(lessonComboBox.Name.Replace("Lesson_", "Cabinet_")) as ComboBox)!;
            ComboBox pairCabinetComboBox = (FindName(lessonComboBox.Name.Replace("Lesson_", "PairCabinet_")) as ComboBox)!;

            Button cabinetComboBoxClearButton = (FindName(cabinetComboBox.Name + "_ClearButton") as Button)!;
            Button pairCabinetComboBoxClearButton = (FindName(pairCabinetComboBox.Name + "_ClearButton") as Button)!;

            CheckBox isBoldCheckBox = (FindName(lessonComboBox.Name.Replace("Lesson_", "IsBold_").Replace("_ComboBox", "_CheckBox")) as CheckBox)!;

            if (lessonComboBox.SelectedIndex == -1)
            {
                cabinetComboBox.SelectedIndex = -1;
                pairCabinetComboBox.SelectedIndex = -1;
                isBoldCheckBox.IsChecked = false;

                cabinetComboBox.IsEnabled = false;
                pairCabinetComboBox.IsEnabled = false;
                isBoldCheckBox.IsEnabled = false;

                cabinetComboBoxClearButton.IsEnabled = false;
                pairCabinetComboBoxClearButton.IsEnabled = false;
            }
            else
            {
                cabinetComboBox.IsEnabled = true;
                pairCabinetComboBox.IsEnabled = true;
                isBoldCheckBox.IsEnabled = true;

                cabinetComboBoxClearButton.IsEnabled = true;
                pairCabinetComboBoxClearButton.IsEnabled = true;

                ClassLesson classLesson = (lessonComboBox.SelectedItem as ClassLesson)!;

                cabinetComboBox.SelectedIndex = classLesson.DefaultCabinetId != null
                    ? cabinetComboBox.Items.IndexOf(cabinetComboBox.ItemsSource.Cast<Cabinet>().First(x => x.CabinetId == classLesson.DefaultCabinetId))
                    : -1;

                pairCabinetComboBox.SelectedIndex = classLesson.PairClassLessonId != null
                    ? pairCabinetComboBox.Items.IndexOf(pairCabinetComboBox.ItemsSource.Cast<Cabinet>().First(x => x.CabinetId == classLesson.PairClassLesson!.DefaultCabinetId))
                    : -1;
            }

            RecalculateDifficulty();
        }

        #endregion

        private void ComboBox_ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (sender as Button)!;

            ComboBox comboBox = (FindName(button.Name.Replace("_ClearButton", "")) as ComboBox)!;
            comboBox.SelectedIndex = -1;
        }
    }
}
