using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectWeb.Areas.SubordinateManage.Application
{
    public class MySubordinateApp

    {
        public object GetList(Pagination pagination, daoben_hr_emp_job queryInfo, string name, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_job>();
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_TERMINALMANAGER)
                    {
                        // 终端经理，查看所有培训师/培训经理/业务员/业务经理/导购员 TODO
                        qable.JoinTable<daoben_org_position>((a, c) => a.position_id == c.id && c.position_type != ConstData.POSITION_OFFICE_NORMAL)
                                .Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_SALESMANAGER)
                    {
                        // 业务经理（区域经理），查看本区域内的所有业务员/导购员
                        qable.Where(a => a.area_l1_id == myPositionInfo.areaL1Id && a.id != LoginInfo.empId);
                    }
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_SALES)
                    {
                        // 业务员，查看挂勾的导购员
                    }
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_TRAINER)
                    {
                        // 培训师，查看挂勾的导购员
                    }
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_TRAINERMANAGER)
                    {
                        // 培训经理，查看下属培训师及培训师挂勾的导购员
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1
                                    || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                    {   // 事业部
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)   // 分公司
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                        qable.Where(a => a.dept_id == myPositionInfo.id);
                    else
                        qable.Where(a => a.supervisor_id == LoginInfo.empId);
                }

                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.work_number))
                        qable.Where(a => a.work_number.Contains(queryInfo.work_number));
                    if (queryInfo.company_id != 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                }
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.entry_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.entry_date < queryTime.startTime2);
                    }
                }

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord) // 差售点
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetAllocationList(Pagination pagination, string name)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                // 业务员/业务经理/导购员
                //增加培训师，培训经理 
                var qable = db.Queryable<daoben_hr_emp_job>()
                        .Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id))
                        .Where(a => a.position_type >= ConstData.POSITION_TRAINERMANAGER && a.area_l1_id == 0 && a.id != LoginInfo.empId);

                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));


                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public string GetInfo(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_info empInfo = db.Queryable<daoben_hr_emp_info>().InSingle(id);
                    daoben_hr_emp_job empJobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(id);
                    if (empJobInfo == null || empInfo == null)
                        return "信息错误：指定的员工信息不存在";
                    daoben_org_position posInfo = db.Queryable<daoben_org_position>().InSingle(empJobInfo.position_id);
                    if (posInfo == null)
                        return "信息错误：指定员工的职位信息不存在";
                    List<daoben_hr_emp_file> imageList = db.Queryable<daoben_hr_emp_file>()
                                .Where(a => a.main_id == id && a.is_del == false)
                                .OrderBy(a => a.type, OrderByType.Asc).ToList();
                    object resultObj = new
                    {
                        empJobInfo = empJobInfo,
                        posInfo = posInfo,
                        empInfo = empInfo,
                        imageList = imageList
                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public string SalesAdd(string empId, int areaId, daoben_distributor_info distributorInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empJob = db.Queryable<daoben_hr_emp_job>().InSingle(empId);
                    if (empJob == null)
                        return "信息错误：指定的业务员不存在！";
                    daoben_org_area areaInfo = db.Queryable<daoben_org_area>().InSingle(areaId);
                    if (areaInfo == null)
                        return "信息错误：指定的区域不存在！";
                    daoben_distributor_info origDistributor = db.Queryable<daoben_distributor_info>().InSingle(distributorInfo.id);
                    if (areaInfo == null)
                        return "信息错误：指定的经销商不存在！";
                    object upObj1 = new
                    {
                        sales_id = empJob.id,
                        sales_name = empJob.name,
                    };
                    object upObj2 = new
                    {
                        area_id = areaInfo.id,
                        area_name = areaInfo.name,
                    };
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_distributor_info>(upObj1, a => a.id == distributorInfo.id);
                    db.Update<daoben_hr_emp_job>(upObj2, a => a.id == empId);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                return "success";
            }
        }
        public string TrainerAdd(string empId, int areaId, daoben_distributor_info distributorInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empJob = db.Queryable<daoben_hr_emp_job>().InSingle(empId);
                    if (empJob == null)
                        return "信息错误：指定的培训师不存在！";
                    daoben_org_area areaInfo = db.Queryable<daoben_org_area>().InSingle(areaId);
                    if (areaInfo == null)
                        return "信息错误：指定的区域不存在！";
                    daoben_distributor_info origDistributor = db.Queryable<daoben_distributor_info>().InSingle(distributorInfo.id);
                    if (areaInfo == null)
                        return "信息错误：指定的经销商不存在！";
                    object upObj1 = new
                    {
                        trainer_id = empJob.id,
                        trainer_name = empJob.name,
                    };
                    object upObj2 = new
                    {
                        area_id = areaInfo.id,
                        area_name = areaInfo.name,
                    };
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_distributor_info>(upObj1, a => a.id == distributorInfo.id);
                    db.Update<daoben_hr_emp_job>(upObj2, a => a.id == empId);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                return "success";
            }
        }
        public string SalesManageAdd(string empId, int areaId)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empJob = db.Queryable<daoben_hr_emp_job>().InSingle(empId);
                    if (empJob == null)
                        return "信息错误：指定的业务经理不存在！";
                    daoben_org_area areaInfo = db.Queryable<daoben_org_area>().InSingle(areaId);
                    if (areaInfo == null)
                        return "信息错误：指定的区域不存在！";

                    //区域信息修改
                    object upObj = new
                    {
                        area_id = areaInfo.id,
                        area_name = areaInfo.name,
                    };
                    db.Update<daoben_hr_emp_job>(upObj, a => a.id == empId);
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
                return "success";
            }
        }

        public string TrainerManageAdd(string empId, int areaId)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empJob = db.Queryable<daoben_hr_emp_job>().InSingle(empId);
                    if (empJob == null)
                        return "信息错误：指定的培训经理不存在！";
                    daoben_org_area areaInfo = db.Queryable<daoben_org_area>().InSingle(areaId);
                    if (areaInfo == null)
                        return "信息错误：指定的区域不存在！";

                    //区域信息修改
                    object upObj = new
                    {
                        area_id = areaInfo.id,
                        area_name = areaInfo.name,
                    };
                    db.Update<daoben_hr_emp_job>(upObj, a => a.id == empId);
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
                return "success";
            }
        }

        public string GuideAdd(string empId, int areaId, daoben_distributor_info distributorInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job salesInfo = null;
                    daoben_distributor_guide guideLink = new daoben_distributor_guide();
                    daoben_hr_guide_link salesLink = new daoben_hr_guide_link();
                    daoben_hr_guide_link trainerLink = new daoben_hr_guide_link();

                    daoben_distributor_info origDistributor = db.Queryable<daoben_distributor_info>().InSingle(distributorInfo.id);
                    if (origDistributor == null)
                        return "信息错误：指定的经销商不存在！";
                    daoben_org_area areaInfo = db.Queryable<daoben_org_area>().InSingle(areaId);
                    if (areaInfo == null)
                        return "信息错误：指定的区域不存在！";
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().InSingle(empId);
                    if (empInfo == null)
                        return "信息错误：指定的导购员不存在！";

                    guideLink.guide_id = empInfo.id;
                    guideLink.guide_name = empInfo.name;
                    guideLink.distributor_id = origDistributor.id;
                    guideLink.distributor_name = origDistributor.name;

                    guideLink.creator_job_history_id = LoginInfo.jobHistoryId;
                    guideLink.creator_name = LoginInfo.empName;
                    guideLink.create_time = DateTime.Now;
                    guideLink.create_time = DateTime.Now;
                    guideLink.inactive = false;

                    if (!string.IsNullOrEmpty(origDistributor.sales_id))
                    {
                        salesInfo = db.Queryable<daoben_hr_emp_job>().InSingle(origDistributor.sales_id);
                        salesLink.emp_id = salesInfo.id;
                        salesLink.guide_id = empId;
                        salesLink.emp_type = 1;
                        salesLink.create_time = DateTime.Now;
                        salesLink.creator_job_history_id = LoginInfo.jobHistoryId;
                        salesLink.creator_name = LoginInfo.empName;
                        salesLink.create_time = DateTime.Now;

                    }
                    //if (!string.IsNullOrEmpty(origDistributor.trainer_id))
                    //{
                    //    trainerInfo = db.Queryable<daoben_hr_emp_job>().InSingle(origDistributor.trainer_id);
                    //    trainerLink.emp_id = trainerInfo.id;
                    //    trainerLink.guide_id = empId;
                    //    trainerLink.emp_type = 2;
                    //    trainerLink.create_time = DateTime.Now;
                    //    trainerLink.creator_id = LoginInfo.accountId;
                    //    trainerLink.creator_name = LoginInfo.empName;
                    //}

                    //区域信息修改
                    object upObj = new
                    {
                        area_id = areaInfo.id,
                        area_name = areaInfo.name,
                    };

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    if (!string.IsNullOrEmpty(salesLink.guide_id))
                        db.Insert(salesLink);
                    if (!string.IsNullOrEmpty(trainerLink.guide_id))
                        db.Insert(trainerLink);
                    db.Insert(guideLink);
                    db.Update<daoben_hr_emp_job>(upObj, a => a.id == empId);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                return "success";
            }
        }

        public string GetDistributorList(int area_id)
        {
            return null;
            // TODO
            //OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            //if (LoginInfo == null)
            //    throw new Exception("用户登陆过期，请重新登录");
            //CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            //PositionInfo myPositionInfo = LoginInfo.positionInfo;

            //using (var db = SugarDao.GetInstance())
            //{
            //    if (area_id == 0)
            //    {
            //        List<daoben_distributor_info> distributorList = db.Queryable<daoben_distributor_info>()
            //            .Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id)).ToList();
            //        return distributorList.ToJson();
            //    }
            //    else
            //    {
            //        List<daoben_distributor_info> distributorList = db.Queryable<daoben_distributor_info>()
            //                .Where(a => a.area_id == area_id)
            //                .Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id)).ToList();
            //        return distributorList.ToJson();
            //    }

            //}
        }

        // TODO Delete this function
        /// <summary>
        /// 根据机构获取部门/区域选择列表
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="type">0: 业务经理；1：业务员；2：导购员</param>
        /// <returns></returns>
        public string GetAreaEmpList(int areaId, uint type)
        {
            if (areaId < 1 || type > 2)
                return null;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_job>()
                            .JoinTable<daoben_org_position>((a, b) => a.position_id == b.id);
                if (type == 0)
                {
                    qable.Where<daoben_org_position>((a, b) => a.area_l1_id == areaId
                            && b.position_type == ConstData.POSITION_SALESMANAGER);
                }
                else if (type == 1)
                {
                    qable.Where<daoben_org_position>((a, b) => a.area_l1_id == areaId
                            && b.position_type == ConstData.POSITION_SALES);
                }
                else
                {
                    qable.Where<daoben_org_position>((a, b) => a.area_l1_id == areaId
                            && b.position_type == ConstData.POSITION_GUIDE);
                }
                string pairListStr = qable.Select<IdNamePair>("a.id, a.name").ToJson();

                return pairListStr;
            }
        }

    }
}
