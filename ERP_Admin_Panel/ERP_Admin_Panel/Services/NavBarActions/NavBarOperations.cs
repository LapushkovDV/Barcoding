namespace ERP_Admin_Panel.Services.NavBarActions
{
    public class NavBarOperation : INavBarOperation
    {
        private List<Action> _actions = new List<Action>();

        public void SetActions(params Action[] actions) => _actions = actions.ToList();

        public List<Action> GetActions() => _actions;

        public bool IsSearchVisible { get; set; } = false;

    }
}
