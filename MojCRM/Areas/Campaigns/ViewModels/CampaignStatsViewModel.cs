using MojCRM.Areas.Campaigns.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MojCRM.Areas.Campaigns.Helpers;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Areas.Sales.Models;
using MojCRM.Models;

namespace MojCRM.Areas.Campaigns.ViewModels
{
    public class SalesCampaignStatsViewModel
    {
        public Campaign Campaign { get; set; }
        public int TotalCount { get; set; }
        public int StartedCount { get; set; }
        public int NotStartedCount { get; set; }
        public decimal NotStartedPercent { get; set; }
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public SalesCampaignStatsViewModel GetModel(int id)
        {
            var campaign = _db.Campaigns.First(c => c.CampaignType == Campaign.CampaignTypeEnum.Sales && c.CampaignId == id);

            if (_db.Opportunities.Count(op => op.RelatedCampaignId == id) != 0)
            {
                var newCampaign = new SalesCampaignStatsViewModel()
                {
                    Campaign = campaign,
                    TotalCount = _db.Opportunities.Count(op => op.RelatedCampaignId == campaign.CampaignId),
                    StartedCount = _db.Opportunities.Count(op => op.RelatedCampaignId == campaign.CampaignId && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.Start),
                    NotStartedCount = _db.Opportunities.Count(op => op.RelatedCampaignId == campaign.CampaignId && op.OpportunityStatus != Opportunity.OpportunityStatusEnum.Start),
                    NotStartedPercent = Math.Round(((_db.Opportunities.Count(op => op.RelatedCampaignId == campaign.CampaignId && op.OpportunityStatus != Opportunity.OpportunityStatusEnum.Start)
                                                     / (decimal)_db.Opportunities.Count(op => op.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
                };

                return newCampaign;
            }
            else
            {
                var newCampaign = new SalesCampaignStatsViewModel()
                {
                    Campaign = campaign,
                    TotalCount = 0,
                    StartedCount = 0,
                    NotStartedCount = 0,
                    NotStartedPercent = 0
                };
                return newCampaign;
            }
        }
    }

    public class EmailBasesCampaignStatsViewModel
    {
        public Campaign Campaign { get; set; }
        public int TotalCount { get; set; }
        public int NotVerifiedCount { get; set; }
        public decimal CreatedPercent { get; set; }
        public decimal CheckedPercent { get; set; }
        public decimal VerifiedPercent { get; set; }

        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public IQueryable<EmailBasesCampaignStatsViewModel> GetModels()
        {
            var campaignsTemp = _db.Campaigns.Where(c => c.CampaignType == Campaign.CampaignTypeEnum.EmailBases && c.CampaignStatus != Campaign.CampaignStatusEnum.Completed);
            var list = new List<EmailBasesCampaignStatsViewModel>();

            if (campaignsTemp.Count() != 0)
            {
                foreach (var campaign in campaignsTemp)
                {
                    var totalCount = _db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId);
                    if (totalCount != 0)
                    {
                        var newCampaign = new EmailBasesCampaignStatsViewModel()
                        {
                            Campaign = campaign,
                            TotalCount = totalCount,
                            NotVerifiedCount = _db.AcquireEmails.Count(a =>
                                a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus !=
                                AcquireEmail.AcquireEmailStatusEnum.Verified && a.AcquireEmailStatus != AcquireEmail.AcquireEmailStatusEnum.Reviewed),
                            CreatedPercent = Math.Round(((_db.AcquireEmails.Count(a =>
                                                              a.RelatedCampaignId == campaign.CampaignId &&
                                                              a.AcquireEmailStatus ==
                                                              AcquireEmail.AcquireEmailStatusEnum.Created)
                                                          / (decimal) _db.AcquireEmails.Count(a =>
                                                              a.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
                            CheckedPercent = Math.Round(((_db.AcquireEmails.Count(a =>
                                                              a.RelatedCampaignId == campaign.CampaignId &&
                                                              a.AcquireEmailStatus ==
                                                              AcquireEmail.AcquireEmailStatusEnum.Checked)
                                                          / (decimal) _db.AcquireEmails.Count(a =>
                                                              a.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
                            VerifiedPercent = Math.Round(((_db.AcquireEmails.Count(a =>
                                                               a.RelatedCampaignId == campaign.CampaignId &&
                                                               (a.AcquireEmailStatus ==
                                                               AcquireEmail.AcquireEmailStatusEnum.Verified || a.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.Reviewed))
                                                           / (decimal) _db.AcquireEmails.Count(a =>
                                                               a.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
                        };
                        list.Add(newCampaign);
                    }
                    else
                    {
                        var newCampaign = new EmailBasesCampaignStatsViewModel()
                        {
                            Campaign = campaign,
                            TotalCount = totalCount,
                            NotVerifiedCount = _db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus != AcquireEmail.AcquireEmailStatusEnum.Verified),
                            CreatedPercent = 0,
                            CheckedPercent = 0,
                            VerifiedPercent = 0,
                        };
                        list.Add(newCampaign);
                    }
                }
                return list.AsQueryable();
            }
            return list.AsQueryable();
        }
        public EmailBasesCampaignStatsViewModel GetModel(int id)
        {
            var campaign = _db.Campaigns.First(c => c.CampaignType == Campaign.CampaignTypeEnum.EmailBases && c.CampaignId == id);

            if (_db.AcquireEmails.Count(a => a.RelatedCampaignId == id) != 0)
            {
                var newCampaign = new EmailBasesCampaignStatsViewModel()
                {
                    Campaign = campaign,
                    TotalCount = _db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId),
                    NotVerifiedCount = _db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus != AcquireEmail.AcquireEmailStatusEnum.Verified),
                    CreatedPercent = Math.Round(((_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.Created)
                                                  / (decimal)_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
                    CheckedPercent = Math.Round(((_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.Checked)
                                                  / (decimal)_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
                    VerifiedPercent = Math.Round(((_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && (a.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.Verified || a.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.Reviewed))
                                                   / (decimal)_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
                };
                return newCampaign;
            }
            else
            {
                var newCampaign = new EmailBasesCampaignStatsViewModel()
                {
                    Campaign = campaign,
                    TotalCount = 0,
                    NotVerifiedCount = 0,
                    CreatedPercent = 0,
                    CheckedPercent = 0,
                    VerifiedPercent = 0
                };
                return newCampaign;
            }
        }
    }

    public class EmailBasesCampaignStatusStatsViewModel
    {
        public int StartedCount { get; set; }
        public int InProgressCount { get; set; }
        public int StartedTotalCount { get; set; }
        public int StartedCreatedCount { get; set; }
        public int InProgressTotalCount { get; set; }
        public int InProgressCreatedCount { get; set; }
        public decimal StartedPercent { get; set; }
        public decimal InProgressPercent { get; set; }
        public IQueryable<CampaignStatusHelper> StartedEntityStatusStats { get; set; }
        public IQueryable<CampaignStatusHelper> InProgressEntityStatusStats { get; set; }

        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public EmailBasesCampaignStatusStatsViewModel GetModels()
        {
            var model = new EmailBasesCampaignStatusStatsViewModel();
            var campaignBasesStats = new CampaignDetailsViewModel();

            var campaignsStarted = _db.Campaigns.Where(c => c.CampaignType == Campaign.CampaignTypeEnum.EmailBases && c.CampaignStatus == Campaign.CampaignStatusEnum.Start);
            var campaignsInProgress = _db.Campaigns.Where(c => c.CampaignType == Campaign.CampaignTypeEnum.EmailBases && c.CampaignStatus == Campaign.CampaignStatusEnum.InProgress);
            var startedTotalCount = _db.AcquireEmails.Count(x => x.Campaign.CampaignStatus == Campaign.CampaignStatusEnum.Start && x.Campaign.CampaignType == Campaign.CampaignTypeEnum.EmailBases);
            var startedCreatedCount = _db.AcquireEmails.Count(x => x.Campaign.CampaignType == Campaign.CampaignTypeEnum.EmailBases && x.Campaign.CampaignStatus == Campaign.CampaignStatusEnum.Start && x.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.Created);
            var inProgressTotalCount = _db.AcquireEmails.Count(x => x.Campaign.CampaignStatus == Campaign.CampaignStatusEnum.InProgress && x.Campaign.CampaignType == Campaign.CampaignTypeEnum.EmailBases);
            var inProgressCreatedCount = _db.AcquireEmails.Count(x => x.Campaign.CampaignType == Campaign.CampaignTypeEnum.EmailBases && x.Campaign.CampaignStatus == Campaign.CampaignStatusEnum.InProgress && x.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.Created);

            model.StartedCount = campaignsStarted.Count();
            model.InProgressCount = campaignsInProgress.Count();
            model.StartedTotalCount = startedTotalCount;
            model.StartedCreatedCount = startedCreatedCount;
            model.InProgressTotalCount = inProgressTotalCount;
            model.InProgressCreatedCount = inProgressCreatedCount;
            model.StartedPercent = Math.Round(startedCreatedCount / (decimal) startedTotalCount * 100, 0);
            model.InProgressPercent = Math.Round(inProgressCreatedCount / (decimal) inProgressTotalCount * 100, 0);
            model.StartedEntityStatusStats = campaignBasesStats.GetEmailBasesEntityStats(Campaign.CampaignStatusEnum.Start);
            model.InProgressEntityStatusStats = campaignBasesStats.GetEmailBasesEntityStats(Campaign.CampaignStatusEnum.InProgress);

            return model;
        }
    }
}