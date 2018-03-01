using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using MojCRM.Areas.Campaigns.Models;
using static MojCRM.Areas.HelpDesk.Models.AcquireEmail;
using MojCRM.Areas.HelpDesk.Helpers;
using MojCRM.Areas.HelpDesk.ViewModels;
using MojCRM.Helpers;
using OfficeOpenXml;

namespace MojCRM.Areas.HelpDesk.Controllers
{
    public class AcquireEmailController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly HelperMethods _helper = new HelperMethods();
        private readonly AcquireEmailMethodHelpers _acquireEmailMethodHelpers = new AcquireEmailMethodHelpers();
        // GET: HelpDesk/AcquireEmail
        [Authorize]
        public ActionResult Index(AcquireEmailSearchModel model)
        {
            IQueryable<AcquireEmail> list;
            if (User.IsInRole("Administrator") || User.IsInRole("Superadmin") || User.IsInRole("Management"))
            {
                list = _db.AcquireEmails;
            }
            else
            {
                //list = _db.AcquireEmails.Where(ae => ae.AcquireEmailStatus != AcquireEmailStatusEnum.Verified && ae.AssignedTo == User.Identity.Name);
                list = _db.AcquireEmails.Where(x => x.AcquireEmailStatus != AcquireEmailStatusEnum.Verified
                && x.AssignedTo == User.Identity.Name
                && (!(x.Organization.OrganizationDetail.TelephoneNumber == String.Empty || x.Organization.OrganizationDetail.TelephoneNumber == null)
                || !(x.Organization.OrganizationDetail.MobilePhoneNumber == String.Empty || x.Organization.OrganizationDetail.MobilePhoneNumber == null)));
            }

            //Search Engine
            if (!String.IsNullOrEmpty(model.CampaignName))
            {
                list = list.Where(x => x.Campaign.CampaignName.Contains(model.CampaignName));
            }
            if (!String.IsNullOrEmpty(model.OrganizationName))
            {
                list = list.Where(x => x.Organization.SubjectName.Contains(model.OrganizationName));
            }
            if (!String.IsNullOrEmpty(model.TelephoneMobile))
            {
                list = list.Where(x => x.Organization.OrganizationDetail.TelephoneNumber.EndsWith(model.TelephoneMobile) || x.Organization.OrganizationDetail.MobilePhoneNumber.EndsWith(model.TelephoneMobile));
            }
            if (!String.IsNullOrEmpty(model.Mail))
            {
                list = list.Where(x => x.Organization.MerDeliveryDetail.AcquiredReceivingInformation.Contains(model.Mail));
            }
            if (model.EmailStatusEnum != null)
            {
                var tempStatus = (AcquireEmailStatusEnum) model.EmailStatusEnum;
                list = list.Where(x => x.AcquireEmailStatus == tempStatus);
            }
            if (model.EntityStatusEnum != null)
            {
                var tempStatus = (AcquireEmailEntityStatusEnum)model.EntityStatusEnum;
                list = list.Where(x => x.AcquireEmailEntityStatus == tempStatus);
            }

            return View(list.OrderByDescending(x => x.Id));
        }

        [Authorize]
        public ActionResult IndexTemp(AcquireEmailSearchModel model)
        {
            var list = _db.AcquireEmails.Where(x =>
                           (x.Organization.OrganizationDetail.TelephoneNumber == String.Empty ||
                           x.Organization.OrganizationDetail.TelephoneNumber == null)
                       && (x.Organization.OrganizationDetail.MobilePhoneNumber == String.Empty ||
                           x.Organization.OrganizationDetail.MobilePhoneNumber == null)
                           && x.AcquireEmailStatus == AcquireEmailStatusEnum.Created);

            //Search Engine
            if (!String.IsNullOrEmpty(model.CampaignName))
            {
                list = list.Where(x => x.Campaign.CampaignName.Contains(model.CampaignName));
            }
            if (!String.IsNullOrEmpty(model.OrganizationName))
            {
                list = list.Where(x => x.Organization.SubjectName.Contains(model.OrganizationName));
            }
            if (!String.IsNullOrEmpty(model.TelephoneMobile))
            {
                //list = list.Where(x => x.Organization.MerDeliveryDetail.Telephone.Contains(model.TelephoneMail) || x.Organization.MerDeliveryDetail.AcquiredReceivingInformation.Contains(model.TelephoneMail));
                list = list.Where(x => x.Organization.OrganizationDetail.TelephoneNumber.EndsWith(model.TelephoneMobile) || x.Organization.OrganizationDetail.MobilePhoneNumber.EndsWith(model.TelephoneMobile));
            }
            if (!String.IsNullOrEmpty(model.Mail))
            {
                list = list.Where(x => x.Organization.MerDeliveryDetail.AcquiredReceivingInformation.Contains(model.Mail));
            }
            if (model.EmailStatusEnum != null)
            {
                var tempStatus = (AcquireEmailStatusEnum)model.EmailStatusEnum;
                list = list.Where(x => x.AcquireEmailStatus == tempStatus);
            }
            if (model.EntityStatusEnum != null)
            {
                var tempStatus = (AcquireEmailEntityStatusEnum)model.EntityStatusEnum;
                list = list.Where(x => x.AcquireEmailEntityStatus == tempStatus);
            }

            return View(list.OrderByDescending(x => x.Id));
        }

        public ActionResult Details(int id)
        {
            var entity = _db.AcquireEmails.Find(id);
            
            var activities = _db.ActivityLogs.Where(al =>
                al.Department == ActivityLog.DepartmentEnum.DatabaseUpdate && al.ReferenceId == id);

            var model = new AcquireEmailViewModel
            {
                Entity = entity,
                Activities = activities.OrderByDescending(x => x.Id)
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult LogActivity(int entityId, int identifier)
        {
            var entity = _db.AcquireEmails.First(x => x.Id == entityId);
            switch (identifier)
            {
                case 1:
                    _db.ActivityLogs.Add(new ActivityLog()
                    {
                        Description = "Agent " + User.Identity.Name + " je obavio uspješan poziv za ažuriranje baze korisnika.",
                        User = User.Identity.Name,
                        ReferenceId = entityId,
                        ActivityType = ActivityLog.ActivityTypeEnum.Succall,
                        Department = ActivityLog.DepartmentEnum.DatabaseUpdate,
                        Module = ActivityLog.ModuleEnum.AqcuireEmail,
                        InsertDate = DateTime.Now
                    });
                    entity.LastContactedBy = User.Identity.Name;
                    entity.LastContactDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 2:
                    _db.ActivityLogs.Add(new ActivityLog()
                    {
                        Description = "Agent " + User.Identity.Name + " je pokušao obaviti poziv za ažuriranje baze korisnika.",
                        User = User.Identity.Name,
                        ReferenceId = entityId,
                        ActivityType = ActivityLog.ActivityTypeEnum.Unsuccal,
                        Department = ActivityLog.DepartmentEnum.DatabaseUpdate,
                        Module = ActivityLog.ModuleEnum.AqcuireEmail,
                        InsertDate = DateTime.Now
                    });
                    entity.LastContactedBy = User.Identity.Name;
                    entity.LastContactDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
            }
            return Json(new { Status = "OK" });
        }


        [HttpPost]
        public JsonResult ChangeStatus(int entityId, int identifier)
        {
            var entity = _db.AcquireEmails.First(x => x.Id == entityId);
            switch (identifier)
            {
                case 1:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Checked;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 2:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Verified;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 3:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Reviewed;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
            }
            return Json(new { Status = "OK" });
        }

        [HttpPost]
        public ActionResult ChangeStatusAdmin(int entityId, int identifier)
        {
            var entity = _db.AcquireEmails.First(x => x.Id == entityId);
            switch (identifier)
            {
                case 0:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Created;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 1:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Checked;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 2:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Verified;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 3:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Reviewed;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
            }
            return Redirect(Request.UrlReferrer?.ToString());
        }

        [HttpPost]
        public ActionResult ChangeEntityStatus(int entityId, int identifier)
        {
            var entity = _db.AcquireEmails.First(x => x.Id == entityId);
            switch (identifier)
            {
                case 0:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.Created;
                    entity.UpdateDate = DateTime.Now;
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Kreirano", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
                case 1:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.AcquiredInformation;
                    entity.UpdateDate = DateTime.Now;
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Dobivena povratna informacija", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
                case 2:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.NoAnswer;
                    entity.UpdateDate = DateTime.Now;
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Nema odgovora / Ne javlja se", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
                case 3:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.ClosedOrganization;
                    entity.UpdateDate = DateTime.Now;
                    _acquireEmailMethodHelpers.ApplyToAllEntities(AcquireEmailEntityStatusEnum.ClosedOrganization, entityId);
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Zatvoren subjekt", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
                case 4:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.OldPartner;
                    entity.UpdateDate = DateTime.Now;
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Ne posluju s korisnikom", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
                case 5:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.PartnerWillContactUser;
                    entity.UpdateDate = DateTime.Now;
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Partner će se javiti korisniku samostalno", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
                case 6:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.WrittenConfirmationRequired;
                    entity.UpdateDate = DateTime.Now;
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Potrebno poslati pisanu suglasnost", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
                case 7:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.WrongTelephoneNumber;
                    entity.UpdateDate = DateTime.Now;
                    entity.Organization.MerDeliveryDetail.AcquiredReceivingInformation = "NEMA ISPRAVNOG BROJA TELEFONA";
                    entity.Organization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified = true;
                    _acquireEmailMethodHelpers.UpdateWrongTelephoneNumberEntities(entity.RelatedOrganizationId);
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Neispravan kontakt broj", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
                case 8:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.PoslovnaHrvatska;
                    entity.UpdateDate = DateTime.Now;
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Poslovna Hrvatska", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
                case 9:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.NoTelehoneNumber;
                    entity.UpdateDate = DateTime.Now;
                    entity.Organization.MerDeliveryDetail.AcquiredReceivingInformation = "NEMA ISPRAVNOG BROJA TELEFONA";
                    entity.Organization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified = true;
                    _acquireEmailMethodHelpers.UpdateWrongTelephoneNumberEntities(entity.RelatedOrganizationId);
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Ne postoji ispravan kontakt broj", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
                case 10:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.Bankruptcy;
                    entity.UpdateDate = DateTime.Now;
                    _acquireEmailMethodHelpers.ApplyToAllEntities(AcquireEmailEntityStatusEnum.Bankruptcy, entityId);
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Subjekt u stečaju / likvidaciji", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
                case 11:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.NoFinancialAccount;
                    entity.UpdateDate = DateTime.Now;
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Subjekt nema žiro račun", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
                case 12:
                    entity.AcquireEmailEntityStatus = AcquireEmailEntityStatusEnum.ToBeClosed;
                    entity.UpdateDate = DateTime.Now;
                    _helper.LogActivity("Promijenjen status obrade. Novi status: Najava brisanja subjekta", User.Identity.Name, entityId, ActivityLog.ActivityTypeEnum.AcquireEmailEntityStatusChange, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
                    _db.SaveChanges();
                    break;
            }
            return Redirect(Request.UrlReferrer?.ToString());
        }

        [HttpPost]
        public ActionResult CheckEntitiesForImport(HttpPostedFileBase file, int campaignId)
        {
            try
            {
                int importedEntities = 0;
                int validEntities = 0;
                List<string> validVats = new List<string>();
                int invalidEntities = 0;
                List<string> invalidVats = new List<string>();

                //string filepath = Path.Combine(Server.MapPath("~/ImportFiles"), "ImportAcquireEmail.xls");
                //if(!create)
                //    file.SaveAs(filepath);

                var wb = new ExcelPackage(file.InputStream);
                var ws = wb.Workbook.Worksheets[1];

                for (int i = ws.Dimension.Start.Row; i <= ws.Dimension.End.Row; i++)
                {
                    object vat;

                    if ((vat = ws.Cells[i, 1].Value) != null)
                    {
                        if (_db.Organizations.Any(o => (o.SubjectBusinessUnit == "" || o.SubjectBusinessUnit == "11"/*DHL hack/fix*/) && o.VAT == vat.ToString()))
                        {
                            validVats.Add(vat.ToString());
                            ImportEntities(campaignId, vat.ToString());
                            importedEntities++;

                            validEntities++;
                        }
                        else
                        {
                            invalidVats.Add(vat.ToString());
                            invalidEntities++;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                var model = new AcquireEmailCheckResults
                {
                    CampaignId = campaignId,
                    ValidEntities = validEntities,
                    InvalidEntities = invalidEntities,
                    ImportedEntities = importedEntities,
                    ValidVATs = validVats,
                    InvalidVATs = invalidVats
                };

                //if(create)
                //    System.IO.File.Delete(filepath);

                wb.Dispose();
                return View(model);
            }
            catch (COMException)
            {
                return View("ErrorOldExcel");
            }
        }

        [HttpPost]
        public void ImportEntities(int campaignId, string VAT)
        {
            if (VAT != "")
            {
                var relatedOrganization = _db.Organizations.First(o => (o.SubjectBusinessUnit == "" || o.SubjectBusinessUnit == "11"/*DHL hack/fix*/) && o.VAT == VAT);

                if (relatedOrganization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified)
                {
                    CreateEntity(relatedOrganization, AcquireEmailStatusEnum.Verified, campaignId);
                }
                else if (relatedOrganization.MerDeliveryDetail.RequiredPostalService)
                {
                    CreateEntity(relatedOrganization, AcquireEmailStatusEnum.Checked, campaignId);
                }
                else if (relatedOrganization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified && relatedOrganization.MerDeliveryDetail.RequiredPostalService)
                {
                    CreateEntity(relatedOrganization, AcquireEmailStatusEnum.Verified, campaignId);
                }
                else
                {
                    CreateEntity(relatedOrganization, AcquireEmailStatusEnum.Created, campaignId);
                }
            }
        }

        public ActionResult ExportEntities(int campaignId, int identifier)
        {
            #region OldWay
            //var gv = new GridView();
            //gv.DataSource = GetEntityList(CampaignId);
            //gv.DataBind();
            //Response.ClearContent();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment; filename=ExportedEntities.xls");
            //Response.ContentType = "application/ms-excel";
            //Response.Charset = "";
            //Response.ContentEncoding = new System.Text.UTF8Encoding();
            //StringWriter objStringWriter = new StringWriter();
            //HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            //gv.RenderControl(objHtmlTextWriter);
            //Response.Output.Write(objStringWriter.ToString());
            //Response.Flush();
            //Response.End();
            #endregion

            int cell = 2;
            IList<AcquireEmailExportModel> results = null;

            switch (identifier)
            {
                case 1:
                    results = GetEntityList(campaignId);
                    break;
                case 2:
                    results = GetReviewedEntityList(campaignId);
                    break;
            }
            var wb = new ExcelPackage();
            var ws = wb.Workbook.Worksheets.Add("Rezultati obrade baze");
            ws.Cells[1, 1].Value = "Naziv kampanje";
            ws.Cells[1, 2].Value = "OIB partnera";
            ws.Cells[1, 3].Value = "Naziv partnera";
            ws.Cells[1, 4].Value = "Informacija o zaprimanju eRačuna";
            ws.Cells[1, 5].Value = "Status obrade";

            foreach (var res in results)
            {
                ws.Cells[cell, 1].Value = res.CampaignName;
                ws.Cells[cell, 2].Value = res.VAT;
                ws.Cells[cell, 3].Value = res.SubjectName;
                ws.Cells[cell, 4].Value = res.AcquiredReceivingInformation;
                ws.Cells[cell, 5].Value = res.Entity.AcquireEmailEntityStatusString;
                cell++;
            }

            while (cell < 16)
            {
                ws.Cells[cell, 1].Value = "";
                cell++;
            }

            return File(wb.GetAsByteArray(), "application/vnd.ms-excel", "Rezultati.xlsx");
        }

        public ActionResult ExportEntitiesForEmailNotification(int campaignId)
        {
            int cell = 2;
            string campaignName = _db.Campaigns.First(c => c.CampaignId == campaignId).CampaignName;
            IList<AcquireEmailExportForEmailNotificationModel> results = GetEntityListForEmailNotification(campaignId);

            var wb = new ExcelPackage();
            var ws = wb.Workbook.Worksheets.Add("Rezultati obrade baze za tipsku");
            ws.Cells[1, 1].Value = "Informacija o zaprimanju eRačuna";

            foreach (var res in results)
            {
                ws.Cells[cell, 1].Value = res.AcquiredReceivingInformation;
                cell++;
            }

            while (cell < 16)
            {
                ws.Cells[cell, 1].Value = "";
                cell++;
            }

            return File(wb.GetAsByteArray(), "application/vnd.ms-excel", campaignName + ".xlsx");
        }

        public void CreateEntity(Organizations organization, AcquireEmailStatusEnum status, int campaignId)
        {
            AcquireEmailEntityStatusEnum entityStatusEnum = AcquireEmailEntityStatusEnum.Created;
            switch (status)
            {
                case AcquireEmailStatusEnum.Created:
                    entityStatusEnum = AcquireEmailEntityStatusEnum.Created;
                    break;
                case AcquireEmailStatusEnum.Checked:
                    entityStatusEnum = AcquireEmailEntityStatusEnum.Created;
                    break;
                case AcquireEmailStatusEnum.Verified:
                    //entityStatusEnum = organization.MerDeliveryDetail.AcquiredReceivingInformation == "ZATVORENA TVRTKA" ? AcquireEmailEntityStatusEnum.ClosedOrganization : AcquireEmailEntityStatusEnum.AcquiredInformation;
                    if (organization.MerDeliveryDetail.AcquiredReceivingInformation == "ZATVORENA TVRTKA")
                        entityStatusEnum = AcquireEmailEntityStatusEnum.ClosedOrganization;
                    else if (organization.MerDeliveryDetail.AcquiredReceivingInformation ==
                             "NEMA ISPRAVNOG BROJA TELEFONA")
                        entityStatusEnum = AcquireEmailEntityStatusEnum.WrongTelephoneNumber;
                    else
                        entityStatusEnum = AcquireEmailEntityStatusEnum.AcquiredInformation;
                    break;
            }

            _db.AcquireEmails.Add(new AcquireEmail
            {
                RelatedOrganizationId = organization.MerId,
                RelatedCampaignId = campaignId,
                AcquireEmailStatus = status,
                AcquireEmailEntityStatus = entityStatusEnum,
                InsertDate = DateTime.Now
            });
            _db.SaveChanges();
        }

        public JsonResult UpdateEntityStatus(int campaignId)
        {
            int updated = 0;

            var entities = _db.AcquireEmails.Where(ac => ac.RelatedCampaignId == campaignId);

            foreach (var entity in entities)
            {
                if (entity.Organization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified)
                {
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Verified;
                    entity.UpdateDate = DateTime.Now;
                    updated++;
                }
                else if (entity.Organization.MerDeliveryDetail.RequiredPostalService)
                {
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Checked;
                    entity.UpdateDate = DateTime.Now;
                    updated++;
                }
                else if (entity.Organization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified && entity.Organization.MerDeliveryDetail.RequiredPostalService)
                {
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Verified;
                    entity.UpdateDate = DateTime.Now;
                    updated++;
                }
                //else
                //{
                //    entity.AcquireEmailStatus = AcquireEmailStatusEnum.CREATED;
                //    entity.UpdateDate = DateTime.Now;
                //    updated++;
                //}
            }
            _db.SaveChanges();

            return Json(new { updated }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdminAssignEntities(int campaignId, int number, string agent)
        {
            var entites = _db.AcquireEmails.Where(c => c.RelatedCampaignId == campaignId 
            && c.AcquireEmailStatus == AcquireEmailStatusEnum.Created 
            && c.IsAssigned == false
            && (!(c.Organization.OrganizationDetail.TelephoneNumber == String.Empty || c.Organization.OrganizationDetail.TelephoneNumber == null)
            || !(c.Organization.OrganizationDetail.MobilePhoneNumber == String.Empty || c.Organization.OrganizationDetail.MobilePhoneNumber == null))).Take(number);

            foreach (var entity in entites)
            {
                entity.IsAssigned = true;
                entity.AssignedTo = agent;
                entity.UpdateDate = DateTime.Now;
                _helper.LogActivity("Predmet je dodijeljen agentu: " + agent + ". Dodijelio: " + User.Identity.Name, User.Identity.Name, entity.Id, ActivityLog.ActivityTypeEnum.AcquireEmailAssignement, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
            }

            var campaign = _db.Campaigns.First(c => c.CampaignId == campaignId);

            if (campaign.CampaignStatus == Campaign.CampaignStatusEnum.Start)
                campaign.CampaignStatus = Campaign.CampaignStatusEnum.InProgress;

            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        public ActionResult AdminUnassignEntities(int campaignId, string agent)
        {
            var entites = _db.AcquireEmails.Where(c => c.RelatedCampaignId == campaignId && c.AssignedTo == agent);

            foreach (var entity in entites)
            {
                entity.IsAssigned = false;
                entity.AssignedTo = string.Empty;
                entity.UpdateDate = DateTime.Now;
                _helper.LogActivity("Uklonjena dodjela agentu: " + agent + ". Dodjelu uklonio: " + User.Identity.Name, User.Identity.Name, entity.Id, ActivityLog.ActivityTypeEnum.AcquireEmailAssignement, ActivityLog.DepartmentEnum.DatabaseUpdate, ActivityLog.ModuleEnum.AqcuireEmail);
            }
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        public ActionResult AcquireEmailsAssignedStats()
        {
            var entities = _db.AcquireEmails.Where(x => x.AcquireEmailStatus == AcquireEmailStatusEnum.Created && x.IsAssigned)
                .GroupBy(x => new { x.AssignedTo, x.Campaign.CampaignName, x.Campaign.CampaignId});
            var list = new List<AcquireEmailStatsPerAgentAndCampaign>();

            foreach (var entity in entities)
            {
                var entitiesWithoutPhone = _db.AcquireEmails.Where(x =>
                    (x.Organization.OrganizationDetail.TelephoneNumber == String.Empty || x.Organization.OrganizationDetail.TelephoneNumber == null)
                    && (x.Organization.OrganizationDetail.MobilePhoneNumber == String.Empty || x.Organization.OrganizationDetail.MobilePhoneNumber == null)
                    && x.RelatedCampaignId == entity.Key.CampaignId
                    && x.AcquireEmailStatus == AcquireEmailStatusEnum.Created
                    && x.AssignedTo == entity.Key.AssignedTo);
                var temp = new AcquireEmailStatsPerAgentAndCampaign
                {
                    Agent = entity.Key.AssignedTo,
                    CampaignName = entity.Key.CampaignName,
                    CampaignId = entity.Key.CampaignId,
                    NumberOfEntitiesForProcessing = entity.Count(),
                    NumberOfEntitiesWithoutPhoneNumber = entitiesWithoutPhone.Count()
                };
                list.Add(temp);
            }

            var model = new AcquireEmailStatsPerAgentViewModel()
            {
                Campaigns = list.AsQueryable()
            };

            return View(model);
        }

        public IList<AcquireEmailExportModel> GetEntityList(int campaignId)
        {
            var entityList = (from ae in _db.AcquireEmails
                              where ae.RelatedCampaignId == campaignId && (ae.AcquireEmailStatus == AcquireEmailStatusEnum.Reviewed 
                              || ae.AcquireEmailStatus == AcquireEmailStatusEnum.Verified || ae.AcquireEmailStatus == AcquireEmailStatusEnum.Checked /*Ptiček request - temporary!*/)
                              select new AcquireEmailExportModel
                              {
                                  Entity = ae,
                                  CampaignName = ae.Campaign.CampaignName,
                                  VAT = ae.Organization.VAT,
                                  SubjectName = ae.Organization.SubjectName,
                                  AcquiredReceivingInformation = ae.Organization.MerDeliveryDetail.AcquiredReceivingInformation,
                                  AcquiredEmailEntityStatus = ae.AcquireEmailEntityStatus
                              }).ToList();
            return entityList;
        }
        public IList<AcquireEmailExportModel> GetReviewedEntityList(int campaignId)
        {
            var entityList = (from ae in _db.AcquireEmails
                where ae.RelatedCampaignId == campaignId && ae.AcquireEmailStatus == AcquireEmailStatusEnum.Reviewed
                select new AcquireEmailExportModel
                {
                    Entity = ae,
                    CampaignName = ae.Campaign.CampaignName,
                    VAT = ae.Organization.VAT,
                    SubjectName = ae.Organization.SubjectName,
                    AcquiredReceivingInformation = ae.Organization.MerDeliveryDetail.AcquiredReceivingInformation,
                    AcquiredEmailEntityStatus = ae.AcquireEmailEntityStatus
                }).ToList();
            return entityList;
        }

        public IList<AcquireEmailExportForEmailNotificationModel> GetEntityListForEmailNotification(int campaignId)
        {
            var entityList = (from ae in _db.AcquireEmails
                where ae.RelatedCampaignId == campaignId &&
                      ae.Organization.MerDeliveryDetail.AcquiredReceivingInformation.Contains("@")
                      select new AcquireEmailExportForEmailNotificationModel
                      {
                          AcquiredReceivingInformation = ae.Organization.MerDeliveryDetail.AcquiredReceivingInformation
                      }).ToList();
            return entityList;
        }
    }
}