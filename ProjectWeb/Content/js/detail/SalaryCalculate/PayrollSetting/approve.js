var id = $.request("id");
$(function () {
    setInterval(function () { $('#currentTime').text((new Date()).pattern("yyyy-MM-dd HH:mm:ss")) }, 1000);
    $.ajax({
        url: "/SalaryCalculate/PayrollSetting/GetInfoByID?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data);
            $("#approve_name").text(top.clients.loginInfo.empName);
            $("#approve_position_name").text(top.clients.loginInfo.positionInfo.name);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});
//function submitForm() {
//    var data = {};
//    data["approve_status"] = $("input[name='status']:checked").val();
//    data["note"] = $("#note").val();
//    data["id"] = id;
//    $.submitForm({
//        url: "/SalaryCalculate/PayrollSetting/Approve?date=" + new Date(),
//        param: data,
//        success: function (data) {
//            if (data.state == "success") {
//                window.location.href = "/SalaryCalculate/PayrollSetting/Index";
//            }
//        }
//    })
//}

function submitForm() {
    var data = {};
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["id"] = id;
    data["approve_status"] = $("input[name='status']:checked").val();
    data["approve_note"] = $("#approve_note").val();
    
    $.submitForm({
        url: "/SalaryCalculate/PayrollSetting/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/SalaryCalculate/PayrollSetting/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}