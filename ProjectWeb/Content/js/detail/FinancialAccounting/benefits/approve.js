var id = $.request("id");
var $tableEmp = $('#targetEmp');
var eFullList = []; // 所有员工
var company_name = decodeURI($.request("company_name"));
$("#company_name").text(company_name);
$(function () {
    setInterval(function () { $('#currentTime').text((new Date()).pattern("yyyy-MM-dd HH:mm:ss")) }, 1000);

    $.ajax({
        url: "/FinancialAccounting/Benefits/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            TablesInit();
            $("#form1").formSerializeShow(data.benefitsInfo);
            if (data.benefitsInfo.exclude_emp_status) {
                $("#exclude_emp_status").attr("checked", true);
                $("#status").show();
            }
            if (data.benefitsInfo.exclude_entry_date) {
                $("#entry_date").attr("checked", true);
                $("#exclude_entry_date").text(data.benefitsInfo.exclude_entry_date.substring(0, 10));
                $("#date").show();

            }
            //发放对象
            eFullList = data.empList;
            $tableEmp.bootstrapTable('load', eFullList);
            $("#emp_count").text(eFullList.length);


            //福利列表
            $.each(data.benefitslistInfo, function (index, item) {
                var tr;
                var paid_type_val;
                if (item.paid_type == 1) {
                    paid_type_val = item.paid_date.substring(0, 7);
                } else {
                    paid_type_val = item.paid_date.substring(0, 10);
                }

                if (item.paid_type == 1) {
                    paid_type = "工资";
                } else if (item.paid_type == 2) {
                    paid_type = "礼品";
                } else if (item.paid_type == 3) {
                    paid_type = "现金";
                } else if (item.paid_type == 4) {
                    paid_type = "其他形式";
                }
                tr = '<tr><td>' + item.name + '</td>' + '<td>' + paid_type + '</td>' + '<td>' + item.amount + '</td>' + '<td>' + paid_type_val + '</td>' + '<td>' + item.note + '</td></tr>';
                $("#table_list").append(tr);
            })

            //查看审批
            $("#approve_name").text(top.clients.loginInfo.empName);
            $("#approve_position_name").text(top.clients.loginInfo.positionInfo.name);
            $("#creator_position_name").text(data.creator_position_name);

            var tr = '';
            $.each(data.approvelistInfo, function (index, item) {
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
    })
});

// Table START
function TablesInit() {
    initTable($tableEmp, tableEmpHeader);
    $tableEmp.bootstrapTable('load', eFullList);
    $("#emp_count").text(eFullList.length);
}

var tableEmpHeader = [
    { field: "id", visible: false, },
    { field: "name", title: "名称", align: "center" },
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

// Table END
function submitForm() {
    var data = {};
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["main_id"] = id;
    data["status"] = $("input[name='status']:checked").val();
    data["approve_note"] = $("#approve_note").val();
    $.submitForm({
        url: "/FinancialAccounting/Benefits/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/Benefits/ApproveIndex";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}