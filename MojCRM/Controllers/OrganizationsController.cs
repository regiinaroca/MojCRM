using MojCRM.Models;
using MojCRM.Helpers;
using MojCRM.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using MojCRM.Areas.HelpDesk.Helpers;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Areas.Sales.Helpers;
using OfficeOpenXml;
using System.Web;

namespace MojCRM.Controllers
{
    public class OrganizationsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly AcquireEmailMethodHelpers _acquireEmailMethodHelpers = new AcquireEmailMethodHelpers();
        private readonly OpportunityHelperMethods _opportunityHelperMethods = new OpportunityHelperMethods();
        private readonly HelperMethods _helper = new HelperMethods();

        // GET: Organizations
        [Authorize]
        public ActionResult Index(OrganizationSearchHelper model)
        {
            var organizations = from o in _db.Organizations
                                where o.SubjectBusinessUnit == String.Empty || o.SubjectBusinessUnit == "11" //FIX for DHL International...
                                select o;
            int results = 0;

            // Search Engine
            if (!String.IsNullOrEmpty(model.VAT))
            {
                organizations = organizations.Where(o => o.VAT.StartsWith(model.VAT));
                results = organizations.Count();
            }
            if (!String.IsNullOrEmpty(model.SubjectName))
            {
                organizations = organizations.Where(o => o.SubjectName.StartsWith(model.SubjectName));
                results = organizations.Count();
            }
            if (!String.IsNullOrEmpty(model.MainCity))
            {
                organizations = organizations.Where(o => o.OrganizationDetail.MainCity.StartsWith(model.MainCity));
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
        [Authorize]
        public ActionResult Details(int id)
        {
            try
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
                    Educations = _db.Educations.Where(e => e.RelatedOrganizationId == id),
                    Opportunities = _db.Opportunities.Where(op => op.RelatedOrganizationId == id),
                    OpportunitiesCount = _db.Opportunities.Count(op => op.RelatedOrganizationId == id),
                    Leads = _db.Leads.Where(l => l.RelatedOrganizationId == id),
                    LeadsCount = _db.Leads.Count(l => l.RelatedOrganizationId == id),
                    Quotes = _db.Quotes.Where(q => q.RelatedOrganizationId == id),
                    Contracts = _db.Contracts.Where(c => c.RelatedOrganizationId == id).OrderByDescending(c => c.MerId),
                    TicketsAsReceiver = _db.DeliveryTicketModels.Where(t => t.ReceiverId == id).OrderByDescending(t => t.SentDate),
                    TicketsAsReceiverCount = _db.DeliveryTicketModels.Where(t => t.ReceiverId == id).OrderByDescending(t => t.SentDate).Count(),
                    TicketsAsSender = _db.DeliveryTicketModels.Where(t => t.SenderId == id).OrderByDescending(t => t.SentDate),
                    TicketsAsSenderCount = _db.DeliveryTicketModels.Where(t => t.SenderId == id).OrderByDescending(t => t.SentDate).Count(),
                    Attributes = _db.OrganizationAttributes.Where(a => a.OrganizationId == id).OrderBy(a => a.AttributeClass),
                    Activities = _db.ActivityLogs.Where(al => al.Module == ActivityLog.ModuleEnum.Organizations && al.ReferenceId == id).OrderByDescending(al => al.InsertDate),
                    ActivitiesCount = _db.ActivityLogs.Count(al => al.Module == ActivityLog.ModuleEnum.Organizations && al.ReferenceId == id)
                };

                return View(model);
            }
            catch (FormatException)
            {
                return View("ErrorWrongInputFormat");
            }
        }

        /// <summary>
        /// Method which gets and inserts new organizations from Moj-eRačun API
        /// </summary>
        /// <param name="user">System user</param>
        /// <returns></returns>
        // GET: Organizations/GetOrganizations
        public JsonResult GetOrganizations(Guid? user)
        {
            var credentials = new { MerUser = "", MerPass = "" };
            if (String.IsNullOrEmpty(user.ToString()))
            {
                credentials = (from u in _db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            }
            else
            {
                credentials = (from u in _db.Users
                               where u.Id == user.ToString()
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            }

            var referencedId = (from o in _db.Organizations
                               orderby o.MerId descending
                               select o.MerId).First();
            int createdCompanies = 0;
            var response = new MerGetSubjektDataResponse()
            {
                Id = 238,
                Naziv = "Test Klising d.o.o."
            };
            referencedId++;
            try
            {
                while (response != null)
                {
                    MerApiGetSubjekt request = new MerApiGetSubjekt()
                    {
                        Id = credentials.MerUser,
                        Pass = credentials.MerPass,
                        Oib = "99999999927",
                        PJ = "",
                        SoftwareId = "MojCRM-001",
                        SubjektPJ = referencedId.ToString()
                    };

                    string merRequest = JsonConvert.SerializeObject(request);

                    using (var mer = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                        mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                        var apiResponse = mer.UploadString(new Uri(App.MerEndpoint + @"apis/v21/getSubjektData").ToString(), "POST", merRequest);
                        apiResponse = apiResponse.Replace("[", "").Replace("]", "");
                        MerGetSubjektDataResponse result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(apiResponse);
                        if (result == null)
                        {
                            break;
                        }
                        else
                        {
                            _db.Organizations.Add(new Organizations
                            {
                                MerId = result.Id,
                                SubjectName = result.Naziv,
                                SubjectBusinessUnit = result.PoslovnaJedinica,
                                VAT = result.Oib,
                                FirstReceived = result.FirstReceived,
                                FirstSent = result.FirstSent,
                                ServiceProvider = (Organizations.ServiceProviderEnum)result.ServiceProviderId,
                                InsertDate = DateTime.Now
                            });
                            _db.MerDeliveryDetails.Add(new MerDeliveryDetails
                            {
                                MerId = result.Id,
                                TotalSent = result.TotalSent,
                                TotalReceived = result.TotalReceived,
                                AcquiredReceivingInformation = string.Empty
                            });
                            _db.OrganizationDetails.Add(new OrganizationDetail
                            {
                                MerId = result.Id,
                                MainAddress = result.Adresa,
                                MainPostalCode = Int32.Parse(result.Mjesto.Substring(0, 5).Trim()),
                                MainCity = result.Mjesto.Substring(6).Trim(),
                                OrganizationGroup = OrganizationGroupEnum.Nema
                            });
                            _db.SaveChanges();
                        }
                    }
                    referencedId++;
                    createdCompanies++;
                }
            }
            catch (NullReferenceException e)
            {
                _db.LogError.Add(new LogError
                {
                    Method = @"Organizations - GetOrganizations",
                    Parameters = referencedId.ToString(),
                    Message = @"Greška kod preuzimanja podataka o tvrtki",
                    InnerException = e.InnerException?.ToString(),
                    Request = e.Source,
                    User = User.Identity.Name,
                    InsertDate = DateTime.Now
                });
                _db.SaveChanges();
            }

            return Json(new { Status = "OK", CreatedCompanies = createdCompanies }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method which uses Moj-eRačun API to update data for one Organization
        /// </summary>
        /// <param name="merId">Organization ID</param>
        /// <returns></returns>
        // GET: Organization/UpdateOrganization/1
        public ActionResult UpdateOrganization(int merId)
        {
            var credentials = (from u in _db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            var organization = _db.Organizations.First(o => o.MerId == merId);

            using (var mer = new WebClient() { Encoding = Encoding.UTF8 })
            {
                MerApiGetSubjekt request = new MerApiGetSubjekt()
                {
                    Id = credentials.MerUser,
                    Pass = credentials.MerPass,
                    Oib = "99999999927",
                    PJ = "",
                    SoftwareId = "MojCRM-001",
                    SubjektPJ = organization.MerId.ToString()
                };

                string merRequest = JsonConvert.SerializeObject(request);

                mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var response = mer.UploadString(new Uri(App.MerEndpoint + @"apis/v21/getSubjektData").ToString(), "POST", merRequest);
                response = response.Replace("[", "").Replace("]", "");
                MerGetSubjektDataResponse result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(response);

                string postalCode;
                string mainCity;

                if (organization.OrganizationDetail.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.Hr
                    || organization.OrganizationDetail.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.Rs)
                {
                    postalCode = result.Mjesto.Substring(0, 5).Trim();
                    mainCity = result.Mjesto.Substring(6).Trim();
                }
                else
                {
                    postalCode = "00000";
                    mainCity = result.Mjesto;
                }

                organization.SubjectName = result.Naziv;
                organization.FirstReceived = result.FirstReceived;
                organization.FirstSent = result.FirstSent;
                organization.ServiceProvider = (Organizations.ServiceProviderEnum)result.ServiceProviderId;
                organization.UpdateDate = DateTime.Now;
                organization.LastUpdatedBy = User.Identity.Name;
                organization.MerUpdateDate = DateTime.Now;
                organization.OrganizationDetail.MainAddress = result.Adresa;
                organization.OrganizationDetail.MainPostalCode = Int32.Parse(postalCode);
                organization.OrganizationDetail.MainCity = mainCity;
                organization.MerDeliveryDetail.TotalSent = result.TotalSent;
                organization.MerDeliveryDetail.TotalReceived = result.TotalReceived;

                _db.ActivityLogs.Add(new ActivityLog()
                {
                    ActivityType = ActivityLog.ActivityTypeEnum.Organizationupdate,
                    Department = ActivityLog.DepartmentEnum.MojCrm,
                    InsertDate = DateTime.Now,
                    IsSuspiciousActivity = false,
                    Module = ActivityLog.ModuleEnum.Organizations,
                    ReferenceId = merId,
                    User = User.Identity.Name,
                    Description = @"Korisnik " + User.Identity.Name + " je izvršio sinkronizaciju podataka tvrtke s Moj-eRačuna. Stari podaci su: Naziv tvrtke: "
                    + organization.SubjectName + ", Adresa: " + organization.OrganizationDetail.MainAddress + ", Poštanski broj: " + organization.OrganizationDetail.MainPostalCode
                    + ", Grad: " + organization.OrganizationDetail.MainCity + "."
                });
            }
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        /// <summary>
        /// Method which uses Moj-eRačun API to update data for all Organizations
        /// </summary>
        // GET: Organizations/UpdateOrganizations
        public void UpdateOrganizations()
        {
            var credentials = (from u in _db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            var organizations = from o in _db.Organizations
                                 select o;


                foreach (var organization in organizations)
                {
                MerApiGetSubjekt request = new MerApiGetSubjekt()
                    {
                        Id = credentials.MerUser,
                        Pass = credentials.MerPass,
                        Oib = "99999999927",
                        PJ = "",
                        SoftwareId = "MojCRM-001",
                        SubjektPJ = organization.MerId.ToString()
                    };

                    string merRequest = JsonConvert.SerializeObject(request);

                    using (var mer = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                    mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                    var apiResponse = mer.UploadString(new Uri(App.MerEndpoint + @"apis/v21/getSubjektData").ToString(), "POST", merRequest);
                    apiResponse = apiResponse.Replace("[", "").Replace("]", "");
                    MerGetSubjektDataResponse result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(apiResponse);

                    string postalCode = result.Mjesto.Substring(0, 5).Trim();
                    string mainCity = result.Mjesto.Substring(6).Trim();

                    organization.SubjectName = result.Naziv;
                    organization.FirstReceived = result.FirstReceived;
                    organization.FirstSent = result.FirstSent;
                    organization.ServiceProvider = (Organizations.ServiceProviderEnum)result.ServiceProviderId;
                    organization.UpdateDate = DateTime.Now;
                    organization.LastUpdatedBy = User.Identity.Name;
                    organization.MerUpdateDate = DateTime.Now;
                    organization.OrganizationDetail.MainAddress = result.Adresa;
                    organization.OrganizationDetail.MainPostalCode = Int32.Parse(postalCode);
                    organization.OrganizationDetail.MainCity = mainCity;
                    organization.MerDeliveryDetail.TotalSent = result.TotalSent;
                    organization.MerDeliveryDetail.TotalReceived = result.TotalReceived;
                }
            }
            _db.SaveChanges();
        }

        // POST: Organizations/EditImportantComment
        [HttpPost]
        public ActionResult EditImportantComment(string comment, int receiverId)
        {
            var detailForEdit = _db.MerDeliveryDetails.First(dd => dd.MerId == receiverId);

            detailForEdit.ImportantComments = comment;
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        // POST: Organizations/EditOrganizationDetails
        public ActionResult EditOrganizationDetails(EditOrganizationDetails model)
        {
            var organization = _db.OrganizationDetails.First(o => o.MerId == model.MerId);
            string logString = "Agent " + User.Identity.Name + " je napravio izmjene na subjektu: "
                + organization.Organization.SubjectName + ". Izmjenjeni su:";

            if (!String.IsNullOrEmpty(model.TelephoneNumber))
            {
                if (!String.Equals(model.TelephoneNumber, organization.TelephoneNumber))
                    logString += " - broj telefona iz " + organization.TelephoneNumber + " u " + model.TelephoneNumber;
                organization.TelephoneNumber = model.TelephoneNumber;
            }
            if (!String.IsNullOrEmpty(model.MobilePhoneNumber))
            {
                if (!String.Equals(model.MobilePhoneNumber, organization.MobilePhoneNumber))
                    logString += " - broj mobitela iz " + organization.MobilePhoneNumber + " u " + model.MobilePhoneNumber;
                organization.MobilePhoneNumber = model.MobilePhoneNumber;
            }
            if (!String.IsNullOrEmpty(model.EmailAddress))
            {
                if (!String.Equals(model.EmailAddress, organization.EmailAddress))
                    logString += " - e-mail adresa iz " + organization.EmailAddress + " u " + model.EmailAddress;
                organization.EmailAddress = model.EmailAddress;
            }
            if (!String.IsNullOrEmpty(model.ERP))
            {
                if (!String.Equals(model.ERP, organization.ERP))
                    logString += " - ERP iz " + organization.ERP + " u " + model.ERP;
                organization.ERP = model.ERP;
            }
            if (!String.IsNullOrEmpty(model.NumberOfInvoicesSent))
            {
                if (!String.Equals(model.NumberOfInvoicesSent, organization.NumberOfInvoicesSent))
                    logString += " - broj IRA iz " + organization.NumberOfInvoicesSent + " u " + model.NumberOfInvoicesSent;
                organization.NumberOfInvoicesSent = model.NumberOfInvoicesSent;
            }
            if (!String.IsNullOrEmpty(model.NumberOfInvoicesReceived))
            {
                if (!String.Equals(model.NumberOfInvoicesReceived, organization.NumberOfInvoicesReceived))
                    logString += " - broj URA iz " + organization.NumberOfInvoicesReceived + " u " + model.NumberOfInvoicesReceived;
                organization.NumberOfInvoicesReceived = model.NumberOfInvoicesReceived;
            }
            if (!String.IsNullOrEmpty(model.CorrespondenceAddress))
            {
                if (!String.Equals(model.CorrespondenceAddress, organization.CorrespondenceAddress))
                    logString += " - adresa za dostavu iz " + organization.CorrespondenceAddress + " u " + model.CorrespondenceAddress;
                organization.CorrespondenceAddress = model.CorrespondenceAddress;
            }
            if (!String.IsNullOrEmpty(model.CorrespondenceCity))
            {
                if (!String.Equals(model.CorrespondenceCity, organization.CorrespondenceCity))
                    logString += " - grad/mjesto za dostavu iz " + organization.CorrespondenceCity + " u " + model.CorrespondenceCity;
                organization.CorrespondenceCity = model.CorrespondenceCity;
            }
            if (model.CorrespondencePostalCode != 0)
            {
                if (model.CorrespondencePostalCode != organization.CorrespondencePostalCode)
                    logString += " - poštanski broj iz " + organization.CorrespondencePostalCode + " u " + model.CorrespondencePostalCode;
                organization.CorrespondencePostalCode = model.CorrespondencePostalCode;
            }
            logString += ".";
            organization.Organization.UpdateDate = DateTime.Now;
            organization.Organization.LastUpdatedBy = User.Identity.Name;

            _helper.LogActivity(logString, User.Identity.Name, organization.MerId, ActivityLog.ActivityTypeEnum.Organizationupdate, ActivityLog.DepartmentEnum.MojCrm, ActivityLog.ModuleEnum.Organizations);

            _db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // POST: Organizations/EditAcquiredReceivingInformation
        public ActionResult EditAcquiredReceivingInformation(EditAcquiredReceivingInformationHelper model)
        {
            var organization = _db.MerDeliveryDetails.First(o => o.MerId == model.MerId);

            string logString = "Agent " + User.Identity.Name + " je izmjenio informaciju za preuzimanju na subjektu: "
                + organization.Organization.SubjectName + ". Izmjenjeno je:";
            string newInformation = String.Empty;

            if (!String.IsNullOrEmpty(model.NewAcquiredReceivingInformation1))
                newInformation = model.NewAcquiredReceivingInformation1;
            if (!String.IsNullOrEmpty(model.NewAcquiredReceivingInformation2))
                newInformation += ";" + model.NewAcquiredReceivingInformation2;
            if (!String.IsNullOrEmpty(model.NewAcquiredReceivingInformation3))
                newInformation += ";" + model.NewAcquiredReceivingInformation3;

            if (!String.Equals(newInformation, organization.AcquiredReceivingInformation))
                logString += " stara informacija o preuzimanju: " + organization.AcquiredReceivingInformation + ", nova informacija o preuzimanju " + newInformation + ".";

            if (model.AcquireEmailId != null)
            {
                var acquireEmail = _db.AcquireEmails.First(ae => ae.Id == model.AcquireEmailId);

                if (String.IsNullOrEmpty(organization.AcquiredReceivingInformation) && newInformation.Contains("@"))
                    acquireEmail.IsNewlyAcquired = true;
            }

            organization.AcquiredReceivingInformation = newInformation;
            organization.Organization.UpdateDate = DateTime.Now;
            organization.Organization.LastUpdatedBy = User.Identity.Name;

            _helper.LogActivity(logString, User.Identity.Name, organization.MerId, ActivityLog.ActivityTypeEnum.Organizationupdate, ActivityLog.DepartmentEnum.MojCrm, ActivityLog.ModuleEnum.Organizations);

            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        // POST: Organizations/EditImportantOrganizationInfo
        [Authorize(Roles = "Superadmin")]
        public ActionResult EditImportantOrganizationInfo(EditImportantOrganizationInfo model)
        {
            var organization = _db.Organizations.First(o => o.MerId == model.MerId);

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
                    _opportunityHelperMethods.UpdateClosedSubjectOpportunities(model.MerId);
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

            _helper.LogActivity(logString, User.Identity.Name, organization.MerId, ActivityLog.ActivityTypeEnum.Organizationupdate, ActivityLog.DepartmentEnum.MojCrm, ActivityLog.ModuleEnum.Organizations);

            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        // POST: Organizations/EditEmailAddressForVerification
        [HttpPost]
        public ActionResult EditEmailAddressForVerification(int merId, string emailForVerification)
        {
            var entity = _db.MerDeliveryDetails.First(e => e.MerId == merId);
            string logString = "Agent " + User.Identity.Name + " je napravio izmjene na subjektu: "
                + entity.Organization.SubjectName + ". Izmjenjena je email adresa za provjeru iz: " + entity.EmailAddressForVerification + ", u: " + emailForVerification + ".";

            entity.EmailAddressForVerification = emailForVerification;
            entity.Organization.UpdateDate = DateTime.Now;
            entity.Organization.LastUpdatedBy = User.Identity.Name;
            _db.SaveChanges();

            _helper.LogActivity(logString, User.Identity.Name, merId, ActivityLog.ActivityTypeEnum.Organizationupdate, ActivityLog.DepartmentEnum.MojCrm, ActivityLog.ModuleEnum.Organizations);

            return Redirect(Request.UrlReferrer?.ToString());
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
        public ActionResult AddAttribute(AddOrganizationAttribute model)
        {
            _db.OrganizationAttributes.Add(new OrganizationAttribute
            {
                OrganizationId = model.MerId,
                AttributeClass = model.AttributeClass,
                AttributeType = model.AttributeType,
                IsActive = true,
                AssignedBy = User.Identity.Name,
                InsertDate = DateTime.Now
            });
            _db.SaveChanges();
            return RedirectToAction("Details", new { id = model.MerId });
        }

        // POST: Organizations/CopyMainAddress
        [HttpPost]
        public JsonResult CopyMainAddress(EditOrganizationDetails model)
        {
            var organization = _db.OrganizationDetails.First(o => o.MerId == model.MerId);

            organization.CorrespondenceAddress = model.MainAddress;
            organization.CorrespondencePostalCode = model.MainPostalCode;
            organization.CorrespondenceCity = model.MainCity;
            organization.CorrespondenceCountry = model.MainCountry;
            organization.Organization.UpdateDate = DateTime.Now;
            organization.Organization.LastUpdatedBy = User.Identity.Name;
            _db.SaveChanges();

            return Json(new { Status = "OK" });
        }

        [HttpPost]
        public JsonResult MarkAsVerified(int merId, bool unmark = false)
        {
            var organization = _db.MerDeliveryDetails.First(o => o.MerId == merId);
            organization.AcquiredReceivingInformationIsVerified = !unmark;
            organization.Organization.LastUpdatedBy = User.Identity.Name;
            organization.Organization.UpdateDate = DateTime.Now;
            _db.SaveChanges();

            return Json(new {Status = "OK"});
        }

        [HttpPost]
        public JsonResult MarkAsPostalService(int merId, bool unmark = false)
        {
            var organization = _db.MerDeliveryDetails.First(x => x.MerId == merId);

            if (organization.Organization.AqcuireEmails.Any() && unmark == false)
            {
                var acquireEmailEntity = _db.AcquireEmails.First(x => x.RelatedOrganizationId == merId);
                _acquireEmailMethodHelpers.UpdateStatus(AcquireEmail.AcquireEmailStatusEnum.Verified, merId);
                _acquireEmailMethodHelpers.ApplyToAllEntities(AcquireEmail.AcquireEmailEntityStatusEnum.Post, acquireEmailEntity.Id);
            }

            organization.RequiredPostalService = !unmark;
            organization.AcquiredReceivingInformation = !unmark ? "ŽELE POŠTOM" : String.Empty;
            organization.Organization.LastUpdatedBy = User.Identity.Name;
            organization.Organization.UpdateDate = DateTime.Now;
            

            _db.SaveChanges();

            return Json(new { Status = "OK" });
        }

        /// <summary>
        /// Method which reads an Excel file and writes telephone numbers in Database
        /// Excel file should have two columns - VAT and Telephone number
        /// </summary>
        /// <param name="file">Excel file</param>
        /// <returns></returns>
        //GET: Organizations/ImportEmailsForVerification
        public JsonResult ImportEmailsForVerification(HttpPostedFileBase file)
        {
            int importedEmails = 0;
            int unimportedEmails = 0;

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
                        var organization = _db.Organizations.First(o => o.VAT == vatTemp && (o.SubjectBusinessUnit == "" || o.SubjectBusinessUnit == "11"/*DHL hack/fix*/));
                        var emailTemp = ws.Cells[i, 2].Value.ToString();
                        organization.MerDeliveryDetail.EmailAddressForVerification = emailTemp;
                        _db.SaveChanges();

                        importedEmails++;
                    }
                    else
                    {
                        unimportedEmails++;
                    }
                }
                else
                {
                    continue;
                }
            }

                wb.Dispose();
                return Json(new { ImportedEmails = importedEmails, UnimportedEmails = unimportedEmails });
        }

        public JsonResult AutocompleteOrganization(string query)
        {
            try
            {
                var organizations = _db.Organizations.Where(o =>
                o.SubjectName.StartsWith(query) ||
                o.VAT.StartsWith(query))
                .Select(o => new { Organization = o.SubjectName + " - " + o.VAT, o.MerId})
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