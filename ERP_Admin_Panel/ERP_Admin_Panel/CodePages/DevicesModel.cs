using ERP_Admin_Panel.Pages.ComponentsRazor.Devices;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages
{
    public class DevicesModel : ComponentBase
    {
        [Inject] protected ApplicationContext? ApplicationContext { get; set; }
        [Inject] public IModalService? ModalService { get; set; }

        protected List<Device> Devices { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            Devices = await Task.FromResult(ApplicationContext.Devices.ToList());
        }

        protected void AddDevice() => ShowAddModal();

        protected void EditDevice(Device device) => ShowEditModal(device);

        protected void RemoveDevice(Device device) => ShowRemoveModal(device);

        private void ShowEditModal(Device device)
        {
            var parameters = new ModalParameters();

            parameters.Add(nameof(Device), device);

            ModalService.OnClose += ClosedEditModal;
            ModalService?.Show<EditDevice>("Редактирование данных устройства", parameters);
        }

        private void ShowAddModal()
        {
            ModalService.OnClose += ClosedAddAndRemoveModal;
            ModalService?.Show<AddDevice>("Добавить новое устройство", new ModalParameters());
        }

        private void ShowRemoveModal(Device device)
        {
            var parameters = new ModalParameters();

            parameters.Add(nameof(Device), device);

            ModalService.OnClose += ClosedAddAndRemoveModal;
            ModalService?.Show<RemoveDevice>("Подтверждение", parameters);
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

                Devices = await Task.FromResult(ApplicationContext.Devices.ToList());

                StateHasChanged();
            }

            ModalService.OnClose -= ClosedEditModal;

        }
    }
}
