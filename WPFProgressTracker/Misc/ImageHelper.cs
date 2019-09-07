using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace WPFProgressTracker.Misc
{
    public static class ImageHelper
    {
        public static BitmapImage Base64StringToBitmap(string base64String)
        {
            byte[] imgBytes = Convert.FromBase64String(base64String);

            var bitmapImage = new BitmapImage();
            MemoryStream ms = new MemoryStream(imgBytes);
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        public static string BitmapToBase64String(BitmapImage image)
        {
            var bytes = new BinaryReader(image.StreamSource).ReadBytes(int.MaxValue);
            var imgString = Convert.ToBase64String(bytes);

            return imgString;
        }
    }
}
