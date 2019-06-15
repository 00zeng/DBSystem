var id = $.request("id");
var salary = 0;//应发
var base_salary = 0;//基本工资
var position_salary = 0;//岗位工资
var house_subsidy = 0;//住房补贴
var traffic_subsidy = 0;//交通补贴
var attendance_reward = 0;//全勤奖金
var attendance_reward_old = 0;//全勤奖金
var kpi = 0;//KPI工资
var kpi_standard = 0;//KPI标准
var kpi_standard_score = 0;//KPI评分
var kpi_subsidy = 0;//KPI补贴
var kpi_subsidy_score = 0;//KPI类型
var seniority_salary = 0;//工龄工资
var salary_subsidy = 0;//特聘补贴
var leaving_deduction = 0;//请假扣款
var resign_deposit = 0;//风险金
var insurance_fee = 0;//社保
var actual_salary = 0;//实发
var BenefitRewardsTotal = 0;//福利+奖罚
var rewards = 0;
var benefits = 0;
var others = 0;
var emp_category;
var intern_salary = 0;//实习工资
var intern_salary_type;
var subList = [];
//留守补助
var stay_subsidy_company = 0;
var stay_subsidy_personal = 0;
$(function () {
    $("#form1").queryFormValidate();
    $.ajax({
        url: "/SalaryCalculate/PayrollOffice/GetSettingInfo",
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#form1").formSerializeShow(data.payrollInfo);
            $("#salary").text(ToThousandsStr(data.payrollInfo.salary));
            $("#base_salary").text(ToThousandsStr(data.payrollInfo.base_salary));
            $("#position_salary").text(ToThousandsStr(data.payrollInfo.position_salary));
            $("#house_subsidy").text(ToThousandsStr(data.payrollInfo.house_subsidy));
            $("#traffic_subsidy").text(ToThousandsStr(data.payrollInfo.traffic_subsidy));
            $("#attendance_reward").text(ToThousandsStr(data.payrollInfo.attendance_reward));
            //$("#work_days").text(ToThousandsStr(data.payrollInfo.work_days));
            $("#work_days").val(26);
            $("#onduty_day").text(ToThousandsStr(data.payrollInfo.onduty_day));
            $("#seniority_salary").text(ToThousandsStr(data.payrollInfo.seniority_salary));
            $("#salary_subsidy").text(ToThousandsStr(data.payrollInfo.salary_subsidy));
            $("#kpi").text(ToThousandsStr(data.payrollInfo.kpi));
            $("#kpi_standard").text(ToThousandsStr(data.payrollInfo.kpi_standard));
            $("#kpi_subsidy").text(ToThousandsStr(data.payrollInfo.kpi_subsidy));

            $("#form1").formSerializeShow(data.empInfo);

            //留守补助
            if (!!data.staySubsidy && data.staySubsidy != null) {
                $("#staySubsidyWrap").show();
                stay_subsidy_company = data.staySubsidy.company_amount;
                $("#stay_subsidy_company").text(ToThousandsStr(stay_subsidy_company));
                stay_subsidy_personal = data.staySubsidy.emp_amount;
                $("#stay_subsidy_personal").text(ToThousandsStr(stay_subsidy_personal));
            }
            else
                $("#staySubsidyWrap").hide();

            if (!!data.seKpiSubsidy || data.seKpiSubsidy != null) {
                var kpi_subsidy_note = data.seKpiSubsidy.kpi_subsidy_note;
                if (kpi_subsidy_note == null) {
                    $("#kpi_subsidy_note").text("无");

                } else {

                    $("#kpi_subsidy_note").text(kpi_subsidy_note);
                }
            } else {
                $("#kpi_subsidy_note").text("无");

            }
            //是否为实习生
            intern_salary_type = data.payrollInfo.intern_salary_type;
            emp_category = data.empInfo.emp_category;

            salary = data.payrollInfo.salary;
            base_salary = data.payrollInfo.base_salary;
            position_salary = data.payrollInfo.position_salary;
            house_subsidy = data.payrollInfo.house_subsidy;
            traffic_subsidy = data.payrollInfo.traffic_subsidy;
            attendance_reward = data.payrollInfo.attendance_reward;
            attendance_reward_old = data.payrollInfo.attendance_reward;
            kpi = data.payrollInfo.kpi;
            kpi_standard = data.payrollInfo.kpi_standard;
            kpi_standard_score = data.payrollInfo.kpi_standard_score;
            kpi_subsidy = data.payrollInfo.kpi_subsidy;
            kpi_subsidy_score = data.payrollInfo.kpi_subsidy_score;
            seniority_salary = data.payrollInfo.seniority_salary;
            salary_subsidy = data.payrollInfo.salary_subsidy;
            resign_deposit = data.payrollInfo.resign_deposit;
            insurance_fee = data.payrollInfo.insurance_fee;
            intern_salary = data.payrollInfo.intern_salary;
            month = data.payrollInfo.month;
            $("#year").text(data.seniorityStr);
            if (emp_category == "实习生") {
                if (intern_salary_type == 1) {//比例
                    $("#Trainee_radio").css("display", "");
                    $("#Employee").css("display", "");
                    $("#intern_salary_radio").html(ToThousandsStr(data.payrollInfo.intern_salary));
                } else {
                    $("#Trainee").css("display", "");
                    $("#intern_salary_type").text("按固定金额");
                    $("#intern_salary").text(ToThousandsStr(data.payrollInfo.intern_salary));
                }
                $("#position_name").text(data.empInfo.position_name + "（实习生）");
            } else {
                $("#Employee").css("display", "");
            }
            //  -1全额发放
            if (data.payrollInfo.kpi_subsidy_score == -1) {
                $("#kpi_subsidy_score").text("全额发放");
            } else {
                $("#kpi_subsidy_score").text("按评分发放");
            }
            //补贴小于0 类型不显示
            if (data.payrollInfo.kpi_subsidy == 0 || data.payrollInfo.kpi_subsidy < 0) {
                $("#type").css("display", "none");
            } else {
                $("#type").css("display", "");
            }

            $("#benefit").css("display", "");
            $("#benefitNone").css("display", "none");
            var Div1 = '';
            var Div2 = '';
            var hasBenefit = false;
            var hasReward = false;
            subList = data.payrollSubList;
            $.each(subList, function (index, item) {
                if (item.category == 12) {
                    hasBenefit = true;
                    Div1 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right" name="benefit_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center" onblur="Benefits()" name="benefits">' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag" data-href="/FinancialAccounting/Benefits/Show?id=' + item.category_id + '" data-btndetail="福利详情">查看详情</a></div></div>';
                    benefits += item.amount;
                }
                if (item.category == 11) {
                    hasReward = true;
                    Div2 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="reward_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center" onblur="Rewards()" name="rewards" >' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"  data-href="/FinancialAccounting/Rewards/Show?id=' + item.category_id + '" data-btndetail="奖罚详情">查看详情</a></div></div>';
                    rewards += item.amount;
                }
                BenefitRewardsTotal += item.amount;

            })
            if (hasBenefit == true) {
                $("#benefitList").after(Div1);
            } else {
                $("#benefitList").css("display", "none");
            }
            if (hasReward == true) {
                $("#rewardsList").after(Div2);
            } else {
                $("#rewardsList").css("display", "none");
            }
            if (hasBenefit == false && hasReward == false) {
                $("#benefitNone").css("display", "");
                $("#benefit").css("display", "none");
            }
            getSalary();
            getSum();
            Work();
        },
        error: function (data) {
            if (data.responseText == "noOfficeSalarySetting") {
                $('#modalSalary').modal('show');
                $('#saleDetail').attr('data-href', "/FinancialAccounting/EmployeeSalary/OfficeIndex?id=" + id);
            } else {
                $.modalAlert(data.responseText, "error");
            }
        }
    });
})
$("#modalSalary").on('hide.bs.modal', function (e) {
    window.location.href = "/SalaryCalculate/PayrollOffice/Index";
})

//请假扣款 leaving_deduction
$("#leaving_deduction").blur(function () {
    leaving_deduction = $("#leaving_deduction").val();
    if (leaving_deduction < 0) {
        leaving_deduction = leaving_deduction;
    } else {
        leaving_deduction = 0 - leaving_deduction;
    }
    $("#leaving_deduction").val(leaving_deduction);
    getSum();
})

//风险金 resign_deposit
$("#resign_deposit").blur(function () {
    resign_deposit = $("#resign_deposit").val();
    if (resign_deposit < 0) {
        resign_deposit = resign_deposit;
    } else {
        resign_deposit = 0 - resign_deposit;
    }
    $("#resign_deposit").val(resign_deposit);
    getSum();
})

//社保 insurance_fee
$("#insurance_fee").blur(function () {
    insurance_fee = $("#insurance_fee").val();
    if (insurance_fee < 0) {
        insurance_fee = insurance_fee;
    } else {
        insurance_fee = 0 - insurance_fee;
    }
    $("#insurance_fee").val(insurance_fee);
    getSum();
})

//其他  others
function Others() {
    others = 0;
    var InputList = $("input[name=others]");
    var len = InputList.length;
    for (var i = 0; i < len; i++) {
        others += Number($(InputList[i]).val());
    }
    getSum();
}

//出勤 attendance_reward
function Work() {
    var work_days = $("#work_days").val();
    var onduty_day = $("#onduty_day").val();
    if (work_days - onduty_day > 0.5) {
        $("#attendance_reward").text("0");
        attendance_reward = 0;
    } else {
        $("#attendance_reward").text(ToThousandsStr(attendance_reward_old));
        attendance_reward = attendance_reward_old;
    }
    getSalary();
    getSum();

}
//获取总数
function getSum() {
    actual_salary = salary + benefits + rewards + Number(leaving_deduction) + Number(resign_deposit)
                + Number(insurance_fee) + others + stay_subsidy_company + stay_subsidy_personal;
    actual_salary = actual_salary.toFixed(2);
    $("#actual_salary").val(ToThousandsStr(actual_salary));
}

function getSalary() {
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

//其他
function addRow(e) {
    var Tr = ' <div class="col-sm-12 col-md-12"> <div class="col-xs-12 col-sm-6 col-lg-5"><div class="input-group margin-bottom formValue">'
        + ' <span class="input-group-addon  no-border text-blue">其&nbsp;&nbsp;&nbsp;&nbsp;他&nbsp;&nbsp;&nbsp;</span><input type="text" name="others" value="0" class="form-control  text-blue" onblur="Others()">'
        + ' <span class="input-group-addon text-blue">元</span> </div> </div> <div class="col-sm-6 col-lg-6">  <span class="form-control no-border"> <i class="fa fa-minus text-red" onclick="deleteRow(this)"></i>'
        + '</span> </div></div>';
    $("#other").after(Tr);
}

function deleteRow(e) {
    $(e).parent().parent().parent().remove();
}

function submitForm() {
    // 提交验证
    if (!$("#form1").formValid())
        return false;
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }

    


    data['emp_id'] = id;
    data['start_date'] = $("#start_date").text();
    data['end_date'] = $("#end_date").text();
    data['month'] = month;
    data['salary'] = salary;
    data['base_salary'] = base_salary;
    data['actual_salary'] = actual_salary;
    data['position_salary'] = position_salary;
    data['traffic_subsidy'] = traffic_subsidy;
    //全勤
    if (emp_category != "实习生") {
        if ($("#work_days").val() <= 0) {
            $.modalAlert("请核对应出勤天数！");
            return false;
        } else {
            data['work_days'] = $("#work_days").val();
        }
    }

    data['house_subsidy'] = house_subsidy;
    data['attendance_reward'] = attendance_reward;
    data['kpi'] = kpi;
    data['kpi_standard'] = kpi_standard;
    data['kpi_standard_score'] = kpi_standard_score;
    data['kpi_subsidy'] = kpi_subsidy;
    data['kpi_subsidy_score'] = kpi_subsidy_score;
    data['seniority_salary'] = seniority_salary;
    data['salary_subsidy'] = salary_subsidy;
    data['intern_salary'] = intern_salary;
    data['intern_salary_type'] = intern_salary_type;
    data['stay_subsidy_company'] = stay_subsidy_company;
    data['stay_subsidy_personal'] = stay_subsidy_personal;
    //其他
    var othersInputList = $("input[name='others']");
    var len2 = othersInputList.length;
    for (var i = 0; i < len2; i++) {
        var others = {};
        others.category = 31;
        others.amount = $(othersInputList[i]).val();
        subList.push(others);
    }
    data['subList'] = subList;
    $.submitForm({
        url: "/SalaryCalculate/PayrollOffice/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/SalaryCalculate/PayrollOffice/Show?id=" + data.data;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

