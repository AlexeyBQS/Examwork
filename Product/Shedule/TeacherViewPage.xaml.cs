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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow window = Window.GetWindow(this) as MainWindow ?? null!;
            window.WindowTitle = "Расписание - Педагоги";

            PhotoBorder.Background = new ImageBrush(ServiceImage.DefaultPhoto);
            SaveTeacherPhotoButton.IsEnabled = false;
            ChangeTeacherPhotoButton.IsEnabled = false;
            DeleteTeacherPhotoButton.IsEnabled = false;
            SaveChangeTeacherButton.IsEnabled = false;
            DeleteTeacherButton.IsEnabled = false;

            UpdateDataListBox();
        }

        private void UpdateDataListBox()
        {
            using (DatabaseContext context = new())
            {
                TeacherListBox.ItemsSource = context.Teachers.ToList().Select(x => new TeacherViewItemSource(x));
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
                PhotoBorder.Background = new ImageBrush(ServiceImage.DefaultPhoto);
                EducationTextBox.Text = null!;

                SaveTeacherPhotoButton.IsEnabled = false;
                ChangeTeacherPhotoButton.IsEnabled = false;
                DeleteTeacherPhotoButton.IsEnabled = false;

                SaveChangeTeacherButton.IsEnabled = false;
                DeleteTeacherButton.IsEnabled = false;
            }
        }

        private void SaveTeacherPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (TeacherListBox.SelectedItems.Count > 0)
            {
                int? teacherId = (TeacherListBox.SelectedItems[0] as TeacherViewItemSource)?.TeacherId ?? -1;

                using (DatabaseContext context = new())
                {
                    var teacher = context.Teachers.FirstOrDefault(x => x.TeacherId == (int)teacherId);

                    if (teacher != null)
                    {
                        if (teacher.Photo != null && teacher.Photo?.Length != 0)
                        {
                            BitmapImage image = MyConvert.ByteArrayToImage(teacher.Photo ?? null!);

                            SaveFileDialog dialog = new SaveFileDialog();
                            dialog.FileName = $"Image_{TeacherIdTextBox.Text ?? "0"}.png";
                            dialog.Filter = "All Files (*.*)|*.*";
                            dialog.FilterIndex = 0;

                            if (dialog.ShowDialog() == true)
                            {
                                string pathFile = $"{dialog.InitialDirectory}\\{dialog.FileName}".Remove(0, 1);

                                BitmapEncoder encoder = new PngBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(image));

                                using (var fileStream = new FileStream(pathFile, FileMode.Create))
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
            OpenFileDialog dialog = new OpenFileDialog();
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
            PhotoBorder.Background = new ImageBrush(ServiceImage.DefaultPhoto);

            SaveTeacherPhotoButton.IsEnabled = false;
            DeleteTeacherPhotoButton.IsEnabled = false;

            SaveChangeTeacherButton.IsEnabled = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveChangeTeacherButton.IsEnabled = true;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveChangeTeacherButton.IsEnabled = true;
        }

        private void TeacherListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateViewTabControl();
        }

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
                    var teacher = context.Teachers.FirstOrDefault(x => x.TeacherId == (int)teacherId);

                    if (teacher != null)
                    {
                        BitmapImage photo = (PhotoBorder.Background as ImageBrush)?.ImageSource as BitmapImage ?? null!;

                        teacher.Surname = SurnameTextBox.Text;
                        teacher.Name = NameTextBox.Text;
                        teacher.Patronymic = PatronymicTextBox.Text;
                        teacher.Birthday = MyConvert.GetOnlyDate(BirthdayDatePicker.SelectedDate ?? null!);
                        teacher.Photo = photo != ServiceImage.DefaultPhoto ? MyConvert.ImageToByteArray(photo) : null!;
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
                    var teacher = context.Teachers.FirstOrDefault(x => x.TeacherId == (int)teacherId);

                    if (teacher != null)
                    {
                        context.Teachers.Remove(teacher ?? new());

                        context.SaveChangesAsync();
                    }
                }

                UpdateDataListBox();
            }
        }

        private void OnlyDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Service.PreviewTextInput.OnlyDigit_PreiewTextInput(sender, e);
            base.OnPreviewTextInput(e);
        }
    }
}
