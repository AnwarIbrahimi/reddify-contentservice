using Microsoft.EntityFrameworkCore;
using ContentService.Models;

namespace ContentService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {

        }
        public DbSet<Content> Contents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Host=postgres-service-content;Database=mydatabase;Username=myuser;Password=mypassword;");
        }
    }
}
