using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.SaleManage.Application;
using ProjectShare.Database;
using System;
using System.Web;
using ProjectShare.Models;
using System.IO;
using System.Collections.Generic;

namespace ProjectWeb.Areas.SaleManage.Controllers
{
    public class DistributorStatisticController : ControllerBase
    {
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Resignation()
        {
            return View();
        }
    }
}