using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Shedule
{
    public static class Message
    {
        public static void Error_NonExistSqlLocalDb()
        {
            MessageBox.Show(
                "Отсутствует установленная программа Microsoft SQL Server LocalDB!\nУстановите программу или обратитесь к системному администратору.",
                "Ошибка!",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
