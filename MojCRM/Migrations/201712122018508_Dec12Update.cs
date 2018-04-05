namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dec12Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quotes", "RelatedLeadId", c => c.Int());
            CreateIndex("dbo.Quotes", "RelatedLeadId");
            AddForeignKey("dbo.Quotes", "RelatedLeadId", "dbo.Leads", "LeadId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Quotes", "RelatedLeadId", "dbo.Leads");
            DropIndex("dbo.Quotes", new[] { "RelatedLeadId" });
            DropColumn("dbo.Quotes", "RelatedLeadId");
        }
    }
}
