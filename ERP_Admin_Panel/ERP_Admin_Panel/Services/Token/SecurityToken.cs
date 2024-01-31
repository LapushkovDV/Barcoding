namespace ERP_Admin_Panel.Services.Token
{
    public class SecurityToken
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string AccessToken { get; set;} = string.Empty;
    }
}
