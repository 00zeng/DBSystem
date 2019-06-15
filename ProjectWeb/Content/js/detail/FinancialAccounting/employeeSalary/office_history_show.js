var id = $.request("id");
$(function () {
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetSalaryInfo",
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.generalInfo);
            $("#resign_deposit").text(ToThousandsStr(data.generalInfo.resign_deposit));
            $("#traffic_subsidy").text(ToThousandsStr(data.generalInfo.traffic_subsidy));

            $("#form1").formSerializeShow(data.jobInfo);
            $("#form1").formSerializeShow(data.salaryInfo);
            var intern_salary_type;
            intern_salary_type = data.generalInfo.intern_salary_type;
            var emp_category = data.jobInfo.emp_category;
            //是否为实习生
            if (emp_category == "实习生") {
                $("#EmpCategory").css("display", "");
                $("#position_name").text(data.jobInfo.position_name + "(" + emp_category + ")");
            }
            if (intern_salary_type == 1) {
                $("#salary_fix").css('display', 'none');
                $("#salary_ratio").css('display', '');
                $("#intern_fix_salary").val('');
            } else if (intern_salary_type == 2) {
                $("#salary_ratio").css('display', 'none');
                $("#salary_fix").css('display', '');
                $("#intern_fix_salary").text(ToThousandsStr(data.generalInfo.intern_fix_salary));

                $("#intern_ratio_salary").val('');
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});