namespace Datalayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EngineOptions",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Type = c.Int(nullable: false),
                        Engine_Name = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Name)
                .ForeignKey("dbo.Engines", t => t.Engine_Name)
                .Index(t => t.Engine_Name);
            
            CreateTable(
                "dbo.Engines",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Author = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EngineOptions", "Engine_Name", "dbo.Engines");
            DropIndex("dbo.EngineOptions", new[] { "Engine_Name" });
            DropTable("dbo.Engines");
            DropTable("dbo.EngineOptions");
        }
    }
}
