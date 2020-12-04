using Aetna.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public ActionResult TeamMaintenanceData(int page, int rows)
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var context = new AetnaContext();
            var teamMaintenanceRecords = context.TeamMaintenances.ToList();
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

        [HttpPost]
        public JsonResult CreateTeamMaintenance(TeamMaintenance teamMaintenance)
        {
            StringBuilder msg = new StringBuilder();
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
    }
}