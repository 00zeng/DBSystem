var id = $.request("id");
$(function () {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetImport?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#form1").formSerializeShow(data.getInfo);
            if (data.getInfo.effect_date == '' || data.getInfo.effect_now == true) {
                $("#effect_date").text("立即生效");
            }
            
            if (data.getInfo.approve_status == 0)
                $("#cur_status").text("未审批");
            else if (data.getInfo.approve_status == 100)
                $("#cur_status").text("审批通过");
            else if(data.getInfo.approve_status < 0)
                $("#cur_status").text("审批不通过");
            else
                $("#cur_status").text("审批中");

            var columns = inittitle(tableHeader);
            //1.初始化Table
            var oTable = new TableInit(data.infoList, columns);
            oTable.Init();
            //resoveresult(tableHeader, data.listPriceInfo);
            //审批
            var tr = '';
            $.each(data.approveList, function (index, item) {
                if (item.status > 0) {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                } else {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);
            if (data.approveList.length == 1) {
                $("#next_approve").html('财务经理审批');
            }

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})
var $table = $('#tb_table');
var importData = [];
var tableHeader =
    {
        "类型": "grade_category_display",
        "职层": "grade_level_display",
        "职等": "grade",
        "职位": "position_display",
        "基本工资": "base_salary",
        "岗位工资": "position_salary",
        "住房补助": "house_subsidy",
        "全勤奖": "attendance_reward",
        "工龄工资": "seniority_wage",
        "车费补贴": "traffic_subsidy_display",
        "KPI": "kpi",
        "合计": "total",
    };


function initTable() {
    var columns = inittitle(tableHeader);
    //1.初始化Table
    var oTable = new TableInit([], columns);
    oTable.Init();
}
var TableInit = function (data, columns) {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            url: '',         //请求后台的URL（*）
            data: data,
            method: 'get',                      //请求方式（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "asc",                   //排序方式
            queryParams: '',                    //传递参数（*）
            sidePagination: "client",           //分页方式：client客户端分页，server服务端分页（*）
            pageNumber: 1,                       //初始化加载第一页，默认第一页
            pageSize: 10,                       //每页的记录行数（*）
            pageList: [10, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            strictSearch: true,
            // showColumns: true,                  //是否显示所有的列 
            uniqueId: "no",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: columns,
        });
    };
    return oTableInit;
};

function inittitle(gtitle) {
    var columns = []; //; [{ field: "id", title: "", align: "center", edit: false, formatter: function (value, row, index) { return index; }}]

    for (var a in gtitle) {
        var colObj = {
            field: gtitle[a],
            title: a,
            align: "center",
            width: 100,
        };
        if (gtitle[a] == "effect_date" || gtitle[a] == "expire_date") {
            colObj.width = 180;
            colObj.formatter = (function (value) { var date = new Date(value); return date.pattern("yyyy-MM-dd HH:mm"); });
        }
        columns.push(colObj);
    }
    return columns;
}

function resoveresult(config, list) {
    $table.bootstrapTable('showLoading');
    if (list.length > 0) {
        for (var one in list) {
            var obj = {};
            for (var index in config) {

                var key = list[one][index];
                if (!key) {
                    obj[config[index]] = "";
                } else {
                    if (config[index] == "effect_date" || config[index] == "expire_date") {
                        var date = new Date(key);
                        obj[config[index]] = date.pattern("yyyy-MM-dd HH:mm");
                    }
                    else
                        obj[config[index]] = key;
                }

            }
            obj.id = Number(one);
            importData.push(obj);
        }
        $table.bootstrapTable('load', importData);
    }
    $table.bootstrapTable('hideLoading');
}

//撤回
function delForm() {
    top.layer.confirm("确认要撤回? ", function (index) {

        var data = {};
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["id"] = id;
        $.ajax({
            url: "/saleManage/buyoutImport/Delete?id=" + id,
            type: "post",
            data: data,
            success: function (data) {
                top.layer.msg("撤回成功");
                window.location.href = "javascript: history.go(-1)";

            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}
function submitForm() {
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["status"] = $("input[name='status']:checked").val();
    data["salary_position_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/PositionSalary/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/EmployeeSalary/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
