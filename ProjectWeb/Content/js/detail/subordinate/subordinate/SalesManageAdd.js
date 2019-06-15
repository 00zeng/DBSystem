var id = $.request("id");
var company_id = $.request("company_id");
$(function () {
    $("#area_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Area/GetIdNameList",
        param: { company_id: company_id },
        firstText: "--请选择区域--",
        search: true,
    });
    $("#select2-company_id-container").width("210px"); // 改变所属机构显示的宽度，原select2插件生成的显示宽度仅150px。
})
function submitForm() {
    var data = $("#form1").formSerialize();
    data["area_id"] = $("#area_id").val();
    data["emp_id"] = id;
    $.submitForm({
        url: "/SubordinateManage/MySubordinate/SalesManageAdd?date=" + new Date(),
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
