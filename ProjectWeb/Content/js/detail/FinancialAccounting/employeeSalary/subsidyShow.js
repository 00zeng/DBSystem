var id = $.request("id");
var company_name = decodeURI($.request("company_name"));
$("#company_name").text(company_name);
var position_name = decodeURI($.request("position_name"));
$("#position_name").text(position_name);
var name = decodeURI($.request("name"));
$("#name").text(name);
var entry_date = $.request("entry_date");
$("#entry_date").text(entry_date);
var grade = decodeURI($.request("grade"));
$("#grade").text(grade);
var subsidy;

$(function () {
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetSalaryInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.subsidyInfo);
            $("#form1").formSerializeShow(data.salaryInfo);
            subsidy = data.subsidyInfo.subsidy;
            if (data.salaryInfo.effect_now == true) {
                $("#effect_date").text("立即生效");
            } 
            $(function () {
                $.ajax({
                    url: "/FinancialAccounting/EmployeeSalary/GetPosSalary?date=" + new Date(),
                    type: "get",
                    dataType: "json",
                    data: { id: id },
                    success: function (data) {
                        var subsidy_sum = data + subsidy;
                        $("#subsidy_sum").text(subsidy_sum);


                    },
                    error: function (data) {
                        $.modalAlert(data.responseText, 'error');
                    }
                })
            });
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});