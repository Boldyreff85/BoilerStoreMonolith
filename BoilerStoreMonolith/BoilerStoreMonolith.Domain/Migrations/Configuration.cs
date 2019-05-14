namespace BoilerStoreMonolith.Domain.Migrations
{
    using Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BoilerStoreMonolith.Domain.Concrete.ApplicationContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BoilerStoreMonolith.Domain.Concrete.ApplicationContext db)
        {
            Product product = new Product
            {
                Description =
        @"Газовый настенный отопительный аппарат со встроенным приготовлением горячей хозяйственной воды, \n
                    Мощность аппарата регулируется модулирующей горелкой",
                Category = "Настенные",
                Price = "от 55 500 руб./шт.",
                Firm = "Protherm",
                Power = "28"
            };

            for (int i = 0; i <= 18; i++)
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
            db.InfoEntities.AddOrUpdate(infoEntity);
            db.SaveChanges();
        }
    }
}
