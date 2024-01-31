using ERP_API_Service.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ERP_API_Service.Controllers
{
    public partial class AdminController : Controller
    {
        private List<Setting> _settings = new List<Setting>();
        
        [Authorize]
        [HttpGet("settingList")]
        public IActionResult SettingList()
        {
            return View("./event/EventList");
        }

        public void FillSettings()
        {
            string guery = $"select coalesce(u.Login,''), coalesce(i.Imei,''), ae.DateTime, ae.Type, ae.Message " +
                           $"from ApiEvents ae " +
                           $"left join Service_Users u on ae.userId = u.id " +
                           $"left join Imeis i on ae.imeiId = i.id ";

            _connection.Open();
            
            var cmd = new SqlCommand(guery, _connection);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var eventValue = new Event
                    {
                        UserLogin = (string)reader.GetValue(0),
                        UserImei = (string)reader.GetValue(1),
                        EvDateTime = (DateTime)reader.GetValue(2),
                        Type = (string)reader.GetValue(3),
                        Message = (string)reader.GetValue(4)
                    };

                    _events.Add(eventValue);
                }
            }

            _connection.Close();
        }
    }
}


