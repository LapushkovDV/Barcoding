using System;
using System.Collections.Generic;

namespace ERP_API_Service.Objects
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Descr { get; set; }
        public List<UserImei> UserImeis { get; set; }
        public List<UserRole> UserRoles { get; set; }

        public bool IsAdmin { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }

    public class Imei
    {
        public int Id { get; set; }
        public string ImeiCode { get; set; }

        public string Descr { get; set; }
        public bool IsActive { get; set; } = false;

    }
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Descr { get; set; }
        public List<RoleMenu> RoleMenus { get; set; }

    }

    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public string Descr { get; set; }
        public int Npp { get; set; }
    }

    public class Error
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string Descr { get; set; }
    }
    public class UserImei
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Imei Imei { get; set; }
    }

    public class UserRole
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
        public DateTime GiveDate { get; set; }
    }
    public class RoleMenu
    {
        public int Id { get; set; }
        public Role Role { get; set; }
        public Menu Menu { get; set; }
        public string Descr { get; set; }
    }

    public class UserImeiIds
    {
        public int UserId { get; set; }
        public int ImeiId { get; set; }

    }

    public class Event
    {
        public string UserLogin { get; set; }
        public string UserImei { get; set; }
        public DateTime EvDateTime { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }

    public class EventDate
    {
        public string beginDate { get; set; }
        public string endDate { get; set; }
    }

    public class Setting
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ConfigurationSystem
    {
        public bool LinkImeiUser { get; set; }
        public int LifeTimeToken { get; set; }
        public string ESB_GetObjectById { get; set; }
        public string ESB_ExecOperation { get; set; }
        public string ESB_ExecOperationAsync { get; set; }
        public string ESB_GetDescrShk { get; set; }

    }

}
 