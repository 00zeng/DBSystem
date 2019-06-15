//部门管理
var $table = $('#gridList');
var h = $(window).height() - 30;

$(function () {
    $("#queryForm").queryFormValidate();
    //1. 初始化Table
    initTable();

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
    {
        field: "button", title: '操作', align: 'center', width: 100,
        formatter: function (value, row, index) {
            var name = row.name;
            var id = row.id;
            var btnString = "";
            btnString = "<a name='show' class='btn btn-primary btn-xs auth'  href='/OrganizationManage/Department/Edit?id=" + row.id + "'>修改</a> "
            if (row.delible) {
                btnString += "<a name='Del' class='btn btn-danger btn-xs auth'  href='javascript:;' onclick='Delete(\""
                                        + id + "\", \"" + name + "\")'>删除</a> "
            }
            return btnString;
        }
    },
    { field: "name", title: "部门名称", align: "center", sortable: true, width: 150 },
   // { field: "manager", title: "部门经理", align: "center", sortable: true }, // TODO 点击跳转到员工信息
    { field: "grade_category_display", title: "部门类型", align: "center", sortable: true, width: 100 },
    { field: "parent_name", title: "上级部门", align: "center", sortable: true, width: 100 },
    { field: "company_linkname", title: "所属机构", align: "center", sortable: true, width: 180 },
    { field: "city", title: "所在省市", align: "center", sortable: true, width: 120 },
    { field: "address", title: "地址", align: "center", sortable: true, width: 150 },
    { field: "contact_phone", title: "联系电话", align: "center", sortable: true, width: 120 },
    { field: "note", title: "备注说明", align: "center", sortable: true, width: 100 },
    { field: "creator_name", title: "创建人", align: "center", sortable: true, width: 100 },
    { field: "create_time", title: "创建时间", align: "center", sortable: true, width: 100, formatter: (function (value) { var date = new Date(value); return date.pattern("yyyy-MM-dd"); }) },
        

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
            url: "/OrganizationManage/Department/GetGridJson?date=" + new Date(),         //请求后台的URL（*）
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
                    company_id: $('#company_id').val(),
                };
                return param;
            },
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
})

function OpenForm(url, title) {
    $.modalOpen({
        id: "Form",
        title: title,
        url: url,
        width: "680px",
        height: "630px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}

function Delete(id, name) {
    top.layer.confirm("确认要删除: " + name, function (index) {
        var data = {};
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["id"] = id;
        $.ajax({
            url: "/OrganizationManage/Department/Delete?date=" + new Date(),
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

//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}