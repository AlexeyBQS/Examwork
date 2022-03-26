using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Services
{
    public class ConfigControlFile
    {
        public string ConnectionString { get; set; } = $"Data Source=SheduleDatabase.db";
    }
}
