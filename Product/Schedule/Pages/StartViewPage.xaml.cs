using Schedule.Services;
using System;
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
    /// Логика взаимодействия для StartViewPage.xaml
    /// </summary>
    public partial class StartViewPage : Page
    {
        public StartViewPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)!.WindowTitle = "Расписание - Загрузка";

            if (!ConfigManager.CancelStartAnimation)
            {
                await StartAnimationAsync();
            }
            else
            {
                LogoTextBlock.Foreground = Brushes.Black;
                DescriptionTextBlock.Foreground = Brushes.Black;
            }

            Auth();
        }

        private async Task StartAnimationAsync()
        {
            SolidColorBrush brush = new(Color.FromArgb(0, 0, 0, 0));

            for (byte i = 0; i < byte.MaxValue; ++i)
            {
                brush.Color = Color.FromArgb(i, 0, 0, 0);

                Task.Run(() => Dispatcher.Invoke(() =>
                {
                    LogoTextBlock.Foreground = brush;
                    DescriptionTextBlock.Foreground = brush;
                }));

                await Task.Delay(1);
            }
        }

        private void Auth()
        {
            Frame MainFrame = (Window.GetWindow(this) as MainWindow)!.MainFrame;

            MainFrame.Content = PasswordManager.GetPasswordFromDatabase() is not null
                ? new AuthViewPage()
                : new MainPage();
        }
    }
}
