using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using MojCRM.Areas.CRM.Helpers;
using MojCRM.Areas.CRM.Models;
using MojCRM.Helpers;
using MojCRM.Models;
using Newtonsoft.Json;

namespace MojCRM.Areas.CRM.Controllers
{
    public class ContractController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly HelperMethods _helper = new HelperMethods();
        private readonly OrganizationHelperMethods _organizationHelper = new OrganizationHelperMethods();
        // GET: CRM/Contract
        public ActionResult Index(ContractSearchModel model)
        {
            IQueryable<Contract> contracts = _db.Contracts;

            if (!String.IsNullOrEmpty(model.ContractNumber))
            {
                contracts = contracts.Where(x => x.MerContractNumber.Contains(model.ContractNumber));
            }
            if (!String.IsNullOrEmpty(model.OrganizationName))
            {
                contracts = contracts.Where(x => x.Organization.SubjectName.Contains(model.OrganizationName));
            }
            if (!String.IsNullOrEmpty(model.OrganizationVat))
            {
                contracts = contracts.Where(x => x.Organization.SubjectName.Contains(model.OrganizationVat));
            }

            return View(contracts.OrderByDescending(c => c.Id));
        }

        public JsonResult GetContracts(Guid user)
        {
            var credentials = new { MerUser = "", MerPass = "" };
            if (String.IsNullOrEmpty(user.ToString()))
            {
                credentials = (from u in _db.Users
                    where u.UserName == User.Identity.Name
                    select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            }
            else
            {
                credentials = (from u in _db.Users
                    where u.Id == user.ToString()
                    select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            }

            var referenceDate = DateTime.Today.AddDays(-1); //ovo će biti na produkciji
            //var referenceDate = new DateTime(2016, 1, 1); //init verzija
            int importedContracts;
            int updatedContracts = 0;
            int newContracts = 0;
            var agent = user.ToString();
            MerApiGetContracts request = new MerApiGetContracts()
            {
                Id = credentials.MerUser,
                Pass = credentials.MerPass,
                Oib = "99999999927",
                PJ = string.Empty,
                SoftwareId = "MojCRM-001",
                From = referenceDate
            };

            string merRequest = JsonConvert.SerializeObject(request);

            using (var mer = new WebClient() { Encoding = Encoding.UTF8 })
            {
                mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var response = mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/GetContracts").ToString(), "POST", merRequest);
                MerGetContractsResponse[] results = JsonConvert.DeserializeObject<MerGetContractsResponse[]>(response);

                foreach (var result in results)
                {
                    if (!_db.Contracts.Any(x => x.MerId == result.Id))
                    {
                        _db.Contracts.Add(new Contract()
                        {
                            MerId = result.Id,
                            MerContractNumber = result.ContractNumber,
                            MerQuoteNumber = result.ProposalNumber,
                            RelatedOrganizationId = result.SubjektId,
                            MerContractNote = result.Note,
                            StartDate = result.DateFrom,
                            EndDate = result.DateTo,
                            MerActivationDate = result.ActivatedDate,
                            GracePeriod = result.GracePeriod,
                            IsActive = result.IsActive,
                            IsPartnerStatus = false,
                            InsertDate = DateTime.Now
                        });

                        if (!result.ContractNumber.Contains("PrePaid"))
                        {
                            if (_db.OrganizationAttributes.Any(x => x.OrganizationId == result.SubjektId && x.AttributeType == OrganizationAttribute.AttributeTypeEnum.CONTRACT))
                            {
                                var attributeForUpdate =
                                    _db.OrganizationAttributes.First(x => x.OrganizationId == result.SubjektId);

                                attributeForUpdate.IsActive = result.IsActive;
                                attributeForUpdate.UpdateDate = DateTime.Now;
                            }
                            if (!_db.OrganizationAttributes.Any(x => x.OrganizationId == result.SubjektId && x.AttributeType == OrganizationAttribute.AttributeTypeEnum.CONTRACT))
                            {
                                _db.OrganizationAttributes.Add(new OrganizationAttribute()
                                {
                                    OrganizationId = result.SubjektId,
                                    AttributeClass = OrganizationAttribute.AttributeClassEnum.MER,
                                    AttributeType = OrganizationAttribute.AttributeTypeEnum.CONTRACT,
                                    IsActive = result.IsActive,
                                    AssignedBy = "MojCRM - ContractImport",
                                    InsertDate = DateTime.Now
                                });
                            }
                        }
                        if (result.ContractNumber.Contains("PrePaid"))
                        {
                            if (_db.OrganizationAttributes.Any(x => x.OrganizationId == result.SubjektId && x.AttributeType == OrganizationAttribute.AttributeTypeEnum.ADVANCE))
                            {
                                var attributeForUpdate =
                                    _db.OrganizationAttributes.First(x => x.OrganizationId == result.SubjektId);

                                attributeForUpdate.IsActive = result.IsActive;
                                attributeForUpdate.UpdateDate = DateTime.Now;
                            }
                            if (!_db.OrganizationAttributes.Any(x => x.OrganizationId == result.SubjektId && x.AttributeType == OrganizationAttribute.AttributeTypeEnum.CONTRACT))
                            {
                                _db.OrganizationAttributes.Add(new OrganizationAttribute()
                                {
                                    OrganizationId = result.SubjektId,
                                    AttributeClass = OrganizationAttribute.AttributeClassEnum.MER,
                                    AttributeType = OrganizationAttribute.AttributeTypeEnum.ADVANCE,
                                    IsActive = result.IsActive,
                                    AssignedBy = "MojCRM - ContractImport",
                                    InsertDate = DateTime.Now
                                });
                            }
                        }

                        newContracts++;
                        _organizationHelper.UpdateOrganization(result.SubjektId, user.ToString());
                    }

                    if (_db.Contracts.Any(x => x.MerId == result.Id))
                    {
                        var contractTemp = _db.Contracts.First(c => c.MerId == result.Id);

                        contractTemp.IsActive = result.IsActive;
                        contractTemp.MerDeactivationDate = result.DeactivatedDate;
                        contractTemp.MerDeactivationReason = result.DeactivateReason;
                        contractTemp.EndDate = result.DateTo;
                        contractTemp.UpdateDate = DateTime.Now;

                        updatedContracts++;
                    }
                }
                _db.SaveChanges();

                foreach (var result in results)
                {
                    if (!_db.Contracts.Any(x => x.MerId == result.Id))
                    {
                        foreach (var product in result.Products)
                        {
                            var contractTemp = _db.Contracts.First(c => c.MerId == result.Id);
                            var serviceTemp = _db.Services.First(s => s.MerId == product.ProductId);
                            _db.ContractedServices.Add(new ContractedService()
                            {
                                RelatedContractId = contractTemp.Id,
                                RelatedServiceId = serviceTemp.ServiceId,
                                ContractedQuantity = product.Quantity,
                                ContractedPrice = product.Price,
                                InsertDate = DateTime.Now
                            });
                        }
                    }
                }
                _db.SaveChanges();

                importedContracts = results.Count();
                _helper.LogActivity("Preuzeto ugovora: " + importedContracts + ", od toga: " + newContracts + "novih i " + updatedContracts + "ažuriranih."
                    , agent, 0, ActivityLog.ActivityTypeEnum.System, ActivityLog.DepartmentEnum.MojCrm, ActivityLog.ModuleEnum.Crm);
            }
            return Json(new { ImportedContracts = importedContracts, UpdatedContracts = updatedContracts, NewContracts = newContracts }, JsonRequestBehavior.AllowGet);
        }
    }
}