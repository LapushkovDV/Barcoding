using ERP_Admin_Panel.Services.Database;

namespace ERP_Admin_Panel.Services.Configuration
{
    public interface IConfigurationManager
    {
        string GetConnectionString();
        DataProvider GetTypeDataBase();
        void SetTypeDataBase(DataProvider dataProvider);
        void SetConnectionString(string connectionString);
        string FormatConnectionString(string host, string userName, string password, string dataBase = "ERPDB", DataProvider dataProvider = DataProvider.SQLServer, string port = "5433");
        DataProvider FormatTypeDataBase(string typeDataBase);
    }
}
