var id = $.request("id");
var $tableEmp = $('#targetEmployee');
var $tableProduct = $('#targetProduct');
var $pTableWrap = $("#targetProductWrap");
var eSelList = [];  // 员工
var pSelList = [];  // 机型
//var emp_category;
var start_date;
var end_date;
$(function () {
    $.ajax({
        url: "/ActivityManage/SalesPerformance/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#creator_position_name").text(data.creator_position_name);
            $("#form1").formSerializeShow(data.perfInfo);
            start_date = data.perfInfo.start_date;
            end_date = data.perfInfo.end_date;
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

            if (data.perfInfo.emp_category == 20) {
                $("#emp_category").text("业务员");
            } else if(data.perfInfo.emp_category == 21){
                $("#emp_category").text("业务经理");
            }
                    
            //Table 
            TablesInit();
            eSelList = data.empList;
            pSelList = data.productList;
            $tableEmp.bootstrapTable('load', eSelList);
            $("#emp_count").text(eSelList.length);
            if (data.perfInfo.target_product == 2) {//指定机型
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

function TablesInit() {
   
    initTable($tableEmp, tableEmployeeHeader);   
    initTable($tableProduct, tableProductHeader);
}

var tableEmployeeHeader = [
    { field: "id", visible: false, },
    { field: "emp_name", title: "姓名", align: "center" },
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
function OpenForm(url, title) {
        url += "?id=" + id +"&start_date=" + start_date+ "&end_date=" + end_date;
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