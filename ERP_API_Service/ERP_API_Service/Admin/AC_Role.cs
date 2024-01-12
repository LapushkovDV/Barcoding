using ERP_API_Service.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ERP_API_Service.Controllers
{
    public partial class AdminController : Controller
    {
        private List<Role> _roles = new List<Role>();
        private List<RoleMenu> _roleMenus = new List<RoleMenu>();

        [Authorize]
        [HttpGet("roles/roleList")]
        public IActionResult RoleList()
        {
            FillRoles();

            return View("./Role/RoleList", _roles);
        }

        [Authorize]
        [HttpGet("roles/edit")]
        public ActionResult RoleEdit(int Id)
        {
            FillRoles();

            var role = _roles.Where(x => x.Id == Id).FirstOrDefault();
            
            FillRoleMenus(role);
            
            role.RoleMenus = _roleMenus;
            
            return View("./Role/RoleEdit", role);
        }

        [Authorize]
        [HttpPost("roles/edit")]
        public ActionResult RoleEdit(Role r)
        {
            var guery = $"update Roles set Name = '{r.Name}', Descr = '{r.Descr}' where id = {r.Id}";
            
            try
            {
                _connection.Open();
                
                var cmd = new SqlCommand(guery, _connection);
                
                cmd.ExecuteScalar();
                _connection.Close();
            }
            catch (Exception e)
            {
                Startup.errorMessage = e.Message;
                
                return RedirectToAction("_ErrorPage");
            }
            
            return RedirectToAction("RoleList");
        }

        [Authorize]
        [HttpGet("roles/create")]
        public ActionResult RoleCreate()
        {
            return View("./Role/RoleCreate");
        }

        [Authorize]
        [HttpPost("roles/create")]
        public ActionResult RoleCreate(Role r)
        {
            var guery = $"insert into Roles (Name, Descr) values ('{r.Name}','{r.Descr}')";
            
            try
            {
                _connection.Open();
                
                var cmd = new SqlCommand(guery, _connection);
                
                cmd.ExecuteScalar();
                _connection.Close();
            }
            catch (Exception e)
            {
                Startup.errorMessage = e.Message;
                
                return RedirectToAction("_ErrorPage");
            }
            
            return RedirectToAction("RoleList");
        }

        [Authorize]
        [HttpGet("roles/delete")]
        public ActionResult RoleDelete(int Id)
        {
            FillRoles();
            
            var role = _roles.Where(x => x.Id == Id).FirstOrDefault();

            return View("./Role/RoleDelete", role);
        }

        [Authorize]
        [HttpPost("roles/delete")]
        public ActionResult RoleDelete(Role r)
        {
            var guery = $"delete from Roles where ID = {r.Id} ";
            
            try
            {
                _connection.Open();
                
                var cmd = new SqlCommand(guery, _connection);
                
                cmd.ExecuteScalar();
                _connection.Close();
            }
            catch (Exception e)
            {
                Startup.errorMessage = e.Message;
                
                return RedirectToAction("_ErrorPage");
            }
            
            return RedirectToAction("RoleList");
        }

        public void FillRoles()
        {
            var guery = $"select r.Id, r.Name, r.Descr from Roles r";

            _connection.Open();
            
            var cmd = new SqlCommand(guery, _connection);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var role = new Role
                    {
                        Id = (int)reader.GetValue(0),
                        Name = (string)reader.GetValue(1),
                        Descr = (string)reader.GetValue(2)
                    };
                    
                    _roles.Add(role);
                }
            }

            _connection.Close();
        }

        public void FillRoleMenus(Role r)
        {
            var guery = $"select rm.Id, rm.menuId, m.Name, m.Action, m.Descr, rm.Descr " +
                           $"from Role_Menu rm " +
                           $"left join Menus m on rm.menuId = m.id " +
                           $"where rm.roleId = {r.Id.ToString()}";

            _connection.Open();
            
            var cmd = new SqlCommand(guery, _connection);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var roleMenu = new RoleMenu
                    {
                        Id = (int)reader.GetValue(0),
                        Role = r,
                        Menu = new Menu
                        {
                            Id = (int)reader.GetValue(1),
                            Name = (string)reader.GetValue(2),
                            Action = (string)reader.GetValue(3),
                            Descr = (string)reader.GetValue(4),
                        },
                        Descr = (string)reader.GetValue(5)
                    };

                    _roleMenus.Add(roleMenu);
                }
            }

            _connection.Close();
        }
    }
}


