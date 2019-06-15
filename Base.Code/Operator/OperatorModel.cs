using System;

namespace Base.Code
{
    public class OperatorModel
    {
        public string work_number { get; set; }
        public int accountId { get; set; }
        public string account { get; set; }
        public DateTime loginTime { get; set; }
        public string loginToken { get; set; }
        public int roleId { get; set; }
        public string roleName { get; set; }
        public string empId { get; set; }
        public string empName { get; set; }
        public string jobHistoryId { get; set; }
        /// <summary>
        /// 0-员工账户；1-经销商账户；2-店员账户
        /// </summary>
        public int empType { get; set; }
        public CompanyInfo companyInfo { get; set; }
        public PositionInfo positionInfo { get; set; }
    }

    public class CompanyInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string linkId { get; set; }
        public string linkName { get; set; }
        public string category { get; set; }
        public int parentId { get; set; }
        public string parentName { get; set; }
    }
    public class PositionInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string grade { get; set; }
        /// <summary>
        /// 1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门经理；6-部门主管；7-行政普通员工；
        /// 11-培训经理；12-培训师；21-业务经理；22-业务员；31-导购员
        /// </summary>
        public int positionType { get; set; }
        public int deptId { get; set; }
        public string deptName { get; set; }
        public int areaId { get; set; }
        public string areaName { get; set; }
        /// <summary>
        /// 经理片区
        /// </summary>
        public int areaL1Id { get; set; }
        public string areaL1Name { get; set; }
        /// <summary>
        /// 业务片区
        /// </summary>
        public int areaL2Id { get; set; }
        /// <summary>
        /// 业务片区
        /// </summary>
        public string areaL2Name { get; set; }
        public string supervisorId { get; set; } // 我的上级
        public string supervisorName { get; set; }
    }

}
