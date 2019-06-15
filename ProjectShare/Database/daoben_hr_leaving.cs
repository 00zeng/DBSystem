using System;

namespace ProjectShare.Database
{
    public class daoben_hr_leaving
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
        /// 是否拥有部门经理？0:1
        /// </summary>
        public int isnosuperior { get; set; }
        /// <summary>
        /// 0-普通职位；1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门/区域经理
        /// </summary>
        public int position_type { get; set; }
        /// <summary>
        /// 所属职位ID
        /// </summary>
        public int position_id { get; set; }
        /// <summary>
        /// 所属职位名称
        /// </summary>
        public string position_name { get; set; }
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
        /// 假期类型：1-事假 2-病假 3-婚假 4-产假 5-丧假 6-其他
        /// </summary>
        public int leaving_type { get; set; }
        /// <summary>
        /// 代职者姓名
        /// </summary>
        public string substitute { get; set; }
        /// <summary>
        /// 代职者ID（表daoben_hr_emp_job）
        /// </summary>
        public string substitute_id { get; set; }
        /// <summary>
        /// 假期开始时间
        /// </summary>
        public DateTime? begin_time { get; set; }
        /// <summary>
        /// 假期结束时间
        /// </summary>
        public DateTime? end_time { get; set; }
        /// <summary>
        /// 天数
        /// </summary>
        public decimal days { get; set; }
        /// <summary>
        /// 实际开始时间
        /// </summary>
        public DateTime? actual_begin_time { get; set; }
        /// <summary>
        /// 实际结束时间
        /// </summary>
        public DateTime? actual_end_time { get; set; }
        /// <summary>
        /// 实际天数
        /// </summary>
        public decimal? actual_days { get; set; }
        /// <summary>
        /// 请假内容-原因
        /// </summary>
        public string content_reason { get; set; }

        /// <summary>
        /// 销假：0-未销假；1-已销假
        /// </summary>
        public bool? leaving_cancel { get; set; }
        /// <summary>
        /// 销假ID（表daoben_hr_leaving_cancel）
        /// </summary>
        public string leaving_cancel_id { get; set; }
        /// <summary>
        /// 状态：-2-审批未结束；-1未休假；1-进行中；2-已结束
        /// </summary>
        public int leaving_status { get; set; }
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