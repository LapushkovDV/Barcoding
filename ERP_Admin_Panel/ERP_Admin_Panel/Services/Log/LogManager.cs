using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.GenericRepository;
using ERP_Admin_Panel.Services.Database.Models;

namespace ERP_Admin_Panel.Services.Log
{
    /// <summary>
    /// Класс для работы с логом БД
    /// </summary>
    public class LogManager : ILogManager
    {
        private readonly IDbContextFactory _contextFactory;
        private IGenericRepository<Event> _genericRepositoryEvents;
        public LogManager(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Запись события в БД
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        public void WriteEvent(string eventType, string message, int? userId = null, int? deviceId = null)
        {
            _genericRepositoryEvents = new GenericRepository<Event>(_contextFactory.Instance);

            if (_contextFactory != null && _contextFactory.IsConnect())
            {
                var eventValue = new Event
                {
                    DateTime = DateTime.Now,
                    Type = eventType,
                    Message = message,
                    UserId = userId,
                    DeviceId = deviceId
                };

                _genericRepositoryEvents.Create(eventValue);
            }
        }

        /// <summary>
        /// Вывод лога по интервалу
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns>Список событий, входящие в интервал</returns>
        public List<Event> ReadIntervalEvents(DateTime startDateTime, DateTime endDateTime)
        {
            _genericRepositoryEvents = new GenericRepository<Event>(_contextFactory.Instance);

            if (_contextFactory != null && _contextFactory.IsConnect())
            {
                return _genericRepositoryEvents.GetAll(x => x.DateTime >= startDateTime && x.DateTime <= endDateTime, x => x.OrderBy(element => element.DateTime), x => x.User, x => x.Device);
            }

            return null;
        }
    }
}
