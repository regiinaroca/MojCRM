﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MojCRM.Helpers;
using System.ComponentModel;

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
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public enum ActivityTypeEnum
        {
            [Description("Test")]
            TEST,

            [Description("Uspješan poziv")]
            SUCCALL,

            [Description("Uspješan poziv (nekonkretni)")]
            SUCCALSHORT,

            [Description("Neuspješan poziv")]
            UNSUCCAL,

            [Description("Izmjena e-mail obavijesti")]
            MAILCHANGE,

            [Description("Ponovno slanje obavijesti o dostavi")]
            RESEND,

            [Description("Slanje e-mailova prema kontaktima dostave")]
            DELMAIL
        }
        public enum DepartmentEnum
        {
            [Description("Moj-CRM")]
            MojCRM,

            [Description("Dostava")]
            Delivery,
        }

        public string ActivityTypeString
        {
            get
            {
                switch (ActivityType)
                {
                    case ActivityTypeEnum.TEST: return "Test";
                    case ActivityTypeEnum.SUCCALL: return "Uspješan poziv";
                    case ActivityTypeEnum.SUCCALSHORT: return "Uspješan poziv (nekonkretni)";
                    case ActivityTypeEnum.UNSUCCAL: return "Neuspješan poziv";
                    case ActivityTypeEnum.MAILCHANGE: return "Izmjena e-mail obavijesti";
                    case ActivityTypeEnum.RESEND: return "Ponovno slanje obavijesti o dostavi";
                    case ActivityTypeEnum.DELMAIL: return "Slanje e-mailova prema kontaktima dostave";
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
                    case DepartmentEnum.MojCRM: return "Moj-CRM";
                    case DepartmentEnum.Delivery: return "Odjel dostave eRačuna";
                }
                return "Odjel";
            }
        }
    }
}