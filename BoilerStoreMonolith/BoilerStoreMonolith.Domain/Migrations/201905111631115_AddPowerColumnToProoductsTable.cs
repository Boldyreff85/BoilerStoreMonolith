namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPowerColumnToProoductsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Power", c => c.String(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Power");
        }
    }
}
