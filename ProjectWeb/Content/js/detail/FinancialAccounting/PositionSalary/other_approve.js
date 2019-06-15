var id = $.request("id");
var approve_result;
$(function () {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetSeniority?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.getInfo);
            if (data.getInfo.effect_date == '' || data.getInfo.effect_now == true) {
                $("#effect_date").text("立即生效");
            }
            $.each(data.infoList, function (index, item) {
                var tr;
                if (item.year_max != -1) {
                    tr = '<tr><td>' + item.year_min + "-" + item.year_max + '</td>' + '<td>' + item.salary + '</td></tr>';
                } else {
                    tr = '<tr><td>' + item.year_min + "以上"+ '</td>' + '<td>' + item.salary + '</td></tr>';
                }
                $("#seniority_list").append(tr);
            })
            //审批
            //var positionSalary = data.positionSalary;
            //$("#form1").formSerializeShow(positionSalary);

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