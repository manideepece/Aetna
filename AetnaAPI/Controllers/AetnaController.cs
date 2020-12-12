using AetnaAPI.Models;
using AetnaAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AetnaAPI.Controllers
{
    public class AetnaController : ApiController
    {
        public List<TeamMaintenance> GetTeamMaintenanceData()
        {
            var repository = new DataAccess();
            var output = repository.GetTeamMaintenanceData();
            return output;
        }

        [HttpPost]
        public bool AddTeamMaintenance(TeamMaintenance teamMaintenance)
        {
            var repository = new DataAccess();
            var output = repository.AddTeamMaintenance(teamMaintenance);
            return output;
        }

        [HttpPost]
        public bool EditTeamMaintenance(TeamMaintenance teamMaintenance)
        {
            var repository = new DataAccess();
            var output = repository.EditTeamMaintenance(teamMaintenance);
            return output;
        }

        public List<ReportModel> GetReports()
        {
            var repository = new DataAccess();
            var output = repository.GetReports();
            return output;
        }

        public List<RegionModel> GetRegions()
        {
            var repository = new DataAccess();
            var output = repository.GetRegions();
            return output;
        }

        public List<SubsegmentModel> GetSubsegments()
        {
            var repository = new DataAccess();
            var output = repository.GetSubsegments();
            return output;
        }


        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
