var EmpInfo = [];
$(function () {
    ActionBinding();
})
// 绑定标签事件
function ActionBinding() {
    $("#company_id").on("change", function (e) {
        GetDeptList();
    });
    $("#dept_id").on("change", function (e) {
        $("#bigBox").empty();
        GetEmpSalaryList();
    });
}
$("#company_id").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameList",
    search: true,
    firstText: "--请选择机构--",
});
//经理片区--调区
function GetDeptList() {
    var curCompany = $("#company_id").val();
    $("#dept_id").empty();
    if (curCompany > 0) {
        $("#dept_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Department/GetIdNameList",
            param: { company_id: curCompany },
            firstText: "--请选择部门--",
        });
    }
}
function GetEmpSalaryList() {
    $.ajax({
        url: "/SalaryCalculate/PayrollOffice/GetSettingInfoList?dept_id=" + $("#dept_id").val(),
        type: "get",
        dataType: "json",
        success: function (data) {
            var boxClone = "";
            var salary = 0;
            resList = data.resList;
            resLen = resList.length;  
            if (resLen <= 0) {
                $("#orgBox").css("dispaly", "");
                $("#warningMsg").text("该部门无员工，请重新选择！");
            } else {
                $("#orgBox").css("dispaly", "none");
                for (var i = 0; i < resLen ; i++) {
                    salary = resList[i].empInfo.base_salary + resList[i].empInfo.position_salary + resList[i].empInfo.house_subsidy + resList[i].empInfo.traffic_subsidy + resList[i].empInfo.attendance_reward
                           + resList[i].empInfo.kpi + resList[i].empInfo.seniority_salary + resList[i].empInfo.salary_subsidy;
                    //salary = base_salary + position_salary + house_subsidy + traffic_subsidy + attendance_reward + kpi + seniority_salary + salary_subsidy;
                    boxClone = '<div class="box box-widget" name="cal_' + i + '" data-index=' + i + '>'
                            + '<div class="box-header">'
                            + '<span style="margin-left:20px;">' + resList[i].empInfo.name + '</span>'
                            + '<span style="margin-left:36px;">' + resList[i].empInfo.position_name + '</span>'
                            + '<span style="margin-left:36px;">' + resList[i].empInfo.grade + '</span>'
                            + '<span style="margin-left:36px;">入职时间：' + resList[i].empInfo.entry_date.substring(0, 10) + '</span>'
                            + '</div>'
                            + '<div class="box-body">'
                            + '<div class="row">'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon no-border">应发工资</span>'
                            + '<span id="salary_' + i + '" name="salary" class="form-control">' + salary + '</span>'
                            + '<span class="input-group-addon">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon no-border  text-blue">实发工资</span>'
                            + '<input type="text" id="actual_salary_' + i + '" name="actual_salary" class="form-control required number  text-blue">'
                            + '<span class="input-group-addon  text-blue">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-4">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon  no-border text-blue">备&nbsp;&nbsp;&nbsp;&nbsp;注&nbsp;&nbsp;&nbsp;</span>'
                            + '<input type="text" id="note_' + i + '" name="note" class="form-control text-blue"/>'
                            + '</div></div></div>'
                            + '<div class="row">'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon  no-border  text-blue">应&nbsp;&nbsp;出&nbsp;&nbsp;勤</span>'
                            + '<input type="text" id="work_days_' + i + '" name="work_days" class="form-control required number text-blue" onblur="Work()" value="26" />'
                            + '<span class="input-group-addon   text-blue">天</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon  no-border  text-blue">实际出勤</span>'
                            + '<input type="text" id="onduty_day_' + i + '" name="onduty_day" class="form-control required number text-blue" onblur="Work()" />'
                            + '<span class="input-group-addon  text-blue ">天</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon  no-border  text-blue">请假扣款</span>'
                            + '<input type="text" id="leaving_deduction_' + i + '" name="leaving_deduction" class="form-control required number text-blue">'
                            + '<span class="input-group-addon  text-blue">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon  no-border text-blue">风&nbsp; 险&nbsp;金&nbsp;</span>'
                            + '<input type="text" id="resign_deposit_' + i + '" name="resign_deposit" class="form-control required number text-blue">'
                            + '<span class="input-group-addon text-blue">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon  no-border text-blue">社&nbsp;&nbsp;&nbsp;&nbsp;保&nbsp;&nbsp;&nbsp;</span>'
                            + '<input type="text" id="insurance_fee_' + i + '" name="insurance_fee" class="form-control required number text-blue">'
                            + '<span class="input-group-addon text-blue">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2"  name="otherInput' + i + '">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon  no-border text-blue">其&nbsp;&nbsp;&nbsp;&nbsp;他&nbsp;&nbsp;&nbsp;</span>'
                            + '<input type="text" name="others" value="0" class="form-control  text-blue" onblur="Others()">'
                            + '<span class="input-group-addon text-blue">元</span>'
                            + '<span class="input-group-addon no-border"><i class="fa fa-plus text-blue" onclick="addRow(this)"></i></span>'
                            + '</div></div></div>'
                            + '<div class="row">'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon no-border  ">基本工资</span>'
                            + '<span type="text" id="base_salary_' + i + '" name="base_salary" class="form-control">12</span>'
                            + '<span class="input-group-addon  ">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon no-border  ">岗位工资</span>'
                            + '<span type="text" id="position_salary_' + i + '" name="position_salary" class="form-control">12</span>'
                            + '<span class="input-group-addon  ">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon no-border  ">住房补贴</span>'
                            + '<span type="text" id="house_subsidy_' + i + '" name="house_subsidy" class="form-control">12</span>'
                            + '<span class="input-group-addon  ">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon no-border  ">交通补贴</span>'
                            + '<span type="text" id="traffic_subsidy_' + i + '" name="traffic_subsidy" class="form-control">12</span>'
                            + '<span class="input-group-addon  ">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon no-border  ">工龄工资</span>'
                            + '<span type="text" id="seniority_salary_' + i + '" name="seniority_salary" class="form-control">12</span>'
                            + '<span class="input-group-addon  ">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon no-border  ">特聘补贴</span>'
                            + '<span type="text" id="salary_subsidy_' + i + '" name="salary_subsidy" class="form-control">12</span>'
                            + '<span class="input-group-addon  ">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon no-border  ">KPI&nbsp;工资</span>'
                            + '<span type="text" id="kpi_' + i + '" name="kpi" class="form-control">12</span>'
                            + '<span class="input-group-addon  ">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon no-border  ">全勤奖金</span>'
                            + '<span id="attendance_reward_' + i + '" name="attendance_reward" class="form-control">12</span>'
                            + '<span class="input-group-addon  ">元</span>'
                            + '</div></div>'
                            + '<div id="staySubsidyWrap">'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon  no-border">留守补助-公司</span>'
                            + '<span id="stay_subsidy_company_' + i + '" name="stay_subsidy_company" class="form-control">12</span>'
                            + '<span class="input-group-addon">元</span>'
                            + '</div></div>'
                            + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                            + '<div class="input-group margin-bottom formValue">'
                            + '<span class="input-group-addon  no-border">留守补助-个人</span>'
                            + '<span id="stay_subsidy_personal_' + i + '" name="stay_subsidy_personal" class="form-control">12</span>'
                            + '<span class="input-group-addon">元</span>'
                            + '</div></div></div></div></div></div>';
                    $("#bigBox").append(boxClone);
                }
                getSalary();

                for (var i = 0; i < resLen ; i++) {

                }
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, "error");
        }
    });
}
//其他
function addRow(e) {
    var box_index = $(e).parents("div[name^=cal]").data("index");
    console.log(box_index);
    var Tr = '<div class="col-xs-12 col-sm-6 col-lg-2" >'
        + '<div class="input-group margin-bottom formValue">'
        + '<span class="input-group-addon  no-border text-blue">其&nbsp;&nbsp;&nbsp;&nbsp;他&nbsp;&nbsp;&nbsp;</span>'
        + '<input type="text" name="others" value="0" class="form-control  text-blue" onblur="Others()">'
        + '<span class="input-group-addon text-blue">元</span>'
        + '<span class="input-group-addon no-border"><i class="fa fa-minus text-red" onclick="deleteRow(this)"></i></span>'
        + '</div></div>';
  
    $("div[name=otherInput" + box_index+"]").after(Tr);
}
function deleteRow(e) {
    $(e).parent().parent().parent().remove();
}

function getSalary() {
    for (var i = 0; i < resLen ; i++) {



    }
    salary = base_salary + position_salary + house_subsidy + traffic_subsidy + attendance_reward + kpi + seniority_salary + salary_subsidy;
    if (emp_category == "实习生") {
        if (intern_salary_type == 1) { //比例
            salary = Number(salary) * Number(intern_salary) / 100.00;
        } else if (intern_salary_type == 2) {//固定金额
            salary = intern_salary;
        }
    }
    $("#salary").text(ToThousandsStr(salary));
}
