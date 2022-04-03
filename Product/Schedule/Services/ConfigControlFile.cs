using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Services
{
    public class ConfigControlFile
    {
        public string ConnectionString { get; set; } = $"Data Source=ScheduleDatabase.db";
    }
}
