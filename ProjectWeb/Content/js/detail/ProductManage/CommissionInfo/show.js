var id = $.request("id");
$(function () {
    $.ajax({
        url: "/ProductManage/CommissionInfo/GetInfoMain?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            if (!data || data == null || data.length < 1) {
                $.modalAlert("信息不存在");
                window.history.go(-1);
                return;
            }
            $(".form").formSerializeShow(data.mainInfo);
            $(".form").formSerializeShow(data.approveList[0]);
            initTable();
            var approveStatus = data.mainInfo.status;
            var accJobid = top.clients.loginInfo.jobHistoryId;
            if (approveStatus == 0) {
                $("#approveInfoWrap").hide();
                $("#approve_status").text("未审批");
                $("#approve_status").addClass("label-warning");
                if (accJobid == data.mainInfo.creator_job_history_id)
                    $("#btnDel").css("display", "block");
            }
            else if (approveStatus > 0) {
                $("#approveInfoWrap").show();
                $("#approve_status,#status").text("通过");
                $("#approve_status,#status").addClass("label-success");
            }
            else if (approveStatus < 0){
                $("#approveInfoWrap").show();
                $("#approve_status,#status").text("不通过");
                $("#approve_status,#status").addClass("label-danger");
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
        "型号": "model",
        "颜色": "color",
        "导购提成": "guide_commission",
        "包销提成": "exclusive_commission",
        "核算底薪": "salary_include",
        "开始时间": "effect_date",
        "结束时间": "expire_date",
        "备注": "note",
    };



function initTable() {
    var columns = inittitle(tableHeader);
    //1.初始化Table
    var oTable = new TableInit([], columns);
    oTable.Init();
    $('.fixed-table-toolbar').append('<div class="pull-right search"><input id="queryStr" class="form-control" type="text" placeholder="型号"></div>');
    $('#queryStr').bind('keydown', function (event) {
        if (event.keyCode == "13") {
            $("#queryStr").val($("#queryStr").val().trim());
            $table.bootstrapTable({ offset: 0 }); // 第一页
            $table.bootstrapTable('removeAll');
            $table.bootstrapTable('refresh');

        }
    });
}
var TableInit = function (data, columns) {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            url: '/ProductManage/CommissionInfo/GetInfoPage',         //请求后台的URL（*）
            //  data: data,
            method: 'get',                      //请求方式（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "asc",                   //排序方式
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageNumber: 1,                       //初始化加载第一页，默认第一页
            pageSize: 10,                       //每页的记录行数（*）
            pageList: [10, 50, 100, 300, 500],        //可供选择的每页的行数（*）          
            strictSearch: false,
            showColumns: true,                  //是否显示所有的列 
            //uniqueId: "no",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: columns,
            queryParams: function (params) {
                var pageInfo = {
                    id: id,
                    rows: params.limit,                              //页面大小
                    page: (params.offset / params.limit) + 1,        //页码
                    sidx: params.sort,
                    sord: params.order,
                    queryStr: $("#queryStr").val(), //搜索框
                };
                return pageInfo;
            },
        });
    };
    return oTableInit;
};

function inittitle(gtitle) {
    var columns = []; 

    for (var a in gtitle) {
        var colObj = {
            field: gtitle[a],
            title: a,
            align: "center",
            width: 100,
            sortable: true,
            search: true,
        };
        if (gtitle[a] == "model" || gtitle[a] == "note") {
            colObj.width = 180;
        }
        else if (gtitle[a] == "effect_date" || gtitle[a] == "expire_date") {
            colObj.width = 160;
            colObj.formatter = (function (value) { var date = new Date(value); return date.pattern("yyyy-MM-dd"); });
        }
        else if (gtitle[a] == "salary_include") {
            colObj.formatter = (function (value) { return (value ? "是" : "否"); });
        }
        else if (gtitle[a] == "guide_commission" || gtitle[a] == "exclusive_commission") {
            colObj.formatter = (function (value) { return ToThousandsStr(value); });
        }
        columns.push(colObj);
    }
    return columns;
}

//审批
function approval() {
    window.location.href = "/ProductManage/CommissionInfo/Approve?id=" + id;
}

//撤回
function delForm() {
    top.layer.confirm("确认要撤回? ", function (index) {

        var data = {};
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["main_id"] = id;
        $.ajax({
            url: "/ProductManage/CommissionInfo/Delete?id=" + id,
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