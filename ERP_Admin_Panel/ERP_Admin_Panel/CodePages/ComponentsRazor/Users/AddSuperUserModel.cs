using ERP_Admin_Panel.Data.ViewModels;
using ERP_Admin_Panel.Services.Cryptography;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.GenericRepository;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor.Users
{
    public class AddSuperUserModel : ComponentBase
    {
        private GenericRepository<RoleUser> _genericRepositoryRoleUser;
        private GenericRepository<User> _genericRepositoryUser;
        [Inject] protected IDbContextFactory DbContextFactory { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }
        [Inject] public IModalService? ModalService { get; set; }
        public UserViewModel UserData { get; set; } = new UserViewModel();

        protected override void OnInitialized()
        {
            _genericRepositoryRoleUser = new GenericRepository<RoleUser>(DbContextFactory.Instance);
            _genericRepositoryUser = new GenericRepository<User>(DbContextFactory.Instance);
        }

        protected void AddSuperUser()
        {
            if (UserData.Login != string.Empty && UserData.Password != string.Empty && UserData.Password == UserData.ConfirmPassword)
            {
                var user = new User
                {
                    Login = UserData.Login,
                    Password = CryptographyHash.ComputeSHA256(UserData.Password),
                    Description = UserData.Description,
                    IsActive = false,
                    RoleUserId = _genericRepositoryRoleUser.Get(x => x.Tag == LevelRange.SuperAdmin).Id,
                    CreateDate = DateTime.Now.Date
                };

                var added = _genericRepositoryUser.Create(user);

                if (added)
                {
                    ToastService?.ShowSuccess($"Суперадминистратор \"{UserData.Login}\" успешно добавлен");

                    Done();
                }
                else
                {
                    ToastService?.ShowError($"Суперадминистратор \"{UserData.Login}\" не добавлен");
                }
            }
        }

        protected void Done() => ModalService?.Close(ModalResult.Ok("Форма отправлена"));
    }
}
