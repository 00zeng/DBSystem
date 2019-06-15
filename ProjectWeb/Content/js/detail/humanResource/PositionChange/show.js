var id = $.request("id");
$(function () {
    $.ajax({
        url: "/HumanResource/PositionChange/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        //async:false,
        success: function (data) {
            
            $("#form1").formSerializeShow(data.mainInfo);
            $("#form1").formSerializeShow(data.empInfo);
            $("#entry_date").text(data.entry_date.substring(0, 10));
            approve_status = data.mainInfo.approve_status;
            $("#approve_status").text(data.mainInfo.approve_status);

            if (data.mainInfo.approve_status == 0) {
                $("#approve_status").text("未审批");
            } else if (data.mainInfo.approve_status > 0) {
                $("#approve_status").text("审批中");
            } else if (data.mainInfo.approve_status == 100) {
                $("#approve_status").text("通过");
            } else
                $("#approve_status").text("未通过");
            if (data.mainInfo.type == 1) {
                $("#type").text("转区域");
            } else if (data.mainInfo.type == 2) {
                $("#type").text("转部门");
            } else if (data.mainInfo.type == 3) {
                $("#type").text("转机构");
            }
            if (data.empInfo.gender == 0) {
                $("#gender").text("女");
            } else if (data.empInfo.gender == 1) {
                $("#gender").text("男");
            } else {
                $("#gender").text("未指定");
            }
            var tr = '';
            $.each(data.approveInfoList, function (index, item) {
                //if (item.status == 1) {
                //    tr += '<tr><td>核准转岗时间</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.date_approve + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                //}
                if (item.status > 0) {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }else {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }

                
            })
            $("#approve tr:eq(0)").after(tr);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
})
