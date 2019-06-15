var id = $.request("id");
//var increase_award_status;//奖励启用状态
var guide_base_type;
$(function () {
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetInfo?id=" + id,
        type: "get",
        dataType: "json",
        success: function (data) {
            $("#form1").formSerializeShow(data.jobInfo); 
            $("#form1").formSerializeShow(data.generalSalaryInfo);

            if (data.generalInfo== null) {//未设置导购薪资
                $("#form1").formSerializeShow(data.guidePosInfo);
                $("#form1").formSerializeShow(data.guidePosMainInfo);
                guide_base_type = data.guidePosInfo.guide_base_type;
                if (guide_base_type == 4) {
                    $("#guide_base_type").text("保底工资");
                    $("#salary_base").css("display", "");
                    $("#guide_salary_base").text(ToThousandsStr(data.guidePosInfo.guide_salary_base));
                } else if (guide_base_type == 1) {
                    $("#guide_base_type").text("达标底薪");
                    $("#salary_commission").css("display", "");
                    $("#guide_standard_commission").text(ToThousandsStr(data.guidePosInfo.standard_commission));
                    $("#guide_standard_salary").text(ToThousandsStr(data.guidePosInfo.standard_salary));
                } else if (guide_base_type == 2) {
                    $("#guide_base_type").text("星级制");
                } else if (guide_base_type == 3) {
                    $("#guide_base_type").text("浮动底薪");
                } else if (guide_base_type == 0) {
                    $("#guide_base_type").text("0底薪");
                }
                if (data.guidePosInfo.guide_annualbonus_type == 1) {
                    $("#guide_annualbonus_type").text("按销量");
                } else if (data.guidePosInfo.guide_annualbonus_type == 2) {
                    $("#guide_annualbonus_type").text("按星级");
                }
                $("#noSetting").css("display", "");

            } else {
                $("#form1").formSerializeShow(data.generalInfo);
                $("#resign_deposit").text(ToThousandsStr(data.generalInfo.resign_deposit));

                guide_base_type = data.generalInfo.guide_base_type;
                if (guide_base_type == 4) {
                    $("#guide_base_type").text("保底工资");
                    $("#salary_base").css("display", "");
                    $("#guide_salary_base").text(ToThousandsStr(data.generalInfo.guide_salary_base));
                } else if (guide_base_type == 1) {
                    $("#guide_base_type").text("达标底薪");
                    $("#salary_commission").css("display", "");
                    $("#guide_standard_commission").text(ToThousandsStr(data.generalInfo.guide_standard_commission));
                    $("#guide_standard_salary").text(ToThousandsStr(data.generalInfo.guide_standard_salary));
                } else if (guide_base_type == 2) {
                    $("#guide_base_type").text("星级制");
                } else if (guide_base_type == 3) {
                    $("#guide_base_type").text("浮动底薪");
                } else if (guide_base_type == 0) {
                    $("#guide_base_type").text("0底薪");
                }
                if (data.generalInfo.guide_annualbonus_type == 1) {
                    $("#guide_annualbonus_type").text("按销量");
                } else if (data.generalInfo.guide_annualbonus_type == 2) {
                    $("#guide_annualbonus_type").text("按星级");
                }
            }
            var increase_award_status = data.guidePosInfo.increase_award_status;
            if (increase_award_status == 1) {
                $("#increase_award_status").text("启用");
            } else {
                $("#increase_award_status").text("关闭");
            }
            
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});