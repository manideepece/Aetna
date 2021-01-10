using Aetna.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Aetna.Controllers
{
    //[SiteMinderAuthentication(AuthenticateMode.Enforce)]
    public class SecurityUIController : Controller
    {
        static HttpClient _objHttpCl = new HttpClient();

        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["WebApiUrl"]);
        string NTGroupName = Convert.ToString(ConfigurationManager.AppSettings["SecurtiyNTGroup"]);
        public SecurityUIController()
        {
            _objHttpCl = new HttpClient();
            _objHttpCl.BaseAddress = baseAddress;
        }
        // GET: Reports
        public ActionResult Index(string Id)
        {
            ViewBag.ReportId = Id;
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
            HttpResponseMessage response = _objHttpCl.GetAsync(_objHttpCl.BaseAddress + "/security/GetTeamMaintenanceData").Result;
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

            }
            int totalRecords = teamMaintenanceRecords.Count;
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            teamMaintenanceRecords = teamMaintenanceRecords.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = teamMaintenanceRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RegionMaintenance()
        {
            return View();
        }
        public ActionResult Region()
        {
            return View();
        }
        public ActionResult GetRegionList()
        {
            HttpResponseMessage response = _objHttpCl.GetAsync(_objHttpCl.BaseAddress + "/security/GetRegions").Result;
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
            requestRegion.REGION_ID = regionId!="" ? Convert.ToInt32(regionId) : 0;
            requestRegion.REGION_CD = regionCode;
            requestRegion.REGION_DESCR = regionDescription;
            requestRegion.ModifiedUser = "N376656" /*((UserProfile)Session["userProfile"]).aetnaId*/;             
            var myContent = JsonConvert.SerializeObject(requestRegion);
            HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/AddRegionMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
            if (response.IsSuccessStatusCode)
            {
                var res = response.Content.ReadAsStringAsync().Result;
            }
            return Json(!string.IsNullOrEmpty(regionId) ? "Updated Successfully" : "Saved Successfully", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegion(string regionId)
        {
            var myContent = JsonConvert.SerializeObject(regionId);
            HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/DeleteRegionMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
            if (response.IsSuccessStatusCode)
            {
                string res = response.Content.ReadAsStringAsync().Result;
            }
            
            return Json("Removed Successfully",JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> RegionMaintenanceData(int page, int rows, bool _search, string searchField, string searchString)
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            HttpResponseMessage response = _objHttpCl.GetAsync(_objHttpCl.BaseAddress + "/security/GetRegions").Result;
            var regions = new List<Region>();
            if (response.IsSuccessStatusCode)
            {
                var output = await response.Content.ReadAsStringAsync();
                regions = JsonConvert.DeserializeObject<List<Region>>(output);
            }
            if (Request.Params["_search"] == "true")
            {
                var searchFields = JsonConvert.DeserializeObject<SearchParameter>(Request.Params["filters"]);
                foreach (var field in searchFields.rules)
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
            }
            int totalRecords = regions.Count;
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

        //public ActionResult UserTeamMapping()
        //{
        //    var _getNTGroupResult = (Aetna.Tkc.DirectoryServices.ActiveDirectorySupport.GetMembersOfDomainGroup(NTGroupName)).ToList();
        //    if (_getNTGroupResult.Count > 0)
        //    {
        //        List<SelectListItem> _list = new List<SelectListItem>();

        //        _list = _getNTGroupResult.Select(x => new SelectListItem() { Text = x.ToString() }).ToList();
        //        Session["NTGroupUserList"] = _list;
        //    }

        //    return View();
        //}

        public JsonResult GetNTGroupUserList(string term)
        {
            term = term != "" ? term.ToUpper() : "N";
            var _list = (List<SelectListItem>)Session["NTGroupUserList"];
            var userlist = (from user in _list
                            where user.Text.Contains(term)
                            select new
                            {
                                label = user.Text,
                                val = user.Text
                            }).ToList();
            return Json(userlist, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> UserTeamMappingData(int page, int rows, bool _search, string searchField, string searchString)
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            HttpResponseMessage response = _objHttpCl.GetAsync(_objHttpCl.BaseAddress + "/security/GetUserTeamMappingData").Result;
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
            }
            int totalRecords = userTeamMappingRecords.Count;
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
            HttpResponseMessage response = _objHttpCl.GetAsync(_objHttpCl.BaseAddress + "/security/GetSubsegments").Result;
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

            }
            int totalRecords = subsegments.Count;
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
            HttpResponseMessage response = _objHttpCl.GetAsync(_objHttpCl.BaseAddress + "/Security/GetReports").Result;
            var reports = new List<Report>();
            if (response.IsSuccessStatusCode)
            {
                var output = await response.Content.ReadAsStringAsync();
                reports = JsonConvert.DeserializeObject<List<Report>>(output);
            }
            return Json(reports, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllRegions()
        {
            HttpResponseMessage response = _objHttpCl.GetAsync(_objHttpCl.BaseAddress + "/Security/GetRegions").Result;
            var regions = new List<Region>();
            if (response.IsSuccessStatusCode)
            {
                var output = await response.Content.ReadAsStringAsync();
                regions = JsonConvert.DeserializeObject<List<Region>>(output);
            }
            return Json(regions, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllSubsegments()
        {
            HttpResponseMessage response = _objHttpCl.GetAsync(_objHttpCl.BaseAddress + "/Security/GetSubsegments").Result;
            var subsegments = new List<Subsegment>();
            if (response.IsSuccessStatusCode)
            {
                var output = await response.Content.ReadAsStringAsync();
                subsegments = JsonConvert.DeserializeObject<List<Subsegment>>(output);
            }
            return Json(subsegments, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllTeams()
        {
            HttpResponseMessage response = _objHttpCl.GetAsync(_objHttpCl.BaseAddress + "/Security/GetTeams").Result;
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
                    if (oper == "edit")
                    {
                        teamMaintenance.TeamMaintenanceID = Convert.ToInt32(Request.Form["TeamMaintenanceID"]);
                        return await EditTeamMaintenance(teamMaintenance);
                    }
                    //UserProfile user = (UserProfile)Session["userProfile"];
                    //teamMaintenance.ModifiedUser = user == null ? "System" : user.aetnaId;
                    var myContent = JsonConvert.SerializeObject(teamMaintenance);
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/AddTeamMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
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
                    var myContent = JsonConvert.SerializeObject(TeamMaintenanceID);
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/DeleteTeamMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        await response.Content.ReadAsStringAsync();
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
                    //UserProfile user = (UserProfile)Session["userProfile"];
                    //region.ModifiedUser = user == null ? "System" : user.aetnaId;
                    var myContent = JsonConvert.SerializeObject(region);
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/AddRegionMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
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
                    var myContent = JsonConvert.SerializeObject(REGION_ID);
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/DeleteRegionMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
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
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/EditTeamMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        await response.Content.ReadAsStringAsync();
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
        public async Task<ActionResult> EditTeamMaintenance(TeamMaintenance teamMaintenance)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var myContent = JsonConvert.SerializeObject(teamMaintenance);
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/EditTeamMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        await response.Content.ReadAsStringAsync();
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
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/EditRegionMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
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
                    //UserProfile user = (UserProfile)Session["userProfile"];
                    //subsegment.ModifiedUser = user == null ? "System" : user.aetnaId;
                    var myContent = JsonConvert.SerializeObject(subsegment);
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/AddSubsegmentMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
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
                    var myContent = JsonConvert.SerializeObject(SUB_SEGMENT_ID);
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/DeleteSubsegmentMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
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
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/EditSubsegmentMaintenance", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
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
                    //UserProfile user = (UserProfile)Session["userProfile"];
                    //userTeamMapping.ModifiedUser = user == null ? "System" : user.aetnaId;
                    var myContent = JsonConvert.SerializeObject(userTeamMapping);
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/AddUserTeamMapping", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
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
        public async Task<ActionResult> DeleteUserTeamMapping(string USER_ID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var myContent = JsonConvert.SerializeObject(USER_ID);
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/DeleteUserTeamMapping", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
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
        public async Task<ActionResult> EditCellUserTeamMapping(UserTeamMappingEdit userTeamMapping)
        {
            try
            {
                var utm = new UserTeamMapping();
                utm.USER_ID = userTeamMapping.USER_ID;
                if (ModelState.IsValid)
                {
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
                    HttpResponseMessage response = _objHttpCl.PostAsync(_objHttpCl.BaseAddress + "/Security/EditUserTeamMapping", new StringContent(myContent, UnicodeEncoding.UTF8, "application/json")).Result;
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


    }
}