using MojCRM.Helpers;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojCRM.ViewModels
{
    public class DeliveryDetailsViewModel
    {
        public int TicketId { get; set; }

        [Display(Name = "Pošiljatelj:")]
        public string SenderName { get; set; }

        public string SenderVAT { get; set; }

        [Display(Name = "Primatelj:")]
        public string ReceiverName { get; set; }

        public int? ReceiverId { get; set; }
        public string ReceiverVAT { get; set; }

        [Display(Name = "Interni broj dokumenta:")]
        public string InvoiceNumber { get; set; }

        [Display(Name = "Datum slanja dokumenta:")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime SentDate { get; set; }

        public int MerElectronicId { get; set; }
        public int MerDocumentTypeId { get; set; }
        public int MerDocumentStatusId { get; set; }

        [Display(Name = "E-mail adresa primatelja:")]
        public string ReceiverEmail { get; set; }

        [Display(Name = "Napomena:")]
        public string MerDeliveryDetailComment { get; set; }

        [Display(Name = "Kontakt podaci:")]
        public string MerDeliveryDetailTelephone { get; set; }

        [Display(Name = "Napomene za dostavu eRačuna ovom primatelju: ")]
        public string ImportantComment { get; set; }

        [Display(Name = "Ime kontakta")]
        public string DeliveryContactFirstName { get; set; }

        [Display(Name = "Prezime kontakta")]
        public string DeliveryContactLastName { get; set; }

        [Display(Name = "Broj telefona")]
        public string DeliveryContactTelephone { get; set; }

        [Display(Name = "E-mail adresa kontakta")]
        public string DeliveryContactEmail { get; set; }

        [Display(Name = "Broj mobitela")]
        public string DeliveryContactMobileTelephone { get; set; }

        [Display(Name = "E-mail adresa")]
        public string DeliveryEmailAddress { get; set; }

        [Display(Name = "Agent")]
        public string DeliveryContactAgent { get; set; }

        [Display(Name = "Tvrtka primatelj")]
        public string DeliveryDetailReceiver { get; set; }

        [Display(Name = "Agent")]
        public string DeliveryDetailAgent { get; set; }

        [Display(Name = "Datum i vrijeme napomene")]
        [DataType(DataType.DateTime)]
        public DateTime DeliveryDetailDateTime { get; set; }

        [Display(Name = "Povezani kontakt")]
        public string DeliveryDetailContact { get; set; }

        [Display(Name = "Napomena")]
        public string DeliveryDetailDetail { get; set; }

        [Display(Name = "Dodijeljeno")]
        public bool IsAssigned { get; set; }

        [Display(Name = "Dodijeljeno agentu")]
        public string AssignedTo { get; set; }

        [Display(Name = "Zadnja bilješka")]
        public string LastDeliveryDetail { get; set; }

        [Display(Name = "Tip dokumenta:")]
        public string MerDocumentTypeIdString
        {
            get
            {
                switch (MerDocumentTypeId)
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

        [Display(Name = "Status dokumenta")]
        public string DocumentStatusString
        {
            get
            {
                switch (MerDocumentStatusId)
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
        public IEnumerable<Delivery> RelatedInvoices { get; set; }
        public IEnumerable<Contact> RelatedDeliveryContacts { get; set; }
        public IEnumerable<DeliveryDetail> RelatedDeliveryDetails { get; set; }
        public IEnumerable<ActivityLog> RelatedActivities { get; set; }
        public IEnumerable<MerGetSentDocumentsResponse> DocumentHistory { get; set; }
        public MessagesOutboundOpenResponse PostmarkOpenings { get; set; }
        public BouncesResponse PostmarkBounces { get; set; }
        public IList<SelectListItem> RelatedDeliveryContactsForDetails
        {
            get
            {
                var list = (from t in RelatedDeliveryContacts
                            select new SelectListItem()
                            {
                                Text = t.ContactFirstName + " " + t.ContactLastName,
                                Value = t.ContactFirstName + " " + t.ContactLastName
                            }).ToList();
                return list;
            }
            set { }
        }
        public IList<SelectListItem> DeliveryDetailsIds
        {
            get
            {
                var list = (from t in RelatedDeliveryDetails
                            select new SelectListItem()
                            {
                                Text = t.DetailNote,
                                Value = t.Id.ToString()
                            }).ToList();
                return list;
            }
            set { }
        }
    }
}