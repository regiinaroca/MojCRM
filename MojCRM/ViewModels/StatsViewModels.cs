using MojCRM.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static MojCRM.Models.ActivityLog;

namespace MojCRM.ViewModels
{
    public class DailyDelivery
    {
        public DateTime ReferenceDate { get; set; }
        public int CreatedTicketsCount { get; set; }
        public int CreatedTicketsFirstTimeCount { get; set; }
        public int AssignedToCount { get; set; }
        public int SentCount { get; set; }
        public int DeliveredCount { get; set; }
        public int OtherCount { get; set; }
        public string AssignedTo { get; set; }
    }
    public class DeliveryStatsViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int CreatedTicketsTodayCount { get; set; }
        public int CreatedTicketsTodayFirstTimeCount { get; set; }
        public IList<DailyDelivery> Deliveries { get; set; }
        public IList<SelectListItem> Agents
        {
            get
            {
                var agents = (from u in db.Users
                              select new SelectListItem()
                              {
                                  Text = u.UserName,
                                  Value = u.UserName
                              }).ToList();
                return agents;
            }
            set { }
        }
    }
    public class AssigningTickets
    {
        public DateTime? TicketDate { get; set; }
        public DateTime SentDate { get; set; }
        public string Agent { get; set; }
    }

    public class CallCenterDaily
    {
        [Display(Name = "Agent")]
        public string Agent { get; set; }
        [Display(Name = "Broj uspješnih poziva")]
        public int NumberSuccessfulCalls { get; set; }
        [Display(Name = "Broj neuspješnih poziva")]
        public int NumberUnsuccessfulCalls { get; set; }
        [Display(Name = "Broj ispravaka mailova")]
        public int? NumberMailchange { get; set; }
        [Display(Name = "Broj ponovno poslanih obavijesti o dostavi")]
        public int? NumberResend { get; set; }
        [Display(Name = "Broj poslanih e-mailova vezanih za dostavu")]
        public int? NumberDeliveryMail { get; set; }
    }
    public class CallCenterDailyByDepartment
    {
        [Display(Name = "Odjel")]
        public DepartmentEnum Department { get; set; }
        [Display(Name = "Zbroj uspješnih poziva")]
        public int NumberSuccessfulCalls { get; set; }
        [Display(Name = "Zbroj neuspješnih poziva")]
        public int NumberUnsuccessfulCalls { get; set; }
        [Display(Name = "Zbroj poslanih e-mailova korisnicima")]
        public int? NumberMail { get; set; }
        [Display(Name = "Zbroj ispravaka mailova (DOSTAVA)")]
        public int? NumberMailchange { get; set; }
        [Display(Name = "Zbroj ponovno poslanih obavijesti o dostavi (DOSTAVA)")]
        public int? NumberResend { get; set; }
        

        public string DepartmentString
        {
            get
            {
                switch (Department)
                {
                    case DepartmentEnum.MojCRM: return "Moj-CRM";
                    case DepartmentEnum.Delivery: return "Odjel dostave eRačuna";
                    case DepartmentEnum.Sales: return "Odjel prodaje";
                }
                return "Odjel";
            }
        }
    }
    public class CallCenterDailyStatsViewModel
    {
        public IList<CallCenterDaily> Activities { get; set; }
        public IList<CallCenterDailyByDepartment> ActivitiesByDepartment { get; set; }
        [Display(Name = "Zbroj uspješnih poziva")]
        public int SumSuccessfulCalls { get; set; }
        [Display(Name = "Zbroj neuspješnih poziva")]
        public int SumUnsuccessfulCalls { get; set; }
        [Display(Name = "Zbroj poslanih e-mailova vezanih za dostavu")]
        public int? SumSentMail { get; set; }
        [Display(Name = "Zbroj ispravaka mailova (DOSTAVA)")]
        public int? SumMailchange { get; set; }
        [Display(Name = "Zbroj ponovno poslanih obavijesti o dostavi (DOSTAVA)")]
        public int? SumResend { get; set; }
    }

    public class PersonalDailyActivitiesViewModel
    {
        public IList<ActivityLog> PersonalActivities { get; set; }
        public IEnumerable<ApplicationUser> Agents { get; set; }
        [Display(Name = "Ukupno uspješnih poziva")]
        public int SumSuccessfulCalls { get; set; }
        [Display(Name = "Ukupno uspješnih kratkih poziva")]
        public int SumShortSuccessfulCalls { get; set; }
        [Display(Name = "Ukupno neuspješnih poziva")]
        public int SumUnsuccessfulCalls { get; set; }
        [Display(Name = "Ukupno poslanih e-mailova korisnicima")]
        public int? SumSentMail { get; set; }
        [Display(Name = "Ukupno ispravaka mailova")]
        public int? SumMailchange { get; set; }
        [Display(Name = "Ukupno ponovno poslanih obavijesti o dostavi")]
        public int? SumResend { get; set; }
        public IList<SelectListItem> AgentList
        {
            get
            {
                var ListAgents = (from u in Agents
                                  select new SelectListItem()
                                  {
                                      Text = u.UserName,
                                      Value = u.UserName
                                  }).ToList();
                return ListAgents;
            }
            set { }
        }
    }
}