using System.Web.Mvc;
using Base.Code;
using ProjectShare.Database;
using System;
using System.Collections.Generic;
using ProjectWeb.Areas.Statistics.Application;
using ProjectShare.Models;

namespace ProjectWeb.Areas.Statistics.Controllers
{
    public class CostAnalysisController : ControllerBase
    {
        StatisticsApp app = new StatisticsApp();

        /// <summary>
        /// 出库统计
        /// </summary>
        /// <param name="companyL1Id"、"companyL2Id"、"areaL1Id"、"areaL2Id"、"distributorId"> 有且只能有一个不为0</param>
        /// <param name="startDate"、"endDate">统计时间</param>
        /// <param name="dateType"> 1-按月统计，2-按年统计，3-按季度统计，其他-按天统计</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult SaleStatistics(int companyL1Id = 0, int companyL2Id = 0, int areaL1Id = 0, int areaL2Id = 0,
            string distributorId = null, DateTime? startDate = null, DateTime? endDate = null, int dateType = 0)
        {
            string data = app.SaleStatistics(companyL1Id, companyL2Id, areaL1Id, areaL2Id, distributorId,
                        startDate, endDate, dateType);
            return Content(data);
        }

        

    }
}