using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MojCRM.Helpers;
using Newtonsoft.Json;

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

            model.NumberOfAttributesUpdated = _adminHelper.UpdateOrganizationAttributes();

            var returnModel = JsonConvert.SerializeObject(model);
            return Json(returnModel, JsonRequestBehavior.AllowGet);
        }
    }
}