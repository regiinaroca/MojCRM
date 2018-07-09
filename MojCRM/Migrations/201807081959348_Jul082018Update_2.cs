namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul082018Update_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quotes", "AcquireEmailPayment", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quotes", "AcquireEmailPayment");
        }
    }
}
