using ERP_Admin_Panel.Pages.ComponentsRazor.Users;
using ERP_Admin_Panel.Services.Account;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Database.GenericRepository;
using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Modal;
using ERP_Admin_Panel.Services.NavBarActions;
using Microsoft.AspNetCore.Components;

namespace ERP_Admin_Panel.CodePages
{
    public class UsersModel : ComponentBase
    {
        private GenericRepository<User> _genericRepositoryUser;
        private GenericRepository<RoleUser> _genericRepositoryRoleUser;
        private LevelRange _currentRole;
        private int _currentRoleId;
        private bool _isSortedAscending;
        private string _activeSortColumn = string.Empty;

        [Inject] protected INavBarOperation NavBarOperation { get; set; }
        [Inject] public ApplicationContext? ApplicationContext { get; set; }
        [Inject] public IModalService? ModalService { get; set; }
        [Inject] public IAccountManager? AccountManager { get; set; }

        protected List<User> Users { get; set; }

        protected override async Task OnInitializedAsync()
        {
            NavBarOperation.IsSearchVisible = true;
            _currentRoleId = await AccountManager.GetRoleId();

            if (_currentRoleId != -1)
            {
                _genericRepositoryUser = new(ApplicationContext);
                _genericRepositoryRoleUser = new(ApplicationContext);
                _currentRole = _genericRepositoryRoleUser.Get(x => x.Id == _currentRoleId).Tag;

                Users = await Task.FromResult(_genericRepositoryUser.GetAll(x => x.RoleUser.Tag < _currentRole, includes: x => x.RoleUser));
            }
        }

        protected void AddUser() => ShowAddModal();

        protected void EditUser(User user) => ShowEditModal(user);

        protected void RemoveUser(User user) => ShowRemoveModal(user);

        protected void BindingDevice(User user) => ShowBindingDeviceModal(user);

        private void ShowEditModal(User user)
        {
            var parameters = new ModalParameters();

            parameters.Add(nameof(User), user);

            ModalService.OnClose += ClosedEditModal;
            ModalService?.Show<EditUser>("Редактирование данных пользователя", parameters);
        }

        private void ShowAddModal()
        {
            ModalService.OnClose += ClosedAddAndRemoveModal;
            ModalService?.Show<AddUser>("Добавить нового пользователя", new ModalParameters());
        }

        private void ShowRemoveModal(User user)
        {
            var parameters = new ModalParameters();

            parameters.Add(nameof(User), user);

            ModalService.OnClose += ClosedAddAndRemoveModal;
            ModalService?.Show<RemoveUser>("Подтверждение", parameters);
        }

        private void ShowBindingDeviceModal(User user)
        {
            var parameters = new ModalParameters();

            parameters.Add(nameof(User), user);

            ModalService.OnClose += ClosedBindingDeviceModal;
            ModalService?.Show<BindingDevice>("Привязка устройств", parameters);
        }

        private void ClosedEditModal(ModalResult result)
        {
            Console.WriteLine("Окно закрыто");

            if (result.Cancelled)
            {
                Console.WriteLine("Редактирование отменено");
            }
            else
            {
                Console.WriteLine(result.Data.ToString());
                StateHasChanged();
            }

            ModalService.OnClose -= ClosedEditModal;
        }



        private async void ClosedAddAndRemoveModal(ModalResult result)
        {
            Console.WriteLine("Окно закрыто");

            if (result.Cancelled)
            {
                Console.WriteLine("Редактирование отменено");
            }
            else
            {
                Console.WriteLine(result.Data.ToString());

                Users = await Task.FromResult(_genericRepositoryUser.GetAll(x => x.RoleUser.Tag < _currentRole, includes: x => x.RoleUser));

                StateHasChanged();
            }

            ModalService.OnClose -= ClosedEditModal;

        }

        private async void ClosedBindingDeviceModal(ModalResult result)
        {
            Console.WriteLine("Окно закрыто");

            if (result.Cancelled)
            {
                Console.WriteLine("Привязка устройства отменена");
            }
            else
            {
                Console.WriteLine(result.Data.ToString());
                StateHasChanged();
            }

            ModalService.OnClose -= ClosedBindingDeviceModal;

        }

        protected void SortTable(string columnName)
        {
            if (columnName != _activeSortColumn)
            {
                Users = Users.OrderBy(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
                _isSortedAscending = true;
                _activeSortColumn = columnName;
            }
            else
            {
                if (_isSortedAscending)
                {
                    Users = Users.OrderByDescending(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
                }
                else
                {
                    Users = Users.OrderBy(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
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
    }
}