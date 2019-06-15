using System;

namespace ProjectShare.Database
{
    public class daoben_salary_position_guide
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 岗位薪资表daoben_salary_position(主表)ID
        /// </summary>
        public string salary_position_id { get; set; }
        /// <summary>
        /// 达标底薪
        /// </summary>
        public decimal standard_salary { get; set; }
        /// <summary>
        /// 达标提成
        /// </summary>
        public decimal standard_commission { get; set; }
        /// <summary>
        /// 奖励启用状态：0-关闭；1-开启
        /// </summary>
        public int increase_award_status { get; set; }
        /// <summary>
        /// 新员工入职increase_month月，介绍人享受介绍费奖励
        /// </summary>
        public int increase_month { get; set; }
        /// <summary>
        /// 保护期，单位：月
        /// </summary>
        public int increase_protect { get; set; }
        /// <summary>
        /// 介绍费奖金
        /// </summary>
        public decimal increase_salary { get; set; }
        /// <summary>
        /// 增员提成
        /// </summary>
        public decimal increase_commission { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime? increase_effect_time { get; set; }
        /// <summary>
        /// 导购员底薪类型：0-0底薪，1-达标底薪，2-星级制，3-浮动底薪，4-保底工资
        /// </summary>
        public int guide_base_type { get; set; }
        /// <summary>
        /// 导购员保底工资
        /// </summary>
        public decimal guide_salary_base { get; set; }
        
        /// <summary>
        /// 年终奖方案类型：1-按销量，2-按星级
        /// </summary>
        public int guide_annualbonus_type { get; set; }
       
        /// <summary>
        /// 将所有个人设置重置为此方案：0-不重置；1-重置；（仅底薪）
        /// </summary>
        public bool reset_all_base { get; set; }
        /// <summary>
        /// 将所有个人设置重置为此方案：0-不重置；1-重置；（仅年终奖）
        /// </summary>
        public bool reset_all_annualbonus { get; set; }



    }
}