using Xamarin.Common.Mvvm.Base;

namespace TSD.Model
{
    /// <summary>
    /// Общая модель
    /// </summary>
    public abstract class SharedModel : BindableBase
    {
        /// <summary>
        /// Идентификатор объекта модели
        /// </summary>
        public int Id { get; set; }
    }
}
