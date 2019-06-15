﻿//业务考核-活动列表
var $table = $('#gridList');
var h = $(window).height() - 30;

var show_inactive = false;    // 初始不显示已注销
$(function () {

    //1. 初始化Table
    initTable();
    $("#querySubmit").click(querySubmit);
    $('#name').bind('input propertychange', function (event) {
        $table.bootstrapTable({ offset: 0 }); // 第一页
        $table.bootstrapTable('removeAll');
        $table.bootstrapTable('refresh');

    });

    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
})
var tableHeader = [
 {
     field: "", title: "操作", align: "center", sortable: true, width: 80,
     formatter: function (value, row) {
         if (row.category == 1) {
             if (row.activity_status == -2 || row.activity_status == -1 || row.activity_status == 0)
                 return "<a class='btn btn-primary btn-xs' href='/ActivityManage/SalesPerformance/MonthlyPerfShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
             else if (row.activity_status == 1 || row.activity_status == 2)
                 return "<a class='btn btn-primary btn-xs' href='/ActivityManage/SalesPerformance/MonthlyPerfShowActivity?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
         } else if (row.category == 2) {
             if (row.activity_status == -2 || row.activity_status == -1 || row.activity_status == 0)
                 return "<a class='btn btn-primary btn-xs' href='/ActivityManage/SalesPerformance/SalePerfShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
             else if (row.activity_status == 1 || row.activity_status == 2)
                 return "<a class='btn btn-primary btn-xs' href='/ActivityManage/SalesPerformance/SalePerfShowActivity?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
         } else if (row.category == 3) {
             if (row.activity_status == -2 || row.activity_status == -1 || row.activity_status == 0 || row.activity_status == 1)
                 return "<a class='btn btn-primary btn-xs' href='/ActivityManage/SalesPerformance/TeamPerfShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
             else if (row.activity_status == 2)
                 return "<a class='btn btn-primary btn-xs' href='/ActivityManage/SalesPerformance/TeamPerfShowActivity?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
         }
     }
 },
    {
        field: "name", title: "考核名称", align: "center", width: 200, sortable: true,
    },
     {
         field: "category", title: "考核类型", align: "center", width: 100, sortable: true,
         formatter: function (value, options, rowObject) {
             if (value == 1) {
                 return "月度考核";
             } else if (value == 2) {
                 return "销量考核";
             } else if (value == 3) {
                 return "导购人数考核";
             }
         }
     },
    { field: "company_name", title: "分公司", align: "center", width: 150, sortable: true },
    {
        field: "activity_status", title: "活动状态", align: "center", width: 100, sortable: true,
        formatter: function (value) {
            switch (value) {
                case -2: return "<span class='label label-info'>审核未结束</span>";
                case -1: return "<span class='label label-info'>未开始</span>";
                case 1: return "<span class='label label-success'>进行中</span>";
                case 2: return "<span class='label label-danger'>已结束</span>";
                default: return "<span class='label label-warning'>未知状态</span>";
            }
        }
    },
     {
         field: "approve_status", title: "审批状态", align: "center", width: 100, sortable: true,
         formatter: function (value, row, index) {
             if (value < 0)
                 return "<span class='label label-danger'>不通过</span>";
             else if (value == 100)
                 return "<span class='label label-success'>通过</span>";
             else if (value == 0)
                 return "<span class='label label-warning'>未审批</span>";
             else
                 return "<span class='label label-danger'>审批中</span>";
         }
     },
    { field: "emp_count", title: "参与人数", align: "center", width: 100, sortable: true },
    {
        field: "end_date", title: "考核周期", align: "center", width: 180, sortable: true,
        formatter: function (value, row, index) {
            var startDate = new Date(row.start_date);
            var endDate = new Date(value);
            return startDate.pattern("yyyy-MM-dd") + ' 至 ' + endDate.pattern("yyyy-MM-dd");

        }
    },
    { field: "creator_name", title: "发起人", align: "center", width: 100, sortable: true },
    { field: "create_time", title: "发起时间", align: "center", width: 150, sortable: true },
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
            url: "/ActivityManage/SalesPerformance/GetList?date=" + new Date(),         //请求后台的URL（*）
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
                    name: $('#name').val().trim(),
                    activity_status: $('#activity_status').val(),
                    approve_status: $('#approve_status').val(),
                    company_id: $('#company_id').val(),
                    startTime1: $('#startDate').val(),
                    startTime2: $('#endDate').val(),
                    category: $('#category').val(),
                    area_l1_id: $('#area_id').val(),
                };
                return param;
            },
        });
    };
    return oTableInit;
};

$("#company_id").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameList",
    search: true,
    firstText: "--请选择分公司--",
});

$("#company_id").on("change", function () {
    $("#branchcompany_id").empty();
    $("#area_id").empty();
    $("#area_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Area/GetIdNameList",
        param: { company_id: $("#company_id").val() },
        firstText: "--请选择区域--",
    });
})

function querySubmit() {
    $table.bootstrapTable({ offset: 0 }); // 第一页
    $table.bootstrapTable('removeAll');
    $table.bootstrapTable('refresh');
    $('#modalQuery').modal('hide')
}

//清空筛选
$("#clear").click(function () {
    $(".modal-body input").each(function () {
        $(this).val('');
    });
    $(".modal-body select").each(function () {
        $(this).val('').trigger("change");
    });
})

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


