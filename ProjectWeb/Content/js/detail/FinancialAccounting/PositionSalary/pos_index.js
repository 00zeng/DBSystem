//岗位薪资
var $table = $('#gridList');
var h = $(window).height() - 30;

var show_inactive = false;    // 初始不显示已注销
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
    { field: "name", title: "职位", align: "center", sortable: true, },
    { field: "dept_name", title: "部门", align: "center", sortable: true },
    { field: "company_linkname", title: "机构", align: "center", sortable: true },
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
            url: "/FinancialAccounting/PositionSalary/GetListPosition?date=" + new Date(),        //请求后台的URL（*）
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
                    company_id: $('#company_id').val(),
                    dept_id: $('#dept_id').val(),
                    position_name: $('#position_name').val(),
                };
                return param;
            },
            onClickRow: function (row) {
                var position_type=row.position_type;
                //'1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门经理；6-部门主管；7-行政普通员工；11-培训经理；12-培训师；21-业务经理；22-业务员；31-导购员',

                if (position_type == 22|| position_type == 21)  // 21-业务经理；22-业务员；
                    location.href = "/FinancialAccounting/PositionSalary/SalesCommissionSetting?id=" + row.id + "&name=" + encodeURI(encodeURI(row.name)) + "&company_linkname=" + encodeURI(encodeURI(row.company_linkname)) + "&company_id=" + row.company_id;
                else if (position_type == 31) //31-导购员
                    location.href = "/FinancialAccounting/PositionSalary/SalarySetting?id=" + row.id + "&name=" + encodeURI(encodeURI(row.name)) + "&company_linkname=" + encodeURI(encodeURI(row.company_linkname)) + "&company_id=" + row.company_id;
                else if (position_type == 12) //12-培训师
                    location.href = "/FinancialAccounting/PositionSalary/TrainerKPI?id=" + row.id + "&name=" + encodeURI(encodeURI(row.name)) + "&company_linkname=" + encodeURI(encodeURI(row.company_linkname)) + "&company_id=" + row.company_id;
                else if (position_type == 11) //11-培训经理
                    location.href = "/FinancialAccounting/PositionSalary/TrainerManageKPISetting?id=" + row.id + "&name=" + encodeURI(encodeURI(row.name)) + "&company_linkname=" + encodeURI(encodeURI(row.company_linkname)) + "&company_id=" + row.company_id;
                else 
                    location.href = "/FinancialAccounting/PositionSalary/OfficeKPISetting?id=" + row.id + "&name=" + encodeURI(encodeURI(row.name)) + "&company_linkname=" + encodeURI(encodeURI(row.company_linkname)) + "&company_id=" + row.company_id;

            }
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