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

var leaving_deduction = 0;//请假扣款
var resign_deposit = 0;//风险金
var insurance_fee = 0;//社保
var activityTotal = 0;//活动+奖罚
var others = 0;
var month;
var start_date;
var end_date;
var name;
$(function () {
    $.ajax({
        url: "/SalaryCalculate/PayrollGuide/GetInfo",
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#form1").formSerializeShow(data.mainInfo);
            $("#salary").text(ToThousandsStr(data.mainInfo.salary));
            $("#actual_salary").text(ToThousandsStr(data.mainInfo.actual_salary));
            $("#base_salary").text(ToThousandsStr(data.mainInfo.base_salary));
            $("#commission_basesalary").text(ToThousandsStr(data.mainInfo.commission_basesalary));
            $("#sale_commission").text(ToThousandsStr(data.mainInfo.sale_commission));
            $("#sale_count").text(ToThousandsStr(data.mainInfo.sale_count));
            $("#exclusive_commission").text(ToThousandsStr(data.mainInfo.exclusive_commission));
            $("#exclusive_count").text(ToThousandsStr(data.mainInfo.exclusive_count));
            $("#buyout_commission").text(ToThousandsStr(data.mainInfo.buyout_commission));
            $("#buyout_count").text(ToThousandsStr(data.mainInfo.buyout_count));
            $("#increase_reward").text(ToThousandsStr(data.mainInfo.increase_reward));
            $("#increase_count").text(ToThousandsStr(data.mainInfo.increase_count));
            $("#increase_commission").text(ToThousandsStr(data.mainInfo.increase_commission));
            $("#increase_sale_count").text(ToThousandsStr(data.mainInfo.increase_sale_count));
            $("#attendance_reward").text(ToThousandsStr(data.mainInfo.attendance_reward));
            $("#leaving_deduction").text(ToThousandsStr(data.mainInfo.leaving_deduction));
            $("#resign_deposit").text(ToThousandsStr(data.mainInfo.resign_deposit));
            $("#insurance_fee").text(ToThousandsStr(data.mainInfo.insurance_fee));
            if (data.mainInfo.stay_subsidy_company == 0 && data.mainInfo.stay_subsidy_personal == 0)
                $("#staySubsidyWrap").hide();
            else
            {
                $("#staySubsidyWrap").show();
                $("#stay_subsidy_company").text(ToThousandsStr(data.mainInfo.stay_subsidy_company));
                $("#stay_subsidy_personal").text(ToThousandsStr(data.mainInfo.stay_subsidy_personal));
            }
            $("#form1").formSerializeShow(data.empInfo);
            start_date = data.mainInfo.start_date;
            end_date = data.mainInfo.end_date;
            name = data.empInfo.name;
            $('#saleDetail').attr('data-href', "/SaleManage/Summary/GuideIndex?emp_id=" + data.mainInfo.emp_id + "&start_date=" + start_date + "&end_date=" + end_date + "&name=" + encodeURI(encodeURI(name)));

            if (data.increase == 1) {//开启
                $("#increase").css("display", "");
            } else {
                $("#increase").css("display", "none");
            }
            //是否为实习生
            var emp_category = data.empInfo.emp_category;
            if (emp_category == "实习生") {
                $("#position_name").text(data.empInfo.position_name + "(实习生)")
            }
            base_type = data.mainInfo.base_type;
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

            salary = data.mainInfo.salary;
            //基本
            base_salary = data.mainInfo.base_salary;
            sale_commission = data.mainInfo.sale_commission;
            sale_count = data.mainInfo.sale_count;
            exclusive_commission = data.mainInfo.exclusive_commission;
            exclusive_count = data.mainInfo.exclusive_count;//当月包销
            buyout_commission = data.mainInfo.buyout_commission;
            buyout_count = data.mainInfo.buyout_count;

            increase_reward = data.mainInfo.increase_reward;
            increase_count = data.mainInfo.increase_count;
            increase_commission = data.mainInfo.increase_commission;
            increase_sale_count = data.mainInfo.increase_sale_count;

            leaving_deduction = data.mainInfo.leaving_deduction;
            resign_deposit = data.mainInfo.resign_deposit;
            insurance_fee = data.mainInfo.insurance_fee;
            month = data.mainInfo.month;

            $("#year").text(data.seniorityStr);
            // 活动为空
            if (data.subList.length < 1) {
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
                var Div6 = '';
                var hasRecom = false;
                var hasAtta = false;
                var hasPK = false;
                var hasRank = false;
                var hasBenefit = false;
                var hasReward = false;
                $.each(data.subList, function (index, item) {
                    if (item.category == 1) {//主推
                        hasRecom = true;
                        Div1 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right" name="recom_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                             + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center" name="recom">' + ToThousandsStr(item.amount) + '</span>'
                             + '<span class="input-group-addon">元</span></div> </div>'
                             + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag" data-href="/ActivityManage/Recommendation/ShowActivity?id=' + item.category_id + '" data-btndetail="主推详情">查看详情</a></div></div>';
                    }
                    if (item.category == 2) {//达量
                        hasAtta = true;
                        Div2 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="atta_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                             + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center"  name="atta" >' + ToThousandsStr(item.amount) + '</span>'
                             + '<span class="input-group-addon">元</span></div> </div>'
                             + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"  data-href="/ActivityManage/Attaining/ShowActivity?id=' + item.category_id + '" data-btndetail="达量详情">查看详情</a></div></div>';
                    }
                    if (item.category == 3) {//排名
                        hasRank = true;
                        Div3 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="rank_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                             + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center"  name="rank" >' + ToThousandsStr(item.amount) + '</span>'
                             + '<span class="input-group-addon">元</span></div> </div>'
                             + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"  data-href="/ActivityManage/Ranking/ShowActivity?id=' + item.category_id + '"  data-btndetail="排名详情">查看详情</a></div></div>';
                    }
                    if (item.category == 4) {//PK
                        hasPK = true;
                        Div41 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="pk_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                             + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center"  name="pk" >' + ToThousandsStr(item.amount) + '</span>'
                             + '<span class="input-group-addon">元</span></div> </div>'
                             + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"  data-href="/ActivityManage/PK/ShowActivity?id=' + item.category_id + '"  data-btndetail="PK详情">查看详情</a></div></div>';
                    }
                    if (item.category == 12) {//福利
                        hasBenefit = true;
                        Div4 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="benefits_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                             + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center" name="benefits" >' + ToThousandsStr(item.amount) + '</span>'
                             + '<span class="input-group-addon">元</span></div> </div>'
                             + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"  data-href="/FinancialAccounting/Benefits/Show?id=' + item.category_id + '" data-btndetail="福利详情">查看详情</a></div></div>';
                    }
                    if (item.category == 11) {//奖罚
                        hasReward = true;
                        Div5 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="reward_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                             + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center"  name="rewards" >' + ToThousandsStr(item.amount) + '</span>'
                             + '<span class="input-group-addon">元</span></div> </div>'
                             + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"  data-href="/FinancialAccounting/Rewards/Show?id=' + item.category_id + '" data-btndetail="奖罚详情">查看详情</a></div></div>';
                    }
                    if (item.category == 31) {
                        Div6 += ' <div class="col-md-12"> <div class="col-xs-12 col-sm-6 col-lg-5"><div class="input-group margin-bottom formValue">'
                             + ' <span class="input-group-addon  no-border">其&nbsp;&nbsp;&nbsp;&nbsp;他&nbsp;&nbsp;&nbsp;</span><span name="others" class="form-control" >' + ToThousandsStr(item.amount) + '</span>'
                             + ' <span class="input-group-addon">元</span> </div> </div> </div>';
                    }
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
                } else {
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
                $("#other").after(Div6);
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, "error");
        }
    });
})

//其他
function addRow(e) {
    var Tr = ' <div class="col-md-12"> <div class="col-xs-12 col-sm-6 col-lg-5"><div class="input-group margin-bottom formValue">'
        + ' <span class="input-group-addon  no-border text-blue">其&nbsp;&nbsp;&nbsp;&nbsp;他&nbsp;&nbsp;&nbsp;</span><input type="text" name="others" value="0" class="form-control  text-blue" onblur="Others()">'
        + ' <span class="input-group-addon text-blue">元</span> </div> </div> <div class="col-lg-6">  <span class="form-control no-border"> <i class="fa fa-minus text-red" onclick="deleteRow(this)"></i>'
        + '</span> </div></div>';
    $("#other").after(Tr);
}


