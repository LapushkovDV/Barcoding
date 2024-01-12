using ERP_Admin_Panel.Services.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;

namespace ERP_Admin_Panel.Services.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly string appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        public readonly IConfiguration _configuration;
        public ConfigurationManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            var json = File.ReadAllText(appSettingsPath);
            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.Converters.Add(new ExpandoObjectConverter());
            jsonSettings.Converters.Add(new StringEnumConverter());

            dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings);

            return config.ConnectionStrings.DbConnectionString;
        }
        public DataProvider GetTypeDataBase()
        {
            var json = File.ReadAllText(appSettingsPath);
            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.Converters.Add(new ExpandoObjectConverter());
            jsonSettings.Converters.Add(new StringEnumConverter());

            dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings);
            var typeDataBase = config.ConnectionStrings.DataBase;

            switch (typeDataBase)
            {
                case "SQLSERVER":
                    {
                        return DataProvider.SQLServer;
                    }
                case "POSTGRESQL":
                    {
                        return DataProvider.PostgreSQL;
                    }
                default: return DataProvider.SQLServer;
            }
        }
        public void SetTypeDataBase(DataProvider dataProvider)
        {
            var json = File.ReadAllText(appSettingsPath);
            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.Converters.Add(new ExpandoObjectConverter());
            jsonSettings.Converters.Add(new StringEnumConverter());

            dynamic config =  JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings);

            if (dataProvider == null) return;

            switch (dataProvider)
            {
                case DataProvider.SQLServer:
                    {
                        config.ConnectionStrings.DataBase = "SQLSERVER";
                        break;
                    }
                case DataProvider.PostgreSQL:
                    {
                        config.ConnectionStrings.DataBase = "POSTGRESQL";
                        break;
                    }
            }

            File.WriteAllText(appSettingsPath, JsonConvert.SerializeObject(config, Formatting.Indented, jsonSettings));
        }
        public void SetConnectionString(string connectionString)
        {
            var json = File.ReadAllText(appSettingsPath);
            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.Converters.Add(new ExpandoObjectConverter());
            jsonSettings.Converters.Add(new StringEnumConverter());

            dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings);

            config.ConnectionStrings.DbConnectionString = connectionString;

            File.WriteAllText(appSettingsPath, JsonConvert.SerializeObject(config, Formatting.Indented, jsonSettings));
        }

        public string FormatConnectionString(string host, string userName, string password, string dataBase = "ERPDB", DataProvider dataProvider = DataProvider.SQLServer, string port = "5433")
        {
            switch (dataProvider)
            {
                case DataProvider.SQLServer:
                    {
                        return $"Data Source={host};Initial Catalog={dataBase};UserName={userName};password={password}";

                    }
                case DataProvider.PostgreSQL:
                    {
                        return $"Host={host};Port={port};Database={dataBase};Username={userName};Password={password}";
                    }
                default: return string.Empty;
            }
        }

        public DataProvider FormatTypeDataBase(string typeDataBase)
        {
            switch (typeDataBase.ToUpper())
            {
                case "SQLSERVER":
                    {
                        return DataProvider.SQLServer;
                    }
                case "POSTGRESQL":
                    {
                        return DataProvider.PostgreSQL;
                    }
                default: return DataProvider.SQLServer;
            }
        }
    }
}
