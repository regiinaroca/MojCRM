using MojCRM.Areas.Campaigns.Models;
using MojCRM.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Areas.Sales.Models
{
    public class Opportunity
    {
        [Key]
        public int OpportunityId { get; set; }

        [Display(Name = "Naziv prodajne prilike")]
        public string OpportunityTitle { get; set; }

        [Display(Name = "Opis prodajne prilike")]
        public string OpportunityDescription { get; set; }
        public int? RelatedCampaignId { get; set; }
        public int? RelatedOrganizationId { get; set; }

        [Display(Name = "Status")]
        public OpportunityStatusEnum OpportunityStatus { get; set; }
        public string StatusDescription { get; set; }

        [Display(Name = "Razlog odbijanja")]
        public OpportunityRejectReasonEnum? RejectReason { get; set; }
        public string RejectReasonDescription { get; set; }

        [Display(Name = "Kanal kreiranja")]
        public OpportunityEntryChannelEnum? OpportunityEntryChannel { get; set; }

        [Display(Name = "Kreirao")]
        public string CreatedBy { get; set; }

        [Display(Name = "Dodijeljeno agentu")]
        public string AssignedTo { get; set; }

        [Display(Name = "Zadnje kontaktirao")]
        public string LastContactedBy { get; set; }

        [Display(Name = "Zadnje mijenjao")]
        public string LastUpdatedBy { get; set; }

        [Display(Name = "Dodijeljeno")]
        public bool IsAssigned { get; set; }

        [Display(Name = "Datum kreiranja")]
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Datum zadnjeg kontakta")]
        public DateTime? LastContactDate { get; set; }

        [ForeignKey("RelatedCampaignId")]
        public virtual Campaign RelatedCampaign { get; set; }
        [ForeignKey("RelatedOrganizationId")]
        public virtual Organizations RelatedOrganization { get; set; }

        public enum OpportunityStatusEnum
        {
            [Description("Početno")]
            Start,

            [Description("U kontaktu")]
            Incontact,

            [Description("Lead")]
            Lead,

            [Description("Odbijeno")]
            Rejected,

            [Description("Potrebno dogovoriti sastanak")]
            Arrangemeeting,

            [Description("Procesne poteškoće")]
            Processdifficulties,

            [Description("Moj-eRačun korisnik")]
            Meruser,

            [Description("FINA korisnik")]
            Finauser,

            [Description("eFaktura korisnik")]
            EFakturauser,

            [Description("Zatvorena tvrtka")]
            ClosedSubject
        }

        public enum OpportunityRejectReasonEnum
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

        public enum OpportunityEntryChannelEnum
        {
            [Description("Web-forma")]
            Web,

            [Description("Info-telefon")]
            InfoTelephone,

            [Description("Info-mail")]
            InfoMail
        }

        public string OpportunityStatusString
        {
            get
            {
                switch(OpportunityStatus)
                {
                    case OpportunityStatusEnum.Start: return "Kreirano";
                    case OpportunityStatusEnum.Incontact: return "U kontaktu";
                    case OpportunityStatusEnum.Lead: return "Kreiran lead";
                    case OpportunityStatusEnum.Rejected: return "Odbijeno";
                    case OpportunityStatusEnum.Arrangemeeting: return "Potrebno dogovoriti sastanak";
                    case OpportunityStatusEnum.Processdifficulties: return "Procesne poteškoće";
                    case OpportunityStatusEnum.Meruser: return "Moj-eRačun korisnik";
                    case OpportunityStatusEnum.Finauser: return "FINA korisnik";
                    case OpportunityStatusEnum.EFakturauser: return "eFaktura korisnik";
                    case OpportunityStatusEnum.ClosedSubject: return "Zatvorena tvrtka";
                }
                return "Status prodajne prilike";
            }
        }

        public string OpportunityRejectReasonString
        {
            get
            {
                switch(RejectReason)
                {
                    case OpportunityRejectReasonEnum.Noinfo: return "Ne želi navesti";
                    case OpportunityRejectReasonEnum.Nointerest: return "Nema interesa za uslugu";
                    case OpportunityRejectReasonEnum.Price: return "Previsoka cijena";
                    case OpportunityRejectReasonEnum.Quote: return "Neadekvatna ponuda";
                    case OpportunityRejectReasonEnum.Serviceprovider: return "Koristi drugog posrednika";
                    case OpportunityRejectReasonEnum.Notime: return "Nedostatak vremena za pokretanje projekta";
                    case OpportunityRejectReasonEnum.Foreigncompany: return "Dio strane grupacije / Strano vlasništvo";
                    case OpportunityRejectReasonEnum.Other: return "Drugo / Ostalo";
                }
                return "Nije odbijeno";
            }
        }

        public string OpportunityEntryChannelString
        {
            get
            {
                switch (OpportunityEntryChannel)
                {
                    case OpportunityEntryChannelEnum.Web: return "Web-forma";
                    case OpportunityEntryChannelEnum.InfoTelephone: return "Info telefon";
                    case OpportunityEntryChannelEnum.InfoMail: return "Info email";
                }
                return "Nije poznato";
            }
        }
    }
}