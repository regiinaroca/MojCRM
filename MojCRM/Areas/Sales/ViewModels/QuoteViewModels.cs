using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MojCRM.Areas.Sales.Models;

namespace MojCRM.Areas.Sales.ViewModels
{
    public class CreateFromLeadViewModel
    {
        public int OrganizationId { get; set; }
        public int? CampaignId { get; set; }
        public int? LeadId { get; set; }
        public string AssignedTo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Quote.QuoteTypeEnum? QuoteType { get; set; }

        public IList<SelectListItem> QuoteTypeList
        {
            get
            {
                var quoteTypeList = new List<SelectListItem>
                {
                    new SelectListItem{ Value = null, Text = @"-- Odaberi tip ponude --"},
                    new SelectListItem{ Value = "0", Text = @"Ugovor - slobodno slanje" },
                    new SelectListItem{ Value = "1", Text = @"Ugovor - paketi" },
                    new SelectListItem{ Value = "2", Text = @"Avansna uplata" },
                };
                return quoteTypeList;
            }
        }
    }

    public class QuoteDetailsViewModel
    {
        public Quote Quote { get; set; }
        public IQueryable<QuoteLine> QuoteLines { get; set; }
        public IQueryable<QuoteMember> QuoteMembers { get; set; }
    }
}