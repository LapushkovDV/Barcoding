using ERP_API_Service.Objects;
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace ERP_API_Service.Controllers
{
    public class Authorization
    {
        private SqlConnection _connection;
        private CredentialsBody _hashCreds;
        private CredentialsBody _creds;

        private readonly string token;

        public Authorization(CredentialsBody hashCreds, ref CredentialsBody creds)
        {
            var connectionString = Startup.ConnectionString;

            _connection = new SqlConnection(connectionString);

            _hashCreds = hashCreds;
            _creds = creds;
        }

        public Authorization(string _token)
        {
            string connectionString = Startup.ConnectionString;
            _connection = new SqlConnection(connectionString);

            token = _token;
        }

        public bool TestConn()
        {
            return TestConnection.Test(_connection);
        }

        public int CheckGetUserId()
        {
            if (TestConnection.Test(_connection))
            {
                int? loginID = 0;
                
                try
                {
                    var guery = $"select u.Id, u.Login, u.Password from Service_Users u where u.IsActive = 1";

                    _connection.Open();
                    
                    var cmd = new SqlCommand(guery, _connection);
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var login = (string)reader.GetValue(1);
                            var password = (string)reader.GetValue(2);

                            using (var sha256Hash = SHA256.Create())
                            {
                                if (VerifyHash(sha256Hash, login, _hashCreds.Login) && VerifyHash(sha256Hash, password, _hashCreds.Password))
                                {
                                    loginID = (int?)reader.GetValue(0);
                                    _creds.Login = login;
                                    _creds.Password = password;
                                    
                                    break;
                                }

                            }
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

                if (loginID != null)
                {
                    return (int)loginID;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public string GetUserNameById(int userId)
        {
            if (TestConnection.Test(_connection))
            {
                var userName = string.Empty;

                try
                {
                    var guery = $"select u.Id, u.Login from Service_Users u where u.Id = {userId} and u.IsActive = 1";

                    _connection.Open();
                    
                    var cmd = new SqlCommand(guery, _connection);
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            userName = (string)reader.GetValue(1);
                            
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

                return userName;
            }
            else
            {
                return string.Empty;
            }
        }
        public int GetUserIdByLogin(string login)
        {
            if (TestConnection.Test(_connection))
            {
                var userId = 0;

                try
                {
                    string guery = $"select u.Id, u.Login from Service_Users u where u.Login = {login} and u.IsActive = 1";

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

        public int CheckGetImeiId(int userId)
        {
            if (TestConnection.Test(_connection))
            {
                int? imeiId = 0;
                
                try
                {
                    var linkImei = GetTunes.LinkImeiAndUser(_connection);
                    var guery = $"select coalesce(i.Id,-1) as Id, coalesce(i.Imei,'') as Imei " +
                                   $"from User_Imei ui " +
                                   $"left join Imeis i on ui.imeiId = i.id and i.IsActive = 1" +
                                   $"where ui.userID = '{userId}'";

                    if (!linkImei)
                    {
                        guery = $"select coalesce(i.Id,-1) as Id, coalesce(i.Imei, '') as Imei " +
                                       $"from Imeis i " +
                                       $"where i.IsActive = 1 ";
                    }

                    _connection.Open();
                    
                    var cmd = new SqlCommand(guery, _connection);
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string imei = (string)reader.GetValue(1);

                            using (var sha256Hash = SHA256.Create())
                            {
                                if (VerifyHash(sha256Hash, imei, _hashCreds.Imei))
                                {
                                    imeiId = (int?)reader.GetValue(0);
                                    _creds.Imei = imei;
                                    
                                    break;
                                }
                            }
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

                if (imeiId != null)
                {
                    return (int)imeiId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public bool CheckImei(int userId)
        {
            if (TestConnection.Test(_connection))
            {
                var imeiExists = false;

                try
                {
                    var guery = $"select i.Imei " +
                                   $"from User_Imei ui " +
                                   $"left join Imeis i on ui.imeiId = i.id " +
                                   $"where ui.userID = '{userId}'";

                    _connection.Open();
                    
                    var cmd = new SqlCommand(guery, _connection);
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var imei = (string)reader.GetValue(0);

                            using (var sha256Hash = SHA256.Create())
                            {
                                if (VerifyHash(sha256Hash, imei, _hashCreds.Imei))
                                {
                                    imeiExists = true;
                                    _creds.Imei = imei;
                                    
                                    break;
                                }

                            }
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

                return imeiExists;
            }
            else
            {
                return false;
            }
        }

        public int GetUserIdByToken()
        {
            if (TestConnection.Test(_connection))
            {
                int? userId = 0;
                
                try
                {
                    var guery = $"select ut.userId " +
                                   $"from User_Tokens ut " +
                                   $"where ut.token = '{token}' and ut.ExpireDate > GETDATE()";

                    _connection.Open();
                    
                    var checkTok = new SqlCommand(guery, _connection);
                    
                    userId = (int?)checkTok.ExecuteScalar();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    _connection.Close();
                }

                if (userId != null)
                {
                    return (int)userId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public int GetImeiIdByToken()
        {
            if (TestConnection.Test(_connection))
            {
                int? imeiId = 0;
                try
                {
                    var guery = $"select ut.imeiId " +
                                   $"from User_Tokens ut " +
                                   $"where ut.token = '{token}' and ut.ExpireDate > GETDATE()";

                    _connection.Open();
                    
                    var checkTok = new SqlCommand(guery, _connection);
                    
                    imeiId = (int?)checkTok.ExecuteScalar();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    _connection.Close();
                }

                if (imeiId != null)
                {
                    return (int)imeiId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public string GetImeiByToken()
        {
            if (TestConnection.Test(_connection))
            {
                var imei = string.Empty;
                
                try
                {
                    var guery = $"select i.imei " +
                                   $"from User_Tokens ut " +
                                   $"left join Imeis i on ut.imeiId = i.Id " +
                                   $"where ut.token = '{token}' and ut.ExpireDate > GETDATE()";

                    _connection.Open();
                    
                    var checkTok = new SqlCommand(guery, _connection);
                    
                    imei = (string)checkTok.ExecuteScalar();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    _connection.Close();
                }
                return imei;
            }
            else
            {
                return string.Empty;
            }
        }

        public UserImeiIds GetIdsByToken()
        {
            var ids = new UserImeiIds();

            if (TestConnection.Test(_connection))
            {

                try
                {
                    string guery = $"select ut.userId, ut.imeiId " +
                                   $"from User_Tokens ut " +
                                   $"where ut.token = '{token}' and ut.ExpireDate > GETDATE()";

                    _connection.Open();
                    
                    var cmd = new SqlCommand(guery, _connection);
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ids.UserId = (int)reader.GetValue(0);
                            ids.ImeiId = (int)reader.GetValue(1);
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
                return ids;
            }
            else
            {
                return ids;
            }
        }

        public int GetActionIdByName(string actionName)
        {
            if (TestConnection.Test(_connection))
            {
                int? actionId = 0;
                
                try
                {
                    var guery = $"select a.Id " +
                                   $"from Actions a " +
                                   $"where a.Name = '{actionName}'";

                    _connection.Open();
                    
                    var getAction = new SqlCommand(guery, _connection);
                    actionId = (int?)getAction.ExecuteScalar();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    _connection.Close();
                }

                if (actionId != null)
                {
                    return (int)actionId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            var hashOfInput = GetHash(hashAlgorithm, input);

            var comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}
