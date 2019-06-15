//权限设置
var id = $.request("id");
var name = decodeURI($.request("name"));
$(function () {
    $("#role_name").text(name);
    initControl();
});
function initControl() {
    //获取权限表
    $("#auth_tree").authTree({
        //height: 444,
        showcheck: true,
        url: "/SystemManage/MsRole/GetAuthorityTree?date=" + new Date(),
        param: { role_id: id }
    });
}
function submitForm() {
    var strSelect = String($("#auth_tree").getCheckedNodes());
    var $id = $("#auth_tree");
    var _length = $id.attr('id').trim().length + 1;
    var auth_list = [];
    $id.find('.bbit-tree-node-cb').each(function () {
        var _src = $(this).attr('src');
        _src = _src.substr(_src.lastIndexOf("/") + 1);
        var _value = $(this).attr('id').substring(parseInt(_length)).replace(/_/g, "-");
        _value = _value.substring(0, _value.length - 3);
        if (_src != 'checkbox_0.png') {//已选中
            var json = { "role_id": id, "value_id": _value, "type": $(this).data("category") };
            auth_list.push(json);
        }
    });
    var data = {};
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["role_id"] = id;
    data["auth_list_str"] = JSON.stringify(auth_list);
    $.submitForm({
        url: "/SystemManage/MsRole/EditAuthority?date=" + new Date(),
        param: data,
        success: function (data) {
            window.location.href = "/SystemManage/MsRole/Index";
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}