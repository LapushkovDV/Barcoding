using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using ERP_Admin_Panel.Services.Token;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.ProtectedBrowserStorage;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor
{
    public class SignOutModel : ComponentBase
    {
        [Inject] public IModalService? ModalService { get; set; }
        [Inject] public ProtectedSessionStorage? ProtectedSessionStore { get;set; }
        [Inject] public NavigationManager? NavigationManager { get; set; }

        protected async Task Accept()
        {
            if (ProtectedSessionStore is null) return;

            await ProtectedSessionStore.DeleteAsync(nameof(SecurityToken));
            Done();
            NavigationManager?.NavigateTo("/login");
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
