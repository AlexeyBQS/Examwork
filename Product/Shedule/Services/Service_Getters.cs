using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Shedule.Services
{
    public static partial class Service
    {
        public static DateTime? GetOnlyDate(DateTime? dateTime) =>
            dateTime != null
            ? new DateTime((dateTime ?? new()).Year, (dateTime ?? new()).Month, (dateTime ?? new()).Day)
            : null!;

        private static BitmapImage GetImage(string fileName) =>
            File.Exists($"{Directory.GetCurrentDirectory()}\\Images\\{fileName}")
            ? new BitmapImage(new Uri($"{Directory.GetCurrentDirectory()}\\Images\\{fileName}"))
            : null!;

        public static BitmapImage DefaultPhoto => GetImage("DefaultPhoto.png");
    }
}
