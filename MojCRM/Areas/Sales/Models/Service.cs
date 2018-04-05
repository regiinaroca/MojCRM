using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MojCRM.Areas.Sales.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        public int? MerId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string ServiceTitle { get; set; }
        public bool IsActive { get; set; }
        public bool IsFixedAmount { get; set; }
        public decimal BasePrice { get; set; }
        public ServiceBaseQuantityEnum BaseQuantity { get; set; }
        public ServiceTypeEnum ServiceType { get; set; }
        public DateTime InsertDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<QuoteLine> QuoteLines { get; set; }

        public enum ServiceBaseQuantityEnum
        {
            [Description("Komad")]
            Piece
        }

        public string ServiceBaseQuantityString
        {
            get
            {
                switch (BaseQuantity)
                {
                    case ServiceBaseQuantityEnum.Piece: return "Komad";
                }
                return "Jedinica mjere";
            }
        }

        public enum ServiceTypeEnum
        {
            [Description("eDokument")]
            Documents,

            [Description("Paketi")]
            Packages,

            [Description("Ostale usluge")]
            OtherServices
        }

        public string ServiceTypeString
        {
            get
            {
                switch (ServiceType)
                {
                    case ServiceTypeEnum.Documents: return "eDokumenti";
                    case ServiceTypeEnum.Packages: return "Paketi";
                    case ServiceTypeEnum.OtherServices: return "Ostale usluge";
                }
                return "Tip usluge";
            }
        }
    }
}