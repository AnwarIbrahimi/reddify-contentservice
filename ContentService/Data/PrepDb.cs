using ContentService.Models;

namespace ContentService.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using ( var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext context)
        {
            if(!context.Contents.Any())
            {
                Console.WriteLine("--> Seeding Data...");

                context.Contents.AddRange(
                    new Content() { Name = "Dot Net", Publisher = "Micorsoft", Description = "Free" },
                    new Content() { Name = "SQL Server Express", Publisher = "Micorsoft", Description = "Free" },
                    new Content() { Name = "Kubernetes", Publisher = "CNCF", Description = "Free" });

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}
