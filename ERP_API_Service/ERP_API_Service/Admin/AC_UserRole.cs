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
        private List<UserRole> _usersRoles = new List<UserRole>();

        [Authorize]
        [HttpGet("users/userRoleList")]
        public IActionResult UserRoleList()
        {
            FillUsersRole();
            return View("./UserRole/userRoleList", _usersRoles);
        }

        [Authorize]
        [HttpGet("users/userRoleCreate")]
        public ActionResult UserRoleCreate(User u)
        {
            var userRole = new UserRole();

            userRole.User = u;

            FillRoles();
            
            var roleList = new List<SelectListItem>();

            foreach (var role in _roles)
            {
                roleList.Add(new SelectListItem(role.Name, role.Id.ToString()));
            }
            ViewBag.roles = roleList.OrderBy(p => p.Text);

            return View("./UserRole/UserRoleCreate", userRole);
        }

        [Authorize]
        [HttpPost("users/userRoleCreate")]
        public ActionResult UserRoleCreate(UserRole ur)
        {
            var guery = $"insert into User_Roles (userID, roleId, GiveDate) values ('{ur.User.Id}','{ur.Role.Id}',getdate())";
            
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

            return RedirectToAction("UserEdit", new { id = ur.User.Id });
        }

        [Authorize]
        [HttpGet("users/userRoleEdit")]
        public ActionResult UserRoleEdit(int Id)
        {
            FillUsersRole();
            
            var userRole = _usersRoles.Where(s => s.Id == Id).FirstOrDefault();

            FillUsers();
            
            var userList = new List<SelectListItem>();
            
            foreach (var user in _users)
            {
                userList.Add(new SelectListItem(user.Login, user.Id.ToString()));
            }
            
            ViewBag.users = userList.OrderBy(p => p.Text);

            FillRoles();
            
            var roleList = new List<SelectListItem>();

            foreach (var role in _roles)
            {
                roleList.Add(new SelectListItem(role.Name, role.Id.ToString()));
            }
            
            ViewBag.roles = roleList.OrderBy(p => p.Text);

            return View("./UserRole/UserRoleEdit", userRole);       
        }

        [Authorize]
        [HttpPost("users/userRoleEdit")]
        public ActionResult UserRoleEdit(UserRole ur)
        {
            var guery = $"update User_Roles set userID = '{ur.User.Id}', roleId = '{ur.Role.Id}', giveDate = getdate() where id = {ur.Id}";
            
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

            return RedirectToAction("UserEdit", new { id = ur.User.Id });
        }

        [Authorize]
        [HttpGet("users/userRoleDelete")]
        public ActionResult UserRoleDelete(int Id)
        {
            FillUsersRole();

            var userRole = _usersRoles.Where(s => s.Id == Id).FirstOrDefault();

            return View("./UserRole/UserRoleDelete", userRole);
        }

        [Authorize]
        [HttpPost("users/userRoleDelete")]
        public ActionResult UserRoleDelete(UserRole ur)
        {
            var guery = $"delete from User_Roles where ID = {ur.Id} ";
            
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
            
            return RedirectToAction("UserEdit", new { id = ur.User.Id });
        }

        public void FillUsersRole()
        {
            var guery = $"select ur.Id, u.Id, r.id, u.Login, r.Name, ur.GiveDate " +
                           $"from User_Roles ur " +
                           $"left join Service_Users u on ur.userId = u.id " +
                           $"left join Roles r on ur.RoleId = r.id ";

            _connection.Open();
            
            var cmd = new SqlCommand(guery, _connection);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var userRole = new UserRole
                    {
                        User = new User(),
                        Role = new Role(),
                        Id = (int)reader.GetValue(0)
                    };
                    userRole.User.Id = (int)reader.GetValue(1);
                    userRole.Role.Id = (int)reader.GetValue(2);
                    userRole.User.Login = (string)reader.GetValue(3);
                    userRole.Role.Name = (string)reader.GetValue(4);
                    userRole.GiveDate = (DateTime)reader.GetValue(5);
                    
                    _usersRoles.Add(userRole);
                }
            }

            _connection.Close();
        }
    }
}


