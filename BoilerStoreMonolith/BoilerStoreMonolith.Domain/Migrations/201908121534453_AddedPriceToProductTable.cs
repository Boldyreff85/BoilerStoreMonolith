namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPriceToProductTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Price", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Price");
        }
    }
}
