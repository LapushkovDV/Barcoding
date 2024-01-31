using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor
{
    public class BlazoredViewModal : ComponentBase, IDisposable
    {
        const string _defaultStyle = "blazored-modal";
        const string _defaultPosition = "blazored-modal-center";
        [Inject] public IModalService? ModalService { get; set; }

        [Parameter] public bool HideHeader { get; set; }
        [Parameter] public bool HideCloseButton { get; set; }
        [Parameter] public bool DisableBackgroundCancel { get; set; }
        [Parameter] public string Position { get; set; } = string.Empty;
        [Parameter] public string Style { get; set; } = string.Empty;

        protected bool ComponentDisableBackgroundCancel { get; set; }
        protected bool ComponentHideHeader { get; set; }
        protected bool ComponentHideCloseButton { get; set; }
        protected string ComponentPosition { get; set; } = string.Empty;
        protected string ComponentStyle { get; set; } = string.Empty;
        protected bool IsVisible { get; set; }
        protected string Title { get; set; } = string.Empty;

        protected RenderFragment? Content { get; set; }
        protected ModalParameters? Parameters { get; set; }

        protected override void OnInitialized()
        {
            SetModalOptions(new ModalOptions() { Position = "blazored-modal-center", Style= "blazored-modal" });

            ((ModalService)ModalService).OnShow += ShowModal;
            ((ModalService)ModalService).CloseModal += CloseModal;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((ModalService)ModalService).OnShow -= ShowModal;
                ((ModalService)ModalService).CloseModal -= CloseModal;
            }
        }

        private async void ShowModal(string title, RenderFragment renderFragment, ModalParameters parameters, ModalOptions options)
        {
            Title = title;
            Content = renderFragment;
            Parameters = parameters;
            SetModalOptions(options);
            IsVisible = true;

            await InvokeAsync(StateHasChanged);
        }

        private async void CloseModal()
        {
            Title = string.Empty;
            Content = null;
            SetModalOptions(new ModalOptions());
            IsVisible = false;

            await InvokeAsync(StateHasChanged);
        }

        protected void HandleBackgroundClick()
        {
            if (ComponentDisableBackgroundCancel) return;

            ModalService?.Cancel();
        }

        protected void SetModalOptions(ModalOptions options)
        {
            ComponentHideHeader = HideHeader;

            if (options.HideHeader.HasValue)
                ComponentHideHeader = options.HideHeader.Value;

            ComponentDisableBackgroundCancel = DisableBackgroundCancel;

            if (options.DisableBackgroundCancel.HasValue)
                ComponentDisableBackgroundCancel= options.DisableBackgroundCancel.Value;

            ComponentHideCloseButton = HideCloseButton;

            if (options.HideCloseButton.HasValue)
                ComponentHideCloseButton= options.HideCloseButton.Value;

            ComponentPosition = string.IsNullOrWhiteSpace(options.Position) ? Position : options.Position;

            if (string.IsNullOrWhiteSpace(ComponentPosition))
                ComponentPosition = _defaultPosition;

            ComponentStyle = string.IsNullOrWhiteSpace(options.Style) ? Style : options.Style;

            if (string.IsNullOrWhiteSpace(ComponentStyle))
                ComponentStyle = _defaultStyle;
        }

        public void SetTitle(string title)
        {
            Title = title;

            StateHasChanged();
        }
    }
}
