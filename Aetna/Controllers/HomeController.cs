using Aetna.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public async Task<ActionResult> TeamMaintenanceData(int page, int rows, bool _search, string searchField, string searchString)
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
            if (Request.Params["_search"] == "true")
            {
                var searchFields = JsonConvert.DeserializeObject<SearchParameter>(Request.Params["filters"]);
                foreach (var field in searchFields.rules)
                {
                    switch (field.field)
                    {
                        case "TeamCode":
                            teamMaintenanceRecords = teamMaintenanceRecords.Where(x => x.TeamCode.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                        case "TeamName":
                            teamMaintenanceRecords = teamMaintenanceRecords.Where(x => x.TeamName.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                        case "CtrlCnt":
                            teamMaintenanceRecords = teamMaintenanceRecords.Where(x => x.CtrlCnt == field.data).ToList();
                            break;
                        case "Reports":
                            teamMaintenanceRecords = teamMaintenanceRecords.Where(x => x.Reports.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                        case "Region":
                            teamMaintenanceRecords = teamMaintenanceRecords.Where(x => x.Region.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                        case "Subsegment":
                            teamMaintenanceRecords = teamMaintenanceRecords.Where(x => x.Subsegment.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                    }
                }
                //switch (searchField)
                //{
                //    case "TeamCode":
                //        teamMaintenanceRecords = teamMaintenanceRecords.Where(x => x.TeamCode.Contains(searchString)).ToList();
                //        break;
                //    case "TeamName":
                //        teamMaintenanceRecords = teamMaintenanceRecords.Where(x => x.TeamName.Contains(searchString)).ToList();
                //        break;
                //    case "CtrlCnt":
                //        teamMaintenanceRecords = teamMaintenanceRecords.Where(x => x.CtrlCnt == searchString).ToList();
                //        break;
                //    case "Reports":
                //        teamMaintenanceRecords = teamMaintenanceRecords.Where(x => x.Reports.Contains(searchString)).ToList();
                //        break;
                //    case "Region":
                //        teamMaintenanceRecords = teamMaintenanceRecords.Where(x => x.Region.Contains(searchString)).ToList();
                //        break;
                //    case "Subsegment":
                //        teamMaintenanceRecords = teamMaintenanceRecords.Where(x => x.Subsegment.Contains(searchString)).ToList();
                //        break;
                //}

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

        public ActionResult Region()
        {
            return View();
        }

        public ActionResult GetRegionList()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5862/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("api/aetna/GetRegions").Result;  // Blocking call!
            var regions = new List<Region>();
            if (response.IsSuccessStatusCode)
            {
                string res = response.Content.ReadAsStringAsync().Result;
                regions = JsonConvert.DeserializeObject<List<Region>>(res);
            }

            return Json(regions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateRegion(string regionId, string regionCode, string regionDescription)
        {
            Region requestRegion = new Region();
            requestRegion.REGION_ID = regionId != "" ? Convert.ToInt32(regionId) : 0;
            requestRegion.REGION_CD = regionCode;
            requestRegion.REGION_DESCR = regionDescription;
            requestRegion.ModifiedUser = "N376656" /*((UserProfile)Session["userProfile"]).aetnaId*/;
            var myContent = JsonConvert.SerializeObject(requestRegion);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5862/");
            // Add an Accept header for JSON format.    
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.PostAsync("api/aetna/EditRegionMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
            if (response.IsSuccessStatusCode)
            {
                var res = response.Content.ReadAsStringAsync().Result;
            }
            return Json(!string.IsNullOrEmpty(regionId) ? "Updated Successfully" : "Saved Successfully", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegion(string regionId)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5862/");
                // Add an Accept header for JSON format.    
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var myContent = JsonConvert.SerializeObject(regionId);
                HttpResponseMessage response = client.PostAsync("api/aetna/DeleteRegionMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                if (response.IsSuccessStatusCode)
                {
                    var output = response.Content.ReadAsStringAsync();
                    return Json("Deleted Successfully!", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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

        public ActionResult RegionMaintenance()
        {
            return View();
        }

        public async Task<ActionResult> RegionMaintenanceData(int page, int rows, bool _search, string searchField, string searchString)
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5862/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("api/aetna/GetRegions").Result;  // Blocking call!
            var regions = new List<Region>();
            if (response.IsSuccessStatusCode)
            {
                var output = await response.Content.ReadAsStringAsync();
                regions = JsonConvert.DeserializeObject<List<Region>>(output);
            }
            //if (_search)
            //{
            //    switch (searchField)
            //    {
            //        case "REGION_CD":
            //            regions = regions.Where(x => x.REGION_CD.Contains(searchString)).ToList();
            //            break;
            //        case "REGION_DESCR":
            //            regions = regions.Where(x => x.REGION_DESCR.Contains(searchString)).ToList();
            //            break;
            //        case "UPDT_BY_ID":
            //            regions = regions.Where(x => x.UPDT_BY_ID != null && x.UPDT_BY_ID.Contains(searchString)).ToList();
            //            break;
            //    }

            //}
            if (Request.Params["_search"] == "true")
            {
                var searchFields = JsonConvert.DeserializeObject<SearchParameter>(Request.Params["filters"]);
                foreach(var field in searchFields.rules)
                {
                    switch (field.field)
                    {
                        case "REGION_CD":
                            regions = regions.Where(x => x.REGION_CD.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                        case "REGION_DESCR":
                            regions = regions.Where(x => x.REGION_DESCR.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                        case "UPDT_BY_ID":
                            regions = regions.Where(x => x.UPDT_BY_ID != null && x.UPDT_BY_ID.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                    }
                }
                //switch (searchField)
                //{
                //    case "REGION_CD":
                //        regions = regions.Where(x => x.REGION_CD.Contains(searchString)).ToList();
                //        break;
                //    case "REGION_DESCR":
                //        regions = regions.Where(x => x.REGION_DESCR.Contains(searchString)).ToList();
                //        break;
                //    case "UPDT_BY_ID":
                //        regions = regions.Where(x => x.UPDT_BY_ID != null && x.UPDT_BY_ID.Contains(searchString)).ToList();
                //        break;
                //}

            }
            int totalRecords = regions.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            regions = regions.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = regions
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SubsegmentMaintenance()
        {
            return View();
        }

        public ActionResult Subsegment()
        {
            return View();
        }

        public ActionResult GetSubsegmentList()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5862/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("api/aetna/GetSubsegments").Result;  // Blocking call!
            var regions = new List<Subsegment>();
            if (response.IsSuccessStatusCode)
            {
                string res = response.Content.ReadAsStringAsync().Result;
                regions = JsonConvert.DeserializeObject<List<Subsegment>>(res);
            }

            return Json(regions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateSubsegment(string subsegmentId, string subsegmentCode, string subsegmentDescription)
        {
            Subsegment requestSubsegment = new Subsegment();
            requestSubsegment.SUB_SEGMENT_ID = subsegmentId != "" ? Convert.ToInt32(subsegmentId) : 0;
            requestSubsegment.SUB_SEGMENT_CD = subsegmentCode;
            requestSubsegment.SUB_SEGMENT_DESCR = subsegmentDescription;
            requestSubsegment.ModifiedUser = "N376656" /*((UserProfile)Session["userProfile"]).aetnaId*/;
            var myContent = JsonConvert.SerializeObject(requestSubsegment);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5862/");
            // Add an Accept header for JSON format.    
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.PostAsync("api/aetna/EditSubsegmentMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
            if (response.IsSuccessStatusCode)
            {
                var res = response.Content.ReadAsStringAsync().Result;
            }
            return Json(!string.IsNullOrEmpty(subsegmentId) ? "Updated Successfully" : "Saved Successfully", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserTeamMapping()
        {
            return View();
        }

        public async Task<ActionResult> UserTeamMappingData(int page, int rows, bool _search, string searchField, string searchString)
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
            HttpResponseMessage response = client.GetAsync("api/aetna/GetUserTeamMappingData").Result;  // Blocking call!
            var userTeamMappingRecords = new List<UserTeamMapping>();
            if (response.IsSuccessStatusCode)
            {
                var output = await response.Content.ReadAsStringAsync();
                userTeamMappingRecords = JsonConvert.DeserializeObject<List<UserTeamMapping>>(output);
            }
            if (Request.Params["_search"] == "true")
            {
                var searchFields = JsonConvert.DeserializeObject<SearchParameter>(Request.Params["filters"]);
                foreach (var field in searchFields.rules)
                {
                    switch (field.field)
                    {
                        case "USER_ID":
                            userTeamMappingRecords = userTeamMappingRecords.Where(x => x.USER_ID.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                        case "FIRST_NAM":
                            userTeamMappingRecords = userTeamMappingRecords.Where(x => x.FIRST_NAM.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                        case "LAST_NAM":
                            userTeamMappingRecords = userTeamMappingRecords.Where(x => x.LAST_NAM.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                        case "TEAMS":
                            userTeamMappingRecords = userTeamMappingRecords.Where(x => x.TEAMS.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                    }
                }
                //switch (searchField)
                //{
                //    case "USER_ID":
                //        userTeamMappingRecords = userTeamMappingRecords.Where(x => x.USER_ID.Contains(searchString)).ToList();
                //        break;
                //    case "FIRST_NAM":
                //        userTeamMappingRecords = userTeamMappingRecords.Where(x => x.FIRST_NAM.Contains(searchString)).ToList();
                //        break;
                //    case "LAST_NAM":
                //        userTeamMappingRecords = userTeamMappingRecords.Where(x => x.LAST_NAM.Contains(searchString)).ToList();
                //        break;
                //    case "TEAMS":
                //        userTeamMappingRecords = userTeamMappingRecords.Where(x => x.TEAMS.Contains(searchString)).ToList();
                //        break;
                //}

            }
            int totalRecords = userTeamMappingRecords.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            userTeamMappingRecords = userTeamMappingRecords.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = userTeamMappingRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SubsegmentMaintenanceData(int page, int rows, bool _search, string searchField, string searchString)
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5862/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("api/aetna/GetSubsegments").Result;  // Blocking call!
            var subsegments = new List<Subsegment>();
            if (response.IsSuccessStatusCode)
            {
                var output = await response.Content.ReadAsStringAsync();
                subsegments = JsonConvert.DeserializeObject<List<Subsegment>>(output);
            }
            if (Request.Params["_search"] == "true")
            {
                var searchFields = JsonConvert.DeserializeObject<SearchParameter>(Request.Params["filters"]);
                foreach (var field in searchFields.rules)
                {
                    switch (field.field)
                    {
                        case "SUB_SEGMENT_CD":
                            subsegments = subsegments.Where(x => x.SUB_SEGMENT_CD.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                        case "SUB_SEGMENT_DESCR":
                            subsegments = subsegments.Where(x => x.SUB_SEGMENT_DESCR.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                        case "UPDT_BY_ID":
                            subsegments = subsegments.Where(x => x.UPDT_BY_ID != null && x.UPDT_BY_ID.ToLower().Contains(field.data.ToLower())).ToList();
                            break;
                    }
                }
                //switch (searchField)
                //{
                //    case "SUB_SEGMENT_CD":
                //        subsegments = subsegments.Where(x => x.SUB_SEGMENT_CD.Contains(searchString)).ToList();
                //        break;
                //    case "SUB_SEGMENT_DESCR":
                //        subsegments = subsegments.Where(x => x.SUB_SEGMENT_DESCR.Contains(searchString)).ToList();
                //        break;
                //    case "UPDT_BY_ID":
                //        subsegments = subsegments.Where(x => x.UPDT_BY_ID != null && x.UPDT_BY_ID.Contains(searchString)).ToList();
                //        break;
                //}

            }
            int totalRecords = subsegments.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            subsegments = subsegments.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = subsegments
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

        public async Task<ActionResult> GetAllTeams()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5862/");
            // Add an Accept header for JSON format.    
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // List all Names.    
            HttpResponseMessage response = client.GetAsync("api/aetna/GetTeams").Result;  // Blocking call!
            var reports = new List<Team>();
            if (response.IsSuccessStatusCode)
            {
                var output = await response.Content.ReadAsStringAsync();
                reports = JsonConvert.DeserializeObject<List<Team>>(output);
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
                    string oper = Request.Form["oper"];
                    if(oper == "edit")
                    {
                        teamMaintenance.TeamMaintenanceID = Convert.ToInt32(Request.Form["TeamMaintenanceID"]);
                        return await EditTeamMaintenance(teamMaintenance);
                    }
                    teamMaintenance.ModifiedUser = "System";
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var myContent = JsonConvert.SerializeObject(teamMaintenance);
                    HttpResponseMessage response = client.PostAsync("api/aetna/AddTeamMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json(output, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> DeleteTeamMaintenance(string TeamMaintenanceID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var myContent = JsonConvert.SerializeObject(TeamMaintenanceID);
                    HttpResponseMessage response = client.PostAsync("api/aetna/DeleteTeamMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json("Deleted Successfully!", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> CreateRegionMaintenance([Bind(Exclude = "REGION_ID")] Region region)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    region.ModifiedUser = "System";
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var myContent = JsonConvert.SerializeObject(region);
                    HttpResponseMessage response = client.PostAsync("api/aetna/AddRegionMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json(output, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> DeleteRegionMaintenance(string REGION_ID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var myContent = JsonConvert.SerializeObject(REGION_ID);
                    HttpResponseMessage response = client.PostAsync("api/aetna/DeleteRegionMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json("Deleted Successfully!", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> EditCellTeamMaintenance(TeamMaintenanceEdit teamMaintenance)
        {
            try
            {
                var tm = new TeamMaintenance();
                tm.TeamMaintenanceID = teamMaintenance.TeamMaintenanceID;
                if (ModelState.IsValid)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (teamMaintenance.TeamCode != null)
                    {
                        tm.Column = "TeamCode";
                        tm.Value = teamMaintenance.TeamCode;
                    }
                    else if (teamMaintenance.TeamName != null)
                    {
                        tm.Column = "TeamName";
                        tm.Value = teamMaintenance.TeamName;
                    }
                    else if (teamMaintenance.CtrlCnt != null)
                    {
                        tm.Column = "CtrlCnt";
                        tm.Value = teamMaintenance.CtrlCnt;
                    }
                    else if (teamMaintenance.Region != null)
                    {
                        tm.Column = "Region";
                        tm.Value = string.Join(",", teamMaintenance.Region);
                    }
                    else if (teamMaintenance.Reports != null)
                    {
                        tm.Column = "Reports";
                        tm.Value = string.Join(",", teamMaintenance.Reports);
                    }
                    else if (teamMaintenance.Subsegment != null)
                    {
                        tm.Column = "Subsegment";
                        tm.Value = string.Join(",", teamMaintenance.Subsegment);
                    }
                    var myContent = JsonConvert.SerializeObject(tm);
                    HttpResponseMessage response = client.PostAsync("api/aetna/EditTeamMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json("Saved Successfully!", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> EditCellRegionMaintenance(Region region)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (region.REGION_CD != null)
                    {
                        region.Column = "REGION_CD";
                        region.Value = region.REGION_CD;
                    }
                    else if (region.REGION_DESCR != null)
                    {
                        region.Column = "REGION_DESCR";
                        region.Value = region.REGION_DESCR;
                    }
                    var myContent = JsonConvert.SerializeObject(region);
                    HttpResponseMessage response = client.PostAsync("api/aetna/EditRegionMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json("Saved Successfully!", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> CreateSubsegmentMaintenance([Bind(Exclude = "SUB_SEGMENT_ID")] Subsegment subsegment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    subsegment.ModifiedUser = "System";
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var myContent = JsonConvert.SerializeObject(subsegment);
                    HttpResponseMessage response = client.PostAsync("api/aetna/AddSubsegmentMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json(output, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> DeleteSubsegmentMaintenance(string SUB_SEGMENT_ID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var myContent = JsonConvert.SerializeObject(SUB_SEGMENT_ID);
                    HttpResponseMessage response = client.PostAsync("api/aetna/DeleteSubsegmentMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json("Deleted Successfully!", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> EditCellSubsegmentMaintenance(Subsegment subsegment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (subsegment.SUB_SEGMENT_CD != null)
                    {
                        subsegment.Column = "SUB_SEGMENT_CD";
                        subsegment.Value = subsegment.SUB_SEGMENT_CD;
                    }
                    else if (subsegment.SUB_SEGMENT_DESCR != null)
                    {
                        subsegment.Column = "SUB_SEGMENT_DESCR";
                        subsegment.Value = subsegment.SUB_SEGMENT_DESCR;
                    }
                    var myContent = JsonConvert.SerializeObject(subsegment);
                    HttpResponseMessage response = client.PostAsync("api/aetna/EditSubsegmentMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json("Saved Successfully!", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> CreateUserTeamMapping(UserTeamMapping userTeamMapping)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    userTeamMapping.ModifiedUser = "System";
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var myContent = JsonConvert.SerializeObject(userTeamMapping);
                    HttpResponseMessage response = client.PostAsync("api/aetna/AddUserTeamMapping", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json(output, JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> EditCellUserTeamMapping(UserTeamMappingEdit userTeamMapping)
        {
            try
            {
                var utm = new UserTeamMapping();
                utm.USER_ID = userTeamMapping.USER_ID;
                if (ModelState.IsValid)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (userTeamMapping.FIRST_NAM != null)
                    {
                        utm.Column = "FIRST_NAM";
                        utm.Value = userTeamMapping.FIRST_NAM;
                    }
                    else if (userTeamMapping.LAST_NAM != null)
                    {
                        utm.Column = "LAST_NAM";
                        utm.Value = userTeamMapping.LAST_NAM;
                    }
                    else if (userTeamMapping.EMP_STS_CD != null)
                    {
                        utm.Column = "EMP_STS_CD";
                        utm.Value = userTeamMapping.EMP_STS_CD;
                    }
                    else if (userTeamMapping.TEAMS != null)
                    {
                        utm.Column = "TEAMS";
                        utm.Value = string.Join(",", userTeamMapping.TEAMS);
                    }
                    var myContent = JsonConvert.SerializeObject(utm);
                    HttpResponseMessage response = client.PostAsync("api/aetna/EditUserTeamMapping", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json("Saved Successfully!", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> DeleteUserTeamMapping(string USER_ID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var myContent = JsonConvert.SerializeObject(USER_ID);
                    HttpResponseMessage response = client.PostAsync("api/aetna/DeleteUserTeamMapping", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json("Deleted Successfully!", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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
                        return Json("Saved Successfully!", JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> EditTeamMaintenance(TeamMaintenance teamMaintenance)
        {
            StringBuilder msg = new StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {
                    //using (var db = new AetnaContext())
                    //{
                    //    var record = db.TeamMaintenances.Where(x => x.TeamMaintenanceID == teamMaintenance.TeamMaintenanceID).FirstOrDefault();
                    //    record.TeamCode = teamMaintenance.TeamCode;
                    //    record.TeamName = teamMaintenance.TeamName;
                    //    record.CtrlCnt = teamMaintenance.CtrlCnt;
                    //    record.Region = teamMaintenance.Region;
                    //    record.Reports = teamMaintenance.Reports;
                    //    record.Subsegment = teamMaintenance.Subsegment;
                    //    db.SaveChanges();
                    //    return Json("Saved Successfully!", JsonRequestBehavior.AllowGet);
                    //}
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5862/");
                    // Add an Accept header for JSON format.    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var myContent = JsonConvert.SerializeObject(teamMaintenance);
                    HttpResponseMessage response = client.PostAsync("api/aetna/EditTeamMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        return Json("Saved Successfully!", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("An Error Occured!", JsonRequestBehavior.AllowGet);
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

        public JsonResult EditCellTeamMaintenanceUsingContext(TeamMaintenance teamMaintenance)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new AetnaContext())
                    {
                        var record = db.TeamMaintenances.Where(x => x.TeamMaintenanceID == teamMaintenance.TeamMaintenanceID).FirstOrDefault();
                        if (teamMaintenance.TeamCode != null)
                        {
                            record.TeamCode = teamMaintenance.TeamCode;
                        }
                        else if (teamMaintenance.TeamName != null)
                        {
                            record.TeamName = teamMaintenance.TeamName;
                        }
                        else if (teamMaintenance.CtrlCnt != null)
                        {
                            record.CtrlCnt = teamMaintenance.CtrlCnt;
                        }
                        else if (teamMaintenance.Region != null)
                        {
                            record.Region = teamMaintenance.Region;
                        }
                        else if (teamMaintenance.Reports != null)
                        {
                            record.Reports = teamMaintenance.Reports;
                        }
                        else if (teamMaintenance.Subsegment != null)
                        {
                            record.Subsegment = teamMaintenance.Subsegment;
                        }
                        db.SaveChanges();
                        return Json("Saved Successfully!", JsonRequestBehavior.AllowGet);
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