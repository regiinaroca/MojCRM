namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul082018Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quotes", "ContractDuration", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quotes", "ContractDuration");
        }
    }
}
