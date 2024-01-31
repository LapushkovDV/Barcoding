using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace ERP_Admin_Panel.CodePages.ComponentsRazor
{
    public class TableModel<TItem> : ComponentBase
    {
        [Parameter]
        public RenderFragment? TableHeader { get; set; }

        [Parameter]
        public RenderFragment<TItem>? RowTemplate { get; set; }

        [Parameter, AllowNull]
        public IReadOnlyList<TItem> Items { get; set; }
    }
}
