namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jan132019Update : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EducationNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RelatedEducationEntityId = c.Int(nullable: false),
                        User = c.String(),
                        Contact = c.String(),
                        Note = c.String(),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Educations", t => t.RelatedEducationEntityId, cascadeDelete: true)
                .Index(t => t.RelatedEducationEntityId);
            
            CreateTable(
                "dbo.Educations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EducationEntityTitle = c.String(),
                        RelatedCampaignId = c.Int(),
                        RelatedOrganizationId = c.Int(),
                        EducationEntityStatus = c.Int(nullable: false),
                        EducationRejectReason = c.Int(),
                        AtendeesNumber = c.Int(),
                        CreatedBy = c.String(),
                        IsAssigned = c.Boolean(nullable: false),
                        AssignedTo = c.String(),
                        LastUpdatedBy = c.String(),
                        LastContactedBy = c.String(),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        LastContactDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Campaigns", t => t.RelatedCampaignId)
                .ForeignKey("dbo.Organizations", t => t.RelatedOrganizationId)
                .Index(t => t.RelatedCampaignId)
                .Index(t => t.RelatedOrganizationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Educations", "RelatedOrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Educations", "RelatedCampaignId", "dbo.Campaigns");
            DropForeignKey("dbo.EducationNotes", "RelatedEducationEntityId", "dbo.Educations");
            DropIndex("dbo.Educations", new[] { "RelatedOrganizationId" });
            DropIndex("dbo.Educations", new[] { "RelatedCampaignId" });
            DropIndex("dbo.EducationNotes", new[] { "RelatedEducationEntityId" });
            DropTable("dbo.Educations");
            DropTable("dbo.EducationNotes");
        }
    }
}
