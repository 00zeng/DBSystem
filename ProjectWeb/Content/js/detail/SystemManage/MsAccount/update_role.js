var id = $.request("id");
var role_id = $.request("role_id");
$(function () {
    $("#role_id").bindSelect({
        text: "name",
        url: "/SystemManage/MsRole/GetSelectJson",
        search: true,
        firstText: "--请选择所属角色--"
    });
    $("#role_id").val(role_id);
});
function submitForm() {
    role_id = $("#role_id").val();
    if (role_id == "") {
        $.modalAlert("请选择角色", "error");
        return false;
    }
    var data = { id: id, role_id: role_id };
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/SystemManage/MsAccount/UpdateRole?date=" + new Date(),
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
