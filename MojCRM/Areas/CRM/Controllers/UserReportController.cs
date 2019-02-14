﻿using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using MojCRM.Helpers;
using MojCRM.Models;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace MojCRM.Areas.CRM.Controllers
{
    public class UserReportController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        /// INA Index View with menu
        /// </summary>
        /// <returns></returns>
        public ActionResult InaIndex()
        {
            return View();
        }

        /// <summary>
        /// Get View for the report
        /// </summary>
        /// <returns></returns>
        // GET: CRM/UserReport/GetInaDailyReport
        public ActionResult GetInaDailyReport(int? type)
        {
            switch (type)
            {
                case 1:
                    ViewBag.Partner = "dobavljača";
                    ViewBag.ExchangeType = "dostavljeni prema društvu";
                    ViewBag.Type = type;
                    break;
                case 2:
                    ViewBag.Partner = "kupca";
                    ViewBag.ExchangeType = "poslani iz društva";
                    ViewBag.Type = type;
                    break;
                default:
                    break;
            }
            return View();
        }

        /// <summary>
        /// Get data from Moj-eRačun for the report
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="reportType">1 - suppliers, 2 - buyers</param>
        /// <returns>Excel file for the report</returns>
        // POST: CRM/UserReport/GetInaDailyReport
        [HttpPost]
        public FileContentResult GetInaDailyReport(DateTime? startDate, DateTime? endDate, int reportType)
        {
            var credentials = (from u in _db.Users
                where u.UserName == "Alen David Jeđud"
                select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();

            MerApiGetInaDailyReport request = new MerApiGetInaDailyReport()
            {
                Id = credentials.MerUser,
                Pass = credentials.MerPass,
                Oib = "99999999927",
                PJ = string.Empty,
                SoftwareId = "MojCRM-001",
                StartDate = startDate,
                EndDate = endDate,
                ReportType = reportType 
            };

            string merRequest = JsonConvert.SerializeObject(request);
            MerGetInaReportResponse[] results;

            using (var mer = new WebClient() {Encoding = Encoding.UTF8})
            {
                mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var response = mer.UploadString(new Uri(App.MerEndpoint + @"apis/v21/getInaReport").ToString(), "POST", merRequest); // production
                //var response = mer.UploadString(new Uri(@"http://localhost:1312/apis/v21/getInaReport").ToString(), "POST", merRequest); // local test
                results = JsonConvert.DeserializeObject<MerGetInaReportResponse[]>(response);
            }

            int cell = 2;

            var wb = new ExcelPackage();
            var ws = wb.Workbook.Worksheets.Add("Dnevni izvještaj");
            ws.Cells[1, 1].Value = reportType == 1 ? "OIB dobavljača" : "OIB kupca";
            ws.Cells[1, 2].Value = reportType == 1 ? "Naziv dobavljača" : "Naziv kupca";
            ws.Cells[1, 3].Value = "Broj računa";
            ws.Cells[1, 4].Value = "Datum i vrijeme slanja eRačuna";
            ws.Cells[1, 5].Value = "Datum i vrijeme preuzimanja eRačuna";
            ws.Cells[1, 6].Value = "MerId";
            ws.Cells[1, 7].Value = "Status dokumenta";
            ws.Cells[1, 8].Value = "Procesni status dokumenta";
            ws.Cells[1, 9].Value = "Tip dokumenta";
            ws.Cells[1, 10].Value = "E-mail partnera";

            foreach (var res in results)
            {
                string statusTemp = String.Empty;

                switch (res.DokumentStatusId)
                {
                    case 10:
                        statusTemp = "U pripremi";
                        break;
                    case 20:
                        statusTemp = "Potpisan";
                        break;
                    case 30:
                        statusTemp = "Poslan";
                        break;
                    case 40:
                        statusTemp = "Dostavljen";
                        break;
                    case 45:
                        statusTemp = "Ispisan";
                        break;
                    case 50:
                        statusTemp = "Neuspješan";
                        break;
                    case 55:
                        statusTemp = "Uklonjen";
                        break;
                }

                string processStatusTemp = String.Empty;

                switch (res.DokumentProcessStatusId)
                {
                    case 0:
                        processStatusTemp = "Odobren (prihvaćen)";
                        break;
                    case 1:
                        processStatusTemp = "Odbijen";
                        break;
                    case 2:
                        processStatusTemp = "Plaćen u potpunosti";
                        break;
                    case 3:
                        processStatusTemp = "Plaćen djelomično";
                        break;
                    case 4:
                        processStatusTemp = "Potvrda zaprimanja";
                        break;
                    case 99:
                        processStatusTemp = "Zaprimljen";
                        break;
                }

                string typeTemp = String.Empty;

                switch (res.DokumentTypeId)
                {
                    case 0: 
                        typeTemp = "eDokument";
                        break;
                    case 1: 
                        typeTemp = "eRačun";
                        break;
                    case 3: 
                        typeTemp = "Storno";
                        break;
                    case 4:
                        typeTemp = "eOpomena";
                        break;
                    case 6:
                        typeTemp = "ePrimka - tip 6";
                        break;
                    case 7:
                        typeTemp = "eOdgovor";
                        break;
                    case 105:
                        typeTemp = "eNarudžba";
                        break;
                    case 226:
                        typeTemp = "eOpoziv";
                        break;
                    case 230:
                        typeTemp = "eIzmjena";
                        break;
                    case 231:
                        typeTemp = "eOdgovorN";
                        break;
                    case 351:
                        typeTemp = "eOtpremnica";
                        break;
                    case 381:
                        typeTemp = "eOdobrenje";
                        break;
                    case 383: 
                        typeTemp = "eTerećenje";
                        break;
                }

                ws.Cells[cell, 1].Value = res.PartnerOib;
                ws.Cells[cell, 2].Value = res.PartnerNaziv;
                ws.Cells[cell, 3].Value = res.InterniBroj;
                ws.Cells[cell, 4].Value = res.DatumOtpreme.ToString();
                ws.Cells[cell, 5].Value = res.DatumDostave.ToString();
                ws.Cells[cell, 6].Value = res.Id;
                ws.Cells[cell, 7].Value = statusTemp;
                ws.Cells[cell, 8].Value = processStatusTemp;
                ws.Cells[cell, 9].Value = typeTemp;
                ws.Cells[cell, 10].Value = !string.IsNullOrEmpty(res.PartnerEmail) ? res.PartnerEmail : string.Empty;
                cell++;
            }

            while (cell < 16)
            {
                ws.Cells[cell, 1].Value = "";
                cell++;
            }

            return File(wb.GetAsByteArray(), "application/vnd.ms-excel", "Izvještaj.xlsx");
        }

        /// <summary>
        /// Get View for the report
        /// </summary>
        /// <returns></returns>
        public ActionResult GetInaDeliveryReport()
        {
            return View();
        }

        /// <summary>
        /// Get data for the report
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>Excel file for the report</returns>
        // POST: CRM/UserReport/GetInaDailyReport
        [HttpPost]
        public FileContentResult GetInaDeliveryReport(DateTime? startDate, DateTime? endDate)
        {
            int cell = 2;
            var results = _db.DeliveryTicketModels.Where(x => x.SenderId == 7750 // we look at INA
            && x.MerDocumentTypeId != 7 // all documents except responses
            && (x.DocumentStatus == 30 || x.DocumentStatus == 50)); // undelivered documents

            var wb = new ExcelPackage();
            var ws = wb.Workbook.Worksheets.Add("Izvještaj dostave");
            ws.Cells[1, 1].Value = "OIB kupca";
            ws.Cells[1, 2].Value = "Naziv kupca";
            ws.Cells[1, 3].Value = "Broj računa";
            ws.Cells[1, 4].Value = "Datum i vrijeme slanja eRačuna";
            ws.Cells[1, 5].Value = "MerId";
            ws.Cells[1, 6].Value = "Status dokumenta";
            ws.Cells[1, 7].Value = "Tip dokumenta";
            ws.Cells[1, 8].Value = "Zadnja napomena";

            foreach (var res in results)
            {

                ws.Cells[cell, 1].Value = res.Receiver.VAT;
                ws.Cells[cell, 2].Value = res.Receiver.SubjectName;
                ws.Cells[cell, 3].Value = res.InvoiceNumber;
                ws.Cells[cell, 4].Value = res.SentDate.ToString();
                ws.Cells[cell, 5].Value = res.Id;
                ws.Cells[cell, 6].Value = res.DocumentStatusString;
                ws.Cells[cell, 7].Value = res.MerDocumentTypeIdString;
                ws.Cells[cell, 8].Value = res.DeliveryDetails.Any() ? res.DeliveryDetails.OrderByDescending(x => x.Id).First().DetailNote : string.Empty;
                cell++;
            }

            while (cell < 16)
            {
                ws.Cells[cell, 1].Value = "";
                cell++;
            }

            return File(wb.GetAsByteArray(), "application/vnd.ms-excel", "Izvještaj.xlsx");
        }
    }
}