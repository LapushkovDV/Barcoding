namespace ERP_Admin_Panel.Services.Account
{
    public interface IAccountManager
    {
        Task<int> GetRoleId();
        Task<string> GetRoleName();
        Task<int> GetUserId();
        Task<string> GetUserName();
    }
}
