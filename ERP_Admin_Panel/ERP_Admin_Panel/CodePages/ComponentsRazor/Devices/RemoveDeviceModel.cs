using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor.Devices
{
    public class RemoveDeviceModel : ComponentBase
    {
        protected Device? Device { get; set; }
        [CascadingParameter] protected ModalParameters? Parameters { get; set; }
        [Inject] protected ApplicationContext? ApplicationContext { get; set; }
        [Inject] public IModalService? ModalService { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }

        protected override void OnInitialized()
        {
            Device = Parameters?.TryGet<Device>(nameof(Device));
        }

        protected void Accept()
        {
            var removed = ApplicationContext?.Remove(Device);

            if (removed != null && removed.State == Microsoft.EntityFrameworkCore.EntityState.Deleted)
            {
                ToastService?.ShowSuccess($"Устройство \"{Device.DeviceCode}\" успешно удалено");
                ApplicationContext?.SaveChanges();
            }
            else
            {
                ToastService?.ShowError($"Устройство \"{Device.DeviceCode}\" не удалено");
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
