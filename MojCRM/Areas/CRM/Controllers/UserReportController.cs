using System;
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

        // GET: CRM/UserReport/GetInaDailyReport
        public ActionResult GetInaDailyReport()
        {
            return View();
        }

        // POST: CRM/UserReport/GetInaDailyReport
        [HttpPost]
        public FileContentResult GetInaDailyReport(DateTime? startDate, DateTime? endDate)
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
                EndDate = endDate
            };

            string merRequest = JsonConvert.SerializeObject(request);
            MerGetInaReportResponse[] results;

            using (var mer = new WebClient() {Encoding = Encoding.UTF8})
            {
                mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var response = mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getInaReport").ToString(), "POST", merRequest); // production
                //var response = mer.UploadString(new Uri(@"http://localhost:1312/apis/v21/getInaReport").ToString(), "POST", merRequest); // local test
                results = JsonConvert.DeserializeObject<MerGetInaReportResponse[]>(response);
            }

            int cell = 2;

            var wb = new ExcelPackage();
            var ws = wb.Workbook.Worksheets.Add("Dnevni izvještaj");
            ws.Cells[1, 1].Value = "OIB dobavljača";
            ws.Cells[1, 2].Value = "Naziv dobavljača";
            ws.Cells[1, 3].Value = "Broj računa";
            ws.Cells[1, 4].Value = "Datum i vrijeme slanja eRačuna";
            ws.Cells[1, 5].Value = "Datum i vrijeme preuzimanja eRačuna";
            ws.Cells[1, 6].Value = "MerId";
            ws.Cells[1, 7].Value = "Status dokumenta";
            ws.Cells[1, 8].Value = "Tip dokumenta";

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

                ws.Cells[cell, 1].Value = res.PosiljateljOib;
                ws.Cells[cell, 2].Value = res.PosiljateljNaziv;
                ws.Cells[cell, 3].Value = res.InterniBroj;
                ws.Cells[cell, 4].Value = res.DatumOtpreme.ToString();
                ws.Cells[cell, 5].Value = res.DatumDostave.ToString();
                ws.Cells[cell, 6].Value = res.Id;
                ws.Cells[cell, 7].Value = statusTemp;
                ws.Cells[cell, 8].Value = typeTemp;
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