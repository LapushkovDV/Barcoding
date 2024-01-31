using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ERP_Admin_Panel.Services.Database
{
    public class DbContextFactory : IDbContextFactory, IDisposable
    {
        private ApplicationContext _context;
        public ApplicationContext Instance => _context;

        public bool Create(string connectionString, DataProvider dataProvider)
        {
            var dbContextBuilder = new DbContextOptionsBuilder();
            switch (dataProvider)
            {
                case DataProvider.SQLServer:
                    {
                        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
                        dbContextBuilder.UseSqlServer(connectionString);
                        break;
                    }
                case DataProvider.PostgreSQL:
                    {
                        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                        dbContextBuilder.UseNpgsql(connectionString);
                        break;
                    }
                default: return false;
            }

            _context = new ApplicationContext(dbContextBuilder.Options);

            if (!IsConnect())
            {
                try
                {
                    _context.Database.Migrate();
                }
                catch (Exception)
                {
                    _context = null;

                    return false;
                }
            }

            return true;
        }

        public bool IsConnect()
        {
            if (_context == null) return false;

            try
            {
                _context.Database.OpenConnection();
                _context.Database.CloseConnection();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool MigrationDataBase()
        {
            if (_context == null) return false;
            if (!IsConnect()) return false;

            _context.Database.Migrate();

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
        }
    }
}
