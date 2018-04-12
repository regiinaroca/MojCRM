namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nov23Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AcquireEmails", "AcquireEmailEntityStatus", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AcquireEmails", "AcquireEmailEntityStatus");
        }
    }
}
