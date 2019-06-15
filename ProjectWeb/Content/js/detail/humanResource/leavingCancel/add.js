var id = $.request("id");
//var leaving_id;
$(function () {
    $.ajax({
        url: "/HumanResource/Leaving/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        //async:false,
        success: function (data) {
            
            $("#form1").formSerializeShow(data.mainInfo);
            if (data.mainInfo.leaving_type == 1) {
                $("#leaving_type").text("事假");
            } else if (data.mainInfo.leaving_type == 2) {
                $("#leaving_type").text("病假");
            } else if (data.mainInfo.leaving_type == 3) {
                $("#leaving_type").text("婚假");
            } else if (data.mainInfo.leaving_type == 4) {
                $("#leaving_type").text("产假");
            } else if (data.mainInfo.leaving_type == 5) {
                $("#leaving_type").text("丧假");
            } else if (data.mainInfo.leaving_type == 6) {
                $("#leaving_type").text("其他");
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});
//提交内容
function submitForm() {
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["leaving_id"] = id;
    $.submitForm({
        url: "/HumanResource/LeavingCancel/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/LeavingCancel/Index?id=" + id;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
