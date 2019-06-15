﻿//岗位薪资
var $table = $('#gridList');
var h = $(window).height() - 30;

var show_inactive = false;    // 初始不显示已注销
$(function () {

    //1. 初始化Table
    initTable();
    $("#querySubmit").click(querySubmit);
    $('#category').change(querySubmit);

    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
})
var tableHeader = [
    {
        field: "", title: "操作", align: "center", sortable: true, width: 80,
        formatter: (function (value, row) {
            var category = row.category;
            // 1-导购薪资，2-业务提成考核，3-工龄工资，4-职等薪资导入，11-培训师KPI，12-培训经理KPI，13-部门职等KPI
            switch (category) {
                case 1:
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/GuideApprove?id=" + row.id + " ' title='审批'><i class='fa fa-check-square-o'></i></a>";
                case 2:
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/SalesApprove?id=" + row.id + " ' title='审批'><i class='fa fa-check-square-o'></i></a>";
                case 3:
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/OtherApprove?id=" + row.id + " ' title='审批'><i class='fa fa-check-square-o'></i></a>";
                case 4:
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/GradeApprove?id=" + row.id + " ' title='审批'><i class='fa fa-check-square-o'></i></a>";
                case 11:
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/TrainerApprove?id=" + row.id + " ' title='审批'><i class='fa fa-check-square-o'></i></a>";
                case 12:
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/TrainerManageApprove?id=" + row.id + " ' title='审批'><i class='fa fa-check-square-o'></i></a>";
                case 13:
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/DeptApprove?id=" + row.id + " ' title='审批'><i class='fa fa-check-square-o'></i></a>";
                default:
                    break;
            }
        })
    },
    {
        field: "category_display", title: "薪资类型", align: "center", sortable: true,       
    },
    { field: "company_name", title: "机构", align: "center", sortable: true },
    {
        field: "is_template", title: "是否公版", align: "center", sortable: true,
        formatter: function (value) {
            if (value == 1 || value == 2) {
                return "是";
            }
            else
                return "否";
        }
    },
      {
          field: "approve_status", title: "审批状态", align: "center", sortable: true,
          formatter: function (value) {
              if (value == 0) {
                  return "<span class='label label-warning'>未审批</span>";
              } else if (value == 100) {
                  return "<span class='label label-success'>通过</span>";
              } else if (value < 0) {
                  return "<span class='label label-danger'>未通过</span>";
              } else {
                  return "<span class='label label-info'>审批中</span>";
              }
          }
      },
    {
        field: "effect_date", title: "生效时间", align: "center", sortable: true,
        formatter: function (cellvalue, options, rowObject) {
            if (cellvalue == null || cellvalue == '') {
                return "审批通过后立即生效";
            } else return cellvalue.substring(0, 7);
        }
    },
    { field: "creator_name", title: "发起人", align: "center", sortable: true },
    { field: "create_time", title: "发起时间", align: "center", sortable: true },
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
            url: "/FinancialAccounting/PositionSalary/GetListApprove?date=" + new Date(),        //请求后台的URL（*）
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
                    category: $('#category').val(),
                    company_id: $('#company_id').val(),
                    is_template: $('#is_template').val(),
                    approve_status: $('#approve_status').val(),
                    startTime1: $('#startTime1').val(),
                    startTime2: $('#startTime2').val(),
                };
                return param;
            },
           
        });
    };
    return oTableInit;
};

$("#company_id").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameCategoryList",
    search: true,
    firstText: "--请选择机构--",
});

$("#company_id").on("change", function () {
    $("#branchcompany_id").empty();
    $("#dept_id").empty();
    $("#dept_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Department/GetIdNameList",
        param: { company_id: $("#company_id").val() },
        firstText: "--请选择部门--",
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