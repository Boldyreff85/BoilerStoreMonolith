namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmailSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InfoEntities", "Host", c => c.String());
            AddColumn("dbo.InfoEntities", "Port", c => c.Int(nullable: false));
            AddColumn("dbo.InfoEntities", "doUseSsl", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InfoEntities", "doUseSsl");
            DropColumn("dbo.InfoEntities", "Port");
            DropColumn("dbo.InfoEntities", "Host");
        }
    }
}
