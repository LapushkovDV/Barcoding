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
        private List<Imei> _imeis = new List<Imei>();

        [HttpGet("imeis/imeilist")]
        public IActionResult ImeiList()
        {
            FillImeis();

            return View("./Imei/ImeiList", _imeis);
        }

        [Authorize]
        [HttpGet("imeis/edit")]
        public ActionResult ImeiEdit(int Id)
        {
            FillImeis();

            var imei = _imeis.Where(s => s.Id == Id).FirstOrDefault();

            return View("./Imei/ImeiEdit", imei);
        }

        [Authorize]
        [HttpPost("imeis/edit")]
        public ActionResult ImeiEdit(Imei pi)
        {
            var guery = $"update Imeis set imei = '{pi.ImeiCode}', descr = '{pi.Descr}', IsActive = '{pi.IsActive}' where id = {pi.Id}";
            
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

            return RedirectToAction("ImeiList");
        }

        [Authorize]
        [HttpGet("imeis/create")]
        public ActionResult ImeiCreate()
        {
            return View("./Imei/ImeiCreate");
        }

        [Authorize]
        [HttpPost("imeis/create")]
        public ActionResult ImeiCreate(Imei pi)
        {
            var guery = $"insert into Imeis (Imei,Descr,IsActive) values ('{pi.ImeiCode}','{pi.Descr}','{pi.IsActive}')";
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
           
            return RedirectToAction("ImeiList");
        }

        [Authorize]
        [HttpGet("imeis/delete")]
        public ActionResult ImeiDelete(int Id)
        {
            FillImeis();

            var imei = _imeis.Where(s => s.Id == Id).FirstOrDefault();

            return View("./Imei/ImeiDelete", imei);
        }

        [Authorize]
        [HttpPost("imeis/delete")]
        public ActionResult ImeiDelete(Imei i)
        {
            var guery = $"delete from Imeis where ID = {i.Id} ";
            
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
            
            return RedirectToAction("ImeiList");
        }

        public void FillImeis()
        {
            var guery = $"select i.Id, i.Imei, i.descr, i.IsActive from Imeis i";

            _connection.Open();
            
            var cmd = new SqlCommand(guery, _connection);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var imei = new Imei
                    {
                        Id = (int)reader.GetValue(0),
                        ImeiCode = (string)reader.GetValue(1),
                        Descr = (string)reader.GetValue(2),
                        IsActive = (bool)reader.GetValue(3)
                    };

                    _imeis.Add(imei);
                }
            }

            _connection.Close();
        }

    }
}


