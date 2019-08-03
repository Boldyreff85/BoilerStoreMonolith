namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeToCategoryFeatures : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CategoryFeatures", "Category_Id", "dbo.Categories");
            DropIndex("dbo.CategoryFeatures", new[] { "Category_Id" });
            AddColumn("dbo.CategoryFeatures", "CategoryId", c => c.Int(nullable: false));
            DropColumn("dbo.CategoryFeatures", "Category_Id");
            DropTable("dbo.CategorySpecs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CategorySpecs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.CategoryFeatures", "Category_Id", c => c.Int());
            DropColumn("dbo.CategoryFeatures", "CategoryId");
            CreateIndex("dbo.CategoryFeatures", "Category_Id");
            AddForeignKey("dbo.CategoryFeatures", "Category_Id", "dbo.Categories", "Id");
        }
    }
}
