﻿//实销
var id = $.request("id");
$(function () {
    $.ajax({
        url: "/SaleManage/saleInfo/GetInfoMain?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            if (!data || data == null || data.length < 1) {
                $.modalAlert("信息不存在");
                window.history.go(-1);
                return;
            }
            $(".form").formSerializeShow(data[0]);

            initTable();
            var approveStatus = data[0].status;
            var accJobid = top.clients.loginInfo.jobHistoryId;
            if (approveStatus == 0) {
                $("#approveInfoWrap").hide();
                $("#approve_status").text("未审批");
                $("#approve_status").addClass("label-warning");
                if (accJobid == data[0].creator_job_history_id)
                    $("#btnDel").css("display", "block");
            }
            else if (approveStatus > 0) {
                $("#approveInfoWrap").show();
                $("#approve_status,#status").text("通过");
                $("#approve_status,#status").addClass("label-success");
            }
            else if (approveStatus < 0) {
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
        "数据状态": "check_status",
        "经销商": "distributor_name",
        "型号": "model",
        "颜色": "color",
        "串码": "phone_sn",
        "上报人": "reporter_name",
        "状态": "sale_status",
        "业务大区": "company_name",
        "二级地区": "company_name_parent",
        "三级地区": "area_l2_name",
        "日期": "accur_time",
        "类型": "category",
        "业务片区": "sales_name",
    };


function initTable() {
    var columns = inittitle(tableHeader);
    //1.初始化Table
    var oTable = new TableInit([], columns);
    oTable.Init();
    $('.fixed-table-toolbar').append('<div class="pull-right search"><input id="queryStr" class="form-control" type="text" placeholder="经销商/型号/串码"></div>');
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
            url: '/SaleManage/saleInfo/GetInfoPage',         //请求后台的URL（*）
            //data: data,
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
                    queryStr: $("#queryStr").val(),
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
            width: 180,
            sortable:true,
        };
        if (gtitle[a] == "accur_time") {
            colObj.formatter = (function (value) { var date = new Date(value); return date.pattern("yyyy-MM-dd hh:mm"); });
        }
        else if (gtitle[a] == "check_status") {
            colObj.formatter = (function (value, row) {
                switch (value) {
                    case 0: return "<span class='label label-info'>待检查</span>";
                    case 1: return "<span class='label label-success'>正常</span>";
                    case 11: return '<div class="dropdown"><button class="btn btn-danger btn-xs" data-toggle="dropdown"> 异常 </button>'
                        + ' <div class="dropdown-menu dropdown-menu-left" style="width:300px;"><div class="modal-content"><div class="modal-header"><h4 class="modal-title">异常信息</h4></div>'
                        + ' <div class="modal-body"> <p>存在相同串码、型号、颜色、经销商等重复数据</p> </div>'
                        + ' <div class="modal-footer"> <button type="button" data-id="' + row.id + '" data-name="' + row.distributor_name + '" data-phone_sn="' + row.phone_sn + '" title="确认保留该异常数据" class="btn btn-success pull-left" onclick="Keeping(this)" >保留</button>'
                        + ' <button type="button"  data-id="' + row.id + '" data-phone_sn="' + row.phone_sn + '" title="查看对比相同串码" class="btn btn-primary"  onclick="Show(this)">查看</button>'
                        + ' </div> </div> </div> </div>';
                    case 12: return "<span class='label label-danger' data-toggle='tooltip' data-placement='top' title='存在相同串码、型号、颜色、经销商等重复数据'>异常</span>"
                            + "<span class='label label-success' data-toggle='tooltip' data-placement='top' title='保留该异常数据'>已保留</span>";
                    case 13: return "<span class='label label-danger' data-toggle='tooltip' data-placement='top' title='存在相同串码、型号、颜色、经销商等重复数据'>异常</span>"
                            + "<span class='label label-success'>已删除</span>";
                    case 21: return '<div class="dropdown"><button class="btn btn-danger btn-xs" data-toggle="dropdown"> 异常 </button>'
                        + ' <div class="dropdown-menu dropdown-menu-left" style="width:300px;"><div class="modal-content"><div class="modal-header"><h4 class="modal-title">异常信息</h4></div>'
                        + ' <div class="modal-body"> <p>存在相同串码，型号、颜色、经销商等信息不一致</p> </div>'
                        + ' <div class="modal-footer"> <button type="button" data-id="' + row.id + '" data-name="' + row.distributor_name + '" data-phone_sn="' + row.phone_sn + '" title="确认保留该异常数据" class="btn btn-success pull-left" onclick="Keeping(this)" >保留</button>'
                        + ' <button type="button"  data-id="' + row.id + '" data-phone_sn="' + row.phone_sn + '" title="查看对比相同串码" class="btn btn-primary"  onclick="Show(this)">查看</button>'
                        + ' </div> </div> </div> </div>';
                    case 22: return "<span class='label label-danger' data-toggle='tooltip' data-placement='top' title='存在相同串码，型号、颜色、经销商等信息不一致'>异常</span>"
                            + "<span class='label label-success' data-toggle='tooltip' data-placement='top' title='保留该异常数据'>已保留</span>";
                    case 23: return "<span class='label label-danger' data-toggle='tooltip' data-placement='top' title='存在相同串码，型号、颜色、经销商等信息不一致'>异常</span>"
                            + "<span class='label label-success'>已删除</span>";
                    case -101: return "<span class='label label-primary'>已退库</span>";
                    default: return "<span class='label label-warning'>未知状态</span>";
                }
            })
        }
        columns.push(colObj);
    }
    return columns;
}

//查看
function Show(obj) {
    var id = $(obj).data('id');
    var phone_sn = $(obj).data('phone_sn');
    window.location.href = "/SaleManage/saleInfo/Index?id=" + id + "&phone_sn=" + phone_sn;
}

//删除操作
function Del(obj) {
    var data = {};
    var id = $(obj).data('id');
    var name = $(obj).data('name');
    var phone_sn = $(obj).data('phone_sn');
    top.layer.confirm("您确认要删除经销商：“" + name + "”，串码：“" + phone_sn + "”的实销信息吗？", function (index) {
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["idList"] = id;
        data["status"] = 2;
        $.submitForm({
            url: "/SaleManage/OutStorage/UpdateCheckStatus?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    $.currentWindow().ReSearch();
                    top.layer.closeAll();
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}
//保留操作
function Keeping(obj) {
    var data = {};
    var id = $(obj).data('id');
    var name = $(obj).data('name');
    var phone_sn = $(obj).data('phone_sn');
    top.layer.confirm("您确认要保留经销商：“" + name + "”，串码：“" + phone_sn + "”的实销信息，并删除其他重复串码的信息吗？", function (index) {
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["id"] = id;
        $.submitForm({
            url: "/SaleManage/OutStorage/Keeping?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    $.currentWindow().ReSearch();
                    top.layer.closeAll();
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}
//审批
//function approval() {
//    window.location.href = "/SaleManage/saleInfo/Approve?id=" + id;
//}

//撤回
function delForm() {
    top.layer.confirm("确认要撤回? ", function (index) {

        var data = {};
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["id"] = id;
        $.ajax({
            url: "/saleManage/saleInfo/Delete?id=" + id,
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
//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}
