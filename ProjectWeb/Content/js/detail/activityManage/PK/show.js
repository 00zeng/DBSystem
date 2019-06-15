var id = $.request("id");
var $tableProduct = $('#targetProduct');
var $pTableWrap = $("#targetProductWrap");
var pSelList = [];  // 机型
var start_date;
var end_date;
$(function () {
    $(window).resize(function () {
        $tableProduct.bootstrapTable('resetView', {
            height: 300
        });
    });
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

            var emp_category = data.mainInfo.emp_category;
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
                $(".transfont").css("transform", "rotate(180deg)");
            }


            //Table 
            TablesInit();
            pSelList = data.rewardList;
            if (data.mainInfo.product_scope == 2) {//指定机型
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

function TablesInit() {
    initTable($tableProduct, tableProductHeader);
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