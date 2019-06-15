using System;
using System.Collections.Generic;

namespace ProjectShare.Models
{
    public class CommonApiInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string name_v2 { get; set; }
        public string code { get; set; }
    }
    public class CommonApiOrg
    {
        public int id { get; set; }
        public string name { get; set; }
        /// <summary>
        /// Link Name
        /// </summary>
        public string company_linkname { get; set; }
    }
}
