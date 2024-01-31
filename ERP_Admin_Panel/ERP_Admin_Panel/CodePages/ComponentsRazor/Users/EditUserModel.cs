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
    public class EditUserModel : ComponentBase
    {
        private GenericRepository<RoleUser> _genericRepositoryRoleUser;
        private GenericRepository<User> _genericRepositoryUser;
        private User _userExternal = new();
        private int _currentRoleId;
        private LevelRange? _currentRole;

        [CascadingParameter] ModalParameters? Parameters { get; set; }

        [Inject] protected IModalService? ModalService { get; set; }
        [Inject] protected ApplicationContext? ApplicationContext { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }
        [Inject] protected IAccountManager? AccountManager { get; set; }

        protected int RoleId { get; set; }
        protected bool IsChangePassword { get; set; } = false;
        protected UserViewModel? UserData { get; set; } = new UserViewModel();
        protected List<RoleUser> RoleUsers { get; set; } = new List<RoleUser>();

        protected override async Task OnInitializedAsync()
        {
            _currentRoleId = await AccountManager.GetRoleId();

            _genericRepositoryRoleUser = new(ApplicationContext);
            _genericRepositoryUser = new(ApplicationContext);


            _userExternal = Parameters.Get<User>(nameof(User));
            UserData = new UserViewModel
            {
                Login = _userExternal.Login,
                Description = _userExternal.Description,
                IsActive = _userExternal.IsActive,
                RoleId = _userExternal.RoleUserId,
                IsEdit = true
            };

            if (_currentRoleId != -1)
            {
                _currentRole = _genericRepositoryRoleUser.Get(x => x.Id == _currentRoleId).Tag;

                RoleUsers = _genericRepositoryRoleUser.GetAll(x => x.Tag < _currentRole);

                if (RoleUsers != null && RoleUsers.Count != 0)
                {
                    RoleId = UserData.RoleId;
                }
            }
        }

        protected void EditUser()
        {
            _userExternal.Login = UserData.Login;
            _userExternal.Description = UserData.Description;
            _userExternal.IsActive = UserData.IsActive;
            _userExternal.RoleUserId = RoleId;

            var updated = _genericRepositoryUser.Update(_userExternal);

            if (updated)
            {
                ToastService?.ShowSuccess($"Пользователь \"{UserData.Login}\" успешно обновлён");
            }
            else
            {
                ToastService?.ShowError($"Пользователь \"{UserData.Login}\" не обновлён");
            }

            Done();
        }

        protected void ChangePassword()
        {
            IsChangePassword = true;
            StateHasChanged();
        }
        protected void CancelChangePassword()
        {

            _userExternal.Password = UserData.Password = string.Empty;
            UserData.ConfirmPassword = string.Empty;
            IsChangePassword = false;

            StateHasChanged();
        }

        protected void AcceptChangePassword()
        {
            if (UserData.Password == string.Empty || UserData.Password != UserData.ConfirmPassword) return;

            _userExternal.Password = CryptographyHash.ComputeSHA256(UserData.Password);

            var updated = ApplicationContext.Update(_userExternal);

            if (updated != null && updated.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
            {
                ToastService?.ShowSuccess($"Пароль пользователя \"{UserData.Login}\" успешно изменён");
                ApplicationContext?.SaveChanges();

                _userExternal.Password = UserData.Password = string.Empty;
                UserData.ConfirmPassword = string.Empty;
                IsChangePassword = false;
                StateHasChanged();
            }
            else
            {
                ToastService?.ShowError($"Пароль пользователя \"{UserData.Login}\" не изменён");
            }
        }

        protected void Done()
        {
            ModalService?.Close(ModalResult.Ok("Форма отправлена"));
        }
    }
}
