using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Schedule.Services
{
    public static class Service
    {
        #region Converters

        public static BitmapImage ConvertByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null!;
            BitmapImage? image = new();

            using (MemoryStream mem = new(byteArray))
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

        public static byte[]? ConvertImageToByteArray(System.Drawing.Image image)
        {
            ImageConverter converter = new();
            return converter.ConvertTo(image, typeof(byte[]))! as byte[];
        }

        public static Bitmap ConvertImageSourceToBitmap(ImageSource imageSource)
        {
            BitmapSource bitmapSource = (imageSource as BitmapSource)!;
            if (bitmapSource == null) return null!;

            using MemoryStream outStream = new();

            BmpBitmapEncoder encoder = new();

            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(outStream);
            return new(outStream);
        }

        #endregion

        #region Getters

        public static DateTime? GetOnlyDate(DateTime? dateTime) =>
            dateTime != null
            ? new DateTime((dateTime ?? new()).Year, (dateTime ?? new()).Month, (dateTime ?? new()).Day)
            : null!;

        private static BitmapImage GetImage(string fileName) =>
            File.Exists($"{Directory.GetCurrentDirectory()}\\Images\\{fileName}")
            ? new BitmapImage(new Uri($"{Directory.GetCurrentDirectory()}\\Images\\{fileName}"))
            : null!;

        private static BitmapImage defaultPhoto = GetImage("DefaultPhoto.png");
        public static BitmapImage DefaultPhoto => defaultPhoto;

        #endregion

        #region PreviewTextInput

        public static void OnlyDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        public static void OnlyPhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (char.IsNumber(c) || c == '+')
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        #endregion

        #region ResizeImage

        public static Bitmap ResizeImage(System.Drawing.Image image, int width = default, int height = default)
        {
            if (width == default) width = 128;
            if (height == default) height = 128;

            Rectangle destRect = new(0, 0, width, height);
            Bitmap destImage = new(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using ImageAttributes wrapMode = new();

                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }

            return destImage;
        }

        #endregion

        #region ViewCountRecord

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
