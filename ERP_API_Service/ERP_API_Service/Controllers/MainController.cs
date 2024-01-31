using ERP_API_Service.Managers;
using ERP_API_Service.Models;
using ERP_API_Service.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;


namespace ERP_API_Service.Controllers
{
    [Route("api/start")]
    [ApiController]
    public class MainController : Controller
    {
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Service is now running" };
        }
    }

    [Route("api/auth")]
    [ApiController]
    [Produces("application/json")]
    public class ApiAuthController : Controller
    {
        [HttpPost("gettoken")]
        public ActionResult<string> GetToken([FromBody] CredentialsBody hashCreds)
        {
            var answer = new AnswerBody();
            var creds = new CredentialsBody();
            var auth = new Authorization(hashCreds, ref creds);
            var checkConnection = auth.TestConn();

            if (checkConnection == false)
            {
                answer.Result = "No DataBase Connection";
                
                return Json(answer);
            }
            else
            {
                var userId = auth.CheckGetUserId();
                
                if (userId > 0)
                {
                    var imeiId = auth.CheckGetImeiId(userId);
                    
                    if (imeiId > 0)
                    {
                        var model = GetJWTContainerModel(creds.Login, creds.Password, creds.Imei);
                        var authService = new JWTService(model.SecretKey);
                        var token = authService.GenerateToken(model);

                        if (!authService.IsTokenValid(token))
                            throw new UnauthorizedAccessException();
                        else
                        {
                            var claims = authService.GetTokenClaims(token).ToList();
                        }

                        var tc = new TokenController(token, userId, imeiId);
                        var tokenBody = tc.InsertToken();

                        LogWriter.Write(userId, imeiId, $"api/auth/gettoken", $"Request to API");

                        return Json(tokenBody);
                    }
                    else
                    {
                        answer.Result = "IMEI is not allowed";
                        
                        LogWriter.Write(userId, $"api/auth/gettoken", $"Invalid request to API: IMEI is not allowed to UserId: {userId}");
                        
                        return Json(answer);
                    }
                }
                else
                {
                    answer.Result = "Invalid User or Password";
                    
                    LogWriter.Write($"api/auth/gettoken", $"Invalid request to API: Invalid User or Password");
                    
                    return Json(answer);
                }
            }

        }

        private static JWTContainerModel GetJWTContainerModel(string name, string email, string sid)
        {
            return new JWTContainerModel()
            {
                Claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Email, email)
                }
            };
        }
    }

    [Route("api/actions")]
    [ApiController]
    [Produces("application/json")]
    public class ApiActController : Controller
    {
        [HttpGet("getuseractions")]
        public ActionResult<string> GetUserActions()
        {
            var answer = new AnswerBody();
            var bearerToken = Request.Headers["Authorization"].ToString().Substring(7);
            var auth = new Authorization(bearerToken);
            var userId = auth.GetUserIdByToken();
            
            if (userId != 0)
            {
                var db = new DbController();
                var userRoles = db.FilUserRoles(userId);
                
                return Json(userRoles);
            }
            else
            {
                answer.Result = "Token is invalid or has expired";
                
                return Json(answer);
            }
        }
    }
}