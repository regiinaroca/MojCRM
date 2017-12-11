using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Models;
using System;
using System.Collections;
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

        [Display(Name = "Početak")]
        public DateTime CampaignStartDate { get; set; }

        [Display(Name = "Predviđeni završetak")]
        public DateTime CampaignPlannedEndDate { get; set; }

        [Display(Name = "Završetak")]
        public DateTime? CampaignEndDate { get; set; }

        [Display(Name = "Datum kreiranja")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "Datum promjene")]
        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<AcquireEmail> AcquireEmails { get; set; }
        public virtual ICollection<Quote> Quotes { get; set; }

        public enum CampaignTypeEnum
        {
            [Description("Test")]
            TEST,

            [Description("Prikupljanje e-mail adresa")]
            EMAILBASES,

            [Description("Prodajna kampanja")]
            SALES,

            [Description("CRM kampanja")]
            CRM
        }

        public enum CampaignStatusEnum
        {
            [Description("Pokrenuto")]
            START,

            [Description("U tijeku")]
            INPROGRESS,

            [Description("Privremeno zaustavljeno")]
            HOLD,

            [Description("Prekinuto")]
            ENDED,

            [Description("Završeno")]
            COMPLETED
        }

        public string CampaignTypeString
        {
            get
            {
                switch (CampaignType)
                {
                    case CampaignTypeEnum.TEST: return "Test";
                    case CampaignTypeEnum.EMAILBASES: return "Ažuriranje baze korisnika";
                    case CampaignTypeEnum.SALES: return "Prodajna kampanja";
                    case CampaignTypeEnum.CRM: return "CRM kampanja";
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
                    case CampaignStatusEnum.START: return "Pokrenuto";
                    case CampaignStatusEnum.INPROGRESS: return "U tijeku";
                    case CampaignStatusEnum.HOLD: return "Privremeno zaustavljeno";
                    case CampaignStatusEnum.ENDED: return "Prekinuto";
                    case CampaignStatusEnum.COMPLETED: return "Završeno";
                }
                return "Status kampanje";
            }
        }
    }
}