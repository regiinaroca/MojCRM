using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using MojCRM.Models;
using ActiveUp.Net.Mail;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using MojCRM.Helpers;
using System.Data.Entity.Infrastructure;
using System.Text;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Areas.HelpDesk.ViewModels;
using MojCRM.Areas.HelpDesk.Helpers;
using MojCRM.Areas.Stats.ViewModels;

namespace MojCRM.Areas.HelpDesk.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly HelperMethods _helper = new HelperMethods();

        // GET: Delivery
        [Authorize]
        public ActionResult Index(string sortOrder, DeliverySearchHelper model)
        {
            ViewBag.InsertDateParm = String.IsNullOrEmpty(sortOrder) ? "InsertDate" : String.Empty;

            var referenceDate = new DateTime(2017, (DateTime.Today.AddMonths(-2)).Month, 1);
            var resultsNew = _db.DeliveryTicketModels.Where(t => t.InsertDate >= referenceDate);
            var openTickets = _db.DeliveryTicketModels.Where(t => t.DocumentStatus == 30);
            var ticketsCreatedToday = _db.DeliveryTicketModels.Where(t => t.InsertDate > DateTime.Today.Date);
            var ticketsCreatedTodayFirstTime = _db.DeliveryTicketModels.Where(t => t.InsertDate > DateTime.Today.Date && t.FirstInvoice);

            ViewBag.OpenTickets = openTickets.Count();
            ViewBag.OpenTicketsAssigned = openTickets.Count(t => t.IsAssigned);
            ViewBag.TicketsCreatedToday = ticketsCreatedToday.Count();
            ViewBag.TicketsCreatedTodayAssigned = ticketsCreatedToday.Count(t => t.IsAssigned);
            ViewBag.TicketsCreatedTodayFirstTime = ticketsCreatedTodayFirstTime.Count();
            ViewBag.TicketsCreatedTodayFirstTimeAssigned = ticketsCreatedTodayFirstTime.Count(t => t.IsAssigned);
            ViewBag.TotalOpenedTickets = _db.DeliveryTicketModels.Count(t => t.IsAssigned == false && t.InsertDate >= referenceDate && t.DocumentStatus == 30);

            var documentStatusInt = !string.IsNullOrEmpty(model.DocumentStatus) ? int.Parse(model.DocumentStatus) : 30;

            var documentTypeInt = !string.IsNullOrEmpty(model.DocumentType) ? int.Parse(model.DocumentType) : 1;

            if (!string.IsNullOrEmpty(model.Sender))
            {
                resultsNew = resultsNew.Where(t => t.Sender.SubjectName.StartsWith(model.Sender) || t.Sender.VAT.StartsWith(model.Sender));
                ViewBag.SearchResults = resultsNew.Count();
                ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);
            }

            if (!string.IsNullOrEmpty(model.Receiver))
            {
                resultsNew = resultsNew.Where(t => t.Receiver.SubjectName.StartsWith(model.Receiver) || t.Receiver.VAT.StartsWith(model.Receiver));
                ViewBag.SearchResults = resultsNew.Count();
                ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);
            }

            if (!string.IsNullOrEmpty(model.InvoiceNumber))
            {
                resultsNew = resultsNew.Where(t => t.InvoiceNumber.StartsWith(model.InvoiceNumber));
                ViewBag.SearchResults = resultsNew.Count();
                ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);
            }

            if (!string.IsNullOrEmpty(model.SentDate))
            {
                var sentDate = Convert.ToDateTime(model.SentDate);
                var sentDatePlus = sentDate.AddDays(1);
                resultsNew = resultsNew.Where(t => (t.SentDate > sentDate) && (t.SentDate < sentDatePlus));
                ViewBag.SearchResults = resultsNew.Count();
                ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);
            }

            if (!string.IsNullOrEmpty(model.TicketDate))
            {
                var insertDate = Convert.ToDateTime(model.TicketDate);
                var insertDatePlus = insertDate.AddDays(1);
                resultsNew = resultsNew.Where(t => (t.InsertDate > insertDate) && (t.InsertDate < insertDatePlus));
                ViewBag.SearchResults = resultsNew.Count();
                ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);
            }

            if (!string.IsNullOrEmpty(model.BuyerEmail))
            {
                resultsNew = resultsNew.Where(t => t.BuyerEmail.Contains(model.BuyerEmail));
                ViewBag.SearchResults = resultsNew.Count();
                ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);
            }

            if (!string.IsNullOrEmpty(model.DocumentStatus))
            {
                resultsNew = resultsNew.Where(t => t.DocumentStatus == documentStatusInt);
                ViewBag.SearchResults = resultsNew.Count();
                ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);
            }

            if (!string.IsNullOrEmpty(model.DocumentType))
            {
                resultsNew = resultsNew.Where(t => t.MerDocumentTypeId == documentTypeInt);
                ViewBag.SearchResults = resultsNew.Count();
                ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);
            }

            if (!string.IsNullOrEmpty(model.TicketType))
            {
                resultsNew = resultsNew.Where(t => t.FirstInvoice);
                ViewBag.SearchResults = resultsNew.Count();
                ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);
            }

            if (!string.IsNullOrEmpty(model.Assigned))
            {
                if (model.Assigned == "1")
                {
                    resultsNew = resultsNew.Where(t => t.IsAssigned && t.AssignedTo == User.Identity.Name);
                    ViewBag.SearchResults = resultsNew.Count();
                    ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);
                }
                if (model.Assigned == "2")
                {
                    resultsNew = resultsNew.Where(t => t.IsAssigned == false);
                    ViewBag.SearchResults = resultsNew.Count();
                    ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);
                }
            }

            if (!string.IsNullOrEmpty(model.AssignedTo))
            {
                resultsNew = resultsNew.Where(t => t.IsAssigned && t.AssignedTo == model.AssignedTo);
                ViewBag.SearchResults = resultsNew.Count();
                ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);
            }

            ViewBag.SearchResults = resultsNew.Count();
            ViewBag.SearchResultsAssigned = resultsNew.Count(t => t.IsAssigned);

            switch (sortOrder)
            {
                case "InsertDate":
                    resultsNew = resultsNew.OrderBy(d => d.InsertDate);
                    break;
                default:
                    resultsNew = resultsNew.OrderByDescending(d => d.InsertDate);
                    break;
            }

            var returnModel = new DeliveryIndexViewModel()
            {
                Tickets = resultsNew,
                Users = _db.Users
            };

            return View(returnModel);
        }

        // GET: Delivery/CreateTicketsFirstTime
        // Kreiranje kartica za prvo preuzimanje
        //[Authorize(Roles = "Superadmin")]
        public JsonResult CreateTicketsFirstTime(Guid? user)
        {
            var credentials = new { MerUser = "", MerPass = "" };
            string agent;
            if (String.IsNullOrEmpty(user.ToString()))
            {
                credentials = (from u in _db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
                agent = User.Identity.Name;
            }
            else
            {
                credentials = (from u in _db.Users
                               where u.Id == user.ToString()
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
                agent = user.ToString();
            }

            int createdTickets;
            MerApiGetNondeliveredDocuments requestFirstTime = new MerApiGetNondeliveredDocuments()
            {
                Id = credentials.MerUser,
                Pass = credentials.MerPass,
                Oib = "99999999927",
                PJ = "",
                SoftwareId = "MojCRM-001",
                Type = 1
            };

            string merRequestFirstTime = JsonConvert.SerializeObject(requestFirstTime);

            using (var mer = new WebClient() { Encoding = Encoding.UTF8 })
            {
                mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var responseFirstTime = mer.UploadString(new Uri(App.MerEndpoint + @"apis/v21/getNondeliveredDocuments").ToString(), "POST", merRequestFirstTime);
                //var responseFirstTime = mer.UploadString(new Uri(@"http://localhost/apis/v21/getNondeliveredDocuments").ToString(), "POST", merRequestFirstTime);
                //ResponseFirstTime = ResponseFirstTime.Replace("[", "").Replace("]", "");
                MerGetNondeliveredDocumentsResponse[] resultsFirstTime = JsonConvert.DeserializeObject<MerGetNondeliveredDocumentsResponse[]>(responseFirstTime);
                foreach (var result in resultsFirstTime)
                {
                    _db.DeliveryTicketModels.Add(new Delivery
                    {
                        SenderId = result.PosiljateljId,
                        ReceiverId = result.PrimateljId,
                        InvoiceNumber = result.InterniBroj,
                        MerElectronicId = result.Id,
                        SentDate = result.DatumOtpreme,
                        MerDocumentTypeId = result.DokumentTypeId,
                        DocumentStatus = 30,
                        InsertDate = DateTime.Now,
                        BuyerEmail = result.EmailPrimatelja,
                        FirstInvoice = true,
                    });
                }
                _db.SaveChanges();
                createdTickets = resultsFirstTime.Count();
            }

            _helper.LogActivity("Kreirano kartica za prvi put: " + createdTickets, agent, 0, ActivityLog.ActivityTypeEnum.System, ActivityLog.DepartmentEnum.MojCrm, ActivityLog.ModuleEnum.Delivery);

            return Json(new { Status = "OK", CreatedTickets = createdTickets }, JsonRequestBehavior.AllowGet);
        }

        // GET: Delivery/CreateTickets
        // Kreiranje kartica za redovito preuzimanje
        //[Authorize(Roles = "Superadmin")]
        public JsonResult CreateTickets(Guid? user)
        {
            var credentials = new { MerUser = "", MerPass = ""};
            string agent;
            if (string.IsNullOrEmpty(user.ToString()))
            {
                credentials = (from u in _db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
                agent = User.Identity.Name;
            }
            else
            {
                credentials = (from u in _db.Users
                               where u.Id == user.ToString()
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
                agent = user.ToString();
            }

            var createdTickets = 0;
            var requestRegularDelivery = new MerApiGetNondeliveredDocuments()
            {
                Id = credentials.MerUser,
                Pass = credentials.MerPass,
                Oib = "99999999927",
                PJ = "",
                SoftwareId = "MojCRM-001",
                Type = 2
            };

            var merRequestFirstTime = JsonConvert.SerializeObject(requestRegularDelivery);

            using (var mer = new WebClient() { Encoding = Encoding.UTF8 })
            {
                mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var responseRegularDelivery = mer.UploadString(new Uri(App.MerEndpoint + @"apis/v21/getNondeliveredDocuments").ToString(), "POST", merRequestFirstTime);
                //var responseRegularDelivery = mer.UploadString(new Uri(@"http://localhost/apis/v21/getNondeliveredDocuments").ToString(), "POST", merRequestFirstTime);
                //ResponseRegularDelivery = ResponseRegularDelivery.Replace("[", "").Replace("]", "");
                MerGetNondeliveredDocumentsResponse[] resultsRegularDelivery = JsonConvert.DeserializeObject<MerGetNondeliveredDocumentsResponse[]>(responseRegularDelivery);
                foreach (var result in resultsRegularDelivery)
                {
                    var exists = CheckIfExists(result.Id);

                    if (exists)
                    {
                        var ticket = _db.DeliveryTicketModels.First(x => x.MerElectronicId == result.Id);
                        if (ticket.DocumentStatus != 30)
                            ticket.DocumentStatus = 30;
                        ticket.UpdateDate = DateTime.Now;
                        _db.DeliveryDetails.Add(new DeliveryDetail
                        {
                            User = @"Moj-CRM",
                            InsertDate = DateTime.Now,
                            TicketId = ticket.Id,
                            ReceiverId = ticket.ReceiverId,
                            DetailNote = @"Ovoj je kartici resetiran status zbog ponovnog slanja obavijesti o dostavi eRačuna",
                            Contact = ""
                        });
                    }
                    else
                    {
                        _db.DeliveryTicketModels.Add(new Delivery
                        {
                            SenderId = result.PosiljateljId,
                            ReceiverId = result.PrimateljId,
                            InvoiceNumber = result.InterniBroj,
                            MerElectronicId = result.Id,
                            SentDate = result.DatumOtpreme,
                            MerDocumentTypeId = result.DokumentTypeId,
                            DocumentStatus = 30,
                            InsertDate = DateTime.Now,
                            BuyerEmail = result.EmailPrimatelja,
                            FirstInvoice = false,
                        });
                    }
                }
                _db.SaveChanges();
                createdTickets = resultsRegularDelivery.Length;
            }

            _helper.LogActivity("Kreirano kartica: " + createdTickets, agent, 0, ActivityLog.ActivityTypeEnum.System, ActivityLog.DepartmentEnum.MojCrm, ActivityLog.ModuleEnum.Delivery);

            return Json(new { Status = "OK", CreatedTickets = createdTickets }, JsonRequestBehavior.AllowGet);
        }

        // GET: Delivery/CreateTicketsOld
        [Authorize(Roles = "Superadmin")]
        [Obsolete]
        public ActionResult CreateTicketsOld(string Name)
        {
            var MerUser = (from u in _db.Users
                           where u.UserName == Name
                           select u.MerUserUsername).First();
            var MerPass = (from u in _db.Users
                           where u.UserName == Name
                           select u.MerUserPassword).First();

            var Organizations = (from o in _db.Organizations
                                select o).AsEnumerable();

            using (Imap4Client imap = new Imap4Client())
            {
                imap.Connect("mail.moj-eracun.hr");
                imap.Login("dostava@moj-eracun.hr", "m0j.d05tava");
                Mailbox inbox = imap.SelectMailbox("Inbox");

                string MerPattern1 = ".*?";
                string MerPattern2 = "((?:http|https)(?::\\/{2}[\\w]+)(?:[\\/|\\.]?)(?:[^\\s\"]*))";
                Regex MerLink = new Regex(MerPattern1 + MerPattern2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                string MerCheck1 = "(Return)(-)(Path)(:)( )(dostava@moj-eracun\\.hr)";
                Regex MerChecker = new Regex(MerCheck1, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                int OrdinalNumber = inbox.MessageCount;
                int ReferenceOrdinalNumber = 96600;
                DateTime ReferenceDate = DateTime.Today;
                string DeliveryLink;
                DateTime Date = DateTime.Parse(inbox.Fetch.InternalDate(OrdinalNumber));

                while (Date > ReferenceDate)
                {
                    for (OrdinalNumber = inbox.MessageCount; Date > ReferenceDate && OrdinalNumber > ReferenceOrdinalNumber; OrdinalNumber--)
                    {
                        string Body = inbox.Fetch.MessageString(OrdinalNumber);
                        try
                        {
                            Match MerLinkMatch = MerLink.Match(Body);
                            Match MerCheckerMatch = MerChecker.Match(Body);
                            if (MerLinkMatch.Success && MerCheckerMatch.Success)
                            {
                                DeliveryLink = MerLinkMatch.Groups[1].ToString();
                                MerDeliveryJsonResponse Result = ParseJson(DeliveryLink);

                                bool ExistingOrganization = Organizations.Any(o => o.MerId.ToString() == Result.BuyerID.ToString());

                                if (ExistingOrganization)
                                {
                                    _db.DeliveryTicketModels.Add(new Delivery
                                    {
                                        SenderId = Int32.Parse(Result.SupplierID),
                                        ReceiverId = Int32.Parse(Result.BuyerID),
                                        InvoiceNumber = Result.InterniBroj,
                                        MerLink = DeliveryLink,
                                        MerJson = Result,
                                        MerElectronicId = Result.Id,
                                        SentDate = Result.IssueDate,
                                        MerDocumentTypeId = Result.Type,
                                        DocumentStatus = Result.Status,
                                        InsertDate = DateTime.Now,
                                        BuyerEmail = Result.EmailPrimatelja,
                                    });
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    MerApiGetSubjekt Request = new MerApiGetSubjekt()
                                    {
                                        Id = MerUser.ToString(),
                                        Pass = MerPass.ToString(),
                                        Oib = "99999999927",
                                        PJ = "",
                                        SoftwareId = "MojCRM-001",
                                        SubjektPJ = Result.BuyerID
                                    };

                                    string MerRequest = JsonConvert.SerializeObject(Request);

                                    using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
                                    {
                                        Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                        Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                                        var Response = Mer.UploadString(new Uri(App.MerEndpoint + @"apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                                        Response = Response.Replace("[", "").Replace("]", "");
                                        MerGetSubjektDataResponse ResultOrg = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(Response);
                                        _db.Organizations.Add(new Organizations
                                        {
                                            MerId = ResultOrg.Id,
                                            SubjectName = ResultOrg.Naziv,
                                            SubjectBusinessUnit = ResultOrg.PoslovnaJedinica,
                                            VAT = ResultOrg.Oib
                                        });
                                        _db.SaveChanges();

                                        _db.DeliveryTicketModels.Add(new Delivery
                                        {
                                            SenderId = Int32.Parse(Result.SupplierID),
                                            ReceiverId = Int32.Parse(Result.BuyerID),
                                            InvoiceNumber = Result.InterniBroj,
                                            MerLink = DeliveryLink,
                                            MerJson = Result,
                                            MerElectronicId = Result.Id,
                                            SentDate = Result.IssueDate,
                                            MerDocumentTypeId = Result.Type,
                                            DocumentStatus = Result.Status,
                                            InsertDate = DateTime.Now,
                                            BuyerEmail = Result.EmailPrimatelja,
                                        });
                                        _db.SaveChanges();
                                    }
                                }
                            }
                            //TO DO: Remove else and add it to catch segment
                            else
                            {
                                _db.LogError.Add(new LogError
                                {
                                    Method = @"Delivery - CreateTickets",
                                    Parameters = String.Empty,
                                    Message = "Moj-CRM was unable to generete ticket",
                                    InnerException = String.Empty,
                                    Request = String.Empty,
                                    User = "Moj-CRM",
                                    InsertDate = DateTime.Now,
                                });
                            }
                            }
                        //TO DO: Add Exceptions
                        catch
                        {
                                throw;
                        }
                        Date = DateTime.Parse(inbox.Fetch.InternalDate(OrdinalNumber));
                    }
                }
                imap.Disconnect();
            }

            var Count = (from t in _db.DeliveryTicketModels
                         where t.InsertDate > DateTime.Today
                         select t).Count();
            ViewBag.Count = Count;

            return View();
        }

        // GET: Delivery/UpdateStatusIndex/12345
        public ActionResult UpdateStatusIndex(int id)
        {
            var ticketForUpdate = _db.DeliveryTicketModels.First(x => x.Id == id);
            var merString = App.MerEndpoint + "exchange/getstatus?id=" + ticketForUpdate.MerElectronicId + "&ver=5115e32c-6be4-4a92-8e92-afe122e99d1c";

            MerDeliveryJsonResponse response = ParseJson(merString);

            ticketForUpdate.DocumentStatus = response.Status;
            ticketForUpdate.BuyerEmail = response.EmailPrimatelja;
            ticketForUpdate.UpdateDate = DateTime.Now;
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Delivery/UpdateStatus/12345
        public ActionResult UpdateStatusDetails(int TicketId, int ReceiverId)
        {
            var TicketForUpdate = _db.DeliveryTicketModels.Find(TicketId);
            var MerString = App.MerEndpoint + "exchange/getstatus?id=" + TicketForUpdate.MerElectronicId + "&ver=5115e32c-6be4-4a92-8e92-afe122e99d1c";

            MerDeliveryJsonResponse Response = ParseJson(MerString);

            TicketForUpdate.DocumentStatus = Response.Status;
            TicketForUpdate.BuyerEmail = Response.EmailPrimatelja;
            TicketForUpdate.UpdateDate = DateTime.Now;
            _db.SaveChanges();

            return RedirectToAction("Details", new { id = TicketId, receiverId = ReceiverId, Name = User.Identity.Name });
        }

        public void UpdateStatus(int Id)
        {
            var TicketForUpdate = _db.DeliveryTicketModels.Find(Id);
            var MerString = App.MerEndpoint + "exchange/getstatus?id=" + TicketForUpdate.MerElectronicId + "&ver=5115e32c-6be4-4a92-8e92-afe122e99d1c";

            MerDeliveryJsonResponse Response = ParseJson(MerString);

            TicketForUpdate.DocumentStatus = Response.Status;
            TicketForUpdate.BuyerEmail = Response.EmailPrimatelja;
            TicketForUpdate.UpdateDate = DateTime.Now;
            _db.SaveChanges();
        }

        // GET: Delivery/UpdateAllStatuses
        public ActionResult UpdateAllStatusesSent()
        {
            var openTickets = from t in _db.DeliveryTicketModels
                              where t.DocumentStatus == 30
                              select t;

            foreach (var ticket in openTickets)
            {
                //var TicketForUpdate = db.DeliveryTicketModels.Find(Ticket.Id);
                var merString = App.MerEndpoint + "exchange/getstatus?id=" + ticket.MerElectronicId + "&ver=13ca6cad-60a4-4894-ba38-1a6f86b25a3c";
                MerDeliveryJsonResponse result = ParseJson(merString);

                ticket.DocumentStatus = result.Status;
                ticket.BuyerEmail = result.EmailPrimatelja;
                ticket.UpdateDate = DateTime.Now;
            }
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Delivery/UpdateAllStatuses
        public ActionResult UpdateAllStatusesOther()
        {
            var ReferenceDate = new DateTime(2017, (DateTime.Today.AddMonths(-2)).Month, 1);
            var OpenTickets = _db.DeliveryTicketModels.Where(t => t.DocumentStatus != 30 && t.DocumentStatus != 40 && t.DocumentStatus != 55);

            foreach (var Ticket in OpenTickets)
            {
                var MerString = App.MerEndpoint + "exchange/getstatus?id=" + Ticket.MerElectronicId + "&ver=13ca6cad-60a4-4894-ba38-1a6f86b25a3c";
                MerDeliveryJsonResponse Result = ParseJson(MerString);

                Ticket.DocumentStatus = Result.Status;
                Ticket.BuyerEmail = Result.EmailPrimatelja;
                Ticket.UpdateDate = DateTime.Now;
            }
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Delivery/Resend/12345
        [Authorize]
        public ActionResult Resend(int TicketId, int MerElectronicId, int ReceiverId, bool IsTicket = true)
        {
            var InvoiceNumber = "";
            var Credentials = new { MerUser = "", MerPass = "" };
            Credentials = (from u in _db.Users
                           where u.UserName == User.Identity.Name
                           select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();

            if (IsTicket == true)
            {
                InvoiceNumber = (from t in _db.DeliveryTicketModels
                                     where t.Id == TicketId
                                     select t.InvoiceNumber).First();
            }
            else
            {
                InvoiceNumber = null;
            }
            var Receiver = (from o in _db.Organizations
                            where o.MerId == ReceiverId //&& o.SubjectBusinessUnit == ""
                            select o.SubjectName).First();

            MerApiResend Request = new MerApiResend()
            {
                Id = Credentials.MerUser,
                Pass = Credentials.MerPass,
                Oib = "99999999927",
                PJ = "",
                SoftwareId = "MojCRM-001",
                DocumentId = MerElectronicId
            };

            string MerRequest = JsonConvert.SerializeObject(Request);

            using (var Mer = new WebClient())
            {
                Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                Mer.UploadString(new Uri(App.MerEndpoint + @"apis/v21/changeEmail").ToString(), "POST", MerRequest);
            }

            if (InvoiceNumber == null)
            {
                _helper.LogActivity(User.Identity.Name + " je ponovno poslao obavijest za primatelja " + Receiver, User.Identity.Name, TicketId, ActivityLog.ActivityTypeEnum.Resend, ActivityLog.DepartmentEnum.Delivery, ActivityLog.ModuleEnum.Delivery);
            }
            else
            {
                _helper.LogActivity(User.Identity.Name + " je ponovno poslao obavijest za dokument " + InvoiceNumber, User.Identity.Name, TicketId, ActivityLog.ActivityTypeEnum.Resend, ActivityLog.DepartmentEnum.Delivery, ActivityLog.ModuleEnum.Delivery);
            }

            return RedirectToAction("Details", new { id = TicketId, receiverId = ReceiverId });
        }

        // GET Delivery/ChangeEmail
        [Authorize]
        public ActionResult ChangeEmail(ChangeEmailHelper model)
        {
            var credentials = (from u in _db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();

            MerApiChangeEmail request = new MerApiChangeEmail()
            {
                Id = credentials.MerUser,
                Pass = credentials.MerPass,
                Oib = "99999999927",
                PJ = "",
                SoftwareId = "MojCRM-001",
                DocumentId = model.MerElectronicId,
                Email = model.NewEmail
            };

            string merRequest = JsonConvert.SerializeObject(request);

            using (var mer = new WebClient())
            {
                mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                mer.UploadString(new Uri(App.MerEndpoint + @"apis/v21/changeEmail").ToString(), "POST", merRequest);
            }

            _helper.LogActivity(User.Identity.Name + " je izmijenio e-mail adresu za dostavu eDokumenta iz " + model.OldEmail + " u " + model.NewEmail + " i ponovno poslao obavijest za dokument broj: " + model.InvoiceNumber,
                User.Identity.Name, model.TicketId, ActivityLog.ActivityTypeEnum.Mailchange, ActivityLog.DepartmentEnum.Delivery, ActivityLog.ModuleEnum.Delivery);
            UpdateStatus(model.TicketId);

            return RedirectToAction("Details", new { id = model.TicketId, receiverId = model.ReceiverId });
        }

        // GET: Delivery/ChangeEmailNoTicket
        [Authorize]
        public ActionResult ChangeEmailNoTicket(ChangeEmailHelper model)
        {
            var credentials = (from u in _db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();

            MerApiChangeEmail request = new MerApiChangeEmail()
            {
                Id = credentials.MerUser,
                Pass = credentials.MerPass,
                Oib = "99999999927",
                PJ = "",
                SoftwareId = "MojCRM-001",
                DocumentId = model.MerElectronicId,
                Email = model.NewEmail
            };

            string merRequest = JsonConvert.SerializeObject(request);

            using (var mer = new WebClient())
            {
                mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                mer.UploadString(new Uri(App.MerEndpoint + @"apis/v21/changeEmail").ToString(), "POST", merRequest);
            }

            _helper.LogActivity(User.Identity.Name + " je izmijenio e-mail adresu za dostavu eDokumenta iz " + model.OldEmail + " u " + model.NewEmail + " i ponovno poslao obavijest za dokument broj: " + model.InvoiceNumber,
                User.Identity.Name, 0, ActivityLog.ActivityTypeEnum.Mailchange, ActivityLog.DepartmentEnum.Delivery, ActivityLog.ModuleEnum.Delivery);

            return RedirectToAction("Details", new { id = model.TicketId, receiverId = model.ReceiverId });
        }

        // POST Delivery/Remove/1125768
        [HttpPost]
        public JsonResult Remove(int merElectronicId, int ticketId)
        {
            var ticketForRemoval = _db.DeliveryTicketModels.First(t => t.Id == ticketId);
            ticketForRemoval.DocumentStatus = 55;
            ticketForRemoval.UpdateDate = DateTime.Now;
            _db.SaveChanges();

            return Json(new { Status = "OK" });
        }

        // POST Delivery/RemoveAll
        [HttpPost]
        public JsonResult RemoveAll(int[] TicketIds)
        {
            foreach (var Id in TicketIds)
            {
                var TicketForRemoval = _db.DeliveryTicketModels.Find(Id);
                TicketForRemoval.DocumentStatus = 55;
                TicketForRemoval.UpdateDate = DateTime.Now;
            }
            _db.SaveChanges();

            return Json(new { Status = "OK" });
        }

        // POST Delivery/AssignAll
        [HttpPost]
        public JsonResult AssignAll(int[] TicketIds)
        {
            foreach (var Id in TicketIds)
            {
                var TicketForAssign = _db.DeliveryTicketModels.Find(Id);
                TicketForAssign.IsAssigned = true;
                TicketForAssign.AssignedTo = User.Identity.Name;

                _db.ActivityLogs.Add(new ActivityLog()
                {
                    Description = "Agent " + User.Identity.Name + " si je dodijelio karticu za primatelja: " + TicketForAssign.Receiver.SubjectName + ", za eDokument: " + TicketForAssign.InvoiceNumber,
                    User = User.Identity.Name,
                    ReferenceId = Id,
                    ActivityType = ActivityLog.ActivityTypeEnum.Ticketassign,
                    Department = ActivityLog.DepartmentEnum.Delivery,
                    Module = ActivityLog.ModuleEnum.Delivery,
                    InsertDate = DateTime.Now
                });
            }
            _db.SaveChanges();

            return Json(new { Status = "OK" });
        }

        // GET: Delivery/Details/5
        [Authorize]
        public ActionResult Details(int id, int? receiverId)
        {
            Delivery deliveryTicketModel = _db.DeliveryTicketModels.Find(id);
            if (deliveryTicketModel == null)
            {
                return HttpNotFound();
            }

            //UpdateStatus(ElectronicId);

            DateTime referenceDate = DateTime.Now.AddMonths(-2);

            var relatedInvoicesList = (from t in _db.DeliveryTicketModels
                                        where t.Id != id && t.ReceiverId == deliveryTicketModel.ReceiverId && t.SentDate > referenceDate && t.DocumentStatus == 30
                                        select t);

            var relatedDeliveryContacts = (from t in _db.Contacts
                                            where t.Organization.MerId == receiverId && t.ContactType == Contact.ContactTypeEnum.Delivery
                                            select t);

            var relatedDeliveryDetails = (from t in _db.DeliveryDetails
                                           where t.Receiver.MerId == receiverId
                                           select t).OrderByDescending(t => t.Id).ToList();

            relatedDeliveryDetails.RemoveAll(x =>
                x.TicketId != id && x.DetailNote ==
                @"Ovoj je kartici resetiran status zbog ponovnog slanja obavijesti o dostavi eRačuna");

            var importantComment = (from dd in _db.MerDeliveryDetails
                                     where dd.MerId == receiverId
                                     select dd.ImportantComments).First();

            var relatedActivities = (from a in _db.ActivityLogs
                                      where a.ReferenceId == id && a.Module == ActivityLog.ModuleEnum.Delivery
                                      select a).OrderByDescending(a => a.Id);
            var users = _db.Users;

            //#region Postmark API
            //MessagesOutboundOpenResponse openingHistoryResponse;
            //BouncesResponse bounces;
            //string postmarkLinkOpeningHistory = @"https://api.postmarkapp.com/messages/outbound/opens?count=20&offset=0&recipient=" + deliveryTicketModel.BuyerEmail;
            //string postmarkBounces = @"https://api.postmarkapp.com/bounces?count=20&offset=0&emailFilter=" + deliveryTicketModel.BuyerEmail;
            //using (var postmark = new WebClient())
            //{
            //    postmark.Headers.Add(HttpRequestHeader.Accept, "application/json");
            //    postmark.Headers.Add("X-Postmark-Server-Token", "8ab33a3d-a800-405f-afad-9c75c2f08c0b");
            //    var openingHistoryRequest = postmark.DownloadString(postmarkLinkOpeningHistory);
            //    openingHistoryResponse = JsonConvert.DeserializeObject<MessagesOutboundOpenResponse>(openingHistoryRequest);
            //}

            //using (var postmark = new WebClient())
            //{
            //    postmark.Headers.Add(HttpRequestHeader.Accept, "application/json");
            //    postmark.Headers.Add("X-Postmark-Server-Token", "8ab33a3d-a800-405f-afad-9c75c2f08c0b");
            //    var bouncesRequest = postmark.DownloadString(postmarkBounces);
            //    bounces = JsonConvert.DeserializeObject<BouncesResponse>(bouncesRequest);
            //}
            //#endregion

            var credentials = (from u in _db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();

            MerApiGetSentDocuments requestGetSentDocuments = new MerApiGetSentDocuments()
            {
                Id = credentials.MerUser,
                Pass = credentials.MerPass,
                Oib = "99999999927",
                PJ = "",
                SoftwareId = "MojCRM-001",
                SubjektPJ = receiverId.ToString(),
                Take = 20
            };

            string merRequest = JsonConvert.SerializeObject(requestGetSentDocuments);

            using (var mer = new WebClient() { Encoding = Encoding.UTF8 })
            {
                MerGetSentDocumentsResponse[] resultsDocumentHistory;
                if (deliveryTicketModel.LastGetSentDocumentsDate == null || (int)DateTime.Now.Subtract((DateTime)deliveryTicketModel.LastGetSentDocumentsDate).TotalHours > 3)
                {
                    mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                    var responseRegularDelivery = mer.UploadString(new Uri(App.MerEndpoint + @"apis/v21/getSentDocuments").ToString(), "POST", merRequest);
                    //ResponseRegularDelivery = ResponseRegularDelivery.Replace("[", "").Replace("]", "");
                    resultsDocumentHistory = JsonConvert.DeserializeObject<MerGetSentDocumentsResponse[]>(responseRegularDelivery);

                    deliveryTicketModel.GetSentDocumentsResult = responseRegularDelivery;
                    deliveryTicketModel.UpdateDate = DateTime.Now;
                    deliveryTicketModel.LastGetSentDocumentsDate = DateTime.Now;
                    _db.SaveChanges();
                }
                else
                {
                    resultsDocumentHistory = JsonConvert.DeserializeObject<MerGetSentDocumentsResponse[]>(deliveryTicketModel.GetSentDocumentsResult);
                }


                var deliveryDetails = new DeliveryDetailsViewModel
                {
                    TicketId = id,
                    SenderName = deliveryTicketModel.Sender.SubjectName,
                    ReceiverName = deliveryTicketModel.Receiver.SubjectName,
                    InvoiceNumber = deliveryTicketModel.InvoiceNumber,
                    SentDate = deliveryTicketModel.SentDate,
                    MerDocumentTypeId = deliveryTicketModel.MerDocumentTypeId,
                    MerDocumentStatusId = deliveryTicketModel.DocumentStatus,
                    ReceiverEmail = deliveryTicketModel.BuyerEmail,
                    //MerDeliveryDetailComment = deliveryTicketModel.Receiver.MerDeliveryDetail.Comments,
                    MerDeliveryDetailTelephone = deliveryTicketModel.Receiver.MerDeliveryDetail.Telephone,
                    TelephoneNumber = deliveryTicketModel.Receiver.OrganizationDetail.TelephoneNumber,
                    MobilePhoneNumber = deliveryTicketModel.Receiver.OrganizationDetail.MobilePhoneNumber,
                    ImportantComment = importantComment,
                    MerElectronicId = deliveryTicketModel.MerElectronicId,
                    ReceiverId = deliveryTicketModel.ReceiverId,
                    ReceiverVAT = deliveryTicketModel.Receiver.VAT,
                    SenderVAT = deliveryTicketModel.Sender.VAT,
                    RelatedInvoices = relatedInvoicesList,
                    RelatedDeliveryContacts = relatedDeliveryContacts,
                    RelatedDeliveryDetails = relatedDeliveryDetails.AsQueryable(),
                    RelatedActivities = relatedActivities,
                    IsAssigned = deliveryTicketModel.IsAssigned,
                    AssignedTo = deliveryTicketModel.AssignedTo,
                    DocumentHistory = resultsDocumentHistory.Where(i => (i.DokumentStatusId != 10) && (/*i.DokumentTypeId != 6 && -- This was removed from API on Moj-eRačun*/ i.DokumentTypeId != 632)).AsQueryable(),
                    //PostmarkOpenings = openingHistoryResponse,
                    //PostmarkBounces = bounces,
                    Users = users
                };

                return View(deliveryDetails);
            }
        }

        // GET: Delivery/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AddDetail/1125768
        [HttpPost]
        public ActionResult AddDetail(DeliveryDetailHelper model)
        {
            var contact = _db.Contacts.Find(model.Contact);
            if (model.DetailTemplate == String.Empty && model.DetailNote != String.Empty)
            {
                try
                {
                    _db.DeliveryDetails.Add(new DeliveryDetail
                    {
                        ReceiverId = model.ReceiverId,
                        User = User.Identity.Name,
                        DetailNote = model.DetailNote,
                        InsertDate = DateTime.Now,
                        Contact = contact.ContactFirstName + " " + contact.ContactLastName,
                        TicketId = model.TicketId
                    });
                    _db.SaveChanges();
                }
                catch (NullReferenceException ex)
                {
                    _db.LogError.Add(new LogError
                    {
                        Method = @"Delivery - AddDetail",
                        Parameters = ex.Source,
                        Message = ex.Message,
                        InnerException = String.Empty,
                        Request = ex.TargetSite.ToString(),
                        User = User.Identity.Name,
                        InsertDate = DateTime.Now
                    });
                    _db.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    _db.LogError.Add(new LogError
                    {
                        Method = @"Delivery - AddDetail",
                        Parameters = ex.Source,
                        Message = ex.Message,
                        InnerException = ex.InnerException.ToString(),
                        Request = ex.TargetSite.ToString(),
                        User = User.Identity.Name,
                        InsertDate = DateTime.Now
                    });
                    _db.SaveChanges();
                }
            }
            else if (model.DetailNote == String.Empty && model.DetailTemplate != String.Empty)
            {
                _db.DeliveryDetails.Add(new DeliveryDetail
                {
                    ReceiverId = model.ReceiverId,
                    User = User.Identity.Name,
                    DetailNote = model.DetailTemplate,
                    InsertDate = DateTime.Now,
                    Contact = contact.ContactFirstName + " " + contact.ContactLastName,
                    TicketId = model.TicketId
                });
                _db.SaveChanges();
            }
            else
            {
                _db.DeliveryDetails.Add(new DeliveryDetail
                {
                    ReceiverId = model.ReceiverId,
                    User = User.Identity.Name,
                    DetailNote = model.DetailTemplate + " - " + model.DetailNote,
                    InsertDate = DateTime.Now,
                    Contact = contact.ContactFirstName + " " + contact.ContactLastName,
                    TicketId = model.TicketId
                });
                _db.SaveChanges();
            }

            switch (model.Identifier)
            {
                case 1:
                    _helper.LogActivity(User.Identity.Name + " je obavio uspješan poziv za dostavu eDokumenata broj: " + model.InvoiceNumber,
                        User.Identity.Name, model.TicketId, ActivityLog.ActivityTypeEnum.Succall, ActivityLog.DepartmentEnum.Delivery, ActivityLog.ModuleEnum.Delivery);
                    break;
                case 2:
                    _helper.LogActivity(User.Identity.Name + " je obavio kraći informativni poziv vezano za dostavu eDokumenata broj: " + model.InvoiceNumber,
                        User.Identity.Name, model.TicketId, ActivityLog.ActivityTypeEnum.Succalshort, ActivityLog.DepartmentEnum.Delivery, ActivityLog.ModuleEnum.Delivery);
                    break;
                case 3:
                    _helper.LogActivity(User.Identity.Name + " je pokušao obaviti telefonski poziv vezano za dostavu eDokumenata broj: " + model.InvoiceNumber,
                        User.Identity.Name, model.TicketId, ActivityLog.ActivityTypeEnum.Unsuccal, ActivityLog.DepartmentEnum.Delivery, ActivityLog.ModuleEnum.Delivery);
                    break;
            }

            return RedirectToAction("Details", new { id = model.TicketId, receiverId = model.ReceiverId });
        }

        // POST: Delivery/EditDetail/12345
        [HttpPost]
        [Authorize]
        public ActionResult EditDetail(DeliveryDetailHelper model)
        {
            var DetailForEdit = _db.DeliveryDetails.Find(model.DetailNoteId);
            var contact = _db.Contacts.Find(model.Contact);

            DetailForEdit.DetailNote = model.DetailNote;
            DetailForEdit.Contact = contact.ContactFirstName + " " + contact.ContactLastName;
            DetailForEdit.UpdateDate = DateTime.Now;
            _db.SaveChanges();

            return RedirectToAction("Details", new { id = model.TicketId, receiverId = model.ReceiverId });
        }
        
        // POST: Delivery/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,SenderId,RecepientId,InvoiceNumber,UserId,MeRLink,InsertDate,UpdateDate")] Delivery deliveryTicketModel)
        {
            if (ModelState.IsValid)
            {
                _db.DeliveryTicketModels.Add(deliveryTicketModel);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(deliveryTicketModel);
        }

        // GET: Delivery/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delivery deliveryTicketModel = await _db.DeliveryTicketModels.FindAsync(id);
            if (deliveryTicketModel == null)
            {
                return HttpNotFound();
            }
            return View(deliveryTicketModel);
        }

        // POST: Delivery/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,SenderId,RecepientId,InvoiceNumber,UserId,MeRLink,InsertDate,UpdateDate")] Delivery deliveryTicketModel)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(deliveryTicketModel).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(deliveryTicketModel);
        }

        // GET: Delivery/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delivery deliveryTicketModel = await _db.DeliveryTicketModels.FindAsync(id);
            if (deliveryTicketModel == null)
            {
                return HttpNotFound();
            }
            return View(deliveryTicketModel);
        }

        // POST: Delivery/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Delivery deliveryTicketModel = await _db.DeliveryTicketModels.FindAsync(id);
            _db.DeliveryTicketModels.Remove(deliveryTicketModel);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Delivery/DeleteDetail/5
        [HttpPost, ActionName("DeleteDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDetail(int DetailId, string TicketId, string ReceiverId)
        {
            int TicketIdInt = Int32.Parse(TicketId);
            int ReceiverIdInt = Int32.Parse(ReceiverId);
            DeliveryDetail deliveryDetail = _db.DeliveryDetails.Find(DetailId);
            _db.DeliveryDetails.Remove(deliveryDetail);
            _db.SaveChanges();
            return RedirectToAction("Details", "Delivery", new { id = TicketIdInt, receiverId = ReceiverIdInt });
        }

        // POST: Delivery/Contacts
        public ActionResult Contacts(string Organization, string ContactName, string Number, string Email)
        {
            var Results = _db.Contacts.Where(c => c.ContactType == Contact.ContactTypeEnum.Delivery);

            if (!String.IsNullOrEmpty(Organization))
            {
                Results = Results.Where(c => c.Organization.SubjectName.Contains(Organization) || c.Organization.VAT.Contains(Organization));
            }
            if (!String.IsNullOrEmpty(ContactName))
            {
                Results = Results.Where(c => c.ContactFirstName.Contains(ContactName) || c.ContactLastName.Contains(ContactName));
            }
            if (!String.IsNullOrEmpty(Number))
            {
                Results = Results.Where(c => c.TelephoneNumber.Contains(Number) || c.MobilePhoneNumber.Contains(Number));
            }
            if (!String.IsNullOrEmpty(Email))
            {
                Results = Results.Where(c => c.Email.Contains(Email));
            }

            return View(Results.OrderByDescending(r => r.InsertDate));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        static MerDeliveryJsonResponse ParseJson(string url)
        {
            // TO DO: Make try-catch for the entire method
            using (var MeR = new WebClient())
            {
                var json = MeR.DownloadString(url);
                MerDeliveryJsonResponse Result = JsonConvert.DeserializeObject<MerDeliveryJsonResponse>(json);
                return (Result);
            }
        }

        public void LogEmail(int _TicketId, string _Email)
        {
            var ticket = _db.DeliveryTicketModels.Find(_TicketId);

            _helper.LogActivity(User.Identity.Name + " je poslao e-mail na adresu: " + _Email + " na temu dostave eDokumenata broj: " + ticket.InvoiceNumber,
                User.Identity.Name, _TicketId, ActivityLog.ActivityTypeEnum.Email, ActivityLog.DepartmentEnum.Delivery, ActivityLog.ModuleEnum.Delivery);
        }

        public JsonResult Assign(int id)
        {
            var ticketForAssignement = _db.DeliveryTicketModels.Find(id);
            ticketForAssignement.IsAssigned = true;
            ticketForAssignement.AssignedTo = User.Identity.Name;
            _db.SaveChanges();
            _helper.LogActivity("Agent " + User.Identity.Name + " si je dodijelio karticu za primatelja: " + ticketForAssignement.Receiver.SubjectName + ", za eDokument: " + ticketForAssignement.InvoiceNumber,
                User.Identity.Name, id, ActivityLog.ActivityTypeEnum.Ticketassign, ActivityLog.DepartmentEnum.Delivery, ActivityLog.ModuleEnum.Delivery);

            return Json(new { Status = "OK" });
        }

        public ActionResult Reassing(bool unassign, string reassignedTo, string ticketId)
        {
            int ticketIdInt = Int32.Parse(ticketId);
            var ticketForReassingement = (from t in _db.DeliveryTicketModels
                                          where t.Id == ticketIdInt
                                          select t).First();
            if (unassign)
            {
                ticketForReassingement.IsAssigned = false;
                ticketForReassingement.AssignedTo = String.Empty;
                _db.SaveChanges();
            }
            else
            {
                ticketForReassingement.AssignedTo = reassignedTo;
                _db.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        //POST: Delivery/AssignManagement
        [HttpPost, ActionName("AssignManagement")]
        public JsonResult AssignManagement(AssigningTickets[] forAssign)
        {
            foreach (var assign in forAssign)
            {
                if (assign.TicketDate == null)
                {
                    var sentDate = Convert.ToDateTime(assign.SentDate);
                    var ticketsForAssing = (from t in _db.DeliveryTicketModels
                                            where (t.InsertDate >= DateTime.Today) && (t.SentDate >= sentDate)
                                            select t).AsEnumerable();
                    foreach (var ticket in ticketsForAssing)
                    {
                        ticket.IsAssigned = true;
                        ticket.AssignedTo = assign.Agent;
                    }
                    _db.SaveChanges();
                }
                else
                {
                    var ticketDate = Convert.ToDateTime(assign.TicketDate);
                    var sentDate = Convert.ToDateTime(assign.SentDate);
                    var ticketsForAssing = (from t in _db.DeliveryTicketModels
                                            where (t.InsertDate == ticketDate) && (t.SentDate == sentDate)
                                            select t).AsEnumerable();
                    foreach (var ticket in ticketsForAssing)
                    {
                        ticket.IsAssigned = true;
                        ticket.AssignedTo = assign.Agent;
                    }
                    _db.SaveChanges();
                }
            }
            return Json(new { Status = "OK" });
        }

        // POST: Delivery/PostmarkActivateBounce
        [HttpPost]
        public JsonResult PostmarkActivateBounce(int bounceId, int ticketId)
        {
            string activateLink = String.Format(@"https://api.postmarkapp.com/bounces/" + bounceId + "/activate");

            using (var postmark = new WebClient())
            {
                postmark.Headers.Add(HttpRequestHeader.Accept, "application/json");
                postmark.Headers.Add("X-Postmark-Server-Token", "8ab33a3d-a800-405f-afad-9c75c2f08c0b");
                var response = postmark.UploadString(activateLink, "PUT", activateLink);
                var activateResponse = JsonConvert.DeserializeObject<ActivateBounceResponse>(response);

                _helper.LogActivity(User.Identity.Name + " je reaktivirao e-mail adresu " + activateResponse.Bounce.Email + " u Postmarku",
                    User.Identity.Name, ticketId, ActivityLog.ActivityTypeEnum.Postmarkactivatebounce, ActivityLog.DepartmentEnum.Delivery, ActivityLog.ModuleEnum.Delivery);
            }

            return Json(new { Status = "OK" });
        }

        public bool CheckIfExists(int merElectronicId)
        {
            var ids = _db.DeliveryTicketModels.Select(d => d.MerElectronicId);

            return ids.Any(x => x.Equals(merElectronicId));
        }
    }
}
