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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private class MenuElement
        {
            public string Name { get; set; } = null!;
            public Page Page { get; set; } = null!;

            public MenuElement(string name, Page page)
            {
                Name = name;
                Page = page;
            }
        }


        private readonly MenuElement[] Menu = new MenuElement[]
        {
            new("Педагоги", new TeacherViewPage())
        };

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow window = Window.GetWindow(this) as MainWindow ?? null!;
            window.WindowTitle = "Расписание";

            MenuListBox.ItemsSource = Menu;
        }

        private void OpenTeacherViewPageButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = Window.GetWindow(this) as MainWindow ?? null!;
            window.MainFrame.Content = new TeacherViewPage();
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuListBox.SelectedItems.Count > 0)
            {
                MenuElement selectedMenuElement = (MenuListBox.SelectedItems[0] as MenuElement) ?? null!;

                MenuFrame.Content = selectedMenuElement.Page ?? null!;
            }
        }
    }
}
