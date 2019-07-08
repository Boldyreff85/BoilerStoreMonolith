namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrectEmailSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InfoEntities", "Password", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InfoEntities", "Password");
        }
    }
}
