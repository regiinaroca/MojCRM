using System;
using MojCRM.Areas.Sales.Models;

namespace MojCRM.Areas.Sales.Helpers
{
    public class CreateFromLeadViewQuoteHelper
    {
        public int RelatedOrganizationId { get; set; }
        public int? RelatedCampaignId { get; set; }
    }

    public class CreateQuoteHelper
    {
        public int OrganizationId { get; set; }
        public int? CampaignId { get; set; }
        public int? LeadId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Quote.QuoteTypeEnum QuoteType { get; set; }
    }
}