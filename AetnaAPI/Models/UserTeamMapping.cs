using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AetnaAPI.Models
{
    public class UserTeamMapping
    {
        public string USER_ID { get; set; }
        public string FIRST_NAM { get; set; }
        public string LAST_NAM { get; set; }

        public string EMP_STS_CD { get; set; }
        public string TEAMS { get; set; }
        public string Column { get; set; }
        public string Value { get; set; }
        public string ModifiedUser { get; set; }
    }
}