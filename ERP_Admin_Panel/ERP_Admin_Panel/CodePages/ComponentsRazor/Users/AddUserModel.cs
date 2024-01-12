using ERP_Admin_Panel.Data.ViewModels;
using ERP_Admin_Panel.Services.Account;
using ERP_Admin_Panel.Services.Cryptography;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.GenericRepository;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor.Users
{
    public class AddUserModel : ComponentBase
    {
        private GenericRepository<RoleUser> _genericRepositoryRoleUser;
        private GenericRepository<User> _genericRepositoryUser;
        private int _currentRoleId;
        private LevelRange? _currentRole;

        [Inject] protected IModalService? ModalService { get; set; }
        [Inject] protected ApplicationContext? ApplicationContext { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }
        [Inject] protected IAccountManager AccountManager { get; set; }

        protected int RoleId { get; set; }
        protected UserViewModel? UserData { get; set; } = new();
        protected List<RoleUser> RoleUsers { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            _currentRoleId = await AccountManager.GetRoleId();

            _genericRepositoryRoleUser = new(ApplicationContext);
            _genericRepositoryUser = new(ApplicationContext);

            UserData = new UserViewModel() { IsEdit = false };

            if (_currentRoleId != -1)
            {
                _currentRole = _genericRepositoryRoleUser.Get(x => x.Id == _currentRoleId).Tag;
                RoleUsers = _genericRepositoryRoleUser.GetAll(x => x.Tag < _currentRole);

                if (RoleUsers != null && RoleUsers.Count != 0)
                {
                    RoleId = RoleUsers.FirstOrDefault().Id;
                }
            }
        }

        protected void Done() => ModalService?.Close(ModalResult.Ok("Форма отправлена"));

        protected void Cancel()
        {
            ModalService?.Close(ModalResult.Cancel());
            Done();
        }

        protected void AddUser()
        {
            var user = new User();
            if (UserData.Login != string.Empty && UserData.Password != string.Empty && UserData.Password == UserData.ConfirmPassword)
            {
                user = _genericRepositoryUser.Get(x => x.Login == UserData.Login);

                if (user == null)
                {
                    user = new User
                    {
                        Login = UserData.Login,
                        Password = CryptographyHash.ComputeSHA256(UserData.Password),
                        Description = UserData.Description,
                        IsActive = UserData.IsActive,
                        RoleUserId = RoleId,
                        CreateDate = DateTime.Now.Date
                    };

                    var added = _genericRepositoryUser.Create(user);

                    if (added)
                    {
                        ToastService?.ShowSuccess($"Пользователь \"{UserData.Login}\" успешно добавлен");

                        Done();
                    }
                    else
                    {
                        ToastService?.ShowError($"Пользователь \"{UserData.Login}\" не добавлен");
                    }
                }
                else
                {
                    ToastService?.ShowWarning($"Пользователь \"{UserData.Login}\" уже существует");
                }
            }
        }
    }
}
