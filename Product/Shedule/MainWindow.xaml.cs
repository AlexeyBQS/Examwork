using Shedule.Pages;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string _WindowTitle = null!;
        public string WindowTitle
        {
            get
            {
                return _WindowTitle;
            }
            set
            {
                _WindowTitle = value;
                WindowTitle_ValueChanged();
            }
        }

        private void WindowTitle_ValueChanged()
        {
            Title = WindowTitle;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new MainPage();
        }
    }
}
