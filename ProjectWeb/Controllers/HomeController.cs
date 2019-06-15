using Base.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Process;

namespace ProjectWeb.Controllers
{
    public class HomeController : Controller
    {
        [HandlerHomeLogin]
        public ActionResult Index()
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            string account = "";
            int user_id = 0;
            string role_name = "";
            string position_name = "";
            string dept_name = "";
            if (LoginInfo != null)
            {
                account = LoginInfo.account;
                user_id = LoginInfo.accountId;
                role_name = LoginInfo.roleName;
                position_name = LoginInfo.positionInfo.name;
                dept_name = LoginInfo.positionInfo.deptName;
            }
            ViewBag.account = account;
            ViewBag.position_name = position_name;
            ViewBag.dept_name = dept_name;
            ViewBag.role_name = role_name;
            ViewBag.user_id = user_id;
            return View();
        }

        public ActionResult GetNoticeTaskList()
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    //判断太繁琐 -- 只使用id
                    List<daoben_sys_notification> noticeList = db.Queryable<daoben_sys_notification>()
                            .Where(a => a.status == 1 && a.recipient_type == 1 && a.emp_id == LoginInfo.empId).ToList();
                    List<daoben_sys_task> taskList = db.Queryable<daoben_sys_task>()
                            .Where(a => a.status <= 2 && (a.recipient_type == 1 && a.emp_id == LoginInfo.empId)).ToList();
                    var data = new
                    {
                        noticeList = noticeList,
                        taskList = taskList
                    };
                    return Content(data.ToJson());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string Read(int id, int type)
        {
            object upObj = new
            {
                status = 2,
                read_time = DateTime.Now
            };
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    if (type == 1)
                        db.Update<daoben_sys_notification>(upObj, a => a.id == id);
                    else if (type == 2)
                        db.Update<daoben_sys_task>(upObj, a => a.id == id);
                }
                return "success";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ReadNews(int id)
        {
            string result = this.Read(id, 1);
            if (result == "success")
                return null;
            else
                throw new Exception("系统错误：读取消息出错");
        }
        public ActionResult ReadTask(int id)
        {
            string result = this.Read(id, 2);
            if (result == "success")
                return null;
            else
                throw new Exception("系统错误：读取待办事项出错");
        }

        private ActionResult Success(string v, int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}