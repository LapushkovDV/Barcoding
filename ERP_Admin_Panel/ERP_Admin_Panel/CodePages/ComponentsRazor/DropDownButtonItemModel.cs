using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor
{
    public class DropDownButtonItemModel : ComponentBase
    {
        [Parameter]
        public string Title { get; set; }
        [Parameter]
        public Action? Execute { get; set; }
    }
}
