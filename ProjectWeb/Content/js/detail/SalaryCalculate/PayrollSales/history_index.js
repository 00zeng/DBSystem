//业务工资历史
var $table = $('#gridList');
var h = $(window).height() - 30;

$(function () {
    $("#queryForm").queryFormValidate();

    //1. 初始化Table
    initTable();
    $('#name').bind('input propertychange', function (event) {
        $table.bootstrapTable({ offset: 0 }); // 第一页
        $table.bootstrapTable('removeAll');
        $table.bootstrapTable('refresh');

    });

    $("#querySubmit").click(querySubmit);
    $("#exportExcel").click(Export);
    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
})
var tableHeader = [
     {
         field: "", title: "操作", align: "center",width:100,
         formatter: (function (value, row) {
             return "<a class='btn btn-primary btn-xs' href='/SalaryCalculate/PayrollSales/Show?id=" + row.id + " ' title='查看'><i class='fa fa-search'></i></a>";
         })
     },
    { field: "name", title: "姓名", align: "center", sortable: true, width: 100, },
    { field: "work_number", title: "工号", align: "center", sortable: true, width: 100, },
    {
        field: "month", title: "结算月份", align: "center", sortable: true, width: 100,
        formatter: (function (value) {
            var date = new Date(value);
            return date.pattern("yyyy-MM");
        })
    },
    {
        field: "actual_salary", title: "实发工资", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    { field: "position_name", title: "岗位", align: "center", width: 120, },
    { field: "grade", title: "职等", align: "center", sortable: true, width: 100, },
     {
         field: "entry_date", title: "入职时间", align: "center", sortable: true, width: 120,
         formatter: (function (value) {
             var date = new Date(value);
             return date.pattern("yyyy-MM-dd");
         })
     },
    {
        field: "base_salary", title: "基本工资", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "position_salary", title: "岗位工资", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "house_subsidy", title: "住房补助", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "attendance_reward", title: "全勤奖", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "seniority_salary", title: "工龄工资", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "traffic_subsidy", title: "交通补贴", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    //{ field: "sale_commission", title: "KPI-销量提成", align: "center", sortable: true },
    //{ field: "buyout_commission", title: "KPI-买断提成", align: "center", sortable: true },
    //{ field: "outstorage_commission", title: "KPI-下货提成", align: "center", sortable: true },
    {
        field: "salary", title: "应发工资", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "insurance_fee", title: "扣社保", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "resign_deposit", title: "扣风险金", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "leaving_deduction", title: "请假扣款", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "proxy_subsidy", title: "区域经理代管补助", align: "center", sortable: true, width: 160,
        formatter: (function (value) { return ToThousandsStr(value); })
    },    
    { field: "note", title: "备注", align: "center", width: 200 },
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
            url: "/SalaryCalculate/PayrollSales/GetListHistory?date=" + new Date(),        //请求后台的URL（*）
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
                    company_id: $('#company_id').val(),
                    dept_id: $('#dept_id').val(),
                    position_id: $('#position_id').val(),
                    startTime1: $('#startTime1').val(),
                    startTime2: $('#startTime2').val(),
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
    firstText: "--请选择所属机构--",
});

$("#company_id").on("change", function () {
    $("#branchcompany_id").empty();
    $("#dept_id").empty();
    $("#dept_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Department/GetIdNameList",
        param: { company_id: $("#company_id").val() },
        firstText: "--请选择所属部门--",
    });
})

$("#dept_id").on("change", function () {
    $("#branchcompany_id").empty();
    $("#position_id").empty();
    $("#position_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Position/GetIdNameList",
        param: { company_id: $("#company_id").val(), dept_id: $("#dept_id").val() },
        firstText: "--请选择职位--",
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
    $("#dept_id").empty();
    $("#position_id").empty();
})

//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}

//重新查询   参数同查询参数
function Export() {
    window.location.href = "/SalaryCalculate/PayrollSales/Export?company_id=" + $('#company_id').val()
            + "&position_name=" + $('#position_name').val()
            + "&name=" + $('#name').val();
}