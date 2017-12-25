using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MojCRM.Areas.Campaigns.Models;
using MojCRM.Areas.Cooperation.Models;
using MojCRM.Areas.CRM.Models;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Areas.Sales.Models;
using MojCRM.Areas.Stats.Models;

namespace MojCRM.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Hometown { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string MerUserUsername { get; set; }
        public string MerUserPassword { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Delivery> DeliveryTicketModels { get; set; }

        public DbSet<Organizations> Organizations { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<DeliveryDetail> DeliveryDetails { get; set; }

        public DbSet<LogError> LogError { get; set; }

        public DbSet<ActivityLog> ActivityLogs { get; set; }

        public DbSet<MerDeliveryDetails> MerDeliveryDetails { get; set; }

        public DbSet<Campaign> Campaigns { get; set; }

        public DbSet<CampaignMember> CampaignMembers { get; set; }

        public DbSet<Opportunity> Opportunities { get; set; }

        public DbSet<Lead> Leads { get; set; }

        public DbSet<OrganizationDetail> OrganizationDetails { get; set; }

        public DbSet<OpportunityNote> OpportunityNotes { get; set; }

        public DbSet<LeadNote> LeadNotes { get; set; }

        public DbSet<OrganizationAttribute> OrganizationAttributes { get; set; }

        public DbSet<MerIntegrationSoftware> MerIntegrationSoftware { get; set; }

        public DbSet<MerDocumentExchangeHistory> MerDocumentExchangeHistory { get; set; }

        public DbSet<AcquireEmail> AcquireEmails { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Quote> Quotes { get; set; }

        public DbSet<QuoteLine> QuoteLines { get; set; }

        public DbSet<QuoteMember> QuoteMembers { get; set; }

        public DbSet<Contract> Contracts { get; set; }

        public DbSet<ContractedService> ContractedServices { get; set; }

        public DbSet<ContractRate> ContractRates { get; set; }
    }
}