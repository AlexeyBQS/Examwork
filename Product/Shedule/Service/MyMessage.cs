using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Shedule.Service
{
    public static class MyMessage
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
