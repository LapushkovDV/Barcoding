using System.Collections.ObjectModel;
using TSD.Services.Tasks.Models;

namespace TSD.Services.Tasks
{
    /// <summary>
    /// Модель документа в окне задач
    /// </summary>
    public class DocumentTask : BaseTask
    {
        /// <summary>
        /// Системный идентификатор документа
        /// </summary>
        public string IdDoc { get; set; }

        /// <summary>
        /// Системный идентификатор (штрихкод)
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Разрешение редактирования документа
        /// </summary>
        public bool IsAllowEditDoc { get; set; }

        /// <summary>
        /// Статус документа при асинхронных операциях
        /// </summary>
        public string GuidStatus { get; set; } = string.Empty;

        /// <summary>
        /// Статус документа
        /// </summary>
        public int StatusId { get; set; } = 1;

        /// <summary>
        /// Последняя выполняемая операция
        /// </summary>
        public string LastAction { get; set; } = string.Empty;

        /// <summary>
        /// Системное имя меню для документа
        /// </summary>
        public string MenuAction { get; set; } = string.Empty;

        /// <summary>
        /// Список полей документа
        /// </summary>
        public ObservableCollection<FieldsTask> FieldsTasks { get; set; } = new ObservableCollection<FieldsTask>();
    }
}
