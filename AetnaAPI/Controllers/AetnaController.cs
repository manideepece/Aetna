﻿using AetnaAPI.Models;
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
        public string AddTeamMaintenance(TeamMaintenance teamMaintenance)
        {
            var repository = new DataAccess();
            var output = repository.AddTeamMaintenance(teamMaintenance);
            return output;
        }

        [HttpPost]
        public bool EditTeamMaintenance(TeamMaintenance teamMaintenance)
        {
            var repository = new DataAccess();
            var output = repository.EditTeamMaintenanceNew(teamMaintenance);
            return output;
        }

        [HttpPost]
        public bool DeleteTeamMaintenance([FromBody] string TeamMaintenanceID)
        {
            var repository = new DataAccess();
            var output = repository.DeleteTeamMaintenance(TeamMaintenanceID);
            return output;
        }

        [HttpPost]
        public string AddRegionMaintenance(RegionModel region)
        {
            var repository = new DataAccess();
            var output = repository.AddRegionMaintenance(region);
            return output;
        }

        [HttpPost]
        public bool EditRegionMaintenance(RegionModel region)
        {
            var repository = new DataAccess();
            var output = repository.EditRegionMaintenance(region);
            return output;
        }

        [HttpPost]
        public bool DeleteRegionMaintenance([FromBody]string REGION_ID)
        {
            var repository = new DataAccess();
            var output = repository.DeleteRegionMaintenance(REGION_ID);
            return output;
        }

        [HttpPost]
        public string AddSubsegmentMaintenance(SubsegmentModel subsegment)
        {
            var repository = new DataAccess();
            var output = repository.AddSubsegmentMaintenance(subsegment);
            return output;
        }

        [HttpPost]
        public bool EditSubsegmentMaintenance(SubsegmentModel subsegment)
        {
            var repository = new DataAccess();
            var output = repository.EditSubsegmentMaintenanceNew(subsegment);
            return output;
        }

        [HttpPost]
        public bool DeleteSubsegmentMaintenance([FromBody] string SUB_SEGMENT_ID)
        {
            var repository = new DataAccess();
            var output = repository.DeleteSubsegmentMaintenance(SUB_SEGMENT_ID);
            return output;
        }

        public List<UserTeamMapping> GetUserTeamMappingData()
        {
            var repository = new DataAccess();
            var output = repository.GetUserTeamMappingData();
            return output;
        }

        [HttpPost]
        public string AddUserTeamMapping(UserTeamMapping userTeamMapping)
        {
            var repository = new DataAccess();
            var output = repository.AddUserTeamMapping(userTeamMapping);
            return output;
        }

        [HttpPost]
        public bool EditUserTeamMapping(UserTeamMapping userTeamMapping)
        {
            var repository = new DataAccess();
            var output = repository.EditUserTeamMapping(userTeamMapping);
            return output;
        }

        [HttpPost]
        public bool DeleteUserTeamMapping([FromBody] string USER_ID)
        {
            var repository = new DataAccess();
            var output = repository.DeleteUserTeamMapping(USER_ID);
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

        public List<TeamModel> GetTeams()
        {
            var repository = new DataAccess();
            var output = repository.GetTeams();
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
