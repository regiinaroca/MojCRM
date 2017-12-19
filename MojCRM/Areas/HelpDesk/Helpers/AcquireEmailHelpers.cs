using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Models;

namespace MojCRM.Areas.HelpDesk.Helpers
{
    public class EditAcquiredReceivingInformationHelper
    {
        public int MerId { get; set; }
        public string AcquiredReceivingInformation { get; set; }
        public string NewAcquiredReceivingInformation { get; set; }
    }

    public class AcquireEmailExportModel
    {
        public AcquireEmail Entity { get; set; }
        public string CampaignName { get; set; }
        public string VAT { get; set; }
        public string SubjectName { get; set; }
        public string AcquiredReceivingInformation { get; set; }
        public AcquireEmail.AcquireEmailEntityStatusEnum? AcquiredEmailEntityStatus { get; set; }
    }

    public class AcquireEmailExportForEmailNotificationModel
    {
        public string AcquiredReceivingInformation { get; set; }
    }

    public class AcquireEmailCheckResults
    {
        public int CampaignId { get; set; }
        public int ValidEntities { get; set; }
        public int InvalidEntities { get; set; }
        public int ImportedEntities { get; set; }
        public List<string> ValidVATs { get; set; }
        public List<string> InvalidVATs { get; set; }
    }

    public class AcquireEmailSearchModel
    {
        public string CampaignName { get; set; }
        public string OrganizationName { get; set; }
        public string TelephoneMail { get; set; }
        public int? EmailStatusEnum { get; set; }
    }

    public class AcquireEmailStatsPerAgentAndCampaign
    {
        public string Agent { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int NumberOfEntitiesForProcessing { get; set; }
        public int NumberOfEntitiesWithoutPhoneNumber { get; set; }
    }

    public class AcquireEmailMethodHelpers
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public void UpdateClosedSubjectEntities(int organizationId)
        {
            var entities = _db.AcquireEmails.Where(x => x.RelatedOrganizationId == organizationId);

            foreach (var acquireEmail in entities)
            {
                acquireEmail.AcquireEmailStatus = AcquireEmail.AcquireEmailStatusEnum.Verified;
                acquireEmail.AcquireEmailEntityStatus = AcquireEmail.AcquireEmailEntityStatusEnum.ClosedOrganization;
                acquireEmail.UpdateDate = DateTime.Now;
            }
            _db.SaveChanges();
        }
    }
}