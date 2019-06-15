
//串码汇总  不用父子表
var $table = $('#gridList');
var h = $(window).height() - 30;
var now = (new Date()).pattern("yyyy-MM-dd");
var day1st = now.substring(0, 8) + "01";
$(function () {
    $("#queryForm").queryFormValidate();
    $('#startTime1').val(day1st);
    $('#endTime1').val(now);
    //1. 初始化Table
    initTable();

    $("#querySubmit").click(querySubmit);
    $("#querySubmit1").click(querySubmit);

    $("#exportExcel").click(Export);
    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
})

var tableHeader = [
    {
        field: "check", checkbox: true,
        formatter: function (value, row, index) {
            if (row.sale_type == 1) {//退库不可选
                return {
                    disabled: true,//设置是否可用
                    checked: false,//设置不选中
                };
            }
        },
    },
    {
        field: "phone_sn", title: "串码", align: "center", sortable: true, width: 150,
    },
    { field: "model", title: "型号", align: "center", sortable: true, width: 150 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 100 },
    {
        field: "sale_type", title: "销售状态", align: "center", sortable: true, width: 100,
        formatter: function (value) {
            if (value == -1) {
                return "已退库"
            } else if (value == 0) {
                return "已出库"
            } else if (value == 1) {
                return "正常销售";
            } else if (value == 2) {
                return "买断"
            } else if (value == 3) {
                return "包销"
            }
        }
    },
    {
        field: "price_wholesale", title: "批发价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_retail", title: "实销价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "refund_amount", title: "调价补差", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "guide_commission", title: "导购提成", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    { field: "outstorage_time", title: "出库时间", align: "center", sortable: true, width: 150 },
    { field: "out_distributor", title: "出库经销商", align: "center", sortable: true, width: 180 },
    { field: "out_sales_m", title: "出库业务经理", align: "center", sortable: true, width: 150 },
    { field: "out_sales", title: "出库业务员", align: "center", sortable: true, width: 150 },
    { field: "sale_time", title: "实销时间", align: "center", sortable: true, width: 150 },
    { field: "sale_distributor", title: "实销经销商", align: "center", sortable: true, width: 180 },
    { field: "sales_m", title: "实销业务经理", align: "center", sortable: true, width: 150 },
    { field: "sales", title: "实销业务员", align: "center", sortable: true, width: 150 },
    { field: "reporter", title: "导购员", align: "center", sortable: true, width: 150 },
]

function initTable() {
    //    var columns = inittitle(tableHeader);
    //1.初始化Table
    //    var oTable = new TableInit(columns);
    var oTable = new TableInit(tableHeader);
    oTable.Init();
}
var TableInit = function (columns) {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            height: h,
            url: "/SaleManage/Summary/SnMainList?date=" + new Date(),        //请求后台的URL（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageSize: 20,                       //每页的记录行数（*）
            pageList: [20, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            strictSearch: false,
            showColumns: true,                  //是否显示所有的列 
            showRefresh: true,                  //是否显示刷新按钮
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            checkbox: true,
            detailView: false,               //是否显示父子表
            columns: columns,
            queryParams: function (params) {
                var param = {
                    page: (params.offset / params.limit) + 1,
                    rows: params.limit,
                    sidx: params.sort,
                    sord: params.order,
                    company_id: $('#company_id').val(),
                    phone_sn: $('#phone_sn').val().trim(),
                    model: $('#model').val(),
                    startTime1: $('#startTime1').val(),
                    endTime1: $('#endTime1').val(),
                    startTime2: $('#startTime2').val(),
                    endTime2: $('#endTime2').val(),
                };
                return param;
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

function deleteColumns() {
    var obj = $table.bootstrapTable('getSelections');
    var phoneSnList = $.map(obj, function (row) {
        return row.phone_sn;
    });
    if (phoneSnList.length <= 0) {
        alert("请至少选择一行删除");
        return;
    } else {
        top.layer.confirm("确认要删除这些数据吗?", function (index) {
            var date = {};
            $.submitForm({
                url: "/SaleManage/Summary/SnDelete?snList=" + phoneSnList,
                param: { snList: phoneSnList },
                success: function (data) {
                    if (data.state == "success") {
                        top.layer.closeAll();
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
    $('#modalQuery').modal('hide');

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
//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}
//重新查询 导出 
function Export() {
    window.location.href = "/SaleManage/Summary/ExportExcelSn?company_id=" + $('#company_id').val()
            + "&phone_sn=" + $('#phone_sn').val()
            + "&model=" + $('#model').val()
            + "&startTime1=" + $('#startTime1').val().trim()
            + "&endTime1=" + $('#endTime1').val().trim()
            + "&startTime2=" + $('#startTime2').val().trim()
            + "&endTime2=" + $('#endTime2').val().trim();
}