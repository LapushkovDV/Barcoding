using System;
using System.Globalization;
using Xamarin.Forms;

namespace TSD.Services.MarkupExtensions
{
    /// <summary>
    /// Конвертер значений из ресурсов
    /// </summary>
    public class ResourcesExtension : IValueConverter
    {
        /// <summary>
        /// Метод конвертации значения, полученного с формы
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Resources.ResourceManager.GetString(value.ToString());

            if (result == null)
            {
                result = value.ToString();
            }

            return result;
        }
        /// <summary>
        /// Метод конвертации значения, полученного с формы после потери фокуса
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

