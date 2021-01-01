using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AetnaAPI.Models
{
    public class RegionModel
    {
        public int REGION_ID { get; set; }
        public string REGION_CD { get; set; }
        public string REGION_DESCR { get; set; }
        public string CREAT_BY_ID { get; set; }
        public DateTime? CREAT_TMSTMP { get; set; }
        public string UPDT_BY_ID { get; set; }
        public DateTime? UPDT_TMSTMP { get; set; }
        public string Column { get; set; }
        public string Value { get; set; }
        public string ModifiedUser { get; set; }
    }
}