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
}