using ERP_Admin_Panel.Data.ViewModels;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor.Devices
{
    public class EditDeviceModel : ComponentBase
    {
        private Device? DeviceExternal;
        [CascadingParameter] ModalParameters? Parameters { get; set; }

        [Inject] public IModalService? ModalService { get; set; }
        [Inject] public ApplicationContext? ApplicationContext { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }

        protected DeviceViewModel? DeviceData { get; set; }

        protected override void OnInitialized()
        {
            DeviceExternal = Parameters?.Get<Device>(nameof(Device));
            DeviceData = new DeviceViewModel
            {
                DeviceCode = DeviceExternal.DeviceCode,
                Description = DeviceExternal.Description,
                IsActive = DeviceExternal.IsActive
            };
        }

        protected void EditDevice()
        {
            DeviceExternal.DeviceCode = DeviceData.DeviceCode;
            DeviceExternal.Description = DeviceData.Description;
            DeviceExternal.IsActive = DeviceData.IsActive;

            var updated = ApplicationContext.Update(DeviceExternal);

            if (updated != null && updated.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
            {
                ToastService?.ShowSuccess($"Устройство \"{DeviceData.DeviceCode}\" успешно обновлёно");
                ApplicationContext?.SaveChanges();
            }
            else
            {
                ToastService?.ShowError($"Устройство \"{DeviceData.DeviceCode}\" не обновлёно");
            }

            Done();
        }

        protected void Done()
        {
            ModalService?.Close(ModalResult.Ok("Форма отправлена"));
        }
    }
}
