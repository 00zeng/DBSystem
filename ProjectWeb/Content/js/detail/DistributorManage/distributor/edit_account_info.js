var id = $.request("id");
var account_inactive = "";

$(function () {
    $.ajax({
        url: "/DistributorManage/DistributorManage/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            console.log(data)
            if (!!data) {
                $("#form1").formSerializeShow(data.mainInfo);
                $("#account").text(data.accountInfo.account);
                name = data.mainInfo.name;
                if (data.accountInfo.inactive) {
                    $("#account_state").text("已停用");                   
                    $("#btnActive").val("启用账户").addClass("btn-primary");
                    account_inactive = true;
                }
                else {
                    $("#account_state").text("已启用");                  
                    $("#btnActive").val("停用账户").addClass("btn-danger");
                    account_inactive = false;
                }
           
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });

})


function SetState() {
    var hint = "确定要" + (account_inactive ? "启用“" : "停用“") + name + "”？";
    top.layer.confirm(hint, function (index) {
        var data = { id: id };
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        $.ajax({
            url: "/DistributorManage/DistributorManage/Active?date=" + new Date(),
            data: data,
            type: "post",
            dataType: "json",
            success: function (data) {
                if (data.state == "success") {
                    top.layer.closeAll();
                    if (account_inactive == true) {
                        $("#account_state").text("已启用");
                        $("#btnActive").removeClass("btn-primary");
                        $("#btnActive").val("停用账户").addClass("btn-danger");
                        account_inactive = false;
                    } else if (account_inactive == false) {
                        $("#account_state").text("已停用");
                        $("#btnActive").removeClass("btn-danger");
                        $("#btnActive").val("启用账户").addClass("btn-primary");
                        account_inactive = true;
                    }
                } else {
                    $.modalMsg(data.message, data.state);
                }
            },
            error: function (XMLHttpResponse, textStatus, errorThrown) {
                $.loading(false);
                $.modalMsg(errorThrown, "error");
            },
            beforeSend: function () {
                $.loading(true);
            },
            complete: function () {
                try {
                    $.loading(false);
                    //top.layer.close(index);
                }
                catch ($) { }
            }
        });

    })
}

function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var data = $("#form1").formSerialize();

    var newPassword = $("#newPassword").val();
    var newPassword2 = $("#newPassword2").val();
    if (newPassword != newPassword2) {
        $.modalAlert("两次密码输入不相同");
        return false;
    }

    data["id"] = id;

    $.submitForm({
        url: "/DistributorManage/DistributorManage/Edit?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/DistributorManage/DistributorManage/Index";

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
