using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TSD.Services.Extensions
{
    /// <summary>
    /// Расшиерение Linq
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Функция перевода с одного типа массива в ObservableCollection
        /// </summary>
        /// <typeparam name="T">Универсальный тип значений в массиве</typeparam>
        /// <param name="_linqResult">Массив</param>
        /// <returns>ObservableCollection с указанным типом значений</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> _linqResult) => new ObservableCollection<T>(_linqResult);

        /// <summary>
        /// Асинхронная функция перевода с одного типа массива в ObservableCollection
        /// </summary>
        /// <typeparam name="T">Универсальный тип значений в массиве</typeparam>
        /// <param name="_linqResult">Массив</param>
        /// <returns>ObservableCollection с указанным типом значений</returns>
        public async static Task<ObservableCollection<T>> ToObservableCollectionAsync<T>(this IEnumerable<T> _linqResult) => await Task.Run(() => new ObservableCollection<T>(_linqResult));
    }
}
