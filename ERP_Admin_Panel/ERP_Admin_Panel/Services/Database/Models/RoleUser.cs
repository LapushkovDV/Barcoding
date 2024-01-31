namespace ERP_Admin_Panel.Services.Database.Models
{
    public class RoleUser : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public LevelRange Tag { get; set; }

        public List<User> Users { get; set; }
    }
}
