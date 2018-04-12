using System.Web.Mvc;
using MojCRM.Helpers;

namespace MojCRM.Controllers

{
    public class AdministrationController : Controller
    {
        private readonly AdminHelperMethods _adminHelper = new AdminHelperMethods();
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
    }
}