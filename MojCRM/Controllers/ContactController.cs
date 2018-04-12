using MojCRM.Areas.HelpDesk.Helpers;
using MojCRM.Areas.Sales.Helpers;
using MojCRM.Models;
using MojCRM.ViewModels;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MojCRM.Helpers;

namespace MojCRM.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Contact
        [Authorize]
        public ActionResult Index()
        {
            var contacts = from c in _db.Contacts
                           select c;
            return View(contacts.ToList());
        }

        // GET: Contact/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contact/Create
        [HttpPost]
        public ActionResult Create(CreateContact newContact)
        {
            bool doNotCall = newContact.DoNotCall == 1;

            var contactType = (Contact.ContactTypeEnum) newContact.ContactType;

            _db.Contacts.Add(new Contact
            {
                OrganizationId = newContact.OrganizationId,
                ContactFirstName = newContact.ContactFirstName,
                ContactLastName = newContact.ContactLastName,
                Title = newContact.Title,
                TelephoneNumber = newContact.TelephoneNumber,
                MobilePhoneNumber = newContact.MobilePhoneNumber,
                Email = newContact.Email,
                User = newContact.User,
                DoNotCall = doNotCall,
                ContactType = contactType,
                InsertDate = DateTime.Now
            });
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: Contact/CreateFromDelivery
        [HttpPost]
        public ActionResult CreateFromDelivery(DeliveryContactHelper model)
        {
            try
            {
                _db.Contacts.Add(new Contact
                {
                    OrganizationId = model.ReceiverId,
                    ContactFirstName = model.FirstName,
                    ContactLastName = model.LastName,
                    Title = model.TitleFunction,
                    TelephoneNumber = model.Telephone,
                    MobilePhoneNumber = model.Mobile,
                    Email = model.Email,
                    User = User.Identity.Name,
                    InsertDate = DateTime.Now,
                    ContactType = Contact.ContactTypeEnum.Delivery,
                });

                _db.SaveChanges();

                return Redirect(Request.UrlReferrer?.ToString());
            }
            // TO DO: This catch part throws DbEntityValidationException in first foreach... I need to check why...
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    _db.LogError.Add(new LogError
                    {
                        Method = @"Contact - CreateFromDelivery",
                        Parameters = @"Id = " + model.ReceiverId + @" FirstName = '" + model.FirstName + @"' LastName = '" + model.LastName + @"' TelephoneNumber = '" + model.Telephone + @"' MobilePhoneNumber = '" + model.Mobile + @"' Email = '" + model.Email + @"' User = '" + User.Identity.Name + @"'",
                        Message = @"Entity of type '" + eve.Entry.Entity.GetType().Name + @"' in state '" + eve.Entry.State + @"' has following validation errors",
                        InnerException = "",
                        Request = "",
                        User = User.Identity.Name,
                        InsertDate = DateTime.Now
                    });
                    _db.SaveChanges();
                    foreach (var ve in eve.ValidationErrors)
                    {
                        _db.LogError.Add(new LogError
                        {
                            Method = @"Contact - CreateFromDelivery",
                            Parameters = "",
                            Message = "Property " + ve.PropertyName + " Error " + ve.ErrorMessage,
                            InnerException = "",
                            Request = "",
                            User = User.Identity.Name,
                            InsertDate = DateTime.Now
                        });
                        _db.SaveChanges();
                    }
                }
                throw;
            }
        }

        // POST: Contact/CreateFromDelivery
        [HttpPost]
        public ActionResult CreateFromSales(SalesContactHelper model)
        {
            var organizationId = (from o in _db.Opportunities
                                   where o.OpportunityId == model.RelatedEntityId
                                   select o.RelatedOrganizationId).First().ToString();

            _db.Contacts.Add(new Contact
            {
                OrganizationId = Int32.Parse(organizationId),
                ContactFirstName = model.FirstName,
                ContactLastName = model.LastName,
                Title = model.TitleFunction,
                TelephoneNumber = model.Telephone,
                MobilePhoneNumber = model.Mobile,
                Email = model.ContactEmail,
                User = User.Identity.Name,
                InsertDate = DateTime.Now,
                ContactType = Contact.ContactTypeEnum.Sales,
            });

            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        // POST: Contact/CreateFromDelivery
        [HttpPost]
        public ActionResult CreateFromSalesLead(SalesContactHelper model)
        {
            var organizationId = (from o in _db.Leads
                                   where o.LeadId == model.RelatedEntityId
                                   select o.RelatedOrganizationId).First().ToString();

            _db.Contacts.Add(new Contact
            {
                OrganizationId = Int32.Parse(organizationId),
                ContactFirstName = model.FirstName,
                ContactLastName = model.LastName,
                Title = model.TitleFunction,
                TelephoneNumber = model.Telephone,
                MobilePhoneNumber = model.Mobile,
                Email = model.ContactEmail,
                User = User.Identity.Name,
                InsertDate = DateTime.Now,
                ContactType = Contact.ContactTypeEnum.Sales,
            });

            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        // GET: Contact/Details
        public ActionResult Details(int contactId)
        {
            Contact contactModel = _db.Contacts.Find(contactId);
            if (contactModel == null)
            {
                return HttpNotFound();
            }

            var deliveryDetails = (from d in _db.DeliveryDetails
                                    where d.Contact == (contactModel.ContactFirstName + " " + contactModel.ContactLastName)
                                    select d).AsEnumerable();

            var contactDetails = new ContactDetailsViewModel
            {
                ContactId = contactModel.ContactId,
                ContactFirstName = contactModel.ContactFirstName,
                ContactLastName = contactModel.ContactLastName,
                Title = contactModel.Title,
                TelephoneNumber = contactModel.TelephoneNumber,
                MobilePhoneNumber = contactModel.MobilePhoneNumber,
                Email = contactModel.Email,
                User = contactModel.User,
                InsertDate = contactModel.InsertDate,
                UpdateDate = contactModel.UpdateDate,
                ContactType = contactModel.ContactType,
                DeliveryDetails = deliveryDetails
            };

            return View(contactDetails);
        }

        // GET: Coontact/Delete/5
        public ActionResult Delete(int? contactId)
        {
            if (contactId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = _db.Contacts.Find(contactId);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int contactId)
        {
            Contact contact = _db.Contacts.Find(contactId);
            _db.Contacts.Remove(contact ?? throw new InvalidOperationException());
            _db.SaveChanges();
            return View("Index");
        }

        // GET: Contact/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = _db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contact/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ContactId,ContactFirstName,ContactLastName,Title,TelephoneNumber,MobilePhoneNumber,Email,User,Agent,ContactType")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                var editedContact = (from c in _db.Contacts
                                     where c.ContactId == contact.ContactId
                                     select c).First();
                editedContact.ContactFirstName = contact.ContactFirstName;
                editedContact.ContactLastName = contact.ContactLastName;
                editedContact.Title = contact.Title;
                editedContact.TelephoneNumber = contact.TelephoneNumber;
                editedContact.MobilePhoneNumber = contact.MobilePhoneNumber;
                editedContact.Email = contact.Email;
                editedContact.User = contact.User;
                editedContact.UpdateDate = DateTime.Now;
                _db.SaveChanges();
                return RedirectToAction("Contacts", "Delivery");
            }
            return View(contact);
        }

        // POST: Contact/EditFromDelivery
        [HttpPost]
        public ActionResult EditFromDelivery(DeliveryContactHelper model)
        {
            var contactForUpdate = _db.Contacts.First(c => c.ContactId == model.ContactId);

            if (!String.IsNullOrEmpty(model.FirstName))
                contactForUpdate.ContactFirstName = model.FirstName;
            if (!String.IsNullOrEmpty(model.LastName))
                contactForUpdate.ContactLastName = model.LastName;
            if (!String.IsNullOrEmpty(model.Telephone))
                contactForUpdate.TelephoneNumber = model.Telephone;
            if (!String.IsNullOrEmpty(model.Mobile))
                contactForUpdate.MobilePhoneNumber = model.Mobile;
            if (!String.IsNullOrEmpty(model.Email))
                contactForUpdate.Email = model.Email;
            if (!String.IsNullOrEmpty(model.TitleFunction))
                contactForUpdate.Title = model.TitleFunction;

            contactForUpdate.UpdateDate = DateTime.Now;
            contactForUpdate.User = User.Identity.Name;
            _db.SaveChanges();

            return RedirectToAction("Details", "Delivery", new { area = "HelpDesk", id = model.TicketId, receiverId = model.ReceiverId });
        }

        // POST: Contact/EditFromSales
        [HttpPost]
        public ActionResult EditFromSales(SalesContactHelper model)
        {
            var contactId = Int32.Parse(model.ContactId);
            var contactForUpdate = _db.Contacts.First(c => c.ContactId == contactId);

            if (!String.IsNullOrEmpty(model.FirstName))
            {
                contactForUpdate.ContactFirstName = model.FirstName;
            }
            if (!String.IsNullOrEmpty(model.LastName))
            {
                contactForUpdate.ContactLastName = model.LastName;
            }
            if (!String.IsNullOrEmpty(model.Telephone))
            {
                contactForUpdate.TelephoneNumber = model.Telephone;
            }
            if (!String.IsNullOrEmpty(model.Mobile))
            {
                contactForUpdate.MobilePhoneNumber = model.Mobile;
            }
            if (!String.IsNullOrEmpty(model.ContactEmail))
            {
                contactForUpdate.Email = model.ContactEmail;
            }
            if (!String.IsNullOrEmpty(model.TitleFunction))
            {
                contactForUpdate.Title = model.TitleFunction;
            }
            contactForUpdate.User = User.Identity.Name;
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer?.ToString());
        }

        //// POST: Contact/EditFromSales
        //[HttpPost]
        //public ActionResult EditFromSalesLead(LeadContactHelper Model)
        //{
        //    var ContactForUpdate = (from c in db.Contacts
        //                            where (c.ContactFirstName + " " + c.ContactLastName) == Model.ContactId.ToString()
        //                            select c).First();

        //    if (!String.IsNullOrEmpty(Model.FirstName))
        //    {
        //        ContactForUpdate.ContactFirstName = Model.FirstName;
        //    }
        //    if (!String.IsNullOrEmpty(Model.LastName))
        //    {
        //        ContactForUpdate.ContactLastName = Model.LastName;
        //    }
        //    if (!String.IsNullOrEmpty(Model.Telephone))
        //    {
        //        ContactForUpdate.TelephoneNumber = Model.Telephone;
        //    }
        //    if (!String.IsNullOrEmpty(Model.Mobile))
        //    {
        //        ContactForUpdate.MobilePhoneNumber = Model.Mobile;
        //    }
        //    if (!String.IsNullOrEmpty(Model.Email))
        //    {
        //        ContactForUpdate.Email = Model.Email;
        //    }
        //    if (!String.IsNullOrEmpty(Model.Title))
        //    {
        //        ContactForUpdate.Title = Model.Title;
        //    }
        //    ContactForUpdate.User = User.Identity.Name;
        //    db.SaveChanges();

        //    return Redirect(Request.UrlReferrer.ToString());
        //}

        public JsonResult GetOrganization(string term = "")
        {
            var organizationList = _db.Organizations.Where(
                c => 
                    c.SubjectName.Contains(term) || 
                    c.VAT.Contains(term)
                    )
                    .Select(c => new { Naziv = c.SubjectName, OIB = c.VAT }).Distinct().ToList();
            return Json(organizationList, JsonRequestBehavior.AllowGet);
        }

        //get org by oib -- By: Tomislav Pribić
        public JsonResult GetOrganizationByOib(string term = "")
        {
            var organizationList = _db.Organizations.Where(
                    c =>
                        c.VAT.Contains(term)
                )
                .Select(c => new { Naziv = c.SubjectName, OIB = c.VAT }).Distinct().ToList();
            return Json(organizationList, JsonRequestBehavior.AllowGet);
        }
    }
}