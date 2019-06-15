var $table = $('#gridList');
var h = $(window).height() - 30;

$(function () {

    //1. 初始化Table
    initTable();
    $("#querySubmit").click(querySubmit);

    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
})
var tableHeader = [
    { field: "emp_name", title: "姓名", align: "center", sortable: true },
    { field: "work_number", title: "工号", align: "center", sortable: true },
    {
        field: "approve_status", title: "审批状态", align: "center", sortable: true,
        formatter: function (value) {
            if (value == 0) {
                return "未审批"
            } else if (value == 100) {
                return "通过"
            } else if (value < 0) {
                return "未通过"
            } else
                return "审批中"
        }
    },
    {
        field: "cancel_type", title: "类型", align: "center", sortable: true,
        formatter: function (value) {
            if (value == 1) {
                return "取消休假";
            } else if (value == 2) {
                return "提前回岗";
            } else
                return "未指定";
        }
    },
    { field: "position_name", title: "职位", align: "center", sortable: true },
    { field: "company_name", title: "所属机构", align: "center", sortable: true, },
    { field: "area_name", title: "所属区域", align: "center", sortable: true, },
    { field: "dept_name", title: "所属部门", align: "center", sortable: true, },
    { field: "grade", title: "原等级", align: "center", sortable: true },
    {
        field: "begin_time", title: "开始时间", align: "center", sortable: true,
        formatter: (function (value) {
            if (!!value || value != null) {
                var date = new Date(value);
                return date.pattern("yyyy-MM-dd");
            }
        })
    },
     {
         field: "end_time", title: "结束时间", align: "center", sortable: true,
         formatter: (function (value) {
             if (!!value || value != null) {
                 var date = new Date(value);
                 return date.pattern("yyyy-MM-dd");
             }
         })
     },
     { field: "days", title: "天数", align: "center", sortable: true },
     {
         field: "actual_begin_time", title: "实际开始时间", align: "center", sortable: true,
         formatter: (function (value) {
             if (!!value || value != null) {
                 var date = new Date(value);
                 return date.pattern("yyyy-MM-dd");
             }
         })
     },
     {
         field: "actual_end_time", title: "实际结束时间", align: "center", sortable: true,
         formatter: (function (value) {
             if (!!value || value != null) {
                 var date = new Date(value);
                 return date.pattern("yyyy-MM-dd");
             }
         })
     },
      { field: "actual_days", title: "实际天数", align: "center", sortable: true },
      { field: "content_reason", title: "内容", align: "center", sortable: true },
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
            url: "/HumanResource/LeavingCancel/GetListApprove?date=" + new Date(),        //请求后台的URL（*）
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
                    name: $('#name').val(),
                    work_number: $('#work_number').val(),
                    position_name: $('#position_name').val(),
                    company_id: $('#company_id').val(),
                    startTime1: $('#startDate').val(),
                    startTime2: $('#endDate').val(),
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