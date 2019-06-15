$("#role_id").bindSelect({
    text: "name",
    url: "/SystemManage/MsRole/GetSelectJson",
    search: true,
    firstText: "--请选择所属角色--"
});

function submitForm() {
    if (!$("#form1").formValid())
        return false;
    //验证用户名  账号
    var account = $("#account").val().trim();
    if (!!account) {
        if (!/^[\w\u3E00-\u9FA5]+$/g.test(account)) {
            $.modalAlert("账号为由字母、数字、_或汉字组成", "error");
            return false;
        }
    }

    //确认密码
    var password = $("#password").val();
    var password2 = $("#password2").val();
    if (password2 != password) {
        $.modalAlert("两次密码输入不一致，请核实之后重新输入！", "error");
        return false;
    }

    var data = { account: account, role_id: $("#role_id").val(), password: password.trim() };
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/SystemManage/MsAccount/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                $.currentWindow().ReSearch();
                top.layer.closeAll();
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}