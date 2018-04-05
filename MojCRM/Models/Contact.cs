using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace MojCRM.Models
{
    public class Contact
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [Key]
        public int ContactId { get; set; }

        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organizations Organization { get; set; }

        [Display(Name = "Ime kontakta")]
        public string ContactFirstName { get; set; }

        [Display(Name = "Prezime kontakta")]
        public string ContactLastName { get; set; }

        [Display(Name = "Titula / funkcija")]
        public string Title { get; set; }

        [Display(Name = "Broj telefona")]
        public string TelephoneNumber { get; set; }

        [Display(Name = "Broj mobitela")]
        public string MobilePhoneNumber { get; set; }

        [Display(Name = "E-mail adresa")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,20}", ErrorMessage = "E-mail adresa je nepravilno unesena.")]
        public string Email { get; set; }

        [Display(Name = "Agent")]
        public string User { get; set; }
        [Display(Name = "Zabrana kontakta")]
        public bool DoNotCall { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Tip kontakta")]
        public ContactTypeEnum ContactType { get; set; }

        public enum ContactTypeEnum
        {
            [Description("Općeniti")]
            General,

            [Description("Dostava")]
            Delivery,

            [Description("Prodaja")]
            Sales
        }

        [Display(Name = "Dodijeli novog agenta")]
        [NotMapped]
        public IList<SelectListItem> Agents
        {
            get
            {
                var agents = (from u in _db.Users
                              select new SelectListItem()
                              {
                                  Text = u.UserName,
                                  Value = u.UserName
                              }).ToList();
                return agents;
            }
        }

        public string ContactTypeString
        {
            get
            {
                switch (ContactType)
                {
                    case ContactTypeEnum.General: return "Općeniti";
                    case ContactTypeEnum.Delivery: return "Dostava eDokumenata";
                    case ContactTypeEnum.Sales: return "Prodaja";
                }
                return "Nije poznato";
            }
        }
    }
}