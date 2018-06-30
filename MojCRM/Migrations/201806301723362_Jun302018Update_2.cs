namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jun302018Update_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MerDeliveryDetails", "EmailAddressForVerification", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MerDeliveryDetails", "EmailAddressForVerification");
        }
    }
}
