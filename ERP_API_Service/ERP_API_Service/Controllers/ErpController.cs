using ERP_API_Service.Objects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Threading;

namespace ERP_API_Service.Controllers
{
    [Route("api/erp")]
    [ApiController]
    public class ErpController : Controller
    {
        private SqlConnection _connection;
        public ErpController()
        {
            string connectionString = Startup.ConnectionString;
            _connection = new SqlConnection(connectionString);
        }

        [HttpPost("execoper")]
        [Produces("application/json")]
        public ActionResult<string> ExecOperation([FromBody] OperationBody body)
        {
            var answerBody = new AnswerBody();
            var bearerToken = Request.Headers["Authorization"].ToString().Substring(7);
            var auth = new Authorization(bearerToken);
            var ids = auth.GetIdsByToken();

            if (ids.UserId != 0)
            {
                LogWriter.Write(ids.UserId, ids.ImeiId, $"api/erp/execoper", $"Request to API");

                var userName = auth.GetUserNameById(ids.UserId);
                var imei = auth.GetImeiByToken();
                var TuneValue = GetTunes.GetStringTune(_connection, "ESB_ExecOperation");
                var request = (HttpWebRequest)WebRequest.Create(TuneValue);

                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(new {userName, imei, request = body.RequestBody });
                    
                    streamWriter.Write(json);
                }

                var result = string.Empty;
                var httpResponse = (HttpWebResponse)request.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                var operAnsw = JsonConvert.DeserializeObject<OperationAnswerBody>(result);
                
                if (operAnsw is OperationAnswerBody)
                {
                    return Json(operAnsw);
                }
                else
                {
                    answerBody.Result = "Failed to deserialize object";
                }
                
                return Json(answerBody);
            }
            else
            {
                answerBody.Result = "Token is invalid or has expired";
                
                return Json(answerBody);
            }
        }

        [HttpPost("execoperAsync")]
        [Produces("application/json")]
        public ActionResult<string> ExecOperationAsync([FromBody] OperationBody body)
        {
            var answerBody = new AsyncAnswerBody
            {
                Guid = Guid.NewGuid()
            };
            
            var bearerToken = Request.Headers["Authorization"].ToString().Substring(7);
            var auth = new Authorization(bearerToken);

            UserImeiIds ids = auth.GetIdsByToken();

            if (ids.UserId != 0)
            {
                LogWriter.Write(ids.UserId, ids.ImeiId, $"api/erp/execoperAsync", $"Request to API");

                answerBody.Status = RequestStatus.Created;

                var guery = $"insert into ApiRequests ([Guid],[Body],[Status],[Date],[UserId],[ImeiId]) values " +
                               $"('{answerBody.Guid.ToString()}','{body.RequestBody}','{(int)answerBody.Status}',getdate(),{ids.UserId},{ids.ImeiId})";
                using (var _connection = new SqlConnection(Startup.ConnectionString))
                {
                    var cmd = new SqlCommand(guery, _connection);
                    
                    _connection.Open();
                    cmd.ExecuteScalar();
                }

                answerBody.Result = "OK";
            }
            else
            {
                answerBody.Guid = Guid.Empty;
                answerBody.Status = RequestStatus.Error;
                answerBody.Result = "Token is invalid or has expired";
            }

            return Json(answerBody);
        }

        [HttpPost("getStatus")]
        [Produces("application/json")]
        public ActionResult<string> GetStatus([FromBody] GetStatusBody body)
        {
            AsyncAnswerBody answer = new AsyncAnswerBody();

            string bearerToken = Request.Headers["Authorization"].ToString().Substring(7);

            Authorization auth = new Authorization(bearerToken);

            int userId = auth.GetUserIdByToken();
            int imeiId = auth.GetImeiIdByToken();

            if (userId != 0)
            {
                LogWriter.Write(userId, imeiId, $"api/erp/getStatus({body.Guid})", $"Request to API");

                try
                {
                    string guery = $"select ar.[Status], ar.[Descr] from ApiRequests ar where ar.Guid = '{body.Guid.ToString()}'";

                    using (SqlConnection _connection = new SqlConnection(Startup.ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand(guery, _connection);
                        _connection.Open();

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                answer.Guid = body.Guid;
                                answer.Status = (RequestStatus)reader.GetValue(0);
                                answer.Result = answer.Status.ToString();
                            }
                        }
                        else
                        {
                            answer.Guid = body.Guid;
                            answer.Status = RequestStatus.NotFound;
                            answer.Result = $"Request with GUID '{body.Guid}' is not found";
                        }
                    }
                }
                catch (Exception e)
                {
                    answer.Guid = body.Guid;
                    answer.Status = RequestStatus.Exception;
                    answer.Result = $"{e.Message}";
                    
                    return Json(answer);
                }
                
                return Json(answer);
            }
            else
            {
                answer.Result = "Token is invalid or has expired";
                
                return Json(answer);
            }
        }

        [HttpPost("getDescrShk")]
        [Produces("application/json")]
        public ActionResult<string> GetDescrShk([FromBody] RequestDescrShk rb)
        {
            var answer = new OperationAnswerBody();
            var bearerToken = Request.Headers["Authorization"].ToString().Substring(7);
            var auth = new Authorization(bearerToken);
            var ids = auth.GetIdsByToken();

            if (ids.UserId != 0)
            {
                LogWriter.Write(ids.UserId, ids.ImeiId, $"api/erp/getDescrShk({rb.actionName})", $"Request to API");

                var TuneValue = GetTunes.GetStringTune(_connection, "ESB_GetDescrShk");
                var request = (HttpWebRequest)WebRequest.Create(TuneValue);

                LogWriter.Write(ids.UserId, ids.ImeiId, $"TuneValue = ({TuneValue})", $"Request to API");

                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(new { rb.actionName });
                    
                    streamWriter.Write(json);
                }

                

                var result = string.Empty;
                try
                {
                    var httpResponse = (HttpWebResponse)request.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }

                    answer = JsonConvert.DeserializeObject<OperationAnswerBody>(result);                    
                }
                catch (Exception e)
                {
                    answer.Value = $"ERROR. {e.Message}";
                }
                
                LogWriter.Write(ids.UserId, ids.ImeiId, $"answer = ({answer.Value})", $"Request to API");
                
                return Json(answer);
            }
            else
            {
                LogWriter.Write(ids.UserId, ids.ImeiId, "Token is invalid or has expired", $"Request to API");
                
                answer.Value = "Token is invalid or has expired";
                
                return Json(answer);
            }
        }


        [HttpPost("getObjectById")]
        [Produces("application/json")]
        public ActionResult<string> GetObjectById([FromBody] RequestObjById rb)
        {
            var answer = new OperationAnswerBody();
            var bearerToken = Request.Headers["Authorization"].ToString().Substring(7);
            var auth = new Authorization(bearerToken);
            var ids = auth.GetIdsByToken();

            if (ids.UserId != 0)
            {
                LogWriter.Write(ids.UserId, ids.ImeiId, $"api/erp/getDescrShk({rb.actionName})", $"Request to API");

                var TuneValue = GetTunes.GetStringTune(_connection, "ESB_GetObjectById");
                var request = (HttpWebRequest)WebRequest.Create(TuneValue); 

                request.ContentType = "application/json";
                request.Method = "POST";
                request.Timeout = Timeout.Infinite;
                request.KeepAlive = true;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var requestObjectIdentity = new RequestObjByIdESB
                    {
                        ActionName = rb.actionName,
                        Ident = rb.ident,
                        UserName = auth.GetUserNameById(ids.UserId),
                        Imei = auth.GetImeiByToken()
                    };

                    var json = JsonConvert.SerializeObject(requestObjectIdentity);
                    
                    streamWriter.Write(json);
                }

                var result = string.Empty;
                try
                {
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }
                    answer = JsonConvert.DeserializeObject<OperationAnswerBody>(result);
                }
                catch (Exception e)
                {
                    answer.Value = $"ERROR. {e.Message}";
                }
                return Json(answer);

            }
            else
            {
                answer.Value = "Token is invalid or has expired";
                return Json(answer);
            }
        }
    }

    public enum RequestStatus : int
    {
        Created = 0,
        InProcess = 1,
        Completed = 2,
        Error = -1,
        NotFound = -2,
        Exception = -3
    }
}
