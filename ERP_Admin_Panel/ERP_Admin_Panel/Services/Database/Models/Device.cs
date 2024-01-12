namespace ERP_Admin_Panel.Services.Database.Models
{
    public class Device : BaseModel
    {
        public string DeviceCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;

        public List<User> Users { get; } = new();
    }
}
