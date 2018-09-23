using MojCRM.Models;
using MojCRM.Areas.Campaigns.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Areas.HelpDesk.Models
{
    public class AcquireEmail
    {
        [Key]
        public int Id { get; set; }
        public int? RelatedOrganizationId { get; set; }
        public int? RelatedCampaignId { get; set; }
        public bool? IsNewlyAcquired { get; set; }
        public bool IsAssigned { get; set; }
        public string AssignedTo { get; set; }
        public AcquireEmailStatusEnum AcquireEmailStatus { get; set; }
        public AcquireEmailEntityStatusEnum? AcquireEmailEntityStatus { get; set; }
        public string LastContactedBy { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? LastContactDate { get; set; }

        [ForeignKey("RelatedOrganizationId")]
        public virtual Organizations Organization { get; set; }
        [ForeignKey("RelatedCampaignId")]
        public virtual Campaign Campaign { get; set; }

        public enum AcquireEmailStatusEnum
        {
            [Description("Kreirano")]
            Created,

            [Description("Provjereno")]
            Checked,

            [Description("Verificirano")]
            Verified,

            [Description("Revidirano")]
            Reviewed
        }

        public string AcquireEmailStatusString
        {
            get
            {
                switch (AcquireEmailStatus)
                {
                    case AcquireEmailStatusEnum.Created: return "Kreirano";
                    case AcquireEmailStatusEnum.Checked: return "Provjereno";
                    case AcquireEmailStatusEnum.Verified: return "Verificirano";
                    case AcquireEmailStatusEnum.Reviewed: return "Revidirano";
                }
                return "Status provjere";
            }
        }

        public enum AcquireEmailEntityStatusEnum
        {
            [Description("Kreirano")]
            Created,

            [Description("Dobivena povratna informacija")]
            AcquiredInformation,

            [Description("Nema odgovora / Ne javlja se")]
            NoAnswer,

            [Description("Zatvoren subjekt")]
            ClosedOrganization,

            [Description("Ne posluju s korisnikom")]
            OldPartner,

            [Description("Partner će se javiti korisniku samostalno")]
            PartnerWillContactUser,

            [Description("Potrebno poslati pisanu suglasnost")]
            WrittenConfirmationRequired,

            [Description("Neispravan kontakt broj")]
            WrongTelephoneNumber,

            [Description("Poslovna Hrvatska")]
            PoslovnaHrvatska,

            [Description("Ne postoji ispravan kontakt broj")]
            NoTelehoneNumber,

            [Description("Subjekt u stečaju / likvidaciji")]
            Bankruptcy,

            [Description("Subjekt nema žiro račun")]
            NoFinancialAccount,

            [Description("Najava brisanja subjekta")]
            ToBeClosed,

            [Description("Žele primati račune poštom")]
            Post,

            [Description("Inozemna tvrtka")]
            Foreign,

            [Description("Tvrtka u mirovanju")]
            OnHold,

            [Description("Pošta provjereno")]
            PostChecked,

            [Description("Ne javlja se, prethodni status POŠTA")]
            NoAnswerOldPost,

            [Description("Prikupljena povratna informacija, ne žele obavijest")]
            AcquiredInformationNoEmail
        }

        public string AcquireEmailEntityStatusString
        {
            get
            {
                switch (AcquireEmailEntityStatus)
                {
                    case AcquireEmailEntityStatusEnum.Created: return "Kreirano";
                    case AcquireEmailEntityStatusEnum.AcquiredInformation: return "Prikupljena povratna informacija";
                    case AcquireEmailEntityStatusEnum.NoAnswer: return "Nema odgovora / Ne javlja se";
                    case AcquireEmailEntityStatusEnum.ClosedOrganization: return "Zatvorena tvrtka";
                    case AcquireEmailEntityStatusEnum.OldPartner: return "Ne posluju s korisnikom";
                    case AcquireEmailEntityStatusEnum.PartnerWillContactUser: return "Partner će se javiti korisniku samostalno";
                    case AcquireEmailEntityStatusEnum.WrittenConfirmationRequired: return "Potrebno poslati pisanu suglasnost";
                    case AcquireEmailEntityStatusEnum.WrongTelephoneNumber: return "Neispravan kontakt broj";
                    case AcquireEmailEntityStatusEnum.PoslovnaHrvatska: return "Kontakt u bazi";
                    case AcquireEmailEntityStatusEnum.NoTelehoneNumber: return "Ne postoji ispravan kontakt broj";
                    case AcquireEmailEntityStatusEnum.Bankruptcy: return "Subjekt u stečaju / likvidaciji";
                    case AcquireEmailEntityStatusEnum.NoFinancialAccount: return "Subjekt nema žiro račun";
                    case AcquireEmailEntityStatusEnum.ToBeClosed: return "Najava brisanja subjekta";
                    case AcquireEmailEntityStatusEnum.Post: return "POŠTA";
                    case AcquireEmailEntityStatusEnum.Foreign: return "Inozemna tvrtka";
                    case AcquireEmailEntityStatusEnum.OnHold: return "Tvrtka u mirovanju";
                    case AcquireEmailEntityStatusEnum.PostChecked: return "POŠTA PROVJERENO";
                    case AcquireEmailEntityStatusEnum.NoAnswerOldPost: return "Ne javlja se, PSP";
                    case AcquireEmailEntityStatusEnum.AcquiredInformationNoEmail: return "Prikupljena povratna informacija, ne žele obavijest";
                }
                return "Status unosa";
            }
        }
    }
}