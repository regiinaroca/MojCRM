using System;
using System.Linq;
using MojCRM.Models;

namespace MojCRM.Areas.CRM.Helpers
{
    public class ContractSearchModel
    {
        public string ContractNumber { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationVat { get; set; }
    }

    public class ContractHelperMethods
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        /// Method which returns Contract StartDate of first active contract
        /// </summary>
        /// <param name="organizationId">ID of an Organization for which we want to check StartDate</param>
        /// <returns></returns>
        public DateTime? GetContractDate(int organizationId)
        {
            if (_db.Organizations.Any(o => o.MerId == organizationId && o.Contracts.Any()))
                //21.11.2018. commented, in future uncomment because we want to check whether some user has active contract before we start some detail work with campaigns
                //return _db.Contracts.First(c => c.RelatedOrganizationId == organizationId && c.IsActive).StartDate;
                return _db.Contracts.OrderByDescending(c => c.Id).First(c => c.RelatedOrganizationId == organizationId).StartDate;

            return null;
        }
    }
}