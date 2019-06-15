var id = $.request("id");
var approve_result;
$(function () {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetSales?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.getInfo);
            if (data.getInfo.effect_date == '' || data.getInfo.effect_now == true) {
                $("#effect_date").text("立即生效");
            }
            var tr;
            $.each(data.infoList, function (index, item) {
                switch (item.category) {
                    case 1:
                        if (item.target_max == -1) {
                            tr = '<tr><td>' + item.target_min + '%及以上' + '</td>' + '<td>' + item.amount + '</td>' + '<td>' + item.maiduan + '</td></tr>';
                        } else {
                            tr = '<tr><td>' + item.target_min + '%' + '-' + item.target_max + '%' + '</td>' + '<td>' + item.amount + '</td>' + '<td>' + item.maiduan + '</td></tr>';
                        }
                        $("#finish_rate").append(tr);
                        break;
                    case 2:
                        if (item.target_max == -1) {
                            tr = '<tr><td>' + item.target_min + '及以上' + '</td>' + '<td>' + item.amount + '</td>' + '<td>' + item.maiduan + '</td></tr>';
                        } else {
                            tr = '<tr><td>' + item.target_min + '-' + item.target_max + '</td>' + '<td>' + item.amount + '</td>' + '<td>' + item.maiduan + '</td></tr>';
                        }
                        $("#sales_volume").append(tr);
                        break;
                    default: break;
                }
            })
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
