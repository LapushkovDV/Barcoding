using ERP_Admin_Panel.Data.ViewModels;
using ERP_Admin_Panel.Pages.ComponentsRazor;
using ERP_Admin_Panel.Pages.ComponentsRazor.Users;
using ERP_Admin_Panel.Services.Configuration;
using ERP_Admin_Panel.Services.Cryptography;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.GenericRepository;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Log;
using ERP_Admin_Panel.Services.Modal;
using ERP_Admin_Panel.Services.Token;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.ProtectedBrowserStorage;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor
{
    public class LoginModel : ComponentBase
    {
        private GenericRepository<User> _genericRepositoryUser;
        [Inject] protected ProtectedSessionStorage? ProtectedSessionStore { get; set; }
        [Inject] protected NavigationManager? NavigationManager { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }
        [Inject] protected IModalService? ModalService { get; set; }
        [Inject] protected IConfigurationManager ConfigurationManager { get; set; }
        [Inject] protected IDbContextFactory DbContextFactory { get; set; }
        [Inject] protected ILogManager LogManager { get; set; }

        protected LoginViewModel LoginData { get; set; } = new LoginViewModel();
        protected string ErrorMessage { get; set; } = string.Empty;

        protected async Task LoginAsync()
        {
            var hashPassword = CryptographyHash.ComputeSHA256(LoginData.Password);
            var user = _genericRepositoryUser.Get(x => x.Login == LoginData.Login && x.Password == hashPassword, x => x.RoleUser);

            if (user == null)
            {
                ErrorMessage = "Такой пользователь не зарегистрирован в базе данных";
                LogManager.WriteEvent($"WebAdmin login", $"Неудачная попытка входа с логином \"{LoginData.Login}\"");
                ToastService?.ShowError(ErrorMessage);
                return;
            }

            if (user.RoleUser.Tag > LevelRange.UserTSD)
            {
                var token = new SecurityToken()
                {
                    UserId = user.Id,
                    RoleId = user.RoleUserId,
                    UserName = user.Login,
                    Role = user.RoleUser.Name,
                    AccessToken = JwtTokenSecurity.CreateJwt(user.Login + user.CreateDate + user.RoleUser.Name)
                };


                if (ProtectedSessionStore != null)
                {
                    await ProtectedSessionStore.SetAsync((nameof(SecurityToken)), token);
                    LogManager.WriteEvent($"WebAdmin login", $"Пользователь \"{token.UserName}\" вошел в панель администратора");
                    NavigationManager?.NavigateTo("/home", true);
                }
            }
            else
            {
                ErrorMessage = $"Пользователь {user.Login} не является администратором системы";

                ToastService?.ShowToast(Services.Toast.ToastLevel.Warning, ErrorMessage);
            }
        }

        private void ShowAddSuperAdminModal()
        {
            var parameters = new ModalParameters();
            var options = new ModalOptions
            {
                DisableBackgroundCancel = true,
                HideCloseButton = true
            };

            ModalService.OnClose += ClosedAddSuperAdminModal;
            ModalService?.Show<AddSuperUser>("Добавление суперадминистратора", parameters, options);
        }

        private void ShowConnectDataBaseModal()
        {
            var parameters = new ModalParameters();
            var options = new ModalOptions
            {
                DisableBackgroundCancel = true,
                HideCloseButton = true
            };

            ModalService.OnClose += ClosedAddSuperAdminModal;
            ModalService?.Show<ConnectDataBase>("Подключение к базе данных...", parameters, options);
        }

        private void ClosedAddSuperAdminModal(ModalResult result)
        {
            Console.WriteLine("Окно закрыто");

            if (result.Cancelled)
            {
                Console.WriteLine("Редактирование отменено");
            }
            else
            {
                Console.WriteLine(result.Data.ToString());
                StateHasChanged();
            }

            ModalService.OnClose -= ClosedAddSuperAdminModal;
        }

        protected override void OnInitialized()
        {
            InitDataBase();

            if (DbContextFactory.IsConnect())
            {
                LoginData = new LoginViewModel();
                _genericRepositoryUser = new(DbContextFactory.Instance);

                Services.Database.Mocks.MocksData.RoleUserAdd(DbContextFactory.Instance);

                if (_genericRepositoryUser.GetAll().Count == 0)
                {
                    ShowAddSuperAdminModal();
                }
            }
            else
            {
                ShowConnectDataBaseModal();
            }
        }

        protected void InitDataBase()
        {
            if (DbContextFactory.Instance != null) return;

            var connectionString = ConfigurationManager.GetConnectionString();
            var typeDataBase = ConfigurationManager.GetTypeDataBase();

            DbContextFactory.Create(connectionString, typeDataBase);
        }
    }
}
