var id = $.request("id");

var salary = 0;//应发
var actual_salary = 0;//实发

var base_salary = 0;//底薪
var base_type = 0;
var sale_commission = 0;//实销提成
var sale_count = 0;//当月实销

var exclusive_commission = 0;//包销提成
var exclusive_count = 0;//当月包销

var buyout_commission = 0;//买断提成
var buyout_count = 0;//当月买断

var increase_reward = 0;//增员奖励
var increase_count = 0;
var increase_commission = 0;
var increase_sale_count = 0;

var attendance_reward = 0;//全勤奖金
var leaving_deduction = 0;//请假扣款
var resign_deposit = 0;//风险金
var insurance_fee = 0;//社保
var activityTotal = 0;//活动+奖罚
var others = 0;
var month;
var subList = [];
var start_date;
var end_date;
var name;
//留守补助
var stay_subsidy_company = 0;
var stay_subsidy_personal = 0;
$(function () {
    $("#form1").queryFormValidate();
    $.ajax({
        url: "/SalaryCalculate/PayrollGuide/GetSettingInfo",
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#form1").formSerializeShow(data.payrollInfo);
            $("#salary").text(ToThousandsStr(data.payrollInfo.salary));
            $("#base_salary").text(ToThousandsStr(data.payrollInfo.base_salary));
            $("#commission_basesalary").text(ToThousandsStr(data.payrollInfo.commission_basesalary));
            $("#sale_commission").text(ToThousandsStr(data.payrollInfo.sale_commission));
            $("#sale_count").text(ToThousandsStr(data.payrollInfo.sale_count));
            $("#exclusive_commission").text(ToThousandsStr(data.payrollInfo.exclusive_commission));
            $("#exclusive_count").text(ToThousandsStr(data.payrollInfo.exclusive_count));
            $("#buyout_commission").text(ToThousandsStr(data.payrollInfo.buyout_commission));
            $("#buyout_count").text(ToThousandsStr(data.payrollInfo.buyout_count));
            $("#increase_reward").text(ToThousandsStr(data.payrollInfo.increase_reward));
            $("#increase_count").text(ToThousandsStr(data.payrollInfo.increase_count));
            $("#increase_commission").text(ToThousandsStr(data.payrollInfo.increase_commission));
            $("#increase_sale_count").text(ToThousandsStr(data.payrollInfo.increase_sale_count));
            $("#attendance_reward").text(ToThousandsStr(data.payrollInfo.attendance_reward));
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

            $("#form1").formSerializeShow(data.empInfo);
            start_date = data.payrollInfo.start_date;
            end_date = data.payrollInfo.end_date;
            name = data.empInfo.name;
            $('#saleDetail').attr('data-href', "/SaleManage/Summary/GuideIndex?emp_id=" + data.empInfo.id + "&start_date=" + start_date + "&end_date=" + end_date + "&name=" + encodeURI(encodeURI(name)));

            if (data.increase == 1) {//开启
                $("#increase").css("display", "");
            } else {
                $("#increase").css("display", "none");
            }
            //是否为实习生
            var emp_category = data.empInfo.emp_category;
            if (emp_category == "实习生") {
                $("#position_name").text(data.empInfo.position_name+"(实习生)")
            }
            base_type = data.payrollInfo.base_type;
            if (base_type == 4) {
                $("#base_type").text("保底工资");
            } else if (base_type == 1) {
                $("#base_type").text("达标底薪");
            } else if (base_type == 2) {
                $("#base_type").text("星级制");
            } else if (base_type == 3) {
                $("#base_type").text("浮动底薪");
            } else if (base_type == 0) {
                $("#base_type").text("0底薪");
            }

            //salary = data.payrollInfo.salary;
            //基本
            base_salary = data.payrollInfo.base_salary;
            sale_commission = data.payrollInfo.sale_commission;
            sale_count = data.payrollInfo.sale_count;
            exclusive_commission = data.payrollInfo.exclusive_commission;
            exclusive_count = data.payrollInfo.exclusive_count;//当月包销
            buyout_commission = data.payrollInfo.buyout_commission;
            buyout_count = data.payrollInfo.buyout_count;
            //全勤
            attendance_reward = data.payrollInfo.attendance_reward;

            increase_reward = data.payrollInfo.increase_reward;
            increase_count = data.payrollInfo.increase_count;
            increase_commission = data.payrollInfo.increase_commission;
            increase_sale_count = data.payrollInfo.increase_sale_count;

            leaving_deduction = data.payrollInfo.leaving_deduction;
            resign_deposit = data.payrollInfo.resign_deposit;
            insurance_fee = data.payrollInfo.insurance_fee;
            month = data.payrollInfo.month;

            $("#year").text(data.seniorityStr);
            // 活动为空
            if (data.payrollSubList.length < 1) {
                $("#activity").css("display", "none");
                $("#benefit").css("display", "none");
                $("#activityNone").css("display", "");
                $("#benefitNone").css("display", "");
            } else {//有数据
                $("#activity").css("display", "");
                $("#benefit").css("display", "");
                $("#activityNone").css("display", "none");
                $("#benefitNone").css("display", "none");

                var Div1 = '';
                var Div2 = '';
                var Div3 = '';
                var Div4 = '';
                var Div41 = '';
                var Div5 = '';
                var hasRecom = false;
                var hasPK = false;
                var hasAtta = false;
                var hasRank = false;
                var hasBenefit = false;
                var hasReward = false;
                subList = data.payrollSubList;
                $.each(subList, function (index, item) {
                    if (item.category == 1) {//主推
                        hasRecom = true;
                        Div1 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right" name="recom_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                             + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center" name="recom">' + ToThousandsStr(item.amount) + '</span>'
                             + '<span class="input-group-addon">元</span></div> </div>'
                             + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag " data-href="/ActivityManage/Recommendation/ShowActivity?id=' + item.category_id + '"   data-btndetail="主推详情">查看详情</a></div></div>';
                    }
                    if (item.category == 2) {//达量
                        hasAtta = true;
                        Div2 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="atta_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                             + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center"  name="atta" >' + ToThousandsStr(item.amount) + '</span>'
                             + '<span class="input-group-addon">元</span></div> </div>'
                             + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag "  data-href="/ActivityManage/Attaining/ShowActivity?id=' + item.category_id + '"  data-btndetail="达量详情">查看详情</a></div></div>';
                    }
                    if (item.category == 3) {//排名
                        hasRank = true;
                        Div3 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="rank_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                             + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center"  name="rank" >' + ToThousandsStr(item.amount) + '</span>'
                             + '<span class="input-group-addon">元</span></div> </div>'
                             + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag "  data-href="/ActivityManage/Ranking/ShowActivity?id=' + item.category_id + '"  data-btndetail="排名详情">查看详情</a></div></div>';
                    }
                    if (item.category == 4) {//PK
                        hasPK = true;
                        Div41 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="pk_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                             + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center"  name="pk" >' + ToThousandsStr(item.amount) + '</span>'
                             + '<span class="input-group-addon">元</span></div> </div>'
                             + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag "  data-href="/ActivityManage/PK/ShowActivity?id=' + item.category_id + '"  data-btndetail="PK详情">查看详情</a></div></div>';
                    }
                    if (item.category == 12) {//福利
                        hasBenefit = true;
                        Div4 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="benefits_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                             + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center" name="benefits" >' + ToThousandsStr(item.amount) + '</span>'
                             + '<span class="input-group-addon">元</span></div> </div>'
                             + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag "  data-href="/FinancialAccounting/Benefits/Show?id=' + item.category_id + '"  data-btndetail="福利详情">查看详情</a></div></div>';
                    }
                    if (item.category == 11) {//奖罚
                        hasReward = true;
                        Div5 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="reward_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                             + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center"  name="rewards" >' + ToThousandsStr(item.amount) + '</span>'
                             + '<span class="input-group-addon">元</span></div> </div>'
                             + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag "  data-href="/FinancialAccounting/Rewards/Show?id=' + item.category_id + '"  data-btndetail="奖罚详情">查看详情</a></div></div>';
                    }
                    activityTotal += item.amount;
                })
                
                if (hasRecom == true) {
                    $("#recomList").after(Div1);
                } else {
                    $("#recomList").css("display", "none");
                }

                if (hasAtta == true) {
                    $("#attaList").after(Div2);
                } else {
                    $("#attaList").css("display", "none");
                }

                if (hasRank == true) {
                    $("#rankList").after(Div3);
                } else {
                    $("#rankList").css("display", "none");
                }

                if (hasPK == true) {
                    $("#pkList").after(Div41);
                } else {
                    $("#pkList").css("display", "none");
                }


                if (hasBenefit == true) {
                    $("#benefitList").after(Div4);
                }else{
                    $("#benefitList").css("display", "none");
                }

                if (hasReward == true) {
                    $("#rewardsList").after(Div5);
                } else {
                    $("#rewardsList").css("display", "none");
                }
                if (hasBenefit == false && hasReward == false) {
                    $("#benefitNone").css("display", "");
                    $("#benefit").css("display", "none");
                }
                if (hasRecom == false && hasAtta == false && hasRank == false && hasPK == false) {
                    $("#activityNone").css("display", "");
                    $("#activity").css("display", "none");
                }
            }
            getSalary();
            getSum();
        },
        error: function (data) {
            if (data.responseText == "noGuidesSalarySetting") {
                $('#modalSalary').modal('show');
                $('#salaryDetail').attr('data-href', "/FinancialAccounting/EmployeeSalary/GuideSetting?id=" + id);
            } else {
                $.modalAlert(data.responseText, "error");
            }
        }
    });
})
$("#modalSalary").on('hide.bs.modal', function (e) {
    window.location.href = "/SalaryCalculate/PayrollGuide/Index";
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
    actual_salary = salary + activityTotal + Number(leaving_deduction) + Number(resign_deposit)
                + Number(insurance_fee) + others + stay_subsidy_company + stay_subsidy_personal;
    actual_salary = actual_salary.toFixed(2);
    $("#actual_salary").val(ToThousandsStr(actual_salary));
}
//应发
function getSalary() {
    salary = base_salary + sale_commission + exclusive_commission + buyout_commission + increase_reward + attendance_reward;
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
    data['base_type'] = base_type;
    data['sale_commission'] = sale_commission;
    data['sale_count'] = sale_count;
    data['exclusive_commission'] = exclusive_commission;
    data['exclusive_count'] = exclusive_count;
    data['buyout_commission'] = buyout_commission;
    data['buyout_count'] = buyout_count;
    data['increase_reward'] = increase_reward;
    data['increase_count'] = increase_count;
    data['increase_commission'] = increase_commission;
    data['increase_sale_count'] = increase_sale_count;
    data['commission_basesalary'] = $("#commission_basesalary").text();
    data['stay_subsidy_company'] = stay_subsidy_company;
    data['stay_subsidy_personal'] = stay_subsidy_personal;
    
    data['attendance_reward'] = attendance_reward;
    //其他
    var othersInputList = $("input[name='others']");
    var len6 = othersInputList.length;
    for (var i = 0; i < len6; i++) {
        var others = {};
        others.category = 31;
        others.amount = $(othersInputList[i]).val();
        subList.push(others);
    }
    data['subList'] = subList;
    $.submitForm({
        url: "/SalaryCalculate/PayrollGuide/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/SalaryCalculate/PayrollGuide/Show?id=" + data.data;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

