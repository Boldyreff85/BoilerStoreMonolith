namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSpecValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategorySpecs", "Value", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CategorySpecs", "Value");
        }
    }
}
