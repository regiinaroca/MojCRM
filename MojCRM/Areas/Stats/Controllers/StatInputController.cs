using MojCRM.Areas.Stats.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojCRM.Areas.Stats.Controllers
{

    public class StatInputController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Stats/StatInput
        public ActionResult StatInput()
        {
            return View();
        }
           
   
    [HttpPost]
    public ActionResult StatInput(TechDepController model)
    {
            /*

            _db.TechDepartmentStat.Add(new TechDepController()
    {
                Agent = User.Identity.Name,
                TechIncomingCall = model.TechIncomingCall,
                TechOutgoingCall = model.TechOutgoingCall,
                TechCallDuration = model.TechCallDuration,
                TechCompaniesActivated = model.TechCompaniesActivated,
                TechEmialCustomers = model.TechEmialCustomers,
                TechEmialIntegrators = model.TechEmialIntegrators

            }
            );*/

            _db.TechDepartmentStat.Add(new TechDepController()
            {
                Agent = "sdfsdf",
                TechIncomingCall = 1,
                TechOutgoingCall = 2,
                TechCallDuration =2,
                TechCompaniesActivated = 2,
                TechEmialCustomers =2,
                TechEmialIntegrators = 2

            }
           ); 

            _db.SaveChanges();
        
            return View();

        }

    }
}
