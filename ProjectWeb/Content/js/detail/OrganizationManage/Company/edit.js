var id = $.request("id");
var name = "";
$(function () {
    $.ajax({
        url: "/OrganizationManage/Company/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            if (!!data)
            {
                $("#form1").formSerialize(data);
                name = data.name;
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
    if ($("#name").val() != name) {
        top.layer.confirm("修改分公司名称后，所有数据导入将以新的名称匹配，确定修改？", function (index) {
            data["name"] = $("#name").val();
            $.submitForm({
                url: "/OrganizationManage/Company/Edit?date=" + new Date(),
                param: data,
                success: function (data) {
                    if (data.state == "success") {
                        top.layer.closeAll();
                        window.location.href = "/OrganizationManage/Company/Index";
                    }
                },
                error: function (data) {
                    $.modalAlert(data.responseText, 'error');
                }
            })
        })
    } else {
        data["name"] = $("#name").val();
        $.submitForm({
            url: "/OrganizationManage/Company/Edit?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    window.location.href = "/OrganizationManage/Company/Index";
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    }
}
