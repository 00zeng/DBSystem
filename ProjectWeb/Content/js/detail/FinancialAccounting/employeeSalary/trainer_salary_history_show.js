var id = $.request("id");

$(function () {
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetSalaryInfo",
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.jobInfo);
            $("#form1").formSerializeShow(data.salaryInfo);
            $("#form1").formSerializeShow(data.generalInfo);
            
            $("#resign_deposit").text(ToThousandsStr(data.generalInfo.resign_deposit));
           
            var intern_salary_type = data.generalInfo.intern_salary_type;
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
                $("#base_salary").text(ToThousandsStr(data.generalInfo.base_salary));
                $("#house_subsidy").text(ToThousandsStr(data.generalInfo.house_subsidy));
                $("#attendance_reward").text(ToThousandsStr(data.generalInfo.attendance_reward));
                $("#seniority_salary").text(ToThousandsStr(data.generalInfo.seniority_salary));
                $("#traffic_subsidy").text(ToThousandsStr(data.generalInfo.traffic_subsidy));
                var sum = data.generalInfo.base_salary + data.generalInfo.house_subsidy + data.generalInfo.attendance_reward + data.generalInfo.seniority_salary + data.generalInfo.traffic_subsidy;
                $("#base_all").text(ToThousandsStr(sum));
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});