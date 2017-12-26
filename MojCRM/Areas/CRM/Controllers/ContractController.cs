﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MojCRM.Areas.CRM.Models;
using MojCRM.Helpers;
using MojCRM.Models;
using Newtonsoft.Json;

namespace MojCRM.Areas.CRM.Controllers
{
    public class ContractController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly OrganizationHelperMethods _organizationHelper = new OrganizationHelperMethods();
        // GET: CRM/Contract
        public ActionResult Index()
        {
            return View();
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

            //var referenceDate = DateTime.Today.AddDays(-1); -- ovo će biti na produkciji
            var referenceDate = new DateTime(2016, 1, 1); //init verzija
            int importedContracts;
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

                    _organizationHelper.UpdateOrganization(result.SubjektId, user.ToString());
                }
                _db.SaveChanges();

                foreach (var result in results)
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
                _db.SaveChanges();
                importedContracts = results.Count();
            }
            return Json(new { ImportedContracts = importedContracts }, JsonRequestBehavior.AllowGet);
        }
    }
}