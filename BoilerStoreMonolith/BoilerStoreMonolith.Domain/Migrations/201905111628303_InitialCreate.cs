namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InfoEntities",
                c => new
                    {
                        InfoID = c.Int(nullable: false, identity: true),
                        CompanyInfo = c.String(),
                        Services = c.String(),
                        Email = c.String(),
                        Address = c.String(),
                        Schedule = c.String(),
                        PhoneMain = c.String(),
                        PhoneAdditional = c.String(),
                        ImageData = c.Binary(),
                        ImageMimeType = c.String(),
                        ImageData2 = c.Binary(),
                        ImageMimeType2 = c.String(),
                    })
                .PrimaryKey(t => t.InfoID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Category = c.String(nullable: false),
                        Firm = c.String(nullable: false),
                        Price = c.String(nullable: false),
                        ImageData = c.Binary(),
                        ImageMimeType = c.String(),
                    })
                .PrimaryKey(t => t.ProductID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Products");
            DropTable("dbo.InfoEntities");
        }
    }
}
