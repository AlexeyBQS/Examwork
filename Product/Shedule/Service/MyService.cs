using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Shedule.Service
{
    public static class MyService
    {
        #region [Getters]

        public static DateTime? GetOnlyDate(DateTime? dateTime)
        {
            return dateTime != null
                ? new DateTime((dateTime ?? new()).Year, (dateTime ?? new()).Month, (dateTime ?? new()).Day)
                : null!;
        }

        private static BitmapImage GetImage(string fileName) =>
            File.Exists($"{Directory.GetCurrentDirectory()}\\Images\\{fileName}")
            ? new BitmapImage(new Uri($"{Directory.GetCurrentDirectory()}\\Images\\{fileName}"))
            : null!;

        public static BitmapImage DefaultPhoto => GetImage("DefaultPhoto.png");

        #endregion

        #region [Converters]

        public static BitmapImage ConvertByteArrayToImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null!;
            BitmapImage? image = new BitmapImage();

            using (MemoryStream? mem = new MemoryStream(byteArray))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }

            image.Freeze();
            return image;
        }

        public static byte[] ConvertImageToByteArray(BitmapImage image)
        {
            MemoryStream memStream = new();
            JpegBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(memStream);
            return memStream.ToArray();
        }

        #endregion

        #region [Contols]

        public static void OnlyDigit_PreiewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (char.IsNumber(c))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        public class ViewCountRecord
        {
            public ViewCountRecord(string name, int count)
            {
                Name = name;
                Count = count;
            }

            public string Name { get; set; } = null!;
            public int Count { get; set; } = default;

            public override string ToString() => Name;

        }

        public static void FillViewCountRecordComboBox(ComboBox sender)
        {
            sender.ItemsSource = new ViewCountRecord[]
            {
                new ViewCountRecord("50 строк", 50),
                new ViewCountRecord("100 строк", 100),
                new ViewCountRecord("1000 строк", 1000),
                new ViewCountRecord("Все", -1)
            };

            sender.SelectedIndex = 2;
        }



        #endregion
    }
}
