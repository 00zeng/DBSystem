using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.HumanResource.Application;
using ProjectShare.Database;
using System;
using System.Web;
using ProjectShare.Models;
using System.IO;
using System.Collections.Generic;

namespace ProjectWeb.Areas.HumanResource.Controllers
{
    public class EmployeeStatisticController : ControllerBase
    {
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Resignation()
        {
            return View();
        }
    }
}