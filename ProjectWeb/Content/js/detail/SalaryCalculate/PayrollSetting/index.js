var $table = $('#gridList');
var h = $(window).height() - 30;
var myCompanyInfo = top.clients.loginInfo.companyInfo;

$(function () {

    //1. 初始化Table
    initTable();

    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
})

var tableHeader = [
    {
        field: "month", title: "月份", align: "center", sortable: true,
        formatter: (function (value) {
            var date = new Date(value);
            return date.pattern("yyyy-MM");
        })
    },
    {
        field: "start_date", title: "起算日", align: "center", sortable: true,
        formatter: (function (value) {
            var date = new Date(value);
            return date.pattern("yyyy-MM-dd");
        })
    },
    {
        field: "end_date", title: "结算日", align: "center", sortable: true,
        formatter: (function (value) {
            var date = new Date(value);
            return date.pattern("yyyy-MM-dd");
        })
    },
    { field: "creator_name", title: "设置人", align: "center", sortable: true },
    { field: "note", title: "备注", align: "center", sortable: true },
]
function initTable() {
    if (myCompanyInfo.category != "事业部" && myCompanyInfo.category != "分公司") {
        tableHeader.push({ field: "company_name", title: "机构", align: "center", sortable: true })
    }
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
            url: "/SalaryCalculate/PayrollSetting/GetList?date=" + new Date(),        //请求后台的URL（*）
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
                };
                return param;
            },
        });
    };
    return oTableInit;
};

function OpenForm(url, title) {
    if (title == "结算日审批")
        url += "?id=" + approveId;
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