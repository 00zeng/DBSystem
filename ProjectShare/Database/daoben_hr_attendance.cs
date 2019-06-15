using System;

namespace ProjectShare.Database
{
    public class daoben_hr_attendance
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string emp_name { get; set; }
        /// <summary>
        /// 员工ID
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public DateTime? month { get; set; }
        /// <summary>
        /// 应出勤
        /// </summary>
        public decimal work_days { get; set; }
        /// <summary>
        /// 实际出勤
        /// </summary>
        public decimal attendance { get; set; }
        /// <summary>
        /// 出勤率（*100，如40表示40%）
        /// </summary>
        public decimal attendance_rate { get; set; }
        /// <summary>
        /// 表daoben_hr_attendance_approve
        /// </summary>
        public string import_file_id { get; set; }

    }
}