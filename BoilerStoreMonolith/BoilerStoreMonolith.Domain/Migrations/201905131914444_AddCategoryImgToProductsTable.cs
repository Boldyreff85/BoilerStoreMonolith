namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategoryImgToProductsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "CategoryImageData", c => c.Binary());
            AddColumn("dbo.Products", "CategoryImageMimeType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "CategoryImageMimeType");
            DropColumn("dbo.Products", "CategoryImageData");
        }
    }
}
