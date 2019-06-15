var id = $.request("id");
function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var oldPassword = $("#oldPassword").val();
    var newPassword = $("#newPassword").val();
    var newPassword2 = $("#newPassword2").val();
    if (newPassword != newPassword2) {
        top.layer.alert("两次密码输入不相同");
        return false;
    }
    var data = { pass: oldPassword, new_pass: newPassword.trim() };
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/Account/updatePassword?date=" + new Date(),
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