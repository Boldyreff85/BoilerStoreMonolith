namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductChanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "Category", c => c.String());
            AlterColumn("dbo.Products", "Firm", c => c.String());
            DropColumn("dbo.Products", "CategoryImageData");
            DropColumn("dbo.Products", "CategoryImageMimeType");
            DropColumn("dbo.Products", "FirmImageData");
            DropColumn("dbo.Products", "FirmImageMimeType");
            DropColumn("dbo.Products", "Power");
            DropColumn("dbo.Products", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Price", c => c.String(nullable: false));
            AddColumn("dbo.Products", "Power", c => c.String(nullable: false));
            AddColumn("dbo.Products", "FirmImageMimeType", c => c.String());
            AddColumn("dbo.Products", "FirmImageData", c => c.Binary());
            AddColumn("dbo.Products", "CategoryImageMimeType", c => c.String());
            AddColumn("dbo.Products", "CategoryImageData", c => c.Binary());
            AlterColumn("dbo.Products", "Firm", c => c.String(nullable: false));
            AlterColumn("dbo.Products", "Category", c => c.String(nullable: false));
        }
    }
}
