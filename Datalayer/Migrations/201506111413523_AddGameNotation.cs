namespace Datalayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGameNotation : DbMigration
    {
        public override void Up()
        {
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
            DropIndex("dbo.ChessBoardPieces", new[] { "GameHistory_Id" });
            DropTable("dbo.ChessBoardPieces");
            DropTable("dbo.GameHistories");
        }
    }
}
