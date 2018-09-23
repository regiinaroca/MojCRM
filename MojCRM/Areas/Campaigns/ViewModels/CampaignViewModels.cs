using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MojCRM.Areas.Campaigns.Helpers;
using MojCRM.Areas.Campaigns.Models;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Areas.Sales.Models;
using MojCRM.Areas.Stats.ViewModels;
using MojCRM.Models;

namespace MojCRM.Areas.Campaigns.ViewModels
{
    public class CampaignDetailsViewModel
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public Campaign Campaign { get; set; }
        public EmailBasesCampaignStatsViewModel EmailBasesStats { get; set; }
        public SalesCampaignStatsViewModel SalesStats { get; set; }
        public int NumberOfUnassignedEntities { get; set; }
        public int NumberOfUnassignedEntitiesWithoutTelephone { get; set; }
        public IQueryable<CampaignMember> AssignedMembers { get; set; }
        public IQueryable<CampaignAssignedAgents> AssignedAgents { get; set; }
        public IQueryable<CampaignStatusHelper> EmailsBasesEntityStatusStats { get; set; }
        public IQueryable<CampaignStatusHelper> SalesOpportunitiesStatusStats { get; set; }
        public IQueryable<CampaignStatusHelper> SalesLeadsStatusStats { get; set; }
        public GeneralCampaignStatusViewModel SalesGeneralStatus { get; set; }
        public IQueryable<CampaignLeadsAgentEfficiency> CampaignLeadsAgentEfficiencies { get; set; }
        public string CampaignAttributes { get; set; }
        public int? NumberOfNewlyAcquiredReceivingInformation { get; set; }

        public IQueryable<SelectListItem> CampaignStatusList
        {
            get
            {
                var statusList = new List<SelectListItem>
                {
                    new SelectListItem {Value = null, Text = @"-- Odaberi status kampanje --"},
                    new SelectListItem {Value = "1", Text = @"U tijeku"},
                    new SelectListItem {Value = "2", Text = @"Privremeno zaustavljeno"},
                    //new SelectListItem {Value = "3", Text = @"Prekinuto"},
                    new SelectListItem {Value = "5", Text = @"Baza za tipsku"},
                    new SelectListItem {Value = "4", Text = @"Završeno"}
                };
                return statusList.AsQueryable();
            }
        }

        public IQueryable<SelectListItem> AcquireEmailEntityStatusList
        {
            get
            {
                var statusList = new List<SelectListItem>
                {
                    new SelectListItem{ Value = null, Text = @"-- Odaberi status obrade--"},
                    new SelectListItem{ Value= @"0", Text = @"Kreirano"},
                    new SelectListItem{ Value= @"1", Text = @"Dobivena povratna informacija"},
                    new SelectListItem{ Value= @"2", Text = @"Nema odgovora / Ne javlja se"},
                    new SelectListItem{ Value= @"4", Text = @"Ne posluju s korisnikom"},
                    new SelectListItem{ Value= @"13", Text = @"POŠTA"},
                    new SelectListItem{ Value= @"5", Text = @"Partner će se javiti korisniku samostalno"},
                    new SelectListItem{ Value= @"6", Text = @"Potrebno poslati pisanu suglasnost"},
                    new SelectListItem{ Value= @"7", Text = @"Neispravan kontakt broj"},
                    new SelectListItem{ Value= @"3", Text = @"Zatvoren subjekt (sudski / obrtni registar)"},
                    new SelectListItem{ Value= @"12", Text = @"Najava brisanja subjekta"},
                    new SelectListItem{ Value= @"10", Text = @"Subjekt u stečaju / likvidaciji"},
                    new SelectListItem{ Value= @"11", Text = @"Subjekt nema žiro račun"},
                    //new SelectListItem{ Value= @"8", Text = @"PH"},
                    new SelectListItem{ Value= @"9", Text = @"Nema broja"},
                    new SelectListItem{ Value= @"14", Text = @"Inozemna tvrtka"},
                    new SelectListItem{ Value= @"15", Text = @"Tvrtka u mirovanju"},
                    new SelectListItem{ Value= @"16", Text = @"POŠTA PROVJERENO"},
                    new SelectListItem{ Value= @"17", Text = @"Ne javlja se, PSP"}
                };
                return statusList.AsQueryable();
            }
        }

        public IQueryable<SelectListItem> AcquireEmailStatusList
        {
            get
            {
                var statusList = new List<SelectListItem>
                {
                    new SelectListItem{ Value = null, Text = @"-- Odaberi status predmeta--"},
                    new SelectListItem{ Value= @"0", Text = @"Kreirano"},
                    new SelectListItem{ Value= @"1", Text = @"Provjereno"},
                    new SelectListItem{ Value= @"2", Text = @"Verificirano"}
                };
                return statusList.AsQueryable();
            }
        }

        public IQueryable<SelectListItem> MemberRoleList
        {
            get
            {
                var roleList = new List<SelectListItem>
                {
                    new SelectListItem {Value = null, Text = @"-- Odaberi rolu agenta u kampanji --"},
                    new SelectListItem {Value = "0", Text = @"Nositelj kampanje"},
                    new SelectListItem {Value = "1", Text = @"Nadzornik kampanje"},
                    new SelectListItem {Value = "2", Text = @"Član kampanje"}
                };
                return roleList.AsQueryable();
            }
        }
        public IQueryable<SelectListItem> AgentList
        {
            get
            {
                var agents = (from a in _db.Users
                              where a.Email != String.Empty
                              select a);

                var agentsList = new List<SelectListItem>();

                foreach (var agent in agents)
                {
                    agentsList.Add(new SelectListItem { Value = agent.UserName, Text = agent.UserName });
                }

                return agentsList.AsQueryable();
            }
        }

        public int GetUnassignedEntities(int campaignId)
        {
            var number = _db.AcquireEmails.Count(x => x.Campaign.CampaignId == campaignId && x.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.Created && x.IsAssigned == false);
            return number;
        }

        public int GetUnassignedEntitiesWithoutTelephone(int campaignId)
        {
            var number = _db.AcquireEmails.Count(x => x.Campaign.CampaignId == campaignId 
            && x.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.Created 
            && x.IsAssigned == false
            && (x.Organization.OrganizationDetail.TelephoneNumber == String.Empty || x.Organization.OrganizationDetail.TelephoneNumber == null)
            && (x.Organization.OrganizationDetail.MobilePhoneNumber == String.Empty || x.Organization.OrganizationDetail.MobilePhoneNumber == null));
            return number;
        }

        public IQueryable<CampaignAssignedAgents> GetAssignedAgentsInfo(int campaignId)
        {
            var agents = _db.Users.Select(x => x.UserName);
            var model = new List<CampaignAssignedAgents>();

            foreach (var agent in agents)
            {
                var temp = new CampaignAssignedAgents()
                {
                    Agent = agent,
                    NumberOfAssignedEntities =
                        _db.AcquireEmails.Count(e => e.AssignedTo == agent && e.RelatedCampaignId == campaignId)
                };
                model.Add(temp);
            }
            return model.AsQueryable();
        }

        public IQueryable<CampaignStatusHelper> GetEmailBasesEntityStats(int campaignId)
        {
            var entites = _db.AcquireEmails.Where(x => x.Campaign.CampaignId == campaignId).GroupBy(x => x.AcquireEmailEntityStatus);
            var model = new List<CampaignStatusHelper>();

            foreach (var entity in entites)
            {
                string status;
                switch (entity.Key)
                {
                    case AcquireEmail.AcquireEmailEntityStatusEnum.Created:
                        status = "Kreirano";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.AcquiredInformation:
                        status = "Prikupljena povratna informacija";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.NoAnswer:
                        status = "Nema odgovora / Ne javlja se";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.ClosedOrganization:
                        status = "Zatvorena tvrtka";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.OldPartner:
                        status = "Ne poslujus s korisnikom";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.PartnerWillContactUser:
                        status = "Partner će se javiti korisniku samostalno";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.WrittenConfirmationRequired:
                        status = "Potrebno poslati pisanu suglasnost";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.WrongTelephoneNumber:
                        status = "Neispravan kontakt broj";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.PoslovnaHrvatska:
                        status = "Kontakt u bazi";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.NoTelehoneNumber:
                        status = "Ne postoji ispravan kontakt broj";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.Bankruptcy:
                        status = "Subjekt u stečaju / likvidaciji";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.NoFinancialAccount:
                        status = "Subjekt nema žiro račun";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.ToBeClosed:
                        status = "Najava brisanja subjekta";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.Post:
                        status = "Žele primati račune poštom";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.Foreign:
                        status = "Inozemna tvrtka";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.OnHold:
                        status = "Tvrtka u mirovanju";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.PostChecked:
                        status = "Pošta provjereno";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.NoAnswerOldPost:
                        status = "Ne javlja se, PSP";
                        break;
                    case AcquireEmail.AcquireEmailEntityStatusEnum.AcquiredInformationNoEmail:
                        status = "Prikupljena povratna informacija, ne žele obavijest";
                        break;
                    default:
                        status = "Status unosa";
                        break;
                }
                var temp = new CampaignStatusHelper()
                {
                    StatusName = status,
                    SumOfEntities = entity.Count()
                };
                model.Add(temp);
            }
            return model.AsQueryable();
        }

        public IQueryable<CampaignStatusHelper> GetOpportunitiesSalesStatusStats(int campaignId)
        {
            var entites = _db.Opportunities.Where(x => x.RelatedCampaignId == campaignId).GroupBy(x => x.OpportunityStatus);
            var model = new List<CampaignStatusHelper>();

            foreach (var entity in entites)
            {
                string status = GetOpportunityStatusString(entity.Key);
                var temp = new CampaignStatusHelper
                {
                    StatusName = status,
                    SumOfEntities = entity.Count()
                };
                model.Add(temp);
            }
            return model.AsQueryable();
        }

        public IQueryable<CampaignStatusHelper> GetLeadsSalesStatusStats(int campaignId)
        {
            var entites = _db.Leads.Where(x => x.RelatedCampaignId == campaignId).GroupBy(x => x.LeadStatus);
            var model = new List<CampaignStatusHelper>();

            foreach (var entity in entites)
            {
                string status;
                switch (entity.Key)
                {
                    case Lead.LeadStatusEnum.Start:
                        status = "Kreirano";
                        break;
                    case Lead.LeadStatusEnum.Incontact:
                        status = "U kontaktu";
                        break;
                    case Lead.LeadStatusEnum.Rejected:
                        status = "Odbijeno";
                        break;
                    case Lead.LeadStatusEnum.Quotesent:
                        status = "Poslana ponuda";
                        break;
                    case Lead.LeadStatusEnum.Accepted:
                        status = "Prihvaćena ponuda";
                        break;
                    case Lead.LeadStatusEnum.Meeting:
                        status = "Dogovoren sastanak";
                        break;
                    case Lead.LeadStatusEnum.Processdifficulties:
                        status = "Procesne poteškoće";
                        break;
                    default:
                        status = "Status leada";
                        break;
                }
                var temp = new CampaignStatusHelper
                {
                    StatusName = status,
                    SumOfEntities = entity.Count()
                };
                model.Add(temp);
            }
            return model.AsQueryable();
        }

        public GeneralCampaignStatusViewModel GetSalesGeneralStatus(int campaignId)
        {
            var opportunities = _db.Opportunities.Where(x => x.RelatedCampaignId == campaignId);
            var leads = _db.Leads.Where(x => x.RelatedCampaignId == campaignId);
            var campaign = _db.Campaigns.First(x => x.CampaignId == campaignId);

            var countModel = new GeneralCampaignStatusViewModelCount
            {
                NumberOfOpportunitiesCreated = opportunities.Count(),
                NumberOfOpportunitiesInProgress = opportunities.Count(o => o.OpportunityStatus == Opportunity.OpportunityStatusEnum.Start || o.OpportunityStatus == Opportunity.OpportunityStatusEnum.Arrangemeeting || o.OpportunityStatus == Opportunity.OpportunityStatusEnum.Incontact || o.OpportunityStatus == Opportunity.OpportunityStatusEnum.Processdifficulties),
                NumberOfOpportunitiesUser = opportunities.Count(o => o.OpportunityStatus == Opportunity.OpportunityStatusEnum.Meruser),
                NumberOfOpportunitiesToLead = opportunities.Count(o => o.OpportunityStatus == Opportunity.OpportunityStatusEnum.Lead),
                NumberOfOpportunitiesRejected = opportunities.Count(o => o.OpportunityStatus == Opportunity.OpportunityStatusEnum.Rejected),
                NumberOfLeadsCreated = leads.Count(),
                NumberOfLeadsInProgress = leads.Count(l => l.LeadStatus == Lead.LeadStatusEnum.Incontact || l.LeadStatus == Lead.LeadStatusEnum.Meeting || l.LeadStatus == Lead.LeadStatusEnum.Start),
                NumberOfLeadsMeetings = leads.Count(l => l.LeadStatus == Lead.LeadStatusEnum.Meeting),
                NumberOfLeadsQuotes = leads.Count(l => l.LeadStatus == Lead.LeadStatusEnum.Quotesent),
                NumberOfLeadsRejected = leads.Count(l => l.LeadStatus == Lead.LeadStatusEnum.Rejected),
                NumberOfLeadsAccepted = leads.Count(l => l.LeadStatus == Lead.LeadStatusEnum.Accepted)
            };

            decimal numberOfOpportunitiesInProgressPercent = 0, numberOfOpportunitiesUserPercent = 0, numberOfOpportunitiesToLeadPercent = 0, numberOfOpportunitiesRejectedPercent = 0,
                numberOfLeadsInProgressPercent = 0, numberOfLeadsMeetingsPercent = 0, numberOfLeadsQuotesPercent = 0, numberOfLeadsRejectedPercent = 0, numberOfLeadsAcceptedPercent = 0;

            if (opportunities.Count() != 0)
            {
                numberOfOpportunitiesInProgressPercent = Math.Round(
                    ((countModel.NumberOfOpportunitiesInProgress / (decimal) countModel.NumberOfOpportunitiesCreated) *
                     100), 2);
                numberOfOpportunitiesUserPercent =
                    Math.Round(
                        ((countModel.NumberOfOpportunitiesUser / (decimal) countModel.NumberOfOpportunitiesCreated) *
                         100), 2);
                numberOfOpportunitiesToLeadPercent =
                    Math.Round(
                        ((countModel.NumberOfOpportunitiesToLead / (decimal) countModel.NumberOfOpportunitiesCreated) *
                         100), 2);
                numberOfOpportunitiesRejectedPercent = Math.Round(
                    ((countModel.NumberOfOpportunitiesRejected / (decimal) countModel.NumberOfOpportunitiesCreated) *
                     100), 2);
            }
            if (leads.Count() != 0)
            {
                numberOfLeadsInProgressPercent =
                    Math.Round(((countModel.NumberOfLeadsInProgress / (decimal) countModel.NumberOfLeadsCreated) * 100),
                        2);
                numberOfLeadsMeetingsPercent =
                    Math.Round(((countModel.NumberOfLeadsMeetings / (decimal) countModel.NumberOfLeadsCreated) * 100),
                        2);
                numberOfLeadsQuotesPercent =
                    Math.Round(((countModel.NumberOfLeadsQuotes / (decimal) countModel.NumberOfLeadsCreated) * 100), 2);
                numberOfLeadsRejectedPercent =
                    Math.Round(((countModel.NumberOfLeadsRejected / (decimal) countModel.NumberOfLeadsCreated) * 100),
                        2);
                numberOfLeadsAcceptedPercent =
                    Math.Round(((countModel.NumberOfLeadsAccepted / (decimal) countModel.NumberOfLeadsCreated) * 100),
                        2);
            }

            var model = new GeneralCampaignStatusViewModel
            {
                RelatedCampaignId = 6,
                RelatedCampaignName = campaign.CampaignName,
                NumberOfOpportunitiesCreated = countModel.NumberOfOpportunitiesCreated,
                NumberOfOpportunitiesInProgress = countModel.NumberOfOpportunitiesInProgress,
                NumberOfOpportunitiesInProgressPercent = numberOfOpportunitiesInProgressPercent,
                NumberOfOpportunitesUser = countModel.NumberOfOpportunitiesUser,
                NumberOfOpportunitiesUserPercent = numberOfOpportunitiesUserPercent,
                NumberOfOpportunitiesToLead = countModel.NumberOfOpportunitiesToLead,
                NumberOfOpportunitiesToLeadPercent = numberOfOpportunitiesToLeadPercent,
                NumberOfOpportunitiesRejected = countModel.NumberOfOpportunitiesRejected,
                NumberOfOpportunitiesRejectedPercent = numberOfOpportunitiesRejectedPercent,
                NumberOfLeadsCreated = countModel.NumberOfLeadsCreated,
                NumberOfLeadsInProgress = countModel.NumberOfLeadsInProgress,
                NumberOfLeadsInProgressPercent = numberOfLeadsInProgressPercent,
                NumberOfLeadsMeetings = countModel.NumberOfLeadsMeetings,
                NumberOfLeadsMeetingsPercent = numberOfLeadsMeetingsPercent,
                NumberOfLeadsQuotes = countModel.NumberOfLeadsQuotes,
                NumberOfLeadsQuotesPercent = numberOfLeadsQuotesPercent,
                NumberOfLeadsRejected = countModel.NumberOfLeadsRejected,
                NumberOfLeadsRejectedPercent = numberOfLeadsRejectedPercent,
                NumberOfLeadsAccepted = countModel.NumberOfLeadsAccepted,
                NumberOfLeadsAcceptedPercent = numberOfLeadsAcceptedPercent
            };

            return model;
        }

        public IQueryable<CampaignLeadsAgentEfficiency> GetCampaignLeadsAgentEfficiencies(int campaignId)
        {
            var leads = _db.Leads.Where(l => l.RelatedCampaignId == campaignId)
                .GroupBy(o => o.AssignedTo);
            var model = new List<CampaignLeadsAgentEfficiency>();

            foreach (var lead in leads)
            {
                var opportunitiesCount = _db.Opportunities.Count(l => l.RelatedCampaignId == campaignId && l.AssignedTo == lead.Key);
                var totalCount =
                    _db.Leads.Count(l => l.RelatedCampaignId == campaignId && l.AssignedTo == lead.Key);
                var temp = new CampaignLeadsAgentEfficiency()
                {
                    Agent = lead.Key,
                    NumberOfOpportunitiesTotal = opportunitiesCount,
                    ConverionPercent = Math.Round(totalCount / (decimal)opportunitiesCount * 100, 2),
                    AssignedTotalCount = totalCount,
                    AcceptedCount = _db.Leads.Count(l => l.RelatedCampaignId == campaignId && l.AssignedTo == lead.Key && l.LeadStatus == Lead.LeadStatusEnum.Accepted),
                    AcceptedPercent = Math.Round(((_db.Leads.Count(l => l.RelatedCampaignId == campaignId && l.AssignedTo == lead.Key && l.LeadStatus == Lead.LeadStatusEnum.Accepted) / (decimal)totalCount) * 100), 2),
                    RejectedCount = _db.Leads.Count(l => l.RelatedCampaignId == campaignId && l.AssignedTo == lead.Key && l.LeadStatus == Lead.LeadStatusEnum.Rejected),
                    RejectedPercent = Math.Round(((_db.Leads.Count(l => l.RelatedCampaignId == campaignId && l.AssignedTo == lead.Key && l.LeadStatus == Lead.LeadStatusEnum.Rejected) / (decimal)totalCount) * 100), 2),
                };
                model.Add(temp);
            }
            return model.AsQueryable();
        }

        public string GetOpportunityStatusString(Opportunity.OpportunityStatusEnum status)
        {
            string statusString;
            switch (status)
            {
                case Opportunity.OpportunityStatusEnum.Start:
                    statusString = "Kreirano";
                    break;
                case Opportunity.OpportunityStatusEnum.Incontact:
                    statusString = "U kontaktu";
                    break;
                case Opportunity.OpportunityStatusEnum.Lead:
                    statusString = "Kreiran lead";
                    break;
                case Opportunity.OpportunityStatusEnum.Rejected:
                    statusString = "Odbijeno";
                    break;
                case Opportunity.OpportunityStatusEnum.Arrangemeeting:
                    statusString = "Potrebno dogovoriti sastanak";
                    break;
                case Opportunity.OpportunityStatusEnum.Processdifficulties:
                    statusString = "Procesne poteškoće";
                    break;
                case Opportunity.OpportunityStatusEnum.Meruser:
                    statusString = "Moj-eRačun korisnik";
                    break;
                case Opportunity.OpportunityStatusEnum.Finauser:
                    statusString = "FINA korisnik";
                    break;
                case Opportunity.OpportunityStatusEnum.EFakturauser:
                    statusString = "eFaktura korisnik";
                    break;
                default:
                    statusString = "Status prodajne prilike";
                    break;
            }
            return statusString;
        }
    }
}