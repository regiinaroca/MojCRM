namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul31Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityLogs", "Module", c => c.Int());
            AddColumn("dbo.Leads", "StatusDescription", c => c.String());
            AddColumn("dbo.Leads", "RejectReasonDescription", c => c.String());
            AddColumn("dbo.Opportunities", "StatusDescription", c => c.String());
            AddColumn("dbo.Opportunities", "RejectReasonDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Opportunities", "RejectReasonDescription");
            DropColumn("dbo.Opportunities", "StatusDescription");
            DropColumn("dbo.Leads", "RejectReasonDescription");
            DropColumn("dbo.Leads", "StatusDescription");
            DropColumn("dbo.ActivityLogs", "Module");
        }
    }
}
