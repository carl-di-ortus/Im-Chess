namespace Datalayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAppSettingsColumnType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ApplicationSettings", "Value", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ApplicationSettings", "Value", c => c.Int(nullable: false));
        }
    }
}
