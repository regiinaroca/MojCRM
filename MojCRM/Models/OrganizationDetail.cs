using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MojCRM.Areas.Cooperation.Models;
using MojCRM.Helpers;

namespace MojCRM.Models
{
    public class OrganizationDetail
    {
        [Key, ForeignKey("Organization")]
        public int MerId { get; set; }

        //Main address info
        [Display(Name = "Adresa:")]
        public string MainAddress { get; set; }
        [Display(Name = "Poštanski broj:")]
        public int MainPostalCode { get; set; }
        [Display(Name = "Grad / Mjesto:")]
        public string MainCity { get; set; }
        [Display(Name = "Država:")]
        public CountryIdentificationCodeEnum MainCountry { get; set; }

        //Address info for correspondence
        [Display(Name = "Adresa:")]
        public string CorrespondenceAddress { get; set; }
        [Display(Name = "Poštanski broj:")]
        public int CorrespondencePostalCode { get; set; }
        [Display(Name = "Grad / Mjesto:")]
        public string CorrespondenceCity { get; set; }
        [Display(Name = "Država:")]
        public CountryIdentificationCodeEnum CorrespondenceCountry { get; set; }

        public string TelephoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ERP { get; set; }
        public string NumberOfInvoicesSent { get; set; }
        public string NumberOfInvoicesReceived { get; set; }
        public OrganizationGroupEnum OrganizationGroup { get; set; }

        public int? MerSendSoftware { get; set; }
        [ForeignKey("MerSendSoftware")]
        public virtual MerIntegrationSoftware SendSoftware { get; set; }

        public int? MerReceiveSoftware { get; set; }
        [ForeignKey("MerReceiveSoftware")]
        public virtual MerIntegrationSoftware ReceiveSoftware { get; set; }

        public virtual Organizations Organization { get; set; }

        public enum CountryIdentificationCodeEnum
        {
            [Description("Nema podatka")]
            Noinfo,

            [Description("Hrvatska")]
            Hr,

            [Description("Slovenija")]
            Si,

            [Description("Austrija")]
            At,

            [Description("Poljska")]
            Pl,

            [Description("Italija")]
            It,

            [Description("Njemačka")]
            De,

            [Description("Belgija")]
            Be,

            [Description("Bugarska")]
            Bg,

            [Description("Cipar")]
            Cy,

            [Description("Češka")]
            Cz,

            [Description("Danska")]
            Dk,

            [Description("Estonija")]
            Ee,

            [Description("Grčka")]
            El,

            [Description("Španjolska")]
            Es,

            [Description("Finska")]
            Fi,

            [Description("Francuska")]
            Fr,

            [Description("Ujedinjena Kraljevina")]
            Gb,

            [Description("Mađarska")]
            Hu,

            [Description("Irska")]
            Ie,

            [Description("Litva")]
            Lt,

            [Description("Luksemburg")]
            Lu,

            [Description("Latvija")]
            Lv,

            [Description("Malta")]
            Mt,

            [Description("Nizozemska")]
            Nl,

            [Description("Portugal")]
            Pt,

            [Description("Rumunjska")]
            Ro,

            [Description("Švedska")]
            Se,

            [Description("Slovačka")]
            Sk
        }

        public string CountryIdentificationCode
        {
            get
            {
                switch (MainCountry)
                {
                    case CountryIdentificationCodeEnum.Noinfo: return "Nema podatka";
                    case CountryIdentificationCodeEnum.Hr: return "Hrvatska";
                    case CountryIdentificationCodeEnum.Si: return "Slovenija";
                    case CountryIdentificationCodeEnum.At: return "Austrija";
                    case CountryIdentificationCodeEnum.Pl: return "Poljska";
                    case CountryIdentificationCodeEnum.It: return "Italija";
                    case CountryIdentificationCodeEnum.De: return "Njemačka";
                    case CountryIdentificationCodeEnum.Be: return "Belgija";
                    case CountryIdentificationCodeEnum.Bg: return "Bugarska";
                    case CountryIdentificationCodeEnum.Cy: return "Cipar";
                    case CountryIdentificationCodeEnum.Cz: return "Češka";
                    case CountryIdentificationCodeEnum.Dk: return "Danska";
                    case CountryIdentificationCodeEnum.Ee: return "Estonija";
                    case CountryIdentificationCodeEnum.El: return "Grčka";
                    case CountryIdentificationCodeEnum.Es: return "Španjolska";
                    case CountryIdentificationCodeEnum.Fi: return "Finska";
                    case CountryIdentificationCodeEnum.Fr: return "Francuska";
                    case CountryIdentificationCodeEnum.Gb: return "Ujedinjena Kraljevina";
                    case CountryIdentificationCodeEnum.Hu: return "Mađarska";
                    case CountryIdentificationCodeEnum.Ie: return "Irska";
                    case CountryIdentificationCodeEnum.Lt: return "Litva";
                    case CountryIdentificationCodeEnum.Lu: return "Luksemburg";
                    case CountryIdentificationCodeEnum.Lv: return "Latvija";
                    case CountryIdentificationCodeEnum.Mt: return "Malta";
                    case CountryIdentificationCodeEnum.Nl: return "Nizozemska";
                    case CountryIdentificationCodeEnum.Pt: return "Portugal";
                    case CountryIdentificationCodeEnum.Ro: return "Rumunjska";
                    case CountryIdentificationCodeEnum.Se: return "Švedska";
                    case CountryIdentificationCodeEnum.Sk: return "Slovačka";
                }
                return "Nema podatka";
            }
        }

        public string OrganizationGroupString
        {
            get
            {
                switch (OrganizationGroup)
                {
                    case OrganizationGroupEnum.Nema: return "Ne pripada grupaciji";
                    case OrganizationGroupEnum.AdrisGrupa: return "Adris Grupa";
                    case OrganizationGroupEnum.Agrokor: return "Koncern Agrokor";
                    case OrganizationGroupEnum.AtlanticGrupa: return "Atlantic Grupa";
                    case OrganizationGroupEnum.AutoHrvatska: return "Poslovna grupacija Auto Hrvatska";
                    case OrganizationGroupEnum.BabićPekare: return "Babić Pekare";
                    case OrganizationGroupEnum.COMET: return "COMET";
                    case OrganizationGroupEnum.CIOS: return "CIOS";
                    case OrganizationGroupEnum.CVH: return "CVH";
                    case OrganizationGroupEnum.Holcim: return "Holcim Grupa";
                    case OrganizationGroupEnum.MSAN: return "MSAN Grupa";
                    case OrganizationGroupEnum.NEXE: return "NEXE Grupa";
                    case OrganizationGroupEnum.NTL: return "NTL Grupa";
                    case OrganizationGroupEnum.PivacGrupa: return "Pivac Grupa";
                    case OrganizationGroupEnum.RijekaHolding: return "Rijeka Holding";
                    case OrganizationGroupEnum.STRABAG: return "STRABAG";
                    case OrganizationGroupEnum.StyriaGrupa: return "Styria Grupa";
                    case OrganizationGroupEnum.SunceKoncern: return "Koncern Sunce";
                    case OrganizationGroupEnum.UltraGros: return "Ultra Gros";
                    case OrganizationGroupEnum.Žito: return "Žito Grupa";
                    case OrganizationGroupEnum.ZagrebačkiHolding: return "Zagrebački Holding";
                    case OrganizationGroupEnum.Ciak: return "C.I.A.K. Grupa";
                }
                return "Ne pripada grupaciji";
            }
        }
    }
}