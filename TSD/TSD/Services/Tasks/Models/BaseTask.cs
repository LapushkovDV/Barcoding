using Xamarin.Common.Mvvm.Base;

namespace TSD.Services.Tasks.Models
{
    /// <summary>
    /// Базовый класс для задач
    /// </summary>
    public abstract class BaseTask : BindableBase
    {
        /// <summary>
        /// Идентификатор документа
        /// </summary>
        public int Id { get; set; }
    }
}
