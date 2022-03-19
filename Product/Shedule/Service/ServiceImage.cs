using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Shedule.Service
{
    public static class ServiceImage
    {
        private static BitmapImage? GetPhoto(string fileName) =>
            File.Exists($"{Directory.GetCurrentDirectory()}\\Images\\{fileName}")
            ? new BitmapImage(new Uri($"{Directory.GetCurrentDirectory()}\\Images\\{fileName}"))
            : null!;

        public static BitmapImage? DefaultPhoto() => GetPhoto("DefaultPhoto.png");
    }
}
