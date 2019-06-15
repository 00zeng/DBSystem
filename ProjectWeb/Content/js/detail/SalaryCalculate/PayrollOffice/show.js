var id = $.request("id");
var salary = 0;//应发
var base_salary = 0;//基本工资
var position_salary = 0;//岗位工资
var house_subsidy = 0;//住房补贴
var attendance_reward = 0;//全勤奖金
var attendance_reward_old = 0;//全勤奖金
var kpi = 0;//KPI工资
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
$(function () {
    $.ajax({
        url: "/SalaryCalculate/PayrollOffice/GetInfo",
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#form1").formSerializeShow(data.mainInfo);
            $("#salary").text(ToThousandsStr(data.mainInfo.salary));
            $("#actual_salary").text(ToThousandsStr(data.mainInfo.actual_salary));
            $("#base_salary").text(ToThousandsStr(data.mainInfo.base_salary));
            $("#position_salary").text(ToThousandsStr(data.mainInfo.position_salary));
            $("#house_subsidy").text(ToThousandsStr(data.mainInfo.house_subsidy));
            $("#traffic_subsidy").text(ToThousandsStr(data.mainInfo.traffic_subsidy));
            $("#attendance_reward").text(ToThousandsStr(data.mainInfo.attendance_reward));
            $("#work_days").text(ToThousandsStr(data.mainInfo.work_days));
            $("#onduty_day").text(ToThousandsStr(data.mainInfo.onduty_day));
            $("#seniority_salary").text(ToThousandsStr(data.mainInfo.seniority_salary));
            $("#leaving_deduction").text(ToThousandsStr(data.mainInfo.leaving_deduction));
            $("#resign_deposit").text(ToThousandsStr(data.mainInfo.resign_deposit));
            $("#insurance_fee").text(ToThousandsStr(data.mainInfo.insurance_fee));
            $("#salary_subsidy").text(ToThousandsStr(data.mainInfo.salary_subsidy));
            $("#kpi_subsidy").text(ToThousandsStr(data.mainInfo.kpi_subsidy));
            $("#kpi_standard_score").text(ToThousandsStr(data.mainInfo.kpi_standard_score));
            $("#kpi_standard").text(ToThousandsStr(data.mainInfo.kpi_standard));
            $("#kpi").text(ToThousandsStr(data.mainInfo.kpi));
            if (data.mainInfo.stay_subsidy_company == 0 && data.mainInfo.stay_subsidy_personal == 0)
                $("#staySubsidyWrap").hide();
            else {
                $("#staySubsidyWrap").show();
                $("#stay_subsidy_company").text(ToThousandsStr(data.mainInfo.stay_subsidy_company));
                $("#stay_subsidy_personal").text(ToThousandsStr(data.mainInfo.stay_subsidy_personal));
            }
            $("#form1").formSerializeShow(data.empInfo);
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
            intern_salary_type = data.mainInfo.intern_salary_type;
            emp_category = data.empInfo.emp_category;
            if (emp_category == "实习生") {
                if (intern_salary_type == 1) {//比例
                    $("#Trainee_radio").css("display", "");
                    $("#Employee").css("display", "");
                    $("#intern_salary_radio").html(ToThousandsStr(data.mainInfo.intern_salary));
                } else {
                    $("#Trainee").css("display", "");
                    $("#intern_salary").text(ToThousandsStr(data.mainInfo.intern_salary));
                }
                $("#position_name").text(data.empInfo.position_name + "（实习生）");
            } else {
                $("#Employee").css("display", "");
            }
            //  -1全额发放
            if (data.mainInfo.kpi_subsidy_score == -1) {
                $("#kpi_subsidy_score").text("全额发放");
            } else {
                $("#kpi_subsidy_score").text("按评分发放");
            }
            //补贴小于0 类型不显示
            if (data.mainInfo.kpi_subsidy == 0 || data.mainInfo.kpi_subsidy < 0) {
                $("#type").css("display", "none");
            } else {
                $("#type").css("display", "");
            }
            $("#benefit").css("display", "");
            $("#benefitNone").css("display", "none");
            var Div1 = '';
            var Div2 = '';
            var Div3 = '';
            var hasBenefit = false;
            var hasReward = false;
            $.each(data.subList, function (index, item) {
                if (item.category == 12) {
                    hasBenefit = true;
                    Div1 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span  class="form-control text-center" name="benefits"> ' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag" data-href="/FinancialAccounting/Benefits/Show?id=' + item.category_id + '"  data-btndetail="福利详情">查看详情</a></div></div>';//href="/FinancialAccounting/Benefits/Show?id="' + item.id + '"
                }
                if (item.category == 11) {
                    hasReward = true;
                    Div2 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span class="form-control text-center" >' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"  data-href="/FinancialAccounting/Rewards/Show?id=' + item.category_id + '"  data-btndetail="奖罚详情">查看详情</a></div></div>';
                }
                if (item.category == 31) {
                    Div3 += ' <div class="col-md-12"> <div class="col-xs-12 col-sm-6 col-lg-5"><div class="input-group margin-bottom formValue">'
                         + ' <span class="input-group-addon  no-border">其&nbsp;&nbsp;&nbsp;&nbsp;他&nbsp;&nbsp;&nbsp;</span><span name="others" class="form-control" >' + ToThousandsStr(item.amount) + '</span>'
                         + ' <span class="input-group-addon">元</span> </div> </div> </div>';
                }
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
            $("#other").after(Div3);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

