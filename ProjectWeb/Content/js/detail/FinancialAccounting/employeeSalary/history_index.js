//员工薪资
var $table = $('#gridList');
var h = $(window).height() - 30;

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
        formatter: (function (value, row) {
            var category = row.category;
            var position_type = row.position_type;
            if (category == 1) {//基本薪资
                if (position_type == 31) {//导购
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/EmployeeSalary/GuideHistoryShow?id=" + row.id + " ' title='查看薪资详情'><i class='fa fa-search'></i></a>";
                } else if (row.position_type == 12 || row.position_type == 11) {//培训
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/EmployeeSalary/TrainerSalaryHistoryShow?id=" + row.id + " ' title='查看薪资详情'><i class='fa fa-search'></i></a>";
                }
                else {//行政人员
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/EmployeeSalary/OfficeHistoryShow?id=" + row.id + "'title='查看薪资详情'><i class='fa fa-search'></i></a>";

                }
            }
            else if ( category == 2 ) 
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/EmployeeSalary/KPISubsidyHistoryShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
            else if ( category == 3 )
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/EmployeeSalary/SubsidyHistoryShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
            else if ( category == 4 )
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/EmployeeSalary/SalesHistoryShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
            else if ( category == 5 )
                    return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/EmployeeSalary/TrainerHistoryShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
        })
    },
    {
        field: "name", title: "姓名", align: "center", sortable: true, width: 120
    },
    {
        field: "work_number", title: "工号", align: "center", sortable: true, width: 120
    },
    {
        field: "category", title: "薪资类型", align: "center", sortable: true, width: 150,
        formatter: function (value, row) {
            if (value == 1) {
                if (row.position_type == 31) {
                    return "导购薪资";
                } if (row.position_type == 12 || row.position_type == 11) {
                    return "培训薪资";
                } else {
                    return "行政薪资";
                }
            }
            else if (value == 2) {
                return "KPI补助";
            }
            else if (value == 3) {
                return "薪资补助";
            }
            else if (value == 4) {
                return "业务薪资";
            }
            else if (value == 5) {
                return "培训考核";
            }
        }
    },
    { field: "position_name", title: "职位", align: "center", sortable: true, width: 150 },
    { field: "dept_name", title: "部门", align: "center", sortable: true, width: 150 },
    { field: "company_name", title: "机构", align: "center", sortable: true, width: 150 },
    {
        field: "effect_status", title: "生效状态", align: "center", sortable: true, width: 120,
        formatter: function (value) {
            if (value == -2) {
                return "<span class='label label-info'>审核未结束</span>";
            } else if (value == -1) {
                return "<span class='label label-warning'>未生效</span>";
            } else if (value == 1) {
                return "<span class='label label-success'>有效</span>";
            } else {
                return "<span class='label label-danger'>已失效</span>";
            }
        }
    },
    { field: "entry_date", title: "入职时间", align: "center", sortable: true, width: 150, formatter: (function (value) { var date = new Date(value); return date.pattern("yyyy-MM-dd"); }) },
    { field: "create_time", title: "创建时间", align: "center", sortable: true, width: 150 },
    { field: "creator_name", title: "创建人名称", align: "center", sortable: true, width: 120 },
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
            url: "/FinancialAccounting/EmployeeSalary/GetHistoryList?date=" + new Date(),        //请求后台的URL（*）
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
                    endTime1: $('#endTime1').val(),
                    endTime2: $('#endTime2').val(),
                    startTime1: $('#startTime1').val(),
                    startTime2: $('#startTime2').val(),
                };
                return param;
            },
            // onClickRow: function (row) {
            //    var category = row.category;
            //    var url = "";
            //    // 1-导购薪资，2-业务提成考核，3-工龄工资，4-职等薪资导入，11-培训师KPI，12-培训经理KPI，13-部门职等KPI
            //    switch (category) {
            //        case 1:
            //            url = "/FinancialAccounting/PositionSalary/GuideShow?id=" + row.id; break;
            //        case 2:
            //            url = "/FinancialAccounting/PositionSalary/SalesShow?id=" + row.id; break;
            //        case 3:
            //            url = "/FinancialAccounting/PositionSalary/OtherShow?id=" + row.id; break;
            //        case 4:
            //            url = "/FinancialAccounting/PositionSalary/GradeShow?id=" + row.id; break;
            //        case 11:
            //            url = "/FinancialAccounting/PositionSalary/TrainerKPIShow?id=" + row.id; break;
            //        case 12:
            //            url = "/FinancialAccounting/PositionSalary/TrainerManageKPIShow?id=" + row.id; break;
            //        case 13:
            //            url = "/FinancialAccounting/PositionSalary/DeptShow?id=" + row.id + "&company_name=" + encodeURI(encodeURI(row.company_name)) + "&position_name=" + encodeURI(encodeURI(row.position_name)); break;
            //        default:
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