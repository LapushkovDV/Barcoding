using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.GenericRepository;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor.Users
{
    public class RemoveUserModel : ComponentBase
    {
        private GenericRepository<User> _genericRepositoryUser;

        protected User User { get; set; } = new();
        [CascadingParameter] protected ModalParameters? Parameters { get; set; }
        [Inject] protected ApplicationContext? ApplicationContext { get; set; }
        [Inject] public IModalService? ModalService { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }

        protected override void OnInitialized()
        {
            _genericRepositoryUser = new(ApplicationContext);

            User = Parameters?.TryGet<User>(nameof(User));
        }

        protected void Accept()
        {
            var removed = _genericRepositoryUser.Delete(User);

            if (removed)
            {
                ToastService?.ShowSuccess($"Пользователь \"{User.Login}\" успешно удален" );
            }
            else
            {
                ToastService?.ShowError($"Пользователь \"{User.Login}\" не удален");
            }

            Done();
        }

        protected void Cancel()
        {
            ModalService?.Close(ModalResult.Cancel());
            Done();
        }

        private void Done()
        {
            ModalService?.Close(ModalResult.Ok("Форма отправлена"));
        }
    }
}
