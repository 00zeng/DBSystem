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

            $("#form1").formSerializeShow(data.generalInfo);
            
            $("#form1").formSerializeShow(data.jobInfo);
            $("#form1").formSerializeShow(data.kpiSubInfo);
            $("#kpi_subsidy").text(ToThousandsStr(data.kpiSubInfo.kpi_subsidy));

            $("#form1").formSerializeShow(data.subsidyInfo); 
            $("#form1").formSerializeShow(data.SubInfo);
            $("#subsidy").text(ToThousandsStr(data.SubInfo.subsidy));

            $("#form1").formSerializeShow(data.generalSalaryInfo);
            var intern_salary_type;
            intern_salary_type = data.generalInfo.intern_salary_type;
            var emp_category = data.jobInfo.emp_category;
            if (emp_category == "实习生") {
                $("#position_name").text(data.jobInfo.position_name + "(" + emp_category + ")");
                $("#EmpType2").css("display", "");
                if (intern_salary_type == 1) {
                    $("#intern_salary_type").html("按总工资比例");
                    $("#salary").html(ToThousandsStr(data.generalInfo.intern_ratio_salary));
                    $("#salary_type").html("%");
                } else {
                    $("#intern_salary_type").html("按固定金额"); 
                    $("#salary").html(ToThousandsStr(data.generalInfo.intern_fix_salary));
                    $("#salary_type").html("元/月");
                }
            } else {
                $("#EmpType").css("display", "");
                $("#traffic_subsidy").text(ToThousandsStr(data.generalInfo.traffic_subsidy));
                var sum = data.payrollInfo.base_salary + data.payrollInfo.house_subsidy + data.payrollInfo.attendance_reward + data.payrollInfo.seniority_salary + data.generalInfo.traffic_subsidy;
                $("#base_all").text(ToThousandsStr(sum));
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});