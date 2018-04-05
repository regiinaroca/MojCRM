namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dec11Update : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Quotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuoteNumber = c.String(),
                        RelatedOrganizationId = c.Int(nullable: false),
                        RelatedCampaignId = c.Int(),
                        AssignedTo = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        QuoteType = c.Int(nullable: false),
                        QuoteStatus = c.Int(nullable: false),
                        QuoteSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuoteSumTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsertDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        UpdateDate = c.DateTime(),
                        UpdatedBy = c.String(),
                        RecallDate = c.DateTime(),
                        RecalledBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Campaigns", t => t.RelatedCampaignId)
                .ForeignKey("dbo.Organizations", t => t.RelatedOrganizationId, cascadeDelete: true)
                .Index(t => t.RelatedOrganizationId)
                .Index(t => t.RelatedCampaignId);
            
            CreateTable(
                "dbo.QuoteLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RelatedQuoteId = c.Int(nullable: false),
                        LineNumber = c.Int(nullable: false),
                        RelatedServiceId = c.Int(nullable: false),
                        LineText = c.String(),
                        Quantity = c.Int(nullable: false),
                        BaseAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quotes", t => t.RelatedQuoteId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.RelatedServiceId, cascadeDelete: true)
                .Index(t => t.RelatedQuoteId)
                .Index(t => t.RelatedServiceId);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        ServiceId = c.Int(nullable: false, identity: true),
                        MerId = c.Int(),
                        ServiceName = c.String(),
                        ServiceDescription = c.String(),
                        ServiceTitle = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsFixedAmount = c.Boolean(nullable: false),
                        BasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BaseQuantity = c.Int(nullable: false),
                        ServiceType = c.Int(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        UpdateDate = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.ServiceId);
            
            CreateTable(
                "dbo.QuoteMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RelatedQuoteId = c.Int(nullable: false),
                        MemberName = c.String(),
                        QuoteMemberRole = c.Int(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quotes", t => t.RelatedQuoteId, cascadeDelete: true)
                .Index(t => t.RelatedQuoteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuoteMembers", "RelatedQuoteId", "dbo.Quotes");
            DropForeignKey("dbo.QuoteLines", "RelatedServiceId", "dbo.Services");
            DropForeignKey("dbo.QuoteLines", "RelatedQuoteId", "dbo.Quotes");
            DropForeignKey("dbo.Quotes", "RelatedOrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Quotes", "RelatedCampaignId", "dbo.Campaigns");
            DropIndex("dbo.QuoteMembers", new[] { "RelatedQuoteId" });
            DropIndex("dbo.QuoteLines", new[] { "RelatedServiceId" });
            DropIndex("dbo.QuoteLines", new[] { "RelatedQuoteId" });
            DropIndex("dbo.Quotes", new[] { "RelatedCampaignId" });
            DropIndex("dbo.Quotes", new[] { "RelatedOrganizationId" });
            DropTable("dbo.QuoteMembers");
            DropTable("dbo.Services");
            DropTable("dbo.QuoteLines");
            DropTable("dbo.Quotes");
        }
    }
}
