namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeCategoryFeatures : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategoryFeatures", "FeatureId", c => c.Int(nullable: false));
            DropColumn("dbo.CategoryFeatures", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CategoryFeatures", "Name", c => c.String());
            DropColumn("dbo.CategoryFeatures", "FeatureId");
        }
    }
}
