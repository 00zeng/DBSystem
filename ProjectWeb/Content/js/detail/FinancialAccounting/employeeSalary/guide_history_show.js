var id = $.request("id");
//var increase_award_status;//奖励启用状态
var guide_base_type;
$(function () {
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetSalaryInfo?id=" + id,
        type: "get",
        dataType: "json",
        success: function (data) {
            $("#form1").formSerializeShow(data.jobInfo);
            $("#form1").formSerializeShow(data.generalInfo);
            $("#resign_deposit").text(ToThousandsStr(data.generalInfo.resign_deposit));

            $("#form1").formSerializeShow(data.salaryInfo);
            //生效时间
            guide_base_type = data.generalInfo.guide_base_type;
            if (guide_base_type == 4) {
                $("#salary_base").css("display", "");
                $("#guide_salary_base").text(ToThousandsStr(data.generalInfo.guide_salary_base));
            } else if (guide_base_type == 1) {
                $("#salary_commission").css("display", "");
                $("#guide_standard_commission").text(ToThousandsStr(data.generalInfo.guide_standard_commission));
                $("#guide_standard_salary").text(ToThousandsStr(data.generalInfo.guide_standard_salary));
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});