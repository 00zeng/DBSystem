using Base.Code;
using Base.Code.Security;
using System.Linq;
using System;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Process;
using ProjectShare.Models;
using System.Collections.Generic;

namespace ProjectWeb.Application
{
    /// <summary>
    /// 系统定时任务
    /// </summary>
    public class CronApp
    {
        public void ActivityStatus()
        {
            DateTime now = DateTime.Now;
            object startActivityObj = new { activity_status = 1 };
            object endActivityObj = new { activity_status = 2 };
            object startEffectObj = new { effect_status = 1 };
            object endEffectObj = new { effect_status = 2 };
            //消息通知 + 待办事项
            object newsReadObj = new { status = 2 };
            object taskIgnoreObj = new { status = 4 };
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    #region 活动管理
                    db.Update<daoben_activity_attaining>(startActivityObj, a => a.activity_status == -1 && a.start_date <= now);
                    db.Update<daoben_activity_attaining>(endActivityObj, a => a.activity_status == 1 && a.end_date <= now);

                    db.Update<daoben_activity_pk>(startActivityObj, a => a.activity_status == -1 && a.start_date <= now);
                    db.Update<daoben_activity_pk>(endActivityObj, a => a.activity_status == 1 && a.end_date <= now);

                    db.Update<daoben_activity_ranking>(startActivityObj, a => a.activity_status == -1 && a.start_date <= now);
                    db.Update<daoben_activity_ranking>(endActivityObj, a => a.activity_status == 1 && a.end_date <= now);

                    db.Update<daoben_activity_recommendation>(startActivityObj, a => a.activity_status == -1 && a.start_date <= now);
                    db.Update<daoben_activity_recommendation>(endActivityObj, a => a.activity_status == 1 && a.end_date <= now);

                    db.Update<daoben_activity_sales_perf>(startActivityObj, a => a.activity_status == -1 && a.start_date <= now);
                    db.Update<daoben_activity_sales_perf>(endActivityObj, a => a.activity_status == 1 && a.end_date <= now);
                    #endregion
                    #region 经销商活动管理
                    db.Update<daoben_distributor_attaining>(startActivityObj, a => a.activity_status == -1 && a.start_date <= now);
                    db.Update<daoben_distributor_attaining>(endActivityObj, a => a.activity_status == 1 && a.end_date <= now);

                    db.Update<daoben_distributor_image>(startActivityObj, a => a.activity_status == -1 && a.start_date <= now);
                    db.Update<daoben_distributor_image>(endActivityObj, a => a.activity_status == 1 && a.end_date <= now);

                    db.Update<daoben_distributor_recommendation>(startActivityObj, a => a.activity_status == -1 && a.start_date <= now);
                    db.Update<daoben_distributor_recommendation>(endActivityObj, a => a.activity_status == 1 && a.end_date <= now);
                    #endregion

                    #region 岗位薪资
                    List<daoben_salary_position> effectPositionList = db.Queryable<daoben_salary_position>()
                            .Where(a => a.effect_status == -1 && a.effect_date <= now)
                            .Select("id, effect_status, category, company_id").ToList();
                    effectPositionList.ForEach(e =>
                    {   // 先失效，后生效
                        db.Update<daoben_salary_position>(endEffectObj,
                                a => a.effect_status == 1 && a.category == e.category && a.company_id == e.company_id);
                    });
                    List<string> idList = effectPositionList.Select(a => a.id).ToList();
                    db.Update<daoben_salary_position>(startEffectObj, a => idList.Contains(a.id));
                    #endregion


                    #region 消息通知(默认14天) 待办事项（默认30天+到期时间已过）//TODO 待调整
                    db.Update<daoben_sys_notification>(newsReadObj, a => (a.read_time == null && a.create_time <= now.AddDays(-14)));
                    db.Update<daoben_sys_task>(taskIgnoreObj, a => (a.expired_time == null && a.create_time <= now.AddDays(-30))
                            || (a.expired_time != null && a.expired_time <= now));
                    #endregion 
                }
            }
            catch (Exception ex)
            {
                ExceptionApp.WriteLog("CronApp(ActivityStatus)：" + ex.Message);
            }
        }


    }
}
