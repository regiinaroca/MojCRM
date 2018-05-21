using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using MojCRM.Areas.Sales.Models;
using MojCRM.Areas.Stats.ViewModels;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Areas.Campaigns.Models;

namespace MojCRM.Areas.Stats.Controllers
{
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Stats
        public ActionResult Index()
        {
            return View();
        }

        // GET: Stats/PersonalDailyActivities
        public ActionResult PersonalDailyActivities(string name, string agent, string searchDate)
        {
            var successfulCalls = (from a in _db.ActivityLogs
                                   where a.ActivityType == ActivityLog.ActivityTypeEnum.Succall 
                                   select a);
            var shortSuccessfulCalls = (from a in _db.ActivityLogs
                                   where a.ActivityType == ActivityLog.ActivityTypeEnum.Succalshort 
                                   select a);
            var unsuccessfulCalls = (from a in _db.ActivityLogs
                                     where a.ActivityType == ActivityLog.ActivityTypeEnum.Unsuccal
                                     select a);
            var mailChange = (from a in _db.ActivityLogs
                              where a.ActivityType == ActivityLog.ActivityTypeEnum.Mailchange
                              select a);
            var resend = (from a in _db.ActivityLogs
                          where a.ActivityType == ActivityLog.ActivityTypeEnum.Resend
                          select a);
            var deliveryMail = (from a in _db.ActivityLogs
                                where a.ActivityType == ActivityLog.ActivityTypeEnum.Email
                                select a);
            var acquiredEmails = _db.ActivityLogs.Where(al => al.Description.Contains("@") && al.Description.Contains("nova informacija o preuzimanju"));
            var acquiredTelephoneNumbers = _db.ActivityLogs.Where(al =>
                al.Description.Contains("- broj mobitela") || al.Description.Contains("- broj telefona"));

            var agents = (from u in _db.Users
                           select u);
           
            var activities = (from a in _db.ActivityLogs
                               select a);

          
            var searchDateDt = Convert.ToDateTime(searchDate);
           
            var searchDatePlus = searchDateDt.AddDays(1);

            var distinctDepartments = (from a in _db.ActivityLogs
                                        where (a.User == name) && (a.InsertDate >= DateTime.Today)
                                        select a.Department).Distinct().Count();
            
            if (String.IsNullOrEmpty(searchDate))
            {
                activities = activities.Where(a => (a.User == name) && /*(a.InsertDate >= ReferenceDate) && (a.InsertDate < DateTime.Today)*/ (a.InsertDate>= DateTime.Today));

                successfulCalls = successfulCalls.Where(a => a.InsertDate >= DateTime.Today && a.User == name);
                shortSuccessfulCalls = shortSuccessfulCalls.Where(a => a.InsertDate >= DateTime.Today && a.User == name);

                unsuccessfulCalls = unsuccessfulCalls.Where(t => t.InsertDate >= DateTime.Today && t.User == name);
                mailChange = mailChange.Where(t => t.InsertDate >= DateTime.Today && t.User == name);
                resend = resend.Where(t => t.InsertDate >= DateTime.Today && t.User == name);
                deliveryMail = deliveryMail.Where(t => t.InsertDate >= DateTime.Today && t.User == name);
                acquiredEmails = acquiredEmails.Where(t => t.InsertDate >= DateTime.Today && t.User == name);
                acquiredTelephoneNumbers = acquiredTelephoneNumbers.Where(t => t.InsertDate >= DateTime.Today && t.User == name);


                ViewBag.DistinctDepartments = distinctDepartments;
                ViewBag.Date = DateTime.Today.ToShortDateString();
            }
            if (!String.IsNullOrEmpty(searchDate) && String.IsNullOrEmpty(agent))
            {

                successfulCalls = successfulCalls.Where(a => ((a.InsertDate >= searchDateDt) && (a.InsertDate < searchDatePlus)) && a.User == name);
                shortSuccessfulCalls = shortSuccessfulCalls.Where(a => ((a.InsertDate >= searchDateDt) && (a.InsertDate < searchDatePlus)) && a.User == name);

                unsuccessfulCalls = unsuccessfulCalls.Where(t => ((t.InsertDate >= searchDateDt) && (t.InsertDate < searchDatePlus)) && t.User == name);
                mailChange = mailChange.Where(t => ((t.InsertDate >= searchDateDt) && (t.InsertDate < searchDatePlus)) && t.User == name);
                resend = resend.Where(t => ((t.InsertDate >= searchDateDt) && (t.InsertDate < searchDatePlus)) && t.User == name);
                deliveryMail = deliveryMail.Where(t => ((t.InsertDate >= searchDateDt) && (t.InsertDate < searchDatePlus)) && t.User == name);
                activities = activities.Where(a => (a.User == name) && (a.InsertDate >= searchDateDt) && (a.InsertDate < searchDatePlus));
                acquiredEmails = acquiredEmails.Where(a => (a.User == name) && (a.InsertDate >= searchDateDt) && (a.InsertDate < searchDatePlus));
                acquiredTelephoneNumbers = acquiredTelephoneNumbers.Where(a => (a.User == name) && (a.InsertDate >= searchDateDt) && (a.InsertDate < searchDatePlus));
                
                distinctDepartments = (from a in _db.ActivityLogs
                                        where (a.User == name) && (a.InsertDate >= searchDateDt) && (a.InsertDate < searchDatePlus)
                                        select a.Department).Distinct().Count();
                ViewBag.Date = searchDateDt.ToShortDateString();
                ViewBag.DistinctDepartments = distinctDepartments;
            }
            if (!String.IsNullOrEmpty(searchDate) && !String.IsNullOrEmpty(agent))
            {
                
                successfulCalls = successfulCalls.Where(a => ((a.InsertDate >= searchDateDt) && (a.InsertDate < searchDatePlus)) && a.User == agent);
                shortSuccessfulCalls = shortSuccessfulCalls.Where(a => ((a.InsertDate >= searchDateDt) && (a.InsertDate < searchDatePlus)) && a.User == agent);

                unsuccessfulCalls = unsuccessfulCalls.Where(t => ((t.InsertDate >= searchDateDt) && (t.InsertDate < searchDatePlus)) && t.User == agent);
                mailChange = mailChange.Where(t => ((t.InsertDate >= searchDateDt) && (t.InsertDate < searchDatePlus)) && t.User == agent);
                resend = resend.Where(t => ((t.InsertDate >= searchDateDt) && (t.InsertDate < searchDatePlus)) && t.User == agent);
                deliveryMail = deliveryMail.Where(t => ((t.InsertDate >= searchDateDt) && (t.InsertDate < searchDatePlus)) && t.User == agent);
                acquiredEmails = acquiredEmails.Where(t => ((t.InsertDate >= searchDateDt) && (t.InsertDate < searchDatePlus)) && t.User == agent);
                acquiredTelephoneNumbers = acquiredTelephoneNumbers.Where(t => ((t.InsertDate >= searchDateDt) && (t.InsertDate < searchDatePlus)) && t.User == agent);

                activities = activities.Where(a => (a.User == agent) && (a.InsertDate >= searchDateDt) && (a.InsertDate < searchDatePlus));

                distinctDepartments = (from a in _db.ActivityLogs
                                        where (a.User == agent) && (a.InsertDate >= searchDateDt) && (a.InsertDate < searchDatePlus)
                                        select a.Department).Distinct().Count();
                ViewBag.DistinctDepartments = distinctDepartments;
                ViewBag.Date = searchDateDt.ToShortDateString();
            }

            ViewBag.User = !String.IsNullOrEmpty(agent) ? agent : name;

            var personalActivities = new PersonalDailyActivitiesViewModel
            {
                PersonalActivities = activities,
                Agents = agents,
                SumSuccessfulCalls = successfulCalls.Count(),
                SumShortSuccessfulCalls = shortSuccessfulCalls.Count(),
                SumUnsuccessfulCalls = unsuccessfulCalls.Count(),
                SumMailchange = mailChange.Count(),
                SumResend = resend.Count(),
                SumSentMail = deliveryMail.Count(),
               SumAcquiredEmails = acquiredEmails.Count(),
               SumAcquiredPhoneNumbers = acquiredTelephoneNumbers.Count()
            };

            return View(personalActivities);
        }

        // GET: Stats/CallCenterDaily
        public ActionResult CallCenterDaily(string search)
        {
            var agentActivities = from a in _db.ActivityLogs
                where a.InsertDate >= DateTime.Today
                group a by a.User into ga
                select ga;
            var departmentActivities = from a in _db.ActivityLogs
                where a.InsertDate >= DateTime.Today
                group a by a.Department into ga
                select ga;
            var successfulCalls = _db.ActivityLogs.Where(a =>
                a.ActivityType == ActivityLog.ActivityTypeEnum.Succall ||
                a.ActivityType == ActivityLog.ActivityTypeEnum.Succalshort);
            var successfulCallsSuspicious = successfulCalls.Where(a => a.IsSuspiciousActivity);
            var unsuccessfulCalls = _db.ActivityLogs.Where(a => a.ActivityType == ActivityLog.ActivityTypeEnum.Unsuccal);
            var unsuccessfulCallsSuspicious = unsuccessfulCalls.Where(a => a.IsSuspiciousActivity);
            var mailChange = _db.ActivityLogs.Where(a => a.ActivityType == ActivityLog.ActivityTypeEnum.Mailchange);
            var resend = _db.ActivityLogs.Where(a => a.ActivityType == ActivityLog.ActivityTypeEnum.Resend);
            var deliveryMail = _db.ActivityLogs.Where(a => a.ActivityType == ActivityLog.ActivityTypeEnum.Email);
            var ticketsAssigned = _db.ActivityLogs.Where(a => a.ActivityType == ActivityLog.ActivityTypeEnum.Ticketassign);
            var acquiredEmails = _db.ActivityLogs.Where(al => al.Description.Contains("@") && al.Description.Contains("nova informacija o preuzimanju"));
            var acquiredTelephoneNumbers = _db.ActivityLogs.Where(al =>
                al.Description.Contains("- broj mobitela") || al.Description.Contains("- broj telefona"));
            var activities = new List<CallCenterDaily>();
            var activitiesByDepartment = new List<CallCenterDailyByDepartment>();

            if (!string.IsNullOrEmpty(search))
            {
                var searchDate = Convert.ToDateTime(search);
                var searchDatePlus = searchDate.AddDays(1);
                successfulCalls = successfulCalls.Where(a => (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus));
                successfulCallsSuspicious = successfulCallsSuspicious.Where(a => (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus));
                unsuccessfulCalls = unsuccessfulCalls.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                unsuccessfulCallsSuspicious = unsuccessfulCalls.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                mailChange = mailChange.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                resend = resend.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                deliveryMail = deliveryMail.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                ticketsAssigned = ticketsAssigned.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                acquiredEmails = acquiredEmails.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                acquiredTelephoneNumbers = acquiredTelephoneNumbers.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));

                agentActivities = from a in _db.ActivityLogs
                    where (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                    group a by a.User into ga
                    select ga;
                departmentActivities = from a in _db.ActivityLogs
                    where (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                    group a by a.Department into ga
                    select ga;
                foreach (var day in agentActivities)
                {
                    var dailyActivities = new CallCenterDaily
                    {
                        Agent = day.Key,
                        NumberSuccessfulCalls = successfulCalls.Count(a => a.User == day.Key),
                        NumberSuccessfulCallsSuspicious = successfulCallsSuspicious.Count(a => a.User == day.Key),
                        NumberUnsuccessfulCalls = unsuccessfulCalls.Count(a => a.User == day.Key),
                        NumberUnsuccessfulCallsSuspicious = unsuccessfulCallsSuspicious.Count(a => a.User == day.Key),
                        NumberMailchange = mailChange.Count(a => a.User == day.Key),
                        NumberResend = resend.Count(a => a.User == day.Key),
                        NumberMail = deliveryMail.Count(a => a.User == day.Key),
                        NumberTicketsAssigned = ticketsAssigned.Count(a => a.User == day.Key),
                        NumberAcquiredEmails = acquiredEmails.Count(a => a.User == day.Key),
                        NumberAcquiredTelephoneNumbers = acquiredTelephoneNumbers.Count(a => a.User == day.Key)
                    };
                    activities.Add(dailyActivities);
                }
                foreach (var department in departmentActivities)
                {
                    var dailyActivitiesByDepartment = new CallCenterDailyByDepartment
                    {
                        Department = department.Key,
                        NumberSuccessfulCalls = successfulCalls.Count(a => a.Department == department.Key),
                        NumberUnsuccessfulCalls = unsuccessfulCalls.Count(a => a.Department == department.Key),
                        NumberMailchange = mailChange.Count(a => a.Department == department.Key),
                        NumberResend = resend.Count(a => a.Department == department.Key),
                        NumberMail = deliveryMail.Count(a => a.Department == department.Key)
                    };
                    activitiesByDepartment.Add(dailyActivitiesByDepartment);
                }
            }
            else
            {
                successfulCalls = successfulCalls.Where(a => a.InsertDate >= DateTime.Today);
                successfulCallsSuspicious = successfulCallsSuspicious.Where(a => a.InsertDate >= DateTime.Today);
                unsuccessfulCalls = unsuccessfulCalls.Where(t => t.InsertDate >= DateTime.Today);
                unsuccessfulCallsSuspicious = unsuccessfulCallsSuspicious.Where(a => a.InsertDate >= DateTime.Today);
                mailChange = mailChange.Where(t => t.InsertDate >= DateTime.Today);
                resend = resend.Where(t => t.InsertDate >= DateTime.Today);
                deliveryMail = deliveryMail.Where(t => t.InsertDate >= DateTime.Today);
                ticketsAssigned = ticketsAssigned.Where(t => t.InsertDate >= DateTime.Today);
                acquiredEmails = acquiredEmails.Where(t => t.InsertDate >= DateTime.Today);
                acquiredTelephoneNumbers = acquiredTelephoneNumbers.Where(t => t.InsertDate >= DateTime.Today);
                foreach (var day in agentActivities)
                {
                    var dailyActivities = new CallCenterDaily
                    {
                        Agent = day.Key,
                        NumberSuccessfulCalls = successfulCalls.Count(a => a.User == day.Key),
                        NumberSuccessfulCallsSuspicious = successfulCallsSuspicious.Count(a => a.User == day.Key),
                        NumberUnsuccessfulCalls = unsuccessfulCalls.Count(a => a.User == day.Key),
                        NumberUnsuccessfulCallsSuspicious = unsuccessfulCallsSuspicious.Count(a => a.User == day.Key),
                        NumberMailchange = mailChange.Count(a => a.User == day.Key),
                        NumberResend = resend.Count(a => a.User == day.Key),
                        NumberMail = deliveryMail.Count(a => a.User == day.Key),
                        NumberTicketsAssigned = ticketsAssigned.Count(a => a.User == day.Key),
                        NumberAcquiredEmails = acquiredEmails.Count(a => a.User == day.Key),
                        NumberAcquiredTelephoneNumbers = acquiredTelephoneNumbers.Count(a => a.User == day.Key)
                    };
                    activities.Add(dailyActivities);
                }
                foreach (var department in departmentActivities)
                {
                    var dailyActivitiesByDepartment = new CallCenterDailyByDepartment
                    {
                        Department = department.Key,
                        NumberSuccessfulCalls = successfulCalls.Count(a => a.Department == department.Key),
                        NumberUnsuccessfulCalls = unsuccessfulCalls.Count(a => a.Department == department.Key),
                        NumberMailchange = mailChange.Count(a => a.Department == department.Key),
                        NumberResend = resend.Count(a => a.Department == department.Key),
                        NumberMail = deliveryMail.Count(a => a.Department == department.Key)
                    };
                    activitiesByDepartment.Add(dailyActivitiesByDepartment);
                }
            }

            var model = new CallCenterDailyStatsViewModel
            {
                Activities = activities.AsQueryable(),
                ActivitiesByDepartment = activitiesByDepartment.AsQueryable(),
                SumSuccessfulCalls = successfulCalls.Count(),
                SumSuccessfulCallsSuspicious = successfulCallsSuspicious.Count(),
                SumUnsuccessfulCalls = unsuccessfulCalls.Count(),
                SumUnsuccessfulCallsSuspicious = unsuccessfulCallsSuspicious.Count(),
                SumMailchange = mailChange.Count(),
                SumResend = resend.Count(),
                SumSentMail = deliveryMail.Count(),
                SumTicketsAssigned = ticketsAssigned.Count(),
                SumAcquiredEmails = acquiredEmails.Count(),
                SumAcquiredTelephoneNumbers = acquiredTelephoneNumbers.Count()
            };

            return View(model);
        }

        // GET: Stats/CallCenterWeekly
        // Initially planned as weekly stats, but currently displays secificaly defined range
        public ActionResult CallCenterWeekly(string searchStart, string searchEnd)
        {

            IQueryable<IGrouping<string, ActivityLog>> agentActivities;
            IQueryable<IGrouping<ActivityLog.DepartmentEnum, ActivityLog>> departmentActivities;
            var successfulCalls = (_db.ActivityLogs.Where(a =>
                a.ActivityType == ActivityLog.ActivityTypeEnum.Succall ||
                a.ActivityType == ActivityLog.ActivityTypeEnum.Succalshort));
            var successfulCallsSuspicious = successfulCalls.Where(a => a.IsSuspiciousActivity);
            var unsuccessfulCalls = _db.ActivityLogs.Where(a => a.ActivityType == ActivityLog.ActivityTypeEnum.Unsuccal);
            var unsuccessfulCallsSuspicious = unsuccessfulCalls.Where(a => a.IsSuspiciousActivity);
            var mailChange = _db.ActivityLogs.Where(a => a.ActivityType == ActivityLog.ActivityTypeEnum.Mailchange);
            var resend = _db.ActivityLogs.Where(a => a.ActivityType == ActivityLog.ActivityTypeEnum.Resend);
            var deliveryMail = _db.ActivityLogs.Where(a => a.ActivityType == ActivityLog.ActivityTypeEnum.Email);
            var ticketsAssigned = _db.ActivityLogs.Where(a => a.ActivityType == ActivityLog.ActivityTypeEnum.Ticketassign);
            var acquiredEmails = _db.ActivityLogs.Where(al => al.Description.Contains("@") && al.Description.Contains("nova informacija o preuzimanju"));
            var acquiredTelephoneNumbers = _db.ActivityLogs.Where(al =>
                al.Description.Contains("- broj mobitela") || al.Description.Contains("- broj telefona"));
            var activities = new List<CallCenterWeekly>();
            var activitiesByDepartment = new List<CallCenterWeeklyByDepartment>();

            if (!string.IsNullOrEmpty(searchStart))
            {
                var searchDateStart = Convert.ToDateTime(searchStart);
                var searchDateEnd = Convert.ToDateTime(searchEnd).AddDays(1);
                successfulCalls = successfulCalls.Where(a => (a.InsertDate >= searchDateStart) && (a.InsertDate < searchDateEnd));
                successfulCallsSuspicious = successfulCallsSuspicious.Where(a => (a.InsertDate >= searchDateStart) && (a.InsertDate < searchDateEnd));
                unsuccessfulCalls = unsuccessfulCalls.Where(t => (t.InsertDate >= searchDateStart) && (t.InsertDate < searchDateEnd));
                unsuccessfulCallsSuspicious = unsuccessfulCallsSuspicious.Where(a => (a.InsertDate >= searchDateStart) && (a.InsertDate < searchDateEnd));
                mailChange = mailChange.Where(t => (t.InsertDate >= searchDateStart) && (t.InsertDate < searchDateEnd));
                resend = resend.Where(t => (t.InsertDate >= searchDateStart) && (t.InsertDate < searchDateEnd));
                deliveryMail = deliveryMail.Where(t => (t.InsertDate >= searchDateStart) && (t.InsertDate < searchDateEnd));
                ticketsAssigned = ticketsAssigned.Where(t => (t.InsertDate >= searchDateStart) && (t.InsertDate < searchDateEnd));
                acquiredEmails = acquiredEmails.Where(t => (t.InsertDate >= searchDateStart) && (t.InsertDate < searchDateEnd));
                acquiredTelephoneNumbers = acquiredTelephoneNumbers.Where(t => (t.InsertDate >= searchDateStart) && (t.InsertDate < searchDateEnd));

                agentActivities = _db.ActivityLogs
                    .Where(a => (a.InsertDate >= searchDateStart) && (a.InsertDate < searchDateEnd))
                    .GroupBy(a => a.User)
                    .Select(ga => ga);
                departmentActivities = _db.ActivityLogs
                    .Where(a => (a.InsertDate >= searchDateStart) && (a.InsertDate < searchDateEnd))
                    .GroupBy(a => a.Department)
                    .Select(ga => ga);
                foreach (var day in agentActivities)
                {
                    var dailyActivities = new CallCenterWeekly
                    {
                        Agent = day.Key,
                        NumberSuccessfulCalls = successfulCalls.Count(a => a.User == day.Key),
                        NumberSuccessfulCallsSuspicious = successfulCallsSuspicious.Count(a => a.User == day.Key),
                        NumberUnsuccessfulCalls = unsuccessfulCalls.Count(a => a.User == day.Key),
                        NumberUnsuccessfulCallsSuspicious = unsuccessfulCallsSuspicious.Count(a => a.User == day.Key),
                        NumberMailchange = mailChange.Count(a => a.User == day.Key),
                        NumberResend = resend.Count(a => a.User == day.Key),
                        NumberMail = deliveryMail.Count(a => a.User == day.Key),
                        NumberTicketsAssigned = ticketsAssigned.Count(a => a.User == day.Key),
                        NumberAcquiredEmails = acquiredEmails.Count(a => a.User == day.Key),
                        NumberAcquiredTelephoneNumbers = acquiredTelephoneNumbers.Count(a => a.User == day.Key),
                    };
                    activities.Add(dailyActivities);
                }
                foreach (var department in departmentActivities)
                {
                    var dailyActivitiesByDepartment = new CallCenterWeeklyByDepartment
                    {
                        Department = department.Key,
                        NumberSuccessfulCalls = successfulCalls.Count(a => a.Department == department.Key),
                        NumberUnsuccessfulCalls = unsuccessfulCalls.Count(a => a.Department == department.Key),
                        NumberMailchange = mailChange.Count(a => a.Department == department.Key),
                        NumberResend = resend.Count(a => a.Department == department.Key),
                        NumberMail = deliveryMail.Count(a => a.Department == department.Key)
                    };
                    activitiesByDepartment.Add(dailyActivitiesByDepartment);
                }
            }
            else
            {
                successfulCalls = successfulCalls.Where(a => a.InsertDate >= DateTime.Today);
                successfulCallsSuspicious = successfulCallsSuspicious.Where(a => a.InsertDate >= DateTime.Today);
                unsuccessfulCalls = unsuccessfulCalls.Where(t => t.InsertDate >= DateTime.Today);
                unsuccessfulCallsSuspicious= unsuccessfulCallsSuspicious.Where(a => a.InsertDate >= DateTime.Today);
                mailChange = mailChange.Where(t => t.InsertDate >= DateTime.Today);
                resend = resend.Where(t => t.InsertDate >= DateTime.Today);
                deliveryMail = deliveryMail.Where(t => t.InsertDate >= DateTime.Today);
                ticketsAssigned = ticketsAssigned.Where(t => t.InsertDate >= DateTime.Today);
                acquiredEmails = acquiredEmails.Where(t => t.InsertDate >= DateTime.Today);
                acquiredTelephoneNumbers = acquiredTelephoneNumbers.Where(t => t.InsertDate >= DateTime.Today);

                agentActivities = _db.ActivityLogs
                    .Where(t => t.InsertDate >= DateTime.Today)
                    .GroupBy(a => a.User)
                    .Select(ga => ga);
                departmentActivities = _db.ActivityLogs
                    .Where(t => t.InsertDate >= DateTime.Today)
                    .GroupBy(a => a.Department)
                    .Select(ga => ga);
                foreach (var day in agentActivities)
                {
                    var dailyActivities = new CallCenterWeekly
                    {
                        Agent = day.Key,
                        NumberSuccessfulCalls = successfulCalls.Count(a => a.User == day.Key),
                        NumberSuccessfulCallsSuspicious = successfulCallsSuspicious.Count(a => a.User == day.Key),
                        NumberUnsuccessfulCalls = unsuccessfulCalls.Count(a => a.User == day.Key),
                        NumberUnsuccessfulCallsSuspicious = unsuccessfulCallsSuspicious.Count(a => a.User == day.Key),
                        NumberMailchange = mailChange.Count(a => a.User == day.Key),
                        NumberResend = resend.Count(a => a.User == day.Key),
                        NumberMail = deliveryMail.Count(a => a.User == day.Key),
                        NumberTicketsAssigned = ticketsAssigned.Count(a => a.User == day.Key),
                        NumberAcquiredEmails = acquiredEmails.Count(a => a.User == day.Key),
                        NumberAcquiredTelephoneNumbers = acquiredTelephoneNumbers.Count(a => a.User == day.Key)
                    };
                    activities.Add(dailyActivities);
                }
                foreach (var department in departmentActivities)
                {
                    var dailyActivitiesByDepartment = new CallCenterWeeklyByDepartment
                    {
                        Department = department.Key,
                        NumberSuccessfulCalls = successfulCalls.Count(a => a.Department == department.Key),
                        NumberUnsuccessfulCalls = unsuccessfulCalls.Count(a => a.Department == department.Key),
                        NumberMailchange = mailChange.Count(a => a.Department == department.Key),
                        NumberResend = resend.Count(a => a.Department == department.Key),
                        NumberMail = deliveryMail.Count(a => a.Department == department.Key)
                    };
                    activitiesByDepartment.Add(dailyActivitiesByDepartment);
                }
            }

            var model = new CallCenterWeeklyStatsViewModel
            {
                Activities = activities.AsQueryable(),
                ActivitiesByDepartment = activitiesByDepartment.AsQueryable(),
                SumSuccessfulCalls = successfulCalls.Count(),
                SumSuccessfulCallsSuspicious = successfulCallsSuspicious.Count(),
                SumUnsuccessfulCalls = unsuccessfulCalls.Count(),
                SumUnsuccessfulCallsSuspicious = unsuccessfulCallsSuspicious.Count(),
                SumMailchange = mailChange.Count(),
                SumResend = resend.Count(),
                SumSentMail = deliveryMail.Count(),
                SumTicketsAssigned = ticketsAssigned.Count(),
                SumAcquiredEmails = acquiredEmails.Count(),
                SumAcquiredTelephoneNumbers = acquiredTelephoneNumbers.Count()
            };

            return View(model);
        }

        // GET: Stats/Delivery
        public ActionResult Delivery(string search)
        {
            var createdTickets = from t in _db.DeliveryTicketModels
                                  select t;
            var createdTicketsFirst = from t in _db.DeliveryTicketModels
                                       where t.FirstInvoice
                                       select t;
            var groupedDeliveries = (from t in _db.DeliveryTicketModels
                                     where t.InsertDate >= DateTime.Today
                                     group t by new { Date = DbFunctions.TruncateTime(t.SentDate), t.AssignedTo }  into gt
                                     select gt).ToList();
            var deliveries = new List<DailyDelivery>();

            if (!String.IsNullOrEmpty(search))
            {
                var searchDate = Convert.ToDateTime(search);
                var searchDatePlus = searchDate.AddDays(1);
                createdTickets = createdTickets.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                createdTicketsFirst = createdTicketsFirst.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                groupedDeliveries = (from t in _db.DeliveryTicketModels
                                     where (t.InsertDate > searchDate) && (t.InsertDate < searchDatePlus)
                                     group t by new { Date = DbFunctions.TruncateTime(t.SentDate), t.AssignedTo } into gt
                                     select gt).ToList();

                foreach (var day in groupedDeliveries)
                {
                    var dailyDelivery = new DailyDelivery
                    {
                        ReferenceDate = (DateTime)day.Key.Date,
                        CreatedTicketsCount = day.Count(),
                        CreatedTicketsFirstTimeCount = day.Count(t => t.FirstInvoice),
                        SentCount = day.Count(t => t.DocumentStatus == 30),
                        DeliveredCount = day.Count(t => t.DocumentStatus == 40),
                        OtherCount = day.Count(t => (t.DocumentStatus != 30 && t.DocumentStatus != 40)),
                        AssignedToCount = day.Count(t => t.IsAssigned),
                        AssignedTo = day.Key.AssignedTo
                    };
                    deliveries.Add(dailyDelivery);
                }
            }
            else
            {
                createdTickets = createdTickets.Where(t => t.InsertDate >= DateTime.Today);
                createdTicketsFirst = createdTicketsFirst.Where(t => t.InsertDate >= DateTime.Today);

                foreach (var day in groupedDeliveries)
                {
                    var dailyDelivery = new DailyDelivery
                    {
                        ReferenceDate = (DateTime)day.Key.Date,
                        CreatedTicketsCount = day.Count(),
                        CreatedTicketsFirstTimeCount = day.Count(t => t.FirstInvoice),
                        SentCount = day.Count(t => t.DocumentStatus == 30),
                        DeliveredCount = day.Count(t => t.DocumentStatus == 40),
                        OtherCount = day.Count(t => (t.DocumentStatus != 30 && t.DocumentStatus != 40)),
                        AssignedToCount = day.Count(t => t.IsAssigned),
                        AssignedTo = day.Key.AssignedTo
                    };
                    deliveries.Add(dailyDelivery);
                }  
            }

            var model = new DeliveryStatsViewModel
            {
                CreatedTicketsTodayCount = createdTickets.Count(),
                CreatedTicketsTodayFirstTimeCount = createdTicketsFirst.Count(),
                Deliveries = deliveries.AsQueryable()
            };

            var date = new DateTime(2017, 7, 1);
            ViewBag.TotalOpenedTickets = (from t in _db.DeliveryTicketModels
                                          where t.IsAssigned == false && t.InsertDate >= date && t.DocumentStatus == 30
                                          select t).Count();

            return View(model);
        }

        // GET: Stats/OrganizationsByCountry
        public ActionResult OrganizationsByCountry()
        {
            var model = new List<OrganizationsByCountryViewModel>();

            var organizations = _db.OrganizationDetails.Where(x => x.Organization.SubjectBusinessUnit == "" || x.Organization.SubjectBusinessUnit == "11").GroupBy(x => x.MainCountry).Select(gx => gx);
            var organizationCount = _db.OrganizationDetails.Count(x => x.Organization.SubjectBusinessUnit == "" || x.Organization.SubjectBusinessUnit == "11");

            foreach (var organization in organizations)
            {
                var tempModel = new OrganizationsByCountryViewModel()
                {
                    Country = organization.Key,
                    NumberOfOrganizations = organization.Count(),
                    PercentOfOrganizations = Math.Round(organization.Count() / (decimal)organizationCount * 100, 2)
                };
                model.Add(tempModel);
            }

            return View(model.OrderByDescending(x => x.NumberOfOrganizations).AsQueryable());
        }

        // GET: Stats/OpportunityEntryChannel
        public ActionResult OpportunityEntryChannel(string startDate, string endDate)
        {
            var model = new List<OpportunityEntryChannelViewModel>();

            var opportunities = _db.Opportunities.Where(x => x.OpportunityEntryChannel != null).GroupBy(x => x.OpportunityEntryChannel).Select(gx => gx);

            if (!String.IsNullOrEmpty(startDate))
            {
                var startDatePlus = Convert.ToDateTime(startDate);
                var endDatePlus = Convert.ToDateTime(endDate).AddDays(1);
                opportunities = _db.Opportunities.Where(x => x.OpportunityEntryChannel != null && x.InsertDate >= startDatePlus && x.InsertDate < endDatePlus).GroupBy(x => x.OpportunityEntryChannel).Select(gx => gx);
            }
            var opportunitiesCount = _db.Opportunities.Count(x => x.OpportunityEntryChannel != null);

            foreach (var opportunity in opportunities)
            {
                if (opportunity.Key != null)
                {
                    var tempModel = new OpportunityEntryChannelViewModel()
                    {
                        OpportunityEntryChannel = (Opportunity.OpportunityEntryChannelEnum)opportunity.Key,
                        NumberOfOpportunities = opportunity.Count(),
                        PercentOfOpportunities = Math.Round(opportunity.Count() / (decimal)opportunitiesCount * 100, 2)
                    };
                    model.Add(tempModel);
                }
            }

            return View(model.OrderByDescending(x => x.NumberOfOpportunities).AsQueryable());
        }

        // GET: Stats/OrganizationsForMeeting
        public ActionResult OrganizationsForMeeting()
        {
            var entities = _db.Opportunities.Where(o => o.OpportunityStatus == Opportunity.OpportunityStatusEnum.Arrangemeeting).Distinct();

            return View(entities);
        }

        // GET: Stats/AcquireEmailPaymentStat
        public ActionResult AcquireEmailPaymentStat(Campaign.CampaignStatusEnum? campaignStatus)
        {
            var entities = _db.Campaigns.Where(c => c.CampaignType == Campaign.CampaignTypeEnum.EmailBases);
            var sumTotalAmount = 0.00;

            if (campaignStatus != null)
            {
                entities = entities.Where(c => c.CampaignStatus == campaignStatus);
            }

            var models = new List<AcquireEmailPaymentStatTempViewModel>();

            foreach (var campaign in entities)
            {
                var count = _db.AcquireEmails.Count(ae => ae.RelatedCampaignId == campaign.CampaignId && ae.IsNewlyAcquired == true);

                var tempModel = new AcquireEmailPaymentStatTempViewModel()
                {
                    CampaignName = campaign.CampaignName,
                    CampaignId = campaign.CampaignId,
                    IsNewlyAcquiredCount = count,
                    TotalAmount = (count * 1.49) + 19.99
                };
                models.Add(tempModel);
                sumTotalAmount += tempModel.TotalAmount;
            }

            var model = new AcquireEmailPaymentStatViewModel()
            {
                List = models.AsQueryable(),
                SumTotalAmount = Math.Round((decimal)sumTotalAmount, 2)
            };

            return View(model);
        }


        // GET: Stats/Sales
        public ActionResult SalesStat(string agent, string searchDateStart, string searchDateEnd)
         {
             var agents = from u in _db.Users
                            select u;
             var leads = from u in _db.Leads
                          where u.IsAssigned
                          select u;
             var opportunities = from u in _db.Opportunities
                                  where u.IsAssigned
                                  select u;
 
             var assignedOpportunities = _db.Opportunities.Where(s => s.IsAssigned).GroupBy(d => d.AssignedTo)
                 .Select(d => new SaleAgentGrouping
                 {
                    Name = d.Key,
                    Count = d.Count()
                 });
 
             var assignedLeads = _db.Leads.Where(s => s.IsAssigned).GroupBy(d => d.AssignedTo)
                .Select(d => new SaleAgentGrouping
                {
                    Name = d.Key,
                    Count = d.Count()
                });
 
 
             if(!String.IsNullOrEmpty(searchDateStart) && !String.IsNullOrEmpty(searchDateEnd))
             {
               /* 
              
               _Leads = _Leads.Where(t => t.InsertDate=>)
                  _Opportunities = 
 
                  assignedOpportunities = 
 
                  assignedLeads =
                  
              */
             }
             if (String.IsNullOrEmpty(searchDateStart) && String.IsNullOrEmpty(searchDateEnd))
             {
 
             }
             if (!String.IsNullOrEmpty(searchDateStart) && String.IsNullOrEmpty(searchDateEnd))
             {
 
             }
             if (String.IsNullOrEmpty(searchDateStart) && !String.IsNullOrEmpty(searchDateEnd))
             {
 
             }
 
             var salesStat = new SalesStatsViewModel
             {
                 Leads = leads,
                 Opportunities = opportunities,
                 SumAssignedOpportunities = assignedOpportunities,
                 SumAssignedLeads=assignedLeads,
                 Agents = agents,
             };
 
             return View(salesStat);
 
         }
    }
}