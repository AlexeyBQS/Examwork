using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Shedule.Services
{
    public static class Message
    {
        public static MessageBoxResult Action_SaveChangesRecord() =>
            MessageBox.Show(
                "Вы действительно хотите сохранить изменения в записи?",
                "Внимание!",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

        public static MessageBoxResult Action_DeleteRecord() =>
            MessageBox.Show(
                "Вы действительно хотите удалить запись?",
                "Внимание!",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);
    }
}
