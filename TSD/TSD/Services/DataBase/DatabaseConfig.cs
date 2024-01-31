using SQLite;
using TSD.Services.DataBase.Interfaces;
using Xamarin.Forms;

namespace TSD.Services.DataBase
{
    public static class DatabaseConfig
    {
        readonly static string _dbPath = Resources.NameDataBase;
        
        /// <summary>
        /// Константа флагов SQLite
        /// </summary>
        public const SQLiteOpenFlags FLAGS = SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex;

        /// <summary>
        /// Путь до базы данных
        /// </summary>
        public static string PathDatabase = DependencyService.Get<IPath>().GetDatabasePath(_dbPath);
    }
}
