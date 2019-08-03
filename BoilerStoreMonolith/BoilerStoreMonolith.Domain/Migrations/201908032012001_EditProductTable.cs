namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditProductTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Features", "Product_ProductID", "dbo.Products");
            DropIndex("dbo.Features", new[] { "Product_ProductID" });
            DropColumn("dbo.Features", "Product_ProductID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Features", "Product_ProductID", c => c.Int());
            CreateIndex("dbo.Features", "Product_ProductID");
            AddForeignKey("dbo.Features", "Product_ProductID", "dbo.Products", "ProductID");
        }
    }
}
