using ERP_Admin_Panel.Services.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ERP_Admin_Panel.Services.Database
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options) { }

        #region Источники данных
        public DbSet<User>? Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleUser> RoleUsers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Setting> Settings { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Devices);
        }
    }
}
