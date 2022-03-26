using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shedule.Services
{
    public static partial class Service
    {
        public static void OnlyDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (char.IsNumber(c))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        public static void OnlyPhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (char.IsNumber(c) || c == '+')
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
