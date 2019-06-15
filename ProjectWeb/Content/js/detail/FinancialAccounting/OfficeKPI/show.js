var id = $.request("id");

//获取登录人信息
var myPositionInfo = top.clients.loginInfo.positionInfo;
//myPositionInfo.positionType
$(function () {
    $.ajax({
        url: "/FinancialAccounting/OfficeKPI/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            console.log(data);
            $("#form1").formSerializeShow(data.ApprovelistInfo);
            $("#form1").formSerializeShow(data.kpiscoreInfo);
            $("#kpi").text(ToThousandsStr(data.kpiscoreInfo.kpi));

            //审批
            var tr = '';
            $.each(data.ApprovelistInfo, function (index, item) {
                if (item.status > 0) {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                } else {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});
