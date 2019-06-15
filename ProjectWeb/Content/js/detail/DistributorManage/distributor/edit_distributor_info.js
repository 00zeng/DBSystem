var id = $.request("id");


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
                name_v2 = data.mainInfo.name_v2; 
                code = data.mainInfo.code;
                $("#city").val(data.mainInfo.city).trigger("change");
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

    data["potential_salepoint"] = $("input[name='potential_salepoint']:checked").val();
    data["id"] = id;
    var addrCity = $("#addrCity .select-item");
    data["city_code"] = $(addrCity).length > 0 ? $(addrCity).last().data("code") : "";

    if ($("#name").val() != name || $("#name_v2").val() != name_v2 || $("#code").val() != code) {
        top.layer.confirm("修改经销商名称、V2报量名称、快捷编码后，所有数据导入将以新的名称匹配，确定修改？", function (index) {
            data["name"] = $("#name").val();
            data["name_v2"] = $("#name_v2").val();
            data["code"] = $("#code").val();
            $.submitForm({
                url: "/DistributorManage/DistributorManage/Edit?date=" + new Date(),
                param: data,
                success: function (data) {
                    if (data.state == "success") {
                        top.layer.closeAll();
                        window.location.href = "/DistributorManage/DistributorManage/Index";
                    }
                },
                error: function (data) {
                    $.modalAlert(data.responseText, 'error');
                }
            })
        })
    } else {
        data["name"] = $("#name").val();
        data["name_v2"] = $("#name_v2").val();
        data["code"] = $("#code").val();
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
}
