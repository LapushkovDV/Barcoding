namespace ERP_Admin_Panel.Services.NavBarActions
{
    public interface INavBarOperation
    {
        bool IsSearchVisible { get; set; }

        void SetActions(params Action[] actions);

        List<Action> GetActions();
    }
}