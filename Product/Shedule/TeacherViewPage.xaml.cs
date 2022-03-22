using Microsoft.Win32;
using Shedule.Database;
using Shedule.Service;
using Shedule.ViewItemSource;
using System;
using System.Collections.Generic;
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

namespace Shedule
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
            MainWindow window = Window.GetWindow(this) as MainWindow ?? null!;
            window.WindowTitle = "Расписание - Педагоги";

            MyService.FillViewCountRecordComboBox(ViewCountRecordComboBox);

            PhotoBorder.Background = new ImageBrush(MyService.DefaultPhoto);
            SaveTeacherPhotoButton.IsEnabled = false;
            ChangeTeacherPhotoButton.IsEnabled = false;
            DeleteTeacherPhotoButton.IsEnabled = false;
            SaveChangeTeacherButton.IsEnabled = false;
            DeleteTeacherButton.IsEnabled = false;

            UpdateDataListBox();
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

                IEnumerable<TeacherViewItemSource> viewItemSource = CountViewRecord > 0
                    ? teachers.ToList().Take(CountViewRecord).Select(x => new TeacherViewItemSource(x))
                    : teachers.ToList().Select(x => new TeacherViewItemSource(x));

                TeacherListBox.ItemsSource = viewItemSource;

                StatusTextBlock.Text = $"Всего: {context.Teachers.Count()} | Всего с фильтрами: {teachers.Count()} | Отображается: {viewItemSource.Count()}";
            }
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
                }
            }
            else
            {
                TeacherIdTextBox.Text = null!;
                SurnameTextBox.Text = null!;
                NameTextBox.Text = null!;
                PatronymicTextBox.Text = null!;
                BirthdayDatePicker.Text = null!;
                PassportTextBox.Text = null!;
                PhotoBorder.Background = new ImageBrush(MyService.DefaultPhoto);
                EducationTextBox.Text = null!;

                SaveTeacherPhotoButton.IsEnabled = false;
                ChangeTeacherPhotoButton.IsEnabled = false;
                DeleteTeacherPhotoButton.IsEnabled = false;

                SaveChangeTeacherButton.IsEnabled = false;
                DeleteTeacherButton.IsEnabled = false;
            }
        }

        #endregion

        #region [WorkWithListBox]

        private void TeacherListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateViewTabControl();
        }

        private void ViewCountRecordComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CountViewRecord = ((sender as ComboBox)!.SelectedItem as MyService.ViewCountRecord)!.Count;
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
                int? teacherId = (TeacherListBox.SelectedItems[0] as TeacherViewItemSource)?.TeacherId ?? -1;

                using (DatabaseContext context = new())
                {
                    Teacher? teacher = context.Teachers.FirstOrDefault(x => x.TeacherId == (int)teacherId);

                    if (teacher != null)
                    {
                        if (teacher.Photo != null && teacher.Photo?.Length != 0)
                        {
                            BitmapImage image = MyService.ConvertByteArrayToImage(teacher.Photo ?? null!);

                            SaveFileDialog dialog = new();
                            dialog.FileName = $"Image_{TeacherIdTextBox.Text ?? "0"}.png";
                            dialog.Filter = "All Files (*.*)|*.*";
                            dialog.FilterIndex = 0;

                            if (dialog.ShowDialog() == true)
                            {
                                string pathFile = $"{dialog.InitialDirectory}\\{dialog.FileName}".Remove(0, 1);

                                BitmapEncoder encoder = new PngBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(image));

                                using (FileStream? fileStream = new FileStream(pathFile, FileMode.Create))
                                {
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
            }

            SaveTeacherPhotoButton.IsEnabled = true;
            DeleteTeacherPhotoButton.IsEnabled = true;

            SaveChangeTeacherButton.IsEnabled = true;
        }

        private void DeleteTeacherPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoBorder.Background = new ImageBrush(MyService.DefaultPhoto);

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
                context.SaveChanges();
            }

            UpdateDataListBox();

            TeacherListBox.SelectedIndex = TeacherListBox.Items.Count - 1;
        }

        private void SaveChangeTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            if (TeacherListBox.SelectedItems.Count > 0)
            {
                TeacherViewItemSource viewItemSource = (TeacherListBox.SelectedItems[0] as TeacherViewItemSource) ?? null!;
                int? teacherId = viewItemSource?.TeacherId ?? -1;

                using (DatabaseContext context = new())
                {
                    Teacher? teacher = context.Teachers.FirstOrDefault(x => x.TeacherId == (int)teacherId);

                    if (teacher != null)
                    {
                        BitmapImage photo = (PhotoBorder.Background as ImageBrush)?.ImageSource as BitmapImage ?? null!;

                        teacher.Surname = SurnameTextBox.Text;
                        teacher.Name = NameTextBox.Text;
                        teacher.Patronymic = PatronymicTextBox.Text;
                        teacher.Birthday = MyService.GetOnlyDate(BirthdayDatePicker.SelectedDate ?? null!);
                        teacher.Photo = photo != MyService.DefaultPhoto ? MyService.ConvertImageToByteArray(photo) : null!;
                        teacher.Education = EducationTextBox.Text;
                    }

                    context.SaveChangesAsync();
                }

                UpdateDataListBox();
            }
        }

        private void DeleteTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            if (TeacherListBox.SelectedItems.Count > 0)
            {
                int? teacherId = (TeacherListBox.SelectedItems[0] as TeacherViewItemSource)?.TeacherId ?? -1;

                using (DatabaseContext context = new())
                {
                    Teacher? teacher = context.Teachers.FirstOrDefault(x => x.TeacherId == (int)teacherId);

                    if (teacher != null)
                    {
                        context.Teachers.Remove(teacher ?? new());

                        context.SaveChangesAsync();
                    }
                }

                UpdateDataListBox();
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
            EducationFilterTextBox.Text = null!;
        }

        #endregion

        private void OnlyDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Service.MyService.OnlyDigit_PreiewTextInput(sender, e);
            base.OnPreviewTextInput(e);
        }
    }
}
