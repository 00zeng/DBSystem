var id = $.request("id");

$(function () {
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetSalaryInfo",
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.kpiSubInfo);
            $("#kpi_subsidy").text(ToThousandsStr(data.kpiSubInfo.kpi_subsidy));

            $("#form1").formSerializeShow(data.jobInfo);
            var date = data.salaryInfo.effect_date.substring(0, 7);
            $("#effect_date").text(date);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});