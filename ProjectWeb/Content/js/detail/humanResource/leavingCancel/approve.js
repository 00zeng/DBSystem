var id = $.request("id");
var Myposition_name = top.clients.loginInfo.positionInfo.name;
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
            $("#next_approve").html(Myposition_name + '审批');

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
        url: "/HumanResource/LeavingCancel/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/LeavingCancel/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}