using MojCRM.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MojCRM.Areas.Sales.Models;
using System.Collections.Generic;

namespace MojCRM.Areas.CRM.Models
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }
        public int MerId { get; set; }
        public string MerContractNumber { get; set; }
        public string MerQuoteNumber { get; set; }
        public int RelatedOrganizationId { get; set; }

        [ForeignKey("RelatedOrganizationId")]
        public virtual Organizations Organization { get; set; }

        public int? RelatedQuoteId { get; set; }

        [ForeignKey("RelatedQuoteId")]
        public virtual Quote Quote { get; set; }

        public string ContractedBy { get; set; }
        public string MerContractNote { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? MerActivationDate { get; set; }
        public DateTime? MerDeactivationDate { get; set; }
        public string MerDeactivationReason { get; set; }
        public int GracePeriod { get; set; }
        public bool IsActive { get; set; }
        public bool IsPartnerStatus { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<ContractedService> ContractedServices { get; set; }
        public virtual ICollection<ContractRate> ContractRates { get; set; }
    }
}