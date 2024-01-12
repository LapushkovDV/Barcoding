using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor.Menus
{
    public class RemoveMenuModel : ComponentBase
    {
        protected Menu? Menu { get; set; }
        [CascadingParameter] protected ModalParameters? Parameters { get; set; }
        [Inject] protected ApplicationContext? ApplicationContext { get; set; }
        [Inject] public IModalService? ModalService { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }

        protected override void OnInitialized() => Menu = Parameters?.TryGet<Menu>(nameof(Menu));

        protected void RemoveMenu()
        {
            var removed = ApplicationContext?.Remove(Menu);
            if (removed != null && removed.State == Microsoft.EntityFrameworkCore.EntityState.Deleted)
            {
                ToastService?.ShowSuccess($"Меню \"{Menu.Name}\" успешно удалено");
                ApplicationContext?.SaveChanges();
            }
            else
            {
                ToastService?.ShowError($"Меню \"{Menu.Name}\" не удалено");
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
