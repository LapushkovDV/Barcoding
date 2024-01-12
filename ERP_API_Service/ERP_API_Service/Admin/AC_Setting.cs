using ERP_API_Service.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;

namespace ERP_API_Service.Controllers
{
    public partial class AdminController : Controller
    {
        private ConfigurationSystem _configure = new ConfigurationSystem();

        [Authorize]
        [HttpGet("Setting/config")]
        public IActionResult Config()
        {
            FillSettingsConfig();

            return View("./Setting/config", _configure);
        }

        [Authorize]
        [HttpPost("Setting/config")]
        public IActionResult Config(ConfigurationSystem c)
        {
            var check = c.LinkImeiUser ? "+":"-";
            var guery = $"update Settings set value = '{check}' where name = 'LinkImeiUser';" + Environment.NewLine;
            
            guery += $"update Settings set value = '{c.LifeTimeToken}' where name = 'LifeTimeToken';";
            guery += $"update Settings set value = '{c.ESB_GetObjectById}' where name = 'ESB_GetObjectById';";
            guery += $"update Settings set value = '{c.ESB_ExecOperation}' where name = 'ESB_ExecOperation';";
            guery += $"update Settings set value = '{c.ESB_ExecOperationAsync}' where name = 'ESB_ExecOperationAsync';";
            guery += $"update Settings set value = '{c.ESB_GetDescrShk}' where name = 'ESB_GetDescrShk';";

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
            return View("./Setting/config", _configure);
        }

        public void FillSettingsConfig()
        {
            var guery = $"select s.Id, s.type, s.Name, s.value from Settings s order by s.name ";

            _connection.Open();
            var cmd = new SqlCommand(guery, _connection);
            var reader = cmd.ExecuteReader();
            
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    switch ((string)reader.GetValue(2))
                    {
                        case "LinkImeiUser":
                            _configure.LinkImeiUser = ((string)reader.GetValue(3) == "+");
                            break;
                        case "LifeTimeToken":
                            int ltt;
                            int.TryParse((string)reader.GetValue(3), out ltt);
                            
                            _configure.LifeTimeToken = ltt;
                            
                            break;

                        case "ESB_GetObjectById":
                            _configure.ESB_GetObjectById = (string)reader.GetValue(3);
                            
                            break;

                        case "ESB_ExecOperation":
                            _configure.ESB_ExecOperation = (string)reader.GetValue(3);
                            
                            break;

                        case "ESB_ExecOperationAsync":
                            _configure.ESB_ExecOperationAsync = (string)reader.GetValue(3);
                            
                            break;

                        case "ESB_GetDescrShk":
                            _configure.ESB_GetDescrShk = (string)reader.GetValue(3);
                            
                            break;

                    }
                }

            }
            
            _connection.Close();
        }
    }
}


