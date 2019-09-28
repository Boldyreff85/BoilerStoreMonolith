namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFeatures : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Features", "Value");
            DropColumn("dbo.Features", "Unit");
            DropColumn("dbo.Features", "ProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Features", "ProductId", c => c.Int(nullable: false));
            AddColumn("dbo.Features", "Unit", c => c.String());
            AddColumn("dbo.Features", "Value", c => c.String());
        }
    }
}
