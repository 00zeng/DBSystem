var id = $.request("id");
var $tableEmp = $('#targetEmp');
var $tableProduct = $('#targetProduct');
var $pTableWrap = $("#targetProductWrap");
var eFullList = [];  // 经销商
var pSelList = [];  // 机型
var start_date;
var end_date;
var $tableSale = $('#gridList');
var emp_category = 0;
var product_scope = 0;  //1.全部机型 2.指定机型
//返利政策
var str1 = "<tr style='display: table;width: 100%;table-layout: fixed;'><td><div class='input-group'><input  class='form-control text-center' value='";
var str2 = "'disabled><span class='input-group-addon no-border'>-</span><input  class='form-control text-center'value='";
var str3 = "";
var str4 = "";
var totalReward = 0;
var totalAmount = 0;
var totalCount = 0;
var totalNormalCount = 0;

$(function () {
    $(window).resize(function () {
        $tableEmp.bootstrapTable('resetView', {
            height: 300
        });
        $tableProduct.bootstrapTable('resetView', {
            height: 300
        });
    });
    $("#querySubmit").on('click', querySubmit);
    $.ajax({
        url: "/ActivityManage/Attaining/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#creator_position_name").text(data.creator_position_name);
            $("#form1").formSerializeShow(data.mainInfo);

            var activity_status = data.mainInfo.activity_status;
            if (activity_status == 1) {
                $("#activity_status").text("进行中");
            } else if (activity_status == -1) {
                $("#activity_status").text("未开始");
            } else if (activity_status == 2) {
                $("#activity_status").text("已结束");
            }

            $("#activity_target").text(ToThousandsStr(data.mainInfo.activity_target));           
            emp_category = data.mainInfo.emp_category;
            switch (emp_category) {
                case 3:
                    $("#emp_category").html("导购员");
                    break;
                case 20:
                    $("#emp_category").html("业务员");
                    break;
                case 21:
                    $("#emp_category").html("业务经理");
                    break;
                default:
                    break;
            }           
            var checkRebateVal = data.mainInfo.rebate_mode; //奖励金额
            var checkTargetVal = data.mainInfo.target_mode;//返利模式
            var checkSaleVal = data.mainInfo.target_sale;//销量阶梯


            ////统计时间
            $("#start_time").text(data.statisticsTime.start_time.substring(0, 10));
            $("#end_time").text(data.statisticsTime.end_time.substring(0, 10));

            $.each(data.empList, function (index, item) {
                totalReward += item.total_reward;
                totalAmount += item.total_amount;
                totalCount += item.total_count;
                totalNormalCount += item.total_normal_count;
            })
            product_scope = data.mainInfo.product_scope;
            //Table 
            TablesInit();
            eFullList = data.empList;
            pSelList = data.productList;
            $tableEmp.bootstrapTable('load', eFullList);
            $("#emp_count").text(eFullList.length);
            if (product_scope == 2) {//指定机型
                $("#productCountWrap").show();
                $("#targetProductWrap").show();
                $tableProduct.bootstrapTable('load', pSelList);
                $("#product_count").text(pSelList.length);
            } else {
                $("#productCountWrap").hide();
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
            //append机型
            var tr = '';
            var rebateMax = "";
            rewardList = data.rewardList;
            proRewardList = data.proRewardList;
            var len = rewardList.length;
            var len2 = proRewardList.length;

            function RebateMode1() {
                for (var i = 0; i < len; i++) {
                    var id = rewardList[i].id;
                    var rowSpan = 0;
                    var trProR = "";

                    for (var j = 0; j < len2; j++) {
                        var reward_id = proRewardList[j].reward_id;

                        if (id == reward_id) {
                            rowSpan++;
                            if (proRewardList[j].reward >= 0) {
                                btnClass = "primary";
                                rewards = "奖";
                                reward = proRewardList[j].reward;
                            } else {
                                btnClass = "danger";
                                rewards = "罚";
                                reward = 0 - proRewardList[j].reward;
                            }
                            if (proRewardList[j].target_max == -1)
                                proRebateMax = "以上";
                            else
                                proRebateMax = proRewardList[j].target_max;

                            if (rowSpan > 1)
                                trProR += "<tr>";
                            trProR += "<td  class='col-sm-4'  style='text-align:center;vertical-align:middle;'><div class='input-group formValue'>"
                                    + "<input class='form-control text-center' name='pro_target_min' disabled value='" + ToThousandsStr(proRewardList[j].target_min) + "'>"
                                    + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center  'disabled name='pro_target_max'  value='" + ToThousandsStr(proRebateMax) + "'>"
                                    + "</div></td>"
                                    + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;' ><div class='input-group  col-sm-12'>"
                                    + "<div class='input-group-btn'><button type='button'  class='btn btn-" + btnClass + "' name='pro_rewards' >" + ToThousandsStr(rewards) + "</button></div>"
                                    + "<span class='form-control text-center' name='rebate_p'>" + ToThousandsStr(reward) + "</span><span name='rebate_mode_input1' class='input-group-addon no-border'>" + str2 + "</span>"
                                    + "<span name='rebate_mode_input2' class='input-group-addon no-border no-padding'>" + str4 + "</span></div></td></tr>";
                        }
                    }
                    if (rewardList[i].target_max == -1)
                        rebateMax = "以上";
                    else
                        rebateMax = rewardList[i].target_max;

                    var trR = "<tr><td  class='col-sm-4' rowspan='" + rowSpan + "' style='text-align:center;vertical-align:middle;'><div class='input-group formValue'>"
                                    + "<input class='form-control text-center' name='pro_target_min' disabled value='" + ToThousandsStr(rewardList[i].target_min) + "'>"
                                    + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center required number'disabled name='pro_target_max'  value='" + rebateMax + "'>"
                                    + "</div></td>";
                    tr += trR + trProR;
                }
                $("#rebateMode1").append('<tbody style="display: block;height:400px;overflow-y:scroll;overflow-x:hidden">' + tr + '</tbody>');

            }


            function RebateMode4() {
                for (var i = 0; i < len; i++) {
                    var id = rewardList[i].id;
                    var rowSpan = 0;
                    var trProR = "";

                  
                    for (var j = 0; j < len2; j++) {
                        var reward_id = proRewardList[j].reward_id;
                        if (id == reward_id) {
                            rowSpan++;
                            if (proRewardList[j].reward >= 0) {
                                btnClass = "primary";
                                rewards = "奖";
                                reward = proRewardList[j].reward;
                            } else {
                                btnClass = "danger";
                                rewards = "罚";
                                reward = 0 - proRewardList[j].reward;
                            }
                            if (rowSpan > 1)
                                trProR += "<tr>";
                            trProR += "<td  class='col-sm-3' style='text-align:center;vertical-align:middle;' name='model_pro' >" + proRewardList[j].model + "</td><td class='col-sm-2' style='text-align:center;vertical-align:middle;' name='color_pro'>" + proRewardList[j].color + "</td>"
                                    + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;' ><div class='input-group  col-sm-12'>"
                                    + "<div class='input-group-btn'><button type='button'  class='btn btn-" + btnClass + "' name='pro_rewards' >" + ToThousandsStr(rewards) + "</button></div>"
                                    + "<span class='form-control text-center' name='rebate_p'>" + ToThousandsStr(reward) + "</span><span name='rebate_mode_input1' class='input-group-addon no-border'>" + str2 + "</span>"
                                    + "<span name='rebate_mode_input2' class='input-group-addon no-border no-padding'>" + str4 + "</span></div></td></tr>";
                        }
                    }
                    if (rewardList[i].target_max == -1)
                        rebateMax = "以上";
                    else
                        rebateMax = rewardList[i].target_max;

                    var trR = "<tr><td  class='col-sm-3' rowspan='" + rowSpan + "' style='text-align:center;'><div class='input-group formValue'>"
                                    + "<input class='form-control text-center' name='pro_target_min' disabled value='" + ToThousandsStr(rewardList[i].target_min) + "'>"
                                    + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center required number'disabled name='pro_target_max'  value='" + ToThousandsStr(rebateMax) + "'>"
                                    + "</div></td>";


                    tr += trR + trProR;
                }
                $("#rebateMode4").append('<tbody style="display: block;height:400px;overflow-y:scroll;overflow-x:hidden">' + tr + '</tbody>');
            }

            function RebateMode6() {
                for (var i = 0; i < len; i++) {
                    var id = rewardList[i].id;
                    var rowSpan = 0;
                    var trProR = "";
                   
                    for (var j = 0; j < len2; j++) {
                        var reward_id = proRewardList[j].reward_id;
                        if (proRewardList[j].reward >= 0) {
                            btnClass = "primary";
                            rewards = "奖";
                            reward = proRewardList[j].reward;
                        } else {
                            btnClass = "danger";
                            rewards = "罚";
                            reward = 0 - proRewardList[j].reward;
                        }
                        if (id == reward_id) {
                            trProR += "<td class='col-sm-4' style='text-align:center;vertical-align:middle;' ><div class='input-group  col-sm-12'>"
                                    + "<div class='input-group-btn'><button type='button'  class='btn btn-" + btnClass + "' name='pro_rewards' >" + ToThousandsStr(rewards) + "</button></div>"
                                    + "<span class='form-control text-center' name='rebate_p'>" + ToThousandsStr(reward) + "</span><span name='rebate_mode_input1' class='input-group-addon no-border'>" + str2 + "</span>"
                                    + "<span name='rebate_mode_input2' class='input-group-addon no-border no-padding'>" + str4 + "</span></div></td></tr>";
                        }
                    }
                    if (rewardList[i].target_max == -1)
                        rebateMax = "以上";
                    else
                        rebateMax = rewardList[i].target_max;

                    var trR = "<tr><td  class='col-sm-6'  style='text-align:center;vertical-align:middle;'><div class='input-group formValue'>"
                                    + "<input class='form-control text-center' name='pro_target_min' disabled value='" + ToThousandsStr(rewardList[i].target_min) + "'>"
                                    + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center required number'disabled name='pro_target_max'  value='" + rebateMax + "'>"
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

var tableEmpHeader = [
    { field: "id", visible: false, },
    {
        field: "emp_name", title: "姓名", align: "center",
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
        field: "total_ratio", title: "完成率（%）", align: "center", visible: false,
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: (function (value) { return "--"; })
    },
    {
        field: "total_reward", title: "奖励金额（元）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); }),
        footerFormatter: (function (value) { return ToThousandsStr(String(totalReward)); })
    },
]
var tableProductHeader = [
    { field: "id", visible: false, },
    { field: "model", title: "型号", align: "center" },
    { field: "color", title: "颜色", align: "center", },
    {
        field: "total_count", title: "累计销量（台）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "total_amount", title: "累计销量总金额（元）", align: "center",
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
            columns: tableEmpHeader,
        });
    };
    return oTableInit;
};

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

function initTable2() {
    var oTable = new TableInit2();
    oTable.Init();
}
var TableInit2 = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $tableSale.bootstrapTable({
            height: 400,
            url: "/ActivityManage/Attaining/GetSaleList?date=" + new Date(),        //请求后台的URL（*）
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
                    target_content: $('#target_content').val(),
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
