namespace TSD.Services.Tasks.Models
{
    /// <summary>
    /// Модель полей для окна задач
    /// </summary>
    public class FieldsTask : BaseTask
    {
        /// <summary>
        /// Свойство названия поля
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Свойство системного названия поля
        /// </summary>
        public string SysName { get; set; }

        /// <summary>
        /// Свойство значения поля
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Свойство размера поля
        /// </summary>
        public double SizeColumn { get; set; }
    }
}
