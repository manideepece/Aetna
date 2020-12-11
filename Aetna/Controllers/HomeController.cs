using Aetna.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aetna.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult TeamMaintenance()
        {
            return View();
        }

        public async Task<ActionResult> TeamMaintenanceData(int page, int rows)
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            //var context = new AetnaContext();
            //var teamMaintenanceRecords = context.TeamMaintenances.ToList();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5862/");
            // Add an Accept header for JSON format.    
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // List all Names.    
            HttpResponseMessage response = client.GetAsync("api/aetna/GetTeamMaintenanceData").Result;  // Blocking call!
            var teamMaintenanceRecords = new List<TeamMaintenance>();
            if (response.IsSuccessStatusCode)
            {
                var output = await response.Content.ReadAsStringAsync();
                teamMaintenanceRecords = JsonConvert.DeserializeObject<List<TeamMaintenance>>(output);
            }
            int totalRecords = teamMaintenanceRecords.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            teamMaintenanceRecords = teamMaintenanceRecords.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //var teamMaintenanceRecords = new List<TeamMaintenance>()
            //{
            //    new TeamMaintenance(){TeamCode = "DEV1a", TeamName = "Developers Ext", CtrlCnt = "10", Reports = "Medical PNC with Rx Rebates, Medical PNC without Rx Rebates",  Region = "Mid America, Northeast, Other Region, Southeast, West", Subsegment = "AAM, Intl Expat"},
            //    new TeamMaintenance(){TeamCode = "ACT3a", TeamName = "Acturial LC", CtrlCnt = "10", Reports = "All",  Region = "AM, NE, OT, SE, WE", Subsegment = "All"},
            //    new TeamMaintenance(){TeamCode = "ACT3a", TeamName = "Acturial FIN", CtrlCnt = "10", Reports = "All",  Region = "AM, NE, OT, SE, WE", Subsegment = "All"},
            //    new TeamMaintenance(){TeamCode = "ACT3a", TeamName = "Acturial SPB", CtrlCnt = "10", Reports = "All",  Region = "AM, NE, OT, SE, WE", Subsegment = "All"},
            //};
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = teamMaintenanceRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllReports()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5862/");
            // Add an Accept header for JSON format.    
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // List all Names.    
            HttpResponseMessage response = client.GetAsync("api/aetna/GetReports").Result;  // Blocking call!
            var reports = new List<Report>();
            if (response.IsSuccessStatusCode)
            {
                var output = await response.Content.ReadAsStringAsync();
                reports = JsonConvert.DeserializeObject<List<Report>>(output);
            }
            //var list = new List<object>() { new { key = "Medical PNC with Rx Rebates", value = "Medical PNC with Rx Rebates" } , 
            //                                new { key = "Medical PNC without Rx Rebates", value = "Medical PNC without Rx Rebates"}, 
            //                                new { key = "NA", value = "NA" }};
            return Json(reports, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllRegions()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5862/");
            // Add an Accept header for JSON format.    
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // List all Names.    
            HttpResponseMessage response = client.GetAsync("api/aetna/GetRegions").Result;  // Blocking call!
            var reports = new List<Region>();
            if (response.IsSuccessStatusCode)
            {
                var output = await response.Content.ReadAsStringAsync();
                reports = JsonConvert.DeserializeObject<List<Region>>(output);
            }
            return Json(reports, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllSubsegments()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5862/");
            // Add an Accept header for JSON format.    
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // List all Names.    
            HttpResponseMessage response = client.GetAsync("api/aetna/GetSubsegments").Result;  // Blocking call!
            var reports = new List<Subsegment>();
            if (response.IsSuccessStatusCode)
            {
                var output = await response.Content.ReadAsStringAsync();
                reports = JsonConvert.DeserializeObject<List<Subsegment>>(output);
            }
            return Json(reports, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTeamMaintenance([Bind(Exclude = "TeamMaintenanceID")] TeamMaintenance teamMaintenance)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var myContent = JsonConvert.SerializeObject(teamMaintenance);
                    HttpResponseMessage response = client.PostAsync("api/aetna/AddTeamMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json("Saved Successfully", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var errorList = (from item in ModelState
                                     where item.Value.Errors.Any()
                                     select item.Value.Errors[0].ErrorMessage).ToList();

                    return Json(errorList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var errormessage = "Error occured: " + ex.Message;
                return Json(errormessage, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateTeamMaintenanceUsingContext([Bind(Exclude = "TeamMaintenanceID")] TeamMaintenance teamMaintenance)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new AetnaContext())
                    {
                        db.TeamMaintenances.Add(teamMaintenance);
                        db.SaveChanges();
                        return Json("Saved Successfully", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var errorList = (from item in ModelState
                                     where item.Value.Errors.Any()
                                     select item.Value.Errors[0].ErrorMessage).ToList();

                    return Json(errorList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var errormessage = "Error occured: " + ex.Message;
                return Json(errormessage, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EditTeamMaintenance(TeamMaintenance teamMaintenance)
        {
            StringBuilder msg = new StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new AetnaContext())
                    {
                        var record = db.TeamMaintenances.Where(x => x.TeamMaintenanceID == teamMaintenance.TeamMaintenanceID).FirstOrDefault();
                        record.TeamCode = teamMaintenance.TeamCode;
                        record.TeamName = teamMaintenance.TeamName;
                        record.CtrlCnt = teamMaintenance.CtrlCnt;
                        record.Region = teamMaintenance.Region;
                        record.Reports = teamMaintenance.Reports;
                        record.Subsegment = teamMaintenance.Subsegment;
                        db.SaveChanges();
                        return Json("Saved Successfully", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var errorList = (from item in ModelState
                                     where item.Value.Errors.Any()
                                     select item.Value.Errors[0].ErrorMessage).ToList();

                    return Json(errorList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var errormessage = "Error occured: " + ex.Message;
                return Json(errormessage, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EditCellTeamMaintenance(TeamMaintenance teamMaintenance)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new AetnaContext())
                    {
                        var record = db.TeamMaintenances.Where(x => x.TeamMaintenanceID == teamMaintenance.TeamMaintenanceID).FirstOrDefault();
                        if(teamMaintenance.TeamCode != null)
                        {
                            record.TeamCode = teamMaintenance.TeamCode;
                        }
                        else if(teamMaintenance.TeamName != null)
                        {
                            record.TeamName = teamMaintenance.TeamName;
                        }
                        else if(teamMaintenance.CtrlCnt != null)
                        {
                            record.CtrlCnt = teamMaintenance.CtrlCnt;
                        }
                        else if(teamMaintenance.Region != null)
                        {
                            record.Region = teamMaintenance.Region;
                        }
                        else if(teamMaintenance.Reports != null)
                        {
                            record.Reports = teamMaintenance.Reports;
                        }
                        else if(teamMaintenance.Subsegment != null)
                        {
                            record.Subsegment = teamMaintenance.Subsegment;
                        }    
                        db.SaveChanges();
                        return Json("Saved Successfully", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var errorList = (from item in ModelState
                                     where item.Value.Errors.Any()
                                     select item.Value.Errors[0].ErrorMessage).ToList();

                    return Json(errorList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var errormessage = "Error occured: " + ex.Message;
                return Json(errormessage, JsonRequestBehavior.AllowGet);
            }
        }
    }
}