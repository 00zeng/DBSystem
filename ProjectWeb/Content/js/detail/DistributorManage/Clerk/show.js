var id = $.request("id");
var account_inactive = true;
var name = "";
$(function () {
    $.ajax({
        url: "/DistributorManage/ClerkManage/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            if (!!data[0]) {
                $("#form1").formSerializeShow(data[0]);
                name = data[0].name;
                $("#clerk_name").text(name);
                if (data[0].account_state) {
                    $("#account_state").text("未启用");
                    $("#btnActive").val("账户启用");
                    account_inactive = true;
                }
                else {
                    $("#account_state").text("已启用");
                    $("#btnActive").val("账户停用");
                    account_inactive = false;
                }
                if (data[0].delible)
                    $("#btnDel").show();

                if (!(data[0].inactive))
                    $("#btnClose").show(); // TODO 状态显示
                else {
                    $("#btnEdit").css("display", "none");
                    $("#btnActive").css("display", "none");
                    $("#account_tr1").css("display", "none");
                    $("#account_tr2").css("display", "none");
                    $("#inactive_close_tr").css("display", "");
                }
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

function Edit() {
    window.location.href = "/DistributorManage/ClerkManage/Edit?id=" + id;
}

function SetClose() {
    var hint = "“" + name + "”将离职，此操作不可撤消！";
    top.layer.confirm(hint, function (index) {
        var data = { id: id };
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        $.submitForm({
            url: "/DistributorManage/ClerkManage/Resign?id=" +id,
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    top.layer.closeAll();
                    setTimeout("location.reload();", 1000)
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}

function SetState() {
    var hint = "确定要" + (account_inactive ? "启用“" : "停用“") + name + "”？";
    top.layer.confirm(hint, function (index) {
        var data = { id: id };
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        $.submitForm({
            url: "/DistributorManage/ClerkManage/Active?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    top.layer.closeAll();
                    setTimeout("location.reload();", 1000)
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}

/*删除数据*/
function Delete() {
    top.layer.confirm("确认要删除 " + name, function (index) {
        var data = {};
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }

        data["id"] = id;
        $.ajax({
            url: "/DistributorManage/ClerkManage/Delete?date=" + new Date(),
            type: "post",
            data: data,
            success: function (data) {
                top.layer.closeAll();
                window.location.href = "/DistributorManage/ClerkManage/Index";
            },
                error: function (data) {
                    $.modalAlert(data.responseText, 'error');
                }
        })
    })
}
