using System;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using static MojCRM.Models.Organizations;
using MojCRM.Models;
using static MojCRM.Models.OrganizationDetail;

namespace MojCRM.Helpers
{
    public class EditOrganizationDetails
    {
        public int MerId { get; set; }
        public string MainAddress { get; set; }
        public int MainPostalCode { get; set; }
        public string MainCity { get; set; }
        public CountryIdentificationCodeEnum MainCountry { get; set; }
        public string CorrespondenceAddress { get; set; }
        public int CorrespondencePostalCode { get; set; }
        public string CorrespondenceCity { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ERP { get; set; }
        public string NumberOfInvoicesSent { get; set; }
        public string NumberOfInvoicesReceived { get; set; }
    }
    public class EditImportantOrganizationInfo
    {
        public int MerId { get; set; }
        public LegalFormEnum? LegalForm { get; set; }
        public OrganizationGroupEnum? OrganizationGroup { get; set; }
        public ServiceProviderEnum? ServiceProvider { get; set; }
        public int? LegalStatus { get; set; }
    }
    public class AddOrganizationAttribute
    {
        public int MerId { get; set; }
        public OrganizationAttribute.AttributeClassEnum AttributeClass { get; set; }
        public OrganizationAttribute.AttributeTypeEnum AttributeType { get; set; }
    }

    //JSON object for dropdown menu when searching the company via autocomplete
    public class OrganizationSearch
    {
        [JsonProperty]
        public int MerId { get; set; }
        public string OrganizationName { get; set; }
    }

    public class OrganizationSearchHelper
    {
        public string VAT { get; set; }
        public string SubjectName { get; set; }
        public string MainCity { get; set; }
        public string IsActive { get; set; }
        public int? Group { get; set; }
    }

    public class OrganizationHelperMethods
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public void UpdateOrganization(int merId, string user)
        {
            var credentials = (from u in _db.Users
                               where u.Id == user
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            var organization = _db.Organizations.Find(merId);

            using (var mer = new WebClient() { Encoding = Encoding.UTF8 })
            {
                MerApiGetSubjekt request = new MerApiGetSubjekt()
                {
                    Id = credentials.MerUser,
                    Pass = credentials.MerPass,
                    Oib = "99999999927",
                    PJ = "",
                    SoftwareId = "MojCRM-001",
                    SubjektPJ = organization.MerId.ToString()
                };

                string merRequest = JsonConvert.SerializeObject(request);

                mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var response =
                    mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST",
                        merRequest);
                response = response.Replace("[", "").Replace("]", "");
                MerGetSubjektDataResponse result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(response);

                string postalCode = result.Mjesto.Substring(0, 5).Trim();
                string mainCity = result.Mjesto.Substring(6).Trim();

                organization.SubjectName = result.Naziv;
                organization.FirstReceived = result.FirstReceived;
                organization.FirstSent = result.FirstSent;
                organization.ServiceProvider = (ServiceProviderEnum)result.ServiceProviderId;
                organization.UpdateDate = DateTime.Now;
                organization.LastUpdatedBy = "Moj-CRM - ImportContract";
                organization.MerUpdateDate = DateTime.Now;
                organization.OrganizationDetail.MainAddress = result.Adresa;
                organization.OrganizationDetail.MainPostalCode = Int32.Parse(postalCode);
                organization.OrganizationDetail.MainCity = mainCity;
                organization.OrganizationDetail.CorrespondenceAddress = result.Adresa;
                organization.OrganizationDetail.CorrespondencePostalCode = Int32.Parse(postalCode);
                organization.OrganizationDetail.CorrespondenceCity = mainCity;
                organization.MerDeliveryDetail.TotalSent = result.TotalSent;
                organization.MerDeliveryDetail.TotalReceived = result.TotalReceived;
            }
            _db.SaveChanges();
        }
    }
}