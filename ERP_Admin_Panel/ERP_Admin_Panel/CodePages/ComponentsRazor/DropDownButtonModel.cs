using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor
{
    public class DropDownButtonModel : ComponentBase
    {
        [Parameter]
        public string ClassButton { get; set; } = string.Empty;
        [Parameter]
        public RenderFragment ContentButton { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected bool Show = false;

        protected async Task CloseMenuOutFocus()
        {
            await Task.Delay(100);

            Show = false;
        }

    }
}
