using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

            if (activityType != ActivityLog.ActivityTypeEnum.AchievedSales)
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
}