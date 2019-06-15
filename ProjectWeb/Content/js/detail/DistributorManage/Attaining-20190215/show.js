var id = $.request("id");
var $tableDistributor = $('#targetDistributor');
var $tableProduct = $('#targetProduct');
var $pTableWrap = $("#targetProductWrap");
var dFullList = [];  // 经销商
var pFullList = [];  // 机型
var rebateList = [];
var proRebateList = [];
//返利政策
var str2 = "";
var str4 = "";
$(function () {
    $(window).resize(function () {
        $tableDistributor.bootstrapTable('resetView', {
            height: 300
        });
        $tableProduct.bootstrapTable('resetView', {
            height: 300
        });
    });
    $.ajax({
        url: "/DistributorManage/Attaining/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#creator_position_name").text(data.creator_position_name);
            $("#form1").formSerializeShow(data.mainInfo);
            $("#activity_target").text(ToThousandsStr(data.mainInfo.activity_target));

            var checkRebateVal = data.mainInfo.rebate_mode; //返利金额
            var checkTargetVal = data.mainInfo.target_mode;//返利模式
            var checkSaleVal = data.mainInfo.target_sale;//销量阶梯
            //统计时间
            $("#start_time").text(data.statisticsTime.start_time.substring(0, 10));
            $("#end_time").text(data.statisticsTime.end_time.substring(0, 10));
            var totalRebate = 0;
            var totalAmount = 0;
            var totalCount = 0;
            var totalNormalCount = 0;
            $.each(data.distributorList, function (index, item) {
                totalRebate += item.total_rebate;
                totalAmount += item.total_amount;
                totalCount += item.total_count;
                totalNormalCount += item.total_normal_count;

            })
            $("#totalAmount").text(ToThousandsStr(totalAmount));
            $("#totalRebate").text(ToThousandsStr(totalRebate));
            $("#totalCount").text(ToThousandsStr(totalCount));
            $("#totalNormalCount").text(ToThousandsStr(totalNormalCount));



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
                $("#productCountWrap").hide();
                $pTableWrap.hide();
            }
            //销量阶梯
            if (checkSaleVal == 1) {
                $("span[name='sale_change']").html("完成率（%）");
                $("span[name='product_change']").html("完成率（%）");
                // 按完成率
            } else if (checkSaleVal == 2) {
                // 按台数
                $("span[name='sale_change']").html("销量（台）");
                $("span[name='product_change']").html("销量（台）");
            }
            //返利模式
            if (checkTargetVal == 3) {
                // 按零售价
                $("span[name='target_mode_change']").html("零售价（元/台）");
                $("#rebateMode1").show();
                $("#rebateMode4").hide();
                $("#rebateMode6").hide();
            } else if (checkTargetVal == 4) {
                //型号
                $("#rebateMode1").hide();
                $("#rebateMode6").hide();
                $("#rebateMode4").show();
            } else if (checkTargetVal == 5) {
                // 按批发价
                $("span[name='target_mode_change']").html("批发价（元/台）");
                $("#rebateMode1").show();
                $("#rebateMode4").hide();
                $("#rebateMode6").hide();
            } else if (checkTargetVal == 6) {
                // 无
                $("#rebateMode1").hide();
                $("#rebateMode4").hide();
                $("#rebateMode6").show();
            }
            //返利金额
            if (checkRebateVal == 1) {
                $("span[name='rebate_mode_change']").html("返利金额");
                str2 = "元/台";
                str4 = "";
            } else if (checkRebateVal == 2) {
                $("span[name='rebate_mode_change']").html("批发价比例");

                str2 = "%";
                str4 = "*批发价";
            } else if (checkRebateVal == 3) {
                $("span[name='rebate_mode_change']").html("零售价比例");

                str2 = "%";
                str4 = "*零售价";
            } else if (checkRebateVal == 4) {//如果返利模式是3那么变1
                $("span[name='rebate_mode_change']").html("固定金额");
                str2 = "元";
                str4 = "";
            }
            $("span[name='rebate_mode_input1']").html(str2);
            $("span[name='rebate_mode_input2']").html(str4);

            //function RebateMode1() {
            //    var tr = "";
            //    $.each(data.rebateList, function (index, item) {
            //        if (item.target_max == -1) {
            //            item.target_max = "以上";
            //        }
            //        tr += "<tr style='display: table;width: 100%;table-layout: fixed;'><td><div class='input-group'><input  class='form-control text-center' value='" + item.target_min + "' disabled><span class='input-group-addon no-border'>-</span><input  class='form-control text-center'value='" + item.target_max + "' disabled>"
            //                 + "<td style='text-align:center;vertical-align:middle;'><div><span>" + item.rebate + "</span><span name='rebate_mode_input1' >" + str2 + "</span>&nbsp; <span name='rebate_mode_input2' >" + str4 + "</span></div></td></tr>";
            //    })

            //    $("#rebateMode1").append('<tbody style="display: block;height:300px;overflow-y:scroll;">' + tr + '</tbody>');
            //}

            //append机型
            var tr = '';
            var rebateMax = "";
            rebateList = data.rebateList;
            proRebateList = data.proRebateList;
            var len = rebateList.length;
            var len2 = proRebateList.length;

            function RebateMode1() {
                for (var i = 0; i < len; i++) {
                    var id = rebateList[i].id;
                    var rowSpan = 0;
                    var trProR = "";

                    for (var j = 0; j < len2; j++) {
                        var rebate_id = proRebateList[j].rebate_id;

                        if (id == rebate_id) {
                            rowSpan++;
                            if (proRebateList[j].target_max == -1)
                                proRebateMax = "以上";
                            else
                                proRebateMax = proRebateList[j].target_max;

                            if (rowSpan > 1)
                                trProR += "<tr>";
                            trProR += "<td  class='col-sm-4'  style='text-align:center;vertical-align:middle;'><div class='input-group formValue'>"
                                    + "<input class='form-control text-center' name='pro_target_min' disabled value='" + ToThousandsStr(proRebateList[j].target_min) + "'>"
                                    + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center  'disabled name='pro_target_max'  value='" + ToThousandsStr(proRebateMax) + "'>"
                                    + "</div></td>"
                                    + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;' ><span>" + ToThousandsStr(proRebateList[j].rebate) + "</span>&nbsp;<span name='rebate_mode_input1' >" + str2 + "</span>&nbsp;"
                                    + "<span name='rebate_mode_input2'>" + str4 + "</span></td></tr>";
                        }
                    }
                    if (rebateList[i].target_max == -1)
                        rebateMax = "以上";
                    else
                        rebateMax = rebateList[i].target_max;

                    var trR = "<tr><td  class='col-sm-4' rowspan='" + rowSpan + "' style='text-align:center;vertical-align:middle;'><div class='input-group formValue'>"
                                    + "<input class='form-control text-center' name='pro_target_min' disabled value='" + rebateList[i].target_min + "'>"
                                    + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center required number'disabled name='pro_target_max'  value='" + rebateMax + "'>"
                                    + "</div></td>";
                    tr += trR + trProR;
                }
                $("#rebateMode1").append('<tbody style="display: block;height:400px;overflow-y:scroll;overflow-x:hidden">' + tr + '</tbody>');

            }


            function RebateMode4() {
                for (var i = 0; i < len; i++) {
                    var id = rebateList[i].id;
                    var rowSpan = 0;
                    var trProR = "";

                    for (var j = 0; j < len2; j++) {
                        var rebate_id = proRebateList[j].rebate_id;
                        if (id == rebate_id) {
                            rowSpan++;
                            if (rowSpan > 1)
                                trProR += "<tr>";
                            trProR += "<td  class='col-sm-3' style='text-align:center;vertical-align:middle;' name='model_pro' >" + proRebateList[j].model + "</td><td class='col-sm-3' style='text-align:center;vertical-align:middle;' name='color_pro'>" + proRebateList[j].color + "</td>"
                                    + "<td class='col-sm-3' style='text-align:center;vertical-align:middle;' ><span>" + ToThousandsStr(proRebateList[j].rebate) + "</span>&nbsp;<span name='rebate_mode_input1'>" + str2 + "</span>&nbsp;"
                                    + "<span name='rebate_mode_input2' >" + str4 + "</span></td></tr>";
                        }
                    }
                    if (rebateList[i].target_max == -1)
                        rebateMax = "以上";
                    else
                        rebateMax = rebateList[i].target_max;

                    var trR = "<tr><td  class='col-sm-3' rowspan='" + rowSpan + "' style='text-align:center;'><div class='input-group formValue'>"
                                    + "<input class='form-control text-center' name='pro_target_min' disabled value='" + ToThousandsStr(rebateList[i].target_min) + "'>"
                                    + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center required number'disabled name='pro_target_max'  value='" + ToThousandsStr(rebateMax) + "'>"
                                    + "</div></td>";


                    tr += trR + trProR;
                }
                $("#rebateMode4").append('<tbody style="display: block;height:400px;overflow-y:scroll;overflow-x:hidden">' + tr + '</tbody>');
            }

            function RebateMode6() {
                for (var i = 0; i < len; i++) {
                    var id = rebateList[i].id;
                    var rowSpan = 0;
                    var trProR = "";
                    for (var j = 0; j < len2; j++) {
                        var rebate_id = proRebateList[j].rebate_id;
                        if (id == rebate_id) {
                            trProR += "<td class='col-sm-6' style='text-align:center;vertical-align:middle;' >"
                                + "<span>" + ToThousandsStr(proRebateList[j].rebate) + "</span>&nbsp;<span name='rebate_mode_input1'>" + str2 + "</span>&nbsp;"
                                    + "<span name='rebate_mode_input2' >" + str4 + "</span></td></tr>";
                        }
                    }
                    if (rebateList[i].target_max == -1)
                        rebateMax = "以上";
                    else
                        rebateMax = rebateList[i].target_max;

                    var trR = "<tr><td  class='col-sm-6'  style='text-align:center;vertical-align:middle;'><div class='input-group formValue'>"
                                    + "<input class='form-control text-center' name='pro_target_min' disabled value='" + ToThousandsStr(rebateList[i].target_min) + "'>"
                                    + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center required number'disabled name='pro_target_max'  value='" + ToThousandsStr(rebateMax) + "'>"
                                    + "</div></td>";
                    tr += trR + trProR;
                }
                $("#rebateMode6").append('<tbody style="display: block;height:400px;overflow-y:scroll;overflow-x:hidden">' + tr + '</tbody>');

            }

            //查看审批
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
            //返利模式
            if (checkTargetVal == 3) {
                RebateMode1();
            } else if (checkTargetVal == 4) {
                RebateMode4();
            } else if (checkTargetVal == 5) {
                RebateMode1();
            } else if (checkTargetVal == 6) {
                RebateMode6();
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

function TablesInit() {
    initTable($tableDistributor, tableDistributorHeader);
    $tableDistributor.bootstrapTable('load', dFullList);
    $("#distributor_count").text(dFullList.length);
    initTable($tableProduct, tableProductHeader);
    $tableProduct.bootstrapTable('load', pFullList);
    $("#product_count").text(pFullList.length);
}

var tableDistributorHeader = [
    { field: "id", visible: false, },
    { field: "distributor_name", title: "经销商", align: "center" },
    { field: "area_l1_name", title: "经理片区", align: "center", },
]

var tableProductHeader = [
    { field: "id", visible: false, },
    { field: "model", title: "型号", align: "center" },
    { field: "color", title: "颜色", align: "center", },
    {
        field: "price_wholesale", title: "批发价（元/台）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); })
    },
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
