var id = $.request("id");
var approve_status;
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
            } else if (data.mainInfo.approve_status ==100) {
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
            } else
                $("#gender").text("未指定");

            if (data.approveInfoList.length == 0) {
                $("#approve1").show();
                $("#request_date_top").text(data.mainInfo.request_date.substring(0, 10));
            }else
                $("#approve").show();
                //$("#request_date_top").text(data.mainInfo.request_date.substring(0, 10));

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
})
function submitForm() {
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    if (approve_status == 0) {
        data["status"] = $("input[name='status1']:checked").val();
        data["approve_note"] = $("#approve_note1").val();
    }else 
        data["status"] = $("input[name='status']:checked").val();


    data["id"] = id;
    data["main_id"] = id;
    $.submitForm({
        url: "/HumanResource/PositionChange/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/PositionChange/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

