var $table = $('#gridList');
var h = $(window).height() - 30;
var SelectNum = 0;
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

    $("#exportExcel").click(Export);
    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
    ActionBinding();
    //GetAreaL1List();
})
// 绑定标签事件
function ActionBinding() {
    $("#company_id2").on("change", function (e) {
        GetAreaL1List();
    });
    $("#area_l1_id").on("change", function (e) {
        GetAreaL2List();
    });
}
var tableHeader = [
    {checkbox:true},
    {
        field: "", title: "操作", align: "center",width: 80,
        formatter: (function (value, row) {
            return "<a class='btn btn-primary btn-xs' href='/DistributorManage/DistributorManage/Show?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
        })
    },
    { field: "name", title: "经销商名称", align: "center", sortable: true, width: 200 },
    { field: "name_v2", title: "V2报量名称", align: "center", sortable: true, width: 180 },
    { field: "code", title: "快捷编码", align: "center", sortable: true, width: 120 },
    { field: "account", title: "账户名称", align: "center", sortable: true, width: 120 },
    {
        field: "account_status", title: "账户状态", align: "center",   sortable: true, width: 100,
        formatter: (function (value) {
            return (value ? "停用" : "启用");
        })
    },
    { field: "area_l2_name", title: "所属区域", align: "center", sortable: true, width: 150 },
    { field: "company_linkname", title: "所属机构", align: "center", sortable: true, width: 180 },
    { field: "city", title: "所在省市", align: "center", sortable: true, width: 220 },
    { field: "address", title: "详细地址", align: "center", sortable: true, width: 230 },
    { field: "distributor_attribute", title: "属性", align: "center", sortable: true, width: 100 },
    { field: "sp_attribute", title: "运营商属性", align: "center", sortable: true, width: 120 },
    //{ field: "creator_name", title: "创建人", align: "center", sortable: true, width: 120 },
    //{ field: "create_time", title: "创建时间", align: "center",   sortable: true, width: 150 },
   
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
            url: "/DistributorManage/DistributorManage/GetGridJson?date=" + new Date(),        //请求后台的URL（*）
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
            checkbox:true,
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
                    area_l2_id: $('#area_l2_id').val(),
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
//更多搜索 - 分公司
$("#company_id").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameList",
    search: true,
    firstText: "--请选择分公司--",
});

$("#company_id").on("change", function () {
    $("#branchcompany_id").empty();
    $("#area_l2_id").empty();
    $("#area_l2_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Area/GetIdNameList",
        param: { company_id: $("#company_id").val() },
        firstText: "--请选择区域--",
    });
})
//区域调整 - 分公司
$("#company_id2").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameList",
    search: true,
    firstText: "--请选择机构--",
});
//区域调整 - 经理片区
function GetAreaL1List() {
    var curCompany = $("#company_id2").val();
    $("#area_l1_id").empty();
    if (curCompany > 0) {
        $("#area_l1_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Area/GetManagerAreaIdNameList",
            param: { company_id: curCompany },
            firstText: "--请选择经理片区--",
        });
    }
}
//区域调整 - 业务片区
function GetAreaL2List() {
    var areaL1Id = $("#area_l1_id").val();
    $("#area_l2_id2").empty();
    if (areaL1Id > 0) {
        $("#area_l2_id2").bindSelect({
            text: "name",
            url: "/OrganizationManage/Area/GetSalesAreaIdNameList",
            param: { id: areaL1Id },
            firstText: "--请选择业务片区--",
        });
    }
}
function deleteColumns() {
    var obj = $table.bootstrapTable('getSelections');
    var idArray = $.map(obj, function (row) {
        return row.id;
    });
    if (idArray.length <= 0) {
        alert("请至少选择一个经销商注销");
        return;
    } else {
        top.layer.confirm("确认要注销选中的经销商吗?注意此操作不可逆！", function (index) {
            var data = { idArray: idArray, effect_date: $("#effect_date").val() };
            $.submitForm({
                url: "/DistributorManage/DistributorManage/Delete",
                param: data,
                success: function (data) {
                    if (data.state == "success") {
                        top.layer.closeAll();
                        $('#modalDel').modal('hide');
                        //删除成功 checkbox 重置
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
function AjustDistributor() {
    var obj = $table.bootstrapTable('getSelections');
    var idArray = $.map(obj, function (row) {
        return row.id;
    });
    if (idArray.length <= 0) {
        alert("请至少选择一个经销商调区！");
        return;
    } else {
        top.layer.confirm("确定要将选中的经销商进行调区吗?", function (index) {
            var date = {};
            $.submitForm({
                url: "/DistributorManage/DistributorManage/Ajust",
                param: { idArray: idArray, effect_date: $("#effect_date2").val(), company_id: $("#company_id2").val(), area_l1_id: $("#area_l1_id").val(), area_l2_id: $("#area_l2_id2").val() },
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
        width: "600px",
        height: "300px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}

//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}

//导出
function Export() {
    window.location.href = "/DistributorManage/DistributorManage/Export?company_id=" + $('#company_id').val()
            + "&area_l2_id=" + $('#area_l2_id').val()
            + "&name=" + $('#name').val().trim()           
}
