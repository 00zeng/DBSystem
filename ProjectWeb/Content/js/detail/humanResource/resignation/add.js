var job_info_id;
var id = top.clients.loginInfo.empId;
//获取数据
$(function () {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data:{id:id},
        success: function (data) {
            
            $("#form1").formSerializeShow(data.empJobInfo);
            $("#form1").formSerializeShow(data.empInfo);
            if (data.empInfo.gender == 0) {
                $("#gender").text("女");
            } else if (data.empInfo.gender == 1){
                $("#gender").text("男");
            }else{
                $("#gender").text("未指定");
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
});
//提交内容
function submitForm() {
    var data = $("#form1").formSerialize();
   
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["emp_id"] = id;
    
    
    $.submitForm({
        url: "/HumanResource/Resign/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/Resign/Index?id=" + id;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })

}
