using System;
using System.IO;
using Xamarin.Forms;

namespace TSD.Services.Converters
{
    public static class ConverterImage
    {
        public static ImageSource ConvertByte64ToImageSource(string dataByte64) => ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(dataByte64)));

        public static ImageSource ConvertByteToByte64(byte[] dataByte) => Convert.ToBase64String(dataByte);

        public static ImageSource ConvertByteToImageSource(byte[] dataByte) => ImageSource.FromStream(() => new MemoryStream(dataByte));
    }
}
