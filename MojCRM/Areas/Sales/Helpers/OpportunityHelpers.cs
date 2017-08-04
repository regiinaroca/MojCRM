using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using static MojCRM.Areas.Sales.Models.Opportunity;

namespace MojCRM.Areas.Sales.Helpers
{
    public class OpportunityNoteHelper
    {
        public string[] NoteTemplates { get; set; }
        public int? RelatedOpportunityId { get; set; }
        public string User { get; set; }
        public string Contact { get; set; }
        public string Note { get; set; }
        public int? NoteId { get; set; }
        public string Email { get; set; }
        public int Identifier { get; set; }
        public bool IsActivity { get; set; }
    }

    public class SalesContactHelper
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string TitleFunction { get; set; }
        public int? RelatedEntityId { get; set; }
        public string ContactId { get; set; }
    }

    public class OpportunityAssignHelper
    {
        public string AssignedTo { get; set; }
        public int? RelatedOpportunityId { get; set; }
        public bool Unassign { get; set; }
    }

    public class OpportunitySearchHelper
    {
        public string Campaign { get; set; }
        public string Opportunity { get; set; }
        public string Organization { get; set; }
        public OpportunityStatusEnum? OpportunityStatus { get; set; }
        public OpportunityRejectReasonEnum? RejectReason { get; set; }
        public string Assigned { get; set; }
        public string AssignedTo { get; set; }
    }

    public class OpportunityEditHelper
    {
        public int OpportunityId { get; set; }
        public string OpportunityTitle { get; set; }
        public string OpportunityDescription { get; set; }
        public OpportunityStatusEnum OpportunityStatus { get; set; }
        public OpportunityRejectReasonEnum? RejectReason { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedTo { get; set; }
        public string LastContactedBy { get; set; }
        public string LastUpdatedBy { get; set; }
    }

    public class OpportunityChangeStatusHelper
    {
        public OpportunityStatusEnum NewStatus { get; set; }
        public int RelatedOpportunityId { get; set; }
    }

    public class OpportunityMarkRejectedHelper
    {
        public OpportunityRejectReasonEnum RejectReason { get; set; }
        public int RelatedOpportunityId { get; set; }
    }

    public class ConvertToLeadHelper
    {
        public string OrganizationName { get; set; }
        public string OpportunityDescription { get; set; }
        public int? RelatedCampaignId { get; set; }
        public string RelatedCampaignName { get; set; }
        public int OpportunityId { get; set; }
        public int? OrganizationId { get; set; }
        public string AssignedTo { get; set; }
        public bool IsAssigned { get; set; }
    }
}