using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class DatabaseContext : DbContext
    {
        public DbSet<MessageModel> Messages { get; set; }

        public DbSet<DeviceModel> Devices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=GatewayDatabase.db");
        }
    }
}