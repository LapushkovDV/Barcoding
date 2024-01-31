using ClosedXML.Excel;
using ERP_API_Service.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace ERP_API_Service.Controllers
{
    public partial class AdminController : Controller
    {
        private List<Event> _events = new List<Event>();
        [Authorize]
        [HttpGet("eventList")]
        public IActionResult EventList()
        {
            return View("./event/EventList");
        }

        [Authorize]
        [HttpPost("eventList")]
        public ActionResult EventList(EventDate ed)
        {
            if (ed.beginDate == null && ed.endDate == null)
            {
                Startup.errorMessage = "Укажите диапазон дат";
                
                return RedirectToAction("_ErrorPage");
            }
            else
            {
                var guery = $"select coalesce(u.Login,'') as Login,  coalesce(i.Imei,'') as IMEI, ae.DateTime, ae.Type, ae.Message " +
                               $"from ApiEvents ae " +
                               $"left join Service_Users u on ae.userId = u.id " +
                               $"left join Imeis i on ae.imeiId = i.id " +
                               $"where ae.DateTime between '{ed.beginDate}' and '{ed.endDate}'";
                var dataTable = new DataTable();
                
                try
                {
                    _connection.Open();
                    
                    var cmd = new SqlCommand(guery, _connection);
                    var dataAdapter = new SqlDataAdapter(cmd);

                    dataAdapter.Fill(dataTable);

                    _connection.Close();
                }
                catch (Exception e)
                {
                    Startup.errorMessage = e.Message;
                    
                    return RedirectToAction("_ErrorPage");
                }

                var workBook = new XLWorkbook();
                var workSheet = workBook.Worksheets.Add(dataTable, "WorksheetName");

                workSheet.ColumnWidth = 30;

                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"events_{ed.beginDate}_to_{ed.endDate}.xlsx");
                }
            }
        }

        public void FillEvents()
        {
            var guery = $"select coalesce(u.Login,''), coalesce(i.Imei,''), ae.DateTime, ae.Type, ae.Message " +
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
                    var e = new Event
                    {
                        UserLogin = (string)reader.GetValue(0),
                        UserImei = (string)reader.GetValue(1),
                        EvDateTime = (DateTime)reader.GetValue(2),
                        Type = (string)reader.GetValue(3),
                        Message = (string)reader.GetValue(4)
                    };

                    _events.Add(e);
                }
            }
            _connection.Close();
        }
    }
}


