using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aetna.Models
{
    public class Rule
    {
        public string field { get; set; }
        public string op { get; set; }
        public string data { get; set; }
    }

    public class SearchParameter
    {
        public string groupOp { get; set; }
        public List<Rule> rules { get; set; }
    }
}