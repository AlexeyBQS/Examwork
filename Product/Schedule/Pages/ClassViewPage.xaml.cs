using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Schedule.Database;
using Schedule.Services;
using Schedule.ViewItemSources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Schedule.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClassViewPage.xaml
    /// </summary>
    public partial class ClassViewPage : Page
    {
        public ClassViewPage()
        {
            InitializeComponent();
        }

        private int CountViewRecord { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)!.WindowTitle = "Расписание - Классы";

            Service.FillViewCountRecordComboBox(ViewCountRecordComboBox);
            ScheduleLessonsClassDatePicker.SelectedDate = DateTime.Today.Date;

            UpdateDataComboBox();
            UpdateDataListBox();
            UpdateViewTabControl();
        }

        #region [Updaters]

        private void UpdateDataComboBox()
        {
            using DatabaseContext context = new();

            IEnumerable<Teacher> teachers = context.Teachers.ToList();

            TeacherIdComboBox.ItemsSource = teachers;
            TeacherIdFilterComboBox.ItemsSource = teachers;

            IEnumerable<Cabinet> cabinets = context.Cabinets.ToList();

            CabinetIdComboBox.ItemsSource = cabinets;
            CabinetIdFilterComboBox.ItemsSource = cabinets;
        }

        private void UpdateDataListBox()
        {
            using DatabaseContext context = new();

            IQueryable<Class> classes = context.Classes
                .Include(_class => _class.Teacher)
                .Include(_class => _class.Cabinet)
                .Include(_class => _class.Cabinet!.Teacher);

            if (ClassIdFilterTextBox.Text != null)
            {
                string classId = ClassIdFilterTextBox.Text;
                classes = classes.Where(_class => _class.ClassId.ToString().Contains(classId));
            }

            if (TeacherIdFilterComboBox.SelectedItem != null)
            {
                int teacherId = (TeacherIdFilterComboBox.SelectedItem as Teacher)!.TeacherId;
                classes = classes.Where(_class => _class.TeacherId == teacherId);
            }

            if (CabinetIdFilterComboBox.SelectedItem != null)
            {
                int cabinetId = (CabinetIdFilterComboBox.SelectedItem as Cabinet)!.CabinetId;
                classes = classes.Where(_class => _class.CabinetId == cabinetId);
            }

            if (NameFilterTextBox.Text != null!)
            {
                string name = NameFilterTextBox.Text;
                classes = classes.Where(_class => _class.Name!.Contains(name));
            }

            if (CountPupilsFilterTextBox.Text != null)
            {
                string countPupils = CountPupilsFilterTextBox.Text;
                classes = classes.Where(_class => _class.CountPupils.ToString()!.Contains(countPupils));
            }

            IEnumerable<ClassViewItemSource> viewItemSources = CountViewRecord > 0
                ? classes.ToList().Take(CountViewRecord).Select(x => new ClassViewItemSource(x))
                : classes.ToList().Select(x => new ClassViewItemSource(x));

            ClassListBox.ItemsSource = viewItemSources;

            StatusTextBlock.Text = $"Всего: {context.Classes.Count()} | Всего с фильтрами: {classes.Count()} | Отображается: {viewItemSources.Count()}";
        }

        private void DefaultViewTabControl()
        {
            ClassIdTextBox.Text = null!;
            TeacherIdComboBox.SelectedIndex = -1;
            CabinetIdComboBox.SelectedIndex = -1;
            NameTextBox.Text = null!;
            CountPupilsTextBox.Text = null!;
            MaxDifficultyTextBox.Text = null!;
            PhotoBorder.Background = new ImageBrush(Service.DefaultPhoto);

            SaveClassPhotoButton.IsEnabled = false;
            ChangeClassPhotoButton.IsEnabled = false;
            DeleteClassPhotoButton.IsEnabled = false;

            SaveChangeClassButton.IsEnabled = false;
            DeleteClassButton.IsEnabled = false;

            ClassIdTextBox.IsEnabled = false;
            TeacherIdComboBox.IsEnabled = false;
            CabinetIdComboBox.IsEnabled = false;
            NameTextBox.IsEnabled = false;
            CountPupilsTextBox.IsEnabled = false;
            MaxDifficultyTextBox.IsEnabled = false;

            TeacherIdComboBoxClearButton.IsEnabled = false;
            CabinetIdComboBoxClearButton.IsEnabled = false;
        }

        private void UpdateViewTabControl()
        {
            if (ClassListBox.SelectedItems.Count > 0)
            {
                ClassViewItemSource _class = ClassListBox.SelectedItems[0] as ClassViewItemSource ?? null!;

                if (_class != null)
                {
                    ClassIdTextBox.Text = _class.ClassId.ToString();

                    using (DatabaseContext context = new())
                    {
                        IEnumerable<Teacher> teachers = context.Teachers.ToList();

                        TeacherIdComboBox.ItemsSource = teachers;
                        TeacherIdFilterComboBox.ItemsSource = teachers;

                        TeacherIdComboBox.SelectedIndex = _class.TeacherId != null
                            ? TeacherIdComboBox.Items.IndexOf(context.Teachers.First(t => t.TeacherId == _class.TeacherId))
                            : -1;

                        IEnumerable<Cabinet> cabinets = context.Cabinets.ToList();

                        CabinetIdComboBox.ItemsSource = cabinets;
                        CabinetIdFilterComboBox.ItemsSource = cabinets;

                        CabinetIdComboBox.SelectedIndex = _class.CabinetId != null
                            ? CabinetIdComboBox.Items.IndexOf(context.Cabinets.First(c => c.CabinetId == _class.CabinetId))
                            : -1;
                    }

                    NameTextBox.Text = _class.Name;
                    CountPupilsTextBox.Text = _class.CountPupils.ToString();
                    MaxDifficultyTextBox.Text = _class.MaxDifficulty.ToString();
                    PhotoBorder.Background = new ImageBrush(_class.Photo_Image);

                    TeacherIdComboBoxClearButton.IsEnabled = true;
                    CabinetIdComboBoxClearButton.IsEnabled = true;

                    SaveChangeClassButton.IsEnabled = false;
                    DeleteClassButton.IsEnabled = true;

                    ChangeClassPhotoButton.IsEnabled = true;

                    if (_class.Photo == null! || _class.Photo?.Length == 0)
                    {
                        SaveClassPhotoButton.IsEnabled = false;
                        DeleteClassPhotoButton.IsEnabled = false;
                    }
                    else
                    {
                        SaveClassPhotoButton.IsEnabled = true;
                        DeleteClassPhotoButton.IsEnabled = true;
                    }

                    ClassIdTextBox.IsEnabled = true;
                    TeacherIdComboBox.IsEnabled = true;
                    CabinetIdComboBox.IsEnabled = true;
                    NameTextBox.IsEnabled = true;
                    CountPupilsTextBox.IsEnabled = true;
                    MaxDifficultyTextBox.IsEnabled = true;
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

        private void ClassListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            SaveChangeClassButton.IsEnabled = true;
        }

        private void ViewComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveChangeClassButton.IsEnabled = true;
        }

        #endregion

        #region [WorkWithRecord]

        private void AddClassButton_Click(object sender, RoutedEventArgs e)
        {
            using (DatabaseContext context = new())
            {
                context.Classes.Add(new Class());
                context.SaveChangesAsync();
            }

            UpdateDataListBox();

            ClassListBox.SelectedIndex = ClassListBox.Items.Count - 1;
        }

        private void SaveChangeClassButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClassListBox.SelectedItems.Count > 0)
            {
                int? classId = (ClassListBox.SelectedItems[0] as ClassViewItemSource)?.ClassId ?? null!;

                if (classId != null)
                {
                    using (DatabaseContext context = new())
                    {
                        Class? _class = context.Classes.FirstOrDefault(c => c.ClassId == (int)classId);

                        if (_class != null)
                        {
                            ImageSource photo = (PhotoBorder.Background as ImageBrush)!.ImageSource;
                            byte[]? photoByteArray =
                                Service.ConvertImageToByteArray(
                                Service.ResizeImage(
                                Service.ConvertImageSourceToBitmap(photo)));

                            _class.TeacherId = TeacherIdComboBox.SelectedIndex >= 0
                                ? (TeacherIdComboBox.Items[TeacherIdComboBox.SelectedIndex] as Teacher)!.TeacherId
                                : null!;

                            _class.CabinetId = CabinetIdComboBox.SelectedIndex >= 0
                                ? (CabinetIdComboBox.Items[CabinetIdComboBox.SelectedIndex] as Cabinet)!.CabinetId
                                : null!;

                            _class.Name = NameTextBox.Text;
                            _class.CountPupils = CountPupilsTextBox.Text != string.Empty ? int.Parse(CountPupilsTextBox.Text) : null!;
                            _class.MaxDifficulty = MaxDifficultyTextBox.Text != string.Empty ? int.Parse(MaxDifficultyTextBox.Text) : null!;
                            _class.Photo = photoByteArray;
                        }

                        context.SaveChangesAsync();
                    }

                    UpdateDataListBox();
                }
            }
        }

        private void DeleteClassButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClassListBox.SelectedItems.Count > 0)
            {
                if (Message.Action_DeleteRecord() == MessageBoxResult.Yes)
                {
                    int? classId = (ClassListBox.SelectedItems[0] as ClassViewItemSource)?.ClassId ?? -1;

                    using (DatabaseContext context = new())
                    {
                        Class? _class = context.Classes.FirstOrDefault(c => c.ClassId == (int)classId);

                        if (_class != null)
                        {
                            context.Classes.Remove(_class);

                            context.SaveChangesAsync();
                        }
                    }

                    UpdateDataListBox();
                }
            }
        }

        private void TeacherIdComboBoxClearButton_Click(object sender, RoutedEventArgs e)
        {
            TeacherIdComboBox.SelectedIndex = -1;
        }

        private void CabinetIdComboBoxClearButton_Click(object sender, RoutedEventArgs e)
        {
            CabinetIdComboBox.SelectedIndex = -1;
        }

        #region [WorkWithPhoto]

        private void SaveClassPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClassListBox.SelectedItems.Count > 0)
            {
                int? classId = (ClassListBox.SelectedItems[0] as ClassViewItemSource)?.ClassId ?? null!;

                if (classId != null)
                {
                    using DatabaseContext context = new();

                    Class? _class = context.Classes.FirstOrDefault(c => c.ClassId == (int)classId);

                    if (_class != null)
                    {
                        BitmapImage image = Service.ConvertByteArrayToBitmapImage(_class.Photo ?? null!);

                        SaveFileDialog dialog = new();
                        dialog.FileName = $"Класс_{ClassIdTextBox.Text ?? "0"}.png";
                        dialog.Filter = "All Files (*.*)|*.*";
                        dialog.FilterIndex = 0;

                        if (dialog.ShowDialog() == true)
                        {
                            string pathFile = $"{dialog.InitialDirectory}\\{dialog.FileName}".Remove(0, 1);

                            BitmapEncoder encoder = new PngBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(image));

                            using FileStream? fileStream = new(pathFile, FileMode.Create);

                            encoder.Save(fileStream);
                        }
                    }
                }
            }
        }

        private void ChangeClassPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new();
            dialog.Filter = "Image Files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png;";
            dialog.FilterIndex = 0;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                string pathFile = $"{dialog.InitialDirectory}\\{dialog.FileName}".Remove(0, 1);
                PhotoBorder.Background = new ImageBrush(new BitmapImage(new Uri(pathFile)));

                SaveClassPhotoButton.IsEnabled = true;
                DeleteClassPhotoButton.IsEnabled = true;

                SaveChangeClassButton.IsEnabled = true;
            }
        }

        private void DeleteClassPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoBorder.Background = new ImageBrush(Service.DefaultPhoto);

            SaveClassPhotoButton.IsEnabled = false;
            DeleteClassPhotoButton.IsEnabled = false;

            SaveChangeClassButton.IsEnabled = true;
        }

        #endregion

        #endregion

        #region [WorkWithFiltres]

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDataListBox();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDataListBox();
        }

        private void TeacherIdFilterComboBoxClearButton_Click(object sender, RoutedEventArgs e)
        {
            TeacherIdFilterComboBox.SelectedIndex = -1;
        }

        private void CabinetIdFilterComboBoxClearButton_Click(object sender, RoutedEventArgs e)
        {
            CabinetIdFilterComboBox.SelectedIndex = -1;
        }

        private void ClearFiltresButton_Click(object sender, RoutedEventArgs e)
        {
            ClassIdFilterTextBox.Text = null!;
            TeacherIdFilterComboBox.SelectedIndex = -1;
            CabinetIdFilterComboBox.SelectedIndex = -1;
            NameFilterTextBox.Text = null!;
            CountPupilsFilterTextBox.Text = null!;
        }

        #endregion

        #region Busyness

        private void UpdateBusynessTabItem()
        {
            ClassViewItemSource? _class = (ClassListBox.SelectedItem as ClassViewItemSource);
            DateTime? date = ScheduleLessonsClassDatePicker.SelectedDate;

            if (_class != null && date != null)
            {
                using DatabaseContext context = new();

                IQueryable<ScheduleLesson> scheduleLessonsClass = context.ScheduleLessons
                    .Where(x => x.Date == date)
                    .Include(x => x.ClassLesson)
                    .Include(x => x.ClassLesson.Class)
                    .Where(x => x.ClassLesson.Class!.ClassId == _class.ClassId)
                    .Include(x => x.Cabinet)
                    .Include(x => x.ClassLesson.Lesson)
                    .Include(x => x.ClassLesson.Teacher)
                    .Include(x => x.PairCabinet)
                    .Include(x => x.ClassLesson.PairClassLesson)
                    .Include(x => x.ClassLesson.PairClassLesson!.Class)
                    .Include(x => x.ClassLesson.PairClassLesson!.Lesson)
                    .Include(x => x.ClassLesson.PairClassLesson!.Teacher);

                for (int numberLesson = 1; numberLesson <= 8; ++numberLesson)
                {
                    ScheduleLesson? scheduleLesson = scheduleLessonsClass.FirstOrDefault(x => x.NumberLesson == numberLesson);

                    CheckBox isAvailableCheckBox = (FindName($"Lesson{numberLesson}_IsAvailableCheckBox") as CheckBox)!;
                    TextBox lessonNameTextBox = (FindName($"Lesson{numberLesson}_LessonNameTextBox") as TextBox)!;
                    TextBox teacherNameTextBox = (FindName($"Lesson{numberLesson}_TeacherNameTextBox") as TextBox)!;
                    TextBox cabinetNameTextBox = (FindName($"Lesson{numberLesson}_CabinetNameTextBox") as TextBox)!;

                    if (scheduleLesson != null)
                    {
                        isAvailableCheckBox.IsChecked = true;
                        lessonNameTextBox.Text = scheduleLesson.ClassLesson.ToShortString();
                        teacherNameTextBox.Text = scheduleLesson.ClassLesson.Teacher!.ToShortString() + (scheduleLesson.ClassLesson.PairClassLesson != null ? $" {scheduleLesson.ClassLesson.PairClassLesson.Teacher?.ToShortString()}" : null!);
                        cabinetNameTextBox.Text = scheduleLesson.Cabinet!.ToShortString() + (scheduleLesson.PairCabinet != null ? $" {scheduleLesson.PairCabinet.ToShortString()}" : null!);
                    }
                    else
                    {
                        isAvailableCheckBox.IsChecked = false;
                        lessonNameTextBox.Text = null!;
                        teacherNameTextBox.Text = null!;
                        cabinetNameTextBox.Text = null!;
                    }
                }
            }
            else
            {
                for (int numberLesson = 1; numberLesson <= 8; ++numberLesson)
                {
                    (FindName($"Lesson{numberLesson}_IsAvailableCheckBox") as CheckBox)!.IsChecked = false;
                    (FindName($"Lesson{numberLesson}_LessonNameTextBox") as TextBox)!.Text = null!;
                    (FindName($"Lesson{numberLesson}_TeacherNameTextBox") as TextBox)!.Text = null!;
                    (FindName($"Lesson{numberLesson}_CabinetNameTextBox") as TextBox)!.Text = null!;
                }
            }
        }

        private void ScheduleLessonsClassDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBusynessTabItem();
        }

        #endregion

        private void OnlyDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Service.OnlyDigit_PreviewTextInput(sender, e);
            base.OnPreviewTextInput(e);
        }
    }
}
