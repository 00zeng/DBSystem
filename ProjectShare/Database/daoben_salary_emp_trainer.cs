using System;

namespace ProjectShare.Database
{
    public class daoben_salary_emp_trainer
    {
        /// <summary>
        /// 薪资设置表-培训考核表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 员工薪资表daoben_salary_emp(主表)ID
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 1-V雪球活跃人数；2-V雪球转化率；3-导购人均产值；4-导购离职率；5-高端机占比；6-培训场次；7-形象执行效率；8-形象罚款；（<-培训师 | 培训经理->）51-导购人均产值；52-导购离职率；53-高端机占比
        /// </summary>
        public int kpi_type { get; set; }
        /// <summary>
        /// 0-不考核该项；1-考核该项
        /// </summary>
        public bool is_included { get; set; }
        /// <summary>
        /// 经理片区，表daoben_org_area
        /// </summary>
        public int area_l1_id { get; set; }
        /// <summary>
        /// 经理片区名称
        /// </summary>
        public string area_l1_name { get; set; }



    }
}