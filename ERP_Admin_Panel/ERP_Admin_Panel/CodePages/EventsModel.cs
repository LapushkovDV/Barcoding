using ERP_Admin_Panel.Data.ViewModels;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages
{
    public class EventsModel : ComponentBase
    {
        [Inject] protected IToastService? ToastService { get; set; }
        private bool _isSortedAscending;
        private string _activeSortColumn = string.Empty;

        protected EventViewModel EventViewModel { get; set; } = new EventViewModel();
        protected List<Event> Events { get; set; } = new List<Event>();

        protected void SortTable(string columnName)
        {
            if (columnName != _activeSortColumn)
            {
                Events = Events.OrderBy(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
                _isSortedAscending = true;
                _activeSortColumn = columnName;
            }
            else
            {
                if (_isSortedAscending)
                {
                    Events = Events.OrderByDescending(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
                }
                else
                {
                    Events = Events.OrderBy(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
                }
                _isSortedAscending = !_isSortedAscending;
            }
        }

        protected string SetSortIcon(string columnName)
        {
            if (_activeSortColumn != columnName)
            {
                return string.Empty;
            }
            if (_isSortedAscending)
            {
                return "fa-sort-up";
            }
            else
            {
                return "fa-sort-down";
            }
        }

        protected void SearchEvents()
        {
            if (EventViewModel.EndDateTime < EventViewModel.StartDateTime)
            {
                //ToastService.ShowWarning();
            }
        }
    }
}
