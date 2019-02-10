using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MojCRM.Areas.Campaigns.Models;
using MojCRM.Areas.CRM.Models;
using MojCRM.Helpers;
using MojCRM.Models;

namespace MojCRM.Areas.Sales.Models
{
    public class Quote
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Broj ponude")]
        public string QuoteNumber { get; set; }

        public int RelatedOrganizationId { get; set; }

        [ForeignKey("RelatedOrganizationId")]
        public virtual Organizations Organization { get; set; }

        public int? RelatedCampaignId { get; set; }

        [ForeignKey("RelatedCampaignId")]
        public virtual Campaign Campaign { get; set; }

        public int? RelatedLeadId { get; set; }

        [ForeignKey("RelatedLeadId")]
        public virtual Lead Lead { get; set; }

        [DisplayName("Vlasnik ponude")]
        public string AssignedTo { get; set; }

        [DisplayName("Datum početka važenja")]
        public DateTime StartDate { get; set; }

        [DisplayName("Datum završetka važenja")]
        public DateTime EndDate { get; set; }

        public QuoteTypeEnum QuoteType { get; set; }
        public QuoteStatusEnum QuoteStatus { get; set; }

        [Display(Name = "Ukupan iznos ponude (bez PDV-a)")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [DataType(DataType.Currency)]
        public decimal QuoteSum { get; set; }

        [Display(Name = "Ukupan iznos ponude (s PDV-om)")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [DataType(DataType.Currency)]
        public decimal QuoteSumTotal { get; set; }

        [Display(Name = "Trajanje ugovora")]
        public int? ContractDuration { get; set; }
        [DisplayName("Opcija plaćanja obrade baza")]
        public bool? AcquireEmailPayment { get; set; }

        [DefaultValue(1)]
        public PriorityEnum Priority { get; set; }

        [DisplayName("Datum i vrijeme kreiranja")]
        public DateTime InsertDate { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? RecallDate { get; set; }
        public string RecalledBy { get; set; }

        public virtual ICollection<QuoteMember> QuoteMembers { get; set; }
        public virtual ICollection<QuoteLine> QuoteLines { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }

        public enum QuoteTypeEnum
        {
            [Description("Ugovor - slobodno slanje")]
            ContractFree,

            [Description("Ugovor - paketi")]
            ContractFixed,

            [Description("Avansna uplata")]
            AdvancePayment
        }

        [DisplayName("Tip ponude")]
        public string QuoteTypeString
        {
            get
            {
                switch (QuoteType)
                {
                    case QuoteTypeEnum.ContractFree: return "Ugovor - slobodno slanje";
                    case QuoteTypeEnum.ContractFixed: return "Ugovor - paketi";
                    case QuoteTypeEnum.AdvancePayment: return "Avansna uplata";
                }
                return "Tip ponude";
            }
        }

        public enum QuoteStatusEnum
        {
            [Description("Kreirana ponuda")]
            Created,

            [Description("Poslana ponuda")]
            Sent,

            [Description("Revidirana")]
            Reviewed,

            [Description("Prihvaćena ponuda")]
            Accepted,

            [Description("Prihvaćena nakon revidiranja")]
            AcceptedAfterReview,

            [Description("Odbijena")]
            Rejected,

            [Description("Opozvana")]
            Recalled
        }

        [DisplayName("Status ponude")]
        public string QuoteStatusString
        {
            get
            {
                switch (QuoteStatus)
                {
                    case QuoteStatusEnum.Created: return "Kreirana ponuda";
                    case QuoteStatusEnum.Sent: return "Poslana ponuda";
                    case QuoteStatusEnum.Reviewed: return "Revidirana ponuda";
                    case QuoteStatusEnum.Accepted: return "Prihvaćena ponuda";
                    case QuoteStatusEnum.AcceptedAfterReview: return "Prihvaćena ponuda nakon revidiranja";
                    case QuoteStatusEnum.Rejected: return "Odbijena ponuda";
                    case QuoteStatusEnum.Recalled: return "Opozvana ponuda";
                }
                return "Status ponude";
            }
        }

        [DisplayName("Opcija plaćanja obrade baza")]
        public string AcquireEmailPaymentString
        {
            get
            {
                switch (AcquireEmailPayment)
                {
                    case true: return "Uključeno";
                    case false: return "Nije uključeno";
                }
                return "Nije specificirano";
            }
        }
    }
}