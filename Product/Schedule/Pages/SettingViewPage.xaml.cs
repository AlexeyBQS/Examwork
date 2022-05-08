using Schedule.Database;
using Schedule.Services;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для SettingViewPage.xaml
    /// </summary>
    public partial class SettingViewPage : Page
    {
        public SettingViewPage()
        {
            InitializeComponent();
        }

        private CancellationTokenSource CancellationTokenSource = new();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)!.WindowTitle = "Расписание - Настройки";

            TrackPasswordBlockAsync(CancellationTokenSource.Token);
            TrackDatabaseBlockAsync(CancellationTokenSource.Token);
            StartAnimationCheckBox.IsChecked = ConfigManager.CancelStartAnimation;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CancellationTokenSource.Cancel();
        }

        #region PasswordBlock

        private void CheckPasswordBlock()
        {
            if (PasswordManager.GetPasswordFromDatabase() is not null)
            {
                StatusPasswordTextBox.Text = "Пароль установлен";
                StatusPasswordTextBox.Foreground = Brushes.Green;
                OldPasswordBox.IsEnabled = true;
                ChangePasswordButton.Content = "Сменить пароль";
            }
            else
            {
                StatusPasswordTextBox.Text = "Пароль не установлен";
                StatusPasswordTextBox.Foreground = Brushes.Red;
                OldPasswordBox.IsEnabled = false;
                ChangePasswordButton.Content = "Установить пароль";
            }

            OldPasswordBox.Password = string.Empty;
            NewPasswordBox.Password = string.Empty;
            RepeatNewPasswordBox.Password = string.Empty;
        }

        private async void TrackPasswordBlockAsync(CancellationToken cancellationToken = default!)
        {
            CheckPasswordBlock();

            while (!cancellationToken.IsCancellationRequested)
            {
                ChangePasswordButton.IsEnabled =
                    (!OldPasswordBox.IsEnabled || OldPasswordBox.Password.Length > 0)
                    && (!OldPasswordBox.IsEnabled
                        ? (NewPasswordBox.Password.Length > 0 && NewPasswordBox.Password == RepeatNewPasswordBox.Password)
                        : (NewPasswordBox.Password == RepeatNewPasswordBox.Password)
                    );

                await Task.Delay(100);
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            using DatabaseContext context = new();

            if (PasswordManager.GetPasswordFromDatabase() is not null)
            {
                if (PasswordManager.GetPassword(OldPasswordBox.Password)! == PasswordManager.GetPasswordFromDatabase()!)
                {
                    PasswordManager.SetPasswordInDatabase(PasswordManager.GetPassword(NewPasswordBox.Password));
                }
                else
                {
                    Message.Message_IncorrectPassword();
                }
            }
            else
            {
                PasswordManager.SetPasswordInDatabase(PasswordManager.GetPassword(NewPasswordBox.Password));
            }

            CheckPasswordBlock();
        }

        #endregion

        #region DatabaseBlock

        private async void TrackDatabaseBlockAsync(CancellationToken cancellationToken = default!)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                DeleteDatabaseButton.IsEnabled = File.Exists($"{Directory.GetCurrentDirectory()}\\{ConfigManager.DatabaseFileName}");

                await Task.Delay(100);
            }
        }

        private void DeleteDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Message.Action_DeleteDatabase() == MessageBoxResult.Yes)
            {
                using DatabaseContext context = new();

                context.Database.EnsureDeletedAsync();
            }
        }

        #endregion

        #region StartLogoAnimation

        private void StartAnimationCheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            ConfigManager.CancelStartAnimation = StartAnimationCheckBox.IsChecked ?? false;
        }

        #endregion

        #region SessionBlock

        private void CloseSessionButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)!.MainFrame.Content = new StartViewPage();
        }

        #endregion
    }
}
