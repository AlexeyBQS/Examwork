using Schedule.Database;
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
        public static MessageBoxResult Action_DeleteRecord() => MessageBox.Show(
            "Вы действительно хотите удалить запись?",
            "Внимание!",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);

        public static MessageBoxResult Action_SaveChangeScheduleWithIncreasedDifficulty() => MessageBox.Show(
            "Вы действительно хотите сохранить изменения в расписании с повышенной сложностью?",
            "Внимание!",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        public static MessageBoxResult Action_DeleteDatabase() => MessageBox.Show(
            "Вы действительно хотите удалить базу данных?",
            "Внимание!",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        public static void Message_IncorrectPassword() => MessageBox.Show(
            "Неверный пароль!",
            "Ошибка!",
            MessageBoxButton.OK,
            MessageBoxImage.Error);


        public static void Message_EmptyScheduleLessons() => MessageBox.Show(
            "Нет расписания на выбранный период!",
            "Уведомление",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        public static void Message_ErrorInitalizeExcel() => MessageBox.Show(
            "Ошибка инициализации Excel документа. Проверьте наличие приложения Excel и права доступа к нему.",
            "Ошибка!",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        public static void Message_NotExistFiles(string[] notExistFiles) => MessageBox.Show(
            $"Отсутствуют некоторые файлы программы:\n\n{string.Join("\n", notExistFiles)}\n\nПереустановите программу или обратитесь к системному адиминистратору",
            "Ошибка",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        public static void Message_BusyTeacher(int numberLesson, ClassLesson classLesson, Teacher teacher, ClassLesson busyClassLesson) => MessageBox.Show(
            $"Учитель {teacher.ToShortString()} дисциплины {classLesson.Lesson!.Name} занят в это время на уроке №{numberLesson} {busyClassLesson.Lesson!.Name} класса {busyClassLesson.Class!.Name}. Удалите данный урок у текущего класса или выберите другой урок.",
            $"Ошибка!",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        public static MessageBoxResult Action_BusyCabinet(int numberLesson, ClassLesson classLesson, Cabinet cabinet, ClassLesson busyClassLesson) => MessageBox.Show(
            $"Кабинет {cabinet.ToShortString()} дисциплины {classLesson.Lesson!.Name} занят в это время на уроке №{numberLesson} {busyClassLesson.Lesson!.Name} класса {busyClassLesson.Class!.Name}. Вы уверены, что хотите продолжить?",
            $"Ошибка!",
            MessageBoxButton.YesNo,
            MessageBoxImage.Error,
            MessageBoxResult.No);
    }
}
