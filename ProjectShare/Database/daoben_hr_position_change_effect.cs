using System;

namespace ProjectShare.Database
{
    public class daoben_hr_position_change_effect
    {
        /// <summary>
        /// 调岗结果的原下属变更表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_hr_position_change
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 原下属ID
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 原下属的新上级ID
        /// </summary>
        public string supervisor_id { get; set; }
        /// <summary>
        /// 原下属的新上级ID姓名（非账户名）
        /// </summary>
        public string supervisor_name { get; set; }
        /// <summary>
        /// 0-未处理；1-已处理（系统定时任务）
        /// </summary>
        public bool is_finished { get; set; }



    }
}