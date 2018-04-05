namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jan21Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Opportunities", "OpportunityEntryChannel", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Opportunities", "OpportunityEntryChannel");
        }
    }
}
