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
    {
        field: "inactive", title: "状态", align: "center", sortable: true,
        formatter: (function (value) {
            return "离职";
        })
    },
    { field: "company_name", title: "所属机构", align: "center", sortable: true, },
    { field: "area_name", title: "所属区域", align: "center", sortable: true, },
    { field: "dept_name", title: "所属部门", align: "center", sortable: true, },
    { field: "name", title: "姓名", align: "center", sortable: true },
    { field: "work_number", title: "工号", align: "center", sortable: true },
    { field: "position_name", title: "职位", align: "center", sortable: true },
    { field: "gender", title: "性别", align: "center", sortable: true },
    { field: "education", title: "学历", align: "center", sortable: true },
    {
        field: "entry_date", title: "入职日期", align: "center", sortable: true,
        formatter: (function (value) {
            var date = new Date(value);
            return date.pattern("yyyy-MM-dd");
        })
    },
    {
        field: "resign_date", title: "离职日期", align: "center", sortable: true,
        formatter: (function (value) {
            var date = new Date(value);
            return date.pattern("yyyy-MM-dd");
        })
    },
    { field: "", title: "工作年限", align: "center", sortable: true, },
    { field: "birthdate", title: "出生日期", align: "center", sortable: true, },
    { field: "age", title: "年龄", align: "center", sortable: true, },
    { field: "resign_type", title: "离职类型", align: "center", sortable: true, },
    { field: "resign_reason", title: "离职原因", align: "center", sortable: true, },
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
            url: "/HumanResource/EmployeeManage/GetListResign?date=" + new Date(),        //请求后台的URL（*）
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
    firstText: "--请选择机构--",
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