using System.Collections.Generic;

namespace ProjectShare.Models
{
    public class CategoryWithProductList
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<ProductList> products { get; set; }
    }
    public class ProductList
    {
        public string id { get; set; }
        public string name { get; set; }
        public string cover_url { get; set; }
        public int sales { get; set; }
        public decimal cur_price { get; set; }
        public int star { get; set; }
    }
}
