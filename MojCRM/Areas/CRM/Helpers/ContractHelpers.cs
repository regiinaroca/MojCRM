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
        public DateTime? GetContractDate(int organizationId)
        {
            if (_db.Organizations.Any(o => o.MerId == organizationId && o.Contracts.Any()))
                return _db.Contracts.First(c => c.RelatedOrganizationId == organizationId && c.IsActive).StartDate;

            return null;
        }
    }
}