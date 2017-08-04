using MojCRM.Models;
using MojCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojCRM.Controllers
{
    public class StatsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Stats
        public ActionResult Index()
        {
            return View();
        }

        // GET: Stats/PersonalDailyActivities
        public ActionResult PersonalDailyActivities(string Name, string Agent, string SearchDate)
        {


            var SuccessfulCalls = (from a in db.ActivityLogs
                                   where a.ActivityType == ActivityLog.ActivityTypeEnum.SUCCALL 
                                   select a);
            var ShortSuccessfulCalls = (from a in db.ActivityLogs
                                   where a.ActivityType == ActivityLog.ActivityTypeEnum.SUCCALSHORT 
                                   select a);
            var UnsuccessfulCalls = (from a in db.ActivityLogs
                                     where a.ActivityType == ActivityLog.ActivityTypeEnum.UNSUCCAL
                                     select a);
            var MailChange = (from a in db.ActivityLogs
                              where a.ActivityType == ActivityLog.ActivityTypeEnum.MAILCHANGE
                              select a);
            var Resend = (from a in db.ActivityLogs
                          where a.ActivityType == ActivityLog.ActivityTypeEnum.RESEND
                          select a);
            var DeliveryMail = (from a in db.ActivityLogs
                                where a.ActivityType == ActivityLog.ActivityTypeEnum.EMAIL
                                select a);

            

            var ReferenceDate = DateTime.Today.AddDays(-1);
           
            var _Agents = (from u in db.Users
                           select u).AsEnumerable();
           
            var _Activities = (from a in db.ActivityLogs
                               select a).ToList();

          
            var searchDate = Convert.ToDateTime(SearchDate);
           
            var searchDatePlus = searchDate.AddDays(1);

            var _DistinctDepartments = (from a in db.ActivityLogs
                                        where (a.User == Name) && (a.InsertDate >= ReferenceDate) && (a.InsertDate < DateTime.Today)
                                        select a.Department).Distinct().Count();

            if (String.IsNullOrEmpty(SearchDate))
            {
                _Activities = _Activities.Where(a => (a.User == Name) && /*(a.InsertDate >= ReferenceDate) && (a.InsertDate < DateTime.Today)*/ (a.InsertDate>= DateTime.Today)).ToList();

                

               
                    SuccessfulCalls = SuccessfulCalls.Where(a => a.InsertDate >= DateTime.Today && a.User == Name);
                ShortSuccessfulCalls = ShortSuccessfulCalls.Where(a => a.InsertDate >= DateTime.Today && a.User == Name);

                UnsuccessfulCalls = UnsuccessfulCalls.Where(t => t.InsertDate >= DateTime.Today && t.User == Name);
                MailChange = MailChange.Where(t => t.InsertDate >= DateTime.Today && t.User == Name);
                Resend = Resend.Where(t => t.InsertDate >= DateTime.Today && t.User == Name);
                DeliveryMail = DeliveryMail.Where(t => t.InsertDate >= DateTime.Today && t.User == Name);



                ViewBag.DistinctDepartments = _DistinctDepartments;
                ViewBag.Date = ReferenceDate.ToShortDateString();
            }
            if (!String.IsNullOrEmpty(SearchDate))
            {

                SuccessfulCalls = SuccessfulCalls.Where(a => ((a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)) && a.User == Name);
                ShortSuccessfulCalls = ShortSuccessfulCalls.Where(a => ((a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)) && a.User == Name);

                UnsuccessfulCalls = UnsuccessfulCalls.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Name);
                MailChange = MailChange.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Name);
                Resend = Resend.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Name);
                DeliveryMail = DeliveryMail.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Name);
                _Activities = _Activities.Where(a => (a.User == Name) && (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)).ToList();
                
                _DistinctDepartments = (from a in db.ActivityLogs
                                        where (a.User == Name) && (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                                        select a.Department).Distinct().Count();
                ViewBag.Date = searchDate.ToShortDateString();
                ViewBag.DistinctDepartments = _DistinctDepartments;
            }
            if (!String.IsNullOrEmpty(SearchDate) && !String.IsNullOrEmpty(Agent))
            {

                SuccessfulCalls = SuccessfulCalls.Where(a => ((a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)) && a.User == Agent);
                ShortSuccessfulCalls = ShortSuccessfulCalls.Where(a => ((a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)) && a.User == Agent);

                UnsuccessfulCalls = UnsuccessfulCalls.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Agent);
                MailChange = MailChange.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Agent);
                Resend = Resend.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Agent);
                DeliveryMail = DeliveryMail.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Name);

                _Activities = _Activities.Where(a => (a.User == Agent) && (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)).ToList();
                _DistinctDepartments = (from a in db.ActivityLogs
                                        where (a.User == Agent) && (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                                        select a.Department).Distinct().Count();
                ViewBag.DistinctDepartments = _DistinctDepartments;
                ViewBag.Date = searchDate.ToShortDateString();
            }

            if (!String.IsNullOrEmpty(Agent))
            {
                ViewBag.User = Agent;
            }
            else
            {
                ViewBag.User = Name;
            }

            var PersonalActivities = new PersonalDailyActivitiesViewModel
            {
                PersonalActivities = _Activities,
                Agents = _Agents,
                SumSuccessfulCalls = SuccessfulCalls.Count(),
                SumShortSuccessfulCalls = ShortSuccessfulCalls.Count(),
                SumUnsuccessfulCalls = UnsuccessfulCalls.Count(),
                SumMailchange = MailChange.Count(),
                SumResend = Resend.Count(),
                SumSentMail = DeliveryMail.Count(),
               
            };

            return View(PersonalActivities);
        }

        // GET: Stats/CallCenterDaily
        public ActionResult CallCenterDaily(string SearchDate)
        {
            var AgentActivities = (from a in db.ActivityLogs
                                   where a.InsertDate >= DateTime.Today
                                   group a by a.User into ga
                                   select ga).ToList();
            var DepartmentActivities = (from a in db.ActivityLogs
                                        where a.InsertDate >= DateTime.Today
                                        group a by a.Department into ga
                                        select ga).ToList();
            var SuccessfulCalls = (from a in db.ActivityLogs
                                   where a.ActivityType == ActivityLog.ActivityTypeEnum.SUCCALL || a.ActivityType == ActivityLog.ActivityTypeEnum.SUCCALSHORT
                                   select a);
            var UnsuccessfulCalls = (from a in db.ActivityLogs
                                     where a.ActivityType == ActivityLog.ActivityTypeEnum.UNSUCCAL
                                     select a);
            var MailChange = (from a in db.ActivityLogs
                              where a.ActivityType == ActivityLog.ActivityTypeEnum.MAILCHANGE
                              select a);
            var Resend = (from a in db.ActivityLogs
                          where a.ActivityType == ActivityLog.ActivityTypeEnum.RESEND
                          select a);
            var DeliveryMail = (from a in db.ActivityLogs
                                where a.ActivityType == ActivityLog.ActivityTypeEnum.EMAIL
                                select a);
            var Activities = new List<CallCenterDaily>();
            var ActivitiesByDepartment = new List<CallCenterDailyByDepartment>();

            if (!String.IsNullOrEmpty(SearchDate))
            {
                var searchDate = Convert.ToDateTime(SearchDate);
                var searchDatePlus = searchDate.AddDays(1);
                SuccessfulCalls = SuccessfulCalls.Where(a => (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus));
                UnsuccessfulCalls = UnsuccessfulCalls.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                MailChange = MailChange.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                Resend = Resend.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                DeliveryMail = DeliveryMail.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));

                AgentActivities = (from a in db.ActivityLogs
                                   where (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                                   group a by a.User into ga
                                   select ga).ToList();
                DepartmentActivities = (from a in db.ActivityLogs
                                        where (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                                        group a by a.Department into ga
                                        select ga).ToList();
                foreach (var Day in AgentActivities)
                {
                    var dailyActivities = new CallCenterDaily
                    {
                        Agent = Day.Key,
                        NumberSuccessfulCalls = SuccessfulCalls.Where(a => a.User == Day.Key).Count(),
                        NumberUnsuccessfulCalls = UnsuccessfulCalls.Where(a => a.User == Day.Key).Count(),
                        NumberMailchange = MailChange.Where(a => a.User == Day.Key).Count(),
                        NumberResend = Resend.Where(a => a.User == Day.Key).Count(),
                        NumberDeliveryMail = DeliveryMail.Where(a => a.User == Day.Key).Count()
                    };
                    Activities.Add(dailyActivities);
                }
                foreach (var Department in DepartmentActivities)
                {
                    var dailyActivitiesByDepartment = new CallCenterDailyByDepartment
                    {
                        Department = Department.Key,
                        NumberSuccessfulCalls = SuccessfulCalls.Where(a => a.Department == Department.Key).Count(),
                        NumberUnsuccessfulCalls = UnsuccessfulCalls.Where(a => a.Department == Department.Key).Count(),
                        NumberMailchange = MailChange.Where(a => a.Department == Department.Key).Count(),
                        NumberResend = Resend.Where(a => a.Department == Department.Key).Count(),
                        NumberMail = DeliveryMail.Where(a => a.Department == Department.Key).Count()
                    };
                    ActivitiesByDepartment.Add(dailyActivitiesByDepartment);
                }
            }
            else
            {
                SuccessfulCalls = SuccessfulCalls.Where(a => a.InsertDate >= DateTime.Today);
                UnsuccessfulCalls = UnsuccessfulCalls.Where(t => t.InsertDate >= DateTime.Today);
                MailChange = MailChange.Where(t => t.InsertDate >= DateTime.Today);
                Resend = Resend.Where(t => t.InsertDate >= DateTime.Today);
                DeliveryMail = DeliveryMail.Where(t => t.InsertDate >= DateTime.Today);
                foreach (var Day in AgentActivities)
                {
                    var dailyActivities = new CallCenterDaily
                    {
                        Agent = Day.Key,
                        NumberSuccessfulCalls = SuccessfulCalls.Where(a => a.User == Day.Key).Count(),
                        NumberUnsuccessfulCalls = UnsuccessfulCalls.Where(a => a.User == Day.Key).Count(),
                        NumberMailchange = MailChange.Where(a => a.User == Day.Key).Count(),
                        NumberResend = Resend.Where(a => a.User == Day.Key).Count(),
                        NumberDeliveryMail = DeliveryMail.Where(a => a.User == Day.Key).Count()
                    };
                    Activities.Add(dailyActivities);
                }
                foreach (var Department in DepartmentActivities)
                {
                    var dailyActivitiesByDepartment = new CallCenterDailyByDepartment
                    {
                        Department = Department.Key,
                        NumberSuccessfulCalls = SuccessfulCalls.Where(a => a.Department == Department.Key).Count(),
                        NumberUnsuccessfulCalls = UnsuccessfulCalls.Where(a => a.Department == Department.Key).Count(),
                        NumberMailchange = MailChange.Where(a => a.Department == Department.Key).Count(),
                        NumberResend = Resend.Where(a => a.Department == Department.Key).Count(),
                        NumberMail = DeliveryMail.Where(a => a.Department == Department.Key).Count()
                    };
                    ActivitiesByDepartment.Add(dailyActivitiesByDepartment);
                }
            }

            var model = new CallCenterDailyStatsViewModel
            {
                Activities = Activities,
                ActivitiesByDepartment = ActivitiesByDepartment,
                SumSuccessfulCalls = SuccessfulCalls.Count(),
                SumUnsuccessfulCalls = UnsuccessfulCalls.Count(),
                SumMailchange = MailChange.Count(),
                SumResend = Resend.Count(),
                SumSentMail = DeliveryMail.Count()
            };

            return View(model);
        }

        // GET: Stats/Delivery
        public ActionResult Delivery(string SearchDate)
        {
            var CreatedTickets = (from t in db.DeliveryTicketModels
                                  select t);
            var CreatedTicketsFirst = (from t in db.DeliveryTicketModels
                                       where (t.FirstInvoice == true)
                                       select t);
            var GroupedDeliveries = (from t in db.DeliveryTicketModels
                                     where t.InsertDate >= DateTime.Today
                                     group t by new { Date = DbFunctions.TruncateTime(t.SentDate), t.AssignedTo }  into gt
                                     select gt).ToList();
            var Deliveries = new List<DailyDelivery>();

            if (!String.IsNullOrEmpty(SearchDate))
            {
                var searchDate = Convert.ToDateTime(SearchDate);
                var searchDatePlus = searchDate.AddDays(1);
                CreatedTickets = CreatedTickets.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                CreatedTicketsFirst = CreatedTicketsFirst.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                GroupedDeliveries = (from t in db.DeliveryTicketModels
                                     where (t.InsertDate > searchDate) && (t.InsertDate < searchDatePlus)
                                     group t by new { Date = DbFunctions.TruncateTime(t.SentDate), t.AssignedTo } into gt
                                     select gt).ToList();
                var GroupedDeliveriesAssigned = (from t in db.DeliveryTicketModels
                                                 where (t.InsertDate > searchDate) && (t.InsertDate < searchDatePlus)
                                                 select t.AssignedTo).Distinct();
                foreach (var Day in GroupedDeliveries)
                {
                    var dailyDelivery = new DailyDelivery
                    {
                        ReferenceDate = (DateTime)Day.Key.Date,
                        CreatedTicketsCount = Day.Count(),
                        CreatedTicketsFirstTimeCount = Day.Where(t => t.FirstInvoice == true).Count(),
                        SentCount = Day.Where(t => t.DocumentStatus == 30).Count(),
                        DeliveredCount = Day.Where(t => t.DocumentStatus == 40).Count(),
                        OtherCount = Day.Where(t => (t.DocumentStatus != 30 && t.DocumentStatus != 40)).Count(),
                        AssignedToCount = Day.Where(t => t.IsAssigned == true).Count(),
                        AssignedTo = Day.Key.AssignedTo
                    };
                    Deliveries.Add(dailyDelivery);
                }
            }
            else
            {
                CreatedTickets = CreatedTickets.Where(t => t.InsertDate >= DateTime.Today);
                CreatedTicketsFirst = CreatedTicketsFirst.Where(t => t.InsertDate >= DateTime.Today);
                var GroupedDeliveriesAssigned = (from t in db.DeliveryTicketModels
                                                 where t.SentDate >= DateTime.Today
                                                 select t.AssignedTo).Distinct();
                foreach (var Day in GroupedDeliveries)
                {
                    var dailyDelivery = new DailyDelivery
                    {
                        ReferenceDate = (DateTime)Day.Key.Date,
                        CreatedTicketsCount = Day.Count(),
                        CreatedTicketsFirstTimeCount = Day.Where(t => t.FirstInvoice == true).Count(),
                        SentCount = Day.Where(t => t.DocumentStatus == 30).Count(),
                        DeliveredCount = Day.Where(t => t.DocumentStatus == 40).Count(),
                        OtherCount = Day.Where(t => (t.DocumentStatus != 30 && t.DocumentStatus != 40)).Count(),
                        AssignedToCount = Day.Where(t => t.IsAssigned == true).Count(),
                        AssignedTo = Day.Key.AssignedTo
                    };
                    Deliveries.Add(dailyDelivery);
                }  
            }

            var model = new DeliveryStatsViewModel
            {
                CreatedTicketsTodayCount = CreatedTickets.Count(),
                CreatedTicketsTodayFirstTimeCount = CreatedTicketsFirst.Count(),
                Deliveries = Deliveries
            };

            var _date = new DateTime(2017, 7, 1);
            ViewBag.TotalOpenedTickets = (from t in db.DeliveryTicketModels
                                          where t.IsAssigned == false && t.InsertDate >= _date && t.DocumentStatus == 30
                                          select t).Count();

            return View(model);
        }
    }
}