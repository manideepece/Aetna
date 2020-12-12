using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AetnaAPI.Models
{
    public class TeamModel
    {
        public int TEAM_ID { get; set; }
        public string TEAM_CD { get; set; }
        public string TEAM_DESCR { get; set; }
        public int CTRL_COUNT { get; set; }
        public string CREAT_BY_ID { get; set; }
        public DateTime? CREAT_TMSTMP { get; set; }
        public string UPDT_BY_ID { get; set; }
        public DateTime? UPDT_TMSTMP { get; set; }
        public string Column { get; }
        public string Value { get; }
    }
}