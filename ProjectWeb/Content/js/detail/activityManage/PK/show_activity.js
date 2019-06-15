var id = $.request("id");
var $tableProduct = $('#targetProduct');
var $pTableWrap = $("#targetProductWrap");
var pSelList = [];  // 机型
var start_date;
var end_date;
var emp_category;
var $tableSale = $('#gridList');
var product_scope = 0;  //1.全部机型 2.指定机型
$(function () {
    $(window).resize(function () {
        $tableProduct.bootstrapTable('resetView', {
            height: 300
        });
    });
    $("#querySubmit").on('click', querySubmit);
    $.ajax({
        url: "/ActivityManage/PK/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.mainInfo);
            start_date = data.mainInfo.start_date;
            end_date = data.mainInfo.end_date;
            if (data.mainInfo.activity_status == 2 || data.mainInfo.approve_status != 100) {
                $("#btn_edit").css("display", "none");
            }

            var activity_status = data.mainInfo.activity_status;
            if (activity_status == 1) {
                $("#activity_status").text("进行中");
            } else if (activity_status == -1) {
                $("#activity_status").text("未开始");
            } else if (activity_status == 2) {
                $("#activity_status").text("已结束");
            }

            $("#win_lose").text(ToThousandsStr(data.mainInfo.win_lose));
            $("#win_company").text(ToThousandsStr(data.mainInfo.win_company));
            $("#win_penalty").text(ToThousandsStr(data.mainInfo.win_penalty));
            $("#lose_win").text(ToThousandsStr(data.mainInfo.lose_win));
            $("#lose_penalty").text(ToThousandsStr(data.mainInfo.lose_penalty));

            emp_category = data.mainInfo.emp_category;
            if (emp_category == 3) {
                $("#emp_category").text("导购员");
            } else if (emp_category == 20) {
                $("#emp_category").text("业务员");
            } else if (emp_category == 21) {
                $("#emp_category").text("业务经理");
            }
            var tr1 = '';
            //var tr2 = '<td class="col-sm-6">'
            //          + '<div class="progress-group"><div class="progress sm"><div class="progress-bar progress-bar-blue" style="width: 50%"></div>'
            //          + '</div> </div></td>';
            var tr3 = '';
            var imgLeft = "";
            var imgRight = "";
            var total_ratio_left = 0;
            var total_ratio_right = 0;
            var empList = data.empList;
            var emp_count = data.empList.length;
            $("#emp_count").text(emp_count);

            for (var i = 0; i < empList.length; i += 2) {
                if (empList[i].winner == true) {
                    imgLeft = '<img src="/Content/img/icon/win.png"/>';
                } else {
                    imgLeft = '';
                }
                tr1 = '<tr style="height:50px"> <td class="col-sm-3 " name="left"  style="text-align:center;vertical-align:middle">' + empList[i].emp_name + "(" + empList[i].area_l2_name + ')' + imgLeft + '</td>';
                if (empList[i + 1].winner == true) {
                    imgRight = '<img src="/Content/img/icon/win.png"/>';
                } else {
                    imgRight = '';
                }
                if (empList[i].total_ratio > 100) {
                    total_ratio_left = 100;
                } else {
                    total_ratio_left = empList[i].total_ratio;
                }
                if (empList[i + 1].total_ratio > 100) {
                    total_ratio_right = 100;
                } else {
                    total_ratio_right = empList[i + 1].total_ratio;
                }
                var tr2 = '<td class="col-sm-6"><div class="progress-group col-sm-6"> <span class="progress-text">目标销量：' + ToThousandsStr(empList[i].activity_target) + '台&nbsp;&nbsp;&nbsp;&nbsp;销量：' + ToThousandsStr(empList[i].total_count) + '台&nbsp;&nbsp;&nbsp;&nbsp;总奖金：' + ToThousandsStr(empList[i].total_reward) + '元</span>'
                    + ' <div class="progress"><div class="progress-bar progress-bar-striped" style="width: ' + total_ratio_left + '%">' + empList[i].total_ratio + '%</div></div>'
                    + ' </div> <div class="progress-group col-sm-6">'
                    + ' <span class="progress-text">目标销量：' + ToThousandsStr(empList[i + 1].activity_target) + '台&nbsp;&nbsp;&nbsp;&nbsp;销量：' + ToThousandsStr(empList[i + 1].total_count) + '台&nbsp;&nbsp;&nbsp;&nbsp;总奖金：' + ToThousandsStr(empList[i + 1].total_reward) + '元<div class="progress transForm" >'
                    + ' <div class="progress-bar progress-bar-striped" style="width:' + total_ratio_right + '%"><div class="transfont">' + empList[i + 1].total_ratio + '%</div></div></div></div></td>';

                tr3 = '<td class="col-sm-3" name="right"  style="text-align:center;vertical-align:middle" >' + empList[i + 1].emp_name + "(" + empList[i + 1].area_l2_name + ')' + imgRight + '</td></tr>';

                $("#race").append(tr1 + tr2 + tr3);
            }
            product_scope = data.mainInfo.product_scope;
            //Table 
            TablesInit();
            pSelList = data.rewardList;
            if (product_scope == 2) {//指定机型
                $("#productCountWrap").show();
                $pTableWrap.show();
                $("#product_count").text(pSelList.length);
                $tableProduct.bootstrapTable('load', pSelList);
            } else {
                $("#productCountWrap").hide();
            }

            //活动政策
            if (data.mainInfo.win_company != 0) {
                $("input[name='win_company']").attr("checked", true);
            } else {
                $("input[name='win_company']").attr("checked", false);
            }

            if (data.mainInfo.win_penalty != 0) {
                $("input[name='win_penalty']").attr("checked", true);
            } else {
                $("input[name='win_penalty']").attr("checked", false);
            }

            if (data.mainInfo.lose_penalty != 0) {
                $("input[name='lose_penalty']").attr("checked", true);
            } else {
                $("input[name='lose_penalty']").attr("checked", false);
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
})
function querySubmit() {
    $tableSale.bootstrapTable({ offset: 0 }); // 第一页
    $tableSale.bootstrapTable('removeAll');
    $tableSale.bootstrapTable('refresh');
}

function TablesInit() {
    initTable($tableProduct, tableProductHeader);
    initTable2();
}


var tableProductHeader = [
    { field: "id", visible: false, },
    { field: "model", title: "型号", align: "center" },
    { field: "color", title: "颜色", align: "center", },
    {
        field: "price_wholesale", title: "批发价（元/台）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); })
    },
]
var tableSaleHeader = [
    { field: "name", title: "姓名", align: "center", sortable: true, width: 100, },
    { field: "phone_sn", title: "串码", align: "center", sortable: true, width: 150, },
    { field: "model", title: "型号", align: "center", sortable: true, width: 150 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 100 },
    {
        field: "price_wholesale", title: "批发价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_retail", title: "零售价", align: "center", sortable: true, width: 100,
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

function initTable($table, tableHeader) {
    //1.初始化Table
    var oTable = new TableInit($table, tableHeader);
    oTable.Init();
}
function initTable2() {
    var oTable = new TableInit2();
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
var TableInit2 = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $tableSale.bootstrapTable({
            height: 400,
            url: "/ActivityManage/PK/GetSaleList?date=" + new Date(),        //请求后台的URL（*）
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
                    product_scope: product_scope,
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