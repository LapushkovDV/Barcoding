namespace ERP_Admin_Panel.Services.Database.Models
{
    public class Event : BaseModel
    {
        public int? UserId { get; set; }
        public User User { get; set; }

        public int? DeviceId { get; set; }
        public Device Device { get; set; }

        public DateTime DateTime { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
