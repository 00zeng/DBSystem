function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var name = $("#name").val().trim();
    var data = { name: name, role_desc: $("#role_desc").val()};

    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/SystemManage/MsRole/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
               $.currentWindow().ReSearch();//更新
                top.layer.closeAll();
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
