using ERP_Admin_Panel.Data.ViewModels;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor.Roles
{
    public class EditRoleModel : ComponentBase
    {
        private Role? RoleExternal;
        [CascadingParameter] ModalParameters? Parameters { get; set; }

        [Inject] public IModalService? ModalService { get; set; }
        [Inject] public ApplicationContext? ApplicationContext { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }

        protected RoleViewModel? RoleData { get; set; }

        protected override void OnInitialized()
        {
            RoleExternal = Parameters?.Get<Role>(nameof(Role));
            RoleData = new RoleViewModel
            {
                Name = RoleExternal.Name,
                Description = RoleExternal.Description,
            };
        }

        protected void EditRole()
        {
            RoleExternal.Name = RoleData.Name;
            RoleExternal.Description = RoleData.Description;

            var updated = ApplicationContext.Update(RoleExternal);

            if (updated != null && updated.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
            {
                ToastService?.ShowSuccess($"Роль \"{RoleData.Name}\" успешно обновлена");
                ApplicationContext?.SaveChanges();
            }
            else
            {
                ToastService?.ShowError($"Роль \"{RoleData.Name}\" не обновлёна");
            }

            Done();
        }

        protected void Done()
        {
            ModalService?.Close(ModalResult.Ok("Форма отправлена"));
        }
    }
}
