using ERP_Admin_Panel.Services.Toast;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor
{
    public partial class ToastModel : ComponentBase
    {
        [CascadingParameter] private ToastsModel? ToastsModel { get; set; }

        [Parameter] public Guid ToastId { get; set; }
        [Parameter] public ToastSetting? ToastSetting { get; set; }

        internal void Close()
        {
            ToastsModel?.RemoveToast(ToastId);
        }
    }
}
