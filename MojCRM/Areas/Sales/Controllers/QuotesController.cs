using System;
using System.Linq;
using System.Web.Mvc;
using MojCRM.Areas.Sales.Helpers;
using MojCRM.Areas.Sales.Models;
using MojCRM.Areas.Sales.ViewModels;
using MojCRM.Models;
using MojCRM.Helpers;

namespace MojCRM.Areas.Sales.Controllers
{
    public class QuotesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly HelperMethods _helper = new HelperMethods();
        private readonly QuoteHelperMethods _quoteHelper = new QuoteHelperMethods();
        // GET: Sales/Quotes
        public ActionResult Index()
        {
            return View();
        }

        // POST: Sales/Quotes/Create
        [HttpPost]
        public ActionResult Create(CreateQuoteHelper model)
        {
            var organization = _db.Organizations.First(o => o.MerId == model.OrganizationId);
            var quotes = _db.Quotes.Count(q => q.RelatedOrganizationId == model.OrganizationId && q.InsertDate.Year == DateTime.Now.Year);

            if (quotes != 1)
                quotes++;

            var quoteCount = quotes;

            var quoteNumber = organization.MerId + @"-" + DateTime.Now.Year + @"-" + DateTime.Now.Month + @"/" + quoteCount;
            var returnModel = _db.Quotes.Add(new Quote
            {
                QuoteNumber = quoteNumber,
                RelatedOrganizationId = model.OrganizationId,
                RelatedCampaignId = model.CampaignId,
                RelatedLeadId = model.LeadId,
                AssignedTo = User.Identity.Name,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                QuoteType = model.QuoteType,
                QuoteStatus = Quote.QuoteStatusEnum.Created,
                QuoteSum = 0,
                QuoteSumTotal = 0,
                InsertDate = DateTime.Now,
                CreatedBy = User.Identity.Name
            });
            _db.SaveChanges();
            return View(returnModel);
        }

        // GET: Sales/Quotes/Details/5
        public ActionResult Details(int id)
        {
            var model = new QuoteDetailsViewModel
            {
                Quote = _db.Quotes.First(q => q.Id == id),
                QuoteMembers = _db.QuoteMembers.Where(qm => qm.RelatedQuoteId == id),
                QuoteLines = _db.QuoteLines.Where(ql => ql.RelatedQuoteId == id)
            };

            return View(model);
        }

        // GET: Sales/Quotes/CreateFromLead
        [Obsolete]
        public ActionResult CreateFromLead(int relatedCampaignId, int organizationId, int leadId)
        {
            var returnModel = new CreateFromLeadViewModel
            {
                OrganizationId = organizationId,
                CampaignId = relatedCampaignId,
                LeadId = leadId,
                AssignedTo = User.Identity.Name,
                StartDate = null,
                EndDate = null,
                QuoteType = null
            };
            return View(returnModel);
        }

        [HttpPost]
        public ActionResult AddMember(string agent, QuoteMember.QuoteMemberRoleEnum role, int quote)
        {
            _db.QuoteMembers.Add(new QuoteMember
            {
                RelatedQuoteId = quote,
                MemberName = agent,
                QuoteMemberRole = role,
                InsertDate = DateTime.Now
            });
            _db.SaveChanges();
            return Redirect(Request.UrlReferrer?.ToString());
        }

        [HttpPost]
        public ActionResult AddQuoteLine(AddQuoteLineHelper model)
        {
            var lines = _db.QuoteLines.Count(q => q.RelatedQuoteId == model.RelatedQuoteId);
            var linesCount = 1;
            if (lines != 0)
            {
                lines++;
                linesCount = lines;
            }

            var service = _db.Services.First(s => s.ServiceId == model.RelatedServiceId);

            _db.QuoteLines.Add(new QuoteLine
            {
                RelatedQuoteId = model.RelatedQuoteId,
                LineNumber = linesCount,
                RelatedServiceId = model.RelatedServiceId,
                LineText = service.ServiceName,
                Quantity = model.Quantity,
                BaseAmount = service.BasePrice,
                Price = model.Price,
                LineTotal = model.Quantity * model.Price,
                InsertDate = DateTime.Now
            });
            _db.SaveChanges();

            _quoteHelper.UpdateQuoteSum(model.RelatedQuoteId);

            return Redirect(Request.UrlReferrer?.ToString());
        }

        public ActionResult RemoveQuoteLine(int quoteId, int lineItem)
        {
            var quoteLine = _db.QuoteLines.First(ql => ql.RelatedQuoteId == quoteId && ql.LineNumber == lineItem);

            _db.QuoteLines.Remove(quoteLine);
            _db.SaveChanges();

            var newLines = _db.QuoteLines.Where(ql => ql.RelatedQuoteId == quoteId);
            var newLinesInt = 1;

            foreach (var line in newLines)
            {
                line.LineNumber = newLinesInt;
                line.UpdateDate = DateTime.Now;
                newLinesInt++;
            }

            _db.SaveChanges();

            _quoteHelper.UpdateQuoteSum(quoteId);

            return Redirect(Request.UrlReferrer?.ToString());
        }

        public ActionResult ChangeStatus(int newStatus, int quoteId)
        {
            var quote = _db.Quotes.First(q => q.Id == quoteId);

            switch (newStatus)
            {
                case 0:
                    quote.QuoteStatus = Quote.QuoteStatusEnum.Created;
                    quote.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 1:
                    quote.QuoteStatus = Quote.QuoteStatusEnum.Sent;
                    quote.Lead.LeadStatus = Lead.LeadStatusEnum.Quotesent;
                    quote.Lead.UpdateDate = DateTime.Now;
                    quote.Lead.LastUpdatedBy = User.Identity.Name;
                    quote.UpdateDate = DateTime.Now;
                    _helper.LogActivity("Djelatnik " + User.Identity.Name + " je poslao ponudu vezanu za lead " + quote.Lead.LeadTitle + ".",
                        User.Identity.Name, quoteId, ActivityLog.ActivityTypeEnum.QuoteSent, ActivityLog.DepartmentEnum.Sales, ActivityLog.ModuleEnum.Quotes);
                    _db.SaveChanges();
                    break;
                case 2:
                    quote.QuoteStatus = Quote.QuoteStatusEnum.Reviewed;
                    quote.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 3:
                    quote.QuoteStatus = Quote.QuoteStatusEnum.Accepted;
                    quote.Lead.LeadStatus = Lead.LeadStatusEnum.Accepted;
                    quote.Lead.UpdateDate = DateTime.Now;
                    quote.Lead.LastUpdatedBy = User.Identity.Name;
                    quote.UpdateDate = DateTime.Now;
                    _helper.LogActivity("Djelatnik " + User.Identity.Name + " je ostvario prodaju vezanu za ponudu " + quote.QuoteNumber + " koja je dostavljena tvrtki " + quote.Organization.SubjectName + ".",
                        User.Identity.Name, quoteId, ActivityLog.ActivityTypeEnum.AchievedSales, ActivityLog.DepartmentEnum.Sales, ActivityLog.ModuleEnum.Quotes);
                    _db.SaveChanges();
                    break;
                case 4:
                    quote.QuoteStatus = Quote.QuoteStatusEnum.AcceptedAfterReview;
                    quote.Lead.LeadStatus = Lead.LeadStatusEnum.Accepted;
                    quote.Lead.UpdateDate = DateTime.Now;
                    quote.Lead.LastUpdatedBy = User.Identity.Name;
                    quote.UpdateDate = DateTime.Now;
                    _helper.LogActivity("Djelatnik " + User.Identity.Name + " je ostvario prodaju vezanu za ponudu " + quote.QuoteNumber + " koja je dostavljena tvrtki " + quote.Organization.SubjectName + ".",
                        User.Identity.Name, quoteId, ActivityLog.ActivityTypeEnum.AchievedSales, ActivityLog.DepartmentEnum.Sales, ActivityLog.ModuleEnum.Quotes);
                    _db.SaveChanges();
                    break;
                case 5:
                    quote.QuoteStatus = Quote.QuoteStatusEnum.Rejected;
                    quote.Lead.LeadStatus = Lead.LeadStatusEnum.Rejected;
                    quote.Lead.UpdateDate = DateTime.Now;
                    quote.Lead.LastUpdatedBy = User.Identity.Name;
                    quote.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 6:
                    quote.QuoteStatus = Quote.QuoteStatusEnum.Recalled;
                    quote.RecallDate = DateTime.Now;
                    quote.RecalledBy = User.Identity.Name;
                    quote.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
            }

            return Redirect(Request.UrlReferrer?.ToString());
        }
    }
}