namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPowerToProductsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Power", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Power");
        }
    }
}
