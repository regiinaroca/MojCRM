﻿using MojCRM.Areas.Sales.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojCRM.Areas.Sales.ViewModels
{
    public class OpportunityDetailViewModel
    {
        public int OpportunityId { get; set; }

        [Display(Name = "Detalji prodajne prilike")]
        public string OpportunityDescription { get; set; }

        [Display(Name = "Status prodajne prilike")]
        public string OpportunityStatus { get; set; }

        [Display(Name = "Razlog odbijanja prodajne prilike")]
        public string RejectReasson { get; set; }

        [Display(Name = "Naziv tvrtke")]
        public string OrganizationName { get; set; }

        [Display(Name = "OIB tvrtke")]
        public string OrganizationVAT { get; set; }

        [Display(Name = "Broj telefona")]
        public string TelephoneNumber { get; set; }

        [Display(Name = "Broj mobitela")]
        public string MobilePhoneNumber { get; set; }

        [Display(Name = "E-mail adresa")]
        public string Email { get; set; }

        [Display(Name = "ERP program")]
        public string ERP { get; set; }

        [Display(Name = "Broj računa mjesečno")]
        public int? NumberOfInvoices { get; set; }

        [Display(Name = "Naziv kampanje")]
        public string RelatedCampaignName { get; set; }

        [Display(Name = "Dodijeljeno")]
        public bool IsAssigned { get; set; }

        [Display(Name = "Dodijeljeno agentu")]
        public string AssignedTo { get; set; }

        [Display(Name = "Datum i vrijeme zadnjeg kontakta")]
        public DateTime? LastContactedDate { get; set; }

        [Display(Name = "Zadnje kontaktirao")]
        public string LastContactedBy { get; set; }

        public IEnumerable<Contact> RelatedSalesContacts { get; set; }
        public IEnumerable<OpportunityNote> RelatedOpportunityNotes { get; set; }
        public IEnumerable<ActivityLog> RelatedOpportunityActivities { get; set; }

        //public virtual Contact SalesContact { get; set; }
        public virtual IEnumerable<ApplicationUser> Users { get; set; }

        public IList<SelectListItem> RelatedSalesContactsForDetails
        {
            get
            {
                var list = (from t in RelatedSalesContacts
                            select new SelectListItem()
                            {
                                Text = t.ContactFirstName + " " + t.ContactLastName,
                                Value = t.ContactFirstName + " " + t.ContactLastName
                            }).ToList();
                return list;
            }
            set { }
        }

        public IList<SelectListItem> NoteIds
        {
            get
            {
                var list = (from t in RelatedOpportunityNotes
                            select new SelectListItem()
                            {
                                Text = t.Note,
                                Value = t.Id.ToString()
                            }).ToList();
                return list;
            }
            set { }
        }

        public IList<SelectListItem> SalesAgents
        {
            get
            {
                var list = (from u in Users
                            select new SelectListItem()
                            {
                                Text = u.UserName,
                                Value = u.UserName
                            }).ToList();
                return list;
            }
            set { }
        }
    }
}