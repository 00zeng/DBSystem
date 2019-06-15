var id = $.request("id");
var $tableDistributor = $('#targetDistributor');
var $tableProduct = $('#targetProduct');
var $pTableWrap = $("#targetProductWrap");
var dFullList = [];  // 经销商
var pFullList = [];  // 机型

var totalRebate = 0;
var totalAmount = 0;
var totalCount = 0;
var totalNormalCount = 0;

//返利政策
$(function () {
    $(window).resize(function () {
        $tableDistributor.bootstrapTable('resetView', {
            height:300
        });
        $tableProduct.bootstrapTable('resetView', {
            height:300
        });
    });
    $.ajax({
        url: "/DistributorManage/Recommendation/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#form1").formSerializeShow(data.mainInfo);

            var activity_status = data.mainInfo.activity_status;
            if (activity_status == 1) {
                $("#activity_status").text("进行中");
            } else if (activity_status == -1) {
                $("#activity_status").text("未开始");
            } else if (activity_status == 2) {
                $("#activity_status").text("已结束");
            }

            //统计时间
            $("#start_time").text(data.statisticsTime.start_time.substring(0, 10));
            $("#end_time").text(data.statisticsTime.end_time.substring(0, 10));
            
            $.each(data.distributorList, function (index, item) {
                totalRebate += item.total_rebate;
                totalAmount += item.total_amount;
                totalCount += item.total_count;
                totalNormalCount += item.total_normal_count;
            })

            //Table 
            TablesInit();
            dFullList = data.distributorList;
            pFullList = data.productList;
            $tableDistributor.bootstrapTable('load', dFullList);
            $("#distributor_count").text(dFullList.length);
            if (data.mainInfo.product_scope == 2) {//指定机型
                $pTableWrap.show();
                $tableProduct.bootstrapTable('load', pFullList);
                $("#product_count").text(pFullList.length);
            } else {
                $pTableWrap.hide();
            }

            RebateMode1()
            function RebateMode1() {
                var tr = "";
                $.each(data.rebateList, function (index, item) {
                    if (item.target_max == -1) {
                        item.target_max = "以上";
                    }
                    tr += "<tr style='display: table;width: 100%;table-layout: fixed;'><td><div class='input-group'><input  class='form-control text-center' value='" + ToThousandsStr(item.target_min) + "' disabled><span class='input-group-addon no-border'>-</span><input  class='form-control text-center'value='" + ToThousandsStr(item.target_max) + "' disabled><td style='text-align:center;vertical-align:middle;'>" + ToThousandsStr(item.rebate) + "</div></td></tr>";;
                })

                $("#rebateMode1").append('<tbody style="display: block;height:300px;overflow-y:scroll;">' + tr + '</tbody>');
            }
            //查看审批
            $("#creator_position_name").text(data.creator_position_name);
            var tr = '';
            $.each(data.approveList, function (index, item) {
                if (item.approve_note == null || item.approve_note == '') {
                    approve_note_null = "--";
                } else
                    approve_note_null = item.approve_note;
                if (item.status == 1 || item.status == -1)
                    approve_grade = ("一级审批");
                else if (item.status == 2 || item.status == -2)
                    approve_grade = ("二级审批");
                else if (item.status == 3 || item.status == -3)
                    approve_grade = ("三级审批");
                else if (item.status == 4 || item.status == -4)
                    approve_grade = ("四级审批");
                else if (item.status == 100 || item.status == -100)
                    approve_grade = ("终审");
                if (item.status > 0) {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                } else {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

function TablesInit() {
    initTable1($tableDistributor, tableDistributorHeader);
    $tableDistributor.bootstrapTable('load', dFullList);
    $("#distributor_count").text(dFullList.length);
    initTable($tableProduct, tableProductHeader);
    $tableProduct.bootstrapTable('load', pFullList);
    $("#product_count").text(pFullList.length);
}

function initTable1($tableDistributor, tableDistributorHeader) {
    var oTable1 = new TableInit1($tableDistributor, tableDistributorHeader);
    oTable1.Init();
}

var TableInit1 = function ($tableDistributor, tableDistributorHeader) {
    var oTableInit1 = new Object();
    //初始化Table
    oTableInit1.Init = function () {
        $tableDistributor.bootstrapTable({
            height: 300,
            url: "",        //请求后台的URL（*）
            striped: false,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: false,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "client",           //分页方式：client客户端分页，server服务端分页（*）
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            showFooter: true,
            columns: tableDistributorHeader,
        });
    };
    return oTableInit1;
};

var tableDistributorHeader = [
    { field: "id", visible: false, },
    {
        field: "distributor_name", title: "经销商", align: "center",
        footerFormatter: (function (value) { return "总计"; })
    },
    {
        field: "total_count", title: "总销量（台）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: (function (value) { return ToThousandsStr(String(totalCount)); })
    },
    {
        field: "total_normal_count", title: "正常机销量（台）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: (function (value) { return ToThousandsStr(String(totalNormalCount)); })
    },
    {
        field: "total_amount", title: "总金额（元）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: (function (value) { return ToThousandsStr(String(totalAmount)); })
    },
    {
        field: "total_rebate", title: "返利金额（元）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: (function (value) { return ToThousandsStr(String(totalRebate)); })
    },
]

var tableProductHeader = [
    { field: "id", visible: false, },
    { field: "model", title: "型号", align: "center" },
    { field: "color", title: "颜色", align: "center", },
    {
        field: "price_wholesale", title: "批发价（元/台）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    //{ field: "total_count", title: "累计销量（台）", align: "center", },
    //{ field: "total_amount", title: "累计销量总金额（元）", align: "center", },
]


function initTable($table, tableHeader) {
    //1.初始化Table
    var oTable = new TableInit($table, tableHeader);
    oTable.Init();
}
var TableInit = function ($table, tableHeader) {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            height: 300,
            url: "",        //请求后台的URL（*）
            striped: false,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: false,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "client",           //分页方式：client客户端分页，server服务端分页（*）
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableHeader,
        });
    };
    return oTableInit;
};
// Table END
