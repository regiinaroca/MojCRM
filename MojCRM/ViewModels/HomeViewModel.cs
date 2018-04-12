using MojCRM.Areas.Campaigns.ViewModels;
using MojCRM.Areas.Campaigns.Models;
using MojCRM.Areas.Stats.ViewModels;
using System.Linq;

namespace MojCRM.ViewModels
{
    public class HomeViewModel
    { 
        //public GeneralCampaignStatusViewModel INACampaign { get; set; }
        //public IQueryable<EmailBasesCampaignStatsViewModel> Campaigns { get; set; }
        public IQueryable<CampaignMember> CampaignMembers { get; set; }
        public IQueryable<CallCenterDaily> AgentActivities { get; set; }
    }
}