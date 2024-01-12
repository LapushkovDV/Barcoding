using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor.Roles
{
    public class RemoveRoleModel : ComponentBase
    {
        protected Role? Role { get; set; }
        [CascadingParameter] protected ModalParameters? Parameters { get; set; }
        [Inject] protected ApplicationContext? ApplicationContext { get; set; }
        [Inject] public IModalService? ModalService { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }

        protected override void OnInitialized()
        {
            Role = Parameters?.TryGet<Role>(nameof(Role));
        }

        protected void RemoveRole()
        {
            var removed = ApplicationContext?.Remove(Role);

            if (removed != null && removed.State == Microsoft.EntityFrameworkCore.EntityState.Deleted)
            {
                ToastService?.ShowSuccess($"Роль \"{Role.Name}\" успешно удалена");
                ApplicationContext?.SaveChanges();
            }
            else
            {
                ToastService?.ShowError($"Роль \"{Role.Name}\" не удалена");
            }
            ApplicationContext?.SaveChanges();
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
