namespace TSD.Services.Tasks.Models
{
    /// <summary>
    /// Модель заголовков полей в окне задач
    /// </summary>
    public class HeaderValue : BaseTask
    {
        /// <summary>
        /// Свойство названия поля
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Свойство размера поля
        /// </summary>
        public double SizeColumn { get; set; }
    }
}
