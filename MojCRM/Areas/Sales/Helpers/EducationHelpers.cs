using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static MojCRM.Areas.Sales.Models.Education;

namespace MojCRM.Areas.Sales.Helpers
{
    public class EducationSearchHelper
    {
        public string Campaign { get; set; }
        public string Education { get; set; }
        public string Organization { get; set; }
        public string LastContactDate { get; set; }
        public EducationEntityStatusEnum? EducationStatus { get; set; }
        public EducationRejectReasonEnum? RejectReason { get; set; }
        public string Assigned { get; set; }
    }

    public class EducationNoteHelper
    {
        public int RelatedEducationId { get; set; }
        public string User { get; set; }
        public string Contact { get; set; }
        public string Note { get; set; }
        public int? NoteId { get; set; }
        public string Email { get; set; }
        public int Identifier { get; set; }
        public bool IsActivity { get; set; }
    }

    public class EducationAssignHelper
    {
        public string AssignedTo { get; set; }
        public int? RelatedEducationId { get; set; }
        public bool Unassign { get; set; }
    }

    public class EducationChangeStatusHelper
    {
        public EducationEntityStatusEnum NewStatus { get; set; }
        public int RelatedEducationId { get; set; }
        public string StatusDescription { get; set; }
    }

    public class EducationMarkRejectedHelper
    {
        public EducationRejectReasonEnum RejectReason { get; set; }
        public int RelatedEducationId { get; set; }
        public string RejectReasonDescription { get; set; }
    }

    public class EducationImportResults
    {
        public int CampaignId { get; set; }
        public int ValidEntities { get; set; }
        public int InvalidEntities { get; set; }
        public int ImportedEntities { get; set; }
        public List<string> ValidVATs { get; set; }
        public List<string> InvalidVATs { get; set; }
    }

    public class AtendeesByEducationStatHelper
    {
        public string EducationName { get; set; }
        public DateTime CampaignStartDate { get; set; }
        public DateTime? CampaignPlannedEndDate { get; set; }
        public int? Atendees { get; set; }
    }
}