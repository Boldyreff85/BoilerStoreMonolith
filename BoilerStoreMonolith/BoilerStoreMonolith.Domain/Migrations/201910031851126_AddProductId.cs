namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DescriptionFeatures", "ProductId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DescriptionFeatures", "ProductId");
        }
    }
}
