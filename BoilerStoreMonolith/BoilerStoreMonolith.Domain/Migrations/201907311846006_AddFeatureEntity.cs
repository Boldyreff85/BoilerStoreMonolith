namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFeatureEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Features",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                        Category_Id = c.Int(),
                        Product_ProductID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .ForeignKey("dbo.Products", t => t.Product_ProductID)
                .Index(t => t.Category_Id)
                .Index(t => t.Product_ProductID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Features", "Product_ProductID", "dbo.Products");
            DropForeignKey("dbo.Features", "Category_Id", "dbo.Categories");
            DropIndex("dbo.Features", new[] { "Product_ProductID" });
            DropIndex("dbo.Features", new[] { "Category_Id" });
            DropTable("dbo.Features");
        }
    }
}
