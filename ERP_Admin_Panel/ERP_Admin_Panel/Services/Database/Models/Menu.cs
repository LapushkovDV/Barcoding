namespace ERP_Admin_Panel.Services.Database.Models
{
    public class Menu : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Npp { get; set; }
    }
}
