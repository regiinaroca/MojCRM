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
}