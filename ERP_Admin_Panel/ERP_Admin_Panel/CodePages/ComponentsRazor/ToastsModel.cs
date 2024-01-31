using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Toast;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor
{
    public partial class ToastsModel : ComponentBase
    {
        internal List<ToastInstance> ToastList { get; set; } = new List<ToastInstance>();

        [Inject] public IToastService? ToastService { get; set; }

        [Parameter] public string InfoClass { get; set; } = string.Empty;
        [Parameter] public string InfoIconClass { get; set; } = string.Empty;
        [Parameter] public string SuccessClass { get; set; } = string.Empty;
        [Parameter] public string SuccessIconClass { get; set; } = string.Empty;
        [Parameter] public string WarningClass { get; set; } = string.Empty;
        [Parameter] public string WarningIconClass { get; set; } = string.Empty;
        [Parameter] public string ErrorClass { get; set; } = string.Empty;
        [Parameter] public string ErrorIconClass { get; set; } = string.Empty;
        [Parameter] public ToastPosition Position { get; set; } = ToastPosition.TopRight;
        [Parameter] public int Timeout { get; set; } = 5;

        public string PositionClass { get; set; } = string.Empty;


        public void RemoveToast(Guid toastId)
        {
            InvokeAsync(() =>
            {
                var toastInstance = ToastList.SingleOrDefault(x => x.Id == toastId);

                if (toastInstance != null)
                {
                    ToastList.Remove(toastInstance);
                    StateHasChanged();
                }
            });
        }

        protected override void OnInitialized()
        {
            ToastService.OnShow += ShowToast;
            PositionClass = $"position-{Position.ToString().ToLower()}";
        }

        private ToastSetting BuildToastSettings(ToastLevel level, string message, string heading)
        {
            return level switch
            {
                ToastLevel.Error => new ToastSetting(string.IsNullOrWhiteSpace(heading) ? "Ошибка" : heading, message, "blazored-toast-error", ErrorClass, ErrorIconClass),
                ToastLevel.Info => new ToastSetting(string.IsNullOrWhiteSpace(heading) ? "Информация" : heading, message, "blazored-toast-info", InfoClass, InfoIconClass),
                ToastLevel.Success => new ToastSetting(string.IsNullOrWhiteSpace(heading) ? "Выполнено" : heading, message, "blazored-toast-success", SuccessClass, SuccessIconClass),
                ToastLevel.Warning => new ToastSetting(string.IsNullOrWhiteSpace(heading) ? "Внимание" : heading, message, "blazored-toast-warning", WarningClass, WarningIconClass),
                _ => throw new InvalidOperationException(),
            };
        }

        private void ShowToast(ToastLevel level, string message, string heading)
        {
            InvokeAsync(() =>
            {
                var settings = BuildToastSettings(level, message, heading);
                var toast = new ToastInstance
                {
                    Id = Guid.NewGuid(),
                    TimeStamp = DateTime.Now,
                    ToastSetting = settings
                };

                ToastList.Add(toast);

                var toastTimer = new System.Timers.Timer(Timeout * 1000);

                toastTimer.Elapsed += (sender, args) =>
                {
                    RemoveToast(toast.Id);
                };
                toastTimer.AutoReset = false;
                toastTimer.Start();
                StateHasChanged();

            });
        }
    }
}
