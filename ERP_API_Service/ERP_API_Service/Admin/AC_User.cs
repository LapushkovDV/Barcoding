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
        private List<User> _users = new List<User>();
        private List<UserImei> _userImeis = new List<UserImei>();
        private List<UserRole> _userRoles = new List<UserRole>();

        [Authorize]
        [HttpGet("users/userList")]
        public IActionResult UserList()
        {
            FillUsers();

            return View("./User/UserList", _users);
        }
        [Authorize]
        [HttpGet("users/edit")]
        public ActionResult UserEdit(int Id)
        {
            FillUsers();

            var user = _users.Where(s => s.Id == Id).FirstOrDefault();
            
            FillUserImeis(user);
            FillUserRoles(user);
            
            user.UserImeis = _userImeis;
            user.UserRoles = _userRoles;
            
            return View("./User/UserEdit", user);
        }

        [Authorize]
        [HttpPost("users/edit")]
        public ActionResult UserEdit(User u)
        {
            var isAdmin = u.IsAdmin ? 1 : 0;
            var isActive = u.IsActive ? 1 : 0;
            var guery = $"update Service_Users set Login = '{u.Login}', Password = '{u.Password}', IsAdmin = {isAdmin}, IsActive = {isActive}  where id = {u.Id}";
            
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

            return RedirectToAction("UserList");
        }

        [Authorize]
        [HttpGet("users/create")]
        public ActionResult UserCreate()
        {
            return View("./User/UserCreate");
        }

        [Authorize]
        [HttpPost("users/create")]
        public ActionResult UserCreate(User u)
        {
            var isAdmin = u.IsAdmin ? 1 : 0;
            var isActive = u.IsActive ? 1 : 0;
            var guery = $"insert into Service_Users (Login,Password,CreateDate,IsAdmin,IsActive) values ('{u.Login}','{u.Password}',getdate(),{isAdmin},{isActive})";
            
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

            return RedirectToAction("UserList");
        }

        [Authorize]
        [HttpGet("users/delete")]
        public ActionResult UserDelete(int Id)
        {
            FillUsers();

            var user = _users.Where(s => s.Id == Id).FirstOrDefault();

            return View("./User/UserDelete", user);
        }

        [Authorize]
        [HttpPost("users/delete")]
        public ActionResult UserDelete(User u)
        {
            var guery = $"delete from Service_Users where ID = {u.Id} ";
            
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

            return RedirectToAction("UserList");
        }

        public void FillUsers()
        {
            var guery = $"select u.Id, u.Login, u.Password, u.IsAdmin, u.IsActive from Service_Users u";

            _connection.Open();
            
            var cmd = new SqlCommand(guery, _connection);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var user = new User
                    {
                        Id = (int)reader.GetValue(0),
                        Login = (string)reader.GetValue(1),
                        Password = (string)reader.GetValue(2),
                        IsAdmin = (bool)reader.GetValue(3),
                        IsActive = (bool)reader.GetValue(4)
                    };
                    
                    _users.Add(user);
                }
            }
            
            _connection.Close();
        }

        public void FillUserImeis(User u)
        {
            var guery = $"select ui.Id, ui.imeiId, i.Imei, i.descr from User_Imei ui " +
                           $"left join Imeis i on ui.imeiId = i.id " +
                           $"where ui.UserId = {u.Id.ToString()}";

            _connection.Open();
            
            var cmd = new SqlCommand(guery, _connection);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var userImei = new UserImei
                    {
                        Id = (int)reader.GetValue(0),
                        User = u,
                        Imei = new Imei
                        {
                            Id = (int)reader.GetValue(1),
                            ImeiCode = (string)reader.GetValue(2),
                            Descr = (string)reader.GetValue(3)
                        }
                    };
                    
                    _userImeis.Add(userImei);
                }
            }

            _connection.Close();
        }

        public void FillUserRoles(User u)
        {
            var guery = $"select ur.Id, ur.roleId, r.Name, r.Descr, ur.GiveDate " +
                           $"from User_Roles ur " +
                           $"left join Roles r on ur.roleId = r.id " +
                           $"where ur.UserId = {u.Id.ToString()}";

            _connection.Open();
            
            var cmd = new SqlCommand(guery, _connection);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var userRole = new UserRole
                    {
                        Id = (int)reader.GetValue(0),
                        User = u,
                        Role = new Role
                        {
                            Id = (int)reader.GetValue(1),
                            Name = (string)reader.GetValue(2),
                            Descr = (string)reader.GetValue(3),
                        },
                        GiveDate = (DateTime)reader.GetValue(4)

                    };
                    
                    _userRoles.Add(userRole);
                }
            }

            _connection.Close();
        }
    }
}


