using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectShare.Database
{
    public class daoben_salary_benefit_detail
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_activity_ranking
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 福利名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 发放形式：1-工资，2-礼品，3-现金，4-其他形式
        /// </summary>
        public int paid_type { get; set; }
        /// <summary>
        /// 福利金额
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 发放时间（以工资形式发放时该值为月份）
        /// </summary>
        public DateTime? paid_date { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string note { get; set; }


    }
}
