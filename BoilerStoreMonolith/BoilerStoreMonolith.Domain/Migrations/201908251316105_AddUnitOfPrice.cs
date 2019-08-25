namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUnitOfPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Currency", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Currency");
        }
    }
}
