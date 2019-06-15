var id = $.request("id");
var salary = 0;//应发
var base_salary = 0;//基本工资
var position_salary = 0;//岗位工资
var house_subsidy = 0;//住房补贴
var traffic_subsidy = 0;//交通补贴
var attendance_reward = 0;//全勤奖金
var attendance_reward_old = 0;//全勤奖金
var seniority_salary = 0;//工龄工资
var leaving_deduction = 0;//请假扣款
var resign_deposit = 0;//风险金
var insurance_fee = 0;//社保
var actual_salary = 0;//实发
var BenefitRewardsTotal = 0;//福利+奖罚
var kpiTotal = 0;//kpiTotal
var rewards = 0;
var benefits = 0;
var others = 0;
var emp_category;
var intern_salary = 0;//实习工资
var intern_salary_type;
var subList = [];
var main_id = '';
//留守补助
var stay_subsidy_company = 0;
var stay_subsidy_personal = 0;
$(function () {
    $("#form1").queryFormValidate();
    $.ajax({
        url: "/SalaryCalculate/PayrollTrainer/GetSettingInfo",
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
            $("#seniority_salary").text(ToThousandsStr(data.payrollInfo.seniority_salary));
            $("#work_days").val(26);

            $("#form1").formSerializeShow(data.empInfo);
            //是否为实习生
            intern_salary_type = data.payrollInfo.intern_salary_type;
            emp_category = data.empInfo.emp_category;
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

            //salary = data.payrollInfo.salary;
            base_salary = data.payrollInfo.base_salary;
            position_salary = data.payrollInfo.position_salary;
            house_subsidy = data.payrollInfo.house_subsidy;
            traffic_subsidy = data.payrollInfo.traffic_subsidy;
            attendance_reward = data.payrollInfo.attendance_reward;
            attendance_reward_old = data.payrollInfo.attendance_reward;
            seniority_salary = data.payrollInfo.seniority_salary;
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
            $("#benefit").css("display", "");
            $("#benefitNone").css("display", "none");
            var div1 = '';
            var div2 = '';
            var hasBenefit = false;
            var hasReward = false;
            subList = data.payrollSubList;
            //福利奖罚列表
            $.each(subList, function (index, item) {
                if (item.category == 12) {
                    hasBenefit = true;
                    div1 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right" name="benefit_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center" onblur="Benefits()" name="benefits">' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag isShowDetail" data-href="/FinancialAccounting/Benefits/Show?id=' + item.category_id + '" data-btndetail="福利详情">查看详情</a></div></div>';
                }
                if (item.category == 11) {
                    hasReward = true;
                    div2 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="reward_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center" onblur="Rewards()" name="rewards" >' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag isShowDetail"  data-href="/FinancialAccounting/Rewards/Show?id=' + item.category_id + '" data-btndetail="奖罚详情">查看详情</a></div></div>';
                }
                BenefitRewardsTotal += item.amount;

            })
            if (hasBenefit == true) {
                $("#benefitList").after(div1);
            } else {
                $("#benefitList").css("display", "none");
            }
            if (hasReward == true) {
                $("#rewardsList").after(div2);
            } else {
                $("#rewardsList").css("display", "none");
            }
            if (hasBenefit == false && hasReward == false) {
                $("#benefitNone").css("display", "");
                $("#benefit").css("display", "none");
            }
            var div_kpi = '';
            var kpi_type;
            var kpi_result;
            $.each(data.detailKpiList, function (index, item) {
                main_id = item.main_id;
                switch (item.kpi_type) {
                    case 1:
                        kpi_type = "V雪球活跃人数";
                        kpi_result = item.kpi_result;
                        break;
                    case 2:
                        kpi_type = "V雪球转化率";
                        kpi_result = item.kpi_result;
                        break;
                    case 3:
                        kpi_type = "导购人均产值";
                        kpi_result = item.kpi_result;
                        break;
                    case 4:
                        kpi_type = "导购离职率";
                        kpi_result = item.kpi_result;
                        break;
                    case 5:
                        kpi_type = "高端机占比";
                        kpi_result = item.kpi_result;
                        break;
                    case 6:
                        kpi_type = "培训场次";
                        kpi_result = item.kpi_result;
                        break;
                    case 7:
                        kpi_type = "形象执行效率";
                        kpi_result = item.kpi_result;
                        break;
                    case 8:
                        kpi_type = "形象罚款";
                        kpi_result = item.kpi_result;
                        break;
                    case 9:
                        kpi_type = "终端经理打分";
                        kpi_result = item.kpi_result;
                        break;
                    case 51:
                        kpi_type = "导购人均产值";
                        kpi_result = item.kpi_result;
                        break;
                    case 52:
                        kpi_type = "导购离职率";
                        kpi_result = item.kpi_result;
                        break;
                    case 53:
                        kpi_type = "高端机占比";
                        kpi_result = item.kpi_result;
                        break;
                    default:
                        break;
                }
                if (main_id != '') {
                    $('#kpiDetail').attr('data-href', "/FinancialAccounting/trainerKPI/Show?id=" + main_id);
                    $("#showDetail").css("display", "");
                }

                div_kpi += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right" >' + kpi_type + '</span></div> '
                       + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center" >' + ToThousandsStr(kpi_result) + '</span>'
                       + '<span class="input-group-addon">元</span></div> </div>'
                       + '</div>';
                kpiTotal += kpi_result;
            })

            $("#kpi_type").append(div_kpi);
            getSalary();
            getSum();
            Work();
        },
        error: function (data) {
            if (data.responseText == "noKPI") {
                $('#modalMessage').modal('show');
                $('#saleDetail').attr('data-href', "/FinancialAccounting/trainerKPI/Add?id=" + id);
            } else {
                $.modalAlert(data.responseText, "error");
            }
        }
    });
})
$("#modalMessage").on('hide.bs.modal', function (e) {
    window.location.href = "/SalaryCalculate/PayrollTrainer/Index";
})


//请假扣款 leaving_deduction
$("#leaving_deduction").blur(function () {
    leaving_deduction = $("#leaving_deduction").val();
    if (leaving_deduction < 0) {
        leaving_deduction = leaving_deduction;
    } else {
        leaving_deduction = 0 - leaving_deduction;
    }
    $("#leaving_deduction").val(ToThousandsStr(leaving_deduction));
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
    $("#resign_deposit").val(ToThousandsStr(resign_deposit));
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
    $("#insurance_fee").val(ToThousandsStr(insurance_fee));
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
    actual_salary = salary + BenefitRewardsTotal + kpiTotal + Number(leaving_deduction) + Number(resign_deposit)
                + Number(insurance_fee) + others + stay_subsidy_company + stay_subsidy_personal;
    actual_salary = actual_salary.toFixed(2);
    $("#actual_salary").val(ToThousandsStr(actual_salary));
}

function getSalary() {
    salary = base_salary + position_salary + house_subsidy + traffic_subsidy + attendance_reward + seniority_salary;
    if (emp_category == "实习生") {
        if (intern_salary_type == 1) { //比例
            salary = Number(salary) * Number(intern_salary) / 100.00
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
    data['actual_salary'] = actual_salary;
    data['base_salary'] = base_salary;
    data['position_salary'] = position_salary;
    data['traffic_subsidy'] = traffic_subsidy;
    data['house_subsidy'] = house_subsidy;
    data['attendance_reward'] = attendance_reward;
    data['seniority_salary'] = seniority_salary;
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
        url: "/SalaryCalculate/PayrollTrainer/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/SalaryCalculate/PayrollTrainer/Show?id=" + data.data;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

