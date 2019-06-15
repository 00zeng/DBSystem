var id = $.request("id");
var clerk_name;
$(function () {
    $.ajax({
        url: "/DistributorManage/ClerkManage/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            if (!!data[0]) {
                $("#form1").formSerialize(data[0]);
                $("#form1").formSerializeShow(data[0]);
                $("#clerk_name").text(data[0].name);
                $("#city").val(data[0].city).trigger("change");
                if (account_state == 0) {
                    $("#account_state").text("已启用");
                }
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
    var data = $("#form1").formSerialize();
    data["id"] = id;
    var addrCity = $("#addrCity .select-item");
    data["city_code"] = $(addrCity).length > 0 ? $(addrCity).last().data("code") : "";

    $.submitForm({
        url: "/DistributorManage/ClerkManage/Edit?date=" + new Date(),
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
