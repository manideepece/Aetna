using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aetna.Models
{
    public class Subsegment
    {
        public int SUB_SEGMENT_ID { get; set; }
        public string SUB_SEGMENT_CD { get; set; }
        public string SUB_SEGMENT_DESCR { get; set; }
        public string FOOTNOTE_IND { get; set; }
        public string CREAT_BY_ID { get; set; }
        public DateTime CREAT_TMSTMP { get; set; }
        public string UPDT_BY_ID { get; set; }
        public DateTime UPDT_TMSTMP { get; set; }
    }
}