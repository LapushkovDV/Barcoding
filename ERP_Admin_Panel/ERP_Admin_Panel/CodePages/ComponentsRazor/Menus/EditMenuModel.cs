using ERP_Admin_Panel.Data.ViewModels;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor.Menus
{
    public class EditMenuModel : ComponentBase
    {
        private Menu? MenuExternal;
        [CascadingParameter] ModalParameters? Parameters { get; set; }

        [Inject] public IModalService? ModalService { get; set; }
        [Inject] public ApplicationContext? ApplicationContext { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }

        protected MenuViewModel? MenuData { get; set; }

        protected override void OnInitialized()
        {
            MenuExternal = Parameters?.Get<Menu>(nameof(Menu));
            MenuData = new MenuViewModel
            {
                Name = MenuExternal.Name,
                Description = MenuExternal.Description,
                Action = MenuExternal.Action,
                Npp = MenuExternal.Npp
            };

        }

        protected void EditMenu()
        {
            MenuExternal.Name = MenuData.Name;
            MenuExternal.Description = MenuData.Description;
            MenuExternal.Action = MenuData.Action;
            MenuExternal.Npp = MenuData.Npp;

            var updated = ApplicationContext.Update(MenuExternal);

            if (updated != null && updated.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
            {
                ToastService?.ShowSuccess($"Меню \"{MenuData.Name}\" успешно обновлёно");
                ApplicationContext?.SaveChanges();
            }
            else
            {
                ToastService?.ShowError($"Меню \"{MenuData.Name}\" не обновлёно");
            }

            Done();
        }

        protected void Done()
        {
            ModalService?.Close(ModalResult.Ok("Форма отправлена"));
        }
    }
}
