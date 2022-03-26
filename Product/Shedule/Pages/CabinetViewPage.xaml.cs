using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Shedule.Database;
using Shedule.Services;
using Shedule.ViewItemSources;
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

namespace Shedule.Pages
{
    /// <summary>
    /// Логика взаимодействия для CabinetViewPage.xaml
    /// </summary>
    public partial class CabinetViewPage : Page
    {
        public CabinetViewPage()
        {
            InitializeComponent();
        }

        private int CountViewRecord { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)!.WindowTitle = "Расписание - Кабинеты";

            Service.FillViewCountRecordComboBox(ViewCountRecordComboBox);

            UpdateDataComboBox();
            UpdateDataListBox();
            UpdateViewTabControl();
        }

        #region [Updaters]

        private void UpdateDataComboBox()
        {
            using (DatabaseContext context = new())
            {
                IEnumerable<Teacher> teachers = context.Teachers.ToList();

                TeacherIdComboBox.ItemsSource = teachers;
                TeacherIdFilterComboBox.ItemsSource = teachers;
            }
        }

        private void UpdateDataListBox()
        {
            using (DatabaseContext context = new())
            {
                IQueryable<Cabinet> cabinets = context.Cabinets
                    .Include(cabinet => cabinet.Teacher);

                if (CabinetIdFilterTextBox.Text != null)
                {
                    string cabinetId = CabinetIdFilterTextBox.Text;
                    cabinets = cabinets.Where(cabinet => cabinet.CabinetId.ToString().Contains(cabinetId));
                }

                if (TeacherIdFilterComboBox.SelectedItem != null)
                {
                    int teacherId = (TeacherIdFilterComboBox.SelectedItem as Teacher)!.TeacherId;
                    cabinets = cabinets.Where(cabinet => cabinet.TeacherId == teacherId);
                }

                if (NameFilterTextBox.Text != null)
                {
                    string name = NameFilterTextBox.Text;
                    cabinets = cabinets.Where(cabinet => cabinet.Name!.Contains(name));
                }

                if (CountPlacesFilterTextBox.Text != null)
                {
                    string countPlaces = CountPlacesFilterTextBox.Text;
                    cabinets = cabinets.Where(cabinet => cabinet.CountPlaces.ToString()!.Contains(countPlaces));
                }

                if (DescriptionFilterTextBox.Text != null)
                {
                    string description = DescriptionFilterTextBox.Text;
                    cabinets = cabinets.Where(cabinet => cabinet.Description!.Contains(description));
                }

                IEnumerable<CabinetViewItemSource> viewItemSources = CountViewRecord > 0
                    ? cabinets.ToList().Take(CountViewRecord).Select(x => new CabinetViewItemSource(x))
                    : cabinets.ToList().Select(x => new CabinetViewItemSource(x));

                CabinetListBox.ItemsSource = viewItemSources;

                StatusTextBlock.Text = $"Всего: {context.Cabinets.Count()} | Всего с фильтрами: {cabinets.Count()} | Отображается: {viewItemSources.Count()}";
            }
        }

        private void DefaultViewTabControl()
        {
            CabinetIdTextBox.Text = null!;
            TeacherIdComboBox.SelectedIndex = -1;
            NameTextBox.Text = null!;
            CountPlacesTextBox.Text = null!;
            PhotoBorder.Background = new ImageBrush(Service.DefaultPhoto);
            DescriptionTextBox.Text = null!;

            SaveCabinetPhotoButton.IsEnabled = false;
            ChangeCabinetPhotoButton.IsEnabled = false;
            DeleteCabinetPhotoButton.IsEnabled = false;

            SaveChangeCabinetButton.IsEnabled = false;
            DeleteCabinetButton.IsEnabled = false;

            CabinetIdTextBox.IsEnabled = false;
            TeacherIdComboBox.IsEnabled = false;
            TeacherIdComboBoxClearButton.IsEnabled = false;
            NameTextBox.IsEnabled = false;
            CountPlacesTextBox.IsEnabled = false;
            DescriptionTextBox.IsEnabled = false;
        }

        private void UpdateViewTabControl()
        {
            if (CabinetListBox.SelectedItems.Count > 0)
            {
                CabinetViewItemSource cabinet = CabinetListBox.SelectedItems[0] as CabinetViewItemSource ?? null!;

                if (cabinet != null)
                {
                    CabinetIdTextBox.Text = cabinet.CabinetId.ToString();

                    using (DatabaseContext context = new())
                    {
                        IEnumerable<Teacher> teachers = context.Teachers.ToList();

                        TeacherIdComboBox.ItemsSource = teachers;
                        TeacherIdFilterComboBox.ItemsSource = teachers;

                        TeacherIdComboBox.SelectedIndex = cabinet.TeacherId != null
                            ? TeacherIdComboBox.Items.IndexOf(context.Teachers.First(t => t.TeacherId == cabinet.TeacherId))
                            : -1;
                    }

                    NameTextBox.Text = cabinet.Name;
                    CountPlacesTextBox.Text = cabinet.CountPlaces.ToString();
                    PhotoBorder.Background = new ImageBrush(cabinet.Photo_Image);
                    DescriptionTextBox.Text = cabinet.Description;

                    SaveChangeCabinetButton.IsEnabled = false;
                    DeleteCabinetButton.IsEnabled = true;

                    ChangeCabinetPhotoButton.IsEnabled = true;

                    if (cabinet.Photo == null || cabinet.Photo?.Length == 0)
                    {
                        SaveCabinetPhotoButton.IsEnabled = false;
                        DeleteCabinetPhotoButton.IsEnabled = false;
                    }
                    else
                    {
                        SaveCabinetPhotoButton.IsEnabled = true;
                        DeleteCabinetPhotoButton.IsEnabled = true;
                    }

                    CabinetIdTextBox.IsEnabled = true;
                    TeacherIdComboBox.IsEnabled = true;
                    TeacherIdComboBoxClearButton.IsEnabled = true;
                    NameTextBox.IsEnabled = true;
                    CountPlacesTextBox.IsEnabled = true;
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

        private void CabinetListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            SaveChangeCabinetButton.IsEnabled = true;
        }

        private void ViewDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveChangeCabinetButton.IsEnabled = true;
        }

        private void ViewComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveChangeCabinetButton.IsEnabled = true;
        }

        #endregion

        #region [WorkWithRecord]

        private void AddCabinetButton_Click(object sender, RoutedEventArgs e)
        {
            using (DatabaseContext context = new())
            {
                context.Cabinets.Add(new Cabinet());
                context.SaveChanges();
            }

            UpdateDataListBox();

            CabinetListBox.SelectedIndex = CabinetListBox.Items.Count - 1;
        }

        private void SaveChangeCabinetButton_Click(object sender, RoutedEventArgs e)
        {
            if (CabinetListBox.SelectedItems.Count > 0)
            {
                if (Message.Action_SaveChangesRecord () == MessageBoxResult.Yes)
                {
                    CabinetViewItemSource viewItemSource = (CabinetListBox.SelectedItems[0] as CabinetViewItemSource) ?? null!;
                    int? cabinetId = viewItemSource?.CabinetId ?? -1;

                    using (DatabaseContext context = new())
                    {
                        Cabinet? cabinet = context.Cabinets.FirstOrDefault(x => x.CabinetId == (int)cabinetId);

                        if (cabinet != null)
                        {
                            BitmapImage photo = (PhotoBorder.Background as ImageBrush)!.ImageSource as BitmapImage ?? null!;

                            cabinet.TeacherId = TeacherIdComboBox.SelectedIndex >= 0 ? (TeacherIdFilterComboBox.Items[TeacherIdComboBox.SelectedIndex] as Teacher)!.TeacherId : null!;
                            cabinet.Name = NameTextBox.Text;
                            cabinet.CountPlaces = CountPlacesTextBox.Text != string.Empty ? int.Parse(CountPlacesTextBox.Text) : null!;
                            cabinet.Photo = Service.ConvertImageToByteArray(photo);
                            cabinet.Description = DescriptionTextBox.Text;
                        }

                        context.SaveChangesAsync();
                    }

                    UpdateDataListBox();
                }
            }
        }

        private void DeleteCabinetButton_Click(object sender, RoutedEventArgs e)
        {
            if (CabinetListBox.SelectedItems.Count > 0)
            {
                if (Message.Action_DeleteRecord() == MessageBoxResult.Yes)
                {
                    int? cabinetId = (CabinetListBox.SelectedItems[0] as CabinetViewItemSource)?.CabinetId ?? -1;

                    using (DatabaseContext context = new())
                    {
                        Cabinet? cabinet = context.Cabinets.FirstOrDefault(x => x.CabinetId == (int)cabinetId);

                        if (cabinet != null)
                        {
                            context.Cabinets.Remove(cabinet);

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

        #region [WorkWithPhoto]

        private void SaveCabinetPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (CabinetListBox.SelectedItems.Count > 0)
            {
                int? cabinetId = (CabinetListBox.SelectedItems[0] as CabinetViewItemSource)?.CabinetId ?? -1;

                using (DatabaseContext context = new())
                {
                    Cabinet? cabinet = context.Cabinets.FirstOrDefault(x => x.CabinetId == (int)cabinetId);

                    if (cabinet != null)
                    {
                        if (cabinet.Photo != null && cabinet.Photo?.Length != 0)
                        {
                            BitmapImage image = Service.ConvertByteArrayToImage(cabinet.Photo ?? null!);

                            SaveFileDialog dialog = new();
                            dialog.FileName = $"Педагог_{CabinetIdTextBox.Text ?? "0"}.png";
                            dialog.Filter = "All Files (*.*)|*.*";
                            dialog.FilterIndex = 0;

                            if (dialog.ShowDialog() == true)
                            {
                                string pathFile = $"{dialog.InitialDirectory}\\{dialog.FileName}".Remove(0, 1);

                                BitmapEncoder encoder = new PngBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(image));

                                using (FileStream? fileStream = new(pathFile, FileMode.Create))
                                {
                                    encoder.Save(fileStream);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ChangeCabinetPhotoButton_Click(object sender, RoutedEventArgs e)
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

            SaveCabinetPhotoButton.IsEnabled = true;
            DeleteCabinetPhotoButton.IsEnabled = true;

            SaveChangeCabinetButton.IsEnabled = true;
        }

        private void DeleteCabinetPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoBorder.Background = new ImageBrush(Service.DefaultPhoto);

            SaveCabinetPhotoButton.IsEnabled = false;
            DeleteCabinetPhotoButton.IsEnabled = false;

            SaveChangeCabinetButton.IsEnabled = true;
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

        private void ClearFiltresButton_Click(object sender, RoutedEventArgs e)
        {
            CabinetIdFilterTextBox.Text = null!;
            TeacherIdFilterComboBox.SelectedIndex = -1;
            NameFilterTextBox.Text = null!;
            CountPlacesFilterTextBox.Text = null!;
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
