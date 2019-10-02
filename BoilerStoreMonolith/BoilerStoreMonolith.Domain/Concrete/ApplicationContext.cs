using BoilerStoreMonolith.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace BoilerStoreMonolith.Domain.Concrete
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryFeature> CategoryFeatures { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<DescriptionFeature> DescriptionFeatures { get; set; }
        public DbSet<ProductFeature> ProductFeatures { get; set; }
        public DbSet<Firm> Firms { get; set; }
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
            List<Category> categories = new List<Category>() {
                new Category()
                {
                    Name = "Настенный"
                },
                new Category()
                {
                    Name = "Напольный"
                },
                new Category()
                {
                    Name = "Электрический"
                }

            };

            db.Categories.AddRange(categories);
            db.SaveChanges();

            List<Firm> firms = new List<Firm>() {
                new Firm()
                {
                    Name = "Protherm"
                },
                new Firm()
                {
                    Name = "WIESSMANN"
                }
            };

            db.Firms.AddRange(firms);
            db.SaveChanges();

            List<Product> products = new List<Product>();

            for (int i = 0; i < 36; i++)
            {
                var category = categories[rnd.Next(categories.Count)];
                Product product = new Product
                {
                    Title = "Котёл -- " + i,
                    Description =
                    category.Name + @" котёл со встроенным приготовлением горячей хозяйственной воды, \n
                                Мощность аппарата регулируется модулирующей горелкой",
                    Category = category.Name,
                    Firm = firms[0].Name,
                    Price = "100",
                    Currency = "Рублей"
                };
                products.Add(product);
            }

            for (int i = 0; i < products.Count; i++)
            {
                products[i].Firm = firms[rnd.Next(firms.Count)].Name;
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
                Address = "адрес",
                Schedule = "часы работы",
                PhoneMain = "+7 XXX XXX XXXX",
                PhoneAdditional = "+7 XXX XXX XXXX",
                Email = "vitalisun2@gmail.com",
                Host = "smtp.gmail.com",
                Port = 465,
                doUseSsl = true
            };
            db.InfoEntities.Add(infoEntity);
            db.SaveChanges();
        }
    }

}
