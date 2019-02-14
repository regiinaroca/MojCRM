using MojCRM.Areas.Sales.Helpers;
using MojCRM.Areas.Sales.Models;
using MojCRM.Areas.Sales.ViewModels;
using MojCRM.Helpers;
using MojCRM.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static MojCRM.Models.ActivityLog;

namespace MojCRM.Areas.Sales.Controllers
{
    public class EducationController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly HelperMethods _helper = new HelperMethods();

        // GET: Sales/Education
        public ActionResult Index(EducationSearchHelper model)
        {
            var educations = _db.Educations.Select(o => o);
            if (User.IsInRole("Management") || User.IsInRole("Administrator") || User.IsInRole("Board") || User.IsInRole("Superadmin"))
            {
                //Search Engine -- Admin
                if (!String.IsNullOrEmpty(model.Campaign))
                {
                    educations = educations.Where(op => op.RelatedCampaign.CampaignName.Contains(model.Campaign));
                }
                if (!String.IsNullOrEmpty(model.Education))
                {
                    educations = educations.Where(op => op.EducationEntityTitle.Contains(model.Education));
                }
                if (!String.IsNullOrEmpty(model.Organization))
                {
                    educations = educations.Where(op => op.RelatedOrganization.SubjectName.Contains(model.Organization) || op.RelatedOrganization.VAT.Contains(model.Organization));
                }
                if (!String.IsNullOrEmpty(model.LastContactDate))
                {
                    var dateTemp = Convert.ToDateTime(model.LastContactDate);
                    var dateTempPlus = dateTemp.AddDays(1);
                    educations = educations.Where(op => op.LastContactDate >= dateTemp && op.LastContactDate < dateTempPlus);
                }
                if (!String.IsNullOrEmpty(model.EducationStatus.ToString()))
                {
                    educations = educations.Where(op => op.EducationEntityStatus == model.EducationStatus);
                }
                if (!String.IsNullOrEmpty(model.RejectReason.ToString()))
                {
                    educations = educations.Where(op => op.EducationRejectReason == model.RejectReason);
                }
                if (!String.IsNullOrEmpty(model.Priority.ToString()))
                {
                    educations = educations.Where(op => op.Priority == model.Priority);
                }
                if (!String.IsNullOrEmpty(model.Assigned))
                {
                    if (model.Assigned == "1")
                    {
                        educations = educations.Where(op => op.IsAssigned == false);
                    }
                    if (model.Assigned == "2")
                    {
                        educations = educations.Where(op => op.IsAssigned);
                    }
                }
                if (!String.IsNullOrEmpty(model.AssignedTo))
                {
                    educations = educations.Where(op => op.AssignedTo == model.AssignedTo);
                }
            }
            else
            {
                educations = educations.Where(op => op.AssignedTo == User.Identity.Name && op.RelatedCampaign.CampaignStatus == Campaigns.Models.Campaign.CampaignStatusEnum.InProgress);
                //Search Engine -- User
                if (!String.IsNullOrEmpty(model.Campaign))
                {
                    educations = educations.Where(op => op.RelatedCampaign.CampaignName.Contains(model.Campaign));
                }
                if (!String.IsNullOrEmpty(model.Education))
                {
                    educations = educations.Where(op => op.EducationEntityTitle.Contains(model.Education));
                }
                if (!String.IsNullOrEmpty(model.Organization))
                {
                    educations = educations.Where(op => op.RelatedOrganization.SubjectName.Contains(model.Organization) || op.RelatedOrganization.VAT.Contains(model.Organization));
                }
                if (!String.IsNullOrEmpty(model.LastContactDate))
                {
                    var dateTemp = Convert.ToDateTime(model.LastContactDate);
                    var dateTempPlus = dateTemp.AddDays(1);
                    educations = educations.Where(op => op.LastContactDate >= dateTemp && op.LastContactDate < dateTempPlus);
                }
                if (!String.IsNullOrEmpty(model.EducationStatus.ToString()))
                {
                    educations = educations.Where(op => op.EducationEntityStatus == model.EducationStatus);
                }
                if (!String.IsNullOrEmpty(model.RejectReason.ToString()))
                {
                    educations = educations.Where(op => op.EducationRejectReason == model.RejectReason);
                }
                if (!String.IsNullOrEmpty(model.Priority.ToString()))
                {
                    educations = educations.Where(op => op.Priority == model.Priority);
                }
            }

            ViewBag.SearchResults = educations.Count();
            ViewBag.SearchResultsAssigned = educations.Count(l => l.IsAssigned);

            var returnModel = new EducationIndexViewModel()
            {
                Users = _db.Users
            };

            if (User.IsInRole("Management") || User.IsInRole("Administrator") || User.IsInRole("Board") || User.IsInRole("Superadmin"))
            {
                returnModel.Educations = educations.OrderByDescending(op => op.Priority);
                return View(returnModel);
            }

            returnModel.Educations = educations.Where(op => op.EducationEntityStatus != Models.Education.EducationEntityStatusEnum.Rejected).OrderByDescending(op => op.Priority);
            return View(returnModel);
        }

        // GET: Sales/Education/Details/5
        public ActionResult Details(int id)
        {
            Education education = _db.Educations.Find(id);
            string relatedCampaignName = String.Empty;
            if (education == null)
            {
                return HttpNotFound();
            }

            var relatedSalesContacts = _db.Contacts.Where(c =>
                c.Organization.MerId == education.RelatedOrganizationId &&
                c.ContactType == Contact.ContactTypeEnum.Sales);
            var relatedEducationNotes = _db.EducationNotes.Where(n =>
                n.RelatedEducationEntityId == education.Id).OrderByDescending(n => n.InsertDate);
            var relatedEducationActivities = _db.ActivityLogs.Where(a =>
                a.ReferenceId == id && a.Module == ModuleEnum.Educations).OrderByDescending(a => a.InsertDate);
            var relatedOrganization = _db.Organizations.First(o => o.MerId == education.RelatedOrganizationId);
            var relatedCampaign = _db.Campaigns.First(c => c.CampaignId == education.RelatedCampaignId);
            relatedCampaignName = relatedCampaign.CampaignName;

            var users = _db.Users;

            var rejectReasonList = new List<ListItem>
            {
                new ListItem{ Value= @"3", Text = @"Ne želi navesti"},
                new ListItem{ Value= @"0", Text = @"Neodgovarajući termin"},
                new ListItem{ Value= @"1", Text = @"Nezainteresiranost"},
                new ListItem{ Value= @"2", Text = @"Usluga nije potrebna"}
            };

            var educationDetails = new EducationDetailViewModel()
            {
                Education = education,
                RelatedSalesContacts = relatedSalesContacts,
                RelatedEducationNotes = relatedEducationNotes,
                RelatedEducationActivities = relatedEducationActivities,
                Users = users,
                RejectReasons = rejectReasonList
            };

            return View(educationDetails);
        }

        // POST: Sales/Education/AddNote
        [HttpPost]
        public ActionResult AddNote(EducationNoteHelper model)
        {
            var relatedEducation = _db.Educations.First(o => o.Id == model.RelatedEducationId);

            relatedEducation.LastContactDate = DateTime.Now;
            relatedEducation.LastContactedBy = User.Identity.Name;

            _db.EducationNotes.Add(new EducationNote
            {
                RelatedEducationEntityId = model.RelatedEducationId,
                User = User.Identity.Name,
                Note = model.Note,
                InsertDate = DateTime.Now,
                Contact = model.Contact
            });
            _db.SaveChanges();

            if (model.IsActivity == false)
            {
                return RedirectToAction("Details", new { id = model.RelatedEducationId });
            }
            switch (model.Identifier)
            {
                case 1:
                    _helper.LogActivity(User.Identity.Name + " je obavio uspješan poziv vezan za prodajnu priliku: " + relatedEducation.EducationEntityTitle, User.Identity.Name, model.RelatedEducationId, ActivityTypeEnum.Succall, DepartmentEnum.Sales, ModuleEnum.Educations);
                    break;
                case 2:
                    _helper.LogActivity(User.Identity.Name + " je obavio kraći informativni poziv vezano za prodajnu priliku: " + relatedEducation.EducationEntityTitle, User.Identity.Name, model.RelatedEducationId, ActivityTypeEnum.Succalshort, DepartmentEnum.Sales, ModuleEnum.Educations);
                    break;
                case 3:
                    _helper.LogActivity(model.User + " je pokušao obaviti telefonski poziv vezano za prodajnu priliku: " + relatedEducation.EducationEntityTitle, User.Identity.Name, model.RelatedEducationId, ActivityTypeEnum.Unsuccal, DepartmentEnum.Sales, ModuleEnum.Educations);
                    break;
            }

            return RedirectToAction("Details", new { id = model.RelatedEducationId });
        }

        // POST: Sales/Education/EditNote
        [HttpPost]
        [Authorize]
        public ActionResult EditNote(EducationNoteHelper model)
        {
            var noteForEdit = _db.EducationNotes.First(n => n.Id == model.NoteId);

            noteForEdit.Note = model.Note;
            noteForEdit.Contact = model.Contact;
            noteForEdit.UpdateDate = DateTime.Now;
            _db.SaveChanges();

            return RedirectToAction("Details", new { id = model.RelatedEducationId });
        }

        // POST: Sales/Education/DeleteNote
        [HttpPost, ActionName("DeleteNote")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Superadmin")]
        public ActionResult DeleteNote(EducationNoteHelper model)
        {
            EducationNote educationNote = _db.EducationNotes.First(on => on.Id == model.NoteId);
            _db.EducationNotes.Remove(educationNote);
            _db.SaveChanges();
            return RedirectToAction("Details", new { id = model.RelatedEducationId });
        }

        public ActionResult Assign(EducationAssignHelper model)
        {
            var education = _db.Educations.First(o => o.Id == model.RelatedEducationId);
            education.IsAssigned = true;
            education.AssignedTo = model.AssignedTo;
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        public ActionResult Reassign(EducationAssignHelper model)
        {
            var education = _db.Educations.First(o => o.Id == model.RelatedEducationId);
            if (model.Unassign)
            {
                education.IsAssigned = false;
                education.AssignedTo = String.Empty;
                _db.SaveChanges();
            }
            else
            {
                education.AssignedTo = model.AssignedTo;
                _db.SaveChanges();
            }

            return Redirect(Request.UrlReferrer?.ToString());
        }

        public ActionResult ChangeStatus(EducationChangeStatusHelper model)
        {
            var education = _db.Educations.First(o => o.Id == model.RelatedEducationId);
            education.EducationEntityStatus = model.NewStatus;
            education.UpdateDate = DateTime.Now;
            education.LastUpdatedBy = User.Identity.Name;
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        public ActionResult ChangePriority(PriorityEnum newPriority, int relatedEducationId)
        {
            var education = _db.Educations.First(o => o.Id == relatedEducationId);
            education.Priority = newPriority;
            education.UpdateDate = DateTime.Now;
            education.LastUpdatedBy = User.Identity.Name;
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        public ActionResult MarkRejected(EducationMarkRejectedHelper model)
        {
            var education = _db.Educations.First(o => o.Id == model.RelatedEducationId);
            education.EducationEntityStatus = Education.EducationEntityStatusEnum.Rejected;
            education.EducationRejectReason = model.RejectReason;
            education.UpdateDate = DateTime.Now;
            education.LastUpdatedBy = User.Identity.Name;
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        public ActionResult AddAtendeeNumber(int number, int id)
        {
            var education = _db.Educations.First(o => o.Id == id);
            education.AtendeesNumber = number;
            education.UpdateDate = DateTime.Now;
            education.LastUpdatedBy = User.Identity.Name;
            _db.SaveChanges(); 
            return Redirect(Request.UrlReferrer?.ToString());
        }

        public void LogEmail(EducationNoteHelper model)
        {
            var relatedEducation = _db.Educations.First(o => o.Id == model.RelatedEducationId);

            relatedEducation.LastContactDate = DateTime.Now;
            relatedEducation.LastContactedBy = model.User;
            _helper.LogActivity(model.User + " je poslao e-mail na adresu: " + model.Email + " s pozivom na edukaciju u sklopu kampanje: " + relatedEducation.RelatedCampaign.CampaignName, User.Identity.Name, model.RelatedEducationId, ActivityTypeEnum.Email, DepartmentEnum.Sales, ModuleEnum.Educations);
        }

        [HttpPost]
        public ActionResult InsertEntities(HttpPostedFileBase file, int campaignId)
        {
            try
            {
                int importedEntities = 0;
                int validEntities = 0;
                List<string> validVats = new List<string>();
                int invalidEntities = 0;
                List<string> invalidVats = new List<string>();
                var campaign = _db.Campaigns.First(c => c.CampaignId == campaignId);

                var wb = new ExcelPackage(file.InputStream);
                var ws = wb.Workbook.Worksheets[1];

                for (int i = ws.Dimension.Start.Row; i <= ws.Dimension.End.Row; i++)
                {
                    object vat;

                    if ((vat = ws.Cells[i, 1].Value) != null)
                    {
                        string vatTemp = vat.ToString();

                        if (_db.Organizations.Any(o => (o.SubjectBusinessUnit == "" || o.SubjectBusinessUnit == "11"/*DHL hack/fix*/) && o.VAT == vatTemp))
                        {
                            var organization = _db.Organizations.First(o => (o.SubjectBusinessUnit == "" || o.SubjectBusinessUnit == "11"/*DHL hack/fix*/) && o.VAT == vatTemp);
                            validVats.Add(vatTemp);

                            _db.Educations.Add(new Education
                            {
                                EducationEntityTitle = organization.SubjectName,
                                RelatedCampaignId = campaignId,
                                RelatedOrganizationId = organization.MerId,
                                EducationEntityStatus = Education.EducationEntityStatusEnum.Created,
                                CreatedBy = User.Identity.Name,
                                IsAssigned = false,
                                InsertDate = DateTime.Now
                            });

                            importedEntities++;
                            _db.SaveChanges();

                            validEntities++;
                        }
                        else
                        {
                            invalidVats.Add(vatTemp);
                            invalidEntities++;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                var model = new EducationImportResults
                {
                    CampaignId = campaignId,
                    ValidEntities = validEntities,
                    InvalidEntities = invalidEntities,
                    ImportedEntities = importedEntities,
                    ValidVATs = validVats,
                    InvalidVATs = invalidVats
                };

                wb.Dispose();
                return View(model);
            }
            catch (COMException come)
            {
                _helper.LogError(@"Education - InsertEntities", "CampaignId: " + campaignId ,
                    @"Prilikom učitavanja predmeta za edukaciju javila se greška: " + come.Message, come.InnerException.Message, string.Empty, User.Identity.Name);

                var errorModel = new ErrorModelHelper()
                {
                    ErrorTitle = @"Greška u datoteci",
                    ErrorDescription = @"Prilikom učitavanja predmeta za edukaciju javila se greška: " + come.Message,
                    ErrorArguments = string.Empty,
                    ErrorException = come,
                    ErrorSuggestedSolution = @"Molimo pokušajte učitati datoteku u .xlsx formatu!"
                };

                return View("ErrorNew", errorModel);
            }
        }

        public ActionResult AtendeesByEducation()
        {
            var list = new List<AtendeesByEducationStatHelper>();
            var results = _db.Campaigns.Where(x => x.CampaignType == Campaigns.Models.Campaign.CampaignTypeEnum.Education
            && x.CampaignStatus != Campaigns.Models.Campaign.CampaignStatusEnum.Completed);

            foreach (var res in results)
            {
                list.Add(new AtendeesByEducationStatHelper
                {
                    EducationName = res.CampaignName,
                    CampaignStartDate = res.CampaignStartDate,
                    CampaignPlannedEndDate = res.CampaignPlannedEndDate,
                    Atendees = _db.Educations.Where(x => x.RelatedCampaignId == res.CampaignId && x.AtendeesNumber != null).Sum(x => x.AtendeesNumber)
                });
            }

            return View(list);
        }
    }
}