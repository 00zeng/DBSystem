var id = $.request("id");

$(function () {
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetInfo",
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.payrollInfo);
            $("#base_salary").text(ToThousandsStr(data.payrollInfo.base_salary));
            $("#house_subsidy").text(ToThousandsStr(data.payrollInfo.house_subsidy));
            $("#attendance_reward").text(ToThousandsStr(data.payrollInfo.attendance_reward));
            $("#seniority_salary").text(ToThousandsStr(data.payrollInfo.seniority_salary));
            $("#resign_deposit").text(ToThousandsStr(data.payrollInfo.resign_deposit));

            $("#form1").formSerializeShow(data.jobInfo);
            $("#form1").formSerializeShow(data.salaryInfo);
            $("#form1").formSerializeShow(data.trainerInfo);
            if (data.jobInfo.position_type == 12) {
                $("#trainer").css('display', '');
            } else {
                $("#manager").css('display', '');
            }
            var j = 8;
            if (!data.trainerSubList || data.trainerSubList.length < 1) {
                $('#kpiNotSet').show();
                $('#kpiNotSetM').show();
            }
            else {
                $('#kpiNotSet').hide();
                $('#kpiNotSetM').hide();
                for (i = 0; i < data.trainerSubList.length; i++) {
                    var uncheckList = $("input[name='uncheck']");
                    var areaList = $("span[name='area_l1_id']");
                    var boxList = $("div[name='box']");
                    if (data.jobInfo.position_type == 12) {
                        if (data.trainerSubList[i].is_included == false) {
                            $(uncheckList[i]).attr("checked", "checked");
                            $(areaList[i]).text(data.trainerSubList[i].area_l1_name);
                            $(boxList[i]).css("display", "none");
                        } else {
                            $(areaList[i]).text(data.trainerSubList[i].area_l1_name);
                        }
                    } else {
                        if (data.trainerSubList[i].is_included == false) {
                            $(uncheckList[j]).attr("checked", "checked");
                            j++;
                        }
                    }
                }
            }
            var intern_salary_type = data.trainerInfo.intern_salary_type;
            var emp_category = data.jobInfo.emp_category;
            if (emp_category == "实习生") {
                $("#position_name").text(data.jobInfo.position_name + "(" + emp_category + ")");
                $("#EmpType2").css("display", "");
                if (intern_salary_type == 1) {
                    $("#intern_salary_type").html("按总工资比例");
                    $("#salary").html(ToThousandsStr(data.trainerInfo.intern_ratio_salary));
                    $("#salary_type").html("%");
                } else {
                    $("#intern_salary_type").html("按固定金额");
                    $("#salary").html(ToThousandsStr(data.trainerInfo.intern_fix_salary));
                    $("#salary_type").html("元/月");
                }
            } else {
                $("#EmpType").css("display", "");
                $("#traffic_subsidy").text(ToThousandsStr(data.trainerInfo.traffic_subsidy));
                var sum = data.payrollInfo.base_salary + data.payrollInfo.house_subsidy + data.payrollInfo.attendance_reward + data.payrollInfo.seniority_salary + data.trainerInfo.traffic_subsidy;
                $("#base_all").text(ToThousandsStr(sum));
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});