namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Apr2404Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AcquireEmails", "IsNewlyAcquired", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AcquireEmails", "IsNewlyAcquired");
        }
    }
}
