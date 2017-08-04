using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MojCRM.Areas.Campaigns.Models;
using MojCRM.Models;
using MojCRM.Areas.Campaigns.ViewModels;
using MojCRM.Areas.Sales.Models;
using Excel = Microsoft.Office.Interop.Excel;
using MojCRM.Areas.Sales.ViewModels;
using MojCRM.Areas.Sales.Helpers;
using System.Text;
using System.Web.UI.WebControls;
using static MojCRM.Models.ActivityLog;

namespace MojCRM.Areas.Sales.Controllers
{
    public class OpportunitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Sales/Opportunities
        [Authorize]
        public ActionResult Index(OpportunitySearchHelper Model)
        {
            var opportunities = from o in db.Opportunities
                                select o;
            if (User.IsInRole("Management") || User.IsInRole("Administrator") || User.IsInRole("Board") || User.IsInRole("Superadmin"))
            {
                //Search Engine -- Admin
                if (!String.IsNullOrEmpty(Model.Campaign))
                {
                    opportunities = opportunities.Where(op => op.RelatedCampaign.CampaignName.Contains(Model.Campaign));
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Opportunity))
                {
                    opportunities = opportunities.Where(op => op.OpportunityTitle.Contains(Model.Opportunity));
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Organization))
                {
                    opportunities = opportunities.Where(op => op.RelatedOrganization.SubjectName.Contains(Model.Organization) || op.RelatedOrganization.VAT.Contains(Model.Organization));
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.OpportunityStatus.ToString()))
                {
                    opportunities = opportunities.Where(op => op.OpportunityStatus == Model.OpportunityStatus);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.RejectReason.ToString()))
                {
                    opportunities = opportunities.Where(op => op.RejectReason == Model.RejectReason);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Assigned))
                {
                    if (Model.Assigned == "1")
                    {
                        opportunities = opportunities.Where(op => op.IsAssigned == false);
                        ViewBag.SearchResults = opportunities.Count();
                        ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                    }
                    if (Model.Assigned == "2")
                    {
                        opportunities = opportunities.Where(op => op.IsAssigned == true);
                        ViewBag.SearchResults = opportunities.Count();
                        ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                    }
                }
                if (!String.IsNullOrEmpty(Model.AssignedTo))
                {
                    opportunities = opportunities.Where(op => op.AssignedTo == Model.AssignedTo);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
            }
            else
            {
                opportunities = opportunities.Where(op => op.AssignedTo == User.Identity.Name);
                //Search Engine -- User
                if (!String.IsNullOrEmpty(Model.Campaign))
                {
                    opportunities = opportunities.Where(op => op.RelatedCampaign.CampaignName.Contains(Model.Campaign));
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Opportunity))
                {
                    opportunities = opportunities.Where(op => op.OpportunityTitle.Contains(Model.Opportunity));
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Organization))
                {
                    opportunities = opportunities.Where(op => op.RelatedOrganization.SubjectName.Contains(Model.Organization) || op.RelatedOrganization.VAT.Contains(Model.Organization));
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.OpportunityStatus.ToString()))
                {
                    opportunities = opportunities.Where(op => op.OpportunityStatus == Model.OpportunityStatus);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.RejectReason.ToString()))
                {
                    opportunities = opportunities.Where(op => op.RejectReason == Model.RejectReason);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
            }

            ViewBag.SearchResults = opportunities.Count();
            ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();

            ViewBag.UsersAssigned = opportunities.Where(op => op.AssignedTo == User.Identity.Name).Count();
            ViewBag.UsersCreated = opportunities.Where(op => op.AssignedTo == User.Identity.Name && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.START).Count();
            ViewBag.UsersInContact = opportunities.Where(op => op.AssignedTo == User.Identity.Name && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.INCONTACT).Count();
            ViewBag.UsersLead = opportunities.Where(op => op.AssignedTo == User.Identity.Name && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.LEAD).Count();
            ViewBag.UsersRejected = opportunities.Where(op => op.AssignedTo == User.Identity.Name && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.REJECTED).Count();

            if (User.IsInRole("Management") || User.IsInRole("Administrator") || User.IsInRole("Board") || User.IsInRole("Superadmin"))
            {
                return View(opportunities.ToList().OrderByDescending(op => op.InsertDate));
            }
            else
            {
                return View(opportunities.Where(op => op.OpportunityStatus != Opportunity.OpportunityStatusEnum.LEAD || op.OpportunityStatus != Opportunity.OpportunityStatusEnum.REJECTED).ToList().OrderByDescending(op => op.InsertDate));
            }
        }

        // GET: Sales/Opportunities/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Opportunity opportunity = db.Opportunities.Find(id);
            if (opportunity == null)
            {
                return HttpNotFound();
            }

            var _RelatedSalesContacts = (from c in db.Contacts
                                         where c.Organization.MerId == opportunity.RelatedOrganizationId && c.ContactType == Contact.ContactTypeEnum.SALES
                                         select c).AsEnumerable();
            var _RelatedOpportunityNotes = (from n in db.OpportunityNotes
                                            where n.RelatedOpportunityId == opportunity.OpportunityId
                                            select n).OrderByDescending(n => n.InsertDate).AsEnumerable();
            var _RelatedOpportunityActivities = (from a in db.ActivityLogs
                                                 where a.ReferenceId == id && a.Module == ModuleEnum.Opportunities
                                                 select a).OrderByDescending(a => a.InsertDate).AsEnumerable();
            var _RelatedOrganization = (from o in db.Organizations
                                        where o.MerId == opportunity.RelatedOrganizationId
                                        select o).First();
            var _RelatedOrganizationDetail = (from od in db.OrganizationDetails
                                              where od.MerId == opportunity.RelatedOrganizationId
                                              select od).First();
            var _RelatedCampaign = (from c in db.Campaigns
                                    where c.CampaignId == opportunity.RelatedCampaignId
                                    select c).First();
            var _Users = (from u in db.Users
                          select u).AsEnumerable();
            //var _LastOpportunityNote = (from n in db.OpportunityNotes
            //                            where n.RelatedOpportunityId == opportunity.OpportunityId
            //                            select n).OrderByDescending(n => n.InsertDate).Select(n => n.Note).First().ToString();

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

            var OpportunityDetails = new OpportunityDetailViewModel()
            {
                OpportunityId = id,
                OpportunityDescription = opportunity.OpportunityDescription,
                OpportunityStatus = opportunity.OpportunityStatusString,
                RejectReasson = opportunity.OpportunityRejectReasonString,
                OrganizationId = opportunity.RelatedOrganizationId,
                OrganizationName = _RelatedOrganization.SubjectName,
                OrganizationVAT = _RelatedOrganization.VAT,
                TelephoneNumber = _RelatedOrganizationDetail.TelephoneNumber,
                MobilePhoneNumber = _RelatedOrganizationDetail.MobilePhoneNumber,
                Email = _RelatedOrganizationDetail.EmailAddress,
                ERP = _RelatedOrganizationDetail.ERP,
                NumberOfInvoicesSent = _RelatedOrganizationDetail.NumberOfInvoicesSent,
                NumberOfInvoicesReceived = _RelatedOrganizationDetail.NumberOfInvoicesReceived,
                RelatedCampaignId = opportunity.RelatedCampaignId,
                RelatedCampaignName = _RelatedCampaign.CampaignName,
                IsAssigned = opportunity.IsAssigned,
                AssignedTo = opportunity.AssignedTo,
                LastContactedDate = opportunity.LastContactDate,
                LastContactedBy = opportunity.LastContactedBy,
                RelatedSalesContacts = _RelatedSalesContacts,
                RelatedOpportunityNotes = _RelatedOpportunityNotes,
                RelatedOpportunityActivities = _RelatedOpportunityActivities,
                Users = _Users,
                SalesNoteTemplates = salesNoteTemplates,
                RejectReasons = rejectReasonList
            };

            return View(OpportunityDetails);
        }

        // POST: Sales/Opportunities/AddNote
        [HttpPost]
        public ActionResult AddNote(OpportunityNoteHelper Model)
        {
            var _RelatedOpportunity = (from o in db.Opportunities
                                       where o.OpportunityId == Model.RelatedOpportunityId
                                       select o).First();
            var _NoteString = new StringBuilder();

            _RelatedOpportunity.LastContactDate = DateTime.Now;
            _RelatedOpportunity.LastContactedBy = User.Identity.Name;

            if (Model.NoteTemplates == null)
            {
                db.OpportunityNotes.Add(new OpportunityNote
                {
                    RelatedOpportunityId = Model.RelatedOpportunityId,
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

                db.OpportunityNotes.Add(new OpportunityNote
                {
                    RelatedOpportunityId = Model.RelatedOpportunityId,
                    User = User.Identity.Name,
                    Note = _NoteString + ";" + Model.Note,
                    InsertDate = DateTime.Now,
                    Contact = Model.Contact
                });
                db.SaveChanges();
            }

            if (Model.IsActivity == false)
            {
                return RedirectToAction("Details", new { id = Model.RelatedOpportunityId });
            }
            else
            {
                switch (Model.Identifier)
                {
                    case 1:
                        db.ActivityLogs.Add(new ActivityLog
                        {
                            Description = User.Identity.Name + " je obavio uspješan poziv vezan za prodajnu priliku: " + _RelatedOpportunity.OpportunityTitle,
                            User = User.Identity.Name,
                            ReferenceId = Model.RelatedOpportunityId,
                            ActivityType = ActivityLog.ActivityTypeEnum.SUCCALL,
                            Department = ActivityLog.DepartmentEnum.Sales,
                            Module = ModuleEnum.Opportunities,
                            InsertDate = DateTime.Now
                        });
                        db.SaveChanges();
                        break;
                    case 2:
                        db.ActivityLogs.Add(new ActivityLog
                        {
                            Description = User.Identity.Name + " je obavio kraći informativni poziv vezano za prodajnu priliku: " + _RelatedOpportunity.OpportunityTitle,
                            User = User.Identity.Name,
                            ReferenceId = Model.RelatedOpportunityId,
                            ActivityType = ActivityLog.ActivityTypeEnum.SUCCALSHORT,
                            Department = ActivityLog.DepartmentEnum.Sales,
                            Module = ModuleEnum.Opportunities,
                            InsertDate = DateTime.Now,
                        });
                        db.SaveChanges();
                        break;
                    case 3:
                        db.ActivityLogs.Add(new ActivityLog
                        {
                            Description = Model.User + " je pokušao obaviti telefonski poziv vezano za prodajnu priliku: " + _RelatedOpportunity.OpportunityTitle,
                            User = Model.User,
                            ReferenceId = Model.RelatedOpportunityId,
                            ActivityType = ActivityLog.ActivityTypeEnum.UNSUCCAL,
                            Department = ActivityLog.DepartmentEnum.Sales,
                            Module = ModuleEnum.Opportunities,
                            InsertDate = DateTime.Now,
                        });
                        db.SaveChanges();
                        break;
                }

                return RedirectToAction("Details", new { id = Model.RelatedOpportunityId });
            }
        }

        // POST: Sales/Opportunities/EditNote
        [HttpPost]
        [Authorize]
        public ActionResult EditNote(OpportunityNoteHelper Model)
        {
            var NoteForEdit = (from n in db.OpportunityNotes
                               where n.Id == Model.NoteId
                               select n).First();

            NoteForEdit.Note = Model.Note;
            NoteForEdit.Contact = Model.Contact;
            NoteForEdit.UpdateDate = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = Model.RelatedOpportunityId });
        }

        // POST: Sales/Opportunities/DeleteNote
        [HttpPost, ActionName("DeleteNote")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Superadmin")]
        public ActionResult DeleteNote(OpportunityNoteHelper Model)
        {
            OpportunityNote opportunityNote = db.OpportunityNotes.Find(Model.NoteId);
            db.OpportunityNotes.Remove(opportunityNote);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = Model.RelatedOpportunityId });
        }

        public void LogEmail(OpportunityNoteHelper Model)
        {
            var _RelatedOpportunity = (from o in db.Opportunities
                                       where o.OpportunityId == Model.RelatedOpportunityId
                                       select o).First();

            _RelatedOpportunity.LastContactDate = DateTime.Now;
            _RelatedOpportunity.LastContactedBy = Model.User;
            db.ActivityLogs.Add(new ActivityLog
            {
                Description = Model.User + " je poslao e-mail na adresu: " + Model.Email + " na temu prezentacije usluge u sklopu prodajne prilike: " + _RelatedOpportunity.OpportunityTitle,
                User = Model.User,
                ReferenceId = Model.RelatedOpportunityId,
                ActivityType = ActivityLog.ActivityTypeEnum.EMAIL,
                Department = ActivityLog.DepartmentEnum.Sales,
                Module = ModuleEnum.Opportunities,
                InsertDate = DateTime.Now,
            });
            db.SaveChanges();
        }

        public ActionResult Assign(OpportunityAssignHelper Model)
        {
            var opportunity = (from o in db.Opportunities
                               where o.OpportunityId == Model.RelatedOpportunityId
                               select o).First();
            opportunity.IsAssigned = true;
            opportunity.AssignedTo = Model.AssignedTo;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Reassign(OpportunityAssignHelper Model)
        {
            var opportunity = (from o in db.Opportunities
                               where o.OpportunityId == Model.RelatedOpportunityId
                               select o).First();
            if (Model.Unassign == true)
            {
                opportunity.IsAssigned = false;
                opportunity.AssignedTo = String.Empty;
                db.SaveChanges();
            }
            else
            {
                opportunity.AssignedTo = Model.AssignedTo;
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
            Opportunity opportunity = db.Opportunities.Find(id);
            if (opportunity == null)
            {
                return HttpNotFound();
            }
            return View(opportunity);
        }

        // POST: Sales/Opportunities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Opportunity _Opportunity)
        {
            var Model = new OpportunityEditHelper()
            {
                OpportunityId = _Opportunity.OpportunityId,
                OpportunityDescription = _Opportunity.OpportunityDescription,
                OpportunityStatus = _Opportunity.OpportunityStatus,
                RejectReason = _Opportunity.RejectReason
            };
            if (ModelState.IsValid)
            {
                var opportunity = db.Opportunities.Find(Model.OpportunityId);
                opportunity.OpportunityDescription = Model.OpportunityDescription;
                opportunity.OpportunityStatus = Model.OpportunityStatus;
                opportunity.RejectReason = Model.RejectReason;
                opportunity.UpdateDate = DateTime.Now;
                opportunity.LastUpdatedBy = User.Identity.Name;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Model);
        }

        public ActionResult ConvertToLead(ConvertToLeadHelper Model)
        {
            db.Leads.Add(new Lead()
            {
                LeadTitle = Model.OrganizationName,
                LeadDescription = Model.OpportunityDescription,
                RelatedCampaignId = Model.RelatedCampaignId,
                RelatedOpportunityId = Model.OpportunityId,
                RelatedOrganizationId = Model.OrganizationId,
                LeadStatus = Lead.LeadStatusEnum.START,
                CreatedBy = User.Identity.Name,
                AssignedTo = User.Identity.Name,
                IsAssigned = Model.IsAssigned,
                InsertDate = DateTime.Now
            });
            db.ActivityLogs.Add(new ActivityLog()
            {
                Description = User.Identity.Name + " je kreirao lead za tvrtku: " + Model.OrganizationName + " (Kampanja: " + Model.RelatedCampaignName + ")",
                User = User.Identity.Name,
                ReferenceId = Model.OpportunityId,
                ActivityType = ActivityLog.ActivityTypeEnum.CREATEDLEAD,
                Department = ActivityLog.DepartmentEnum.Sales,
                Module = ModuleEnum.Opportunities,
                InsertDate = DateTime.Now
            });
            var opportunity = db.Opportunities.Find(Model.OpportunityId);
            opportunity.OpportunityStatus = Opportunity.OpportunityStatusEnum.LEAD;
            opportunity.UpdateDate = DateTime.Now;
            opportunity.LastUpdatedBy = User.Identity.Name;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult ChangeStatus(OpportunityChangeStatusHelper Model)
        {
            var opportunity = db.Opportunities.Find(Model.RelatedOpportunityId);
            opportunity.OpportunityStatus = Model.NewStatus;
            opportunity.UpdateDate = DateTime.Now;
            opportunity.LastUpdatedBy = User.Identity.Name;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult MarkRejected(OpportunityMarkRejectedHelper Model)
        {
            var opportunity = db.Opportunities.Find(Model.RelatedOpportunityId);
            opportunity.OpportunityStatus = Opportunity.OpportunityStatusEnum.REJECTED;
            opportunity.RejectReason = Model.RejectReason;
            opportunity.UpdateDate = DateTime.Now;
            opportunity.LastUpdatedBy = User.Identity.Name;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public void ImportOpportunities()
        {
            //create a instance for the Excel object  
            Excel.Application oExcel = new Excel.Application();

            //specify the file name where its actually exist  
            string filepath = @"C:\Temp\Test.xlsx";

            //pass that to workbook object  
            Excel.Workbook WB = oExcel.Workbooks.Open(filepath);


            // statement get the workbookname  
            string ExcelWorkbookname = WB.Name;

            // statement get the worksheet count  
            int worksheetcount = WB.Worksheets.Count;

            Excel.Worksheet wks = (Excel.Worksheet)WB.Worksheets[1];

            // statement get the firstworksheetname  

            string firstworksheetname = wks.Name;

            //statement get the first cell value  
            var firstcellvalue = ((Excel.Range)wks.Cells[1, 1]).Value;
        }
    }
}