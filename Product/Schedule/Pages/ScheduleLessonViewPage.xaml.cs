using Microsoft.EntityFrameworkCore;
using Schedule.Database;
using Schedule.Services;
using Schedule.ViewItemSources;
using System;
using System.Collections;
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

namespace Schedule.Pages
{
    /// <summary>
    /// Логика взаимодействия для ScheduleLessonViewPage.xaml
    /// </summary>
    public partial class ScheduleLessonViewPage : Page
    {
        public class WeekScheduleNumberLessonClass
        {
            public WeekScheduleNumberLessonClass()
            {

            }
            public WeekScheduleNumberLessonClass(ClassViewItemSource _class, DateOnly dateStart)
            {
                Class = _class;
                DateStart = dateStart;
            }

            public DateOnly DateStart { get; set; } = default!;
            public ClassViewItemSource Class { get; set; } = null!;

            public ScheduleLessonViewItemSource? Monday { get; set; } = new();
            public ScheduleLessonViewItemSource? Tuesday { get; set; } = new();
            public ScheduleLessonViewItemSource? Wednesday { get; set; } = new();
            public ScheduleLessonViewItemSource? Thursday { get; set; } = new();
            public ScheduleLessonViewItemSource? Friday { get; set; } = new();
            public ScheduleLessonViewItemSource? Saturday { get; set; } = new();
            public ScheduleLessonViewItemSource? Sunday { get; set; } = new();

            public ScheduleLessonViewItemSource? this[int dayOfWeek]
            {
                get
                {
                    return dayOfWeek switch
                    {
                        0 => Sunday,
                        1 => Monday,
                        2 => Tuesday,
                        3 => Wednesday,
                        4 => Thursday,
                        5 => Friday,
                        6 => Saturday,
                        _ => null!
                    };
                }
                set
                {
                    switch (dayOfWeek)
                    {
                        case 0: Sunday = value; break;
                        case 1: Monday = value; break;
                        case 2: Tuesday = value; break;
                        case 3: Wednesday = value; break;
                        case 4: Thursday = value; break;
                        case 5: Friday = value; break;
                        case 6: Saturday = value; break;
                    }
                }
            }
        }

        public class WeekScheduleLessonsClass
        {
            public WeekScheduleLessonsClass(ClassViewItemSource _class, DateOnly dateStart)
            {
                Class = _class;
                DateStart = dateStart;

                FirstLesson = new(_class, dateStart);
                SecondLesson = new(_class, dateStart);
                ThirdLesson = new(_class, dateStart);
                FourthLesson = new(_class, dateStart);
                FifthLesson = new(_class, dateStart);
                SixthLesson = new(_class, dateStart);
                SeventhLesson = new(_class, dateStart);
                EighthLesson = new(_class, dateStart);
            }

            public DateOnly DateStart { get; set; } = default!;
            public DateOnly DateEnd => DateStart.AddDays(6);
            public ClassViewItemSource Class { get; set; } = null!;

            public WeekScheduleNumberLessonClass? FirstLesson { get; set; } = null!;
            public WeekScheduleNumberLessonClass? SecondLesson { get; set; } = null!;
            public WeekScheduleNumberLessonClass? ThirdLesson { get; set; } = null!;
            public WeekScheduleNumberLessonClass? FourthLesson { get; set; } = null!;
            public WeekScheduleNumberLessonClass? FifthLesson { get; set; } = null!;
            public WeekScheduleNumberLessonClass? SixthLesson { get; set; } = null!;
            public WeekScheduleNumberLessonClass? SeventhLesson { get; set; } = null!;
            public WeekScheduleNumberLessonClass? EighthLesson { get; set; } = null!;

            public WeekScheduleNumberLessonClass? this[int numberLesson]
            {
                get
                {
                    return numberLesson switch
                    {
                        1 => FirstLesson,
                        2 => SecondLesson,
                        3 => ThirdLesson,
                        4 => FourthLesson,
                        5 => FifthLesson,
                        6 => SixthLesson,
                        7 => SeventhLesson,
                        8 => EighthLesson,
                        _ => null!
                    };
                }
                set
                {
                    switch (numberLesson)
                    {
                        case 1: FirstLesson = value; break;
                        case 2: SecondLesson = value; break;
                        case 3: ThirdLesson = value; break;
                        case 4: FourthLesson = value; break;
                        case 5: FifthLesson = value; break;
                        case 6: SixthLesson = value; break;
                        case 7: SeventhLesson = value; break;
                        case 8: EighthLesson = value; break;
                    }
                }
            }

            public IEnumerable<WeekScheduleNumberLessonClass> LessonList => new List<WeekScheduleNumberLessonClass>()
            {
                FirstLesson!,
                SecondLesson!,
                ThirdLesson!,
                FourthLesson!,
                FifthLesson!,
                SixthLesson!,
                SeventhLesson!,
                EighthLesson!,
            };
        }

        public class WeekScheduleLessonsClasses : IEnumerable
        {
            public WeekScheduleLessonsClasses()
            {

            }
            public WeekScheduleLessonsClasses(DateOnly dateStart)
            {
                DateStart = dateStart;
            }

            public DateOnly DateStart { get; set; } = default!;
            public DateOnly DateEnd => DateStart.AddDays(6);

            public List<WeekScheduleLessonsClass> Data { get; private set; } = new();

            public WeekScheduleLessonsClass? this[int classId]
            {
                get => Data.FirstOrDefault(x => x.Class.ClassId == classId);
                set
                {
                    if (Data.Any(x => x.Class.ClassId == classId))
                        Data.RemoveAll(x => x.Class.ClassId == classId);

                    if (value != null) Data.Add(value);
                }
            }

            public IEnumerator GetEnumerator() => Data.GetEnumerator();
        }

        public ScheduleLessonViewPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)!.WindowTitle = "Расписание - Расписание занятий";

            WeekScheduleLessonsClassesDatePicker.SelectedDate = DateTime.Today.Date;
        }

        private static DateOnly GetStartWeek(DateOnly date)
        {
            DayOfWeek day = date.DayOfWeek;

            while (day != DayOfWeek.Monday)
            {
                date = date.AddDays(-1);
                day = date.DayOfWeek;
            }

            return date;
        }

        private async void UpdateScheduleItemsControlAsync(CancellationToken cancellationToken = default!)
        {
            using DatabaseContext context = new();

            IQueryable<Class> classes = context.Classes;
            IQueryable<ScheduleLesson> scheduleLessons = await Task.Run(() =>
                context.ScheduleLessons
                    .Include(scheduleLesson => scheduleLesson.ClassLesson)
                    .Include(scheduleLesson => scheduleLesson.ClassLesson.Lesson)
                    .Include(scheduleLesson => scheduleLesson.ClassLesson.DefaultCabinet)
                    .Include(scheduleLesson => scheduleLesson.ClassLesson.PairClassLesson)
                    .Include(scheduleLesson => scheduleLesson.ClassLesson.PairClassLesson!.Lesson)
                    .Include(scheduleLesson => scheduleLesson.ClassLesson.PairClassLesson!.DefaultCabinet)
                    .Include(scheduleLesson => scheduleLesson.Cabinet)
                , cancellationToken);

            WeekScheduleLessonsClasses weekLessonsClasses = null!;
            DateTime? dateValue = WeekScheduleLessonsClassesDatePicker.SelectedDate;

            if (dateValue != null)
            {
                DateOnly dateStart = new(dateValue?.Year ?? 0, dateValue?.Month ?? 0, dateValue?.Day ?? 0);
                dateStart = GetStartWeek(dateStart);

                weekLessonsClasses = new(dateStart);
                DateTime dateStartDateTime = new(dateStart.Year, dateStart.Month, dateStart.Day);

                await Task.Run(() =>
                {
                    foreach (Class classItem in classes)
                    {
                        ClassViewItemSource classViewItemSource = new(classItem);

                        IQueryable<ScheduleLesson> scheduleLessonsClass = scheduleLessons
                            .Where(scheduleLesson => scheduleLesson.ClassLesson.ClassId == classItem.ClassId)
                            .Where(scheduleLesson => scheduleLesson.Date >= dateStartDateTime)
                            .Where(scheduleLesson => scheduleLesson.Date <= dateStartDateTime.AddDays(6));

                        WeekScheduleLessonsClass weekScheduleLessonsClass = new(classViewItemSource, dateStart);

                        for (int numberLesson = 1; numberLesson <= 7; ++numberLesson)
                        {
                            WeekScheduleNumberLessonClass weekScheduleNumberLessonClass = new(classViewItemSource, dateStart);

                            for (int day = 0; day < 7; ++day)
                            {
                                ScheduleLesson? scheduleLesson = scheduleLessonsClass
                                    .Where(scheduleLesson => scheduleLesson.Date == dateStartDateTime.AddDays(day))
                                    .FirstOrDefault(scheduleLesson => scheduleLesson.NumberLesson == numberLesson);

                                if (scheduleLesson != null) weekScheduleNumberLessonClass[day + 1] = new(scheduleLesson);
                            }

                            weekScheduleLessonsClass[numberLesson] = weekScheduleNumberLessonClass;
                        }

                        weekLessonsClasses[classItem.ClassId] = weekScheduleLessonsClass;
                    }
                }, cancellationToken);

                ScheduleItemsControl.ItemsSource = weekLessonsClasses;
            }
        }

        private void UpdateDayOfWeekTextBlocks()
        {
            DateTime? dateValue = WeekScheduleLessonsClassesDatePicker.SelectedDate;

            if (dateValue != null)
            {
                DateOnly dateStart = new(dateValue?.Year ?? 0, dateValue?.Month ?? 0, dateValue?.Day ?? 0);
                dateStart = GetStartWeek(dateStart);

                MondayTextBlock.Text = $"{dateStart: dd.MM.yyyy} - Пн.";
                TuesdayTextBlock.Text = $"{dateStart.AddDays(1): dd.MM.yyyy} - Вт.";
                WednesdayTextBlock.Text = $"{dateStart.AddDays(2): dd.MM.yyyy} - Ср.";
                ThursdayTextBlock.Text = $"{dateStart.AddDays(3): dd.MM.yyyy} - Чт.";
                FridayTextBlock.Text = $"{dateStart.AddDays(4): dd.MM.yyyy} - Пт.";
                SaturdayTextBlock.Text = $"{dateStart.AddDays(5): dd.MM.yyyy} - Сб.";
            }
            else
            {
                MondayTextBlock.Text = "Пн.";
                TuesdayTextBlock.Text = "Вт.";
                WednesdayTextBlock.Text = "Ср.";
                ThursdayTextBlock.Text = "Чт.";
                FridayTextBlock.Text = "Пт.";
                SaturdayTextBlock.Text = "Сб.";
            }
        }

        private void WeekScheduleLessonsClassesDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateScheduleItemsControlAsync();
            UpdateDayOfWeekTextBlocks();
        }

        private void ScheduleTextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = (sender as TextBlock)!;

            if (textBlock != null && textBlock.BindingGroup.Items.Count > 0)
            {
                WeekScheduleNumberLessonClass weekLessonsClass =
                    (textBlock.DataContext as WeekScheduleNumberLessonClass)!;

                if (weekLessonsClass != null)
                {
                    int dayOfWeek = (int)Service.ConvertStringToDayOfWeek(textBlock.Name.Split('_')[0])!;
                    int classId = weekLessonsClass.Class.ClassId;
                    DateOnly date = weekLessonsClass.DateStart.AddDays(dayOfWeek - 1);
                    DateTime dateDateTime = new(date.Year, date.Month, date.Day);

                    Windows.ScheduleLessonChangeWindow changeWindow = new(dateDateTime, classId);

                    if (changeWindow.ShowDialog() == true)
                    {
                        UpdateScheduleItemsControlAsync();
                    }
                }
            }
        }
    }
}
