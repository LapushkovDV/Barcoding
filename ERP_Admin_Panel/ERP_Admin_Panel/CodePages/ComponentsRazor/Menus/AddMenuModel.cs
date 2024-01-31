using ERP_Admin_Panel.Data.ViewModels;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor.Menus
{
    public class AddMenuModel : ComponentBase
    {
        [Inject] public IModalService? ModalService { get; set; }
        [Inject] public ApplicationContext? ApplicationContext { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }

        protected MenuViewModel? MenuData { get; set; }

        protected override void OnInitialized() => MenuData = new MenuViewModel();

        protected void Done() => ModalService?.Close(ModalResult.Ok("Форма отправлена"));

        protected void Cancel()
        {
            ModalService?.Close(ModalResult.Cancel());
            Done();
        }

        protected void AddMenu()
        {
            if (MenuData.Name != string.Empty && MenuData.Action != string.Empty)
            {
                var menu = new Menu
                {
                    Name = MenuData.Name,
                    Action = MenuData.Action,
                    Description = MenuData.Description,
                    Npp = MenuData.Npp
                };

                var added = ApplicationContext.Menus.Add(menu);

                if (added != null && added.State == Microsoft.EntityFrameworkCore.EntityState.Added)
                {
                    ToastService?.ShowSuccess($"Меню \"{MenuData.Name}\" успешно добавлено");
                    ApplicationContext.SaveChanges();
                }
                else
                {
                    ToastService?.ShowError($"Меню \"{MenuData.Name}\" не добавлено");
                }
            }

            Done();
        }
    }
}
