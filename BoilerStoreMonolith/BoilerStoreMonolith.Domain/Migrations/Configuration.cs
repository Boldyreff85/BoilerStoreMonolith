namespace BoilerStoreMonolith.Domain.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<BoilerStoreMonolith.Domain.Concrete.ApplicationContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BoilerStoreMonolith.Domain.Concrete.ApplicationContext db)
        {

        }
    }
}
