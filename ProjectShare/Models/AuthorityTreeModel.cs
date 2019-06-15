using System.Collections.Generic;

namespace ProjectShare.Models
{
    public class AuthorityTreeModel
    {
        public string parentId { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        public string value { get; set; }
        public int? checkstate { get; set; }
        public bool showcheck { get; set; }
        public bool complete { get; set; }
        public bool isexpand { get; set; }
        public bool hasChildren { get; set; }
        public string img { get; set; }
        public string title { get; set; }
        public int category { get; set; }
        public bool disabled { get; set; }
        public List<AuthorityTreeModel> ChildNodes { get; set; }

    }
}
