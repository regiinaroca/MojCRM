using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MojCRM.Models;
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
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Sales/Opportunities
        [Authorize]
        public ActionResult Index(OpportunitySearchHelper model)
        {
            var opportunities = from o in _db.Opportunities
                                select o;
            if (User.IsInRole("Management") || User.IsInRole("Administrator") || User.IsInRole("Board") || User.IsInRole("Superadmin"))
            {
                //Search Engine -- Admin
                if (!String.IsNullOrEmpty(model.Campaign))
                {
                    opportunities = opportunities.Where(op => op.RelatedCampaign.CampaignName.Contains(model.Campaign));
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                }
                if (!String.IsNullOrEmpty(model.Opportunity))
                {
                    opportunities = opportunities.Where(op => op.OpportunityTitle.Contains(model.Opportunity));
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                }
                if (!String.IsNullOrEmpty(model.Organization))
                {
                    opportunities = opportunities.Where(op => op.RelatedOrganization.SubjectName.Contains(model.Organization) || op.RelatedOrganization.VAT.Contains(model.Organization));
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                }
                if (!String.IsNullOrEmpty(model.OpportunityStatus.ToString()))
                {
                    opportunities = opportunities.Where(op => op.OpportunityStatus == model.OpportunityStatus);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                }
                if (!String.IsNullOrEmpty(model.RejectReason.ToString()))
                {
                    opportunities = opportunities.Where(op => op.RejectReason == model.RejectReason);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                }
                if (!String.IsNullOrEmpty(model.Assigned))
                {
                    if (model.Assigned == "1")
                    {
                        opportunities = opportunities.Where(op => op.IsAssigned == false);
                        ViewBag.SearchResults = opportunities.Count();
                        ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                    }
                    if (model.Assigned == "2")
                    {
                        opportunities = opportunities.Where(op => op.IsAssigned);
                        ViewBag.SearchResults = opportunities.Count();
                        ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                    }
                }
                if (!String.IsNullOrEmpty(model.AssignedTo))
                {
                    opportunities = opportunities.Where(op => op.AssignedTo == model.AssignedTo);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                }
            }
            else
            {
                opportunities = opportunities.Where(op => op.AssignedTo == User.Identity.Name);
                //Search Engine -- User
                if (!String.IsNullOrEmpty(model.Campaign))
                {
                    opportunities = opportunities.Where(op => op.RelatedCampaign.CampaignName.Contains(model.Campaign));
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                }
                if (!String.IsNullOrEmpty(model.Opportunity))
                {
                    opportunities = opportunities.Where(op => op.OpportunityTitle.Contains(model.Opportunity));
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                }
                if (!String.IsNullOrEmpty(model.Organization))
                {
                    opportunities = opportunities.Where(op => op.RelatedOrganization.SubjectName.Contains(model.Organization) || op.RelatedOrganization.VAT.Contains(model.Organization));
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                }
                if (!String.IsNullOrEmpty(model.OpportunityStatus.ToString()))
                {
                    opportunities = opportunities.Where(op => op.OpportunityStatus == model.OpportunityStatus);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                }
                if (!String.IsNullOrEmpty(model.RejectReason.ToString()))
                {
                    opportunities = opportunities.Where(op => op.RejectReason == model.RejectReason);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);
                }
            }

            ViewBag.SearchResults = opportunities.Count();
            ViewBag.SearchResultsAssigned = opportunities.Count(op => op.IsAssigned);

            ViewBag.UsersAssigned = opportunities.Count(op => op.AssignedTo == User.Identity.Name);
            ViewBag.UsersCreated = opportunities.Count(op => op.AssignedTo == User.Identity.Name && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.Start);
            ViewBag.UsersInContact = opportunities.Count(op => op.AssignedTo == User.Identity.Name && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.Incontact);
            ViewBag.UsersLead = opportunities.Count(op => op.AssignedTo == User.Identity.Name && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.Lead);
            ViewBag.UsersRejected = opportunities.Count(op => op.AssignedTo == User.Identity.Name && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.Rejected);

            if (User.IsInRole("Management") || User.IsInRole("Administrator") || User.IsInRole("Board") || User.IsInRole("Superadmin"))
            {
                return View(opportunities.OrderByDescending(op => op.InsertDate));
            }
            return View(opportunities.Where(op => op.OpportunityStatus != Opportunity.OpportunityStatusEnum.Lead || op.OpportunityStatus != Opportunity.OpportunityStatusEnum.Rejected).OrderByDescending(op => op.InsertDate));
        }

        // GET: Sales/Opportunities/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                Opportunity opportunity = _db.Opportunities.Find(id);
                if (opportunity == null)
                {
                    return HttpNotFound();
                }

                var relatedSalesContacts = (from c in _db.Contacts
                                             where c.Organization.MerId == opportunity.RelatedOrganizationId && c.ContactType == Contact.ContactTypeEnum.SALES
                                             select c);
                var relatedOpportunityNotes = (from n in _db.OpportunityNotes
                                                where n.RelatedOpportunityId == opportunity.OpportunityId
                                                select n).OrderByDescending(n => n.InsertDate);
                var relatedOpportunityActivities = (from a in _db.ActivityLogs
                                                     where a.ReferenceId == id && a.Module == ModuleEnum.Opportunities
                                                     select a).OrderByDescending(a => a.InsertDate);
                var relatedOrganization = (from o in _db.Organizations
                                            where o.MerId == opportunity.RelatedOrganizationId
                                            select o).First();
                var relatedOrganizationDetail = (from od in _db.OrganizationDetails
                                                  where od.MerId == opportunity.RelatedOrganizationId
                                                  select od).First();
                var relatedCampaign = (from c in _db.Campaigns
                                        where c.CampaignId == opportunity.RelatedCampaignId
                                        select c).First();
                var users = _db.Users;
                var relatedLeadId = 0;
                if (opportunity.OpportunityStatus == Opportunity.OpportunityStatusEnum.Lead)
                {
                    relatedLeadId = _db.Leads.Where(l => l.RelatedOpportunityId == id).Select(l => l.LeadId).First();
                }
                //var _LastOpportunityNote = (from n in db.OpportunityNotes
                //                            where n.RelatedOpportunityId == opportunity.OpportunityId
                //                            select n).OrderByDescending(n => n.InsertDate).Select(n => n.Note).First().ToString();

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

                var opportunityDetails = new OpportunityDetailViewModel()
                {
                    OpportunityId = id,
                    OpportunityDescription = opportunity.OpportunityDescription,
                    OpportunityStatus = opportunity.OpportunityStatus,
                    OpportunityStatusString = opportunity.OpportunityStatusString,
                    RejectReasson = opportunity.OpportunityRejectReasonString,
                    OrganizationId = opportunity.RelatedOrganizationId,
                    OrganizationName = relatedOrganization.SubjectName,
                    OrganizationVAT = relatedOrganization.VAT,
                    TelephoneNumber = relatedOrganizationDetail.TelephoneNumber,
                    MobilePhoneNumber = relatedOrganizationDetail.MobilePhoneNumber,
                    Email = relatedOrganizationDetail.EmailAddress,
                    ERP = relatedOrganizationDetail.ERP,
                    NumberOfInvoicesSent = relatedOrganizationDetail.NumberOfInvoicesSent,
                    NumberOfInvoicesReceived = relatedOrganizationDetail.NumberOfInvoicesReceived,
                    RelatedCampaignId = opportunity.RelatedCampaignId,
                    RelatedCampaignName = relatedCampaign.CampaignName,
                    IsAssigned = opportunity.IsAssigned,
                    AssignedTo = opportunity.AssignedTo,
                    LastContactedDate = opportunity.LastContactDate,
                    LastContactedBy = opportunity.LastContactedBy,
                    RelatedSalesContacts = relatedSalesContacts,
                    RelatedOpportunityNotes = relatedOpportunityNotes,
                    RelatedOpportunityActivities = relatedOpportunityActivities,
                    Users = users,
                    SalesNoteTemplates = salesNoteTemplates,
                    RejectReasons = rejectReasonList,
                    RelatedLeadId = relatedLeadId
                };

                return View(opportunityDetails);
            }
            catch (InvalidOperationException ioe)
            {
                _db.LogError.Add(new LogError
                {
                    Method = @"Opportunities - Details",
                    Parameters = id.ToString(),
                    Message = @"Prilikom ulaska u detalje prodajne prilike javila se greška: " + ioe.Message,
                    User = User.Identity.Name,
                    InsertDate = DateTime.Now
                });
                _db.SaveChanges();
                return View("ErrorNoLead");
            }
        }

        // POST: Sales/Opportunities/AddNote
        [HttpPost]
        public ActionResult AddNote(OpportunityNoteHelper model)
        {
            var relatedOpportunity = (from o in _db.Opportunities
                                       where o.OpportunityId == model.RelatedOpportunityId
                                       select o).First();
            var noteString = new StringBuilder();

            relatedOpportunity.LastContactDate = DateTime.Now;
            relatedOpportunity.LastContactedBy = User.Identity.Name;

            if (model.NoteTemplates == null)
            {
                _db.OpportunityNotes.Add(new OpportunityNote
                {
                    RelatedOpportunityId = model.RelatedOpportunityId,
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

                _db.OpportunityNotes.Add(new OpportunityNote
                {
                    RelatedOpportunityId = model.RelatedOpportunityId,
                    User = User.Identity.Name,
                    Note = noteString + ";" + model.Note,
                    InsertDate = DateTime.Now,
                    Contact = model.Contact
                });
                _db.SaveChanges();
            }

            if (model.IsActivity == false)
            {
                return RedirectToAction("Details", new { id = model.RelatedOpportunityId });
            }
            switch (model.Identifier)
            {
                case 1:
                    _db.ActivityLogs.Add(new ActivityLog
                    {
                        Description = User.Identity.Name + " je obavio uspješan poziv vezan za prodajnu priliku: " + relatedOpportunity.OpportunityTitle,
                        User = User.Identity.Name,
                        ReferenceId = model.RelatedOpportunityId,
                        ActivityType = ActivityTypeEnum.Succall,
                        Department = DepartmentEnum.Sales,
                        Module = ModuleEnum.Opportunities,
                        InsertDate = DateTime.Now
                    });
                    _db.SaveChanges();
                    break;
                case 2:
                    _db.ActivityLogs.Add(new ActivityLog
                    {
                        Description = User.Identity.Name + " je obavio kraći informativni poziv vezano za prodajnu priliku: " + relatedOpportunity.OpportunityTitle,
                        User = User.Identity.Name,
                        ReferenceId = model.RelatedOpportunityId,
                        ActivityType = ActivityTypeEnum.Succalshort,
                        Department = DepartmentEnum.Sales,
                        Module = ModuleEnum.Opportunities,
                        InsertDate = DateTime.Now,
                    });
                    _db.SaveChanges();
                    break;
                case 3:
                    _db.ActivityLogs.Add(new ActivityLog
                    {
                        Description = model.User + " je pokušao obaviti telefonski poziv vezano za prodajnu priliku: " + relatedOpportunity.OpportunityTitle,
                        User = model.User,
                        ReferenceId = model.RelatedOpportunityId,
                        ActivityType = ActivityTypeEnum.Unsuccal,
                        Department = DepartmentEnum.Sales,
                        Module = ModuleEnum.Opportunities,
                        InsertDate = DateTime.Now,
                    });
                    _db.SaveChanges();
                    break;
            }

            return RedirectToAction("Details", new { id = model.RelatedOpportunityId });
        }

        // POST: Sales/Opportunities/EditNote
        [HttpPost]
        [Authorize]
        public ActionResult EditNote(OpportunityNoteHelper model)
        {
            var noteForEdit = (from n in _db.OpportunityNotes
                               where n.Id == model.NoteId
                               select n).First();

            noteForEdit.Note = model.Note;
            noteForEdit.Contact = model.Contact;
            noteForEdit.UpdateDate = DateTime.Now;
            _db.SaveChanges();

            return RedirectToAction("Details", new { id = model.RelatedOpportunityId });
        }

        // POST: Sales/Opportunities/DeleteNote
        [HttpPost, ActionName("DeleteNote")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Superadmin")]
        public ActionResult DeleteNote(OpportunityNoteHelper model)
        {
            OpportunityNote opportunityNote = _db.OpportunityNotes.Find(model.NoteId);
            _db.OpportunityNotes.Remove(opportunityNote);
            _db.SaveChanges();
            return RedirectToAction("Details", new { id = model.RelatedOpportunityId });
        }

        public void LogEmail(OpportunityNoteHelper model)
        {
            var relatedOpportunity = (from o in _db.Opportunities
                                       where o.OpportunityId == model.RelatedOpportunityId
                                       select o).First();

            relatedOpportunity.LastContactDate = DateTime.Now;
            relatedOpportunity.LastContactedBy = model.User;
            _db.ActivityLogs.Add(new ActivityLog
            {
                Description = model.User + " je poslao e-mail na adresu: " + model.Email + " na temu prezentacije usluge u sklopu prodajne prilike: " + relatedOpportunity.OpportunityTitle,
                User = model.User,
                ReferenceId = model.RelatedOpportunityId,
                ActivityType = ActivityTypeEnum.Email,
                Department = DepartmentEnum.Sales,
                Module = ModuleEnum.Opportunities,
                InsertDate = DateTime.Now,
            });
            _db.SaveChanges();
        }

        public ActionResult Assign(OpportunityAssignHelper model)
        {
            var opportunity = (from o in _db.Opportunities
                               where o.OpportunityId == model.RelatedOpportunityId
                               select o).First();
            opportunity.IsAssigned = true;
            opportunity.AssignedTo = model.AssignedTo;
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Reassign(OpportunityAssignHelper model)
        {
            var opportunity = (from o in _db.Opportunities
                               where o.OpportunityId == model.RelatedOpportunityId
                               select o).First();
            if (model.Unassign)
            {
                opportunity.IsAssigned = false;
                opportunity.AssignedTo = String.Empty;
                _db.SaveChanges();
            }
            else
            {
                opportunity.AssignedTo = model.AssignedTo;
                _db.SaveChanges();
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
            Opportunity opportunity = _db.Opportunities.Find(id);
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
        public ActionResult Edit(Opportunity opportunity)
        {
            var model = new OpportunityEditHelper()
            {
                OpportunityId = opportunity.OpportunityId,
                OpportunityDescription = opportunity.OpportunityDescription,
                OpportunityStatus = opportunity.OpportunityStatus,
                RejectReason = opportunity.RejectReason
            };
            if (ModelState.IsValid)
            {
                var opportunityNew = _db.Opportunities.Find(model.OpportunityId);
                opportunityNew.OpportunityDescription = model.OpportunityDescription;
                opportunityNew.OpportunityStatus = model.OpportunityStatus;
                opportunityNew.RejectReason = model.RejectReason;
                opportunityNew.UpdateDate = DateTime.Now;
                opportunityNew.LastUpdatedBy = User.Identity.Name;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult ConvertToLead(ConvertToLeadHelper model)
        {
            try
            {
                var lastOpportunityNote = _db.OpportunityNotes
                .Where(on => on.RelatedOpportunityId == model.OpportunityId)
                .OrderByDescending(on => on.Id)
                .First();

                _db.Leads.Add(new Lead()
                {
                    LeadTitle = model.OrganizationName,
                    LeadDescription = model.OpportunityDescription,
                    RelatedCampaignId = model.RelatedCampaignId,
                    RelatedOpportunityId = model.OpportunityId,
                    RelatedOrganizationId = model.OrganizationId,
                    LeadStatus = Lead.LeadStatusEnum.Start,
                    CreatedBy = User.Identity.Name,
                    AssignedTo = User.Identity.Name,
                    IsAssigned = model.IsAssigned,
                    InsertDate = DateTime.Now
                });
                _db.ActivityLogs.Add(new ActivityLog()
                {
                    Description = User.Identity.Name + " je kreirao lead za tvrtku: " + model.OrganizationName + " (Kampanja: " + model.RelatedCampaignName + ")",
                    User = User.Identity.Name,
                    ReferenceId = model.OpportunityId,
                    ActivityType = ActivityTypeEnum.Createdlead,
                    Department = DepartmentEnum.Sales,
                    Module = ModuleEnum.Opportunities,
                    InsertDate = DateTime.Now
                });
                var opportunity = _db.Opportunities.Find(model.OpportunityId);
                opportunity.OpportunityStatus = Opportunity.OpportunityStatusEnum.Lead;
                opportunity.UpdateDate = DateTime.Now;
                opportunity.LastUpdatedBy = User.Identity.Name;
                _db.SaveChanges();

                var leadId = _db.Leads.Where(l => l.RelatedCampaignId == model.RelatedCampaignId && l.RelatedOpportunityId == model.OpportunityId).Select(l => l.LeadId).First();
                _db.LeadNotes.Add(new LeadNote
                {
                    RelatedLeadId = leadId,
                    User = lastOpportunityNote.User,
                    Note = "ZADNJA BILJEŠKA IZ PRODAJNE PRILIKE: " + lastOpportunityNote.Note,
                    InsertDate = DateTime.Now,
                    Contact = lastOpportunityNote.Contact
                });
                _db.SaveChanges();
                return Redirect(Request.UrlReferrer?.ToString());
            }
            catch (InvalidOperationException)
            {
                return View("_ErrorNoNote");
            }
        }

        public ActionResult ChangeStatus(OpportunityChangeStatusHelper model)
        {
            var opportunity = _db.Opportunities.Find(model.RelatedOpportunityId);
            opportunity.OpportunityStatus = model.NewStatus;
            opportunity.StatusDescription = model.StatusDescription;
            opportunity.UpdateDate = DateTime.Now;
            opportunity.LastUpdatedBy = User.Identity.Name;
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult MarkRejected(OpportunityMarkRejectedHelper model)
        {
            var opportunity = _db.Opportunities.Find(model.RelatedOpportunityId);
            opportunity.OpportunityStatus = Opportunity.OpportunityStatusEnum.Rejected;
            opportunity.RejectReason = model.RejectReason;
            opportunity.RejectReasonDescription = model.RejectReasonDescription;
            opportunity.UpdateDate = DateTime.Now;
            opportunity.LastUpdatedBy = User.Identity.Name;
            _db.SaveChanges();

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