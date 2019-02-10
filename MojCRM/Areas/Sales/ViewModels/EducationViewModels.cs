using MojCRM.Areas.Sales.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace MojCRM.Areas.Sales.ViewModels
{
    public class EducationDetailViewModel
    {
        public Education Education { get; set; }
        public IQueryable<Contact> RelatedSalesContacts { get; set; }
        public IQueryable<EducationNote> RelatedEducationNotes { get; set; }
        public IQueryable<ActivityLog> RelatedEducationActivities { get; set; }
        public virtual IQueryable<ApplicationUser> Users { get; set; }
        public IList<ListItem> RejectReasons { get; set; }

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
                var list = (from t in RelatedEducationNotes
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
    }

    public class EducationIndexViewModel
    {
        public IQueryable<Education> Educations { get; set; }
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
                    new SelectListItem{ Value= @"1", Text = @"U kontaktu"},
                    new SelectListItem{ Value= @"2", Text = @"Poslan poziv"},
                    new SelectListItem{ Value= @"3", Text = @"Odbijeno"},
                    new SelectListItem{ Value= @"4", Text = @"Prihvaćeno"}
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
                    new SelectListItem{ Value= @"3", Text = @"Ne želi navesti"},
                    new SelectListItem{ Value= @"0", Text = @"Neodgovarajući termin"},
                    new SelectListItem{ Value= @"1", Text = @"Nezainteresiranost"},
                    new SelectListItem{ Value= @"2", Text = @"Usluga nije potrebna"}
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