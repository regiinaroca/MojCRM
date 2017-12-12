using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MojCRM.Areas.Sales.Helpers;
using MojCRM.Areas.Sales.Models;
using MojCRM.Areas.Sales.ViewModels;
using MojCRM.Models;

namespace MojCRM.Areas.Sales.Controllers
{
    public class QuotesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Sales/Quotes
        public ActionResult Index()
        {
            return View();
        }

        // POST: Sales/Create
        [HttpPost]
        public ActionResult Create(CreateQuoteHelper model)
        {
            var organization = _db.Organizations.Find(model.OrganizationId);
            var quoteNumber = organization.MerId + @"-" + DateTime.Now.Year + @"-" + DateTime.Now.Month;
            var returnModel = _db.Quotes.Add(new Quote
            {
                QuoteNumber = quoteNumber,
                RelatedOrganizationId = model.OrganizationId,
                RelatedCampaignId = model.CampaignId,
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

        // GET: Sales/CreateFromLead
        public ActionResult CreateFromLead(int relatedCampaignId, int organizationId)
        {
            var returnModel = new CreateFromLeadViewModel
            {
                OrganizationId = organizationId,
                CampaignId = relatedCampaignId,
                AssignedTo = User.Identity.Name,
                StartDate = null,
                EndDate = null,
                QuoteType = null
            };
            return View(returnModel);
        }
    }
}