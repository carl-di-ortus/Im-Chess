namespace Datalayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApplicationSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationSettings",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Name);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ApplicationSettings");
        }
    }
}
