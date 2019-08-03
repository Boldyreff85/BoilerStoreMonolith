namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditFeatureTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Features", "ProductId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Features", "ProductId");
        }
    }
}
