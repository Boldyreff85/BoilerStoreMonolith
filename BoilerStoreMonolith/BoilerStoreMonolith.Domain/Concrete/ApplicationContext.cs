using BoilerStoreMonolith.Domain.Entities;
using System.Data.Entity;

namespace BoilerStoreMonolith.Domain.Concrete
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<InfoEntity> InfoEntities { get; set; }

        static ApplicationContext()
        {
            Database.SetInitializer(new StoreDbInitializer());
        }

        public ApplicationContext() : base("ApplicationContext") { }

    }

    public class StoreDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationContext>
    {
        protected override void Seed(ApplicationContext db)
        {

            Product product = new Product
            {
                Description =
        @"Газовый настенный отопительный аппарат со встроенным приготовлением горячей хозяйственной воды, \n
            Мощность аппарата регулируется модулирующей горелкой",
                Category = "Настенный",
                Price = "от 55 500 руб./шт.",
                Firm = "Protherm"
            };

            for (int i = 0; i <= 36; i++)
            {
                product.Title = "Котёл -- " + i;
                db.Products.Add(product);
                db.SaveChanges();
            }

            InfoEntity infoEntity = new InfoEntity
            {
                CompanyInfo = "Информация о компании",
                Services = "Описание услуг",
                Email = "email",
                Address = "адрес",
                Schedule = "часы работы",
                PhoneMain = "+7 XXX XXX XXXX",
                PhoneAdditional = "+7 XXX XXX XXXX"
            };
            db.InfoEntities.Add(infoEntity);
            db.SaveChanges();
        }
    }

}
