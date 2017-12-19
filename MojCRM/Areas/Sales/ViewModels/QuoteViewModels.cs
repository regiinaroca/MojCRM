using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MojCRM.Areas.Sales.Models;
using MojCRM.Models;

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
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public Quote Quote { get; set; }
        public IQueryable<QuoteLine> QuoteLines { get; set; }
        public IQueryable<QuoteMember> QuoteMembers { get; set; }

        public IQueryable<SelectListItem> AgentList
        {
            get
            {
                var agents = _db.Users.Where(a => a.Email != String.Empty);

                var agentsList = new List<SelectListItem>();

                foreach (var agent in agents)
                {
                    agentsList.Add(new SelectListItem { Value = agent.UserName, Text = agent.UserName });
                }

                return agentsList.AsQueryable();
            }
        }

        public IQueryable<SelectListItem> ServicesList
        {
            get
            {
                var services = _db.Services.Where(s => s.IsActive);

                var serviceList = new List<SelectListItem>();

                foreach (var service in services)
                {
                    serviceList.Add(new SelectListItem { Value = service.ServiceId.ToString(), Text = service.ServiceName });
                }

                return serviceList.AsQueryable();
            }
        }
        public IQueryable<SelectListItem> MemberRoleList
        {
            get
            {
                var roleList = new List<SelectListItem>
                {
                    new SelectListItem {Value = null, Text = @"-- Odaberi ulogu djelatnika u ponudi --"},
                    new SelectListItem {Value = "1", Text = @"Član grupe"},
                    new SelectListItem {Value = "2", Text = @"Odobravatelj"}
                };
                return roleList.AsQueryable();
            }
        }
    }
}