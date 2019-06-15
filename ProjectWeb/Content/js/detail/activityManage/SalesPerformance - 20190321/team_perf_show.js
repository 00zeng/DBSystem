var id = $.request("id");
var $tableEmp = $('#targetEmployee');
var eSelList = [];  // 员工
//var emp_category = $('#emp_category').val();
var start_date;
var end_date;
$(function () {
    $.ajax({
        url: "/ActivityManage/SalesPerformance/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
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

            TablesInit();
            eSelList = data.empList;
            $tableEmp.bootstrapTable('load', eSelList);
            $("#emp_count").text(eSelList.length);
            //奖罚金额
            var targetModeValue = data.perfInfo.target_mode;//考核模式                      
            if (targetModeValue == 1) {  //按比例       
                $("span[name='rebate_mode_input1']").html("完成比例*");
                $("span[name='rebate_mode_input2']").html("未完成比例*");
                $("span[name='rebate_mode_input']").html("元");

            } else if (targetModeValue == 2) {  //按人数       
                $("span[name='rebate_mode_input1']").html("完成人数*");
                $("span[name='rebate_mode_input2']").html("未完成人数*");
                $("span[name='rebate_mode_input']").html("元/人");

            }
            //查看审批
            //var tr = '';
            //$.each(data.approveList, function (index, item) {
            //    if (item.approve_note == null || item.approve_note == '') {
            //        approve_note_null = "--";
            //    } else
            //        approve_note_null = item.approve_note;
            //    if (item.status == 1 || item.status == -1)
            //        approve_grade = ("一级审批");
            //    else if (item.status == 2 || item.status == -2)
            //        approve_grade = ("二级审批");
            //    else if (item.status == 3 || item.status == -3)
            //        approve_grade = ("三级审批");
            //    else if (item.status == 4 || item.status == -4)
            //        approve_grade = ("四级审批");
            //    else if (item.status == 100 || item.status == -100)
            //        approve_grade = ("终审");
            //    if (item.status > 0) {
            //        tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + approve_note_null + '</td></tr>';
            //    } else {
            //        tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + approve_note_null + '</td></tr>';
            //    }
            //})
            //$("#approve tr:eq(1)").after(tr);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

function TablesInit() {
    initTable($tableEmp, tableDistributorHeader);
}

var tableDistributorHeader = [
    { field: "id", visible: false, },
    { field: "emp_name", title: "姓名", align: "center" },
    { field: "area_l1_name", title: "经理片区", align: "center", },
    { field: "area_l2_name", title: "业务片区", align: "center", },
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