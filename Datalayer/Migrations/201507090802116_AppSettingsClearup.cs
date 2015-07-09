namespace Datalayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppSettingsClearup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EngineOptions",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Type = c.Int(nullable: false),
                        Value = c.String(),
                        MinValue = c.String(),
                        MaxValue = c.String(),
                        Engine_Name = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Name)
                .ForeignKey("dbo.Engines", t => t.Engine_Name)
                .Index(t => t.Engine_Name);
            
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
            
            CreateTable(
                "dbo.Engines",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Author = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.GameHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastGame = c.Boolean(nullable: false),
                        Notation = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChessBoardPieces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.String(),
                        Type = c.Int(nullable: false),
                        Player = c.Int(nullable: false),
                        GameHistory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GameHistories", t => t.GameHistory_Id)
                .Index(t => t.GameHistory_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChessBoardPieces", "GameHistory_Id", "dbo.GameHistories");
            DropForeignKey("dbo.EngineOptionComboes", "Option_Name", "dbo.EngineOptions");
            DropForeignKey("dbo.EngineOptionComboes", "Engine_Name", "dbo.Engines");
            DropForeignKey("dbo.EngineOptions", "Engine_Name", "dbo.Engines");
            DropIndex("dbo.ChessBoardPieces", new[] { "GameHistory_Id" });
            DropIndex("dbo.EngineOptionComboes", new[] { "Option_Name" });
            DropIndex("dbo.EngineOptionComboes", new[] { "Engine_Name" });
            DropIndex("dbo.EngineOptions", new[] { "Engine_Name" });
            DropTable("dbo.ChessBoardPieces");
            DropTable("dbo.GameHistories");
            DropTable("dbo.Engines");
            DropTable("dbo.EngineOptionComboes");
            DropTable("dbo.EngineOptions");
        }
    }
}
