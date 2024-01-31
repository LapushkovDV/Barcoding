using ERP_Admin_Panel.Data.ViewModels;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;


namespace ERP_Admin_Panel.CodePages.ComponentsRazor.Roles
{
    public class AddRoleModel : ComponentBase
    {
        [Inject] public IModalService? ModalService { get; set; }
        [Inject] public ApplicationContext? ApplicationContext { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }

        protected RoleViewModel? RoleData { get; set; }

        protected override void OnInitialized() => RoleData = new RoleViewModel();

        protected void Done() => ModalService?.Close(ModalResult.Ok("Форма отправлена"));

        protected void Cancel()
        {
            ModalService?.Close(ModalResult.Cancel());
            Done();
        }

        protected void AddRole()
        {
            if (RoleData?.Name != string.Empty)
            {
                var role = new Role
                {
                    Name = RoleData.Name,
                    Description = RoleData.Description,
                };

                var added = ApplicationContext.Roles.Add(role);

                if (added != null && added.State == Microsoft.EntityFrameworkCore.EntityState.Added)
                {
                    ToastService?.ShowSuccess($"Роль \"{RoleData.Name}\" успешно добавлена");
                    ApplicationContext.SaveChanges();
                }
                else
                {
                    ToastService?.ShowError($"Роль \"{RoleData.Name}\" не добавлена");
                }
            }

            Done();
        }
    }
}