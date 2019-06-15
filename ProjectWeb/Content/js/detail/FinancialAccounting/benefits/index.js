//员工福利
var $table = $('#gridList');
var h = $(window).height() - 30;

var show_inactive = false;    // 初始不显示已注销
$(function () {

    //1. 初始化Table
    initTable();
    $("#querySubmit").click(querySubmit);
    $("#approve_status").change(querySubmit);

    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
})
var tableHeader = [
    {
        field: "", title: "操作", align: "center", sortable: true, width: 80,
        formatter: function (value, row) {
            return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/Benefits/Show?id=" + row.id + "'title='查看详情'><i class='fa fa-search'></i></a>";
        }
    },
    {
        field: "company_name", title: "机构", align: "center", sortable: true,     
    },
    { field: "count", title: "发放人数", align: "center", sortable: true },   
    {
        field: "month", title: "福利月份", align: "center", sortable: true,
        formatter: (function (value, row) {
                var date = new Date(value);
                return date.pattern("yyyy-MM");
        })
    },
     {
         field: "approve_status", title: "审批状态", align: "center", sortable: true,
         formatter: function (value) {
             if (value == 0) {
                 return "<span class='label label-warning'>未审批</span>";
             } else if (value == 100) {
                 return "<span class='label label-success'>通过</span>";
             } else if (value < 0) {
                 return "<span class='label label-danger'>未通过</span>";
             } else {
                 return "<span class='label label-info'>审批中</span>";
             }
         }
     },
    { field: "creator_name", title: "发起人", align: "center", sortable: true },
    { field: "create_time", title: "发起时间", align: "center", sortable: true, },
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
            url: "/FinancialAccounting/Benefits/GetGridJson?date=" + new Date(),        //请求后台的URL（*）
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
                var pageInfo = {
                    rows: params.limit,                              //页面大小
                    page: (params.offset / params.limit) + 1,        //页码
                    sidx: params.sort,
                    sord: params.order,
                    company_id: $('#company_id').val(),
                    approve_status: $('#approve_status').val(),
                };
                return pageInfo;
            },
            onClickRow: function (row) {
                location.href = "/FinancialAccounting/Benefits/Show?id=" + row.id;
            }
        });
    };
    return oTableInit;
};

$("#company_id").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameCategoryList",
    search: true,
    firstText: "--请选择机构--",
});

//清空筛选
$("#clear").click(function () {
    $(".modal-body input").each(function () {
        $(this).val('');
    });
    $(".modal-body select").each(function () {
        $(this).val('').trigger("change");
    });
})

function querySubmit() {
    $table.bootstrapTable({ offset: 0 }); // 第一页
    $table.bootstrapTable('removeAll');
    $table.bootstrapTable('refresh');
    $('#modalQuery').modal('hide')
}
//初始化页面上面的按钮事件
var ButtonInit = function () {
    var oInit = new Object();
    var postdata = {};

    oInit.Init = function () {
        $('#btn_add').click(addcolumns);
        $('#btn_edit').click(editcolumns);
        $('#btn_delete').click(deletecolumns);
    };
    return oInit;
};

function addcolumns() {
    var table = $table.bootstrapTable('getData'),
        length = table.length;;
    var type = $('#identitys input:radio:checked').val();
    var empty = cloneObj(configjson[type].data);
    empty.id = length + 1;

    $table.bootstrapTable('load', table);
    $table.bootstrapTable('selectPage', 1); //Jump to the first page
    $table.bootstrapTable('prepend', empty);

}


function deletecolumns() {
    var obj = $table.bootstrapTable('getSelections');
    var ids = $.map(obj, function (row) {
        return row.id;
    });
    if (ids.length > 0) {
        $table.bootstrapTable('remove', { field: 'id', values: ids });
    } else {
        alert("请至少选择一行删除")
    }
}

function editcolumns() {
    $table.find('.editable').editable('toggleDisabled');
}

function removeData(index) {
    $table.bootstrapTable('remove', { field: 'id', values: [index] });
}

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

//重新查询
function ReSearch() {
    $gridList.jqGrid('setGridParam', {
        postData: { employee_name: $("#employee_name").val(), account: $("#account").val(), inactive: show_inactive }, page: 1
    }).trigger('reloadGrid');
}