namespace ERP_Admin_Panel.Services.Database
{
    public interface IDbContextFactory
    {
        ApplicationContext Instance { get; }
        bool Create(string connectionString, DataProvider dataProvider);
        bool IsConnect();
        bool MigrationDataBase();
    }
}
