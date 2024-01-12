using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TSD.Services.MarkupExtensions
{
    /// <summary>
    /// Расширение XAML для преобразования пути пространств имен в изображение
    /// </summary>
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        /// <summary>
        /// Путь к изображению в формате пространств имен
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Возвращение объекта из ресурса по Source
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }
            var imageSource = ImageSource.FromResource(Source);

            return imageSource;
        }
    }
}
