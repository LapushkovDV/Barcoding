using ERP_Admin_Panel.Services.Database.Models;

namespace ERP_Admin_Panel.Services.Log
{
    public interface ILogManager
    {
        void WriteEvent(string eventType, string message, int? userId = null, int? deviceId = null);
        List<Event> ReadIntervalEvents(DateTime startDateTime, DateTime endDateTime);
    }
}
