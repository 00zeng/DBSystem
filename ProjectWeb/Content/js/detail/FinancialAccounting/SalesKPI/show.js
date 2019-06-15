﻿var id = $.request("id");
var h = 400;
var $table = $('#gridList');
var empId = "";
//获取登录人信息
var myPositionInfo = top.clients.loginInfo.positionInfo;
//myPositionInfo.positionType
var salesKPI = {};

//返利政策
var str1 = "<tr><td><div class='input-group'><input  class='form-control text-center' value='";
var str2 = "'disabled><span class='input-group-addon no-border'><= X <</span><input  class='form-control text-center'value='";
var str3 = "";
var str4 = "";
var str6 = "";
var str8 = "";
$(function () {
    $("#querySubmit").on('click', querySubmit);
    $("#exportExcel").click(Export);
    $.ajax({
        url: "/FinancialAccounting/SalesKPI/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            salesKPI = data.salesKPI;
            if (salesKPI.is_adjust == false) {
                $("#adjust").css("display", "none");
                $("#kpi_total_a_div").css("display", "none");
                $("#note_div").css("display", "none");
            }

            $("#form1").formSerializeShow(data.empInfo);
            $("#form1").formSerializeShow(salesKPI);
            $("#normal_count").text(ToThousandsStr(salesKPI.normal_count));
            $("#normal_amount_w").text(ToThousandsStr(salesKPI.normal_amount_w));
            $("#normal_amount_r").text(ToThousandsStr(salesKPI.normal_amount_r));
            $("#buyout_count").text(ToThousandsStr(salesKPI.buyout_count));
            $("#buyout_amount_w").text(ToThousandsStr(salesKPI.buyout_amount_w));
            $("#buyout_amount").text(ToThousandsStr(salesKPI.buyout_amount));
            $("#total_count").text(ToThousandsStr(salesKPI.total_count));
            $("#total_amount_w").text(ToThousandsStr(salesKPI.total_amount_w));
            $("#total_amount_r").text(ToThousandsStr(salesKPI.total_amount_r));
            $("#kpi_total").text(ToThousandsStr(salesKPI.kpi_total));
            $("#form1").formSerializeShow(data.salesSalaryInfo);
            $("#note").val(salesKPI.note);
            empId = data.empInfo.id;
            var checkNormalVal = data.salesSalaryInfo.normal_rebate_mode; //正常机
            var checkBuyoutVal = data.salesSalaryInfo.buyout_rebate_mode; //买断机
            var checkTargetVal = data.salesSalaryInfo.target_mode;//返利模式
            //返利政策 改变th


            if (checkTargetVal == 1) {
                $("#forTargetMode1").show();
                $("span[name='target_mode_change']").html("完成率（%）");

            } else if (checkTargetVal == 2) {
                // 按台数
                $("span[name='target_mode_change']").html("销量（台）");
            } else if (checkTargetVal == 3) {
                // 按零售价
                $("span[name='target_mode_change']").html("零售价（元/台）");
            } else if (checkTargetVal == 5) {
                // 按批发价
                $("span[name='target_mode_change']").html("批发价（元/台）");
            }

            //正常机 改变th
            if (checkNormalVal == 1) {
                $("span[name='rebate_mode_change']").html("（元/台）");
                str4 = "";
                str3 = "";
            } else if (checkNormalVal == 2) {
                $("span[name='rebate_mode_change']").html("（元/台）");
                str4 = "*批发价";
                str3 = "%";
            } else if (checkNormalVal == 3) {
                $("span[name='rebate_mode_change']").html("（元/台）");
                str4 = "*零售价 ";
                str3 = "%";
            } else if (checkNormalVal == 4) {
                $("span[name='rebate_mode_change']").html("（元/月）");
                str4 = "";
                str3 = "";
            }
            $("span[name='rebate_mode_input1']").html(str3);
            $("span[name='rebate_mode_input2']").html(str4);


            //买断 改变th
            if (checkBuyoutVal == 1) {
                $("span[name='rebate_mode_change2']").html("（元/台）");
                str8 = "";
                str6 = "";
            } else if (checkBuyoutVal == 2) {
                $("span[name='rebate_mode_change2']").html("（元/台）");
                str8 = "*批发价";
                str6 = "%";
            } else if (checkBuyoutVal == 3) {
                $("span[name='rebate_mode_change2']").html("（元/台）");
                str8 = "*买断价 ";
                str6 = "%";
            } else if (checkBuyoutVal == 4) {
                $("span[name='rebate_mode_change2']").html("（元/月）");
                str8 = "";
                str6 = "";
            }
            //$("span[name='rebate_mode_input3']").html(str6);
            //$("span[name='rebate_mode_input4']").html(str8);

            var tr;
            $.each(data.sublist, function (index, item) {
                if (item.target_max == -1) {
                    tr = str1 + ToThousandsStr(item.target_min) + "'disabled><span class='input-group-addon no-border'><= X &nbsp;&nbsp;</span><input  class='form-control text-center'value='" + '以上' + "'  style='visibility: hidden;'></div></td>"
                         + "<td class='col-sm-4'  style='text-align:center;vertical-align:middle;'><span>" + ToThousandsStr(item.sale_rebate) + "</span><span>" + str3 + "</span>&nbsp;&nbsp;<span>" + str4 + "</span></td>"
                         + "<td class='col-sm-4'  style='text-align:center;vertical-align:middle;'><span>" + ToThousandsStr(item.buyout_rebate) + "</span><span>" + str6 + "</span>&nbsp;&nbsp;<span>" + str8 + "</span></td>";
                } else {
                    tr = str1 + ToThousandsStr(item.target_min) + str2 + ToThousandsStr(item.target_max) + "' disabled></div></td>"
                        + "<td class='col-sm-4'  style='text-align:center;vertical-align:middle;'><span>" + ToThousandsStr(item.sale_rebate) + "</span><span>" + str3 + "</span>&nbsp;&nbsp;<span>" + str4 + "</span></td>"
                        + "<td class='col-sm-4'  style='text-align:center;vertical-align:middle;'><span>" + ToThousandsStr(item.buyout_rebate) + "</span><span>" + str6 + "</span>&nbsp;&nbsp;<span>" + str8 + "</span></td>";
                }
                $("#rebateMode1").append(tr);
            })

            //审批
            var tr = '';
            $.each(data.ApprovelistInfo, function (index, item) {
                if (item.status > 0) {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                } else {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);
            initTable();
        },
        error: function (data) {
            $.modalAlert(data.responseText, "error");
            window.history.go(-1);
        }
    });
});



function querySubmit() {
    $table.bootstrapTable({ offset: 0 }); // 第一页
    $table.bootstrapTable('removeAll');
    $table.bootstrapTable('refresh');
}

var tableHeader = [
    { field: "phone_sn", title: "串码", align: "center", sortable: true, width: 150, },
    { field: "model", title: "型号", align: "center", sortable: true, width: 150 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 100 },
    {
        field: "sale_type", title: "销售状态", align: "center", sortable: true, width: 80,
        formatter: (function (value) {
            if (value == 0) {
                return "已出库";
            } else if (value == 1) {
                return "正常销售";
            } else if (value == 2) {
                return "买断";
            } else if (value == 3) {
                return "包销";
            }
        })
    },
    { field: "outlay", title: "提成", align: "center", sortable: true, width: 80 },
    {
        field: "outlay_type", title: "类型", align: "center", sortable: true, width: 80, formatter: (function (value) {
            if (value == 2) return "下货";
            return "实销";
        })
    },
    {
        field: "price_wholesale", title: "批发价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_sale", title: "零售价", align: "center", sortable: true, width: 100,
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
            url: "/FinancialAccounting/SalesKPI/GetSaleList?date=" + new Date(),        //请求后台的URL（*）
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
            columns: tableHeader,
            queryParams: function (params) {
                var param = {
                    page: (params.offset / params.limit) + 1,
                    rows: params.limit,
                    sidx: params.sort,
                    sord: params.order,
                    salesKPIId: salesKPI.id,
                    origin: 1, // 对应后台
                    phone_sn: $('#phone_sn').val().trim(),
                    model: $('#model').val().trim(),
                    sale_type: $('#sale_type').val(),
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

function Export() {
    window.location.href = "/FinancialAccounting/SalesKPI/ExportExcel?id=" + salesKPI.id
            + "&emp_name=" + $("#name").text() + "&month=" + $('#month').text() + "&origin=2";
}