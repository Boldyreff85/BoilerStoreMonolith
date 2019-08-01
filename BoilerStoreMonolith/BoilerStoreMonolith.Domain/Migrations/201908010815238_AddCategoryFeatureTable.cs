namespace BoilerStoreMonolith.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategoryFeatureTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CategorySpecs", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.Features", "Category_Id", "dbo.Categories");
            DropIndex("dbo.CategorySpecs", new[] { "Category_Id" });
            DropIndex("dbo.Features", new[] { "Category_Id" });
            CreateTable(
                "dbo.CategoryFeatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Category_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .Index(t => t.Category_Id);
            
            DropColumn("dbo.CategorySpecs", "Category_Id");
            DropColumn("dbo.Features", "Category_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Features", "Category_Id", c => c.Int());
            AddColumn("dbo.CategorySpecs", "Category_Id", c => c.Int());
            DropForeignKey("dbo.CategoryFeatures", "Category_Id", "dbo.Categories");
            DropIndex("dbo.CategoryFeatures", new[] { "Category_Id" });
            DropTable("dbo.CategoryFeatures");
            CreateIndex("dbo.Features", "Category_Id");
            CreateIndex("dbo.CategorySpecs", "Category_Id");
            AddForeignKey("dbo.Features", "Category_Id", "dbo.Categories", "Id");
            AddForeignKey("dbo.CategorySpecs", "Category_Id", "dbo.Categories", "Id");
        }
    }
}
