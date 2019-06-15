var id = $.request("id");
var $tableEmp = $('#targetEmp');
var eFullList = []; // 所有员工
$(function () {
    $.ajax({
        url: "/FinancialAccounting/StaySubsidy/GetInfo?id=" + id,
        async: false,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            
            TablesInit();

            $("#form1").formSerializeShow(data.mainInfo);
            $("#company_amount").text(ToThousandsStr(data.mainInfo.company_amount));
            $("#emp_amount").text(ToThousandsStr(data.mainInfo.emp_amount));

            var sum = data.mainInfo.company_amount + data.mainInfo.emp_amount;
            $("#amount").text(ToThousandsStr(sum));
            var date = data.mainInfo.month.substring(0, 7);
            $("#month").text(date);
            //发放对象
            eFullList = data.listEmp;
            $tableEmp.bootstrapTable('load', eFullList);
            $("#emp_count").text(eFullList.length);

            var tr = '';
            $.each(data.approveList, function (index, item) {
                if (item.status > 0) {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                } else {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);
           
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
        
    })
   
})

// Table START
function TablesInit() {
    initTable($tableEmp, tableEmpHeader);
    $tableEmp.bootstrapTable('load', eFullList);
    $("#emp_count").text(eFullList.length);
}

var tableEmpHeader = [
    { field: "id", visible: false, },
    { field: "emp_name", title: "名称", align: "center" },
    { field: "position_name", title: "职位", align: "center", },
    { field: "dept_name", title: "所属部门", align: "center", },
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