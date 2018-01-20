using System;
using System.Linq;
using MojCRM.Areas.Sales.Models;
using MojCRM.Models;

namespace MojCRM.Areas.Sales.Helpers
{
    public class QuoteSearchHelper
    {
        public string QuoteNumber { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationVat { get; set; }
        public int? QuoteType { get; set; }
    }

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

    public class AddQuoteLineHelper
    {
        public int RelatedQuoteId { get; set; }
        public int RelatedServiceId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class QuoteHelperMethods
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public void UpdateQuoteSum(int quoteId)
        {
            var quote = _db.Quotes.First(q => q.Id == quoteId);
            var lines = _db.QuoteLines.Where(ql => ql.RelatedQuoteId == quoteId).Sum(ql => ql.LineTotal);

            quote.QuoteSum = lines;
            quote.QuoteSumTotal = lines * (decimal)1.25;
            quote.UpdateDate = DateTime.Now;
            _db.SaveChanges();
        }
    }
}