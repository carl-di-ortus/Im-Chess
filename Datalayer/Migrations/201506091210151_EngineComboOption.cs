namespace Datalayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EngineComboOption : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EngineOptionComboes",
                c => new
                    {
                        ComboValue = c.String(nullable: false, maxLength: 128),
                        Engine_Name = c.String(maxLength: 128),
                        Option_Name = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ComboValue)
                .ForeignKey("dbo.Engines", t => t.Engine_Name)
                .ForeignKey("dbo.EngineOptions", t => t.Option_Name)
                .Index(t => t.Engine_Name)
                .Index(t => t.Option_Name);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EngineOptionComboes", "Option_Name", "dbo.EngineOptions");
            DropForeignKey("dbo.EngineOptionComboes", "Engine_Name", "dbo.Engines");
            DropIndex("dbo.EngineOptionComboes", new[] { "Option_Name" });
            DropIndex("dbo.EngineOptionComboes", new[] { "Engine_Name" });
            DropTable("dbo.EngineOptionComboes");
        }
    }
}
