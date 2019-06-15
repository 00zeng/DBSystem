var $table = $('#gridList');
var h = $(window).height() - 30;
var cur_id;
var cur_role_id;

var show_inactive = false;    // 初始不显示已注销
$("#role_id").bindSelect({
    text: "name",
    url: "/SystemManage/MsRole/GetSelectJson",
    search: true,
    firstText: "--请选择所属角色--"
});

$("#role_id_update").bindSelect({
    text: "name",
    url: "/SystemManage/MsRole/GetSelectJson",
    search: true,
    firstText: "--请选择所属角色--"
});
$(function () {
    $("#queryForm").queryFormValidate();
    //1. 初始化Table
    initTable();
    $table.bootstrapTable('hideColumn', 'inactive_name');
    $table.bootstrapTable('hideColumn', 'inactive_time');
    $("#querySubmit").click(querySubmit);
    $('#account_name').bind('input propertychange', function (event) {
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
    { field: "account", title: "账户名称", align: "center", sortable: true ,width:100},
    { field: "employee_name", title: "用户名称", align: "center", sortable: true, width: 100 },
    { field: "role_name", title: "角色", align: "center", sortable: true, width: 100 },
    { field: "last_login", title: "最后登录时间", align: "center", sortable: true, width: 150 },
    { field: "creator_name", title: "创建人", align: "center", sortable: true, width: 100 },
    { field: "reg_time", title: "创建时间", align: "center", sortable: true, width: 100, formatter: (function (value) { var date = new Date(value); return date.pattern("yyyy-MM-dd"); }) },
    { field: "inactive_name", title: "注销人", align: "center", sortable: true, width: 100 },
    { field: "inactive_time", title: "注销时间", align: "center", sortable: true, width: 150 },
    {
        field: "button", title: '操作', align: 'center', width: 180,
        formatter: function (value, row, index) {
            var btnString = "";
            var name = row.account;
            var id = row.id;
            if (show_inactive) {
                btnString = "<a name='authActive' class='btn btn-info btn-xs auth' href='javascript:;' onclick='SetState(\"" + id + "\", \"" + name + "\")'>启用</a> "
                        + "<a name='authDel' class='btn btn-danger btn-xs auth' href='javascript:;' onclick='Delete(\"" + id + "\", \"" + name + "\")'>删除</a>";
            }
            else {
                btnString = "<a name='authUpdateRole' class='btn btn-primary btn-xs auth' data-toggle='modal' data-target='#UpdateRole' onclick='UpdateRole(\"" + id + "\",\"" + row.role_id + "\",\"" + name + "\")'>更换角色</a> "
                   + "<a name='authPass' class='btn bg-purple btn-xs auth' data-toggle='modal' data-target='#ResetPassword' onclick='ResetPassword(\"" + id + "\",\"" + name + "\")'>重置密码</a>  "
                   + "<a name='authActive' class='btn btn-xs btn-danger auth' href='javascript:;' onclick='SetState(\"" + id + "\", \"" + name + "\")'>注销</a>";
                //btnString = "<a name='authUpdateRole' class='btn btn-primary btn-xs auth' href='javascript:;' onclick='OpenForm(\"/SystemManage/MsAccount/UpdateRole?id="
                //        + id + "&role_id=" + row.role_id + "\",\"账号：" + name + "\")'>更换角色</a> "
                //        + "<a name='authPass' class='btn bg-purple btn-xs auth' href='javascript:;' onclick='OpenForm(\"/SystemManage/MsAccount/ResetPassword?id="
                //        + id + "\",\"账号：" + name + "\")'>重置密码</a> "
                //        + "<a name='authActive' class='btn btn-xs btn-danger auth' href='javascript:;' onclick='SetState(\"" + id + "\", \"" + name + "\")'>注销</a>";
            }
            return btnString;
        }
    },
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
            url: "/SystemManage/MsAccount/GetGridJson?date=" + new Date(),         //请求后台的URL（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            //search: true,
            searchOnEnterKey: false,
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageSize: 20,                       //每页的记录行数（*）
            pageList: [20, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            strictSearch: false,
            showColumns: true,                  //是否显示所有的列 
            showRefresh: true,                  //是否显示刷新按钮
            showExport: true,
            exportTypes: ['xlsx'],  //导出文件类型  
            exportDataType: "all",
            exportOptions: {
               // ignoreColumn: [0, 1],  //忽略某一列的索引  
                fileName: '贷款总表',  //文件名称设置  
                worksheetName: 'sheet1',  //表格工作区名称  
                tableName: '贷款总表',
              //  excelstyles: ['background-color', 'color', 'font-size', 'font-weight'], //设置格式             
            },

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
                    account_name: $('#account_name').val().trim(),
                    employee_name: $('#employee_name').val().trim(),
                    role_name: $('#role_name').val().trim(),
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
    show_inactive = $("#inactive").prop('checked');
    if (show_inactive) {
        $table.bootstrapTable('showColumn', 'inactive_name');
        $table.bootstrapTable('showColumn', 'inactive_time');
    }
    else {
        $table.bootstrapTable('hideColumn', 'inactive_name');
        $table.bootstrapTable('hideColumn', 'inactive_time');
    }
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
        height: "340px",
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
            url: "/SystemManage/MsAccount/Active?date=" + new Date(),
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
    })
}
//删除
function Delete(id, name) {
    top.layer.confirm("确认要删除: “" + name + "”？", function (index) {
        var data = { id: id };
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }

        $.submitForm({
            url: "/SystemManage/MsAccount/Delete?date=" + new Date(),
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
    })
}
/*新增账户*/
function addRole() {
    if (!$("#form1").formValid())
        return false;
    //验证用户名  账号
    var account = $("#account").val().trim();
    if (!!account) {
        if (!/^[\w\u3E00-\u9FA5]+$/g.test(account)) {
            $.modalAlert("账号为由字母、数字、_或汉字组成", "error");
            return false;
        }
    }
    //确认密码
    var password = $("#password").val();
    var password2 = $("#password2").val();
    if (password2 != password) {
        $.modalAlert("两次密码输入不一致，请核实之后重新输入！", "error");
        return false;
    }
    var data = { account: account, role_id: $("#role_id").val(), password: password.trim() };
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/SystemManage/MsAccount/Add?date=" + new Date(),
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

/*更换角色*/
function UpdateRole(id, role_id, name) {
    cur_id = id;
    cur_role_id = role_id;
    $("#org_role_name").text(name);

}
/*更换角色*/
function submitForm() {
    if (!$("#form2").formValid())
        return false;
    var role_id_update = $("#role_id_update").val();
    if (role_id_update == "") {
        $.modalAlert("请选择角色", "error");
        return false;
    }
    var data = { id: cur_id, role_id: role_id_update };
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/SystemManage/MsAccount/UpdateRole?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                $('#UpdateRole').modal('hide');
                $table.bootstrapTable('refresh');
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
/*重置密码*/
//var passward_id;
function ResetPassword(id,  name) {
    cur_id = id;
    $("#org_role_name1").text(name);

}
/*重置密码*/
function ResetSubmit() {
    if (!$("#form3").formValid())
        return false;
    var newPassword = $("#newPassword").val();
    var newPassword2 = $("#newPassword2").val();
    if (newPassword != newPassword2) {
        top.layer.alert("两次密码输入不相同");
        return false;
    }
    var data = { id: cur_id, new_password: newPassword.trim() };
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/SystemManage/MsAccount/ResetPassword?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                $('#ResetPassword').modal('hide');
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