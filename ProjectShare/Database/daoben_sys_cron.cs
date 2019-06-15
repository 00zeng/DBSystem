using System;

namespace ProjectShare.Database
{
    public class daoben_sys_cron
    {
        /// <summary>
        /// 系统定时任务表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表IDID
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public DateTime? month { get; set; }
        /// <summary>
        /// 任务类型：11-员工福利；12-留守补助；
        /// 111-出库导入检查；112-买断导入检查；113-实销导入检查
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 0-未处理；1-已处理（预留，目前已处理的直接删除）
        /// </summary>
        public bool is_finished { get; set; }
    }
}