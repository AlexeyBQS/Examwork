using Shedule.Database;
using Shedule.Service;
using Shedule.ViewItemSource;
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
            window.WindowTitle = "Расписание - Главное меню";

            PhotoBorder.Background = new ImageBrush(ServiceImage.DefaultPhoto());

            using (DatabaseContext context = new DatabaseContext())
            {
                context.Teachers.Add(new Teacher
                {
                    Surname = "Фамилия",
                    Name = "Имя",
                    Patronymic = "Отчество",
                    Birthday = new DateTime(2022, 1, 1)
                });

                context.SaveChanges();

                TeacherListBox.ItemsSource = context.Teachers.ToList().Select(x => new TeacherViewItemSource(x));
            }
        }

        private void ChangePhotoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeletePhotoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SurnameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PatronymicTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BirthdayDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PassportTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
