using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MojCRM.Models
{
    public class MerDeliveryDetails
    {
        [Key, ForeignKey("Organization")]
        public int MerId { get; set; }
        public virtual Organizations Organization { get; set; }
        public string Comments { get; set; }
        public string Telephone { get; set; }
        public string ImportantComments { get; set; }

        [Display(Name = "Ukupan broj poslanih dokumenata")]
        public int? TotalSent { get; set; }
        [Display(Name = "Ukupan broj primljenih dokumenata")]
        public int? TotalReceived { get; set; }

        [Display(Name = "Prikupljena informacija o zaprimanju eRačuna")]
        public string AcquiredReceivingInformation { get; set; }
        public bool AcquiredReceivingInformationIsVerified { get; set; }
        public bool RequiredPostalService { get; set; }
        [Display(Name = "E-mail adresa za provjeru")] //CroBiz import addresses
        public string EmailAddressForVerification { get; set; }
    }
}