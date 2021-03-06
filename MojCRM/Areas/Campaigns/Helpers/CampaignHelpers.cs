﻿using System;
using Newtonsoft.Json;

namespace MojCRM.Areas.Campaigns.Helpers
{
    public class CampaignSearchHelper
    {
        public string Organization { get; set; }
        public string CampaignName { get; set; }
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
}