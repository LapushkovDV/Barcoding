using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages.Shared
{
    public class NavMenuModel : ComponentBase
    {
        protected bool collapseNavMenu = true;

        protected string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
