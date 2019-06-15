//区域管理
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
    { checkbox: true },
     {
         field: "button", title: '操作', align: 'center', width: 80,
         formatter: function (value, row, index) {
             var name = row.name;
             var id = row.id;
             var btnString = "";
             btnString = "<a name='show' class='btn btn-primary btn-xs auth'  href='/OrganizationManage/Area/Edit?id=" + row.id + "'>修改</a> "
             if (row.delible) {
                 btnString += "<a name='Del' class='btn btn-danger btn-xs auth'  href='javascript:;' onclick='Delete(\""
                                         + id + "\", \"" + name + "\")'>删除</a> "
             }
             return btnString;
         }
     },
    { field: "name", title: "区域名称", align: "center", sortable: true, width: 150 },   
    { field: "company_linkname", title: "所属机构", align: "center", sortable: true, width: 180 },
    { field: "city", title: "所在省市", align: "center", sortable: true, width: 120 },
    { field: "address", title: "地址", align: "center", sortable: true, width: 150 },
    { field: "note", title: "备注说明", align: "center", sortable: true, width: 100 },
    //{ field: "creator_name", title: "创建人", align: "center", sortable: true, width: 100 },
    //{ field: "create_time", title: "创建时间", align: "center", sortable: true, width: 100, formatter: (function (value) { var date = new Date(value); return date.pattern("yyyy-MM-dd"); }) },

]

function initTable() {
    //1.初始化Table
    var oTable = new TableInit();
    oTable.Init();
}
var SelectNum = 0;
var TableInit = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            height: h,
            url: "/OrganizationManage/Area/GetAreaList?date=" + new Date(),         //请求后台的URL（*）
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
            checkbox: true,
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
                    type: 1,
                    company_id: $('#company_id').val(),
                };
                return param;
            },
            onCheck: function (row) {
                SelectNum++;
                if (SelectNum > 0) {
                    $("button[name=orgHide]").css("display", "");
                    $("button[name=orgShow]").css("display", "none");
                    $("div[name=orgShow]").css("display", "none");
                }
            },
            onUncheck: function (row) {
                SelectNum--;
                if (SelectNum == 0) {
                    $("button[name=orgHide]").css("display", "none");
                    $("button[name=orgShow]").css("display", "");
                    $("div[name=orgShow]").css("display", "");
                }
            },
            onCheckAll: function (rows) {
                SelectNum = rows.length;
                if (SelectNum > 0) {
                    $("button[name=orgHide]").css("display", "");
                    $("button[name=orgShow]").css("display", "none");
                    $("div[name=orgShow]").css("display", "none");
                }
            },
            onUncheckAll: function (rows) {
                SelectNum = SelectNum - rows.length;
                if (SelectNum == 0) {
                    $("button[name=orgHide]").css("display", "none");
                    $("button[name=orgShow]").css("display", "");
                    $("div[name=orgShow]").css("display", "");
                }
            },
        });
    };
    return oTableInit;
};

$("#company_id").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameList",
    search: true,
    firstText: "--请选择分公司--",
});
//调整 - 分公司
$("#company_id2").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameList",
    search: true,
    firstText: "--请选择分公司--",
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
        height: "580px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}

function deleteColumns() {
    var obj = $table.bootstrapTable('getSelections');
    var idArray = $.map(obj, function (row) {
        return row.id;
    });
    if (idArray.length <= 0) {
        alert("请至少选择一行删除");
        return;
    } else {
        top.layer.confirm("确认要删除这些数据吗?", function (index) {
            var date = {};
            $.submitForm({
                url: "/OrganizationManage/Area/Delete",
                param: { idArray: idArray, effect_date: $("#effect_date").val() },
                success: function (data) {
                    if (data.state == "success") {
                        top.layer.closeAll();
                        $('#modalDel').modal('hide');
                        //调区成功 checkbox 重置
                        SelectNum = 0;
                        $("button[name=orgHide]").css("display", "none");
                        $("button[name=orgShow]").css("display", "");
                        $("div[name=orgShow]").css("display", "");
                        $table.bootstrapTable('refresh');
                    }
                },
                error: function (data) {
                    $.modalAlert(data.responseText, 'error');
                }
            })
        })
    }
}
function AjustL1() {
    var obj = $table.bootstrapTable('getSelections');
    var idArray = $.map(obj, function (row) {
        return row.id;
    });
    if (idArray.length <= 0) {
        alert("请至少选择一个区域调整！");
        return;
    } else {
        top.layer.confirm("确定要调整区域吗?", function (index) {
            var date = {};
            $.submitForm({
                url: "/OrganizationManage/Area/AjustL1",
                param: { idArray: idArray, effect_date: $("#effect_date2").val(), company_id: $("#company_id2").val()},
                success: function (data) {
                    if (data.state == "success") {
                        top.layer.closeAll();
                        $('#modalChange').modal('hide');
                        //调区成功 checkbox 重置
                        SelectNum = 0;
                        $("button[name=orgHide]").css("display", "none");
                        $("button[name=orgShow]").css("display", "");
                        $("div[name=orgShow]").css("display", "");
                        $table.bootstrapTable('refresh');
                    }
                },
                error: function (data) {
                    $.modalAlert(data.responseText, 'error');
                }
            })
        })
    }
}

//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}