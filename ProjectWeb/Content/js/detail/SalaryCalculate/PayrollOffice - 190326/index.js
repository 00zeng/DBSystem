//岗位薪资
var $table = $('#gridList');
var h = $(window).height() - 30;

var show_all = false;    
$(function () {
    $("#queryForm").queryFormValidate();
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
        field: "", title: "操作", align: "center",width: 80,
        formatter: (function (value, row) {
            return "<a class='btn btn-primary btn-xs' href='/SalaryCalculate/PayrollOffice/Add?id=" + row.id + "&name=" + encodeURI(encodeURI(row.name)) + "&grade=" + encodeURI(encodeURI(row.grade)) + "&position_name=" + encodeURI(encodeURI(row.position_name)) + "&entry_date=" + row.entry_date + " ' title='结算'>&nbsp;<i class='fa fa-rmb'></i>&nbsp;</a>";
        })
    },
   {
       field: "calculated", title: "结算状态", align: "center", width: 100,
       formatter: (function (value, row) {
           if (value == null)
               return "<span class='label label-warning'>当月未结算</span>";
           else
               return "<span class='label label-primary'>当月已结算</span>";
       })
   },
    {
        field: "name", title: "姓名", align: "center", sortable: true,
    },
    //{ field: "name_v2", title: "v2姓名", align: "center", sortable: true },
    { field: "work_number", title: "工号", align: "center", sortable: true },
    { field: "position_name", title: "职位", align: "center", sortable: true },
    { field: "grade", title: "职等", align: "center", sortable: true },
    { field: "dept_name", title: "所属部门", align: "center", sortable: true },
    { field: "company_name", title: "所属机构", align: "center", sortable: true },
    { field: "emp_category", title: "雇员类别", align: "center", sortable: true },
    { field: "entry_date", title: "入职时间", align: "center", sortable: true,
        formatter: (function (value) {
            var date = new Date(value);
            return date.pattern("yyyy-MM-dd");
        })
    },
    { field: "status", title: "在职状态", align: "center", sortable: true ,
        formatter: (function (value) {
            return (value ? "休假" : "正常");
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
            url: "/SalaryCalculate/PayrollOffice/GetListCalculate?date=" + new Date(),        //请求后台的URL（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageSize: 20,                       //每页的记录行数（*）
            pageList: [20, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            strictSearch: false,
            showColumns: true,                  //是否显示所有的列 
            showRefresh: true,                  //是否显示刷新按钮
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableHeader,
            queryParams: function (params) {
                var pageInfo = {
                    rows: params.limit,                              //页面大小
                    page: (params.offset / params.limit) + 1,        //页码
                    sidx: params.sort,
                    sord: params.order,
                    name: $('#name').val().trim(),
                    position_name: $('#position_name').val().trim(),
                    company_id: $('#company_id').val(),
                    show_all: $("input[id='show_all']").prop("checked"),
                };
                return pageInfo;
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
    $("input[id='show_all']").prop("checked", false);
})

//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}

