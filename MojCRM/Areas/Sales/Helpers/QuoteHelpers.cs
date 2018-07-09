using System;
using System.Linq;
using MojCRM.Areas.Sales.Models;
using MojCRM.Models;
using System.Collections.Generic;

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

    public class FillQuoteLinesHelper
    {
        public int contractDuration { get; set; }
        public int quoteId { get; set; }
        public int archiveOption { get; set; }
        public string package { get; set; }
        public int acquireEmail { get; set; }
        public int documents { get; set; }
    }

    public class CheckPackageReturnHelper
    {
        public int InvoiceQuantity { get; set; }
        public int ServiceId { get; set; }
        public decimal Price { get; set; }
        public decimal ArchivePrice { get; set; }
        public int DocumentServiceId { get; set; }
    }

    public class QuoteHelperMethods
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public void UpdateQuoteSum(int quoteId)
        {
            var quote = _db.Quotes.First(q => q.Id == quoteId);
            var lines = _db.QuoteLines.Where(ql => ql.RelatedQuoteId == quoteId);

            if (lines.Count() != 0)
            {
                quote.QuoteSum = lines.Sum(ql => ql.LineTotal);
                quote.QuoteSumTotal = lines.Sum(ql => ql.LineTotal) * (decimal)1.25;
            }
            else
            {
                quote.QuoteSum = 0;
                quote.QuoteSumTotal = 0;
            }


            quote.UpdateDate = DateTime.Now;
            _db.SaveChanges();
        }

        public int CheckQuoteLineNumber(int quoteId)
        {
            var lines = _db.QuoteLines.Count(q => q.RelatedQuoteId == quoteId);
            var linesCount = 1;
            if (lines != 0)
            {
                lines++;
                linesCount = lines;
            }

            return linesCount;
        }

        public bool CheckArchiveAndAqcuireEmailOption(int option)
        {
            if (option == 0)
                return false;
            return true;
        }

        public CheckPackageReturnHelper CheckPackage(string input, int documentsOption)
        {
            int invoiceQuantity = 0;
            int serviceId = 0;
            int documentsServiceId = 0;
            decimal price = 0;
            decimal archivePrice = 0;

            //Define whether contract has eInvoices or eDocuments included
            switch (documentsOption)
            {
                case 1:
                    documentsServiceId = 5;
                    break;
                case 2:
                    documentsServiceId = 28;
                    break;
                default:
                    break;
            }

            if (input.StartsWith("BASIC MICRO"))
            {
                invoiceQuantity = Int32.Parse(input.Substring(12));
                serviceId = 15;

                switch (invoiceQuantity)
                {
                    case 20:
                        price = 59;
                        archivePrice = 10;
                        break;
                    case 50:
                        price = 119;
                        archivePrice = 25;
                        break;
                    case 75:
                        price = 169;
                        archivePrice = 37;
                        break;
                    case 100:
                        price = 219;
                        archivePrice = 49;
                        break;
                    case 150:
                        price = 329;
                        archivePrice = 69;
                        break;
                    default:
                        break;
                }
            }
            else if (input.StartsWith("BASIC"))
            {
                invoiceQuantity = Int32.Parse(input.Substring(6));
                serviceId = 16;

                switch (invoiceQuantity)
                {
                    case 250:
                        price = 549;
                        archivePrice = 119;
                        break;
                    case 350:
                        price = 769;
                        archivePrice = 139;
                        break;
                    case 450:
                        price = 989;
                        archivePrice = 209;
                        break;
                    case 600:
                        price = 1319;
                        archivePrice = 279;
                        break;
                    default:
                        break;
                }
            }

            CheckPackageReturnHelper returnData = new CheckPackageReturnHelper()
            {
                InvoiceQuantity = invoiceQuantity,
                ServiceId = serviceId,
                Price = price,
                ArchivePrice = archivePrice,
                DocumentServiceId = documentsServiceId
            };
            return returnData;
        }
    }
}