namespace Datalayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EngineOptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EngineOptions", "Value", c => c.String());
            AddColumn("dbo.EngineOptions", "MinValue", c => c.String());
            AddColumn("dbo.EngineOptions", "MaxValue", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EngineOptions", "MaxValue");
            DropColumn("dbo.EngineOptions", "MinValue");
            DropColumn("dbo.EngineOptions", "Value");
        }
    }
}
