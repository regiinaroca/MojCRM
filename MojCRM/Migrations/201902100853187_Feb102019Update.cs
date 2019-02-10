namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feb102019Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quotes", "Priority", c => c.Int(nullable: false));
            AddColumn("dbo.Leads", "Priority", c => c.Int(nullable: false));
            AddColumn("dbo.Opportunities", "Priority", c => c.Int(nullable: false));
            AddColumn("dbo.Educations", "Priority", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Educations", "Priority");
            DropColumn("dbo.Opportunities", "Priority");
            DropColumn("dbo.Leads", "Priority");
            DropColumn("dbo.Quotes", "Priority");
        }
    }
}
