var id = $.request("id");
$(function () {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        //async:false,
        success: function (data) {
            $("#form1").formSerialize(data.empInfo);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

function submitForm() {
    //if (!$("#form1").formValid())
    //    return false;
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["id"] = id;
    $.submitForm({
        url: "/HumanResource/EmployeeManage/EditInfo?date=" + new Date(),
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
