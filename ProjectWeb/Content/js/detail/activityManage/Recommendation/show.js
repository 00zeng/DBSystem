var id = $.request("id");
var $tableEmp = $('#targetEmp');
var $tableProduct = $('#targetProduct');
var $pTableWrap = $("#targetProductWrap");
var eFullList = [];  // 经销商
var pSelList = [];  // 机型
var emp_category;
//返利政策
var str1 = "<tr style='display: table;width: 100%;table-layout: fixed;'><td><div class='input-group'><input  class='form-control text-center' value='";
var str2 = "'disabled><span class='input-group-addon no-border'>-</span><input  class='form-control text-center'value='";
var str3 = "";
var str4 = "";

$(function () {
    $(window).resize(function () {
        $tableEmp.bootstrapTable('resetView', {
            height: 300
        });
        $tableProduct.bootstrapTable('resetView', {
            height: 300
        });
    });
    $.ajax({
        url: "/ActivityManage/Recommendation/GetInfo?date=" + new Date(),
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

            //Table 
            TablesInit();
            eFullList = data.empList;
            pSelList = data.productList;
            $tableEmp.bootstrapTable('load', eFullList);
            $("#distributor_count").text(eFullList.length);
            if (data.mainInfo.product_scope == 2) {//指定机型
                $("#productCountWrap").show();
                $pTableWrap.show();
                $tableProduct.bootstrapTable('load', pSelList);
                $("#product_count").text(pSelList.length);
            } else {
                $("#productCountWrap").hide();
            }

            //返利政策 改变th
            if (checkTargetVal != 1)
                $("#forTargetMode1").hide();

            if (checkTargetVal == 1) {
                $("span[name='target_mode_change']").html("完成率（%）");
                $("#rebateMode1").show();
                $("#rebateMode4").hide();
            } else if (checkTargetVal == 2) {
                // 按台数
                $("span[name='target_mode_change']").html("销量（台）");
                $("#rebateMode1").show();
                $("#rebateMode4").hide();

            } else if (checkTargetVal == 3) {
                // 按零售价
                $("span[name='target_mode_change']").html("零售价（元/台）");
                $("#rebateMode1").show();
                $("#rebateMode4").hide();

            } else if (checkTargetVal == 4) {
                // 按型号
                $("#rebateMode4").show();
                $("#rebateMode1").hide();
                ProductScope();
            }

            //奖励金额 改变th
            if (checkRebateVal == 1) {
                $("span[name='rebate_mode_change']").html("奖励金额");
                str4 = "";
                str3 = "元/台";
            } else if (checkRebateVal == 2) {
                $("span[name='rebate_mode_change']").html("批发价比例");
                str4 = "*批发价";
                str3 = "%";
            } else if (checkRebateVal == 3) {
                $("span[name='rebate_mode_change']").html("零售价比例");
                str4 = "*零售价 ";
                str3 = "%";
            } else if (checkRebateVal == 4) {
                $("span[name='rebate_mode_change']").html("固定金额");
                str4 = "";
                str3 = "元";
            }
            $("span[name='rebate_mode_input1']").html(str3);
            $("span[name='rebate_mode_input2']").html(str4);

            if (checkTargetVal != 4) {
                RebateMode1();
                $("#rebateMode1").show();
            }
            function RebateMode1() {
                var tr = "";
                var btnClass = "";
                var rewards = "";
                var reward = "";
                $.each(data.rewardList, function (index, item) {
                    if (item.target_max == -1) {
                        item.target_max = "以上";
                    }
                    if (item.reward >= 0) {
                        btnClass = "primary";
                        rewards = "奖";
                        reward = item.reward;
                    } else {
                        btnClass = "danger";
                        rewards = "罚";
                        reward = 0 - item.reward;
                    }
                    tr += str1 + item.target_min + str2 + ToThousandsStr(item.target_max) + "' disabled><td><div class='input-group'> <div class='input-group-btn'><button type='button' class='btn btn-" + btnClass + "' name='rewards'>" + ToThousandsStr(rewards) + "</button></div><span  class='input-group-addon no-border'>"
                         + ToThousandsStr(reward) + "</span><span name='rebate_mode_input1' class='input-group-addon no-border no-padding'>" + str3 + "</span><span name='rebate_mode_input2' class='input-group-addon no-border no-padding'>" + str4 + "</span></div></td></tr>";
                })

                $("#rebateMode1").append('<tbody style="display: block;height:300px;overflow-y:scroll;">' + tr + '</tbody>');
            }

            //append机型
            function ProductScope() {
                var tr = "";
                var btnClass = "";
                var reward_p = "";
                var reward = "";

                $.each(pSelList, function (index, item) {
                    if (item.reward >= 0) {
                        btnClass = "primary";
                        reward_p = "奖";
                        reward = item.reward;
                    } else {
                        btnClass = "danger";
                        reward_p = "罚";
                        reward = 0 - item.reward;
                    }
                    tr += "<tr style='display: table;width: 100%;table-layout: fixed;'><td style='text-align:center;vertical-align:middle;' class='col-md-3'>" + item.model + "</td><td style='text-align:center;vertical-align:middle;' class='col-md-2'>" + item.color + "</td><td style='text-align:center;vertical-align:middle;' class='col-md-2'>" + ToThousandsStr(item.price_wholesale) + "</td><td style='text-align:center;vertical-align:middle;' class='col-md-5'><div class='input-group'> <div class='input-group-btn'><button type='button' class='btn btn-" + btnClass + "' name='reward_p'>" + ToThousandsStr(reward_p) + "</button></div><span  class='input-group-addon no-border'>"
                    + ToThousandsStr(reward) + "</span><span name='rebate_mode_input1' class='input-group-addon no-border no-padding'>" + str3 + "</span><span name='rebate_mode_input2' class='input-group-addon no-border no-padding'>" + str4 + "</span></div></td></tr>";
                })
                $("#rebateMode4").append('<tbody style="display: block;height:300px;overflow-y:scroll;">' + tr + '</tbody>');
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
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

function TablesInit() {
    initTable($tableEmp, tableDistributorHeader);
    $tableEmp.bootstrapTable('load', eFullList);
    $("#distributor_count").text(eFullList.length);
    initTable($tableProduct, tableProductHeader);
}

var tableDistributorHeader = [
    { field: "id", visible: false, },
    { field: "emp_name", title: "名称", align: "center" },
    { field: "area_l1_name", title: "经理片区", align: "center", },
    { field: "area_l2_name", title: "业务片区", align: "center", },
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
