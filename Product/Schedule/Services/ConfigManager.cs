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
        private static string NameConfigFile => "Config.json";
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
        /// Имя файла базы данных
        /// </summary>
        public static string DatabaseFileName
        {
            get => ConfigFile.DatabaseFileName;
            set
            {
                ConfigControlFile file = ConfigFile;
                file.DatabaseFileName = value;
                ConfigFile = file;
            }
        }

        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        public static string ConnectionString
        {
            get => $"Data Source={DatabaseFileName};";
        }

        /// <summary>
        /// Отключить анимацию при загрузке приложения
        /// </summary>
        public static bool CancelStartAnimation
        {
            get => ConfigFile.CancelStartAnimation;
            set
            {
                ConfigControlFile file = ConfigFile;
                file.CancelStartAnimation = value;
                ConfigFile = file;
            }
        }
    }
}
