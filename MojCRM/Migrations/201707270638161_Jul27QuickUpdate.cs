namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul27QuickUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrganizationDetails", "OrganizationGroup", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrganizationDetails", "OrganizationGroup");
        }
    }
}
