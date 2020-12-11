using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AetnaAPI.Models
{
    public class ReportModel
    {
        public string REPORT_CD { get; set; }
        public string REPORT_DESC { get; set; }
        public string APP_RTGRP_ACC_CD { get; set; }
        public string CREAT_BY_ID { get; set; }
        public DateTime? CREAT_TMSTMP { get; set; }
        public string UPDT_BY_ID { get; set; }
        public DateTime? UPDT_TMSTMP { get; set; }
        public string SEL_RPT_ACRONYM { get; set; }
        public int? REPORT_SORT_ORDER { get; set; }
    }
}