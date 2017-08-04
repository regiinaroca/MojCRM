using MojCRM.Models;
using MojCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MojCRM.Controllers
{
    public class OrganizationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Organizations
        public ActionResult Index(string Organization)
        {
            var Organizations = from o in db.Organizations
                                where o.SubjectBusinessUnit == String.Empty
                                select o;

            if (!String.IsNullOrEmpty(Organization))
            {
                Organizations = Organizations.Where(o => o.SubjectName.Contains(Organization) || o.VAT.Contains(Organization));
            }

            return View(Organizations.ToList().OrderBy(o => o.MerId));
        }

        // GET: Organizations/GetOrganizations
        public void GetOrganizations()
        {
            //var MerUser = (from u in db.Users
            //               where u.UserName == Name
            //               select u.MerUserUsername).First();
            //var MerPass = (from u in db.Users
            //               where u.UserName == Name
            //               select u.MerUserPassword).First();

            var Credentials = (from u in db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();

            var ReferencedId = (from o in db.Organizations
                               orderby o.MerId descending
                               select o.MerId).First();
            var Response = new MerGetSubjektDataResponse()
            {
                Id = 238,
                Naziv = "Test Klising d.o.o."
            };
            ReferencedId++;
            try
            {
                while (Response != null)
                {
                    MerApiGetSubjekt Request = new MerApiGetSubjekt()
                    {
                        Id = Credentials.MerUser,
                        Pass = Credentials.MerPass,
                        Oib = "99999999927",
                        PJ = "",
                        SoftwareId = "MojCRM-001",
                        SubjektPJ = ReferencedId.ToString()
                    };

                    string MerRequest = JsonConvert.SerializeObject(Request);

                    using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                        Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                        var _Response = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                        _Response = _Response.Replace("[", "").Replace("]", "");
                        MerGetSubjektDataResponse Result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(_Response);
                        if (Result == null)
                        {
                            break;
                        }
                        else
                        {
                            db.Organizations.Add(new Organizations
                            {
                                MerId = Result.Id,
                                SubjectName = Result.Naziv,
                                SubjectBusinessUnit = Result.PoslovnaJedinica,
                                VAT = Result.Oib,
                                FirstReceived = Result.FirstReceived,
                                FirstSent = Result.FirstSent,
                                InsertDate = DateTime.Now
                            });
                            db.MerDeliveryDetails.Add(new MerDeliveryDetails
                            {
                                MerId = Result.Id,
                                TotalReceived = Result.TotalReceived
                            });
                            db.OrganizationDetails.Add(new OrganizationDetail
                            {
                                MerId = Result.Id,
                                OrganizationGroup = OrganizationGroupEnum.Nema
                            });
                            db.SaveChanges();
                            Result = Response;
                        }
                    }
                    ReferencedId++;
                }
            }
            catch (NullReferenceException e)
            {
                db.LogError.Add(new LogError
                {
                    Method = @"Organizations - GetOrganizations",
                    Parameters = ReferencedId.ToString(),
                    Message = @"Greška kod preuzimanja podataka o tvrtki",
                    InnerException = e.InnerException.ToString(),
                    Request = e.Source,
                    User = User.Identity.Name,
                    InsertDate = DateTime.Now
                });
                db.SaveChanges();
            }
        }

        // GET: Organization/UpdateOrganization/1
        public ActionResult UpdateOrganization(string Name, int MerId)
        {
            var MerUser = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserUsername).First();
            var MerPass = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserPassword).First();
            var Organization = (from o in db.Organizations
                                where o.MerId == MerId
                                select o).First();

            using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
            {
                MerApiGetSubjekt Request = new MerApiGetSubjekt()
                {
                    Id = MerUser.ToString(),
                    Pass = MerPass.ToString(),
                    Oib = "99999999927",
                    PJ = "",
                    SoftwareId = "MojCRM-001",
                    SubjektPJ = Organization.MerId.ToString()
                };

                string MerRequest = JsonConvert.SerializeObject(Request);

                Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var _Response = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                _Response = _Response.Replace("[", "").Replace("]", "");
                MerGetSubjektDataResponse Result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(_Response);

                Organization.SubjectName = Result.Naziv;
                Organization.FirstReceived = Result.FirstReceived;
                Organization.FirstSent = Result.FirstSent;
                Organization.UpdateDate = DateTime.Now;
                Organization.MerDeliveryDetail.TotalReceived = Result.TotalReceived;
            }
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // GET: Organizations/UpdateOrganizations
        public void UpdateOrganizations(string Name)
        {
            var MerUser = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserUsername).First();
            var MerPass = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserPassword).First();
            var Organizations = (from o in db.Organizations
                                 select o).AsEnumerable();


                foreach (var Organization in Organizations)
                {
                    MerApiGetSubjekt Request = new MerApiGetSubjekt()
                    {
                        Id = MerUser.ToString(),
                        Pass = MerPass.ToString(),
                        Oib = "99999999927",
                        PJ = "",
                        SoftwareId = "MojCRM-001",
                        SubjektPJ = Organization.MerId.ToString()
                    };

                    string MerRequest = JsonConvert.SerializeObject(Request);

                    using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                    Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                    var _Response = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                    _Response = _Response.Replace("[", "").Replace("]", "");
                    MerGetSubjektDataResponse Result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(_Response);

                    Organization.SubjectName = Result.Naziv;
                    Organization.FirstReceived = Result.FirstReceived;
                    Organization.FirstSent = Result.FirstSent;
                    Organization.UpdateDate = DateTime.Now;
                    Organization.MerDeliveryDetail.TotalReceived = Result.TotalReceived;
                }
            }
            db.SaveChanges();
        }

        // POST: Organizations/EditImportantComment
        [HttpPost]
        public ActionResult EditImportantComment(string Comment, string ReceiverId)
        {
            int ReceiverIdInt = Int32.Parse(ReceiverId);

            var DetailForEdit = (from dd in db.MerDeliveryDetails
                                   where dd.MerId == ReceiverIdInt
                                   select dd).First();

            DetailForEdit.ImportantComments = Comment;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // POST: Organizations/EditOrganizationDetails
        public ActionResult EditOrganizationDetails(EditOrganizationDetails Model)
        {
            var organization = db.OrganizationDetails.Find(Model.MerId);

            if (!String.IsNullOrEmpty(Model.TelephoneNumber))
            {
                organization.TelephoneNumber = Model.TelephoneNumber;
            }
            if (!String.IsNullOrEmpty(Model.MobilePhoneNumber))
            {
                organization.MobilePhoneNumber = Model.MobilePhoneNumber;
            }
            if (!String.IsNullOrEmpty(Model.ERP))
            {
                organization.ERP = Model.ERP;
            }
            if (!String.IsNullOrEmpty(Model.NumberOfInvoicesSent))
            {
                organization.NumberOfInvoicesSent = Model.NumberOfInvoicesSent;
            }
            if (!String.IsNullOrEmpty(Model.NumberOfInvoicesReceived))
            {
                organization.NumberOfInvoicesReceived = Model.NumberOfInvoicesReceived;
            }
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}