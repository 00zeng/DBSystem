using System;

namespace ProjectShare.Database
{
    public class daoben_hr_grade_change
    {
        /// <summary>
        /// 员工请假信息表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string work_number { get; set; }
        /// <summary>
        /// 对应员工ID
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string emp_name { get; set; }
        /// <summary>
        /// 转岗类型：1-转区域 2-转部门 3-机构
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 发起转岗时间
        /// </summary>
        public DateTime? request_date { get; set; }
        /// <summary>
        /// 核准转岗时间
        /// </summary>
        public DateTime? date_approve { get; set; }
        /// <summary>
        /// 所属职位ID
        /// </summary>
        public int position_id { get; set; }
        /// <summary>
        /// 所属职位名称
        /// </summary>
        public string position_name { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public string grade { get; set; }
        /// <summary>
        /// 新等级
        /// </summary>
        public string grade_new { get; set; }
        /// <summary>
        /// 所属部门ID
        /// </summary>
        public int dept_id { get; set; }
        /// <summary>
        /// 所属部门名称
        /// </summary>
        public string dept_name { get; set; }
        /// <summary>
        /// 所属区域ID
        /// </summary>
        public int area_id { get; set; }
        /// <summary>
        /// 所属区域名称
        /// </summary>
        public string area_name { get; set; }
        /// <summary>
        /// 所属机构ID
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 所属机构上级ID
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 0-普通职位；1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门/区域经理
        /// </summary>
        public int position_type_new { get; set; }
        /// <summary>
        /// 0-普通职位；1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门/区域经理
        /// </summary>
        public int position_type { get; set; }

        /// <summary>
        /// 新职位ID
        /// </summary>
        public int position_id_new { get; set; }
        /// <summary>
        /// 新职位名称
        /// </summary>
        public string position_name_new { get; set; }
        /// <summary>
        /// 新部门ID
        /// </summary>
        public int dept_id_new { get; set; }
        /// <summary>
        /// 新部门名称
        /// </summary>
        public string dept_name_new { get; set; }
        /// <summary>
        /// 新区域ID
        /// </summary>
        public int area_id_new { get; set; }
        /// <summary>
        /// 新区域名称
        /// </summary>
        public string area_name_new { get; set; }
        /// <summary>
        /// 新机构ID
        /// </summary>
        public int company_id_new { get; set; }
        /// <summary>
        /// 新机构名称（LinkName）
        /// </summary>
        public string company_name_new { get; set; }
        /// <summary>
        /// 新机构上级ID
        /// </summary>
        public int company_id_parent_new { get; set; }
        /// <summary>
        /// 新上级ID（人事审批时补充）
        /// </summary>
        public string supervisor_id_new { get; set; }
        /// <summary>
        /// 新上级ID姓名（非账户名）（人事审批时补充）
        /// </summary>
        public string supervisor_name_new { get; set; }
        /// <summary>
        /// 状态：-2-审批未结束；-1未执行；2-已完成
        /// </summary>
        public int execute_status { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；100表示审批完成且通过
        /// </summary>
        public int approve_status { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }


    }
}