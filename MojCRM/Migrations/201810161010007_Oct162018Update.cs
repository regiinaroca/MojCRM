namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Oct162018Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Campaigns", "ContractStartDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Campaigns", "ContractStartDate");
        }
    }
}
