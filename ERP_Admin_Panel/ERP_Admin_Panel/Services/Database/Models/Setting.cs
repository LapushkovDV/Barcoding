namespace ERP_Admin_Panel.Services.Database.Models
{
    public class Setting : BaseModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
