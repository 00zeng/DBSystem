var id = $.request("id");
var name;
var role_name;
$(function () {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetTrainer?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.infoMain);
            $("#form1").formSerializeShow(data.getInfo);
            if (data.getInfo.effect_date == '' || data.getInfo.effect_now == true) {
                $("#effect_date").text("立即生效");
            }

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});
function backForm() {
    window.location.href = "javascript: history.go(-1)";
}

function submitForm() {
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["status"] = $("input[name='status']:checked").val();
    data["salary_position_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/PositionSalary/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/PositionSalary/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}