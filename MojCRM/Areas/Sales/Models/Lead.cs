using MojCRM.Areas.Campaigns.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Areas.Sales.Models
{
    public class Lead
    {
        [Key]
        public int LeadId { get; set; }

        [Display(Name = "Naslov leada")]
        public string LeadTitle { get; set; }

        [Display(Name = "Opis leada")]
        public string LeadDescription { get; set; }
        public int? RelatedCampaignId { get; set; }
        [ForeignKey("RelatedOpportunity")]
        public int RelatedOpportunityId { get; set; }

        [Display(Name = "ID povezane tvrtke")]
        public int? RelatedOrganizationId { get; set; }
        public LeadStatusEnum LeadStatus { get; set; }
        public string StatusDescription { get; set; }
        public LeadRejectReasonEnum? RejectReason { get; set; }
        public string RejectReasonDescription { get; set; }
        public QuoteTypeLeadEnum? QuoteType { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Dodijeljeno agentu")]
        public string AssignedTo { get; set; }

        [Display(Name = "Zadnje kontaktirao")]
        public string LastContactedBy { get; set; }

        public string LastUpdatedBy { get; set; }

        [Display(Name = "Dodijeljeno")]
        public bool IsAssigned { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Datum i vrijeme zadnjeg kontakta")]
        public DateTime? LastContactDate { get; set; }

        [ForeignKey("RelatedCampaignId")]
        public virtual Campaign RelatedCampaign { get; set; }
        //[ForeignKey("RelatedOpportunityId")]
        public virtual Opportunity RelatedOpportunity { get; set; }
        [ForeignKey("RelatedOrganizationId")]
        public virtual Organizations RelatedOrganization { get; set; }
        public virtual ICollection<Quote> RelatedQuotes { get; set; }

        public enum LeadStatusEnum
        {
            [Description("Početno")]
            Start,

            [Description("U kontaktu")]
            Incontact,

            [Description("Odbijeno")]
            Rejected,

            [Description("Poslana ponuda")]
            Quotesent,

            [Description("Prihvaćena ponuda")]
            Accepted,

            [Description("Dogovoren sastanak")]
            Meeting,

            [Description("Procesne poteškoće")]
            Processdifficulties,
        }

        public enum LeadRejectReasonEnum
        {
            [Description("Ne želi navesti")]
            Noinfo,

            [Description("Nema interesa")]
            Nointerest,

            [Description("Previsoka cijena")]
            Price,

            [Description("Neadekvatna ponuda")]
            Quote,

            [Description("Drugi pružatelj usluga")]
            Serviceprovider,

            [Description("Nedostatak vremena za pokretanje projekta")]
            Notime,

            [Description("Dio strane grupacije / Strano vlasništvo")]
            Foreigncompany,

            [Description("Drugo / Ostalo")]
            Other
        }

        public enum QuoteTypeLeadEnum
        {
            [Description("PrePaid paket (Moj-eRačun)")]
            AdvanceeR,

            [Description("Ugovor (Moj-eRačun)")]
            ContracteR,

            [Description("Paket (Moj-DMS)")]
            Packagedms,

            [Description("Paket (Moj-eArhiv)")]
            PackageeA,

            [Description("Paket (Moj-eRačun + Moj-DMS)")]
            PackageeRdms,

            [Description("Paket (Moj-eRačun + Moj-eArhiv)")]
            PackageeReA,

            [Description("Paket (Moj-eArhiv + Moj-DMS")]
            PackageeAdms,

            [Description("Bundle paket (Moj-eRačun + Moj-DMS + Moj-eArhiv")]
            Bundle

        }

        [Display(Name = "Status leada")]
        public string LeadStatusString
        {
            get
            {
                switch(LeadStatus)
                {
                    case LeadStatusEnum.Start: return "Kreirano";
                    case LeadStatusEnum.Incontact: return "U kontaktu";
                    case LeadStatusEnum.Rejected: return "Odbijeno";
                    case LeadStatusEnum.Quotesent: return "Poslana ponuda";
                    case LeadStatusEnum.Accepted: return "Ponuda prihvaćena";
                    case LeadStatusEnum.Meeting: return "Dogovoren sastanak";
                    case LeadStatusEnum.Processdifficulties: return "Procesne poteškoće";
                }
                return "Status leada";
            }
        }

        [Display(Name = "Razlog odbijanja leada")]
        public string LeadRejectReasonString
        {
            get
            {
                switch (RejectReason)
                {
                    case LeadRejectReasonEnum.Noinfo: return "Ne želi navesti";
                    case LeadRejectReasonEnum.Nointerest: return "Nema interesa za uslugu";
                    case LeadRejectReasonEnum.Price: return "Previsoka cijena";
                    case LeadRejectReasonEnum.Quote: return "Neadekvatna ponuda";
                    case LeadRejectReasonEnum.Serviceprovider: return "Koristi drugog posrednika";
                    case LeadRejectReasonEnum.Notime: return "Nedostatak vremena za pokretanje projekta";
                    case LeadRejectReasonEnum.Foreigncompany: return "Dio strane grupacije / Strano vlasništvo";
                    case LeadRejectReasonEnum.Other: return "Drugo / Ostalo";
                }
                return "Nije odbijeno";
            }
        }
    }
}