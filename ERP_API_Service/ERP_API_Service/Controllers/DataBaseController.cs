using ERP_API_Service.Objects;
using System;
using System.Data.SqlClient;

namespace ERP_API_Service.Controllers
{
    public class DbController
    {
        private SqlConnection _connection;
        public DbController()
        {
            var connectionString = Startup.ConnectionString;

            _connection = new SqlConnection(connectionString);
        }

        public UserMenuBody FilUserRoles(int _userId)
        {
            var user = new UserMenuBody(_userId);

            if (TestConnection.Test(_connection))
            {
                try
                {
                    var guery = $"select distinct m.ID, m.name, m.action " +
                                   $"from User_Roles ur " +
                                   $"left join Roles r on ur.roleId = r.Id " +
                                   $"left join Role_Menu rm on r.Id = rm.roleId " +
                                   $"left join Menus m on rm.menuId = m.Id " +
                                   $"where ur.userId = '{_userId}'";

                    _connection.Open();
                    
                    var checkTok = new SqlCommand(guery, _connection);
                    var reader = checkTok.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read()) 
                        {
                            var menu = new MenuBody
                            {
                                Id = (int)reader.GetValue(0),
                                Name = (string)reader.GetValue(1),
                                Action = (string)reader.GetValue(2)
                            };
                            
                            user.Menu.Add(menu);
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
                return user;
            }
            else
            {
                return user;
            }
        }

    }

    public static class TestConnection
    {
        public static bool Test(SqlConnection _connection)
        {
            var testquery = "Select 1";
            var cmd = new SqlCommand(testquery, _connection);
            
            try
            {
                _connection.Open();
                cmd.ExecuteNonQuery();
                _connection.Close();
                
                return true;
            }
            catch (Exception)
            {
                _connection.Close();
                
                return false;
            }
        }   
    }

    public static class GetTunes
    {

        public static bool LinkImeiAndUser(SqlConnection _connection)
        {
            var testquery = "select value from settings where name = 'LinkImeiUser'";
            var cmd = new SqlCommand(testquery, _connection);
            
            try
            {
                var result = true;
                
                _connection.Open();
                
                if ((string)cmd.ExecuteScalar() != "+") result = false;
                
                _connection.Close();
                
                return result;
            }
            catch (Exception)
            {

                _connection.Close();
                
                return false;
            }
        }
        public static string GetStringTune(SqlConnection _connection, string _tuneName)
        {
            var testquery = "select value from settings where name = '"+ _tuneName + "'";
            var cmd = new SqlCommand(testquery, _connection);
            
            try
            {
                string result = null;
                _connection.Open();
                result = (string)cmd.ExecuteScalar() ;
                _connection.Close();
                return result;
            }
            catch (Exception)
            {

                _connection.Close();
                return null;
            }
        }
    }
}
