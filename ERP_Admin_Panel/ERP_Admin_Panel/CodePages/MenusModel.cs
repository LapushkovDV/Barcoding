using ERP_Admin_Panel.Pages.ComponentsRazor.Menus;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages
{
    public class MenusModel : ComponentBase
    {
        [Inject] protected ApplicationContext? ApplicationContext { get; set; }
        [Inject] public IModalService? ModalService { get; set; }

        protected List<Menu> Menus { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            Menus = await Task.FromResult(ApplicationContext.Menus.ToList());
        }

        protected void AddMenu() => ShowAddModal();

        protected void EditMenu(Menu menu) => ShowEditModal(menu);

        protected void RemoveMenu(Menu menu) => ShowRemoveModal(menu);

        private void ShowEditModal(Menu menu)
        {
            var parameters = new ModalParameters();

            parameters.Add(nameof(Menu), menu);

            ModalService.OnClose += ClosedEditModal;
            ModalService?.Show<EditMenu>("Редактирование меню", parameters);
        }

        private void ShowAddModal()
        {
            ModalService.OnClose += ClosedAddAndRemoveModal;
            ModalService?.Show<AddMenu>("Добавить новое меню", new ModalParameters());
        }

        private void ShowRemoveModal(Menu menu)
        {
            var parameters = new ModalParameters();

            parameters.Add(nameof(Menu), menu);

            ModalService.OnClose += ClosedAddAndRemoveModal;
            ModalService?.Show<RemoveMenu>("Подтверждение", parameters);
        }

        private void ClosedEditModal(ModalResult result)
        {
            Console.WriteLine("Окно закрыто");

            if (result.Cancelled)
            {
                Console.WriteLine("Редактирование отменено");
            }
            else
            {
                Console.WriteLine(result.Data.ToString());
                StateHasChanged();
            }

            ModalService.OnClose -= ClosedEditModal;
        }



        private async void ClosedAddAndRemoveModal(ModalResult result)
        {
            Console.WriteLine("Окно закрыто");

            if (result.Cancelled)
            {
                Console.WriteLine("Редактирование отменено");
            }
            else
            {
                Console.WriteLine(result.Data.ToString());

                Menus = await Task.FromResult(ApplicationContext.Menus.ToList());

                StateHasChanged();
            }

            ModalService.OnClose -= ClosedEditModal;

        }
    }
}
