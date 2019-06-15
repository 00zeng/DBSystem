var id = $.request("id");
var name = decodeURI($.request("name"));
var role_desc = decodeURI($.request("role_desc"));
$(function () {
    $("#name").val(name);
    $("#role_desc").val(role_desc);
});
function submitForm() {
    name = $("#name").val().trim();
    var data = { id: id, name: name, role_desc: role_desc };
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/SystemManage/MsRole/Edit?date=" + new Date(),
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
