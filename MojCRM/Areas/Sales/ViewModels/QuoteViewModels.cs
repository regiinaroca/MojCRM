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
        public IList<SelectListItem> QuoteStatusList
        {
            get
            {
                var quoteStatusList = new List<SelectListItem>
                {
                    new SelectListItem{ Value = null, Text = @"-- Odaberi status ponude --"},
                    new SelectListItem{ Value = "0", Text = @"Kreirana ponuda" },
                    new SelectListItem{ Value = "1", Text = @"Poslana ponuda" },
                    new SelectListItem{ Value = "2", Text = @"Revidirana ponuda" },
                    new SelectListItem{ Value = "3", Text = @"Prihvaćena ponuda" },
                    new SelectListItem{ Value = "4", Text = @"Prihvaćena ponuda nakon revidiranja" },
                    new SelectListItem{ Value = "5", Text = @"Odbijena ponuda" },
                    new SelectListItem{ Value = "6", Text = @"Opozvana ponuda" }
                };
                return quoteStatusList;
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

        public IQueryable<SelectListItem> ContractDurationList
        {
            get
            {
                var list = new List<SelectListItem>
                {
                    new SelectListItem {Value = null, Text = @"-- Odaberi trajanje ugovorne obaveze --"},
                    new SelectListItem {Value = "12", Text = @"12 mjeseci / 1 godina"},
                    new SelectListItem {Value = "24", Text = @"24 mjeseca / 2 godine"},
                    new SelectListItem {Value = "36", Text = @"36 mjeseci / 3 godine"}
                };
                return list.AsQueryable();
            }
        }

        public IQueryable<SelectListItem> ArchiveAndAcquireEmailOptionList
        {
            get
            {
                var list = new List<SelectListItem>
                {
                    new SelectListItem {Value = null, Text = @"-- Odaberi opciju --"},
                    new SelectListItem {Value = "0", Text = @"Nije uključeno"},
                    new SelectListItem {Value = "1", Text = @"Uključeno"}
                };
                return list.AsQueryable();
            }
        }

        public IQueryable<SelectListItem> PackagesList
        {
            get
            {
                var list = new List<SelectListItem>
                {
                    new SelectListItem {Value = null, Text = @"-- Odaberi paket u ugovoru --"},
                    new SelectListItem {Value = @"BASIC MICRO 20", Text = @"BASIC MICRO 20"},
                    new SelectListItem {Value = @"BASIC MICRO 50", Text = @"BASIC MICRO 50"},
                    new SelectListItem {Value = @"BASIC MICRO 75", Text = @"BASIC MICRO 75"},
                    new SelectListItem {Value = @"BASIC MICRO 100", Text = @"BASIC MICRO 100"},
                    new SelectListItem {Value = @"BASIC MICRO 150", Text = @"BASIC MICRO 150"},
                    new SelectListItem {Value = @"BASIC 250", Text = @"BASIC 250"},
                    new SelectListItem {Value = @"BASIC 350", Text = @"BASIC 350"},
                    new SelectListItem {Value = @"BASIC 450", Text = @"BASIC 450"},
                    new SelectListItem {Value = @"BASIC 600", Text = @"BASIC 600"},
                };
                return list.AsQueryable();
            }
        }

        public IQueryable<SelectListItem> InvoiceOrDocumentsOptionList
        {
            get
            {
                var list = new List<SelectListItem>
                {
                    new SelectListItem {Value = null, Text = @"-- Odaberi opciju --"},
                    new SelectListItem {Value = "1", Text = @"Uključeni eRačuni"},
                    new SelectListItem {Value = "2", Text = @"Uključeni eDokumenti"}
                };
                return list.AsQueryable();
            }
        }
    }

    public class QuoteIndexViewModel
    {
        public IQueryable<Quote> Quotes { get; set; }
        public virtual IQueryable<ApplicationUser> Users { get; set; }

        public IList<SelectListItem> SalesAgents
        {
            get
            {
                var list = (from u in Users
                            where u.Email != String.Empty
                            select new SelectListItem()
                            {
                                Text = u.UserName,
                                Value = u.UserName
                            }).ToList();
                return list;
            }
        }

        public IList<SelectListItem> OpportunityStatusList
        {
            get
            {
                var opportunityStatusList = new List<SelectListItem>
                {
                    new SelectListItem{ Value= @"0", Text = @"Kreirano"},
                    new SelectListItem{ Value= @"6", Text = @"Postojeći Moj-eRačun korisnik"},
                    new SelectListItem{ Value= @"1", Text = @"U kontaktu"},
                    new SelectListItem{ Value= @"4", Text = @"Potrebno dogovoriti sastanak"},
                    new SelectListItem{ Value= @"5", Text = @"Procesne poteškoće"},
                    new SelectListItem{ Value= @"2", Text = @"Kreiran lead"},
                    new SelectListItem{ Value= @"7", Text = @"FINA korisnik"},
                    new SelectListItem{ Value= @"8", Text = @"eFaktura korisnik"},
                    new SelectListItem{ Value= @"3", Text = @"Odbijeno"},
                    new SelectListItem{ Value= @"9", Text = @"Zatvorena tvrtka"}
                };
                return opportunityStatusList;
            }
        }

        public IList<SelectListItem> OpportunityRejectReasonList
        {
            get
            {
                var opportunityRejectReasonList = new List<SelectListItem>
                {
                    new SelectListItem{ Value= @"0", Text = @"Ne želi navesti"},
                    new SelectListItem{ Value= @"1", Text = @"Nema interesa za uslugu"},
                    new SelectListItem{ Value= @"2", Text = @"Previsoka cijena"},
                    new SelectListItem{ Value= @"3", Text = @"Neadekvatna ponuda"},
                    new SelectListItem{ Value= @"4", Text = @"Koristi drugog posrednika"}
                };
                return opportunityRejectReasonList;
            }
        }

        public IList<SelectListItem> Priority
        {
            get
            {
                var priority = new List<SelectListItem>
                {
                    new SelectListItem{ Value = @"0", Text = @"Nizak"},
                    new SelectListItem{ Value = @"1", Text = @"Normalan"},
                    new SelectListItem{ Value = @"2", Text = @"Visok"}
                };
                return priority;
            }
        }

        public IList<SelectListItem> Assigned
        {
            get
            {
                var assigned = new List<SelectListItem>
                {
                    new SelectListItem{ Value = @"1", Text = @"Nedodijeljene"},
                    new SelectListItem{ Value = @"2", Text = @"Dodijeljene"}
                };
                return assigned;
            }
        }
    }
}