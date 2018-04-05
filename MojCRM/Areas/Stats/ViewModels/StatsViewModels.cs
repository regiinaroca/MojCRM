using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MojCRM.Areas.Sales.Models;
using MojCRM.Models;

namespace MojCRM.Areas.Stats.ViewModels
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
        public IQueryable<DailyDelivery> Deliveries { get; set; }
        public IQueryable<SelectListItem> Agents
        {
            get
            {
                var agents = from u in db.Users
                              select new SelectListItem
                              {
                                  Text = u.UserName,
                                  Value = u.UserName
                              };
                return agents;
            }
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
        [Display(Name = "Uspješni pozivi (sumnjivi)")]
        public int NumberSuccessfulCallsSuspicious { get; set; }
        [Display(Name = "Broj neuspješnih poziva")]
        public int NumberUnsuccessfulCalls { get; set; }
        [Display(Name = "Neuspješni pozivi (sumnjivi)")]
        public int NumberUnsuccessfulCallsSuspicious { get; set; }
        [Display(Name = "Broj ispravaka mailova")]
        public int? NumberMailchange { get; set; }
        [Display(Name = "Broj ponovno poslanih obavijesti o dostavi")]
        public int? NumberResend { get; set; }
        [Display(Name = "Broj poslanih e-mailova vezanih za dostavu")]
        public int? NumberMail { get; set; }
        [Display(Name = "Broj zaključanih kartica (Odjel dostave)")]
        public int? NumberTicketsAssigned { get; set; }
        [Display(Name = "Broj prikupljenih e-mail adresa")]
        public int? NumberAcquiredEmails { get; set; }
        [Display(Name = "Broj prikupljenih kontakt podataka")]
        public int? NumberAcquiredTelephoneNumbers { get; set; }
        [Display(Name = "Vrijeme od zadnjeg poziva")]
        public int? TimeFromLastCall { get; set; }
    }
    public class CallCenterDailyByDepartment
    {
        [Display(Name = "Odjel")]
        public ActivityLog.DepartmentEnum Department { get; set; }
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
                    case ActivityLog.DepartmentEnum.MojCrm: return "Moj-CRM";
                    case ActivityLog.DepartmentEnum.Delivery: return "Odjel dostave eRačuna";
                    case ActivityLog.DepartmentEnum.Sales: return "Odjel prodaje";
                    case ActivityLog.DepartmentEnum.DatabaseUpdate: return "Odjel prikupa e-mail adresa";
                }
                return "Odjel";
            }
        }
    }
    public class CallCenterDailyStatsViewModel
    {
        public IQueryable<CallCenterDaily> Activities { get; set; }
        public IQueryable<CallCenterDailyByDepartment> ActivitiesByDepartment { get; set; }
        [Display(Name = "Uspješni pozivi")]
        public int SumSuccessfulCalls { get; set; }
        [Display(Name = "Uspješni pozivi (sumnjivi)")]
        public int SumSuccessfulCallsSuspicious { get; set; }
        [Display(Name = "Neuspješni pozivi")]
        public int SumUnsuccessfulCalls { get; set; }
        [Display(Name = "Neuspješni pozivi (sumnjivi)")]
        public int SumUnsuccessfulCallsSuspicious { get; set; }
        [Display(Name = "Poslani e-mailovi vezani za dostavu")]
        public int? SumSentMail { get; set; }
        [Display(Name = "Ispravci e-mailova (Odjel dostave)")]
        public int? SumMailchange { get; set; }
        [Display(Name = "Ponovno poslane obavijesti o dostavi (Odjel dostave)")]
        public int? SumResend { get; set; }
        [Display(Name = "Zaključane kartice (Odjel dostave)")]
        public int? SumTicketsAssigned { get; set; }
        [Display(Name = "Broj prikupljenih e-mail adresa")]
        public int? SumAcquiredEmails { get; set; }
        [Display(Name = "Broj prikupljenih kontakt podataka")]
        public int? SumAcquiredTelephoneNumbers { get; set; }

        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public IQueryable<CallCenterDaily> GetActivitiesForDashboard()
        {
            var referenceDate = DateTime.Now.Date;
            var activities = new List<CallCenterDaily>();
            var successfulCalls = _db.ActivityLogs.Where(a => (a.InsertDate >= referenceDate) && (a.ActivityType == ActivityLog.ActivityTypeEnum.Succall || a.ActivityType == ActivityLog.ActivityTypeEnum.Succalshort));
            var unsuccessfulCalls = _db.ActivityLogs.Where(t => (t.InsertDate >= referenceDate) && (t.ActivityType == ActivityLog.ActivityTypeEnum.Unsuccal));
            var mailChange = _db.ActivityLogs.Where(t => (t.InsertDate >= referenceDate) && (t.ActivityType == ActivityLog.ActivityTypeEnum.Mailchange));
            var resend = _db.ActivityLogs.Where(t => (t.InsertDate >= referenceDate) && (t.ActivityType == ActivityLog.ActivityTypeEnum.Resend));
            var deliveryMail = _db.ActivityLogs.Where(t => (t.InsertDate >= referenceDate) && (t.ActivityType == ActivityLog.ActivityTypeEnum.Email));
            var ticketsAssigned = _db.ActivityLogs.Where(t => (t.InsertDate >= referenceDate) && (t.ActivityType == ActivityLog.ActivityTypeEnum.Ticketassign));

            var agentActivities = (from a in _db.ActivityLogs
                               where (a.InsertDate >= referenceDate)
                               group a by a.User into ga
                               select ga).ToList();
            foreach (var day in agentActivities)
            {
                var dailyActivities = new CallCenterDaily
                {
                    Agent = day.Key,
                    NumberSuccessfulCalls = successfulCalls.Count(a => a.User == day.Key),
                    NumberUnsuccessfulCalls = unsuccessfulCalls.Count(a => a.User == day.Key),
                    NumberMailchange = mailChange.Count(a => a.User == day.Key),
                    NumberResend = resend.Count(a => a.User == day.Key),
                    NumberMail = deliveryMail.Count(a => a.User == day.Key),
                    NumberTicketsAssigned = ticketsAssigned.Count(a => a.User == day.Key),
                    TimeFromLastCall = (int)DateTime.Now.Subtract(GetLastCallDateTime(day.Key)).TotalMinutes
                };
                activities.Add(dailyActivities);
            }

            return activities.AsQueryable();
        }

        private DateTime GetLastCallDateTime(string agent)
        {
            var result = _db.ActivityLogs.OrderByDescending(x => x.Id).First(x =>
                x.User == agent && (x.ActivityType == ActivityLog.ActivityTypeEnum.Succall ||
                                    x.ActivityType == ActivityLog.ActivityTypeEnum.Succalshort));

            if (result != null) return result.InsertDate;
            return DateTime.Now;
        }
    }

    public class CallCenterWeekly
    {
        [Display(Name = "Agent")]
        public string Agent { get; set; }
        [Display(Name = "Broj uspješnih poziva")]
        public int NumberSuccessfulCalls { get; set; }
        [Display(Name = "Uspješni pozivi (sumnjivi)")]
        public int NumberSuccessfulCallsSuspicious { get; set; }
        [Display(Name = "Broj neuspješnih poziva")]
        public int NumberUnsuccessfulCalls { get; set; }
        [Display(Name = "Neuspješni pozivi (sumnjivi)")]
        public int NumberUnsuccessfulCallsSuspicious { get; set; }
        [Display(Name = "Broj ispravaka mailova")]
        public int? NumberMailchange { get; set; }
        [Display(Name = "Broj ponovno poslanih obavijesti o dostavi")]
        public int? NumberResend { get; set; }
        [Display(Name = "Broj poslanih e-mailova vezanih za dostavu")]
        public int? NumberMail { get; set; }
        [Display(Name = "Broj zaključanih kartica (Odjel dostave)")]
        public int? NumberTicketsAssigned { get; set; }
        [Display(Name = "Broj prikupljenih e-mail adresa")]
        public int? NumberAcquiredEmails { get; set; }
        [Display(Name = "Broj prikupljenih kontakt podataka")]
        public int? NumberAcquiredTelephoneNumbers { get; set; }
    }
    public class CallCenterWeeklyByDepartment
    {
        [Display(Name = "Odjel")]
        public ActivityLog.DepartmentEnum Department { get; set; }
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
                    case ActivityLog.DepartmentEnum.MojCrm: return "Moj-CRM";
                    case ActivityLog.DepartmentEnum.Delivery: return "Odjel dostave eRačuna";
                    case ActivityLog.DepartmentEnum.Sales: return "Odjel prodaje";
                    case ActivityLog.DepartmentEnum.DatabaseUpdate: return "Odjel prikupa e-mail adresa";
                }
                return "Odjel";
            }
        }
    }
    public class CallCenterWeeklyStatsViewModel
    {
        public IQueryable<CallCenterWeekly> Activities { get; set; }
        public IQueryable<CallCenterWeeklyByDepartment> ActivitiesByDepartment { get; set; }
        [Display(Name = "Uspješni pozivi")]
        public int SumSuccessfulCalls { get; set; }
        [Display(Name = "Uspješni pozivi (sumnjivi)")]
        public int SumSuccessfulCallsSuspicious { get; set; }
        [Display(Name = "Neuspješni pozivi")]
        public int SumUnsuccessfulCalls { get; set; }
        [Display(Name = "Neuspješni pozivi (sumnjivi)")]
        public int SumUnsuccessfulCallsSuspicious { get; set; }
        [Display(Name = "Poslani e-mailovi vezani za dostavu")]
        public int? SumSentMail { get; set; }
        [Display(Name = "Ispravci e-mailova (Odjel dostave)")]
        public int? SumMailchange { get; set; }
        [Display(Name = "Ponovno poslane obavijesti o dostavi (Odjel dostave)")]
        public int? SumResend { get; set; }
        [Display(Name = "Zaključane kartice (Odjel dostave)")]
        public int? SumTicketsAssigned { get; set; }
        [Display(Name = "Broj prikupljenih e-mail adresa")]
        public int? SumAcquiredEmails { get; set; }
        [Display(Name = "Broj prikupljenih kontakt podataka")]
        public int? SumAcquiredTelephoneNumbers { get; set; }
    }

    public class PersonalDailyActivitiesViewModel
    {
        public IQueryable<ActivityLog> PersonalActivities { get; set; }
        public IQueryable<ApplicationUser> Agents { get; set; }
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
        [Display(Name = "Ukupno prikupljenih e-mail adresa (ažuriranje baza)")]
        public int? SumAcquiredEmails { get; set; }
        [Display(Name = "Ukupno prikupljenih brojeva telefona (ažuriranje baza)")]
        public int? SumAcquiredPhoneNumbers { get; set; }
        public IList<SelectListItem> AgentList
        {
            get
            {
                var listAgents = (from u in Agents
                                  select new SelectListItem()
                                  {
                                      Text = u.UserName,
                                      Value = u.UserName
                                  }).ToList();
                return listAgents;
            }
        }
    }

    public class GeneralCampaignStatusViewModel
    {
        public int RelatedCampaignId { get; set; }
        public string RelatedCampaignName { get; set; }
        public int NumberOfOpportunitiesCreated { get; set; }
        public int NumberOfOpportunitiesInProgress { get; set; }
        public decimal NumberOfOpportunitiesInProgressPercent { get; set; }
        public int NumberOfOpportunitesUser { get; set; }
        public decimal NumberOfOpportunitiesUserPercent { get; set; }
        public int NumberOfOpportunitiesToLead { get; set; }
        public decimal NumberOfOpportunitiesToLeadPercent { get; set; }
        public int NumberOfOpportunitiesRejected { get; set; }
        public decimal NumberOfOpportunitiesRejectedPercent { get; set; }
        public int NumberOfLeadsCreated { get; set; }
        public int NumberOfLeadsInProgress { get; set; }
        public decimal NumberOfLeadsInProgressPercent { get; set; }
        public int NumberOfLeadsMeetings { get; set; }
        public decimal NumberOfLeadsMeetingsPercent { get; set; }
        public int NumberOfLeadsQuotes { get; set; }
        public decimal NumberOfLeadsQuotesPercent { get; set; }
        public int NumberOfLeadsRejected { get; set; }
        public decimal NumberOfLeadsRejectedPercent { get; set; }
        public int NumberOfLeadsAccepted { get; set; }
        public decimal NumberOfLeadsAcceptedPercent { get; set; }
    }
    public class GeneralCampaignStatusViewModelCount
    {
        public int NumberOfOpportunitiesCreated { get; set; }
        public int NumberOfOpportunitiesInProgress { get; set; }
        public int NumberOfOpportunitiesUser { get; set; }
        public int NumberOfOpportunitiesToLead { get; set; }
        public int NumberOfOpportunitiesRejected { get; set; }
        public int NumberOfLeadsCreated { get; set; }
        public int NumberOfLeadsInProgress { get; set; }
        public int NumberOfLeadsMeetings { get; set; }
        public int NumberOfLeadsQuotes { get; set; }
        public int NumberOfLeadsRejected { get; set; }
        public int NumberOfLeadsAccepted { get; set; }
    }

    public class SalesStatsViewModel
    {
            public IQueryable<Lead> Leads { get; set; }
            public IQueryable<Opportunity> Opportunities { get; set; }
            public IQueryable<ApplicationUser> Agents { get; set; }
            [Display(Name = "Broj prodajnih prilika")]
            public IQueryable<SaleAgentGrouping> SumAssignedOpportunities { get; set; }
            [Display(Name = "Broj lead-ova")]
            public IQueryable<SaleAgentGrouping> SumAssignedLeads { get; set; }
    
    
            public IQueryable<SelectListItem> AgentList
            {
                get
                {
                    var listAgents = from u in Agents
                        select new SelectListItem
                        {
                            Text = u.UserName,
                            Value = u.UserName
                        };

                return listAgents;
                }
            }
    }
    public class SaleAgentGrouping
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    #region OrganizationStats

    public class OrganizationsByCountryViewModel
    {
        public OrganizationDetail.CountryIdentificationCodeEnum Country { get; set; }
        public int NumberOfOrganizations { get; set; }
        public decimal PercentOfOrganizations { get; set; }
        public string CountryIdentificationCode
        {
            get
            {
                switch (Country)
                {
                    case OrganizationDetail.CountryIdentificationCodeEnum.Noinfo: return "Nema podatka";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Hr: return "Hrvatska";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Si: return "Slovenija";
                    case OrganizationDetail.CountryIdentificationCodeEnum.At: return "Austrija";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Pl: return "Poljska";
                    case OrganizationDetail.CountryIdentificationCodeEnum.It: return "Italija";
                    case OrganizationDetail.CountryIdentificationCodeEnum.De: return "Njemačka";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Be: return "Belgija";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Bg: return "Bugarska";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Cy: return "Cipar";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Cz: return "Češka";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Dk: return "Danska";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Ee: return "Estonija";
                    case OrganizationDetail.CountryIdentificationCodeEnum.El: return "Grčka";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Es: return "Španjolska";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Fi: return "Finska";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Fr: return "Francuska";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Gb: return "Ujedinjena Kraljevina";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Hu: return "Mađarska";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Ie: return "Irska";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Lt: return "Litva";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Lu: return "Luksemburg";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Lv: return "Latvija";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Mt: return "Malta";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Nl: return "Nizozemska";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Pt: return "Portugal";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Ro: return "Rumunjska";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Se: return "Švedska";
                    case OrganizationDetail.CountryIdentificationCodeEnum.Sk: return "Slovačka";
                }
                return "Nema podatka";
            }
        }
    }
    #endregion

    #region SalesStats

    public class OpportunityEntryChannelViewModel
    {
        public Opportunity.OpportunityEntryChannelEnum OpportunityEntryChannel { get; set; }
        public int NumberOfOpportunities { get; set; }
        public decimal PercentOfOpportunities { get; set; }
        public string OpportunityEntryChannelString
        {
            get
            {
                switch (OpportunityEntryChannel)
                {
                    case Opportunity.OpportunityEntryChannelEnum.Web: return "Web-forma";
                    case Opportunity.OpportunityEntryChannelEnum.InfoTelephone: return "Info telefon";
                    case Opportunity.OpportunityEntryChannelEnum.InfoMail: return "Info email";
                }
                return "Nije poznato";
            }
        }
    }

    public class CampaignLeadsAgentEfficiency
    {
        public string Agent { get; set; }
        public int NumberOfOpportunitiesTotal { get; set; }
        public int AssignedTotalCount { get; set; }
        public decimal ConverionPercent { get; set; }
        public int AcceptedCount { get; set; }
        public decimal AcceptedPercent { get; set; }
        public int RejectedCount { get; set; }
        public decimal RejectedPercent { get; set; }
    }

    #endregion
}