using BoilerStoreMonolith.Domain.Entities;
using BoilerStoreMonolith.Domain.Concrete;
using System.Data.Entity;

namespace BoilerStoreMonolith.Domain.Concrete
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<InfoEntity> InfoEntities { get; set; }

        public ApplicationContext() : base("name=ApplicationContext") {
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<ApplicationContext, Migrations.Configuration>());
        }
    }
}
