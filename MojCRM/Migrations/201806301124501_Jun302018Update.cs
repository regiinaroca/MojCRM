namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jun302018Update : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrganizationDetails", "TelephoneNumber", c => c.String(maxLength: 1000));
            AlterColumn("dbo.OrganizationDetails", "MobilePhoneNumber", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrganizationDetails", "MobilePhoneNumber", c => c.String());
            AlterColumn("dbo.OrganizationDetails", "TelephoneNumber", c => c.String());
        }
    }
}
