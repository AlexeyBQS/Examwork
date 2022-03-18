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

namespace Shedule
{
    public class Program
    {
        private static string _DatabaseName = string.Empty;
        private static string DatabaseName
        {
            get
            {
                if (_DatabaseName == string.Empty)
                {
                    _DatabaseName = ConfigurationManager.ConnectionStrings["DatabaseName"].ConnectionString;
                }

                return _DatabaseName;
            }
        }

        private static bool ExistSqlLocalDb()
        {
            Process cmd = new Process();

            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine("sqllocaldb info");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();

            return cmd.StandardOutput.ReadToEnd().Contains("MSSQLLocalDB");
        }

        private static void ChangePathConnectionStringDatabase()
        {
            if (!ExistSqlLocalDb())
            {
                Message.Error_NonExistSqlLocalDb();
                Environment.Exit(0);
            }

            string DataSource = "(LocalDB)\\MSSQLLocalDB";
            string AttachDbFilename = $"{Directory.GetCurrentDirectory()}\\{DatabaseName}.mdf";
            bool IntegratedSecurity = true;
            string ApplicationIntent = "ReadWrite";
            bool MultiSubnetFailover = false;
            bool TrustedConnection = true;

            string connectionString =
                $"Data Source = {DataSource};"
                + $" AttachDbFilename = {AttachDbFilename};"
                + $" Integrated Security = {IntegratedSecurity};"
                + $" ApplicationIntent = {ApplicationIntent};"
                + $" MultiSubnetFailover = {MultiSubnetFailover};"
                + $" Trusted_Connection = {TrustedConnection}";

            string providerName = "System.Data.SqlClient";

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");

            connectionStringsSection.ConnectionStrings["DatabaseConnection"].ConnectionString = connectionString;
            connectionStringsSection.ConnectionStrings["DatabaseConnection"].ProviderName = providerName;
            config.Save();

            ConfigurationManager.RefreshSection("connectionStrings");
        }

        [STAThread]
        public static void Main(string[] args)
        {
            ChangePathConnectionStringDatabase();

            App.Main();
        }
    }
}
