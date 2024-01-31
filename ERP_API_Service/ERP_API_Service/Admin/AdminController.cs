using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERP_API_Service.Controllers
{
    [Route("api/admin")]
    public partial class AdminController : Controller
    {
        private readonly SqlConnection _connection;


        public AdminController()
        {
            var connectionString = Startup.ConnectionString;

            _connection = new SqlConnection(connectionString);
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewData["ShowLoginError"] = 0;
            
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            if (LoginPasswordIsValid(login, password))
            {
                var userId = GetUserIdByLogin(login);

                await Authenticate(login);

                LogWriter.Write(userId,"WebAdmin login", $"User {login} has logged in");

                return Redirect("/api/admin/menu");
            }
            else
            {
                LogWriter.Write($"WebAdmin login", $"Unsuccessful attempt to login with login: {login} and password: {password}");
                
                ViewData["ShowLoginError"] = 1;
                
                return View();
            }
        }

        [Authorize]
        [HttpGet("menu")]
        public IActionResult Menu()
        {
            return View();
        }

        [HttpGet("error")]
        public IActionResult _ErrorPage()
        {
            ViewData["Message"] = Startup.errorMessage;
            
            return View();
        }

        [HttpGet("logout_exit")]
        public async Task<IActionResult> Logout()
        {
            var login = User.Identity.Name;
            var userId = GetUserIdByLogin(login);
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            LogWriter.Write(userId,$"WebAdmin logout", $"User with login: {login} is logout.");
            
            return Redirect("/api/admin");
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };

            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public bool LoginPasswordIsValid(string login, string password)
        {
            if (TestConnection.Test(_connection))
            {
                var userExists = 0;

                try
                {
                    var guery = $"select count(*) from Service_Users u where u.Login = '{login}' and u.Password = '{password}' and u.IsAdmin = 1 and u.IsActive = 1";

                    _connection.Open();
                    
                    var cmd = new SqlCommand(guery, _connection);
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            userExists = (int)reader.GetValue(0);
                        }
                    }

                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    _connection.Close();
                }

                if (userExists >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public int GetUserIdByLogin(string login)
        {
            if (TestConnection.Test(_connection))
            {
                var userId = 0;

                try
                {
                    var guery = $"select u.Id, u.Login from Service_Users u where u.Login = '{login}' and u.IsActive = 1";

                    _connection.Open();
                    
                    var cmd = new SqlCommand(guery, _connection);
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            userId = (int)reader.GetValue(0);
                            
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    _connection.Close();
                }

                return userId;
            }
            else
            {
                return 0;
            }
        }
    }
}