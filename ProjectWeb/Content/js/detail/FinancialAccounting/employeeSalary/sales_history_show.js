var id = $.request("id");
//返利政策
var str1 = "<tr><td><div class='input-group'><input  class='form-control text-center' value='";
var str2 = "'disabled><span class='input-group-addon no-border'>-</span><input  class='form-control text-center'value='";
var str3 = "";
var str4 = "";
var str6 = "";
var str8 = "";
$(function () {
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetSalaryInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.salaryInfo);
            $("#form1").formSerializeShow(data.jobInfo);
            $("#form1").formSerializeShow(data.salesInfo);
            $("#resign_deposit").text(ToThousandsStr(data.salesInfo.resign_deposit));
            $("#traffic_subsidy").text(ToThousandsStr(data.salesInfo.traffic_subsidy));

            //if (data.getinfo.effect_date == '' && data.getinfo.effect_now == true) {
            //    $("#effect_date").text("立即生效");
            //}
            var intern_salary_type = data.salesInfo.intern_salary_type;
            var emp_category = data.jobInfo.emp_category;
            if (emp_category == "实习生") {
                $("#position_name").text(data.jobInfo.position_name + "(" + emp_category + ")");
                $("#EmpType2").css("display", "");
                if (intern_salary_type == 1) {
                    $("#intern_salary_type").html("按总工资比例");
                    $("#salary").html(ToThousandsStr(data.salesInfo.intern_ratio_salary));
                    $("#salary_type").html("&nbsp;&nbsp;% &nbsp;&nbsp;");
                } else {
                    $("#intern_salary_type").html("按固定金额");
                    $("#salary").html(ToThousandsStr(data.salesInfo.intern_fix_salary));
                    $("#salary_type").html("元/月");
                }
            } else {
                $("#EmpType").css("display", "");
            }


            var checkNormalVal = data.salesInfo.normal_rebate_mode; //正常机
            var checkBuyoutVal = data.salesInfo.buyout_rebate_mode; //买断机
            var checkTargetVal = data.salesInfo.target_mode;//返利模式
            //返利政策 改变th


            if (checkTargetVal == 1) {
                $("#forTargetMode1").show();
                $("span[name='target_mode_change']").html("完成率（%）");

            } else if (checkTargetVal == 2) {
                // 按台数
                $("span[name='target_mode_change']").html("销量（台）");
            } else if (checkTargetVal == 3) {
                // 按零售价
                $("span[name='target_mode_change']").html("零售价（元/台）");
            } else if (checkTargetVal == 5) {
                // 按批发价
                $("span[name='target_mode_change']").html("批发价（元/台）");
            }

            //正常机 改变th
            if (checkNormalVal == 1) {
                $("span[name='rebate_mode_change']").html("（元/台）");
                str4 = "";
                str3 = "";
            } else if (checkNormalVal == 2) {
                $("span[name='rebate_mode_change']").html("（元/台）");
                str4 = "*批发价";
                str3 = "%";
            } else if (checkNormalVal == 3) {
                $("span[name='rebate_mode_change']").html("（元/台）");
                str4 = "*零售价 ";
                str3 = "%";
            } else if (checkNormalVal == 4) {
                $("span[name='rebate_mode_change']").html("（元/月）");
                str4 = "";
                str3 = "";
            }
            $("span[name='rebate_mode_input1']").html(str3);
            $("span[name='rebate_mode_input2']").html(str4);


            //买断 改变th
            if (checkBuyoutVal == 1) {
                $("span[name='rebate_mode_change2']").html("（元/台）");
                str8 = "";
                str6 = "";
            } else if (checkBuyoutVal == 2) {
                $("span[name='rebate_mode_change2']").html("（元/台）");
                str8 = "*批发价";
                str6 = "%";
            } else if (checkBuyoutVal == 3) {
                $("span[name='rebate_mode_change2']").html("（元/台）");
                str8 = "*零售价 ";
                str6 = "%";
            } else if (checkBuyoutVal == 4) {
                $("span[name='rebate_mode_change2']").html("（元/月）");
                str8 = "";
                str6 = "";
            }
            //$("span[name='rebate_mode_input3']").html(str6);
            //$("span[name='rebate_mode_input4']").html(str8);

            var tr;
            $.each(data.subList, function (index, item) {
                if (item.target_max == -1) {
                    tr = str1 + ToThousandsStr(item.target_min) + str2 + '以上' + "' disabled></div></td>"
                         + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;'><span>" + item.sale_rebate + "</span><span>" + str3 + "</span>&nbsp;&nbsp;<span>" + str4 + "</span></td>"
                         + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;'><span>" + item.buyout_rebate + "</span><span>" + str6 + "</span>&nbsp;&nbsp;<span>" + str8 + "</span></td>";
                } else {
                    tr = str1 + ToThousandsStr(item.target_min) + str2 + ToThousandsStr(item.target_max) + "' disabled></div></td>"
                        + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;'><span>" + item.sale_rebate + "</span><span>" + str3 + "</span>&nbsp;&nbsp;<span>" + str4 + "</span></td>"
                         + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;'><span>" + item.buyout_rebate + "</span><span>" + str6 + "</span>&nbsp;&nbsp;<span>" + str8 + "</span></td>";
                }
                $("#rebateMode1").append(tr);
            })

            //查看审批
            var tr = '';
            $.each(data.approveList, function (index, item) {
                if (item.approve_note == null || item.approve_note == '') {
                    approve_note_null = "--";
                } else
                    approve_note_null = item.approve_note;
                if (item.status == 1 || item.status == -1)
                    approve_grade = ("一级审批");
                else if (item.status == 2 || item.status == -2)
                    approve_grade = ("二级审批");
                else if (item.status == 3 || item.status == -3)
                    approve_grade = ("三级审批");
                else if (item.status == 4 || item.status == -4)
                    approve_grade = ("四级审批");
                else if (item.status == 100 || item.status == -100)
                    approve_grade = ("终审");
                if (item.status > 0) {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                } else {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});