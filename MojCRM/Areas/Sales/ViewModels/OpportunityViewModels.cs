using MojCRM.Areas.Sales.Models;
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

    public class OpportunityIndexViewModel
    {
        public IQueryable<Opportunity> Opportunities { get; set; }
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