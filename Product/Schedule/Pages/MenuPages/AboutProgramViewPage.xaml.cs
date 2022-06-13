using Schedule.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
    /// Логика взаимодействия для AboutProgramViewPage.xaml
    /// </summary>
    public partial class AboutProgramViewPage : Page
    {
        public AboutProgramViewPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TitleTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                if (EasterEggGifImage.Visibility == Visibility.Hidden)
                {
                    EasterEgg();
                }
            }
        }

        private async void EasterEgg()
        {
            SoundPlayer soundPlayer = new(
                Application.GetResourceStream(new Uri("pack://application:,,,/Schedule;component/Sounds/СhinaRockJohnson.wav")).Stream);
            EasterEggGifImage.SpeedRatio = 1.6;

            EasterEggBorder.Visibility = Visibility.Visible;
            EasterEggGifImage.Visibility = Visibility.Visible;
            EasterEggGifImage.StartAnimation();
            soundPlayer.Play();

            await Task.Delay(5250);

            EasterEggBorder.Visibility = Visibility.Hidden;
            EasterEggGifImage.Visibility = Visibility.Hidden;
            soundPlayer.Stop();
            EasterEggGifImage.StopAnimation();
            
            EasterEggGifImage.FrameIndex = 0;
        }
    }
}
