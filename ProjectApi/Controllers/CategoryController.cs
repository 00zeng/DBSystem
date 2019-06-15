using ProjectApi.Models;
using ProjectApi.Process;
using ProjectShare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ProjectApi.Controllers
{
    public class CategoryController : ApiController
    {
        [Route("api/category/getlist")]
        [HttpGet]
        public object GetList()
        {
            ProductProc productProc = new ProductProc();
            return productProc.GetCategoryList();
        }

        [Route("api/category/getproductlist")]
        [HttpGet]
        public object GetProductListByCate(int categoryId)
        {
            ProductProc productProc = new ProductProc();
            return productProc.GetProductListByCate(categoryId);
        }
    }
}
