using System;

namespace ProjectShare.Database
{
    public class daoben_hr_leaving_approve
    {
        /// <summary>
        /// 请假审批表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_hr_leaving
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；大于0通过；小于0不通过；以100作为审批完成的标志
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 审批人ID
        /// </summary>
        public int approve_id { get; set; }
        /// <summary>
        /// 审批人姓名（非账户名）
        /// </summary>
        public string approve_name { get; set; }
        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? approve_time { get; set; }
        /// <summary>
        /// 审批职位ID
        /// </summary>
        public int approve_position_id { get; set; }
        /// <summary>
        /// 审批职位名称
        /// </summary>
        public string approve_position_name { get; set; }
        /// <summary>
        /// 审批备注
        /// </summary>
        public string approve_note { get; set; }

    }
}