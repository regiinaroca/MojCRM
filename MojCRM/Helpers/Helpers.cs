using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojCRM.Helpers
{
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
        ZagrebačkiHolding
    }
}