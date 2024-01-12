using ERP_API_Service.Objects;
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
        private List<Menu> _menus = new List<Menu>();
        private List<string> _actions = new List<string>();
        private List<SelectListItem> _actionsList = new List<SelectListItem>();

        [Authorize]
        [HttpGet("menus/menuList")]
        public IActionResult MenuList()
        {
            FillMenus();

            return View("./menu/MenuList", _menus);
        }

        [Authorize]
        [HttpGet("menus/edit")]
        public ActionResult MenuEdit(int Id)
        {
            FillMenus();
            FillActions();
            
            var menu = _menus.Where(s => s.Id == Id).FirstOrDefault();
            
            return View("./menu/MenuEdit", menu);
        }

        [Authorize]
        [HttpPost("menus/edit")]
        public ActionResult MenuEdit(Menu m)
        {
            var guery = $"update Menus set Name = '{m.Name}', Action = '{m.Action}', Descr = '{m.Descr}', Npp = '{m.Npp}' where id = {m.Id}";
            
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
            
            return RedirectToAction("MenuList");
        }

        [Authorize]
        [HttpGet("menus/create")]
        public ActionResult MenuCreate()
        {
            FillActions();

            return View("./menu/MenuCreate");
        }

        [Authorize]
        [HttpPost("menus/create")]
        public ActionResult MenuCreate(Menu m)
        {
            var guery = $"insert into Menus (Name, Action, Descr, Npp) values ('{m.Name}','{m.Action}','{m.Descr}', {m.Npp})";
            
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
            
            return RedirectToAction("MenuList");
        }
        [Authorize]
        [HttpGet("menus/delete")]
        public ActionResult MenuDelete(int Id)
        {
            FillMenus();
            
            var menu = _menus.Where(s => s.Id == Id).FirstOrDefault();

            return View("./menu/MenuDelete", menu);
        }
        
        [Authorize]
        [HttpPost("menus/delete")]
        public ActionResult MenuDelete(Menu m)
        {
            var guery = $"delete from Menus where ID = {m.Id} ";
            
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

            return RedirectToAction("MenuList");
        }

        public List<Menu> UniqMenuByRole(Role r)
        {
            var roleMenuIds = new List<int>();
            var result = new List<Menu>();

            try
            {
                var query = $"select rm.menuID " +
                               $"from Role_Menu rm "+
                               $"WHERE rm.roleID = {r.Id}";
                
                _connection.Open();

                var cmd = new SqlCommand(query, _connection);
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        roleMenuIds.Add((int)reader.GetValue(0));
                    }
                }

                _connection.Close();

                query = $"select m.Id, m.Name, m.Action, m.Descr, m.Npp from Menus m order by m.Npp ";

                _connection.Open();

                cmd = new SqlCommand(query, _connection);
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (!roleMenuIds.Contains((int)reader.GetValue(0)))
                        {
                            var menu = new Menu
                            {
                                Id = (int)reader.GetValue(0),
                                Name = (string)reader.GetValue(1),
                                Action = (string)reader.GetValue(2),
                                Descr = (string)reader.GetValue(3),
                                Npp = (int)reader.GetValue(4)
                            };
                            
                            result.Add(menu);
                        }
                    }

                    result.OrderBy(menu => menu.Npp);
                }

                _connection.Close();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public void FillMenus()
        {
            var guery = $"select m.Id, m.Name, m.Action, m.Descr, m.Npp from Menus m order by m.Npp ";

            _connection.Open();
            
            var cmd = new SqlCommand(guery, _connection);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var menu = new Menu
                    {
                        Id = (int)reader.GetValue(0),
                        Name = (string)reader.GetValue(1),
                        Action = (string)reader.GetValue(2),
                        Descr = (string)reader.GetValue(3),
                        Npp = (int)reader.GetValue(4)
                    };
                    
                    _menus.Add(menu);
                }

                _menus.OrderBy(m => m.Npp);
            }

            _connection.Close();
        }

        public void FillActions()
        {
            var guery = $"select a.Name from Actions a";

            _connection.Open();
            
            var cmd = new SqlCommand(guery, _connection);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var action = (string)reader.GetValue(0);
                    
                    _actions.Add(action);
                    _actionsList.Add(new SelectListItem(action, action));
                }
            }

            ViewBag.actions = _actionsList.OrderBy(action => action.Text);
        }
    }
}


