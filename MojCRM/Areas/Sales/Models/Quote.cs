using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MojCRM.Areas.Campaigns.Models;
using MojCRM.Models;

namespace MojCRM.Areas.Sales.Models
{
    public class Quote
    {
        [Key]
        public int Id { get; set; }
        public string QuoteNumber { get; set; }
        public int RelatedOrganizationId { get; set; }

        [ForeignKey("RelatedOrganizationId")]
        public virtual Organizations Organization { get; set; }

        public int? RelatedCampaignId { get; set; }

        [ForeignKey("RelatedCampaignId")]
        public virtual Campaign Campaign { get; set; }

        public string AssignedTo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public QuoteTypeEnum QuoteType { get; set; }
        public QuoteStatusEnum QuoteStatus { get; set; }
        public decimal QuoteSum { get; set; }
        public decimal QuoteSumTotal { get; set; }
        public DateTime InsertDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? RecallDate { get; set; }
        public string RecalledBy { get; set; }

        public virtual ICollection<QuoteMember> QuoteMembers { get; set; }
        public virtual ICollection<QuoteLine> QuoteLines { get; set; }

        public enum QuoteTypeEnum
        {
            [Description("Ugovor - slobodno slanje")]
            ContractFree,

            [Description("Ugovor - paketi")]
            ContractFixed,

            [Description("Avansna uplata")]
            AdvancePayment
        }

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
    }
}