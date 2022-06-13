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
    /// Логика взаимодействия для AuthViewPage.xaml
    /// </summary>
    public partial class AuthViewPage : Page
    {
        public AuthViewPage()
        {
            InitializeComponent();
        }

        private readonly CancellationTokenSource CancellationTokenSource = new();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)!.WindowTitle = "Расписание - Авторизация";

            TrackLengthPasswordBoxAsync(CancellationTokenSource.Token);
        }

        private async void TrackLengthPasswordBoxAsync(CancellationToken cancellationToken = default!)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                AuthButton.IsEnabled = PasswordBox.Password.Length > 0 || PasswordTextBox.Text.Length > 0;
                await Task.Delay(10, CancellationToken.None);
            }
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            bool showedPassword = ShowPasswordCheckBox.IsChecked ?? false;

            PasswordManager? input = PasswordManager.GetPassword(showedPassword ? PasswordTextBox.Text : PasswordBox.Password);
            PasswordManager? correct = PasswordManager.GetPasswordFromDatabase()!;

            if (input! == correct!)
            {
                (Window.GetWindow(this) as MainWindow)!.MainFrame.Content = new MainPage();
            }
            else
            {
                Message.Message_IncorrectPassword();
                PasswordBox.Password = string.Empty;
            }
        }

        private void PasswordBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (AuthButton.IsEnabled && e.Key == Key.Enter)
            {
                AuthButton_Click(sender, e);
            }
        }

        private void PasswordTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (AuthButton.IsEnabled && e.Key == Key.Enter)
            {
                AuthButton_Click(sender, e);
            }
        }

        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = PasswordBox.Password;
            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Visible;
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = PasswordTextBox.Text;
            PasswordTextBox.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;
        }
    }
}
