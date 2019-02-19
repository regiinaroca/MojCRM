using MojCRM.Areas.Sales.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace MojCRM.Areas.Sales.ViewModels
{
    public class LeadDetailViewModel
    {
        public Lead Lead { get; set; }

        [Display(Name = "Naziv kampanje")]
        public string RelatedCampaignName { get; set; }

        [Display(Name = "Zadnja bilješka")]
        public string LastLeadNote { get; set; }

        public IQueryable<Contact> RelatedSalesContacts { get; set; }
        public IQueryable<LeadNote> RelatedLeadNotes { get; set; }
        public IQueryable<ActivityLog> RelatedLeadActivities { get; set; }
        public IQueryable<Quote> RelatedQuotes { get; set; }

        public virtual IQueryable<ApplicationUser> Users { get; set; }

        public IList<SelectListItem> RelatedSalesContactsForDetails
        {
            get
            {
                var list = (from t in RelatedSalesContacts
                            select new SelectListItem()
                            {
                                Text = t.ContactFirstName + @" " + t.ContactLastName,
                                Value = t.ContactFirstName + @" " + t.ContactLastName
                            }).ToList();
                return list;
            }
        }

        public IList<SelectListItem> RelatedSalesContactsId
        {
            get
            {
                var list = (from t in RelatedSalesContacts
                            select new SelectListItem()
                            {
                                Text = t.ContactFirstName + @" " + t.ContactLastName,
                                Value = t.ContactId.ToString()
                            }).ToList();
                return list;
            }
        }

        public IList<SelectListItem> NoteIds
        {
            get
            {
                var list = (from t in RelatedLeadNotes
                            select new SelectListItem()
                            {
                                Text = t.Note,
                                Value = t.Id.ToString()
                            }).ToList();
                return list;
            }
        }

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

        public IList<ListItem> SalesNoteTemplates { get; set; }
        public IList<ListItem> RejectReasons { get; set; }
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

    public class LeadIndexViewModel
    {
        public IQueryable<Lead> Leads { get; set; }
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

        public IList<SelectListItem> LeadStatusList
        {
            get
            {
                var leadStatusList = new List<SelectListItem>
                {
                    new SelectListItem{ Value= "0", Text = "Kreirano"},
                    new SelectListItem{ Value= "1", Text = "U kontaktu"},
                    new SelectListItem{ Value= "6", Text = "Procesne poteškoće"},
                    new SelectListItem{ Value= "2", Text = "Odbijeno"},
                    new SelectListItem{ Value= "3", Text = "Poslana ponuda"},
                    new SelectListItem{ Value= "4", Text = "Prihvaćena ponuda"}
                };
                return leadStatusList;
            }
        }

        public IList<SelectListItem> LeadRejectReasonList
        {
            get
            {
                var leadRejectReasonList = new List<SelectListItem>
                {
                    new SelectListItem{ Value= @"0", Text = @"Ne želi navesti"},
                    new SelectListItem{ Value= @"1", Text = @"Nema interesa za uslugu"},
                    new SelectListItem{ Value= @"2", Text = @"Previsoka cijena"},
                    new SelectListItem{ Value= @"3", Text = @"Neadekvatna ponuda"},
                    new SelectListItem{ Value= @"4", Text = @"Koristi drugog posrednika"}
                };
                return leadRejectReasonList;
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