using MojCRM.Areas.Sales.Helpers;
using MojCRM.Areas.Sales.Models;
using MojCRM.Areas.Sales.ViewModels;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static MojCRM.Models.ActivityLog;

namespace MojCRM.Areas.Sales.Controllers
{
    public class LeadsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Sales/Leads
        [Authorize]
        public ActionResult Index(LeadSearchHelper Model)
        {
            var leads = from l in db.Leads
                                select l;
            if (User.IsInRole("Management") || User.IsInRole("Administrator") || User.IsInRole("Board") || User.IsInRole("Superadmin"))
            {
                //Search Engine -- Admin
                if (!String.IsNullOrEmpty(Model.Campaign))
                {
                    leads = leads.Where(l => l.RelatedCampaign.CampaignName.Contains(Model.Campaign));
                    ViewBag.SearchResults = leads.Count();
                    ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Lead))
                {
                    leads = leads.Where(l => l.LeadTitle.Contains(Model.Organization));
                    ViewBag.SearchResults = leads.Count();
                    ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Organization))
                {
                    leads = leads.Where(l => l.RelatedOrganization.SubjectName.Contains(Model.Organization) || l.RelatedOrganization.VAT.Contains(Model.Organization));
                    ViewBag.SearchResults = leads.Count();
                    ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.LeadStatus.ToString()))
                {
                    leads = leads.Where(l => l.LeadStatus == Model.LeadStatus);
                    ViewBag.SearchResults = leads.Count();
                    ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.RejectReason.ToString()))
                {
                    leads = leads.Where(l => l.RejectReason == Model.RejectReason);
                    ViewBag.SearchResults = leads.Count();
                    ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Assigned))
                {
                    if (Model.Assigned == "1")
                    {
                        leads = leads.Where(l => l.IsAssigned == false);
                        ViewBag.SearchResults = leads.Count();
                        ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                    }
                    if (Model.Assigned == "2")
                    {
                        leads = leads.Where(l => l.IsAssigned == true);
                        ViewBag.SearchResults = leads.Count();
                        ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                    }
                }
                if (!String.IsNullOrEmpty(Model.AssignedTo))
                {
                    leads = leads.Where(l => l.AssignedTo == Model.AssignedTo);
                    ViewBag.SearchResults = leads.Count();
                    ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                }
            }
            else
            {
                leads = leads.Where(op => op.AssignedTo == User.Identity.Name);
                //Search Engine -- User
                if (!String.IsNullOrEmpty(Model.Campaign))
                {
                    leads = leads.Where(l => l.RelatedCampaign.CampaignName.Contains(Model.Campaign));
                    ViewBag.SearchResults = leads.Count();
                    ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Lead))
                {
                    leads = leads.Where(l => l.LeadTitle.Contains(Model.Organization) || l.RelatedOrganization.VAT.Contains(Model.Organization));
                    ViewBag.SearchResults = leads.Count();
                    ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Organization))
                {
                    leads = leads.Where(l => l.RelatedOrganization.SubjectName.Contains(Model.Organization));
                    ViewBag.SearchResults = leads.Count();
                    ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.LeadStatus.ToString()))
                {
                    leads = leads.Where(l => l.LeadStatus == Model.LeadStatus);
                    ViewBag.SearchResults = leads.Count();
                    ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.RejectReason.ToString()))
                {
                    leads = leads.Where(l => l.RejectReason == Model.RejectReason);
                    ViewBag.SearchResults = leads.Count();
                    ViewBag.SearchResultsAssigned = leads.Where(l => l.IsAssigned == true).Count();
                }
            }

            ViewBag.SearchResults = leads.Count();
            ViewBag.SearchResultsAssigned = leads.Where(op => op.IsAssigned == true).Count();

            ViewBag.UsersAssigned = leads.Where(l => l.AssignedTo == User.Identity.Name).Count();
            ViewBag.UsersCreated = leads.Where(l => l.AssignedTo == User.Identity.Name && l.LeadStatus == Lead.LeadStatusEnum.START).Count();
            ViewBag.UsersInContact = leads.Where(l => l.AssignedTo == User.Identity.Name && l.LeadStatus == Lead.LeadStatusEnum.INCONTACT).Count();
            ViewBag.UsersRejected = leads.Where(l => l.AssignedTo == User.Identity.Name && l.LeadStatus == Lead.LeadStatusEnum.REJECTED).Count();
            ViewBag.QuoteSent = leads.Where(l => l.AssignedTo == User.Identity.Name && l.LeadStatus == Lead.LeadStatusEnum.QOUTESENT).Count();
            ViewBag.QuoteAccepted = leads.Where(l => l.AssignedTo == User.Identity.Name && l.LeadStatus == Lead.LeadStatusEnum.ACCEPTED).Count();

            if (User.IsInRole("Management") || User.IsInRole("Administrator") || User.IsInRole("Board") || User.IsInRole("Superadmin"))
            {
                return View(leads.ToList().OrderByDescending(l => l.InsertDate));
            }
            else
            {
                return View(leads.Where(l => l.LeadStatus != Lead.LeadStatusEnum.REJECTED || l.LeadStatus != Lead.LeadStatusEnum.ACCEPTED).ToList().OrderByDescending(l => l.InsertDate));
            }
        }

        // GET: Sales/Leads/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lead lead = db.Leads.Find(id);
            if (lead == null)
            {
                return HttpNotFound();
            }

            var _RelatedSalesContacts = (from c in db.Contacts
                                         where c.Organization.MerId == lead.RelatedOrganizationId && c.ContactType == Contact.ContactTypeEnum.SALES
                                         select c).AsEnumerable();
            var _RelatedLeadNotes = (from n in db.LeadNotes
                                     where n.RelatedLeadId == lead.LeadId
                                     select n).OrderByDescending(n => n.InsertDate).AsEnumerable();
            var _RelatedLeadActivities = (from a in db.ActivityLogs
                                          where a.ReferenceId == lead.LeadId && a.Module == ModuleEnum.Leads
                                          select a).OrderByDescending(a => a.InsertDate).AsEnumerable();
            var _RelatedOrganization = (from o in db.Organizations
                                        where o.MerId == lead.RelatedOrganizationId
                                        select o).First();
            var _RelatedOrganizationDetail = (from od in db.OrganizationDetails
                                              where od.MerId == lead.RelatedOrganizationId
                                              select od).First();
            var _RelatedCampaign = (from c in db.Campaigns
                                    where c.CampaignId == lead.RelatedCampaignId
                                    select c).First();
            var _Users = (from u in db.Users
                          select u).AsEnumerable();
            //var _LastLeadNote = (from n in db.LeadNotes
            //                     where n.RelatedLeadId == lead.LeadId
            //                     select n).OrderByDescending(n => n.InsertDate).Select(n => n.Note).First().ToString();

            var salesNoteTemplates = new List<ListItem>
                {
                    new ListItem{ Value = "razloženo funkcioniranje servisa (opis onoga što se dogodi nakon što korisnik klikne pošalji eRačun)", Text = "razloženo funkcioniranje servisa (opis onoga što se dogodi nakon što korisnik klikne pošalji eRačun)" },
                    new ListItem{ Value = "argumentirana korisnička podrška -- ažuriranje mailova (90% uspješnost), slanje tipske obavijesti, zvanje za preuzimanje (97% uspješnost)", Text = "argumentirana korisnička podrška -- ažuriranje mailova (90% uspješnost), slanje tipske obavijesti, zvanje za preuzimanje (97% uspješnost)" },
                    new ListItem{ Value = "objašnjena tehnička pozadina s ERPom", Text = "objašnjena tehnička pozadina s ERPom" },
                    new ListItem{ Value = "objašnjena tehnička pozadina s eRa aplikacijom", Text = "objašnjena tehnička pozadina s eRa aplikacijom" },
                    new ListItem{ Value = "razložena potvrda primitka, pretraživanje i arhiviranje", Text = "razložena potvrda primitka, pretraživanje i arhiviranje" },
                    new ListItem{ Value = "istaknuta jednostavnost uvođenja (kod izgovora nemamo vremena, prostora, u restrukturiranju smo)", Text = "istaknuta jednostavnost uvođenja (kod izgovora nemamo vremena, prostora, u restrukturiranju smo)" },
                    new ListItem{ Value = "osvježen kontakt i iznesene novosti", Text = "osvježen kontakt i iznesene novosti" },
                    new ListItem{ Value = "izvršen kvalitetan presing", Text = "izvršen kvalitetan presing" },
                    new ListItem{ Value = "izvršen salesforce (isticanje benefita uz forzu)", Text = "izvršen salesforce (isticanje benefita uz forzu)" },
                    new ListItem{ Value = "poslan mail ps (prijedlog suradnje)", Text = "poslan mail ps (prijedlog suradnje)" },
                    new ListItem{ Value = "kreirati i odaslati PND", Text = "kreirati i odaslati PND" },
                    new ListItem{ Value = "kreirati i odaslati UO", Text = "kreirati i odaslati UO" },
                    new ListItem{ Value = "održan sastanak, poslan FU", Text = "održan sastanak, poslan FU" },
                    new ListItem{ Value = "objašnjena zakonska pozadina i pravovaljanost eRačuna", Text = "objašnjena zakonska pozadina i pravovaljanost eRačuna" },
                    new ListItem{ Value = "kontaktirani za uvođenje zaprimanja", Text = "kontaktirani za uvođenje zaprimanja" },
                    new ListItem{ Value = "obrazložio slanje privitaka", Text = "obrazložio slanje privitaka" },
                    new ListItem{ Value = "obrazložio procesnu pokrivenost primatelja te odagnao brige i strahove u vezi preuzimanja od strane njihovih kupaca", Text = "obrazložio procesnu pokrivenost primatelja te odagnao brige i strahove u vezi preuzimanja od strane njihovih kupaca" }
                };

            var rejectReasonList = new List<ListItem>
            {
                new ListItem{ Value= "0", Text = "Ne želi navesti"},
                new ListItem{ Value= "1", Text = "Nema interesa za uslugu"},
                new ListItem{ Value= "2", Text = "Previsoka cijena"},
                new ListItem{ Value= "3", Text = "Neadekvatna ponuda"},
                new ListItem{ Value= "4", Text = "Koristi drugog posrednika"},
                new ListItem{ Value= "5", Text = "Nedostatak vremena za pokretanje projekta"},
                new ListItem{ Value= "6", Text = "Dio strane grupacije / Strano vlasništvo"},
                new ListItem{ Value= "7", Text = "Drugo / Ostalo"},
            };

            var LeadDetails = new LeadDetailViewModel()
            {
                LeadId = id,
                LeadDescription = lead.LeadDescription,
                LeadStatus = lead.LeadStatusString,
                RejectReason = lead.LeadRejectReasonString,
                OrganizationId = lead.RelatedOrganizationId,
                OrganizationName = _RelatedOrganization.SubjectName,
                OrganizationVAT = _RelatedOrganization.VAT,
                TelephoneNumber = _RelatedOrganizationDetail.TelephoneNumber,
                MobilePhoneNumber = _RelatedOrganizationDetail.MobilePhoneNumber,
                Email = _RelatedOrganizationDetail.EmailAddress,
                ERP = _RelatedOrganizationDetail.ERP,
                NumberOfInvoicesSent = _RelatedOrganizationDetail.NumberOfInvoicesSent,
                NumberOfInvoicesReceived = _RelatedOrganizationDetail.NumberOfInvoicesReceived,
                RelatedCampaignId = lead.RelatedCampaignId,
                RelatedCampaignName = _RelatedCampaign.CampaignName,
                IsAssigned = lead.IsAssigned,
                AssignedTo = lead.AssignedTo,
                LastContactedDate = lead.LastContactDate,
                LastContactedBy = lead.LastContactedBy,
                RelatedSalesContacts = _RelatedSalesContacts,
                RelatedLeadNotes = _RelatedLeadNotes,
                RelatedLeadActivities = _RelatedLeadActivities,
                Users = _Users,
                SalesNoteTemplates = salesNoteTemplates,
                RejectReasons = rejectReasonList
            };

            return View(LeadDetails);
        }

        // POST: Sales/Leads/AddNote
        [HttpPost]
        public ActionResult AddNote(LeadNoteHelper Model)
        {
            var lead = (from l in db.Leads
                        where l.LeadId == Model.RelatedLeadId
                        select l).First();
            var _NoteString = new StringBuilder();

            lead.LastContactDate = DateTime.Now;
            lead.LastContactedBy = User.Identity.Name;

            if (Model.NoteTemplates == null)
            {
                db.LeadNotes.Add(new LeadNote
                {
                    RelatedLeadId = Model.RelatedLeadId,
                    User = User.Identity.Name,
                    Note = Model.Note,
                    InsertDate = DateTime.Now,
                    Contact = Model.Contact
                });
                db.SaveChanges();
            }
            else
            {
                foreach (var Template in Model.NoteTemplates)
                {
                    _NoteString.AppendLine(Template);
                }

                db.LeadNotes.Add(new LeadNote
                {
                    RelatedLeadId = Model.RelatedLeadId,
                    User = User.Identity.Name,
                    Note = _NoteString + ";" + Model.Note,
                    InsertDate = DateTime.Now,
                    Contact = Model.Contact
                });
                db.SaveChanges();
            }

            if (Model.IsActivity == false)
            {
                return RedirectToAction("Details", new { id = Model.RelatedLeadId });
            }
            else
            {
                switch (Model.Identifier)
                {
                    case 1:
                        db.ActivityLogs.Add(new ActivityLog
                        {
                            Description = User.Identity.Name + " je obavio uspješan poziv vezan uz lead: " + lead.LeadTitle,
                            User = User.Identity.Name,
                            ReferenceId = Model.RelatedLeadId,
                            ActivityType = ActivityLog.ActivityTypeEnum.SUCCALL,
                            Department = ActivityLog.DepartmentEnum.Sales,
                            Module = ModuleEnum.Leads,
                            InsertDate = DateTime.Now
                        });
                        db.SaveChanges();
                        break;
                    case 2:
                        db.ActivityLogs.Add(new ActivityLog
                        {
                            Description = User.Identity.Name + " je obavio kraći informativni poziv vezan uz lead: " + lead.LeadTitle,
                            User = User.Identity.Name,
                            ReferenceId = Model.RelatedLeadId,
                            ActivityType = ActivityLog.ActivityTypeEnum.SUCCALSHORT,
                            Department = ActivityLog.DepartmentEnum.Sales,
                            Module = ModuleEnum.Leads,
                            InsertDate = DateTime.Now,
                        });
                        db.SaveChanges();
                        break;
                    case 3:
                        db.ActivityLogs.Add(new ActivityLog
                        {
                            Description = User.Identity.Name + " je pokušao obaviti telefonski poziv vezanvezan uz lead: " + lead.LeadTitle,
                            User = User.Identity.Name,
                            ReferenceId = Model.RelatedLeadId,
                            ActivityType = ActivityLog.ActivityTypeEnum.UNSUCCAL,
                            Department = ActivityLog.DepartmentEnum.Sales,
                            Module = ModuleEnum.Leads,
                            InsertDate = DateTime.Now,
                        });
                        db.SaveChanges();
                        break;
                }
                return RedirectToAction("Details", new { id = Model.RelatedLeadId });
            }
        }

        // POST: Sales/Leads/EditNote
        [HttpPost]
        [Authorize]
        public ActionResult EditNote(LeadNoteHelper Model)
        {
            var NoteForEdit = (from n in db.LeadNotes
                               where n.Id == Model.NoteId
                               select n).First();

            NoteForEdit.Note = Model.Note;
            NoteForEdit.Contact = Model.Contact;
            NoteForEdit.UpdateDate = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = Model.RelatedLeadId });
        }

        // POST: Sales/Opportunities/DeleteNote
        [HttpPost, ActionName("DeleteNote")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Superadmin")]
        public ActionResult DeleteNote(LeadNoteHelper Model)
        {
            LeadNote leadNote = db.LeadNotes.Find(Model.NoteId);
            db.LeadNotes.Remove(leadNote);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = Model.RelatedLeadId });
        }

        public void LogEmail(LeadNoteHelper Model)
        {
            var lead = (from l in db.Leads
                        where l.LeadId == Model.RelatedLeadId
                        select l).First();

            lead.LastContactDate = DateTime.Now;
            lead.LastContactedBy = Model.User;
            db.ActivityLogs.Add(new ActivityLog
            {
                Description = User.Identity.Name + " je poslao e-mail na adresu: " + Model.Email + " na temu prezentacije usluge u sklopu prodajne prilike: " + lead.LeadTitle,
                User = User.Identity.Name,
                ReferenceId = Model.RelatedLeadId,
                ActivityType = ActivityLog.ActivityTypeEnum.EMAIL,
                Department = ActivityLog.DepartmentEnum.Sales,
                Module = ModuleEnum.Leads,
                InsertDate = DateTime.Now,
            });
            db.SaveChanges();
        }

        public ActionResult Assign(LeadAssignHelper Model)
        {
            var lead = (from o in db.Leads
                        where o.LeadId == Model.RelatedLeadId
                        select o).First();
            lead.IsAssigned = true;
            lead.AssignedTo = Model.AssignedTo;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Reassign(LeadAssignHelper Model)
        {
            var lead = (from o in db.Leads
                        where o.LeadId == Model.RelatedLeadId
                        select o).First();
            if (Model.Unassign == true)
            {
                lead.IsAssigned = false;
                lead.AssignedTo = String.Empty;
                db.SaveChanges();
            }
            else
            {
                lead.AssignedTo = Model.AssignedTo;
                db.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        // GET: Sales/Opportunities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lead lead = db.Leads.Find(id);
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
        public ActionResult Edit(Lead _lead)
        {
            var Model = new LeadEditHelper()
            {
                LeadId = _lead.LeadId,
                LeadDescription = _lead.LeadDescription,
                LeadStatus = _lead.LeadStatus,
                RejectReason = _lead.RejectReason
            };
            if (ModelState.IsValid)
            {
                _lead.LeadDescription = Model.LeadDescription;
                _lead.LeadStatus = Model.LeadStatus;
                _lead.RejectReason = Model.RejectReason;
                _lead.UpdateDate = DateTime.Now;
                _lead.LastUpdatedBy = User.Identity.Name;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Model);
        }

        public ActionResult ChangeStatus(LeadChangeStatusHelper Model)
        {
            var lead = db.Leads.Find(Model.RelatedLeadId);
            lead.LeadStatus = Model.NewStatus;
            lead.UpdateDate = DateTime.Now;
            lead.LastUpdatedBy = User.Identity.Name;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult MarkRejected(LeadMarkRejectedHelper Model)
        {
            var lead = db.Leads.Find(Model.RelatedLeadId);
            lead.LeadStatus = Lead.LeadStatusEnum.REJECTED;
            lead.RejectReason = Model.RejectReason;
            lead.UpdateDate = DateTime.Now;
            lead.LastUpdatedBy = User.Identity.Name;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}