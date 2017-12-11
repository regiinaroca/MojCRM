using MojCRM.Areas.Cooperation.Models;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Areas.Stats.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MojCRM.Areas.Sales.Models;

namespace MojCRM.Models
{
    public class Organizations
    {
        [Key]
        [Display(Name = "Moj-eRačun ID")]
        public int MerId { get; set; }

        [Display(Name = "OIB tvrtke")]
        public string VAT { get; set; }

        [Display(Name = "Naziv tvrtke")]
        public string SubjectName { get; set; }

        [Display(Name = "Poslovna jedinica")]
        public string SubjectBusinessUnit { get; set; }

        [Display(Name = "Prvi primljeni dokument")]
        public DateTime? FirstReceived { get; set; }

        [Display(Name = "Prvi poslani dokument")]
        public DateTime? FirstSent { get; set; }

        [Display(Name = "Broj zaposlenih")]
        public int? Employees { get; set; }

        [Display(Name = "Ukupan godišnji prihod")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [DataType(DataType.Currency)]
        public decimal? Income { get; set; }

        [Display(Name = "Pravni oblik")]
        public LegalFormEnum LegalForm { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }

        [Display(Name = "Partner")]
        public bool IsPartner { get; set; }

        [Display(Name = "SLA")]
        public bool HasSLA { get; set; }

        [Display(Name = "Informacijski posrednik")]
        public ServiceProviderEnum ServiceProvider { get; set; }

        [Display(Name = "Zadnju promjenu obavio")]
        public string LastUpdatedBy { get; set; }

        [Display(Name = "Datum unosa")]
        public DateTime? InsertDate { get; set; }

        [Display(Name = "Datum izmijene")]
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Datum zadnje sinkronizacije s Moj-eRačun")]
        public DateTime? MerUpdateDate { get; set; }

        public virtual ICollection<Delivery> Deliveries { get; set; }
        public virtual MerDeliveryDetails MerDeliveryDetail { get; set; }
        public virtual OrganizationDetail OrganizationDetail { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<DeliveryDetail> DeliveryDetails { get; set; }
        public virtual ICollection<MerIntegrationSoftware> IntegrationSoftware { get; set; }
        public virtual ICollection<MerDocumentExchangeHistory> DocumentExchanges { get; set; }
        public virtual ICollection<AcquireEmail> AqcuireEmails { get; set; }
        public virtual ICollection<Quote> Quotes { get; set; }

        public enum LegalFormEnum
        {
            [Description("Nije navedeno")]
            NOINFO,

            [Description("Društvo s ograničenom odgovornošću")]
            DOO,

            [Description("Jednostavno društvo s ograničenom odgovornošću")]
            JDOO,

            [Description("Dioničko društvo")]
            DD,

            [Description("Ostala trgovačka društva (Komanditno društvo, Javno trgovačko društvo")]
            KDJTD,

            [Description("Obrt")]
            OBRT,

            [Description("Ostali pravni oblici (Zadruge, OPG, Udruge, Ustanove i sl.")]
            OTHER
        }
        public enum ServiceProviderEnum
        {
            [Description("Moj-eRačun")]
            MER,

            [Description("FINA - Ministarstva")]
            FINAB2G,

            [Description("FINA - B2B")]
            FINAB2B
        }
    }
}