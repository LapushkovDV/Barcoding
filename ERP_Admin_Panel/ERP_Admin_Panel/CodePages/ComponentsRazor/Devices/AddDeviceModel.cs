using ERP_Admin_Panel.Data.ViewModels;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;


namespace ERP_Admin_Panel.CodePages.ComponentsRazor.Devices
{
    public class AddDeviceModel : ComponentBase
    {
        [Inject] public IModalService? ModalService { get; set; }
        [Inject] public ApplicationContext? ApplicationContext { get; set; }
        [Inject] protected IToastService? ToastService { get; set; }

        protected DeviceViewModel? DeviceData { get; set; }

        protected override void OnInitialized() => DeviceData = new DeviceViewModel();

        protected void Done() => ModalService?.Close(ModalResult.Ok("Форма отправлена"));

        protected void Cancel()
        {
            ModalService?.Close(ModalResult.Cancel());
            Done();
        }

        protected void AddDevice()
        {
            if (DeviceData?.DeviceCode != string.Empty)
            {
                var device = new Device
                {
                    DeviceCode = DeviceData.DeviceCode,
                    Description = DeviceData.Description,
                    IsActive = DeviceData.IsActive,
                };

                var added = ApplicationContext.Devices.Add(device);

                if (added != null && added.State == Microsoft.EntityFrameworkCore.EntityState.Added)
                {
                    ToastService?.ShowSuccess($"Устройство \"{DeviceData.DeviceCode}\" успешно добавлено");
                    ApplicationContext.SaveChanges();
                }
                else
                {
                    ToastService?.ShowError($"Устройство \"{DeviceData.DeviceCode}\" не добавлено");
                }
            }

            Done();
        }
    }
}