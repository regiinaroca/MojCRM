using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MojCRM.Areas.Sales.Models;

namespace MojCRM.Areas.CRM.Models
{
    public class ContractedService
    {
        [Key]
        public int Id { get; set; }
        public int RelatedContractId { get; set; }

        [ForeignKey("RelatedContractId")]
        public virtual Contract Contract { get; set; }

        public int RelatedServiceId { get; set; }

        [ForeignKey("RelatedServiceId")]
        public virtual Service Service { get; set; }

        public decimal? ContractedQuantity { get; set; }
        public decimal ContractedPrice { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}