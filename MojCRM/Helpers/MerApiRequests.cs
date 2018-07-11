using System;
using Newtonsoft.Json;

namespace MojCRM.Helpers
{
    public class MerApiRequest
    {
        [JsonProperty]
        public string Id { get; set; }
        public string Pass { get; set; }
        public string Oib { get; set; }
        public string PJ { get; set; }
        public string SoftwareId { get; set; }
    }

    public class MerApiResend : MerApiRequest
    {
        [JsonProperty]
        public int DocumentId { get; set; }
    }

    public class MerApiChangeEmail : MerApiRequest
    {
        [JsonProperty]
        public int DocumentId { get; set; }
        public string Email { get; set; }
    }

    public class MerApiGetSubjekt : MerApiRequest
    {
        [JsonProperty]
        public string SubjektOib { get; set; }
        public string SubjektPJ { get; set; }
    }

    public class MerApiGetNondeliveredDocuments : MerApiRequest
    {
        [JsonProperty]
        public int Type { get; set; }
    }

    public class MerApiGetSentDocuments : MerApiRequest
    {
        [JsonProperty]
        public string SubjektOib { get; set; }
        public string SubjektPJ { get; set; }
        public int? Take { get; set; }
    }

    public class MerApiGetContracts : MerApiRequest
    {
        [JsonProperty]
        public DateTime From { get; set; }
    }

    public class MerApiGetInaDailyReport : MerApiRequest
    {
        [JsonProperty]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}