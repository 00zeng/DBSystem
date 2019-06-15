var id = $.request("id");
var approve_status;
var name = decodeURI($.request("name"));
$("#name").text(name);
$(function () {
    setInterval(function () { $('#currentTime').text((new Date()).pattern("yyyy-MM-dd HH:mm:ss")) }, 1000);
    $.ajax({
        url: "/DistributorManage/Image/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#approve_name").text(top.clients.loginInfo.empName);
            $("#approve_position_name").text(top.clients.loginInfo.positionInfo.name);

            $("#form1").formSerializeShow(data.mainInfo);
            $("#creator_position_name").text(data.creator_position_name);

            $("input:radio[name=target_mode][value=" + data.mainInfo.target_mode + "]").attr("checked", true);
            $("input:radio[name=target_content][value=" + data.mainInfo.target_content + "]").attr("checked", true);
            if (data.mainInfo.target_mode == 2) {
                $("#mode1").css("display", "none");
                $("#mode2").css("display", "none");
                $("#mode3").css("display", "");
                $("#mode4").css("display", "");
            } else if (data.mainInfo.target_mode == 1) {
                $("#mode1").css("display", "");
                $("#mode2").css("display", "");
                $("#mode3").css("display", "none");
                $("#mode4").css("display", "none");
            };
            approve_status = data.mainInfo.approve_status ;

            //查看审批
            var tr = '';
            $("#creator_position_name").text(data.creator_position_name);
            $.each(data.approveList, function (index, item) {
                if (item.approve_note == null || item.approve_note == '') {
                    approve_note_null = "--";
                } else
                    approve_note_null = item.approve_note;

                if (item.status == 1 || item.status == -1)
                    approve_grade = ("一级审批");
                else if (item.status == 2 || item.status == -2)
                    approve_grade = ("二级审批");
                else if (item.status == 3 || item.status == -3)
                    approve_grade = ("三级审批");
                else if (item.status == 4 || item.status == -4)
                    approve_grade = ("四级审批");
                else if (item.status == 100 || item.status == -100)
                    approve_grade = ("终审");
                if (item.status > 0) {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                } else {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})
function submitForm() {
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["status"] = $("input[name='status']:checked").val();
    data["main_id"] = id;
    $.submitForm({
        url: "/DistributorManage/Image/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/DistributorManage/Image/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}