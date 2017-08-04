using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static MojCRM.Areas.Sales.Models.Lead;

namespace MojCRM.Areas.Sales.Helpers
{
    public class LeadNoteHelper
    {
        public string[] NoteTemplates { get; set; }
        public int? RelatedLeadId { get; set; }
        public string User { get; set; }
        public string Contact { get; set; }
        public string Note { get; set; }
        public int? NoteId { get; set; }
        public string Email { get; set; }
        public int Identifier { get; set; }
        public bool IsActivity { get; set; }
    }

    public class LeadContactHelper
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Agent { get; set; }
        public int? RelatedLeadId { get; set; }
        public string ContactId { get; set; }
    }

    public class LeadAssignHelper
    {
        public string AssignedTo { get; set; }
        public int? RelatedLeadId { get; set; }
        public bool Unassign { get; set; }
    }

    public class LeadSearchHelper
    {
        public string Campaign { get; set; }
        public string Lead { get; set; }
        public string Organization { get; set; }
        public LeadStatusEnum? LeadStatus { get; set; }
        public LeadRejectReasonEnum? RejectReason { get; set; }
        public string Assigned { get; set; }
        public string AssignedTo { get; set; }
    }

    public class LeadEditHelper
    {
        public int LeadId { get; set; }
        public string LeadTitle { get; set; }
        public string LeadDescription { get; set; }
        public LeadStatusEnum LeadStatus { get; set; }
        public LeadRejectReasonEnum? RejectReason { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedTo { get; set; }
        public string LastContactedBy { get; set; }
        public string LastUpdatedBy { get; set; }
    }

    public class LeadChangeStatusHelper
    {
        public LeadStatusEnum NewStatus { get; set; }
        public int RelatedLeadId { get; set; }
    }

    public class LeadMarkRejectedHelper
    {
        public LeadRejectReasonEnum RejectReason { get; set; }
        public int RelatedLeadId { get; set; }
    }
}