using ERP_Admin_Panel.Data.ViewModels;
using ERP_Admin_Panel.Services.Configuration;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor
{
    public class ConnectDataBaseModel : ComponentBase
    {
        private string _dataBaseProvider = string.Empty;

        [Inject] protected IConfigurationManager ConfigurationManager { get; set; }
        [Inject] protected IModalService? ModalService { get; set; }
        [Inject] protected NavigationManager? NavigationManager { get; set; }
        [Inject] protected IDbContextFactory DbContextFactory { get; set; }


        protected ConnectDataBaseViewModel ConnectDataBaseViewModel { get; set; } = new ConnectDataBaseViewModel();
        protected string DataBaseProvider
        {
            get => _dataBaseProvider;
            set
            {
                _dataBaseProvider = value;

                ConnectDataBaseViewModel.DataProvider = ConfigurationManager.FormatTypeDataBase(value);
            }
        }

        protected override void OnInitialized()
        {
            DataBaseProvider = ConnectDataBaseViewModel.DataBaseProviders.FirstOrDefault();
        }

        protected void ConnectDataBase()
        {
            var host = ConnectDataBaseViewModel.DataProvider == DataProvider.SQLServer ?
                $"{ConnectDataBaseViewModel.Host.Trim()}\\{ConnectDataBaseViewModel.Instance.Trim()}" :
                ConnectDataBaseViewModel.Host.Trim();
            var connectionString = ConfigurationManager.FormatConnectionString(host,
                    ConnectDataBaseViewModel.UserName.Trim(),
                    ConnectDataBaseViewModel.Password,
                    ConnectDataBaseViewModel.DataBaseName.Trim(),
                    ConnectDataBaseViewModel.DataProvider,
                    ConnectDataBaseViewModel.Port);

            DbContextFactory.Create(connectionString, ConnectDataBaseViewModel.DataProvider);

            if (DbContextFactory.IsConnect())
            {
                ConfigurationManager.SetTypeDataBase(ConnectDataBaseViewModel.DataProvider);
                ConfigurationManager.SetConnectionString(connectionString);

                Done();

                NavigationManager?.NavigateTo("/login", true);
            }
            else
            {
                ConnectDataBaseViewModel.ErrorConnect = "Ошибка подключения к базе данных по строке подключения: \n" + connectionString;
            }


        }

        protected void Done() => ModalService?.Close(ModalResult.Ok("Форма отправлена"));
    }
}
