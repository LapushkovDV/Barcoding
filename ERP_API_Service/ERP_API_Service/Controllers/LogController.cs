using System;
using System.Data.SqlClient;

namespace ERP_API_Service.Controllers
{
    public class LogWriter
    {
        private static readonly string connectionString = Startup.ConnectionString;
        private static SqlConnection _connection;

        static LogWriter()
        {
            _connection = new SqlConnection(connectionString);
        }

        public static void Write(string _eventType, string _message)
        {
            if (TestConnection.Test(_connection))
            {
                var guery = $"insert into ApiEvents ([DateTime],[Type],[Message]) " +
                               $" values (getDate(), '{_eventType}', '{_message}')";
                try
                {
                    _connection.Open();
                    
                    var cmd = new SqlCommand(guery, _connection);
                    
                    cmd.ExecuteScalar();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
        public static void Write(int _userId, string _eventType, string _message)
        {
            if (TestConnection.Test(_connection))
            {
                var guery = $"insert into ApiEvents ([UserId],[ImeiId],[DateTime],[Type],[Message]) " +
                               $" values ({_userId}, null, getDate(), '{_eventType}', '{_message}') ";
                try
                {
                    _connection.Open();
                    
                    var cmd = new SqlCommand(guery, _connection);
                    
                    cmd.ExecuteScalar();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
        public static void Write(int _userId, int _imeiId,  string _message, string _eventType)
        {
            if (TestConnection.Test(_connection))
            {
                var guery = $"insert into ApiEvents ([UserId],[ImeiId],[DateTime],[Type],[Message]) " +
                               $" values ({_userId}, {_imeiId}, getDate(), '{_eventType}', '{_message}') ";
                try
                {
                    _connection.Open();
                    
                    var cmd = new SqlCommand(guery, _connection);
                    
                    cmd.ExecuteScalar();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    }
}
