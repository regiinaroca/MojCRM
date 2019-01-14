using MojCRM.Areas.Campaigns.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Areas.Sales.Models
{
    public class Education
    {
        [Key]
        public int Id { get; set; }        
        public string EducationEntityTitle { get; set; }

        public int? RelatedCampaignId { get; set; }
        [ForeignKey("RelatedCampaignId")]
        public virtual Campaign RelatedCampaign { get; set; }

        public int? RelatedOrganizationId { get; set; }
        [ForeignKey("RelatedOrganizationId")]
        public virtual Organizations RelatedOrganization { get; set; }

        public EducationEntityStatusEnum EducationEntityStatus { get; set; }
        public EducationRejectReasonEnum? EducationRejectReason { get; set; }
        public int? AtendeesNumber { get; set; }
        public string CreatedBy { get; set; }
        public bool IsAssigned { get; set; }
        public string AssignedTo { get; set; }
        public string LastUpdatedBy { get; set; }
        public string LastContactedBy { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? LastContactDate { get; set; }

        public virtual ICollection<EducationNote> EducationNotes { get; set; }

        public enum EducationEntityStatusEnum
        {
            [Description("Kreirano")]
            Created,

            [Description("U kontaktu")]
            InContact,

            [Description("Poslan poziv")]
            InvitationSent,

            [Description("Odbijeno")]
            Rejected,

            [Description("Prihvaćeno")]
            Accepted
        }

        public string EducationEntityStatusString
        {
            get
            {
                switch (EducationEntityStatus)
                {
                    case EducationEntityStatusEnum.Created: return "Kreirano";
                    case EducationEntityStatusEnum.InContact: return "U kontaktu";
                    case EducationEntityStatusEnum.InvitationSent: return "Poslan poziv";
                    case EducationEntityStatusEnum.Rejected: return "Odbijeno";
                    case EducationEntityStatusEnum.Accepted: return "Prihvaćeno";
                }

                return "Status obrade";
            }
        }

        public enum EducationRejectReasonEnum
        {
            [Description("Neodgovarajući termin")]
            Appointment,

            [Description("Nezainteresiranost")]
            NoInteres,

            [Description("Nepotrebna usluga")]
            NoServiceNeed,

            [Description("Ne želi navesti")]
            NoInfo
        }

        public string EducationRejectReasonString
        {
            get
            {
                switch (EducationRejectReason)
                {
                    case EducationRejectReasonEnum.Appointment: return "Neodgovarajući termin";
                    case EducationRejectReasonEnum.NoInteres: return "Nezainteresiranost";
                    case EducationRejectReasonEnum.NoServiceNeed: return "Usluga nije potrebna";
                    case EducationRejectReasonEnum.NoInfo: return "Ne želi navesti";
                }

                return "Razlog odbijanja";
            }
        }
    }
}