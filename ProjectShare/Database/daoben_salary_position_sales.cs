namespace ProjectShare.Database
{
    public class daoben_salary_position_sales
    {
        /// <summary>
        /// 岗位薪资-业务薪资主表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 岗位薪资表daoben_salary_position(主表)ID
        /// </summary>
        public string salary_position_id { get; set; }
        
        /// <summary>
        /// 将所有个人设置重置为此方案：0-不重置；1-重置；（仅提成考核）
        /// </summary>
        public bool reset_all_perf { get; set; }
        /// <summary>
        /// 计量方式：1-按实销量；2-按下货量
        /// </summary>
        public int target_content { get; set; }
        /// <summary>
        /// 奖励模式：1-按完成率；2-按台数；3-按零售价；4-按型号；5-按批发价
        /// </summary>
        public int target_mode { get; set; }
        /// <summary>
        /// 正常机：金额计算方式：1-每台固定金额；2-每台批发价比例；3-每台零售价比例；4-固定总金额
        /// </summary>
        public int normal_rebate_mode { get; set; }
        /// <summary>
        /// 买断机：金额计算方式：1-每台固定金额；2-每台批发价比例；3-每台买断价比例；4-固定总金额
        /// </summary>
        public int buyout_rebate_mode { get; set; }
        /// <summary>
        /// 目标销量（台），仅当target_mode为“1-按完成率返利”有效
        /// </summary>
        public int activity_target { get; set; }
        /// <summary>
        /// 活动状态：-2-审核未结束；-1未开始；1-进行中；2-已结束
        /// </summary>
        public int activity_status { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；100表示审批完成且通过
        /// </summary>
        public int approve_status { get; set; }
        /// <summary>
        /// 累计完成销量（台）
        /// </summary>
        public int total_sale_count { get; set; }
        /// <summary>
        /// 累计完成销量总金额（元）
        /// </summary>
        public decimal total_sale_amount { get; set; }
        /// <summary>
        /// 累计奖金（元）
        /// </summary>
        public decimal total_reward { get; set; }
        /// <summary>
        /// 累计罚款（元）
        /// </summary>
        public decimal total_penalty { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string note { get; set; }


    }
}