using System;
using System.Linq;
using MojCRM.Areas.Campaigns.Models;
using MojCRM.Models;
using Newtonsoft.Json;

namespace MojCRM.Areas.Campaigns.Helpers
{
    public class CampaignSearchHelper
    {
        public string Organization { get; set; }
        public string CampaignName { get; set; }
        public string ContractStartDate { get; set; }
        public int? CampaignStatus { get; set; }
        public int? CampaignType { get; set; }
    }

    public class CampaignAssignedAgents
    {
        public string Agent { get; set; }
        public int NumberOfAssignedEntities { get; set; }
    }

    public class CampaignStatusHelper
    {
        public string StatusName { get; set; }
        public int SumOfEntities { get; set; }
    }

    public class CampaignAttributeJsonHelper
    {
        [JsonProperty]
        public string Attribute { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class CampaignHelperMethods
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        /// Helper method which adds users as campaign members
        /// </summary>
        /// <param name="campaignId"></param>
        public void UpdateCampaignMembers(int campaignId)
        {
            var opportunities = _db.Opportunities.Where(c => c.RelatedCampaignId == campaignId).Select(c => c.AssignedTo).Distinct();
            var acquireEmails = _db.AcquireEmails.Where(c => c.RelatedCampaignId == campaignId).Select(c => c.AssignedTo).Distinct();

            foreach (var agent in opportunities)
            {
                if (!_db.CampaignMembers.Any(x => x.CampaignId == campaignId && x.MemberName == agent))
                {
                    _db.CampaignMembers.Add(new CampaignMember
                    {
                        CampaignId = campaignId,
                        MemberName = agent,
                        MemberRole = CampaignMember.MemberRoleEnum.Member,
                        InsertDate = DateTime.Now
                    });
                }
            }
            foreach (var agent in acquireEmails)
            {
                if (!_db.CampaignMembers.Any(x => x.CampaignId == campaignId && x.MemberName == agent))
                {
                    _db.CampaignMembers.Add(new CampaignMember
                    {
                        CampaignId = campaignId,
                        MemberName = agent,
                        MemberRole = CampaignMember.MemberRoleEnum.Member,
                        InsertDate = DateTime.Now
                    });
                }
            }
            _db.SaveChanges();
        }
    }
}