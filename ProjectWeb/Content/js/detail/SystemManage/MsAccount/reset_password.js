var id = $.request("id");
function submitForm() {
    var newPassword = $("#newPassword").val();
    var newPassword2 = $("#newPassword2").val();
    if (newPassword != newPassword2) {
        top.layer.alert("两次密码输入不相同");
        return false;
    }
    var data = { id: id, new_password: newPassword.trim() };
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/SystemManage/MsAccount/ResetPassword?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                $.currentWindow().location.reload();
                top.layer.closeAll();
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
