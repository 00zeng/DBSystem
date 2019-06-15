var id = $.request("id");
var gender;
//获取数据
$(function () {
    $.ajax({
        url: "/HumanResource/GradeChange/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            
            $("#form1").formSerializeShow(data.jobInfo);
            $("#form1").formSerializeShow(data.mainInfo);
            var type = data.mainInfo.type;
            switch (type) {
                case 1:
                    $("#type").text("晋升");
                    break;
                case 2:
                    $("#type").text("降级");
                    break;
                default:
                    break;
            }
            if (data.mainInfo.gender == 0) {
                $("#gender").text("未指定");
            } else if (data.mainInfo.gender == 1) {
                $("#gender").text("女");
            } else {
                $("#gender").text("男");
            }
            if (data.mainInfo.approve_status == 0) {
                $("#approve_status").text("未审批");
            } else if (data.mainInfo.approve_status > 0) {
                $("#approve_status").text("审批中");
            } else if (data.mainInfo.approve_status == 100) {
                $("#approve_status").text("通过");
            } else
                $("#approve_status").text("未通过");
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
});