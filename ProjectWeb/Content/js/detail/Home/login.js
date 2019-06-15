$("#login_button").on("click", function () {
    var $username = $("#account");
    var $password = $("#password");
    if ($username.val() == "") {
        $username.focus();
        $.login.formMessage('请输入用户名/手机号/邮箱。');
        return false;
    } else if ($password.val() == "") {
        $password.focus();
        $.login.formMessage('请输入登录密码。');
        return false;
    } else {
        $("#login_button").attr('disabled', 'disabled').html("loading...");
        $.ajax({
            url: "/Account/CheckLogin",
            data: { account: $.trim($username.val()), password: $.trim($password.val()) },
            type: "post",
            dataType: "json",
            success: function (data) {
                if (data.state == "success") {
                    $("#login_button").html("登录成功，正在跳转...");
                    window.setTimeout(function () {
                        window.location.href = "/Home/Index";
                    }, 500);
                } else {
                    $("#login_button").removeAttr('disabled').html("登录");
                    $.login.formMessage(data.message);
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        });
    }
})
$.login = {
    formMessage: function (msg) {
        $('.login_tips').find('.tips_msg').remove();
        $('.login_tips').append('<div class="tips_msg"><i class="fa fa-question-circle"></i>' + msg + '</div>');
    }
}
