using MojCRM.Areas.Sales.Helpers;
using MojCRM.Areas.Sales.Models;
using MojCRM.Areas.Sales.ViewModels;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using MojCRM.Helpers;
using static MojCRM.Models.ActivityLog;

namespace MojCRM.Areas.Sales.Controllers
{
    public class LeadsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly HelperMethods _helper = new HelperMethods();

        // GET: Sales/Leads
        [Authorize]
        public ActionResult Index(LeadSearchHelper model)
        {
            var leads = _db.Leads.Select(l => l);
            if (User.IsInRole("Management") || User.IsInRole("Administrator") || User.IsInRole("Board") || User.IsInRole("Superadmin"))
            {
                //Search Engine -- Admin
                if (!String.IsNullOrEmpty(model.Campaign))
                {
                    leads = leads.Where(l => l.RelatedCampaign.CampaignName.Contains(model.Campaign));
                }
                if (!String.IsNullOrEmpty(model.Lead))
                {
                    leads = leads.Where(l => l.LeadTitle.Contains(model.Lead));
                }
                if (!String.IsNullOrEmpty(model.Organization))
                {
                    leads = leads.Where(l => l.RelatedOrganization.SubjectName.Contains(model.Organization) || l.RelatedOrganization.VAT.Contains(model.Organization));
                }
                if (!String.IsNullOrEmpty(model.LeadStatus.ToString()))
                {
                    leads = leads.Where(l => l.LeadStatus == model.LeadStatus);
                }
                if (!String.IsNullOrEmpty(model.RejectReason.ToString()))
                {
                    leads = leads.Where(l => l.RejectReason == model.RejectReason);
                }
                if (!String.IsNullOrEmpty(model.Assigned))
                {
                    if (model.Assigned == "1")
                    {
                        leads = leads.Where(l => l.IsAssigned == false);
                    }
                    if (model.Assigned == "2")
                    {
                        leads = leads.Where(l => l.IsAssigned);
                    }
                }
                if (!String.IsNullOrEmpty(model.AssignedTo))
                {
                    leads = leads.Where(l => l.AssignedTo == model.AssignedTo);
                }
            }
            else
            {
                leads = leads.Where(op => op.AssignedTo == User.Identity.Name);
                //Search Engine -- User
                if (!String.IsNullOrEmpty(model.Campaign))
                {
                    leads = leads.Where(l => l.RelatedCampaign.CampaignName.Contains(model.Campaign));
                }
                if (!String.IsNullOrEmpty(model.Lead))
                {
                    leads = leads.Where(l => l.LeadTitle.Contains(model.Lead));
                }
                if (!String.IsNullOrEmpty(model.Organization))
                {
                    leads = leads.Where(l => l.RelatedOrganization.SubjectName.Contains(model.Organization) || l.RelatedOrganization.VAT.Contains(model.Organization));
                }
                if (!String.IsNullOrEmpty(model.LeadStatus.ToString()))
                {
                    leads = leads.Where(l => l.LeadStatus == model.LeadStatus);
                }
                if (!String.IsNullOrEmpty(model.RejectReason.ToString()))
                {
                    leads = leads.Where(l => l.RejectReason == model.RejectReason);
                }
            }

            ViewBag.SearchResults = leads.Count();
            ViewBag.SearchResultsAssigned = leads.Count(l => l.IsAssigned);

            ViewBag.UsersAssigned = leads.Count(l => l.AssignedTo == User.Identity.Name);
            ViewBag.UsersCreated = leads.Count(l => l.AssignedTo == User.Identity.Name && l.LeadStatus == Lead.LeadStatusEnum.Start);
            ViewBag.UsersInContact = leads.Count(l => l.AssignedTo == User.Identity.Name && l.LeadStatus == Lead.LeadStatusEnum.Incontact);
            ViewBag.UsersRejected = leads.Count(l => l.AssignedTo == User.Identity.Name && l.LeadStatus == Lead.LeadStatusEnum.Rejected);
            ViewBag.QuoteSent = leads.Count(l => l.AssignedTo == User.Identity.Name && l.LeadStatus == Lead.LeadStatusEnum.Quotesent);
            ViewBag.QuoteAccepted = leads.Count(l => l.AssignedTo == User.Identity.Name && l.LeadStatus == Lead.LeadStatusEnum.Accepted);

            if (User.IsInRole("Management") || User.IsInRole("Administrator") || User.IsInRole("Board") || User.IsInRole("Superadmin"))
            {
                return View(leads.OrderByDescending(l => l.InsertDate));
            }
            return View(leads.Where(l => l.LeadStatus != Lead.LeadStatusEnum.Rejected || l.LeadStatus != Lead.LeadStatusEnum.Accepted).OrderByDescending(l => l.InsertDate));
        }

        // GET: Sales/Leads/Details/5
        public ActionResult Details(int id)
        {
            Lead lead = _db.Leads.Find(id);
            if (lead == null)
            {
                return HttpNotFound();
            }

            var relatedSalesContacts = _db.Contacts.Where(c =>
                c.Organization.MerId == lead.RelatedOrganizationId && c.ContactType == Contact.ContactTypeEnum.SALES);
            var relatedLeadNotes = _db.LeadNotes.Where(n => n.RelatedLeadId == lead.LeadId).OrderByDescending(n => n.InsertDate);
            var relatedLeadActivities = _db.ActivityLogs.Where(a =>
                a.ReferenceId == lead.LeadId && a.Module == ModuleEnum.Leads).OrderByDescending(a => a.InsertDate);
            var relatedOrganization = _db.Organizations.First(o => o.MerId == lead.RelatedOrganizationId);
            var relatedOrganizationDetail = _db.OrganizationDetails.First(od => od.MerId == lead.RelatedOrganizationId);
            var relatedCampaign = _db.Campaigns.First(c => c.CampaignId == lead.RelatedCampaignId);
            var users = _db.Users.Select(u => u);
            //var _LastLeadNote = (from n in db.LeadNotes
            //                     where n.RelatedLeadId == lead.LeadId
            //                     select n).OrderByDescending(n => n.InsertDate).Select(n => n.Note).First().ToString();

            var salesNoteTemplates = new List<ListItem>
                {
                    new ListItem{ Value = @"razloženo funkcioniranje servisa (opis onoga što se dogodi nakon što korisnik klikne pošalji eRačun)", Text = @"razloženo funkcioniranje servisa (opis onoga što se dogodi nakon što korisnik klikne pošalji eRačun)" },
                    new ListItem{ Value = @"argumentirana korisnička podrška -- ažuriranje mailova (90% uspješnost), slanje tipske obavijesti, zvanje za preuzimanje (97% uspješnost)", Text = @"argumentirana korisnička podrška -- ažuriranje mailova (90% uspješnost), slanje tipske obavijesti, zvanje za preuzimanje (97% uspješnost)" },
                    new ListItem{ Value = @"objašnjena tehnička pozadina s ERPom", Text = @"objašnjena tehnička pozadina s ERPom" },
                    new ListItem{ Value = @"objašnjena tehnička pozadina s eRa aplikacijom", Text = @"objašnjena tehnička pozadina s eRa aplikacijom" },
                    new ListItem{ Value = @"razložena potvrda primitka, pretraživanje i arhiviranje", Text = @"razložena potvrda primitka, pretraživanje i arhiviranje" },
                    new ListItem{ Value = @"istaknuta jednostavnost uvođenja (kod izgovora nemamo vremena, prostora, u restrukturiranju smo)", Text = @"istaknuta jednostavnost uvođenja (kod izgovora nemamo vremena, prostora, u restrukturiranju smo)" },
                    new ListItem{ Value = @"osvježen kontakt i iznesene novosti", Text = @"osvježen kontakt i iznesene novosti" },
                    new ListItem{ Value = @"izvršen kvalitetan presing", Text = @"izvršen kvalitetan presing" },
                    new ListItem{ Value = @"izvršen salesforce (isticanje benefita uz forzu)", Text = @"izvršen salesforce (isticanje benefita uz forzu)" },
                    new ListItem{ Value = @"poslan mail ps (prijedlog suradnje)", Text = @"poslan mail ps (prijedlog suradnje)" },
                    new ListItem{ Value = @"kreirati i odaslati PND", Text = @"kreirati i odaslati PND" },
                    new ListItem{ Value = @"kreirati i odaslati UO", Text = @"kreirati i odaslati UO" },
                    new ListItem{ Value = @"održan sastanak, poslan FU", Text = @"održan sastanak, poslan FU" },
                    new ListItem{ Value = @"objašnjena zakonska pozadina i pravovaljanost eRačuna", Text = @"objašnjena zakonska pozadina i pravovaljanost eRačuna" },
                    new ListItem{ Value = @"kontaktirani za uvođenje zaprimanja", Text = @"kontaktirani za uvođenje zaprimanja" },
                    new ListItem{ Value = @"obrazložio slanje privitaka", Text = @"obrazložio slanje privitaka" },
                    new ListItem{ Value = @"obrazložio procesnu pokrivenost primatelja te odagnao brige i strahove u vezi preuzimanja od strane njihovih kupaca", Text = @"obrazložio procesnu pokrivenost primatelja te odagnao brige i strahove u vezi preuzimanja od strane njihovih kupaca" }
                };

            var rejectReasonList = new List<ListItem>
            {
                new ListItem{ Value= @"0", Text = @"Ne želi navesti"},
                new ListItem{ Value= @"1", Text = @"Nema interesa za uslugu"},
                new ListItem{ Value= @"2", Text = @"Previsoka cijena"},
                new ListItem{ Value= @"3", Text = @"Neadekvatna ponuda"},
                new ListItem{ Value= @"4", Text = @"Koristi drugog posrednika"},
                new ListItem{ Value= @"5", Text = @"Nedostatak vremena za pokretanje projekta"},
                new ListItem{ Value= @"6", Text = @"Dio strane grupacije / Strano vlasništvo"},
                new ListItem{ Value= @"7", Text = @"Drugo / Ostalo"},
            };

            var leadDetails = new LeadDetailViewModel()
            {
                LeadId = id,
                LeadDescription = lead.LeadDescription,
                LeadStatus = lead.LeadStatusString,
                RejectReason = lead.LeadRejectReasonString,
                OrganizationId = lead.RelatedOrganizationId,
                OrganizationName = relatedOrganization.SubjectName,
                OrganizationVAT = relatedOrganization.VAT,
                TelephoneNumber = relatedOrganizationDetail.TelephoneNumber,
                MobilePhoneNumber = relatedOrganizationDetail.MobilePhoneNumber,
                Email = relatedOrganizationDetail.EmailAddress,
                ERP = relatedOrganizationDetail.ERP,
                NumberOfInvoicesSent = relatedOrganizationDetail.NumberOfInvoicesSent,
                NumberOfInvoicesReceived = relatedOrganizationDetail.NumberOfInvoicesReceived,
                RelatedCampaignId = lead.RelatedCampaignId,
                RelatedCampaignName = relatedCampaign.CampaignName,
                IsAssigned = lead.IsAssigned,
                AssignedTo = lead.AssignedTo,
                LastContactedDate = lead.LastContactDate,
                LastContactedBy = lead.LastContactedBy,
                RelatedSalesContacts = relatedSalesContacts,
                RelatedLeadNotes = relatedLeadNotes,
                RelatedLeadActivities = relatedLeadActivities,
                Users = users,
                SalesNoteTemplates = salesNoteTemplates,
                RejectReasons = rejectReasonList
            };

            return View(leadDetails);
        }

        // POST: Sales/Leads/AddNote
        [HttpPost]
        public ActionResult AddNote(LeadNoteHelper model)
        {
            var lead = _db.Leads.First(l => l.LeadId == model.RelatedLeadId);
            var noteString = new StringBuilder();

            lead.LastContactDate = DateTime.Now;
            lead.LastContactedBy = User.Identity.Name;

            if (model.NoteTemplates == null)
            {
                _db.LeadNotes.Add(new LeadNote
                {
                    RelatedLeadId = model.RelatedLeadId,
                    User = User.Identity.Name,
                    Note = model.Note,
                    InsertDate = DateTime.Now,
                    Contact = model.Contact
                });
                _db.SaveChanges();
            }
            else
            {
                foreach (var template in model.NoteTemplates)
                {
                    noteString.AppendLine(template);
                }

                _db.LeadNotes.Add(new LeadNote
                {
                    RelatedLeadId = model.RelatedLeadId,
                    User = User.Identity.Name,
                    Note = noteString + ";" + model.Note,
                    InsertDate = DateTime.Now,
                    Contact = model.Contact
                });
                _db.SaveChanges();
            }

            if (model.IsActivity == false)
            {
                return RedirectToAction("Details", new { id = model.RelatedLeadId });
            }
            switch (model.Identifier)
            {
                case 1:
                    _helper.LogActivity(User.Identity.Name + @" je obavio uspješan poziv vezan uz lead: " + lead.LeadTitle, User.Identity.Name, model.RelatedLeadId, ActivityTypeEnum.Succall, DepartmentEnum.Sales, ModuleEnum.Leads);
                    break;
                case 2:
                    _helper.LogActivity(User.Identity.Name + " je obavio kraći informativni poziv vezan uz lead: " + lead.LeadTitle, User.Identity.Name, model.RelatedLeadId, ActivityTypeEnum.Succalshort, DepartmentEnum.Sales, ModuleEnum.Leads);
                    break;
                case 3:
                    _helper.LogActivity(User.Identity.Name + " je pokušao obaviti telefonski poziv vezanvezan uz lead: " + lead.LeadTitle, User.Identity.Name, model.RelatedLeadId, ActivityTypeEnum.Unsuccal, DepartmentEnum.Sales, ModuleEnum.Leads);
                    break;
            }
            return RedirectToAction("Details", new { id = model.RelatedLeadId });
        }

        // POST: Sales/Leads/EditNote
        [HttpPost]
        [Authorize]
        public ActionResult EditNote(LeadNoteHelper model)
        {
            var noteForEdit = _db.LeadNotes.First(n => n.Id == model.NoteId);

            noteForEdit.Note = model.Note;
            noteForEdit.Contact = model.Contact;
            noteForEdit.UpdateDate = DateTime.Now;
            _db.SaveChanges();

            return RedirectToAction("Details", new { id = model.RelatedLeadId });
        }

        // POST: Sales/Opportunities/DeleteNote
        [HttpPost, ActionName("DeleteNote")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Superadmin")]
        public ActionResult DeleteNote(LeadNoteHelper model)
        {
            LeadNote leadNote = _db.LeadNotes.Find(model.NoteId);
            _db.LeadNotes.Remove(leadNote ?? throw new InvalidOperationException());
            _db.SaveChanges();
            return RedirectToAction("Details", new { id = model.RelatedLeadId });
        }

        public void LogEmail(LeadNoteHelper model)
        {
            var lead = _db.Leads.First(l => l.LeadId == model.RelatedLeadId);

            lead.LastContactDate = DateTime.Now;
            lead.LastContactedBy = model.User;
            _helper.LogActivity(User.Identity.Name + " je poslao e-mail na adresu: " + model.Email + " na temu prezentacije usluge u sklopu prodajne prilike: " + lead.LeadTitle, User.Identity.Name, model.RelatedLeadId, ActivityTypeEnum.Email, DepartmentEnum.Sales, ModuleEnum.Leads);
        }

        public ActionResult Assign(LeadAssignHelper model)
        {
            var lead = _db.Leads.First(o => o.LeadId == model.RelatedLeadId);
            lead.IsAssigned = true;
            lead.AssignedTo = model.AssignedTo;
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        public ActionResult Reassign(LeadAssignHelper model)
        {
            var lead = _db.Leads.First(o => o.LeadId == model.RelatedLeadId);
            if (model.Unassign)
            {
                lead.IsAssigned = false;
                lead.AssignedTo = String.Empty;
                _db.SaveChanges();
            }
            else
            {
                lead.AssignedTo = model.AssignedTo;
                _db.SaveChanges();
            }

            return Redirect(Request.UrlReferrer?.ToString());
        }

        // GET: Sales/Opportunities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lead lead = _db.Leads.Find(id);
            if (lead == null)
            {
                return HttpNotFound();
            }
            return View(lead);
        }

        // POST: Sales/Opportunities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lead lead)
        {
            var model = new LeadEditHelper()
            {
                LeadId = lead.LeadId,
                LeadDescription = lead.LeadDescription,
                LeadStatus = lead.LeadStatus,
                RejectReason = lead.RejectReason
            };
            if (ModelState.IsValid)
            {
                lead.LeadDescription = model.LeadDescription;
                lead.LeadStatus = model.LeadStatus;
                lead.RejectReason = model.RejectReason;
                lead.UpdateDate = DateTime.Now;
                lead.LastUpdatedBy = User.Identity.Name;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult ChangeStatus(LeadChangeStatusHelper model)
        {
            var lead = _db.Leads.First(l => l.LeadId == model.RelatedLeadId);
            lead.LeadStatus = model.NewStatus;
            lead.StatusDescription = model.StatusDescription;
            lead.UpdateDate = DateTime.Now;
            lead.LastUpdatedBy = User.Identity.Name;
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        public ActionResult MarkRejected(LeadMarkRejectedHelper model)
        {
            var lead = _db.Leads.First(l => l.LeadId == model.RelatedLeadId);
            lead.LeadStatus = Lead.LeadStatusEnum.Rejected;
            lead.RejectReason = model.RejectReason;
            lead.UpdateDate = DateTime.Now;
            lead.LastUpdatedBy = User.Identity.Name;
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }
    }
}