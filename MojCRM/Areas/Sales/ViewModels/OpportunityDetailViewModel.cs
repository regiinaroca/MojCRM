﻿using MojCRM.Areas.Sales.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static MojCRM.Areas.Sales.Models.Opportunity;

namespace MojCRM.Areas.Sales.ViewModels
{
    public class OpportunityDetailViewModel
    {
        public int OpportunityId { get; set; }

        [Display(Name = "Detalji prodajne prilike")]
        public string OpportunityDescription { get; set; }

        public OpportunityStatusEnum OpportunityStatus { get; set; }

        [Display(Name = "Status prodajne prilike")]
        public string OpportunityStatusString { get; set; }

        [Display(Name = "Razlog odbijanja prodajne prilike")]
        public string RejectReasson { get; set; }

        [Display(Name = "Id tvrtke")]
        public int? OrganizationId { get; set; }

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

        [Display(Name = "Broj IRA mjesečno")]
        public string NumberOfInvoicesSent { get; set; }

        [Display(Name = "Broj URA mjesečno")]
        public string NumberOfInvoicesReceived { get; set; }

        [Display(Name = "Id kampanje")]
        public int? RelatedCampaignId { get; set; }

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

        [Display(Name = "Zadnja bilješka")]
        public string LastOpportunityNote { get; set; }

        [Display(Name = "ID referentnog leada")]
        public int? RelatedLeadId { get; set; }

        public IQueryable<Contact> RelatedSalesContacts { get; set; }
        public IQueryable<OpportunityNote> RelatedOpportunityNotes { get; set; }
        public IQueryable<ActivityLog> RelatedOpportunityActivities { get; set; }

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
                var list = (from t in RelatedOpportunityNotes
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
    }
}