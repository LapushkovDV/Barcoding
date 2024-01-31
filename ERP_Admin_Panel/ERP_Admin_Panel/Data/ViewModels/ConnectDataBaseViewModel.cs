using ERP_Admin_Panel.Services.Database;

namespace ERP_Admin_Panel.Data.ViewModels
{
    public class ConnectDataBaseViewModel
    {
        public string Host { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
        public string DataBaseName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DataProvider DataProvider { get; set; } = DataProvider.SQLServer;
        public bool IsDisabledButton { get; set; } = false;
        public List<string> DataBaseProviders { get; set; } = new List<string> { @"SQLServer", @"PostgreSQL" };
        public string ErrorConnect { get; set; }
    }
}
