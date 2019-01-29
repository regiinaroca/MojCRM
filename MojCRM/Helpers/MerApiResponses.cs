using Newtonsoft.Json;
using System;

namespace MojCRM.Helpers
{
    public class MerResendChangeEmailResponse
    {
        [JsonProperty]
        public int DocumentId { get; set; }
        public string Email { get; set; }
        public string Error { get; set; }
    }

    public class MerGetSubjektDataResponse
    {
        [JsonProperty]
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string PoslovnaJedinica { get; set; }
        public string Oib { get; set; }
        public string GLN { get; set; }
        public string Adresa { get; set; }
        public string Mjesto { get; set; }
        public string Županija { get; set; }
        public string IBAN { get; set; }
        public int? TotalReceived { get; set; }
        public int? TotalSent { get; set; }
        public DateTime? FirstReceived { get; set; }
        public DateTime? FirstSent { get; set; }
        public int ServiceProviderId { get; set; }
    }

    //CreateTickets
    public class MerGetNondeliveredDocumentsResponse
    {
        [JsonProperty]
        public int Id { get; set; }
        public string InterniBroj { get; set; }
        public int DokumentTypeId { get; set; }
        public DateTime DatumOtpreme { get; set; }
        public int PosiljateljId { get; set; }
        public string PosiljateljOib { get; set; }
        public string PosiljateljNaziv { get; set; }
        public string EmailPosiljatelja { get; set; }
        public int PrimateljId { get; set; }
        public string PrimateljOib { get; set; }
        public string PrimateljNaziv { get; set; }
        public string EmailPrimatelja { get; set; }
        public int? TotalReceived { get; set; } // No. of total documents with DocumentStatus = 40
    }

    //DocumentHistory
    public class MerGetSentDocumentsResponse
    {
        [JsonProperty]
        public int Id { get; set; }
        public string InterniBroj { get; set; }
        public int DokumentTypeId { get; set; }
        public int DokumentStatusId { get; set; }
        public DateTime? DatumOtpreme { get; set; }
        public DateTime? DatumZadnjePoruke { get; set; }
        public DateTime? DatumDostave { get; set; }
        public string EmailPrimatelja { get; set; }
        public string EmailPosiljatelja { get; set; }
        public int PosiljateljId { get; set; }
        public string PosiljateljOib { get; set; }
        public string PosiljateljNaziv { get; set; }

        public string DocumentStatusString
        {
            get
            {
                switch (DokumentStatusId)
                {
                    case 10: return "U pripremi";
                    case 20: return "Potpisan";
                    case 30: return "Poslan";
                    case 40: return "Dostavljen";
                    case 45: return "Ispisan";
                    case 50: return "Neuspješan";
                    case 55: return "Uklonjen";
                }
                return "Status";
            }
        }

        public string DocumentTypeIdString
        {
            get
            {
                switch (DokumentTypeId)
                {
                    case 0: return "eDokument";
                    case 1: return "eRačun";
                    case 3: return "Storno";
                    case 4: return "eOpomena";
                    case 6: return "ePrimka (tip 6)";
                    case 7: return "eOdgovor";
                    case 105: return "eNarudžba";
                    case 226: return "eOpoziv";
                    case 230: return "eIzmjena";
                    case 231: return "eOdgovorN";
                    case 351: return "eOtrpemnica";
                    case 381: return "eOdobrenje";
                    case 383: return "eTerećenje";
                    case 632: return "ePrimka";
                }
                return "Tip dokumenta";
            }
        }
    }

    public class MerGetContractsResponse
    {
        [JsonProperty]
        public int Id { get; set; }
        public string ContractNumber { get; set; }
        public string CompanyId { get; set; }
        public int SubjektId { get; set; }
        public string ProposalNumber { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int GracePeriod { get; set; }
        public string Note { get; set; }
        public int? UserId { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public int? ActivatedUserId { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public int? DeactivatedUserId { get; set; }
        public string DeactivateReason { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime Changed { get; set; }
        public DateTime? LevelingDate { get; set; }
        public MerContractProduct[] Products { get; set; }
    }

    public class MerContractProduct
    {
        [JsonProperty]
        public int ContractId { get; set; }
        public int ProductId { get; set; }
        public decimal ListPrice { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public decimal? Quantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public int? UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime Changed { get; set; }
    }

    public class MerGetInaReportResponse
    {
        public string PartnerOib { get; set; }
        public string PartnerNaziv { get; set; }
        public string InterniBroj { get; set; }
        public DateTime? DatumOtpreme { get; set; }
        public DateTime? DatumDostave { get; set; }
        public int Id { get; set; }
        public int DokumentStatusId { get; set; }
        public int? DokumentTypeId { get; set; }
        public string PartnerEmail { get; set; }
    }
}