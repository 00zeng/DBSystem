using System;

namespace ProjectShare.Database
{
    public class daoben_hr_emp_resigned_info
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>
        public string wechat { get; set; }
        /// <summary>
        /// 紧急联系人
        /// </summary>
        public string emergency_contact { get; set; }
        /// <summary>
        /// 紧急联系人电话
        /// </summary>
        public string emergency_contact_phone { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string identity_type { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string identity { get; set; }
        /// <summary>
        /// 身份证地址
        /// </summary>
        public string identity_address { get; set; }
        /// <summary>
        /// 证件签发机关
        /// </summary>
        public string identity_issue { get; set; }
        /// <summary>
        /// 证件起始日期
        /// </summary>
        public DateTime identity_effect { get; set; }
        /// <summary>
        /// 证件失效日期
        /// </summary>
        public DateTime identity_expire { get; set; }
        /// <summary>
        /// 性别: 0-未指定，1-男，2-女
        /// </summary>
        public int gender { get; set; }
        /// <summary>
        /// 政治面貌
        /// </summary>
        public string political { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime birthdate { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime birthday { get; set; }
        /// <summary>
        /// 生日类型：1-农历；2-阳历
        /// </summary>
        public int birthday_type { get; set; }
        /// <summary>
        /// 0-未婚；1-已婚；2-离异；3-丧偶；4-其他
        /// </summary>
        public int marriage { get; set; }
        /// <summary>
        /// 籍贯
        /// </summary>
        public string native { get; set; }
        /// <summary>
        /// 子女个数
        /// </summary>
        public int child_count { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 现居住地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 健康证起始日期
        /// </summary>
        public DateTime health_start { get; set; }
        /// <summary>
        /// 健康证失效日期
        /// </summary>
        public DateTime health_expire { get; set; }
        /// <summary>
        /// 文化程度
        /// </summary>
        public string education { get; set; }
        /// <summary>
        /// 专业
        /// </summary>
        public string profession { get; set; }
        /// <summary>
        /// 毕业院校
        /// </summary>
        public string graduation_school { get; set; }
        /// <summary>
        /// 毕业时间
        /// </summary>
        public DateTime graduation_date { get; set; }
        /// <summary>
        /// 宗教信仰
        /// </summary>
        public string religion { get; set; }
        /// <summary>
        /// 头像ID，表daoben_image
        /// </summary>
        public string avatar_id { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string avatar_url { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 银行类别
        /// </summary>
        public string bank_type { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string bank_name { get; set; }
        /// <summary>
        /// 银行账户
        /// </summary>
        public string bank_account { get; set; }
        /// <summary>
        /// 父母开户银行
        /// </summary>
        public string parents_bank { get; set; }
        /// <summary>
        /// 父母银行卡号
        /// </summary>
        public string parents_bankaccount { get; set; }
        /// <summary>
        /// 父母联系电话
        /// </summary>
        public string parents_phone { get; set; }
        /// <summary>
        /// 信息提交人账户ID，表daoben_ms_account
        /// </summary>
        public int submitter_id { get; set; }
        /// <summary>
        /// 信息提交人姓名
        /// </summary>
        public string submitter_name { get; set; }
        /// <summary>
        /// 信息提交时间 
        /// </summary>
        public DateTime submit_time { get; set; }
    }
}