﻿var $table = $('#gridList');
var h = $(window).height() - 30;

var show_inactive = false;    // 初始不显示已注销
$(function () {

    //1. 初始化Table
    initTable();
    $table.bootstrapTable('hideColumn', 'inactive_name');
    $table.bootstrapTable('hideColumn', 'inactive_time');
    $("#querySubmit").click(querySubmit);

    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
})
var tableHeader = [
    { field: "company_name", title: "所属机构", align: "center", sortable: true },
    {
        field: "effect_status", title: "状态", align: "center", sortable: true,
        formatter: function (value) {
            if (value == -1) return "<span class='label label-warning'>未生效</span>";
            if (value == 1)  return "<span class='label label-success'>有效</span>";
            if (value == 2)  return "<span class='label label-danger'>已失效</span>";
        }
    },
    {
        field: "minimum_fee", title: "最低运费（元）", align: "center", sortable: true,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "free_count", title: "免运费台数（台）", align: "center", sortable: true,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "effect_date", title: "生效时间", align: "center", sortable: true,
        formatter: (function (value) {
            var date = new Date(value);
            return date.pattern("yyyy-MM-dd");
        })
    },
    { field: "creator_name", title: "发起人", align: "center", sortable: true },
    {
        field: "create_time", title: "发起时间", align: "center", sortable: true,
        formatter: (function (value) {
            var date = new Date(value);
            return date.pattern("yyyy-MM-dd");
        })
    },
]

function initTable() {
    //1.初始化Table
    var oTable = new TableInit();
    oTable.Init();
}
var TableInit = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            height: h,
            url: "/DistributorManage/ShippingTemplate/GetGridJson?date=" + new Date(),        //请求后台的URL（*）
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
            showColumns: true,                  //是否显示所有的列 
            showRefresh: true,                  //是否显示刷新按钮
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableHeader,
            queryParams: function (params) {
                var param = {
                    page: (params.offset / params.limit) + 1,
                    rows: params.limit,
                    sidx: params.sort,
                    sord: params.order,
                    account_name: $('#account_name').val().trim(),
                    employee_name: $('#employee_name').val().trim(),
                    role_name: $('#role_name').val().trim(),
                    inactive: $("#inactive").prop('checked'),
                };
                return param;
            },
        });
    };
    return oTableInit;
};

function querySubmit() {
    $table.bootstrapTable({ offset: 0 }); // 第一页
    show_inactive = $("#inactive").prop('checked');
    if (show_inactive) {
        $table.bootstrapTable('showColumn', 'inactive_name');
        $table.bootstrapTable('showColumn', 'inactive_time');
    }
    else {
        $table.bootstrapTable('hideColumn', 'inactive_name');
        $table.bootstrapTable('hideColumn', 'inactive_time');
    }
    $table.bootstrapTable('refresh');
    $('#modalQuery').modal('hide')
}

function OpenForm(url, title) {
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

//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}