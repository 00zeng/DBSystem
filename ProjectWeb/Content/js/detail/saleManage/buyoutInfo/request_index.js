//买断信息
var $table = $('#gridList');
var h = $(window).height() - 30;

var isMyRequest = false;

$(function () {
    $("#queryForm").queryFormValidate();
    //1. 初始化Table
    initTable();

    $("#querySubmit").click(querySubmit);
    $('#distributorName').bind('input propertychange', function (event) {
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
    { field: "distributor_name", title: "经销商", align: "center", sortable: true },
    { field: "sales_name", title: "业务员", align: "center", sortable: true },
    {
        field: "category", title: "买断类型", align: "center", sortable: true,
        formatter: function (cellvalue) {
            if (cellvalue == 1)
                return "仓库买断";
            else
                return "门店买断";
        }
    },
    {
        field: "approve_status", title: "审批状态", align: "center", sortable: true,
        formatter: function (cellvalue) {
            if (cellvalue < 0)
                return "<span class='label label-danger'>退审</span>";
            else if (cellvalue == 100)
                return "<span class='label label-success'>通过</span>";
            else if (cellvalue == 0)
                return "<span class='label label-warning'>未审批</span>";
            else
                return "<span class='label label-info'>审批中</span>";
        }
    },
    { field: "create_time", title: "申请时间", align: "center", sortable: true },
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
            url: "/SaleManage/BuyoutInfo/GetListRequest?date=" + new Date(),        //请求后台的URL（*）
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
                    distributorName: $('#distributorName').val().trim(),
                    salesName: $('#salesName').val().trim(),
                    isMyRequest: $("#isMyRequest").prop('checked'),
                };
                return param;
            },
            onClickRow: function (row) {
                location.href = "/SaleManage/BuyoutInfo/Show?id=" + row.id;
            }
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
    $("input[name='isMyRequest']").prop("checked", false);
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

$("#inactive").on("change", function () {
    isMyRequest = $("#inactive").prop("checked");
})
 


//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}