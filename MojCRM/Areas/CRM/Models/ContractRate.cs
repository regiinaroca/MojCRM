using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MojCRM.Areas.CRM.Models
{
    public class ContractRate
    {
        [Key]
        public int Id { get; set; }
        public int RelatedContractId { get; set; }

        [ForeignKey("RelatedContractId")]
        public virtual Contract Contract { get; set; }

        public int RateSequenceNumber { get; set; }
        public decimal RateAmount { get; set; }
        public DateTime RateDate { get; set; }
        public DateTime RateDueDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}