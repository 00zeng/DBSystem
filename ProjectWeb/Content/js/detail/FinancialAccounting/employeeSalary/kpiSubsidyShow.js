var id = $.request("id");
var company_name = decodeURI($.request("company_name"));
$("#company_name").text(company_name);
var position_name = decodeURI($.request("position_name"));
$("#position_name").text(position_name);
var name = decodeURI($.request("name"));
$("#name").text(name);
var entry_date = $.request("entry_date");
$("#entry_date").text(entry_date.substring(0, 10));
var grade = decodeURI($.request("grade"));
$("#grade").text(grade);
var emp_category = decodeURI($.request("emp_category"));

$(function () {
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetSalaryInfo",
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.kpiSubInfo);
            if (data.salaryInfo.effect_now == true) {
                $("#effect_date").text("立即生效");
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});