using ERP_Admin_Panel.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.Services.Modal
{
    public class ModalService : IModalService
    {
        public event Action<ModalResult> OnClose;
        internal event Action CloseModal;
        internal event Action<string, RenderFragment, ModalParameters, ModalOptions> OnShow;
        private Type _modalType;

        public void Cancel()
        {
            CloseModal?.Invoke();
            OnClose?.Invoke(ModalResult.Cancel(_modalType));
        }

        public void Close(ModalResult modalResult)
        {
            modalResult.ModalType = _modalType;
            CloseModal?.Invoke();
            OnClose?.Invoke(modalResult);
        }

        public void Show<T>(string title, ModalParameters parameters) where T : ComponentBase
        {
            Show<T>(title, parameters, new ModalOptions());
        }

        public void Show<T>(string title, ModalParameters parameters = null, ModalOptions options = null) where T : ComponentBase
        {
            Show(typeof(T), title, parameters, options);
        }

        public void Show(Type contentComponent, string title, ModalParameters parameters, ModalOptions options)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(contentComponent))
            {
                throw new ArgumentException("Не является компонентом Blazor");
            }
            var content = new RenderFragment(x =>
            {
                x.OpenComponent(1, contentComponent);
                x.CloseComponent();
            });
            _modalType = contentComponent;

            OnShow?.Invoke(title, content, parameters, options);
        }
    }
}
