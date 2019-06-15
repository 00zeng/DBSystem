var id = $.request("id");
var subsidy;
$(function () {
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetSalaryInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.subsidyInfo);
            $("#subsidy").text(ToThousandsStr(data.subsidyInfo.subsidy));

            $("#form1").formSerializeShow(data.salaryInfo);
            $("#form1").formSerializeShow(data.jobInfo);
            subsidy = data.subsidyInfo.subsidy;
            var date = data.salaryInfo.effect_date.substring(0, 7);
            $("#effect_date").text(date);
            $(function () {
                $.ajax({
                    url: "/FinancialAccounting/EmployeeSalary/GetPOSSalary?date=" + new Date(),
                    type: "get",
                    dataType: "json",
                    data: { id: data.jobInfo.id },
                    success: function (data) {
                        var subsidy_sum = data + subsidy;
                        $("#subsidy_sum").text(ToThousandsStr(subsidy_sum));


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
