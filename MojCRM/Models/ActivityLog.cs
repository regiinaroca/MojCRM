using System;
using System.ComponentModel;
using System.Linq;

namespace MojCRM.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
        public int? ReferenceId { get; set; }
        public ActivityTypeEnum ActivityType { get; set; }
        public DepartmentEnum Department { get; set; }
        public ModuleEnum? Module { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsSuspiciousActivity { get; set; }
        public enum ActivityTypeEnum
        {
            [Description("Sistemske akcije")]
            System,

            [Description("Uspješan poziv")]
            Succall,

            [Description("Uspješan poziv (nekonkretni)")]
            Succalshort,

            [Description("Neuspješan poziv")]
            Unsuccal,

            [Description("Izmjena e-mail obavijesti")]
            Mailchange,

            [Description("Ponovno slanje obavijesti o dostavi")]
            Resend,

            [Description("Poslanih e-mailova")]
            Email,

            [Description("Rektivacija e-mail adrese u Postmarku")]
            Postmarkactivatebounce,

            [Description("Kreiran lead")]
            Createdlead,

            [Description("Zaključana kartica")]
            Ticketassign,

            [Description("Izmjena podataka tvrtke")]
            Organizationupdate,

            [Description("Prikupljene e-mail adrese")]
            Acquiredemails,

            [Description("Poslana ponuda")]
            QuoteSent,

            [Description("Ostvarena prodaja")]
            AchievedSales,

            [Description("Dodjela predmeta za obradu baze")]
            AcquireEmailAssignement,

            [Description("Promjena statusa obrade za baze")]
            AcquireEmailEntityStatusChange
        }
        public enum DepartmentEnum
        {
            [Description("Moj-CRM")]
            MojCrm,

            [Description("Dostava")]
            Delivery,

            [Description("Prodaja")]
            Sales,

            [Description("Baze")]
            DatabaseUpdate
        }
        public enum ModuleEnum
        {
            [Description("Moj-CRM")]
            MojCrm,

            [Description("Dostava")]
            Delivery,

            [Description("Prodajne prilike")]
            Opportunities,

            [Description("Leadovi")]
            Leads,

            [Description("Tvrtke")]
            Organizations,

            [Description("Ažuriranje baza")]
            AqcuireEmail,

            [Description("Ponude")]
            Quotes,

            [Description("CRM")]
            Crm
        }

        public string ActivityTypeString
        {
            get
            {
                switch (ActivityType)
                {
                    case ActivityTypeEnum.System: return "Sistemske akcije";
                    case ActivityTypeEnum.Succall: return "Uspješan poziv";
                    case ActivityTypeEnum.Succalshort: return "Uspješan poziv (nekonkretni)";
                    case ActivityTypeEnum.Unsuccal: return "Neuspješan poziv";
                    case ActivityTypeEnum.Mailchange: return "Izmjena e-mail obavijesti";
                    case ActivityTypeEnum.Resend: return "Ponovno slanje obavijesti o dostavi";
                    case ActivityTypeEnum.Email: return "Slanje e-mailova";
                    case ActivityTypeEnum.Postmarkactivatebounce: return "Rektivacija e-mail adrese u Postmarku";
                    case ActivityTypeEnum.Createdlead: return "Kreiran lead";
                    case ActivityTypeEnum.Ticketassign: return "Zaključana kartica";
                    case ActivityTypeEnum.Organizationupdate: return "Izmjena podataka tvrtke";
                    case ActivityTypeEnum.Acquiredemails: return "Prikupljene e-mail adrese";
                    case ActivityTypeEnum.QuoteSent: return "Poslana ponuda";
                    case ActivityTypeEnum.AchievedSales: return "Ostvarena prodaja";
                    case ActivityTypeEnum.AcquireEmailAssignement: return "Dodjela predmeta za obradu baze";
                    case ActivityTypeEnum.AcquireEmailEntityStatusChange: return "Promjena statusa obrade za baze";
                }
                return "Tip aktivnosti";
            }
        }

        public string DepartmentString
        {
            get
            {
                switch (Department)
                {
                    case DepartmentEnum.MojCrm: return "Moj-CRM";
                    case DepartmentEnum.Delivery: return "Odjel dostave eRačuna";
                    case DepartmentEnum.Sales: return "Odjel prodaje";
                    case DepartmentEnum.DatabaseUpdate: return "Odjel prikupa e-mail adresa";
                }
                return "Odjel";
            }
        }

        public string ModuleEnumString
        {
            get
            {
                switch (Module)
                {
                    case ModuleEnum.MojCrm: return "Moj-CRM";
                    case ModuleEnum.Delivery: return "Dostava eRačuna";
                    case ModuleEnum.Opportunities: return "Prodajne prilike";
                    case ModuleEnum.Leads: return "Leadovi";
                    case ModuleEnum.AqcuireEmail: return "Ažuriranje baza";
                    case ModuleEnum.Quotes: return "Ponude";
                    case ModuleEnum.Crm: return "CRM";
                }
                return "Modul";
            }
        }

        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public bool CheckSuspiciousActivity(string user, ActivityTypeEnum activityType)
        {
            ActivityLog reference;
            try
            {
                reference = _db.ActivityLogs.OrderByDescending(a => a.InsertDate).First(a =>
                    a.User == user && a.ActivityType == activityType);
            }
            catch (InvalidOperationException)
            {
                reference = new ActivityLog()
                {
                    User = user,
                    InsertDate = DateTime.Now.AddMinutes(-5)
                };
            }
            

            if (reference != null)
                if ((int)DateTime.Now.Subtract(reference.InsertDate).TotalMinutes < 1)
                return true;
            return false;
        }
    }
}