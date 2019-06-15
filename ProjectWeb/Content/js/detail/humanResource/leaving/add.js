var id = $.request("id");
var leaving_id;
//获取登录人的信息
var myInfo = top.clients.loginInfo;
$("#name").text(myInfo.empName);
$("#work_number").text(myInfo.work_number);
if (myInfo.positionInfo.areaId > 0)
    $("#area_name").text(myInfo.positionInfo.areaNmae);
else
    $("#area_name").text(myInfo.companyInfo.linkName);
$("#dept_name").text(myInfo.positionInfo.deptName);
$("#position_name").text(myInfo.positionInfo.name);
    
//获取证明按钮
//$(function () {
//    $("#category_code option").click(function () {
//        //				$(this).index()//获取索引值
//        $("#prove li").eq($(this).index()).show().siblings("li").hide();
//    });
//});

//提交内容
function submitForm() {
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["leaving_type"] = $("#holiday_type").val();
 
    $.submitForm({
        url: "/HumanResource/Leaving/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/Leaving/Index?id=" + id;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
