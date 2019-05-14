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
        @"������� ��������� ������������ ������� �� ���������� �������������� ������� ������������� ����, \n
                    �������� �������� ������������ ������������ ��������",
                Category = "���������",
                Price = "�� 55 500 ���./��.",
                Firm = "Protherm",
                Power = "28"
            };

            for (int i = 0; i <= 18; i++)
            {
                product.Title = "���� -- " + i;
                db.Products.Add(product);
                db.SaveChanges();
            }

            InfoEntity infoEntity = new InfoEntity
            {
                CompanyInfo = "���������� � ��������",
                Services = "�������� �����",
                Email = "email",
                Address = "�����",
                Schedule = "���� ������",
                PhoneMain = "+7 XXX XXX XXXX",
                PhoneAdditional = "+7 XXX XXX XXXX"
            };
            db.InfoEntities.AddOrUpdate(infoEntity);
            db.SaveChanges();
        }
    }
}
