var id = $.request("id");
var leaving_id;
//获取数据
$(function () {
    $.ajax({
        url: "/HumanResource/Leaving/ShowInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {

            if (data.loginJobInfo.id == data.jobInfo.id && data.leavingInfo.approve_result == 0) {
                $("#btnDel").css("display", "block");
                //alert("显示撤回按钮");
            }
            else {
                $("#btnDel").css("display", "none");
                //alert("隐藏撤回按钮");
            }

            if (data.loginJobInfo.id == data.jobInfo.id && data.leavingInfo.approve_result != 0) {
                $("#btnCancel").css("display", "block");
                //alert("显示销假按钮");
            }
            else {
                $("#btnCancel").css("display", "none");
                //alert("隐藏销假按钮");
            }

            leaving_id = data.leavingInfo.id;
            holiday_type = data.leavingInfo.holiday_type;
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
            $("#times").text((data.leavingInfo.begin_time).substring(0, 10) + "  -  " + (data.leavingInfo.end_time).substring(0, 10));
            $("#approve_status").text(data.leavingInfo.approve_status);

            $("#job_number").text(data.leavingInfo.job_number);
            $("#name").text(data.leavingInfo.name);
            if (data.jobInfo.area_id == 0)
                $("#area_id").text("  ");
            $("#area_id").text(data.leavingInfo.area_id);
            $("#department_id").text(data.jobInfo.department_name);
            $("#position_id").text(data.jobInfo.position_name);
            $("#content").text(data.leavingInfo.content);
            $("#days").text(data.leavingInfo.days);
            $("#name").text(data.leavingInfo.name);
            $("#begin_time").text(data.leavingInfo.begin_time);
            $("#end_time").text(data.leavingInfo.end_time);
            //$("#opinion").text(data.leavingInfo.opinion);
            var step = data.leavingInfo.approve_result;
            switch (step) {
                case 3:
                case -3:
                    //console.log(data.approveInfo3);
                    //$("#opinion3").text(data.approveInfo3.opinion);
                    opinion3 = data.approveInfo3.opinion;
                    if (opinion3 == 1) {
                        $("#opinion3").text("同意");
                    } else if (opinion3 == -1) {
                        $("#opinion3").text("不同意");
                    }
                    $("#name3").text(data.approveInfo3.name);
                    $("#approve_time3").text(data.approveInfo3.approve_time);
                    $(".approve3").css('display', 'block');
                case 2:
                case -2:
                    //console.log(data.approveInfo2);
                    opinion2 = data.approveInfo2.opinion;
                    if (opinion2 == 1) {
                        $("#opinion2").text("同意");
                    } else if (opinion2 == -1) {
                        $("#opinion2").text("不同意");
                    }
                    $("#name2").text(data.approveInfo2.name);
                    $("#approve_time2").text(data.approveInfo2.approve_time);
                    $(".approve2").css('display', 'block');
                case 1:
                case -1:
                    //console.log(data.approveInfo1);
                    opinion1 = data.approveInfo1.opinion;
                    if (opinion1 == 1) {
                        $("#opinion1").text("同意");
                    } else if (opinion1 == -1) {
                        $("#opinion1").text("不同意");
                    }
                    $("#name1").text(data.approveInfo1.name);
                    $("#approve_time1").text(data.approveInfo1.approve_time);
                    $(".approve1").css('display', 'block');
                case 0:
                    //console.log(data.leavingInfo);
                    //$("#opinion3").text(data.approveInfo3.opinion);
                    break;
                default:
                    break;
            }



        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });

});

//审批
function approval() {
    window.location.href = "/HumanResource/Leaving/LeavingApproval?id=" + id;
}
//销假
function cancel() {
    window.location.href = "/HumanResource/LeavingCancel/LeavingCancelAdd?id=" + id;
}


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
