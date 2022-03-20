using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Shedule.Service
{
    /// <summary>
    /// Конфигурация приложения
    /// </summary>
    public static class Config
    {
        private static readonly string NameConfigFile = "Config.json";
        private static string PathConfigFile => $"{Directory.GetCurrentDirectory()}\\{NameConfigFile}";

        /// <summary>
        /// Сериализуемый класс конфигурации приложения для чтения/записи файла
        /// </summary>
        private class ControlConfigFile
        {
            /// <summary>
            /// Строка подключения к базе данных
            /// </summary>
            public string ConnectionString { get; set; } = $"Data Source=SheduleDatabase.db";
        }

        static Config()
        {
            if (!File.Exists(PathConfigFile))
            {
                ConfigFile = new ControlConfigFile();
            }
        }

        private static ControlConfigFile ConfigFile
        {
            get
            {
                return JsonSerializer.Deserialize<ControlConfigFile>(File.ReadAllText(PathConfigFile)) ?? new();
            }

            set
            {
                File.WriteAllText(PathConfigFile, JsonSerializer.Serialize(value));
            }
        }

        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return ConfigFile.ConnectionString;
            }

            set
            {
                ConfigFile.ConnectionString = value;
            }
        }
    }
}
