var id = $.request("id");
var $tableEmp = $('#targetEmployee');
var $tableProduct = $('#targetProduct');
var $pTableWrap = $("#targetProductWrap");
var eSelList = [];  // 员工
var pSelList = [];  // 机型
var start_date;
var end_date;
var $tableSale = $('#gridList');
var emp_category = 0;
var target_product = 0;  //1.全部机型 2.指定机型
var target_content = 0;
var totalCountSum;
var totalSalesSum;
var totalRewardSum;

$(function () {
    $("#querySubmit").on('click', querySubmit);
    $.ajax({
        url: "/ActivityManage/SalesPerformance/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#creator_position_name").text(data.perfInfo.creator_position_name);
            $("#form1").formSerializeShow(data.perfInfo);
            start_date = data.perfInfo.start_date;
            end_date = data.perfInfo.end_date;
            totalCountSum = data.perfInfo.total_sale_count;
            totalSalesSum = data.perfInfo.total_sale_amount;
            totalRewardSum = data.perfInfo.total_reward;
            if (data.perfInfo.activity_status == 2 || data.perfInfo.approve_status != 100) {
                $("#btn_edit").css("display", "none");
            }

            var activity_status = data.perfInfo.activity_status;
            if (activity_status == 1) {
                $("#activity_status").text("进行中");
            } else if (activity_status == -1) {
                $("#activity_status").text("未开始");
            } else if (activity_status == 2) {
                $("#activity_status").text("已结束");
            }

            $("#activity_target").text(ToThousandsStr(data.perfInfo.activity_target));
            $("#reward").text(ToThousandsStr(data.perfInfo.reward));
            $("#penalty").text(ToThousandsStr(data.perfInfo.penalty));

            emp_category = data.perfInfo.emp_category;
            target_product = data.perfInfo.target_product;
            target_content = data.perfInfo.target_content;
            if (emp_category == 20) {
                $("#emp_category").text("业务员");
            } else if (emp_category == 21) {
                $("#emp_category").text("业务经理");
            }
            //Table 
            TablesInit();
            eSelList = data.empList;
            pSelList = data.productList;
            $tableEmp.bootstrapTable('load', eSelList);
            $("#emp_count").text(eSelList.length);
            if (target_product == 2) {//指定机型
                $("#productCountWrap").show();
                $pTableWrap.show();
                $("#product_count").text(pSelList.length);
                $tableProduct.bootstrapTable('load', pSelList);
            } else {
                $("#productCountWrap").hide();
            }

            //奖罚金额
            var targetModeValue = data.perfInfo.target_mode;//考核模式                      
            if (targetModeValue == 1) {  //按完成率       
                $("span[name='rebate_mode_input1']").html("完成比例*");
                $("span[name='rebate_mode_input2']").html("未完成比例*");
                $("span[name='rebate_mode_input']").html("元");

            } else if (targetModeValue == 2) {  //按销量       
                $("span[name='rebate_mode_input1']").html("完成数量*");
                $("span[name='rebate_mode_input2']").html("未完成数量*");
                $("span[name='rebate_mode_input']").html("元/台");

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})
function querySubmit() {
    $tableSale.bootstrapTable({ offset: 0 }); // 第一页
    $tableSale.bootstrapTable('removeAll');
    $tableSale.bootstrapTable('refresh');
}
function TablesInit() {

    initTable1();    
    initTable($tableProduct, tableProductHeader);
    initTable2();
}

var tableEmployeeHeader = [
    { field: "id", visible: false, },
    {
        field: "emp_name", title: "姓名", align: "center",
        footerFormatter: (function (value) { return "总计"; })
    },
    {
        field: "total_count", title: "销量", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: (function (value) { return ToThousandsStr(String(totalCountSum)); })
    },
    {
        field: "total_amount", title: "销量总金额", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: (function (value) { return ToThousandsStr(String(totalSalesSum)); })
    },
    {
        field: "total_ratio", title: "完成率（%）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: (function (value) { return "--"; })
    },
    {
        field: "total_reward", title: "奖罚金额", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: (function (value) { return ToThousandsStr(String(totalRewardSum)); })
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
    {
        field: "total_count", title: "累计销量", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); })
    },
]
var tableSaleHeader = [
    { field: "emp_name", title: "姓名", align: "center", sortable: true, width: 100, },
    { field: "phone_sn", title: "串码", align: "center", sortable: true, width: 150, },
    { field: "model", title: "型号", align: "center", sortable: true, width: 150 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 100 },
    {
        field: "price_wholesale", title: "批发价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_sale", title: "实销价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
     {
         field: "time", title: "时间", align: "center", sortable: true, width: 100,
         formatter: function (value, row) {
             var date = new Date(value);
             return date.pattern("yyyy-MM-dd");
         }
     },
]

function initTable1() {
    var oTable = new TableInit1();
    oTable.Init();
}
function initTable($table, tableHeader) {
    var oTable = new TableInit($table, tableHeader);
    oTable.Init();
}
function initTable2() {
    var oTable = new TableInit2();
    oTable.Init();
}

var TableInit1 = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $tableEmp.bootstrapTable({
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
            columns: tableEmployeeHeader,
        });
    };
    return oTableInit;
};

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

var TableInit2 = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $tableSale.bootstrapTable({
            height: 400,
            url: "/ActivityManage/SalesPerformance/GetSaleList?date=" + new Date(),        //请求后台的URL（*）
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
            showColumns: false,                  //是否显示所有的列 
            showRefresh: false,                  //是否显示刷新按钮
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableSaleHeader,
            queryParams: function (params) {
                var param = {
                    page: (params.offset / params.limit) + 1,
                    rows: params.limit,
                    sidx: params.sort,
                    sord: params.order,
                    id: id,
                    start_date: $('#start_date').text(),
                    end_date: $('#end_date').text(),
                    emp_category: emp_category,
                    target_product: target_product,
                    target_content: target_content,
                    name: $('#emp_name').val().trim(),
                    phone_sn: $('#phone_sn').val().trim(),
                    model: $('#model').val().trim(),

                };
                return param;
            },
            onLoadError: function (data) {
                console.log(data);
            }
        });
    };
    return oTableInit;
};

// Table END
function OpenForm(url, title) {
    url += "?id=" + id + "&start_date=" + start_date + "&end_date=" + end_date;
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