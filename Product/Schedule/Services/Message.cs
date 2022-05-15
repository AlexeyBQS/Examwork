using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Schedule.Services
{
    public static class Message
    {
        public static MessageBoxResult Action_DeleteRecord() =>
            MessageBox.Show(
                "Вы действительно хотите удалить запись?",
                "Внимание!",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

        public static MessageBoxResult Action_SaveChangeScheduleWithIncreasedDifficulty() =>
            MessageBox.Show(
                "Вы действительно хотите сохранить изменения в расписании с повышенной сложностью?",
                "Внимание!",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

        public static void Message_IncorrectPassword() =>
            MessageBox.Show(
                "Неверный пароль!",
                "Ошибка!",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

        public static MessageBoxResult Action_DeleteDatabase() =>
            MessageBox.Show(
                "Вы действительно хотите удалить базу данных?",
                "Внимание!",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);
    }
}
