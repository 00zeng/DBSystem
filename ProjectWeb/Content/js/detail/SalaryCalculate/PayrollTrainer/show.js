var id = $.request("id");
var emp_category;
var intern_salary = 0;//实习工资
var intern_salary_type;
var subList = [];
var main_id = '';
$(function () {
    $.ajax({
        url: "/SalaryCalculate/PayrollTrainer/GetInfo",
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
            $("#benefit").css("display", "");
            $("#benefitNone").css("display", "none");
            var div1 = '';
            var div2 = '';
            var div31 = '';
            var hasBenefit = false;
            var hasReward = false;
            subList = data.subList;
            //福利奖罚列表
            $.each(subList, function (index, item) {
                if (item.category == 12) {
                    hasBenefit = true;
                    div1 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right" name="benefit_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center" onblur="Benefits()" name="benefits">' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"  data-href="/FinancialAccounting/Benefits/Show?id=' + item.category_id + '" data-btndetail="福利详情">查看详情</a></div></div>';
                }
                if (item.category == 11) {
                    hasReward = true;
                    div2 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="reward_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center" onblur="Rewards()" name="rewards" >' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"   data-href="/FinancialAccounting/Rewards/Show?id=' + item.category_id + '" data-btndetail="奖罚详情">查看详情</a></div></div>';
                }
                if (item.category == 31) {//其他
                    div31 += ' <div class="col-md-12"> <div class="col-xs-12 col-sm-6 col-lg-5"><div class="input-group margin-bottom formValue">'
                         + ' <span class="input-group-addon  no-border">其&nbsp;&nbsp;&nbsp;&nbsp;他&nbsp;&nbsp;&nbsp;</span><span name="others" class="form-control" >' + ToThousandsStr(item.amount) + '</span>'
                         + ' <span class="input-group-addon">元</span> </div> </div> </div>';
                }
            })
            $("#other").after(div31);
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
            })
            $("#kpi_type").append(div_kpi);
        },
        error: function (data) {
            $.modalAlert(data.responseText, "error");
        }
    });
})
