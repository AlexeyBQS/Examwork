using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Schedule.Database;
using Schedule.Services;
using Schedule.ViewItemSources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
    /// Логика взаимодействия для TeacherViewPage.xaml
    /// </summary>
    public partial class TeacherViewPage : Page
    {
        public TeacherViewPage()
        {
            InitializeComponent();
        }

        private int CountViewRecord { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)!.WindowTitle = "Расписание - Педагоги";

            Service.FillViewCountRecordComboBox(ViewCountRecordComboBox);
            ScheduleLessonsTeacherDatePicker.SelectedDate = DateTime.Today.Date;

            UpdateDataListBox();
            UpdateViewTabControl();
        }

        #region [Updaters]

        private void UpdateDataListBox()
        {
            using (DatabaseContext context = new())
            {
                IQueryable<Teacher> teachers = context.Teachers;

                if (TeacherIdFilterTextBox.Text != null)
                {
                    string teacherId = TeacherIdFilterTextBox.Text;
                    teachers = teachers.Where(teacher => teacher.TeacherId.ToString().Contains(teacherId));
                }

                if (SurnameFilterTextBox.Text != null)
                {
                    string surname = SurnameFilterTextBox.Text;
                    teachers = teachers.Where(teacher => teacher.Surname!.Contains(surname));
                }

                if (NameFilterTextBox.Text != null)
                {
                    string name = NameFilterTextBox.Text;
                    teachers = teachers.Where(teacher => teacher.Name!.Contains(name));
                }

                if (PatronymicFilterTextBox.Text != null)
                {
                    string patronymic = PatronymicFilterTextBox.Text;
                    teachers = teachers.Where(teacher => teacher.Patronymic!.Contains(patronymic));
                }

                if (BirthdayBeginFilterDatePicker.SelectedDate != null)
                {
                    DateTime birthdayBegin = (DateTime)BirthdayBeginFilterDatePicker.SelectedDate;
                    teachers = teachers.Where(teacher => teacher.Birthday >= birthdayBegin);
                }

                if (BirthdayEndFilterDatePicker.SelectedDate != null)
                {
                    DateTime birthdayEnd = (DateTime)BirthdayEndFilterDatePicker.SelectedDate;
                    teachers = teachers.Where(teacher => teacher.Birthday <= birthdayEnd);
                }

                if (PassportFilterTextBox.Text != null)
                {
                    string passport = PassportFilterTextBox.Text;
                    teachers = teachers.Where(teacher => teacher.Passport!.Contains(passport));
                }

                if (PhoneNumberFilterTextBox.Text != null)
                {
                    string phoneNumber = PhoneNumberFilterTextBox.Text;
                    teachers = teachers.Where(teacher => teacher.PhoneNumber!.Contains(phoneNumber));
                }

                if (EducationFilterTextBox.Text != null)
                {
                    string education = EducationFilterTextBox.Text;
                    teachers = teachers.Where(teacher => teacher.Education!.Contains(education));
                }

                IEnumerable<TeacherViewItemSource> viewItemSources = CountViewRecord > 0
                    ? teachers.ToList().Take(CountViewRecord).Select(x => new TeacherViewItemSource(x))
                    : teachers.ToList().Select(x => new TeacherViewItemSource(x));

                TeacherListBox.ItemsSource = viewItemSources;

                StatusTextBlock.Text = $"Всего: {context.Teachers.Count()} | Всего с фильтрами: {teachers.Count()} | Отображается: {viewItemSources.Count()}";
            }
        }

        private void DefaultViewTabControl()
        {
            TeacherIdTextBox.Text = null!;
            SurnameTextBox.Text = null!;
            NameTextBox.Text = null!;
            PatronymicTextBox.Text = null!;
            BirthdayDatePicker.Text = null!;
            PassportTextBox.Text = null!;
            PhoneNumberTextBox.Text = null!;
            PhotoBorder.Background = new ImageBrush(Service.DefaultPhoto);
            EducationTextBox.Text = null!;

            SaveTeacherPhotoButton.IsEnabled = false;
            ChangeTeacherPhotoButton.IsEnabled = false;
            DeleteTeacherPhotoButton.IsEnabled = false;

            SaveChangeTeacherButton.IsEnabled = false;
            DeleteTeacherButton.IsEnabled = false;

            TeacherIdTextBox.IsEnabled = false;
            SurnameTextBox.IsEnabled = false;
            NameTextBox.IsEnabled = false;
            PatronymicTextBox.IsEnabled = false;
            BirthdayDatePicker.IsEnabled = false;
            PassportTextBox.IsEnabled = false;
            PhoneNumberTextBox.IsEnabled = false;
            EducationTextBox.IsEnabled = false;
        }

        private void UpdateViewTabControl()
        {
            if (TeacherListBox.SelectedItems.Count > 0)
            {
                TeacherViewItemSource teacher = TeacherListBox.SelectedItems[0] as TeacherViewItemSource ?? null!;

                if (teacher != null)
                {
                    TeacherIdTextBox.Text = teacher.TeacherId.ToString();
                    SurnameTextBox.Text = teacher.Surname;
                    NameTextBox.Text = teacher.Name;
                    PatronymicTextBox.Text = teacher.Patronymic;
                    BirthdayDatePicker.Text = teacher.Birthday?.ToShortDateString();
                    PassportTextBox.Text = teacher.Passport;
                    PhoneNumberTextBox.Text = teacher.PhoneNumber;
                    PhotoBorder.Background = new ImageBrush(teacher.Photo_Image);
                    EducationTextBox.Text = teacher.Education;

                    SaveChangeTeacherButton.IsEnabled = false;
                    DeleteTeacherButton.IsEnabled = true;

                    ChangeTeacherPhotoButton.IsEnabled = true;

                    if (teacher.Photo == null || teacher.Photo?.Length == 0)
                    {
                        SaveTeacherPhotoButton.IsEnabled = false;
                        DeleteTeacherPhotoButton.IsEnabled = false;
                    }
                    else
                    {
                        SaveTeacherPhotoButton.IsEnabled = true;
                        DeleteTeacherPhotoButton.IsEnabled = true;
                    }

                    TeacherIdTextBox.IsEnabled = true;
                    SurnameTextBox.IsEnabled = true;
                    NameTextBox.IsEnabled = true;
                    PatronymicTextBox.IsEnabled = true;
                    BirthdayDatePicker.IsEnabled = true;
                    PassportTextBox.IsEnabled = true;
                    PhoneNumberTextBox.IsEnabled = true;
                    EducationTextBox.IsEnabled = true;
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

        private void TeacherListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateViewTabControl();
            UpdateBusynessTabItem();
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
            SaveChangeTeacherButton.IsEnabled = true;
        }

        private void ViewDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveChangeTeacherButton.IsEnabled = true;
        }

        #region [WorkWithPhoto]

        private void SaveTeacherPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (TeacherListBox.SelectedItems.Count > 0)
            {
                int? teacherId = (TeacherListBox.SelectedItems[0] as TeacherViewItemSource)?.TeacherId ?? null!;

                if (teacherId != null)
                {
                    using (DatabaseContext context = new())
                    {
                        Teacher? teacher = context.Teachers.FirstOrDefault(x => x.TeacherId == (int)teacherId);

                        if (teacher != null)
                        {
                            if (teacher.Photo != null && teacher.Photo?.Length != 0)
                            {
                                BitmapImage image = Service.ConvertByteArrayToBitmapImage(teacher.Photo ?? null!);

                                SaveFileDialog dialog = new();
                                dialog.FileName = $"Педагог_{TeacherIdTextBox.Text ?? "0"}.png";
                                dialog.Filter = "All Files (*.*)|*.*";
                                dialog.FilterIndex = 0;

                                if (dialog.ShowDialog() == true)
                                {
                                    string pathFile = $"{dialog.InitialDirectory}\\{dialog.FileName}".Remove(0, 1);

                                    BitmapEncoder encoder = new PngBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(image));

                                    using FileStream fileStream = new(pathFile, FileMode.Create);

                                    encoder.Save(fileStream);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ChangeTeacherPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new();
            dialog.Filter = "Image Files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png;";
            dialog.FilterIndex = 0;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                string pathFile = $"{dialog.InitialDirectory}\\{dialog.FileName}".Remove(0, 1);
                PhotoBorder.Background = new ImageBrush(new BitmapImage(new Uri(pathFile)));

                SaveTeacherPhotoButton.IsEnabled = true;
                DeleteTeacherPhotoButton.IsEnabled = true;

                SaveChangeTeacherButton.IsEnabled = true;
            }
        }

        private void DeleteTeacherPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoBorder.Background = new ImageBrush(Service.DefaultPhoto);

            SaveTeacherPhotoButton.IsEnabled = false;
            DeleteTeacherPhotoButton.IsEnabled = false;

            SaveChangeTeacherButton.IsEnabled = true;
        }

        #endregion

        #endregion

        #region [WorkWithRecord]

        private void AddTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            using (DatabaseContext context = new())
            {
                context.Teachers.Add(new Teacher());
                context.SaveChangesAsync();
            }

            UpdateDataListBox();

            TeacherListBox.SelectedIndex = TeacherListBox.Items.Count - 1;
        }

        private void SaveChangeTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            if (TeacherListBox.SelectedItems.Count > 0)
            {
                int? teacherId = (TeacherListBox.SelectedItems[0] as TeacherViewItemSource)?.TeacherId ?? null!;

                if (teacherId != null)
                {
                    using (DatabaseContext context = new())
                    {
                        Teacher? teacher = context.Teachers.FirstOrDefault(x => x.TeacherId == (int)teacherId);

                        if (teacher != null)
                        {
                            ImageSource photo = (PhotoBorder.Background as ImageBrush)!.ImageSource;
                            byte[]? photoByteArray =
                                Service.ConvertImageToByteArray(
                                Service.ResizeImage(
                                Service.ConvertImageSourceToBitmap(photo)));

                            teacher.Surname = SurnameTextBox.Text;
                            teacher.Name = NameTextBox.Text;
                            teacher.Patronymic = PatronymicTextBox.Text;
                            teacher.Birthday = Service.GetOnlyDate(BirthdayDatePicker.SelectedDate ?? null!);
                            teacher.Passport = PassportTextBox.Text;
                            teacher.PhoneNumber = PhoneNumberTextBox.Text;
                            teacher.Photo = photoByteArray;
                            teacher.Education = EducationTextBox.Text;
                        }

                        context.SaveChangesAsync();
                    }

                    UpdateDataListBox();
                }
            }
        }

        private void DeleteTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            if (TeacherListBox.SelectedItems.Count > 0)
            {
                if (Message.Action_DeleteRecord() == MessageBoxResult.Yes)
                {
                    int? teacherId = (TeacherListBox.SelectedItems[0] as TeacherViewItemSource)?.TeacherId ?? -1;

                    using (DatabaseContext context = new())
                    {
                        Teacher? teacher = context.Teachers.FirstOrDefault(x => x.TeacherId == (int)teacherId);

                        if (teacher != null)
                        {
                            context.Teachers.Remove(teacher);

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

        private void BirthdayBeginFilterDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BirthdayEndFilterDatePicker.SelectedDate != null)
                if (BirthdayBeginFilterDatePicker.SelectedDate > BirthdayEndFilterDatePicker.SelectedDate)
                    BirthdayBeginFilterDatePicker.SelectedDate = BirthdayEndFilterDatePicker.SelectedDate;

            UpdateDataListBox();
        }

        private void BirthdayEndFilterDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BirthdayBeginFilterDatePicker.SelectedDate != null)
                if (BirthdayEndFilterDatePicker.SelectedDate < BirthdayBeginFilterDatePicker.SelectedDate)
                    BirthdayEndFilterDatePicker.SelectedDate = BirthdayBeginFilterDatePicker.SelectedDate;

            UpdateDataListBox();
        }

        private void ClearFiltresButton_Click(object sender, RoutedEventArgs e)
        {
            TeacherIdFilterTextBox.Text = null!;
            SurnameFilterTextBox.Text = null!;
            NameFilterTextBox.Text = null!;
            PatronymicFilterTextBox.Text = null!;
            BirthdayBeginFilterDatePicker.SelectedDate = null!;
            BirthdayEndFilterDatePicker.SelectedDate = null!;
            PassportFilterTextBox.Text = null!;
            PhoneNumberFilterTextBox = null!;
            EducationFilterTextBox.Text = null!;
        }

        #endregion

        #region Busyness

        private void UpdateBusynessTabItem()
        {
            TeacherViewItemSource? teacher = (TeacherListBox.SelectedItem as TeacherViewItemSource);
            DateTime? date = ScheduleLessonsTeacherDatePicker.SelectedDate;

            if (teacher != null && date != null)
            {
                using DatabaseContext context = new();

                IQueryable<ScheduleLesson> scheduleLessonsTeacher = context.ScheduleLessons
                    .Where(x => x.Date == date)
                    .Where(x => x.ClassLesson.TeacherId == teacher.TeacherId || x.ClassLesson.PairClassLesson!.TeacherId == teacher.TeacherId)
                    .Include(x => x.Cabinet)
                    .Include(x => x.ClassLesson)
                    .Include(x => x.ClassLesson.Class)
                    .Include(x => x.ClassLesson.Lesson)
                    .Include(x => x.PairCabinet)
                    .Include(x => x.ClassLesson.PairClassLesson)
                    .Include(x => x.ClassLesson.PairClassLesson!.Class)
                    .Include(x => x.ClassLesson.PairClassLesson!.Lesson);

                for (int numberLesson = 1; numberLesson <= 8; ++numberLesson)
                {
                    ScheduleLesson? scheduleLesson = scheduleLessonsTeacher.FirstOrDefault(x => x.NumberLesson == numberLesson);

                    CheckBox isAvailableCheckBox = (FindName($"Lesson{numberLesson}_IsAvailableCheckBox") as CheckBox)!;
                    TextBox classNameTextBox = (FindName($"Lesson{numberLesson}_ClassNameTextBox") as TextBox)!;
                    TextBox lessonNameTextBox = (FindName($"Lesson{numberLesson}_LessonNameTextBox") as TextBox)!;
                    TextBox cabinetNameTextBox = (FindName($"Lesson{numberLesson}_CabinetNameTextBox") as TextBox)!;

                    if (scheduleLesson != null)
                    {
                        if (scheduleLesson.ClassLesson.TeacherId == teacher.TeacherId)
                        {
                            isAvailableCheckBox.IsChecked = true;
                            classNameTextBox.Text = scheduleLesson.ClassLesson.Class!.Name;
                            lessonNameTextBox.Text = scheduleLesson.ClassLesson.ToShortString();
                            cabinetNameTextBox.Text = scheduleLesson.Cabinet!.Name;
                        }
                        else
                        {
                            isAvailableCheckBox.IsChecked = true;
                            classNameTextBox.Text = scheduleLesson.ClassLesson.PairClassLesson!.Class!.Name;
                            lessonNameTextBox.Text = scheduleLesson.ClassLesson.PairClassLesson!.ToShortString();
                            cabinetNameTextBox.Text = scheduleLesson.PairCabinet!.Name;
                        }
                    }
                    else
                    {
                        isAvailableCheckBox.IsChecked = false;
                        classNameTextBox.Text = null!;
                        lessonNameTextBox.Text = null!;
                        cabinetNameTextBox.Text = null!;
                    }
                }
            }
            else
            {
                for (int numberLesson = 1; numberLesson <= 8; ++numberLesson)
                {
                    (FindName($"Lesson{numberLesson}_IsAvailableCheckBox") as CheckBox)!.IsChecked = false;
                    (FindName($"Lesson{numberLesson}_ClassNameTextBox") as TextBox)!.Text = null!;
                    (FindName($"Lesson{numberLesson}_LessonNameTextBox") as TextBox)!.Text = null!;
                    (FindName($"Lesson{numberLesson}_CabinetNameTextBox") as TextBox)!.Text = null!;
                }
            }
        }

        private void ScheduleLessonsTeacherDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBusynessTabItem();
        }

        #endregion

        private void OnlyDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Service.OnlyDigit_PreviewTextInput(sender, e);
            base.OnPreviewTextInput(e);
        }

        private void OnlyPhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Service.OnlyPhoneNumber_PreviewTextInput(sender, e);
            base.OnPreviewTextInput(e);
        }
    }
}
