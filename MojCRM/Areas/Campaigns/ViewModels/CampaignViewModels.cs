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

        public IQueryable<SelectListItem> CampaignStatusList
        {
            get
            {
                var statusList = new List<SelectListItem>
                {
                    new SelectListItem {Value = null, Text = @"-- Odaberi status kampanje --"},
                    new SelectListItem {Value = "1", Text = @"U tijeku"},
                    new SelectListItem {Value = "2", Text = @"Privremeno zaustavljeno"},
                    new SelectListItem {Value = "3", Text = @"Prekinuto"},
                    new SelectListItem {Value = "4", Text = @"Završeno"}
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
                string status;
                switch (entity.Key)
                {
                    case Opportunity.OpportunityStatusEnum.Start:
                        status = "Kreirano";
                        break;
                    case Opportunity.OpportunityStatusEnum.Incontact:
                        status = "U kontaktu";
                        break;
                    case Opportunity.OpportunityStatusEnum.Lead:
                        status = "Kreiran lead";
                        break;
                    case Opportunity.OpportunityStatusEnum.Rejected:
                        status = "Odbijeno";
                        break;
                    case Opportunity.OpportunityStatusEnum.Arrangemeeting:
                        status = "Potrebno dogovoriti sastanak";
                        break;
                    case Opportunity.OpportunityStatusEnum.Processdifficulties:
                        status = "Procesne poteškoće";
                        break;
                    case Opportunity.OpportunityStatusEnum.Meruser:
                        status = "Moj-eRačun korisnik";
                        break;
                    case Opportunity.OpportunityStatusEnum.Finauser:
                        status = "FINA korisnik";
                        break;
                    case Opportunity.OpportunityStatusEnum.EFakturauser:
                        status = "eFaktura korisnik";
                        break;
                        default:
                            status = "Status prodajne prilike";
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

            var model = new GeneralCampaignStatusViewModel
            {
                RelatedCampaignId = 6,
                RelatedCampaignName = campaign.CampaignName,
                NumberOfOpportunitiesCreated = countModel.NumberOfOpportunitiesCreated,
                NumberOfOpportunitiesInProgress = countModel.NumberOfOpportunitiesInProgress,
                NumberOfOpportunitiesInProgressPercent = Math.Round(((countModel.NumberOfOpportunitiesInProgress / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
                NumberOfOpportunitesUser = countModel.NumberOfOpportunitiesUser,
                NumberOfOpportunitiesUserPercent = Math.Round(((countModel.NumberOfOpportunitiesUser / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
                NumberOfOpportunitiesToLead = countModel.NumberOfOpportunitiesToLead,
                NumberOfOpportunitiesToLeadPercent = Math.Round(((countModel.NumberOfOpportunitiesToLead / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
                NumberOfOpportunitiesRejected = countModel.NumberOfOpportunitiesRejected,
                NumberOfOpportunitiesRejectedPercent = Math.Round(((countModel.NumberOfOpportunitiesRejected / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
                NumberOfLeadsCreated = countModel.NumberOfLeadsCreated,
                NumberOfLeadsInProgress = countModel.NumberOfLeadsInProgress,
                NumberOfLeadsInProgressPercent = Math.Round(((countModel.NumberOfLeadsInProgress / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
                NumberOfLeadsMeetings = countModel.NumberOfLeadsMeetings,
                NumberOfLeadsMeetingsPercent = Math.Round(((countModel.NumberOfLeadsMeetings / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
                NumberOfLeadsQuotes = countModel.NumberOfLeadsQuotes,
                NumberOfLeadsQuotesPercent = Math.Round(((countModel.NumberOfLeadsQuotes / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
                NumberOfLeadsRejected = countModel.NumberOfLeadsRejected,
                NumberOfLeadsRejectedPercent = Math.Round(((countModel.NumberOfLeadsRejected / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
                NumberOfLeadsAccepted = countModel.NumberOfLeadsAccepted,
                NumberOfLeadsAcceptedPercent = Math.Round(((countModel.NumberOfLeadsAccepted / (decimal)countModel.NumberOfLeadsCreated) * 100), 2)
            };

            return model;
        }
    }
}