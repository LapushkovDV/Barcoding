using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace TSD.Services.MarkupExtensions.ValuesConverters
{
    /// <summary>
    /// Конвертация строк в ListView
    /// </summary>
    public class StripedBackgroundIndexConverter : IValueConverter
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
            if (value == null || parameter == null) return Color.White;

            var index = ((ListView)parameter).ItemsSource.Cast<object>().ToList().IndexOf(value);

            return index % 2 == 0 ? Color.White : Color.LightGray;
        }

        /// <summary>
        /// Метод конвертации значения, полученного с формы после потери фокуса
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
