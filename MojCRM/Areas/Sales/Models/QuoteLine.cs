using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Areas.Sales.Models
{
    public class QuoteLine
    {
        [Key]
        public int Id { get; set; }
        public int RelatedQuoteId { get; set; }

        [ForeignKey("RelatedQuoteId")]
        public virtual Quote Quote { get; set; }

        public int LineNumber { get; set; }
        public int RelatedServiceId { get; set; }

        [ForeignKey("RelatedServiceId")]
        public virtual Service Service { get; set; }

        public string LineText { get; set; }
        public int Quantity { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}