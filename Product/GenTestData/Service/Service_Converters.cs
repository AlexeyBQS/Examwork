using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenTestData.Service
{
    public static partial class Service
    {
        public static byte[]? ConvertFileToImageByteArray(string path)
        {
            Image image = Image.FromFile(path);

            Bitmap bitmap = Service.ResizeImage(image);

            ImageConverter converter = new();
            return converter.ConvertTo(bitmap, typeof(byte[]))! as byte[];
        }
    }
}
