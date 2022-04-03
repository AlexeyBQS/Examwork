using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Schedule.Services
{
    /// <summary>
    /// Конфигурация приложения
    /// </summary>
    public static class ConfigManager
    {
        private static string NameConfigFile => "config.json";
        private static string PathConfigFile => $"{Directory.GetCurrentDirectory()}\\{NameConfigFile}";

        static ConfigManager()
        {
            if (!File.Exists(PathConfigFile))
            {
                ConfigFile = new ConfigControlFile();
            }
        }

        /// <summary>
        /// Чтение и запись файла конфигурации
        /// </summary>
        private static ConfigControlFile ConfigFile
        {
            get => JsonSerializer.Deserialize<ConfigControlFile>(File.ReadAllText(PathConfigFile)) ?? new();
            set => File.WriteAllText(PathConfigFile, JsonSerializer.Serialize(value));
        }


        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        public static string ConnectionString
        {
            get => ConfigFile.ConnectionString;
            set => ConfigFile.ConnectionString = value;
        }
    }
}
