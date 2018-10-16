namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Oct162018Update_2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contracts", "MerActivationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Contracts", "MerActivationDate", c => c.DateTime(nullable: false));
        }
    }
}
