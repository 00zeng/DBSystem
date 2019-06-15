var $table = $('#gridList');
var h = $(window).height() - 30;

var show_inactive = false;    // 初始不显示已注销
var cur_id;
$(function () {
    $("#queryForm").queryFormValidate();
    //1. 初始化Table
    initTable();
    $table.bootstrapTable('hideColumn', 'inactive_name');
    $table.bootstrapTable('hideColumn', 'inactive_time');
    $("#querySubmit").click(querySubmit);
    $('#name').bind('input propertychange', function (event) {
        $table.bootstrapTable({ offset: 0 }); // 第一页
        $table.bootstrapTable('removeAll');
        $table.bootstrapTable('refresh');

    });

    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
})
var tableHeader = [
    { field: "name", title: "角色名称", align: "center", sortable: true },
    { field: "role_desc", title: "角色描述", align: "center", sortable: true },
    { field: "creator_name", title: "创建人", align: "center", sortable: true },
    { field: "create_time", title: "创建时间", align: "center", sortable: true },
    { field: "inactive_name", title: "注销人", align: "center", sortable: true },
    { field: "inactive_time", title: "注销时间", align: "center", sortable: true },
    {
        field: "button", title: '操作', align: 'center',
        formatter: function (value, row, index) {
            var btnString = "";
            var name = row.name;
            var id = row.id;
            if (show_inactive) {
                btnString = "<a name='authActive' class='btn btn-info btn-xs auth' href='javascript:;' onclick='SetState(\"" + id + "\", \"" + name + "\")'>启用</a>"
                        + "<a name='authDel' class='btn btn-danger btn-xs auth' href='javascript:;' onclick='Delete(\"" + id + "\", \"" + name + "\")'>删除</a> ";
            }
            else {
                //btnString = "<a name='authEdit' class='btn btn-primary btn-xs auth' href='javascript:;' onclick='OpenForm(\"/SystemManage/MsRole/Edit?id="
                //        + id + "&name=" + encodeURI(encodeURI(name)) + "&role_desc=" + encodeURI(encodeURI(row.role_desc)) + "\",\"角色：" + name + "\")'>修改</a> "
                //        + "<a name='authAuthEdit' class='btn bg-purple btn-xs auth' href='/SystemManage/MsRole/AuthorityEdit?id=" + id + "&name=" + encodeURI(encodeURI(name)) + "'>权限设置</a> "
                //        + "<a name='authActive' class='btn btn-xs btn-danger auth' href='javascript:;' onclick='SetState(\"" + id + "\", \"" + name + "\")'>注销</a>";
                btnString = "<a name='authEdit' class='btn btn-primary btn-xs auth'  data-toggle='modal' data-target='#Edit' onclick='editRole(\"" + id + "\",\"" + name + "\",\"" + row.role_desc + "\")'>修改</a> "
                        + "<a name='authAuthEdit' class='btn bg-purple btn-xs auth' href='/SystemManage/MsRole/AuthorityEdit?id=" + id + "&name=" + encodeURI(encodeURI(name)) + "'>权限设置</a> "
                        + "<a name='authActive' class='btn btn-xs btn-danger auth' href='javascript:;' onclick='SetState(\"" + id + "\", \"" + name + "\")'>注销</a>";
            }
            return btnString;
        }
    }
]

function initTable() {
    //1.初始化Table
    var oTable = new TableInit();
    oTable.Init();
}
var TableInit = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            height: h,
            url: "/SystemManage/MsRole/GetGridJson?date=" + new Date(),         //请求后台的URL（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageSize: 20,                       //每页的记录行数（*）
            pageList: [20, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            strictSearch: true,
            showColumns: true,                  //是否显示所有的列 
            showRefresh: true,                  //是否显示刷新按钮
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableHeader,
            queryParams: function (params) {
                var param = {
                    page: (params.offset / params.limit) + 1,
                    rows: params.limit,
                    sidx: params.sort,
                    sord: params.order,
                    name: $('#name').val().trim(),
                    inactive: $("#inactive").prop('checked'),
                };
                return param;
            },
        });
    };
    return oTableInit;
};

function querySubmit() {
    $table.bootstrapTable({ offset: 0 }); // 第一页   
    $table.bootstrapTable('removeAll');
    $table.bootstrapTable('refresh');
    $('#modalQuery').modal('hide')
}


//清空筛选
$("#clear").click(function () {
    $(".modal-body input").each(function () {
        $(this).val('');
    });
    $(".modal-body select").each(function () {
        $(this).val('').trigger("change");
    });
    $("input[name='inactive']").prop("checked", false);
})

function OpenForm(url, title) {
    $.modalOpen({
        id: "Form",
        title: title,
        url: url,
        width: "600px",
        height: "300px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}

function SetState(id, name) {
    var hint = "确定要" + (show_inactive ? "启用“" : "注销“") + name + "”？";
    top.layer.confirm(hint, function (index) {
        var data = { id: id };
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        $.submitForm({
            url: "/SystemManage/MsRole/Active/?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    $.currentWindow().location.reload();
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}

/*删除数据*/
function Delete(id, name) {
    top.layer.confirm("确认要删除: “" + name + "”？", function (index) {
        var data = {};

        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["id"] = id;

        $.ajax({
            url: "/HumanResource/Leaving/Delete?date=" + new Date(),
            type: "post",
            data: data,
            success: function (data) {
                $.currentWindow().location.reload();
                top.layer.msg("删除成功");
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}
/*新增角色*/
function addRole() {
    if (!$("#form1").formValid())
        return false;
    var name = $("#role_name").val().trim();
    var data = { name: name, role_desc: $("#role_desc").val() };

    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/SystemManage/MsRole/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                $('#Add').modal('hide');
                $table.bootstrapTable('refresh');
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
/*修改角色*/
function editRole(id, name, role_desc) {
    cur_id = id;
    $("#org_name").text(name);
    $("#role_name_show").val(name);
    $("#role_desc_show").val(role_desc);
   
}
/*修改角色*/
function submitForm() {
    //name = $("#role_name_show").val().trim();
    var data = { id: cur_id, name: $("#role_name_show").val(), role_desc: $("#role_desc_show").val() };
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/SystemManage/MsRole/Edit?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                $('#Edit').modal('hide');
                $table.bootstrapTable('refresh');
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}

