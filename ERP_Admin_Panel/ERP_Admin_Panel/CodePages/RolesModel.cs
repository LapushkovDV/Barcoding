using ERP_Admin_Panel.Pages.ComponentsRazor.Roles;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using ERP_Admin_Panel.Services.NavBarActions;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages
{
    public class RolesModel : ComponentBase
    {
        [Inject] protected ApplicationContext? ApplicationContext { get; set; }
        [Inject] public IModalService? ModalService { get; set; }
        [Inject] protected INavBarOperation NavBarOperation { get; set; }

        protected List<Role> Roles { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            NavBarOperation.IsSearchVisible = false;

            Roles = await Task.FromResult(ApplicationContext.Roles.ToList());
        }

        protected void AddRole() => ShowAddModal();

        protected void EditRole(Role role) => ShowEditModal(role);

        protected void RemoveRole(Role role) => ShowRemoveModal(role);

        private void ShowEditModal(Role role)
        {
            var parameters = new ModalParameters();

            parameters.Add(nameof(Role), role);

            ModalService.OnClose += ClosedEditModal;
            ModalService?.Show<EditRole>("Редактирование роли", parameters);
        }

        private void ShowAddModal()
        {
            ModalService.OnClose += ClosedAddAndRemoveModal;
            ModalService?.Show<AddRole>("Добавить новую роль", new ModalParameters());
        }

        private void ShowRemoveModal(Role role)
        {
            var parameters = new ModalParameters();

            parameters.Add(nameof(Role), role);

            ModalService.OnClose += ClosedAddAndRemoveModal;
            ModalService?.Show<RemoveRole>("Подтверждение", parameters);
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

                Roles = await Task.FromResult(ApplicationContext.Roles.ToList());

                StateHasChanged();
            }

            ModalService.OnClose -= ClosedEditModal;

        }

    }
}
