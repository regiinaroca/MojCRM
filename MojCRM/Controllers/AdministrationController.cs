using System.Web.Mvc;
using MojCRM.Helpers;
using System.Web;
using OfficeOpenXml;
using MojCRM.Models;
using MojCRM.Areas.HelpDesk.Helpers;
using System.Linq;
using MojCRM.Areas.Sales.Helpers;
using System.Runtime.InteropServices;
using System;

namespace MojCRM.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly AdminHelperMethods _adminHelper = new AdminHelperMethods();
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly HelperMethods _helper = new HelperMethods();
        private readonly AcquireEmailMethodHelpers _acquireEmailMethodHelpers = new AcquireEmailMethodHelpers();
        private readonly OpportunityHelperMethods _opportunityHelperMethods = new OpportunityHelperMethods();
        // GET: Administration
        public ActionResult Index()
        {
            return View();
        }

        // GET: DailyUpdates
        public JsonResult DailyUpdates()
        {
            var model = new DailyUpdateReturnModel();

            model.NumberOfOrganizationCountriesUpdated = _adminHelper.UpdateOrganizationCountries();

            var returnModel = model;
            return Json(returnModel, JsonRequestBehavior.AllowGet);
        }

        // POST: MassUpdateClosedSubjects
        [HttpPost]
        public ActionResult MassUpdateClosedSubjects(HttpPostedFileBase file)
        {
            try
            {
                int updatedEntities = 0;
                int passedEntities = 0;

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
                            organization.IsActive = false;
                            organization.MerDeliveryDetail.AcquiredReceivingInformation = "ZATVORENA TVRTKA";
                            organization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified = true;
                            _acquireEmailMethodHelpers.UpdateClosedSubjectEntities(organization.MerId);
                            _opportunityHelperMethods.UpdateClosedSubjectOpportunities(organization.MerId);
                            updatedEntities++;
                        }
                        else
                        {
                            passedEntities++;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                _db.ActivityLogs.Add(new ActivityLog()
                {
                    ActivityType = ActivityLog.ActivityTypeEnum.System,
                    Department = ActivityLog.DepartmentEnum.MojCrm,
                    InsertDate = DateTime.Now,
                    IsSuspiciousActivity = false,
                    Module = ActivityLog.ModuleEnum.MojCrm,
                    ReferenceId = 0,
                    User = User.Identity.Name,
                    Description = @"Moj-CRM -- MassUpdateClosedSubjects -- Ukupno je ažurirano " + updatedEntities + " tvrtki, a " +
                    passedEntities + " nije ažurirano."
                });
                _db.SaveChanges();

                wb.Dispose();
                return View("Index");
            }
            catch (COMException)
            {
                return View("ErrorOldExcel");
            }
        }
    }
}