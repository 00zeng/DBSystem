var id = $.request("id");
$(function () {
    $("#company_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Company/GetIdNameCategoryList",
        param: { category: "分公司" },
        search: true,
        firstText: "--请选择所属分公司--",
        change: function () {
            GetCompanyAddr();
        }
    });
    $("#select2-company_id-container").width("220px"); // 改变所属机构显示的宽度，原select2插件生成的显示宽度仅150px。
})

function GetCompanyAddr() {
    var curCompany = $("#company_id").val();
    if (curCompany > 0) {
        $("#distributor_id").empty();
        $("#distributor_id").bindSelect({
            text: "name",
            url: "/DistributorManage/DistributorManage/GetIdNameList",
            param: { company_id: curCompany },
            firstText: "--请选择经销商--",
        });
    }
}

function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var data = $("#form1").formSerialize();
    data["inactive"] = $("#inactive").prop("checked");//是否启用
    var addrCity = $("#addrCity .select-item");
    data["city_code"] = $(addrCity).length > 0 ? $(addrCity).last().data("code") : "";

    $.submitForm({
        url: "/DistributorManage/ClerkManage/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/DistributorManage/ClerkManage/Index";

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
