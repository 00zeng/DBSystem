//员工薪资
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
        field: "name", title: "姓名", align: "center", sortable: true,width:100,
        formatter: function (value, row) {
            var position_type = row.position_type;
            var url = "";
            switch (position_type) {
                case 31:
                    return "<a href='/FinancialAccounting/EmployeeSalary/GuideApprove?id=" + row.id + "&company_name=" + encodeURI(encodeURI(row.company_name)) + "&entry_date=" + row.entry_date
                     + "&name=" + encodeURI(encodeURI(row.name)) + "&grade=" + encodeURI(encodeURI(row.grade)) + "'title='点击前往审批页面'><span class='text-muted'>" + value + "</span></a>";
                    break;
                default:
                    return "<a href='/FinancialAccounting/EmployeeSalary/EmpApprove?id=" + row.id + "&company_name=" + encodeURI(encodeURI(row.company_name)) + "&entry_date=" + row.entry_date
                     + "&name=" + encodeURI(encodeURI(row.name)) + "&grade=" + encodeURI(encodeURI(row.grade)) + "'title='点击前往审批页面'><span class='text-muted'>" + value + "</span></a>";
                    break;         
            }
        }
    },
    { field: "work_number", title: "工号", align: "center", sortable: true, width: 100 },
    {
        field: "status", title: "在职状态", align: "center", sortable: true, width: 100,
        formatter: function (cellvalue, options, rowObject) {
            if (cellvalue == 0) {
                return "在职"
            } else {
                return "休假"
            }
        }
    },
    { field: "position_name", title: "职位", align: "center", sortable: true, width: 100 },
    { field: "grade", title: "等级", align: "center", sortable: true, width: 100 },
    { field: "emp_category", title: "雇员类别", align: "center", sortable: true, width: 100 },
    {
        field: "category", title: "类型", align: "center", sortable: true, width: 100,
        formatter: function (value, row, rowObject) {
            if (value == 1) {
                return "基本薪资"
            } else if (value == 2) {
                return "KPI补贴"
            } else if (value == 3) {
                return "薪资补助"
            }

        }
    },
    { field: "salary_category", title: "类别", align: "center", sortable: true, width: 100 },
    { field: "area_name", title: "所属区域", align: "center", sortable: true, width: 100 },
    { field: "dept_name", title: "所属部门", align: "center", sortable: true, width: 100, },
    { field: "company_name", title: "所属机构", align: "center", sortable: true, width: 100 },
    { field: "supervisor_name", title: "上级主管", align: "center", sortable: true, width: 100 },
    { field: "entry_date", title: "入职时间", align: "center", sortable: true, width: 100, formatter: (function (value) { var date = new Date(value); return date.pattern("yyyy-MM-dd"); }) },
    { field: "creator_name", title: "发起人", align: "center", sortable: true, width: 100 },
    { field: "create_time", title: "发起时间", align: "center", sortable: true, width: 150 },
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
            url: "/FinancialAccounting/EmployeeSalary/GetGridJson2?date=" + new Date(),        //请求后台的URL（*）
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
                    company_id: $('#company_id').val(),
                    dept_id: $('#dept_id').val(),
                    position_id: $('#position_id').val(),
                    category: $('#category').val(),
                };
                return param;
            },
            //onClickRow: function (row) {
            //    var position_type = row.position_type;
            //    var url = "";
            //    switch (position_type) {
            //        case 31:
            //            url = "/FinancialAccounting/EmployeeSalary/GuideApprove?id=" + row.id + "&company_name=" + encodeURI(encodeURI(row.company_name)) + "&entry_date=" + row.entry_date
            //             + "&name=" + encodeURI(encodeURI(row.name)) + "&grade=" + encodeURI(encodeURI(row.grade));
            //            break;
            //        default:
            //            url = "/FinancialAccounting/EmployeeSalary/EmpApprove?id=" + row.id + "&company_name=" + encodeURI(encodeURI(row.company_name)) + "&entry_date=" + row.entry_date
            //             + "&name=" + encodeURI(encodeURI(row.name)) + "&grade=" + encodeURI(encodeURI(row.grade));
            //            break;
            //    }
            //    window.location.href = url;
            //}
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
        param: {company_id: $("#company_id").val() , dept_id: $("#dept_id").val() },
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