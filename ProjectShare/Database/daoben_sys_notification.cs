using System;
namespace ProjectShare.Database
{
    public class daoben_sys_notification
    {
        /// <summary>
        /// 通知消息表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 消息类型（决定main_id对应主表）：1-机构新增;2-审批结果通知;3-指派关系变动
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 主表1~100活动管理，101~200经销商管理，201~300人力资源，301~400组织架构；401~500薪资结算，501~600财务核算，601~700销售管理
        /// </summary>
        public int main_table { get; set; }
        /// <summary>
        /// 跳转路径
        /// </summary>
        public string main_url { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string content_abstract { get; set; }
        /// <summary>
        /// 收件人类型：1-员工；2-部门+角色；3-机构+角色；4-区域+角色；5-机构+职位类型;6-职位类型
        /// </summary>
        public int recipient_type { get; set; }
        /// <summary>
        /// 收件人ID
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 收件部门
        /// </summary>
        public int recipient_dept_id { get; set; }
        /// <summary>
        /// 收件机构
        /// </summary>
        public int recipient_company_id { get; set; }
        /// <summary>
        /// 收件区域
        /// </summary>
        public int recipient_area_id { get; set; }
        /// <summary>
        /// 收件角色
        /// </summary>
        public int recipient_role_id { get; set; }
        /// <summary>
        /// 收件职位类型position_type：1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门经理；6-部门主管；7-行政普通员工；11-培训经理；12-培训师；21-业务经理；22-业务员；31-导购员
        /// </summary>
        public int recipient_position_type { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_time { get; set; }
        /// <summary>
        /// 1-未读；2-已读
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 查看时间
        /// </summary>
        public DateTime? read_time { get; set; }



    }
}