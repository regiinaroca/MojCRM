using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MojCRM.Models;

namespace MojCRM.Helpers
{
    public class HelperMethods
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly ActivityLog _al = new ActivityLog();

        public void LogActivity(string activityDescription, string user, int activityReferenceId, 
            ActivityLog.ActivityTypeEnum activityType, ActivityLog.DepartmentEnum department, ActivityLog.ModuleEnum module)
        {
            var isSuspicious = false;

            if (activityType != ActivityLog.ActivityTypeEnum.AchievedSales || activityType != ActivityLog.ActivityTypeEnum.System || activityType != ActivityLog.ActivityTypeEnum.AcquireEmailAssignement)
                isSuspicious = _al.CheckSuspiciousActivity(user, activityType);            
            _db.ActivityLogs.Add(new ActivityLog
            {
                Description = activityDescription,
                User = user,
                ReferenceId = activityReferenceId,
                ActivityType = activityType,
                Department = department,
                Module = module,
                InsertDate = DateTime.Now,
                IsSuspiciousActivity = isSuspicious
            });
            _db.SaveChanges();
        }
    }

    public class ChangeEmailNoTicket
    {
        public int MerElectronicId { get; set; }
        public int ReceiverId { get; set; }

        [Display(Name = "E-mail adresa primatelja:")]
        public string OldEmail { get; set; }
        public int TicketId { get; set; }
        public string Agent { get; set; }
    }

    public enum OrganizationGroupEnum
    {
        [Description("Nema grupacija")]
        Nema,

        [Description("Adris grupa")]
        AdrisGrupa,

        [Description("Agrokor")]
        Agrokor,

        [Description("Atlantic grupa")]
        AtlanticGrupa,

        [Description("Poslovna grupacija Auto Hrvatska")]
        AutoHrvatska,

        [Description("Babić pekare")]
        BabićPekare,

        [Description("COMET")]
        COMET,

        [Description("CIOS Grupa")]
        CIOS,

        [Description("CVH - Centar vozila Hrvatska")]
        CVH,

        [Description("HOLCIM Grupa")]
        Holcim,

        [Description("MSAN Grupa")]
        MSAN,

        [Description("NEXE Grupa")]
        NEXE,

        [Description("NTL - Narodni trgovački lanac")]
        NTL,

        [Description("Pivac Grupa")]
        PivacGrupa,

        [Description("Rijeka Holding")]
        RijekaHolding,

        [Description("STRABAG Grupa")]
        STRABAG,

        [Description("Styria grupa")]
        StyriaGrupa,

        [Description("Sunce Koncern")]
        SunceKoncern,

        [Description("Ultra gros")]
        UltraGros,

        [Description("Žito Grupa")]
        Žito,

        [Description("Zagrebački Holding")]
        ZagrebačkiHolding,

        [Description("C.I.A.K. Grupa")]
        Ciak
    }

    public class DailyUpdateReturnModel
    {
        public int NumberOfOrganizationCountriesUpdated { get; set; }
    }

    public class AdminHelperMethods
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public int UpdateOrganizationAttributes()
        {
            int updated = 0;

            var entities = _db.Contracts;
            var attributes = _db.OrganizationAttributes.Where(a =>
                a.AttributeType == OrganizationAttribute.AttributeTypeEnum.CONTRACT &&
                a.AttributeClass == OrganizationAttribute.AttributeClassEnum.MER);

            foreach (var entity in entities)
            {
                if (entity.IsActive)
                {
                    if (!attributes.Any(x => x.OrganizationId == entity.RelatedOrganizationId))
                    {
                        _db.OrganizationAttributes.Add(new OrganizationAttribute()
                        {
                            OrganizationId = entity.RelatedOrganizationId,
                            AttributeClass = OrganizationAttribute.AttributeClassEnum.MER,
                            AttributeType = OrganizationAttribute.AttributeTypeEnum.CONTRACT,
                            IsActive = true,
                            AssignedBy = "Moj-CRM",
                            InsertDate = DateTime.Now
                        });
                    }
                }
                if (!entity.IsActive)
                {
                    if (!attributes.Any(x => x.OrganizationId == entity.RelatedOrganizationId))
                    {
                        _db.OrganizationAttributes.Add(new OrganizationAttribute()
                        {
                            OrganizationId = entity.RelatedOrganizationId,
                            AttributeClass = OrganizationAttribute.AttributeClassEnum.MER,
                            AttributeType = OrganizationAttribute.AttributeTypeEnum.CONTRACT,
                            IsActive = false,
                            AssignedBy = "Moj-CRM",
                            InsertDate = DateTime.Now
                        });
                    }
                    if (attributes.Any(x => x.OrganizationId == entity.RelatedOrganizationId))
                    {
                        var attributeTemp =
                            _db.OrganizationAttributes.First(a => a.IsActive && a.OrganizationId == entity.RelatedOrganizationId);
                        attributeTemp.IsActive = false;
                        attributeTemp.UpdateDate = DateTime.Now;
                    }
                }
                updated++;
            }
            _db.SaveChanges();

            return updated;
        }

        public int UpdateOrganizationCountries()
        {
            int updated = 0;

            var forUpdate = _db.OrganizationDetails.Where(o => o.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.Noinfo);

            foreach (var organization in forUpdate)
            {
                UpdateCountry(organization.MerId);
                updated++;
            }
            _db.SaveChanges();

            return updated;
        }

        public void UpdateCountry(int organizationId)
        {
            var organization = _db.OrganizationDetails.First(x => x.MerId == organizationId);

            if (organization.Organization.VAT.StartsWith("AT"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.At;
            else if (organization.Organization.VAT.StartsWith("BE"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Be;
            else if (organization.Organization.VAT.StartsWith("BG"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Bg;
            else if (organization.Organization.VAT.StartsWith("CY"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Cy;
            else if (organization.Organization.VAT.StartsWith("CZ"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Cz;
            else if (organization.Organization.VAT.StartsWith("DE"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.De;
            else if (organization.Organization.VAT.StartsWith("DK"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Dk;
            else if (organization.Organization.VAT.StartsWith("EE"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Ee;
            else if (organization.Organization.VAT.StartsWith("EL"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.El;
            else if (organization.Organization.VAT.StartsWith("ES"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Es;
            else if (organization.Organization.VAT.StartsWith("FI"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Fi;
            else if (organization.Organization.VAT.StartsWith("FR"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Fr;
            else if (organization.Organization.VAT.StartsWith("GB"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Gb;
            else if (organization.Organization.VAT.StartsWith("HU"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Hu;
            else if (organization.Organization.VAT.StartsWith("IE"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Ie;
            else if (organization.Organization.VAT.StartsWith("IT"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.It;
            else if (organization.Organization.VAT.StartsWith("LT"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Lt;
            else if (organization.Organization.VAT.StartsWith("LU"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Lu;
            else if (organization.Organization.VAT.StartsWith("LV"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Lv;
            else if (organization.Organization.VAT.StartsWith("MT"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Mt;
            else if (organization.Organization.VAT.StartsWith("NL"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Nl;
            else if (organization.Organization.VAT.StartsWith("PL"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Pl;
            else if (organization.Organization.VAT.StartsWith("PT"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Pt;
            else if (organization.Organization.VAT.StartsWith("RO"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Ro;
            else if (organization.Organization.VAT.StartsWith("SE"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Se;
            else if (organization.Organization.VAT.StartsWith("SI"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Si;
            else if (organization.Organization.VAT.StartsWith("SK"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Sk;
            else if (organization.Organization.VAT.StartsWith("AT"))
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.At;
            else
                organization.MainCountry = OrganizationDetail.CountryIdentificationCodeEnum.Hr;
        }
    }
}