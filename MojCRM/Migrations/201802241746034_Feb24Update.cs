namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feb24Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Campaigns", "CampaignAttributes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Campaigns", "CampaignAttributes");
        }
    }
}
