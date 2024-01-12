using ERP_API_Service.Objects;
using System;
using System.Data.SqlClient;

namespace ERP_API_Service.Controllers
{
    public class TokenController
    {
        private SqlConnection _connection;
        private readonly int _userId;
        private readonly string _token;
        private readonly int _imeiId;

        public TokenController(string token, int userId, int imeiId)
        {
            _userId = userId;
            _token = token;
            _imeiId = imeiId;

            var connectionString = Startup.ConnectionString;

            _connection = new SqlConnection(connectionString);
        }


        public TokenBody InsertToken()
        {
            var tokenBody = new TokenBody
            {
                ExpireDate = DateTime.MinValue
            };
            
            var lifetime = 60;
            
            try
            {
                if (_userId != 0)
                {
                    var query = $"select value from settings where name = 'LifeTimeToken';";
                    var ltt = string.Empty;
                    var cmd = new SqlCommand(query, _connection);
                    
                    _connection.Open();
                    
                    ltt = (string)cmd.ExecuteScalar();
                    
                    if (!int.TryParse(ltt, out lifetime))
                    {
                        lifetime = 1;
                    }
                }
                else
                {
                    tokenBody.Message = "User not found";
                }
            }
            catch (Exception e)
            {
                tokenBody.Message = e.Message;
                
                LogWriter.Write(_userId, _imeiId, $"api/auth/gettoken", e.Message);
                
                return tokenBody;
            }
            finally
            {
                _connection.Close();
            }
            
            if (lifetime < 1) lifetime = 60;
            
            try
            {
                if (_userId != 0)
                {
                    var expireDate = DateTime.UtcNow.AddMinutes(lifetime);
                    var query = $"insert into User_Tokens (userID, imeiId, token, CreateDate, ExpireDate) values ({_userId}, {_imeiId}, '{_token}', getdate(), '{expireDate}');";
                    var cmd = new SqlCommand(query, _connection);
                    
                    _connection.Open();
                    cmd.ExecuteNonQuery();

                    tokenBody.ExpireDate = expireDate;
                    tokenBody.Token = _token;
                    tokenBody.Message = "OK";

                } 
                else
                {
                    tokenBody.Message = "User not found";
                }
            }
            catch (Exception e)
            {
                tokenBody.Message = e.Message;
                
                LogWriter.Write(_userId, _imeiId, $"api/auth/gettoken", e.Message);
                
                return tokenBody;
            }
            finally
            {
                _connection.Close();
            }

            return tokenBody;
        }
    }
}
