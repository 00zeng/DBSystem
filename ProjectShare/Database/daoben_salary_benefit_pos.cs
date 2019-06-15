namespace ProjectShare.Database
{
    public class daoben_salary_benefit_emp
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_salary_benefit
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 员工ID
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string emp_name { get; set; }
        /// <summary>
        /// 页面显示信息（职位名称-部门/区域）
        /// </summary>
        public string display_info { get; set; }
    }
}