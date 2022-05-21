using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Schedule
{
    public class Program
    {
        private static readonly string[] PathFiles = new string[]
        {
            "e_sqlite3.dll",
        };

        private static bool ExistAllFilesProgram()
        {
            return PathFiles.Select(file => $"{Directory.GetCurrentDirectory()}\\{file}").All(File.Exists);
        }

        private static string[] NotExistFiles()
        {
            return PathFiles.Where(file => !File.Exists($"{Directory.GetCurrentDirectory()}\\{file}")).ToArray();
        }

        [STAThread]
        public static void Main(string[] args)
        {
#if DEBUG
            App.Main();
#elif RELEASE
            if (ExistAllFilesProgram() == true)
            {
                App.Main();
            }
            else
            {
                MessageBox.Show(
                    $"Отсутствуют некоторые файлы программы:\n\n{string.Join("\n", NotExistFiles())}\n\nПереустановите программу или обратитесь к системному адиминистратору",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
#endif
        }
    }
}
