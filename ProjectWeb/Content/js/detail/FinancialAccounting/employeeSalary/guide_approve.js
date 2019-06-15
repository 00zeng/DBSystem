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
$(function () {
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetSalaryInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.generalInfo);
            $("#form1").formSerializeShow(data.salaryInfo);
            if (data.salaryInfo.effect_now == true) {
                $("#effect_date").text("立即生效");
            }
            $("input:radio[name=guide_base_type][value=" + data.generalInfo.guide_base_type + "]").attr("checked", true);
            $("input:radio[name=guide_annualbonus_type][value=" + data.generalInfo.guide_annualbonus_type + "]").attr("checked", true);
            if (data.generalInfo.guide_base_type == "4") {
                $("#base").css("display", "inline");
                $("#standard").css("display", "none");
            } else if (data.generalInfo.guide_base_type == "1") {
                $("#standard").css("display", "inline");
                $("#base").css("display", "none");
            } else {
                $("#base").css("display", "none");
                $("#standard").css("display", "none");
            }

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});
function submitForm() {
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["status"] = $("input[name='status']:checked").val();
    data["main_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/EmployeeSalary/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/EmployeeSalary/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

