using ERP_Admin_Panel.Services.Database.Models;
using ERP_Admin_Panel.Services.Database.GenericRepository;

namespace ERP_Admin_Panel.Services.Database.Mocks
{
    public static class MocksData
    {
        private static GenericRepository<RoleUser> _genericRepositorRoleUser;

        public static void RoleUserAdd(ApplicationContext context)
        {
            _genericRepositorRoleUser = new(context);

            if (context != null & _genericRepositorRoleUser.GetAll().Count == 0)
            {
                _genericRepositorRoleUser.Create(new RoleUser { Name = "Пользователь ТСД", Tag = LevelRange.UserTSD });
                _genericRepositorRoleUser.Create(new RoleUser { Name = "Администратор ТСД", Tag = LevelRange.Admin });
                _genericRepositorRoleUser.Create(new RoleUser { Name = "Суперадминистратор", Tag = LevelRange.SuperAdmin });
            }
        }
    }
}
