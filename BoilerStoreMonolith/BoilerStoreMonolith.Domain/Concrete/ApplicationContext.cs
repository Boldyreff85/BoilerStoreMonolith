using BoilerStoreMonolith.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;

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

    public class StoreDbInitializer : CreateDatabaseIfNotExists<ApplicationContext>
    {
        protected override void Seed(ApplicationContext db)
        {
            Random rnd = new Random();
            List<string> categories = new List<string>() {
                "Настенный", "Напольный", "Электрический"
            };

            List<string> firms = new List<string>() {
                "Protherm", "WIESSMANN"
            };

            List<Product> products = new List<Product>();

            for (int i = 0; i < 36; i++)
            {
                var category = categories[rnd.Next(categories.Count)];
                Product product = new Product
                {
                    Title = "Котёл -- " + i,
                    Description =
                    category + @" котёл со встроенным приготовлением горячей хозяйственной воды, \n
                                Мощность аппарата регулируется модулирующей горелкой",
                    Category = category,
                    Price = "от " + rnd.Next(30000, 80000) + " руб./шт.",
                    Firm = "",
                    Power = "28"
                };
                products.Add(product);
            }

            for (int i = 0; i < products.Count; i++)
            {
                products[i].Firm = firms[rnd.Next(firms.Count)];
            }

            for (int i = 0; i < products.Count; i++)
            {
                db.Products.Add(products[i]);
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
