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
        url: "/DistributorManage/Settlement/GetTotalAttaining?distributor_name=" + $('#distributor_name').val() + "&company_id=" + $('#company_id').val()
            + "&startTime1=" + $('#startTime1').val() + "&startTime2=" + $('#startTime2').val(),
        data: {},
        type: "get",
        async:false,
        dataType: "json",
        success: function (data) {
            totalData = data;
            $table.bootstrapTable('refresh');
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

var tableHeader = [
    {
        field: "distributor_name", title: "经销商名称", align: "center", sortable: true, width: 200,
        footerFormatter: (function (value) { return "总计"; })
    },
    {
        field: "total_rebate", title: "返利总金额", align: "center", sortable: true, width: 110,
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: function (value) {
            return ToThousandsStr(String(totalData.total_amount));
        }
    },
    {
        field: "count", title: "返利次数", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: function (value) {
            return ToThousandsStr(String(totalData.total_count));
        }
    },
    {
        field: "address", title: "详细地址", align: "center", sortable: true, width: 250,        
    },
    {
        field: "company_linkname", title: "所属机构", align: "center", sortable: true, width: 150,       
    },
]
var tableHeader2 = [
     {
         field: "", title: "操作", align: "center", sortable: true, width: 80,
         formatter: function (value, row) {
             return "<a class='btn btn-primary btn-xs' href='/DistributorManage/Attaining/ShowActivity?id=" + row.id + " ' title='查看活动详情'><i class='fa fa-search'></i></a>";
         }
     },
    {
        field: "name", title: "返利名称", align: "center", sortable: true, width: 150,       
    },
    {
        field: "target_content", title: "返利类型", align: "center", sortable: true, width: 150,
        formatter: function (value, row, index) {
            if (value == 1) {
                return "按实销量"
            }
            else return "按下货量"
        }
    },
    {
        field: "end_date", title: "活动时间", align: "center", sortable: true, width: 150,
        formatter: function (value, row, index) {
            var startDate = new Date(row.start_date);
            var endDate = new Date(value);
            return startDate.pattern("yyyy-MM-dd") + ' 至 ' + endDate.pattern("yyyy-MM-dd");
        }
    },
    {
        field: "total_rebate", title: "返利金额", align: "center", sortable: true, width: 150,
        formatter: (function (value) { return ToThousandsStr(value); })
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
            url: "/DistributorManage/Settlement/AttainingMainList?date=" + new Date(),        //请求后台的URL（*）
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
                    distributor_name: $('#distributor_name').val().trim(),
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
        var parentid = row.distributor_id;
        var cur_table = $detail.html('<table  style="table-layout:fixed; border: 1px solid #3c8dbc"></table>').find('table');
        $(cur_table).bootstrapTable({
            url: "/DistributorManage/Settlement/AttainingList",        //请求后台的URL（*）
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
                    distributor_id: parentid,                    
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

function querySubmit() {
    $table.bootstrapTable({ offset: 0 }); // 第一页
    $table.bootstrapTable('removeAll');
    $('#modalQuery').modal('hide');
    getTotal();  //getTotal要在refresh之前
    $table.bootstrapTable('refresh');
}

//清空筛选
$("#clear").click(function () {
    $("#distributor_name").val('');
    $(".modal-body select").each(function () {
        $(this).val('').trigger("change");
    });
})

//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}

//导出 
function Export() {
    window.location.href = "/DistributorManage/Settlement/AttainingExport?company_id=" + $('#company_id').val()
            + "&distributor_name=" + $('#distributor_name').val().trim()
            + "&startTime1=" + $('#startTime1').val().trim()
            + "&startTime2=" + $('#startTime2').val().trim();
}