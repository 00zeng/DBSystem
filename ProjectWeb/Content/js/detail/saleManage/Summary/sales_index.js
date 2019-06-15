var $table = $('#gridList');
var totalData = [];
var h = $(window).height() - 30;
var now = (new Date()).pattern("yyyy-MM-dd");
var day1st = now.substring(0, 8) + "01";
$(function () {
    $("#queryForm").queryFormValidate();
    $('#startTime1').val(day1st);
    $('#startTime2').val(now);
    getTotal();
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
function getTotal() {
    $.ajax({
        url: "/SaleManage/Summary/SalesGetTotalInfo?emp_name=" + $('#name').val() + "&company_id=" + $('#company_id').val()
            + "&startTime1=" + $('#startTime1').val() + "&startTime2=" + $('#startTime2').val(),
        data: {},
        type: "get",
        async: false,
        dataType: "json",
        success: function (data) {
            totalData = data;
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
var tableHeader = [
    {
        field: "emp_name", title: "姓名", align: "center", sortable: true, width: 130,
        footerFormatter: (function (value) { return "总计"; })
    },
    {
        field: "outstorage_count", title: "出库总量", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: function (value) {
            return ToThousandsStr(String(totalData.outstorage_count));
        }
    },
     {
         field: "outstorage_amount", title: "出库总金额", align: "center", sortable: true, width: 130,
         formatter: (function (value) { return ToThousandsStr(value); }),
         footerFormatter: function (value) {
             return ToThousandsStr(String(totalData.outstorage_amount));
         }
     },
    {
        field: "sale_count", title: "实销总量", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: function (value) {
            return ToThousandsStr(String(totalData.sale_count));
        }
    },

    {
        field: "sale_amount", title: "实销总金额", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: function (value) {
            return ToThousandsStr(String(totalData.sale_amount));
        }
    },
    {
        field: "normal_count", title: "正常机销量", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: function (value) {
            return ToThousandsStr(String(totalData.normal_count));
        }
    },
    {
        field: "normal_amount", title: "正常机销售额", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: function (value) {
            return ToThousandsStr(String(totalData.normal_amount));
        }
    },
    {
        field: "buyout_count", title: "买断机销量", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: function (value) {
            return ToThousandsStr(String(totalData.buyout_count));
        }
    },
    {
        field: "buyout_amount", title: "买断机销售额", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: function (value) {
            return ToThousandsStr(String(totalData.buyout_amount));
        }
    },
    {
        field: "ex_count", title: "包销机销量", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: function (value) {
            return ToThousandsStr(String(totalData.ex_count));
        }
    },
    {
        field: "ex_amount", title: "包销机销售额", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: function (value) {
            return ToThousandsStr(String(totalData.ex_amount));
        }
    },  
    {
        field: "company_linkname", title: "机构", align: "center", sortable: true, width: 150,
        footerFormatter: (function (value) { return "--"; })
    },
]
var tableHeader2 = [
    { field: "phone_sn", title: "串码", align: "center", sortable: true, width: 100 },
    { field: "model", title: "型号", align: "center", sortable: true, width: 140 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 100 },
    {
        field: "sale_type", title: "销售状态", align: "center", sortable: true, width: 100,
        formatter: function (value) {
            if (value == -1) {
                return "已退库"
            } else if (value == 0) {
                return "已出库"
            } else if (value == 1) {
                return "正常销售"
            } else if (value == 2) {
                return "买断"
            } else if (value == 3) {
                return "包销"
            }
        }
    },
    {
        field: "special_offer", title: "特价机", align: "center", sortable: true, width: 80,
        formatter: (function (value) {
            if (value == 1) return "是";
            return "否";
        })
    },
    {
        field: "high_level", title: "高端机", align: "center", sortable: true, width: 80,
        formatter: (function (value) {
            if (value == 1) return "是";
            return "否";
        })
    },
    {
        field: "price_wholesale", title: "批发价格", align: "center", sortable: true, width: 90,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    { field: "out_sales", title: "出库业务员", align: "center", sortable: true, width: 140 },
    { field: "outstorage_time", title: "出库时间", align: "center", sortable: true, width: 150 },
    {
        field: "price_sale", title: "实销价格", align: "center", sortable: true, width: 90,
        formatter: (function (value) { return ToThousandsStr(value); })
    },      
    { field: "sales", title: "实销业务员", align: "center", sortable: true, width: 140 },
    { field: "sale_time", title: "实销时间", align: "center", sortable: true, width: 150 },

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
            url: "/SaleManage/Summary/SalesMainList?date=" + new Date(),        //请求后台的URL（*）
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
            showFooter: true,
            showRefresh: true,                  //是否显示刷新按钮
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: true,                   //是否显示父子表
            columns: tableHeader,
            queryParams: function (params) {
                var param = {
                    page: (params.offset / params.limit) + 1,
                    rows: params.limit,
                    sidx: params.sort,
                    sord: params.order,
                    emp_name: $('#name').val().trim(),
                    position_type: $('#position_type').val(),
                    company_id: $('#company_id').val(),
                    startTime1: $('#startTime1').val(),
                    startTime2: $('#startTime2').val(),
                };
                return param;
            },
            onLoadSuccess: function (data) {
                if (data.total == 0) {
                    $(".fixed-table-footer").css("display", "none")
                }
            },
            onExpandRow: function (index, row, $detail) {// $detail：当前行下面创建的新行里面的td对象。
                oInit.InitSubTable(index, row, $detail);
            }
        });
    };
    //初始化子表格
    var oInit = new Object();
    oInit.InitSubTable = function (index, row, $detail) {
        var parent_id = row.emp_id;
        var cur_table = $detail.html('<table  style="table-layout:fixed; border: 1px solid #3c8dbc"></table>').find('table');
        $(cur_table).bootstrapTable({
            url: "/SaleManage/Summary/SalesDetailList",        //请求后台的URL（*）
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageSize: 10,                       //每页的记录行数（*）
            pageList: [20, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            strictSearch: false,
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableHeader2,
            queryParams: function (params) {
                var param = {
                    page: (params.offset / params.limit) + 1,
                    rows: params.limit,
                    sidx: params.sort,
                    sord: params.order,
                    emp_id: parent_id,
                    startTime1: $('#startTime1').val(),
                    startTime2: $('#startTime2').val(),
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
    firstText: "--请选择机构--",
});

function querySubmit() {
    $table.bootstrapTable({ offset: 0 }); // 第一页
    $table.bootstrapTable('removeAll');
    $('#modalQuery').modal('hide');
    getTotal();  //getTotal要在refresh之前
    $table.bootstrapTable('refresh');
}

//清空筛选
$("#clear").click(function () {
    $("#name").val('');
    $(".modal-body select").each(function () {
        $(this).val('').trigger("change");
    });
})

//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}


//重新查询  
function Export() {
    window.location.href = "/SaleManage/Summary/ExportExcelSalesDetail?company_id=" + $('#company_id').val()
            + "&emp_name=" + $('#name').val().trim()
            + "&startTime1=" + $('#startTime1').val().trim()
            + "&startTime2=" + $('#startTime2').val().trim();
}