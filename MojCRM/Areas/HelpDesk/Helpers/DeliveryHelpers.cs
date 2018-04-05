using System;
using System.Linq;
using System.Web.Mvc;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Models;

namespace MojCRM.Areas.HelpDesk.Helpers
{
    public class DeliverySearchHelper
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string InvoiceNumber { get; set; }
        public string SentDate { get; set; }
        public string TicketDate { get; set; }
        public string BuyerEmail { get; set; }
        public string DocumentStatus { get; set; }
        public string DocumentType { get; set; }
        public string TicketType { get; set; }
        public string Assigned { get; set; }
        public string AssignedTo { get; set; }
    }

    public class ChangeEmailHelper
    {
        public int MerElectronicId { get; set; }
        public int ReceiverId { get; set; }
        public int TicketId { get; set; }
        public string OldEmail { get; set; }
        public string NewEmail { get; set; }
        public string InvoiceNumber { get; set; }
    }

    public class DeliveryDetailHelper
    {
        public int ReceiverId { get; set; }
        public int TicketId { get; set; }
        public int Contact { get; set; }
        public string DetailTemplate { get; set; }
        public string DetailNote { get; set; }
        public string InvoiceNumber { get; set; }
        public int? DetailNoteId { get; set; }
        public int? Identifier { get; set; }
    }

    public class DeliveryContactHelper
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string TitleFunction { get; set; }
        public int ReceiverId { get; set; }
        public int TicketId { get; set; }
        public int? ContactId { get; set; }
    }

    public class DeliveryIndexViewModel
    {
        public IQueryable<Delivery> Tickets { get; set; }
        public virtual IQueryable<ApplicationUser> Users { get; set; }
        public IQueryable<SelectListItem> DeliveryAgents
        {
            get
            {
                var list = (from u in Users
                    where u.Email != String.Empty
                    select new SelectListItem()
                    {
                        Text = u.UserName,
                        Value = u.UserName
                    });
                return list;
            }
        }
    }
}