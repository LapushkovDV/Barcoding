using ERP_API_Service.Objects;
using ERP_API_Service.WebObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ERP_API_Service.Controllers
{
    public partial class AdminController : Controller
    {
        private List<RoleMenu> _rolesMenus = new List<RoleMenu>();
        private ListBoxViewModel _listBoxViewModel = new ListBoxViewModel();

        [Authorize]
        [HttpGet("roles/roleMenuList")]
        public IActionResult RoleMenuList()
        {
            FillRolesMenu();

            return View("./RoleMenu/RoleMenuList", _rolesMenus);
        }

        [Authorize]
        [HttpGet("roles/roleMenuCreate")]
        public ActionResult RoleMenuCreate(Role role)
        {
            var roleMenu = new RoleMenu
            {
                Role = role
            };

            _listBoxViewModel.RoleMenu = roleMenu;

            _menus = UniqMenuByRole(role);

            _listBoxViewModel.SelectLists = _menus.Select(x => new SelectListItem(x.Name, x.Id.ToString())).OrderBy(x => x.Text).ToList();

            return View("./RoleMenu/RoleMenuCreate", _listBoxViewModel);
        }

        [Authorize]
        [HttpPost("Roles/RoleMenuCreate")]
        public ActionResult RoleMenuCreate(ListBoxViewModel lw)
        {
            if (lw.SelectListItemsIds != null)
            {
                for (var i = 0; i < lw.SelectListItemsIds.Length; i++)
                {
                    var guery = $"insert into Role_Menu (RoleID, MenuId, Descr) values ('{lw.RoleMenu.Role.Id}','{lw.SelectListItemsIds[i]}','{lw.RoleMenu.Descr}')";
                    
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
                }
            }

            return RedirectToAction("RoleEdit", new { id = lw.RoleMenu.Role.Id });
        }

        [Authorize]
        [HttpGet("Roles/RoleMenuEdit")]
        public ActionResult RoleMenuEdit(int Id)
        {
            FillRolesMenu();

            var roleMenu = _rolesMenus.Where(x => x.Id == Id).FirstOrDefault();

            FillMenus();

            var menuList = new List<SelectListItem>();

            foreach (Menu menu in _menus)
            {
                menuList.Add(new SelectListItem(menu.Name, menu.Id.ToString()));
            }

            ViewBag.menus = menuList.OrderBy(menu => menu.Text);

            return View("./RoleMenu/RoleMenuEdit", roleMenu);
        }

        [Authorize]
        [HttpPost("Roles/RoleMenuEdit")]
        public ActionResult RoleMenuEdit(RoleMenu rm)
        {
            var guery = $"update Role_Menu set RoleID = '{rm.Role.Id}', MenuId = '{rm.Menu.Id}', Descr = '{rm.Descr}' where id = {rm.Id}";
            
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
            
            return RedirectToAction("RoleEdit", new { id = rm.Role.Id });
        }

        [Authorize]
        [HttpGet("roles/roleMenuDelete")]
        public ActionResult RoleMenuDelete(int Id)
        {
            FillRolesMenu();

            var roleMenu = _rolesMenus.Where(x => x.Id == Id).FirstOrDefault();

            return View("./RoleMenu/RoleMenuDelete", roleMenu);
        }

        [Authorize]
        [HttpPost("Roles/RoleMenuDelete")]
        public ActionResult RoleMenuDelete(RoleMenu rm)
        {
            var guery = $"delete from Role_Menu where ID = {rm.Id} ";
            
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
            
            return RedirectToAction("RoleEdit", new { id = rm.Role.Id });
        }

        public void FillRolesMenu()
        {
            var guery = $"select rm.Id, r.Id, m.id, r.Name, m.Name, rm.Descr, m.Action " +
                           $"from Role_Menu rm " +
                           $"left join Roles r on rm.RoleId = r.id " +
                           $"left join Menus m on rm.MenuId = m.id ";

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
                        Role = new Role
                        {
                            Id = (int)reader.GetValue(1),
                            Name = (string)reader.GetValue(3),
                        },
                        Menu = new Menu
                        {
                            Id = (int)reader.GetValue(2),
                            Name = (string)reader.GetValue(4),
                            Action = (string)reader.GetValue(6)
                        },
                        Descr = (string)reader.GetValue(5)
                    };

                    _rolesMenus.Add(roleMenu);
                }
            }

            _connection.Close();
        }
    }
}