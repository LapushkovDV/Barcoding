using SQLite;
using TSD.Model;
using TSD.Services.DataBase.Tables;

namespace TSD.Services.DataBase.DataBaseServices
{
    abstract public class BaseDatabaseService
    {
        internal string _pathDatabase = string.Empty;
        internal SQLiteOpenFlags _flags;

        public BaseDatabaseService()
        {
            _pathDatabase = DatabaseConfig.PathDatabase;
            _flags = DatabaseConfig.FLAGS;
        }

        internal Menu AddMenu(SQLiteConnection db, AbstractMenu menu)
        {
            var menuBase = db.Table<Menu>().FirstOrDefault(x => x.Action == menu.Action);

            if (menuBase == null)
            {
                menuBase = new Menu { Action = menu.Action };
                db.Insert(menuBase);
            }

            return menuBase;
        }
    }
}
