﻿using MojCRM.Models;
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
            WrongTelephoneNumber
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
                    case AcquireEmailEntityStatusEnum.OldPartner: return "Ne poslujus s korisnikom";
                    case AcquireEmailEntityStatusEnum.PartnerWillContactUser: return "Partner će se javiti korisniku samostalno";
                    case AcquireEmailEntityStatusEnum.WrittenConfirmationRequired: return "Potrebno poslati pisanu suglasnost";
                    case AcquireEmailEntityStatusEnum.WrongTelephoneNumber: return "Neispravan kontakt broj";
                }
                return "Status unosa";
            }
        }
    }
}