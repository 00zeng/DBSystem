using System.Collections.Generic;

namespace ProjectShare.Models
{
    public class ConstData
    {
        public const int EXPORT_SHEET_LEN = 65535;

        public const string EXCEL_UP_PATH = "~/ExcelUpload/";//存放Excel路径
        public const string FILE_UP_PATH = "/FileUp/";//存放图片路径
        public const string EMP_FILE_PATH = FILE_UP_PATH + "EmpInfo/";      //存放图片路径-个人信息
        public const string EMP_LEAVING_PATH = FILE_UP_PATH + "EmpLeaving/";//存放图片路径-请假
        public const string EMP_CAREER_PATH = FILE_UP_PATH + "EmpCareer/";  //存放图片路径-岗位调整
        public const string EMP_GRADE_PATH = FILE_UP_PATH + "EmpGrade/";    //存放图片路径-晋升降级
        public const string EMP_RESIGN_PATH = FILE_UP_PATH + "EmpResign/";  //存放图片路径-离职
        public const string DISTRIBUTOR_FILE_PATH = FILE_UP_PATH + "DistributorInfo/";      //存放图片路径-经销商信息
        public const string REBATE_FILE_PATH = FILE_UP_PATH + "ImageRebate/";      //存放图片路径-经销商信息

        public const string THUMBPREFIX = "thumb_";//缩略图前缀(前端有调用)

        public const string DEFAULTMENUCODE = "0";//默认一级菜单code
        public const int ROLE_ID_ADMIN = 1;             // 超级管理员
        public const int HR_DEPT = 1;   // 人事部
        public const int FINANCIAL_DEPT = 2;   // 财务部

        //positionType = 0 是错误数据
        public const int POSITION_GM1 = 1;              // 事业部总经理
        public const int POSITION_GM_ASSISTANT1 = 2;    // 事业部助理
        public const int POSITION_GM2 = 3;              // 分公司总经理
        public const int POSITION_GM_ASSISTANT2 = 4;    // 分公司助理
        public const int POSITION_DEPT_M = 5;           // 部门/区域经理
        public const int POSITION_DIRECTOR = 6;         // 部门主管
        public const int POSITION_OFFICE_NORMAL = 7;    // 行政普通员工
        public const int POSITION_TRAINERMANAGER = 11;  // 培训经理
        public const int POSITION_TRAINER = 12;         // 培训师
        public const int POSITION_SALESMANAGER = 21;    // 业务经理
        public const int POSITION_SALES = 22;           // 业务员
        public const int POSITION_GUIDE = 31;           // 导购员

        public const int ROLE_ID_NEW_EMP = 20;     // 未完善信息的新入职员工，该角色只有完善个人信息的权限
        public const int ROLE_ID_CLERK = 2;     // 店员
        public const int ROLE_ID_DISTRIBUTOR = 3;   // 经销商
        public const int ROLE_ID_GUIDE = 4;     // 导购员
        public const int ROLE_ID_SALES = 5;     // 业务员
        public const int ROLE_ID_SALESMANAGER = 6;      // 业务经理
        public const int ROLE_ID_TRAINER = 7;               // 培训师
        public const int ROLE_ID_TRAINERMANAGER = 8;        // 培训经理
        public const int ROLE_ID_TERMINALMANAGER = 9;       // 终端经理
        public const int ROLE_ID_HR = 10;                   // 人事文员
        public const int ROLE_ID_HRMANAGER = 11;             // 人事经理
        public const int ROLE_ID_FINANCIAL = 12;            // 工资财务
        public const int ROLE_ID_FINANCIALMANAGER = 13;     // 财务经理

        // todo:更新菜单数据表时需更改 2019-3-6
        public const string MENU_ID_POSITIONSALARY = "0302";
        public const string BUTTON_ID_DEPT_KPI_ADJUST = "030206";   // 岗位薪资菜单-部门KPI调整按钮ID
        public const string BUTTON_ID_TRAINER_KPI_ADJUST = "030208";   // 岗位薪资菜单-培训KPI调整按钮ID


        // 返回前端状态信息
        public const int OK_STATUS = 1;
        public const int ERR_STATUS_AUTH = -101;
        public const string ERR_MSG_AUTH = "操作失败：没有审批权限";


        public enum AuthorityCategory
        {
            MENU = 1,
            BUTTON = 2
        };

        public static List<IdNamePair> OrgCategory()
        {
            List<IdNamePair> pairList = new List<IdNamePair>();
            pairList.Add(new IdNamePair { id = "1", name = "董事会" });
            pairList.Add(new IdNamePair { id = "2", name = "事业部" });
            pairList.Add(new IdNamePair { id = "3", name = "分公司" });
            return pairList;
        }
    }
}