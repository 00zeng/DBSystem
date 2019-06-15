var id = $.request("id");

//获取数据
$(function () {
    $.ajax({
        url: "/PersonalManage/ResignationRequest/ShowInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {

            $("#job_number").text(data.employeeInfo.job_number);
            $("#name").text(data.employeeInfo.name);
            gender = data.employeeInfo.gender;
            if (gender == 0) {
                $("#gender").text("女");
            } else if (gender == 1) {
                $("#gender").text("男");
            } else {
                $("#gender").text("未指定");
            }
            $("#education").text(data.employeeInfo.education);
            $("#entry_time").text(data.employeeInfo.entry_time.substring(0,10));
            
            $("#department_id").text(data.employeeInfo.department_name);
            $("#area").text(data.employeeInfo.area);
            $("#position_id").text(data.employeeInfo.position_name);
            //$("#city").text(data.employeeInfo.city);
            //$("#address").text(data.employeeInfo.address);

            //data.accountInfo.name

            $("#leave_time").text(data.employeeInfo.leave_time);
            $("#dept_charge_name").text(data.accountInfo.account);
            //$("#note").text(data.employeeInfo.note);
            //$("#note").text(data.employeeInfo.note);
            //$("#note").text(data.employeeInfo.note);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
});


//提交内容
function submitForm() {
    var data = {};
    var resignInfo = {};
  
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }

    resignInfo["job_number"] = $("#job_number").text();
    resignInfo["name"] = $("#name").text();
    resignInfo["gender"] = $("#gender").text();
    resignInfo["education"] = $("#education").text();
    resignInfo["entry_time"] = $("#entry_time").text();

    resignInfo["department_id"] = $("#department_id").text();
    resignInfo["area_id"] = $("#area_id").text();
    resignInfo["position_id"] = $("#position_id").text();
    //resignInfo["city"] = $("#city").text();
    //resignInfo["address"] = $("#address").text();

    resignInfo["leave_time"] = $("#leave_time").text();
    resignInfo["dept_charge_name"] = $("#dept_charge_name").text();
    //resignInfo["position_name"] = $("#position_name").text();
    //resignInfo["city"] = $("#city").text();
    //resignInfo["address"] = $("#address").text();

    resignInfo["content"] = $("#content").val();
    resignInfo["later_arrangement"] = $("#later_arrangement").val();
    resignInfo["later_opinion"] = $("#later_opinion").val();

    data["resignInfo"] = resignInfo;
    
    $.submitForm({
        url: "/PersonalManage/ResignationRequest/SubmitNew?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/PersonalManage/ResignationRequest/Index?id=" + id;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
