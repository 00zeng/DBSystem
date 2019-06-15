var id = $.request("id");
//var leaving_id;
$(function () {
    $.ajax({
        url: "/HumanResource/LeavingCancel/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        //async:false,
        success: function (data) {
            
            $("#form1").formSerializeShow(data.mainInfo); 
            $("#form1").formSerializeShow(data.leavingInfo);
            var leaving_type = data.leavingInfo.leaving_type;
            $("#actual_days").html(data.mainInfo.actual_days);
            switch (leaving_type) {
                case 1:
                    $("#leaving_type").text("事假");
                    break;
                case 2:
                    $("#leaving_type").text("病假");
                    break;
                case 3:
                    $("#leaving_type").text("婚假");
                    break;
                case 4:
                    $("#leaving_type").text("产假");
                    break;
                case 5:
                    $("#leaving_type").text("丧假");
                    break;
                case 6:
                    $("#leaving_type").text("其他");
                    break;
                default:
                    break;
            }
            var cancel_type = data.mainInfo.cancel_type;
            switch (cancel_type) {
                case 1:
                    $("#cancel_type").text("取消休假");
                    break;
                case 2:
                    $("#cancel_type").text("提前回岗");
                    break;
                default:
                    break;
            }
            var tr = '';
            $.each(data.approveInfoList, function (index, item) {
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


//撤回
function delForm() {
    top.layer.confirm("确认要撤回? ", function (index) {

        var data = {};
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["id"] = id;
        $.ajax({
            url: "/HumanResource/Leaving/Delete?id=" + id,
            type: "post",
            data: data,
            success: function (data) {
                top.layer.msg("撤回成功");
                window.location.href = "javascript: history.go(-1)";

            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}
