﻿using MojCRM.Models;
using MojCRM.Helpers;
using MojCRM.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using MojCRM.Areas.HelpDesk.Helpers;

namespace MojCRM.Controllers
{
    public class OrganizationsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly AcquireEmailMethodHelpers _acquireEmailMethodHelpers = new AcquireEmailMethodHelpers();

        // GET: Organizations
        public ActionResult Index(OrganizationSearchHelper model)
        {
            var organizations = from o in _db.Organizations
                                where o.SubjectBusinessUnit == String.Empty || o.SubjectBusinessUnit == "11" //FIX for DHL International...
                                select o;
            int results = 0;

            if (!String.IsNullOrEmpty(model.VAT))
            {
                organizations = organizations.Where(o => o.VAT.Contains(model.VAT));
                results = organizations.Count();
            }
            if (!String.IsNullOrEmpty(model.SubjectName))
            {
                organizations = organizations.Where(o => o.SubjectName.Contains(model.SubjectName));
                results = organizations.Count();
            }
            if (!String.IsNullOrEmpty(model.MainCity))
            {
                organizations = organizations.Where(o => o.OrganizationDetail.MainCity.Contains(model.MainCity));
                results = organizations.Count();
            }
            if (!String.IsNullOrEmpty(model.IsActive))
            {
                if (model.IsActive == "0")
                {
                    organizations = organizations.Where(o => o.IsActive == false);
                    results = organizations.Count();
                }
                if (model.IsActive == "1")
                {
                    organizations = organizations.Where(o => o.IsActive);
                    results = organizations.Count();
                }
            }
            if (model.Group != null)
            {
                var tempGroup = (OrganizationGroupEnum)model.Group;
                organizations = organizations.Where(o => o.OrganizationDetail.OrganizationGroup == tempGroup);
                results = organizations.Count();
            }


            var returnModel = new OrganizationIndexViewModel
            {
                OrganizationList = organizations.OrderBy(o => o.MerId),
                ResultsCount = results
            };

            return View(returnModel);
        }

        // GET: Organizations/Details/5
        public ActionResult Details(int id)
        {
            var organization = _db.Organizations.Find(id);
            var model = new OrganizationDetailsViewModel()
            {
                Organization = organization,
                OrganizationDetails = _db.OrganizationDetails.First(od => od.MerId == id),
                MerDeliveryDetails = _db.MerDeliveryDetails.First(mdd => mdd.MerId == id),
                OrganizationBusinessUnits = _db.Organizations.Where(o => o.VAT == organization.VAT && o.SubjectBusinessUnit != ""),
                Contacts = _db.Contacts.Where(c => c.OrganizationId == id),
                CampaignsFor = _db.Campaigns.Where(c => c.RelatedCompanyId == id),
                AcquireEmails = _db.AcquireEmails.Where(a => a.RelatedOrganizationId == id),
                Opportunities = _db.Opportunities.Where(op => op.RelatedOrganizationId == id),
                OpportunitiesCount = _db.Opportunities.Count(op => op.RelatedOrganizationId == id),
                Leads = _db.Leads.Where(l => l.RelatedOrganizationId == id),
                LeadsCount = _db.Leads.Count(l => l.RelatedOrganizationId == id),
                TicketsAsReceiver = _db.DeliveryTicketModels.Where(t => t.ReceiverId == id).OrderByDescending(t => t.SentDate),
                TicketsAsReceiverCount = _db.DeliveryTicketModels.Where(t => t.ReceiverId == id).OrderByDescending(t => t.SentDate).Count(),
                TicketsAsSender = _db.DeliveryTicketModels.Where(t => t.SenderId == id).OrderByDescending(t => t.SentDate),
                TicketsAsSenderCount = _db.DeliveryTicketModels.Where(t => t.SenderId == id).OrderByDescending(t => t.SentDate).Count(),
                Attributes = _db.OrganizationAttributes.Where(a => a.OrganizationId == id).OrderBy(a => a.AttributeClass),
                Activities = _db.ActivityLogs.Where(al => al.Module == ActivityLog.ModuleEnum.Organizations && al.ReferenceId == id),
                ActivitiesCount = _db.ActivityLogs.Count(al => al.Module == ActivityLog.ModuleEnum.Organizations && al.ReferenceId == id)
            };

            return View(model);
        }

        // GET: Organizations/GetOrganizations
        public JsonResult GetOrganizations(Guid user)
        {
            var Credentials = new { MerUser = "", MerPass = "" };
            if (String.IsNullOrEmpty(user.ToString()))
            {
                Credentials = (from u in _db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            }
            else
            {
                Credentials = (from u in _db.Users
                               where u.Id == user.ToString()
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            }

            var ReferencedId = (from o in _db.Organizations
                               orderby o.MerId descending
                               select o.MerId).First();
            int CreatedCompanies = 0;
            var Response = new MerGetSubjektDataResponse()
            {
                Id = 238,
                Naziv = "Test Klising d.o.o."
            };
            ReferencedId++;
            try
            {
                while (Response != null)
                {
                    MerApiGetSubjekt Request = new MerApiGetSubjekt()
                    {
                        Id = Credentials.MerUser,
                        Pass = Credentials.MerPass,
                        Oib = "99999999927",
                        PJ = "",
                        SoftwareId = "MojCRM-001",
                        SubjektPJ = ReferencedId.ToString()
                    };

                    string MerRequest = JsonConvert.SerializeObject(Request);

                    using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                        Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                        var _Response = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                        _Response = _Response.Replace("[", "").Replace("]", "");
                        MerGetSubjektDataResponse Result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(_Response);
                        if (Result == null)
                        {
                            break;
                        }
                        else
                        {
                            _db.Organizations.Add(new Organizations
                            {
                                MerId = Result.Id,
                                SubjectName = Result.Naziv,
                                SubjectBusinessUnit = Result.PoslovnaJedinica,
                                VAT = Result.Oib,
                                FirstReceived = Result.FirstReceived,
                                FirstSent = Result.FirstSent,
                                ServiceProvider = (Organizations.ServiceProviderEnum)Result.ServiceProviderId,
                                InsertDate = DateTime.Now
                            });
                            _db.MerDeliveryDetails.Add(new MerDeliveryDetails
                            {
                                MerId = Result.Id,
                                TotalSent = Result.TotalSent,
                                TotalReceived = Result.TotalReceived
                            });
                            _db.OrganizationDetails.Add(new OrganizationDetail
                            {
                                MerId = Result.Id,
                                MainAddress = Result.Adresa,
                                MainPostalCode = Int32.Parse(Result.Mjesto.Substring(0, 5).Trim()),
                                MainCity = Result.Mjesto.Substring(6).Trim(),
                                OrganizationGroup = OrganizationGroupEnum.Nema
                            });
                            _db.SaveChanges();
                            Result = Response;
                        }
                    }
                    ReferencedId++;
                    CreatedCompanies++;
                }
            }
            catch (NullReferenceException e)
            {
                _db.LogError.Add(new LogError
                {
                    Method = @"Organizations - GetOrganizations",
                    Parameters = ReferencedId.ToString(),
                    Message = @"Greška kod preuzimanja podataka o tvrtki",
                    InnerException = e.InnerException.ToString(),
                    Request = e.Source,
                    User = User.Identity.Name,
                    InsertDate = DateTime.Now
                });
                _db.SaveChanges();
            }

            return Json(new { Status = "OK", CreatedCompanies = CreatedCompanies }, JsonRequestBehavior.AllowGet);
        }

        // GET: Organization/UpdateOrganization/1
        public ActionResult UpdateOrganization(int MerId)
        {
            var Credentials = (from u in _db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            var Organization = _db.Organizations.Find(MerId);

            using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
            {
                MerApiGetSubjekt Request = new MerApiGetSubjekt()
                {
                    Id = Credentials.MerUser,
                    Pass = Credentials.MerPass,
                    Oib = "99999999927",
                    PJ = "",
                    SoftwareId = "MojCRM-001",
                    SubjektPJ = Organization.MerId.ToString()
                };

                string MerRequest = JsonConvert.SerializeObject(Request);

                Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var _Response = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                _Response = _Response.Replace("[", "").Replace("]", "");
                MerGetSubjektDataResponse Result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(_Response);

                string postalCode = Result.Mjesto.Substring(0, 5).Trim();
                string mainCity = Result.Mjesto.Substring(6).Trim();

                Organization.SubjectName = Result.Naziv;
                Organization.FirstReceived = Result.FirstReceived;
                Organization.FirstSent = Result.FirstSent;
                Organization.ServiceProvider = (Organizations.ServiceProviderEnum)Result.ServiceProviderId;
                Organization.UpdateDate = DateTime.Now;
                Organization.LastUpdatedBy = User.Identity.Name;
                Organization.MerUpdateDate = DateTime.Now;
                Organization.OrganizationDetail.MainAddress = Result.Adresa;
                Organization.OrganizationDetail.MainPostalCode = Int32.Parse(postalCode);
                Organization.OrganizationDetail.MainCity = mainCity;
                Organization.MerDeliveryDetail.TotalSent = Result.TotalSent;
                Organization.MerDeliveryDetail.TotalReceived = Result.TotalReceived;
            }
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // GET: Organizations/UpdateOrganizations
        public void UpdateOrganizations()
        {
            var Credentials = (from u in _db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            var Organizations = from o in _db.Organizations
                                 select o;


                foreach (var Organization in Organizations)
                {
                MerApiGetSubjekt Request = new MerApiGetSubjekt()
                    {
                        Id = Credentials.MerUser,
                        Pass = Credentials.MerPass,
                        Oib = "99999999927",
                        PJ = "",
                        SoftwareId = "MojCRM-001",
                        SubjektPJ = Organization.MerId.ToString()
                    };

                    string MerRequest = JsonConvert.SerializeObject(Request);

                    using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                    Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                    var _Response = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                    _Response = _Response.Replace("[", "").Replace("]", "");
                    MerGetSubjektDataResponse Result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(_Response);

                    string postalCode = Result.Mjesto.Substring(0, 5).Trim();
                    string mainCity = Result.Mjesto.Substring(6).Trim();

                    Organization.SubjectName = Result.Naziv;
                    Organization.FirstReceived = Result.FirstReceived;
                    Organization.FirstSent = Result.FirstSent;
                    Organization.ServiceProvider = (Organizations.ServiceProviderEnum)Result.ServiceProviderId;
                    Organization.UpdateDate = DateTime.Now;
                    Organization.LastUpdatedBy = User.Identity.Name;
                    Organization.MerUpdateDate = DateTime.Now;
                    Organization.OrganizationDetail.MainAddress = Result.Adresa;
                    Organization.OrganizationDetail.MainPostalCode = Int32.Parse(postalCode);
                    Organization.OrganizationDetail.MainCity = mainCity;
                    Organization.MerDeliveryDetail.TotalSent = Result.TotalSent;
                    Organization.MerDeliveryDetail.TotalReceived = Result.TotalReceived;
                }
            }
            _db.SaveChanges();
        }

        // POST: Organizations/EditImportantComment
        [HttpPost]
        public ActionResult EditImportantComment(string Comment, int ReceiverId)
        {
            var DetailForEdit = (from dd in _db.MerDeliveryDetails
                                 where dd.MerId == ReceiverId
                                 select dd).First();

            DetailForEdit.ImportantComments = Comment;
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // POST: Organizations/EditOrganizationDetails
        public ActionResult EditOrganizationDetails(EditOrganizationDetails Model)
        {
            var organization = _db.OrganizationDetails.Find(Model.MerId);
            string LogString = "Agent " + User.Identity.Name + " je napravio izmjene na subjektu: "
                + organization.Organization.SubjectName + ". Izmjenjeni su:";

            if (!String.IsNullOrEmpty(Model.TelephoneNumber))
            {
                if (!String.Equals(Model.TelephoneNumber, organization.TelephoneNumber))
                    LogString += " - broj telefona iz " + organization.TelephoneNumber + " u " + Model.TelephoneNumber;
                organization.TelephoneNumber = Model.TelephoneNumber;
            }
            if (!String.IsNullOrEmpty(Model.MobilePhoneNumber))
            {
                if (!String.Equals(Model.MobilePhoneNumber, organization.MobilePhoneNumber))
                    LogString += " - broj mobitela iz " + organization.MobilePhoneNumber + " u " + Model.MobilePhoneNumber;
                organization.MobilePhoneNumber = Model.MobilePhoneNumber;
            }
            if (!String.IsNullOrEmpty(Model.EmailAddress))
            {
                if (!String.Equals(Model.EmailAddress, organization.EmailAddress))
                    LogString += " - e-mail adresa iz " + organization.EmailAddress + " u " + Model.EmailAddress;
                organization.EmailAddress = Model.EmailAddress;
            }
            if (!String.IsNullOrEmpty(Model.ERP))
            {
                if (!String.Equals(Model.ERP, organization.ERP))
                    LogString += " - ERP iz " + organization.ERP + " u " + Model.ERP;
                organization.ERP = Model.ERP;
            }
            if (!String.IsNullOrEmpty(Model.NumberOfInvoicesSent))
            {
                if (!String.Equals(Model.NumberOfInvoicesSent, organization.NumberOfInvoicesSent))
                    LogString += " - broj IRA iz " + organization.NumberOfInvoicesSent + " u " + Model.NumberOfInvoicesSent;
                organization.NumberOfInvoicesSent = Model.NumberOfInvoicesSent;
            }
            if (!String.IsNullOrEmpty(Model.NumberOfInvoicesReceived))
            {
                if (!String.Equals(Model.NumberOfInvoicesReceived, organization.NumberOfInvoicesReceived))
                    LogString += " - broj URA iz " + organization.NumberOfInvoicesReceived + " u " + Model.NumberOfInvoicesReceived;
                organization.NumberOfInvoicesReceived = Model.NumberOfInvoicesReceived;
            }
            if (!String.IsNullOrEmpty(Model.CorrespondenceAddress))
            {
                if (!String.Equals(Model.CorrespondenceAddress, organization.CorrespondenceAddress))
                    LogString += " - adresa za dostavu iz " + organization.CorrespondenceAddress + " u " + Model.CorrespondenceAddress;
                organization.CorrespondenceAddress = Model.CorrespondenceAddress;
            }
            if (!String.IsNullOrEmpty(Model.CorrespondenceCity))
            {
                if (!String.Equals(Model.CorrespondenceCity, organization.CorrespondenceCity))
                    LogString += " - grad/mjesto za dostavu iz " + organization.CorrespondenceCity + " u " + Model.CorrespondenceCity;
                organization.CorrespondenceCity = Model.CorrespondenceCity;
            }
            if (Model.CorrespondencePostalCode != 0)
            {
                if (Model.CorrespondencePostalCode != organization.CorrespondencePostalCode)
                    LogString += " - poštanski broj iz " + organization.CorrespondencePostalCode + " u " + Model.CorrespondencePostalCode;
                organization.CorrespondencePostalCode = Model.CorrespondencePostalCode;
            }
            LogString += ".";
            organization.Organization.UpdateDate = DateTime.Now;
            organization.Organization.LastUpdatedBy = User.Identity.Name;

            LogActivity(LogString, User.Identity.Name, organization.MerId, ActivityLog.ActivityTypeEnum.Organizationupdate);

            _db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // POST: Organizations/EditAcquiredReceivingInformation
        public ActionResult EditAcquiredReceivingInformation(EditAcquiredReceivingInformationHelper model)
        {
            var organization = _db.MerDeliveryDetails.Find(model.MerId);
            string LogString = "Agent " + User.Identity.Name + " je izmjenio informaciju za preuzimanju na subjektu: "
                + organization.Organization.SubjectName + ". Izmjenjeno je:";

            if (!String.IsNullOrEmpty(model.NewAcquiredReceivingInformation))
            {
                if (!String.Equals(model.NewAcquiredReceivingInformation, organization.AcquiredReceivingInformation))
                    LogString += " stara informacija o preuzimanju: " + organization.AcquiredReceivingInformation + ", nova informacija o preuzimanju " + model.NewAcquiredReceivingInformation;
                organization.AcquiredReceivingInformation = model.NewAcquiredReceivingInformation;
            }

            LogString += ".";
            organization.Organization.UpdateDate = DateTime.Now;
            organization.Organization.LastUpdatedBy = User.Identity.Name;

            LogActivity(LogString, User.Identity.Name, organization.MerId, ActivityLog.ActivityTypeEnum.Organizationupdate);

            _db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // POST: Organizations/EditImportantOrganizationInfo
        [Authorize(Roles = "Superadmin")]
        public ActionResult EditImportantOrganizationInfo(EditImportantOrganizationInfo model)
        {
            var organization = _db.Organizations.Find(model.MerId);

            string logString = "Agent " + User.Identity.Name + " je napravio izmjene na subjektu: "
                + organization.SubjectName + ". Izmjenjeni su:";

            if (model.LegalForm != null)
            {
                if (model.LegalForm != organization.LegalForm)
                    logString += " - pravni oblik iz " + organization.LegalForm + " u " + model.LegalForm;
                organization.LegalForm = (Organizations.LegalFormEnum)model.LegalForm;
            }
            if (model.OrganizationGroup != null)
            {
                if (model.OrganizationGroup != organization.OrganizationDetail.OrganizationGroup)
                    logString += " - članstvo u grupaciji iz " + organization.OrganizationDetail.OrganizationGroup + " u " + model.OrganizationGroup;
                organization.OrganizationDetail.OrganizationGroup = (OrganizationGroupEnum)model.OrganizationGroup;
            }
            if (model.ServiceProvider != null)
            {
                if (model.ServiceProvider != organization.ServiceProvider)
                    logString += " - informacijski posrednik iz " + organization.ServiceProvider + " u " + model.ServiceProvider;
                organization.ServiceProvider = (Organizations.ServiceProviderEnum)model.ServiceProvider;
            }
            if (model.LegalStatus != null)
            {
                if (model.LegalStatus == 0)
                {
                    const bool temp = false;
                    if (temp != organization.IsActive)
                        logString += " - pravni status iz aktivnog u brisano";
                    organization.IsActive = false;
                    organization.MerDeliveryDetail.AcquiredReceivingInformation = "ZATVORENA TVRTKA";
                    organization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified = true;
                    _acquireEmailMethodHelpers.UpdateClosedSubjectEntities(model.MerId);
                }
                if (model.LegalStatus == 1)
                {
                    const bool temp = true;
                    if (temp != organization.IsActive)
                        logString += " - pravni status iz brisanog u aktivno";
                    organization.IsActive = true;
                }
            }
            logString += ".";
            organization.UpdateDate = DateTime.Now;
            organization.LastUpdatedBy = User.Identity.Name;

            LogActivity(logString, User.Identity.Name, organization.MerId, ActivityLog.ActivityTypeEnum.Organizationupdate);

            _db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // GET: Organizations/AddAttribute
        public ActionResult AddAttribute()
        {
            var model = new AddOrganizationAttributeViewModel();
            return View(model);
        }

        // POST: Organizations/AddAttribute
        [HttpPost]
        [Authorize(Roles = "Superadmin")]
        public ActionResult AddAttribute(AddOrganizationAttribute Model)
        {
            _db.OrganizationAttributes.Add(new OrganizationAttribute
            {
                OrganizationId = Model.MerId,
                AttributeClass = Model.AttributeClass,
                AttributeType = Model.AttributeType,
                IsActive = true,
                AssignedBy = User.Identity.Name,
                InsertDate = DateTime.Now
            });
            _db.SaveChanges();
            return RedirectToAction("Details", new { id = Model.MerId });
        }

        // POST: Organizations/CopyMainAddress
        [HttpPost]
        public JsonResult CopyMainAddress(EditOrganizationDetails Model)
        {
            var organization = _db.OrganizationDetails.Find(Model.MerId);

            organization.CorrespondenceAddress = Model.MainAddress;
            organization.CorrespondencePostalCode = Model.MainPostalCode;
            organization.CorrespondenceCity = Model.MainCity;
            organization.CorrespondenceCountry = Model.MainCountry;
            organization.Organization.UpdateDate = DateTime.Now;
            organization.Organization.LastUpdatedBy = User.Identity.Name;
            _db.SaveChanges();

            return Json(new { Status = "OK" });
        }

        [HttpPost]
        public JsonResult MarkAsVerified(int merId)
        {
            var organization = _db.MerDeliveryDetails.Find(merId);
            organization.AcquiredReceivingInformationIsVerified = true;
            organization.Organization.LastUpdatedBy = User.Identity.Name;
            organization.Organization.UpdateDate = DateTime.Now;
            _db.SaveChanges();

            return Json(new {Status = "OK"});
        }

        [HttpPost]
        public JsonResult MarkAsPostalService(int merId, bool unmark = false)
        {
            var organization = _db.MerDeliveryDetails.Find(merId);

            organization.RequiredPostalService = !unmark;
            organization.AcquiredReceivingInformation = !unmark ? "ŽELE POŠTOM" : String.Empty;
            organization.Organization.LastUpdatedBy = User.Identity.Name;
            organization.Organization.UpdateDate = DateTime.Now;
            _db.SaveChanges();

            return Json(new { Status = "OK" });
        }

        public void LogActivity(string ActivityDescription, string User, int ActivityReferenceId, ActivityLog.ActivityTypeEnum ActivityType)
        {
            _db.ActivityLogs.Add(new ActivityLog
            {
                Description = ActivityDescription,
                User = User,
                ReferenceId = ActivityReferenceId,
                ActivityType = ActivityType,
                Department = ActivityLog.DepartmentEnum.MojCrm,
                Module = ActivityLog.ModuleEnum.Organizations,
                InsertDate = DateTime.Now
            });
            _db.SaveChanges();
        }

        public JsonResult AutocompleteOrganization(string query)
        {
            try
            {
                var organizations = _db.Organizations.Where(o =>
                o.SubjectName.StartsWith(query) ||
                o.VAT.StartsWith(query))
                .Select(o => new { Organization = o.SubjectName + " - " + o.VAT, MerId = o.MerId})
                .Take(10)
                .ToList();

                //var _model = new List<OrganizationSearch>();
                //foreach (var org in organizations)
                //    _model.Add(new OrganizationSearch
                //    {
                //        MerId = org.MerId,
                //        OrganizationName = org.SubjectName + " - " + org.VAT
                //    });
                //var model = JsonConvert.SerializeObject(_model);
                return Json(organizations, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _db.LogError.Add(new LogError
                {
                    Method = @"Organizations - AutocompleteOrganization",
                    Parameters = query,
                    Message = @"Dogodila se greška prilikom pretraživanja tvrtki. Opis: " + ex.Message,
                    User = User.Identity.Name,
                    InsertDate = DateTime.Now
                });
                _db.SaveChanges();
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}