using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MojCRM.Areas.Sales.Models;

namespace MojCRM.Areas.Campaigns.Models
{
    public class Campaign
    {
        [Key]
        public int CampaignId { get; set; }

        [Display(Name = "Naziv kampanje")]
        public string CampaignName { get; set; }

        [Display(Name = "Opis kampanje")]
        public string CampaignDescription { get; set; }

        [Display(Name = "Pokrenuo")]
        public string CampaignInitiatior { get; set; }
        public int RelatedCompanyId { get; set; }

        [ForeignKey("RelatedCompanyId")]
        public virtual Organizations RelatedCompany { get; set; }

        [Display(Name = "Tip")]
        public CampaignTypeEnum CampaignType { get; set; }

        [Display(Name = "Status")]
        public CampaignStatusEnum CampaignStatus { get; set; }

        [Display(Name = "Atributi kampanje")]
        public string CampaignAttributes { get; set; }

        [Display(Name = "Početak")]
        public DateTime CampaignStartDate { get; set; }

        [Display(Name = "Predviđeni završetak")]
        public DateTime CampaignPlannedEndDate { get; set; }

        [Display(Name = "Završetak")]
        public DateTime? CampaignEndDate { get; set; }

        [Display(Name = "Datum početka ugovora")]
        public DateTime? ContractStartDate { get; set; }

        [Display(Name = "Datum kreiranja")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "Datum promjene")]
        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<AcquireEmail> AcquireEmails { get; set; }
        public virtual ICollection<Quote> Quotes { get; set; }

        public enum CampaignTypeEnum
        {
            [Description("Test")]
            Test,

            [Description("Prikupljanje e-mail adresa")]
            EmailBases,

            [Description("Prodajna kampanja")]
            Sales,

            [Description("CRM kampanja")]
            Crm
        }

        public enum CampaignStatusEnum
        {
            [Description("Pokrenuto")]
            Start,

            [Description("U tijeku")]
            InProgress,

            [Description("Privremeno zaustavljeno")]
            Hold,

            [Description("Prekinuto")]
            Ended,

            [Description("Završeno")]
            Completed,

            [Description("Baza za tipsku")]
            ReadyForEmailNotification,

            [Description("Baza za tipsku - cross")]
            ReadyForEmailNotificationCross,

            [Description("Završeno - cross")]
            CompletedCross
        }

        public string CampaignTypeString
        {
            get
            {
                switch (CampaignType)
                {
                    case CampaignTypeEnum.Test: return "Test";
                    case CampaignTypeEnum.EmailBases: return "Ažuriranje baze korisnika";
                    case CampaignTypeEnum.Sales: return "Prodajna kampanja";
                    case CampaignTypeEnum.Crm: return "CRM kampanja";
                }
                return "Tip kampanje";
            }
        }
        public string CampaignStatusString
        {
            get
            {
                switch (CampaignStatus)
                {
                    case CampaignStatusEnum.Start: return "Pokrenuto";
                    case CampaignStatusEnum.InProgress: return "U tijeku";
                    case CampaignStatusEnum.Hold: return "Privremeno zaustavljeno";
                    case CampaignStatusEnum.Ended: return "Prekinuto";
                    case CampaignStatusEnum.Completed: return "Završeno";
                    case CampaignStatusEnum.ReadyForEmailNotification: return "Baza za tipsku";
                    case CampaignStatusEnum.ReadyForEmailNotificationCross: return "Baza za tipsku - cross";
                    case CampaignStatusEnum.CompletedCross: return "Završeno - cross";
                }
                return "Status kampanje";
            }
        }
    }
}