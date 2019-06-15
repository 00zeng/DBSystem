var id = $.request("id");
var leaving_id;
var Myposition_name = top.clients.loginInfo.positionInfo.name;
//获取数据
$(function () {
    $.ajax({
        url: "/HumanResource/Leaving/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            
            $("#form1").formSerializeShow(data.mainInfo);
            $("#approve").formSerializeShow(data.mainInfo);
            //请假类型
            holiday_type = data.mainInfo.leaving_type;
            if (holiday_type == 1) {
                $("#holiday_type").text("事假");
            } else if (holiday_type == 2) {
                $("#holiday_type").text("病假");
            } else if (holiday_type == 3) {
                $("#holiday_type").text("婚假");
            } else if (holiday_type == 4) {
                $("#holiday_type").text("陪育假/产假");
            } else if (holiday_type == 5) {
                $("#holiday_type").text("丧假");
            }
            //审批状态
            if (data.mainInfo.approve_status == 0) {
                $("#approve_status").text("未审批");
            } else if (data.mainInfo.approve_status == 100) {
                $("#approve_status").text("通过");
            } else if (data.mainInfo.approve_status < 0) {
                $("#approve_status").text("不通过");
            } else {
                $("#approve_status").text("审批中");
            }
            //请假时间
            $("#create_time1").text(data.mainInfo.create_time);
            $("#creator_name1").text(data.mainInfo.creator_name);
            var tr = '';
            $.each(data.approveInfoList, function (index, item) {
                if (item.status > 0) {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                } else {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }
                
            })
            $("#approve tr:eq(1)").after(tr);
            $("#next_approve").html(Myposition_name+'审批');
           
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });

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
function submitForm() {
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["status"] = $("input[name='status']:checked").val();
    data["main_id"] = id;
    $.submitForm({
        url: "/HumanResource/Leaving/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/Leaving/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}