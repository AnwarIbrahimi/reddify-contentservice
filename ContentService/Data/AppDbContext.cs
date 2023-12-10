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
            optionsBuilder.UseSqlServer("Server=mssqlstud.fhict.local;Database=dbi465821_content;User Id=dbi465821_content;Password=Voucugklir2;TrustServerCertificate=True");
        }
    }
}
