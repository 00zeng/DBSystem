var id = $.request("id");
$(function () {
    $.ajax({
        url: "/OrganizationManage/Department/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            if (!!data) {
                $("#form1").formSerialize(data);
                $("#city").val(data.city).trigger("change");
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var data = {};
    data["id"] = id;
    data["city"] = $("#city").val();
    data["address"] = $("#address").val();
    data["contact_phone"] = $("#contact_phone").val();
    data["note"] = $("#note").val();

    var addrCity = $("#addrCity .select-item");
    data["city_code"] = $(addrCity).length > 0 ? $(addrCity).last().data("code") : "";

    $.submitForm({
        url: "/OrganizationManage/Department/Edit?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/OrganizationManage/Department/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
