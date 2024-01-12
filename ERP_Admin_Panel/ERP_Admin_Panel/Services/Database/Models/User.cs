namespace ERP_Admin_Panel.Services.Database.Models
{
    public class User : BaseModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime? CreateDate { get; set; }

        public int RoleUserId { get; set; }
        public RoleUser RoleUser { get; set; }

        public List<Device> Devices { get; } = new();

        public User Clone() => new()
        {
            Id = Id,
            Login = Login,
            Password = Password,
            Description = Description,
            IsActive = IsActive,
            RoleUser = RoleUser,
            CreateDate = CreateDate
        };
    }
}