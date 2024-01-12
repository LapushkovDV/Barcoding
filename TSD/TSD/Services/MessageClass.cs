namespace TSD.Services
{
    /// <summary>
    /// Класс-маркер для MessagingCenter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageClass<T>
    {
        /// <summary>
        /// Данные, которые нужно передать
        /// </summary>
        public T Data { get; }

        public MessageClass(T data = default)
        {
            Data = data;
        }
    }
}
