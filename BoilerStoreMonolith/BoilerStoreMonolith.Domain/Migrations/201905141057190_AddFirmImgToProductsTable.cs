namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFirmImgToProductsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "FirmImageData", c => c.Binary());
            AddColumn("dbo.Products", "FirmImageMimeType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "FirmImageMimeType");
            DropColumn("dbo.Products", "FirmImageData");
        }
    }
}
