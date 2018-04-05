namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dec25Update : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contracts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MerId = c.Int(nullable: false),
                        MerContractNumber = c.String(),
                        MerQuoteNumber = c.String(),
                        RelatedOrganizationId = c.Int(nullable: false),
                        RelatedQuoteId = c.Int(),
                        ContractedBy = c.String(),
                        MerContractNote = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        MerActivationDate = c.DateTime(nullable: false),
                        MerDeactivationDate = c.DateTime(),
                        MerDeactivationReason = c.String(),
                        GracePeriod = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsPartnerStatus = c.Boolean(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.RelatedOrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.Quotes", t => t.RelatedQuoteId)
                .Index(t => t.RelatedOrganizationId)
                .Index(t => t.RelatedQuoteId);
            
            CreateTable(
                "dbo.ContractedServices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RelatedContractId = c.Int(nullable: false),
                        RelatedServiceId = c.Int(nullable: false),
                        ContractedQuantity = c.Decimal(precision: 18, scale: 2),
                        ContractedPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.RelatedContractId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.RelatedServiceId, cascadeDelete: true)
                .Index(t => t.RelatedContractId)
                .Index(t => t.RelatedServiceId);
            
            CreateTable(
                "dbo.ContractRates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RelatedContractId = c.Int(nullable: false),
                        RateSequenceNumber = c.Int(nullable: false),
                        RateAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RateDate = c.DateTime(nullable: false),
                        RateDueDate = c.DateTime(nullable: false),
                        IsPaid = c.Boolean(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.RelatedContractId, cascadeDelete: true)
                .Index(t => t.RelatedContractId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contracts", "RelatedQuoteId", "dbo.Quotes");
            DropForeignKey("dbo.Contracts", "RelatedOrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.ContractRates", "RelatedContractId", "dbo.Contracts");
            DropForeignKey("dbo.ContractedServices", "RelatedServiceId", "dbo.Services");
            DropForeignKey("dbo.ContractedServices", "RelatedContractId", "dbo.Contracts");
            DropIndex("dbo.ContractRates", new[] { "RelatedContractId" });
            DropIndex("dbo.ContractedServices", new[] { "RelatedServiceId" });
            DropIndex("dbo.ContractedServices", new[] { "RelatedContractId" });
            DropIndex("dbo.Contracts", new[] { "RelatedQuoteId" });
            DropIndex("dbo.Contracts", new[] { "RelatedOrganizationId" });
            DropTable("dbo.ContractRates");
            DropTable("dbo.ContractedServices");
            DropTable("dbo.Contracts");
        }
    }
}
