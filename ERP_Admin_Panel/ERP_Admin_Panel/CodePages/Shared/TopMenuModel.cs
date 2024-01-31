using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Token;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.ProtectedBrowserStorage;
using ERP_Admin_Panel.Pages.ComponentsRazor;
using ERP_Admin_Panel.Services.Modal;
using ERP_Admin_Panel.Data.ViewModels;
using ERP_Admin_Panel.Services.NavBarActions;

namespace ERP_Admin_Panel.CodePages.Shared
{
    public class TopMenuModel : ComponentBase
    {
        [Inject] protected INavBarOperation NavBarOperation { get; set; }
        [Inject] public ProtectedSessionStorage? ProtectedSessionStore { get; set; }
        [Inject] public IModalService? ModalService { get; set; }

        protected AccountViewModel Account { get; set; } = new AccountViewModel();

        protected override async Task OnInitializedAsync()
        {
            Account = new AccountViewModel();

            if (ProtectedSessionStore != null)
            {
                var token = await ProtectedSessionStore.GetAsync<SecurityToken>(nameof(SecurityToken));

                if (token.Value != null)
                {
                    Account.Name = token.Value.UserName;
                    Account.Role = token.Value.Role;

                    StateHasChanged();
                }
            }
        }

        protected void SignOut()
        {
            ModalService.OnClose += ClosedSignOutModal;
            ModalService?.Show<SignOut>("Подтверждение", new ModalParameters());
        }

        private void ClosedSignOutModal(ModalResult result)
        {
            Console.WriteLine("Окно закрыто");

            if (result.Cancelled)
            {
                Console.WriteLine("Редактирование отменено");
            }
            else
            {
                Console.WriteLine(result.Data.ToString());
            }


            StateHasChanged();

            ModalService.OnClose -= ClosedSignOutModal;

        }
    }
}
