using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AetnaAPI.Models
{
    public class TeamMaintenance
    {
        public int TeamMaintenanceID { get; set; }
        public string TeamCode { get; set; }
        public string TeamName { get; set; }
        public int CtrlCnt { get; set; }
        public string Reports { get; set; }
        public string Region { get; set; }
        public string Subsegment { get; set; }
    }
}