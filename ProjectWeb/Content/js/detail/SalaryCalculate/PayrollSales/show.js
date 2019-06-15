var id = $.request("id");
var month_sales = 0;//前端定义 月度考核
var emp_category;
var intern_salary = 0;//实习工资
var intern_salary_type;
var salesKPI = [];
$(function () {
    $.ajax({
        url: "/SalaryCalculate/PayrollSales/GetInfo",
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            salesKPI = data.salesKPI;
            if (data.salesKPI != null) {
                if (salesKPI.id != null || salesKPI.id != '') {
                    $('#saleDetail').attr('data-href', "/FinancialAccounting/SalesKPI/Show?id=" + salesKPI.id);
                } else {
                    $.modalAlert("ID为空！");
                }
                $("#form1").formSerializeShow(salesKPI);
                $("#normal_count").text(ToThousandsStr(salesKPI.normal_count));
                $("#buyout_count").text(ToThousandsStr(salesKPI.buyout_count));
                $("#total_count").text(ToThousandsStr(salesKPI.total_count));
                if (salesKPI.kpi_total_a == 0) {
                    $("#kpi_total").text(ToThousandsStr(salesKPI.kpi_total));
                } else {
                    $("#kpi_total").text(ToThousandsStr(salesKPI.kpi_total_a));
                }
            } else {
                $("#salesExam").css("display", "none");
            }
               
        
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
            $("#form1").formSerializeShow(data.empInfo);
            if (data.mainInfo.stay_subsidy_company == 0 && data.mainInfo.stay_subsidy_personal == 0)
                $("#staySubsidyWrap").hide();
            else {
                $("#staySubsidyWrap").show();
                $("#stay_subsidy_company").text(ToThousandsStr(data.mainInfo.stay_subsidy_company));
                $("#stay_subsidy_personal").text(ToThousandsStr(data.mainInfo.stay_subsidy_personal));
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
                if (data.salesKPI != null) {
                    $("#salesExam").css("display", "");
                }
                $("#Employee").css("display", "");
            }

            $("#benefit").css("display", "");
            $("#benefitNone").css("display", "none");
            var Div1 = '';
            var Div2 = '';
            var Div3 = '';
            var Div41 = '';
            var Div5 = '';
            var Div11 = '';
            var Div12 = '';
            var Div31 = '';
            var hasRecom = false;
            var hasAtta = false;
            var hasPK = false;
            var hasRank = false;
            var hasBenefit = false;
            var hasReward = false;
            var hasSales = false;
            $.each(data.subList, function (index, item) {
                if (item.category == 1) {//主推
                    hasRecom = true;
                    Div1 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right" name="recom_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center" name="recom">' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"  data-href="/ActivityManage/Recommendation/ShowActivity?id=' + item.category_id + '"  data-btndetail="主推详情">查看详情</a></div></div>';
                }
                if (item.category == 2) {//达量
                    hasAtta = true;
                    Div2 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="atta_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center"  name="atta" >' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"  data-href="/ActivityManage/Attaining/ShowActivity?id=' + item.category_id + '"  data-btndetail="达量详情">查看详情</a></div></div>';
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
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"   data-href="/ActivityManage/PK/ShowActivity?id=' + item.category_id + '"  data-btndetail="PK详情">查看详情</a></div></div>';
                }
                if (item.category == 5) {//业务考核
                    hasSales = true;
                    Div5 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center"  name="" >' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"  data-href="/ActivityManage/SalesPerformance/SalePerfShowActivity?id=' + item.category_id + '"  data-btndetail="业务考核详情">查看详情</a></div></div>';
                }
                if (item.category == 11) {//奖罚
                    hasReward = true;
                    Div11 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right"  name="reward_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center"  name="rewards" >' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"   data-href="/FinancialAccounting/Rewards/Show?id=' + item.category_id + '"  data-btndetail="奖罚详情">查看详情</a></div></div>';
                }
                if (item.category == 12) {//福利
                    hasBenefit = true;
                    Div12 += '<div class="row margin-bottom"><div class="col-sm-3"><span class="form-control no-border text-right" name="benefit_name" style="font-size:10px">' + item.sub_name + '</span></div> '
                         + '<div class="col-sm-4">  <div class="input-group"> <span type="text" class="form-control text-center"  name="benefits">' + ToThousandsStr(item.amount) + '</span>'
                         + '<span class="input-group-addon">元</span></div> </div>'
                         + '<div class="col-sm-2"> <a class="label label-primary btnDetailTag"  data-href="/FinancialAccounting/Benefits/Show?id=' + item.category_id + '"  data-btndetail="福利详情">查看详情</a></div></div>';
                }
                if (item.category == 31) {//其他
                    Div31 += ' <div class="col-md-12"> <div class="col-xs-12 col-sm-6 col-lg-5"><div class="input-group margin-bottom formValue">'
                         + ' <span class="input-group-addon  no-border">其&nbsp;&nbsp;&nbsp;&nbsp;他&nbsp;&nbsp;&nbsp;</span><span name="others" class="form-control" >' + ToThousandsStr(item.amount) + '</span>'
                         + ' <span class="input-group-addon">元</span> </div> </div> </div>';
                }
            })
            $("#other").after(Div31);
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
            if (hasSales == true) {
                $("#saleList").after(Div5);
            } else {
                $("#saleList").css("display", "none");
            }
            if (hasReward == true) {
                $("#rewardsList").after(Div11);
            } else {
                $("#rewardsList").css("display", "none");
            }
            if (hasBenefit == true) {
                $("#benefitList").after(Div12);
            } else {
                $("#benefitList").css("display", "none");
            }
            if (hasBenefit == false && hasReward == false) {
                $("#benefitNone").css("display", "");
                $("#benefit").css("display", "none");
            } else {
                $("#benefit").css("display", "");

            }
            if (hasRecom == false && hasAtta == false && hasRank == false && hasPK == false && hasSales == false) {
                $("#activityNone").css("display", "");
                $("#activity").css("display", "none");
            } else {
                $("#activity").css("display", "");
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, "error");
        }
    });
})

