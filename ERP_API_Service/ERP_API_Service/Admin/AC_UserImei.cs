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
        private List<UserImei> _usersImeis = new List<UserImei>();

        [Authorize]
        [HttpGet("users/userImeiList")]
        public IActionResult UserImeiList()
        {
            FillUsersImei();
            
            return View("./UserImei/userImeiList", _usersImeis);
        }

        [Authorize]
        [HttpGet("users/userImeiCreate")]
        public ActionResult UserImeiCreate(User u)
        {
            var userImei = new UserImei
            {
                User = u
            };

            FillImeis();

            var imeiList = new List<SelectListItem>();

            foreach (var imei in _imeis)
            {
                imeiList.Add(new SelectListItem(imei.Descr, imei.Id.ToString()));
            }

            ViewBag.imeis = imeiList.OrderBy(p => p.Text);

            return View("./UserImei/UserImeiCreate", userImei);
        }

        [Authorize]
        [HttpPost("users/userImeiCreate")]
        public ActionResult UserImeiCreate(UserImei ui)
        {
            var guery = $"insert into User_Imei (userID, imeiID, CreateDate) values ('{ui.User.Id}','{ui.Imei.Id}',getdate())";
            
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
            
            return RedirectToAction("UserEdit", new { id = ui.User.Id });
        }

        [Authorize]
        [HttpGet("users/userImeiEdit")]
        public ActionResult UserImeiEdit(int Id)
        {
            FillUsersImei();
            var userImei = _usersImeis.Where(s => s.Id == Id).FirstOrDefault();

            FillUsers();

            var userList = new List<SelectListItem>();
            
            foreach (var user in _users)
            {
                userList.Add(new SelectListItem(user.Login, user.Id.ToString()));
            }

            ViewBag.users = userList.OrderBy(p => p.Text);

            FillImeis();
            
            var imeiList = new List<SelectListItem>();

            foreach (var imei in _imeis)
            {
                imeiList.Add(new SelectListItem(imei.ImeiCode, imei.Id.ToString()));
            }

            ViewBag.imeis = imeiList.OrderBy(p => p.Text);
            
            return View("./UserImei/UserImeiEdit", userImei);
        }

        [Authorize]
        [HttpPost("users/userImeiEdit")]
        public ActionResult UserImeiEdit(UserImei ui)
        {
            var guery = $"update User_Imei set userID = '{ui.User.Id}', imeiID = '{ui.Imei.Id}' where id = {ui.Id}";
            
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
            
            return RedirectToAction("UserEdit", new { id = ui.User.Id });
        }

        [Authorize]
        [HttpGet("users/userImeiDelete")]
        public ActionResult UserImeiDelete(int Id)
        {
            FillUsersImei();

            var userImei = _usersImeis.Where(s => s.Id == Id).FirstOrDefault();

            return View("./UserImei/UserImeiDelete", userImei);
        }

        [Authorize]
        [HttpPost("users/userImeiDelete")]
        public ActionResult UserImeiDelete(UserImei ui)
        {
            var guery = $"delete from User_Imei where ID = {ui.Id} ";
            
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
            
            return RedirectToAction("UserEdit", new { id = ui.User.Id });
        }

        public void FillUsersImei()
        {
            var guery = $"select ui.Id, u.Id, i.id, u.Login, i.Imei, i.Descr " +
                           $"from User_Imei ui " +
                           $"left join Service_Users u on ui.userId = u.id " +
                           $"left join Imeis i on ui.imeiId = i.id ";

            _connection.Open();
            
            var cmd = new SqlCommand(guery, _connection);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var userImei = new UserImei
                    {
                        User = new User(),
                        Imei = new Imei(),
                        Id = (int)reader.GetValue(0)
                    };
                    userImei.User.Id = (int)reader.GetValue(1);
                    userImei.Imei.Id = (int)reader.GetValue(2);
                    userImei.User.Login = (string)reader.GetValue(3);
                    userImei.Imei.ImeiCode = (string)reader.GetValue(4);
                    userImei.Imei.Descr = (string)reader.GetValue(5);
                    
                    _usersImeis.Add(userImei);
                };
            }

            _connection.Close();
        }
    }
}