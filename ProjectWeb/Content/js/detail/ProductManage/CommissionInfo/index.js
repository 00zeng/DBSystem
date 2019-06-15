//价格信息
var $table = $('#gridList');
var h = $(window).height() - 30;

$(function () {
    $("#queryForm").queryFormValidate();
    //1. 初始化Table
    initTable();

    $("#querySubmit").click(querySubmit);
    $('#model').bind('input propertychange', function (event) {
        $table.bootstrapTable({ offset: 0 }); // 第一页
        $table.bootstrapTable('removeAll');
        $table.bootstrapTable('refresh');

    });

    $("#exportExcel").click(Export);
    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
})
var tableHeader = [
    { field: "company_linkname", title: "所属机构", align: "center", sortable: true,width:180 },
    { field: "model", title: "型号", align: "center", sortable: true, width: 180 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 100 },
    { field: "guide_commission", title: "导购提成", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
},
    {
        field: "exclusive_commission", title: "包销提成", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "expire_date", title: "生效时间", align: "center", sortable: true, width: 200,
        formatter: function (value, row, index) {
            var effectDate = new Date(row.effect_date);
            var expireDate = new Date(value);
            return effectDate.pattern("yyyy-MM-dd") + ' 至 ' + expireDate.pattern("yyyy-MM-dd");
        }
    },
    {
        field: "salary_include", title: "是否核算底薪", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return (value ? "是" : "否"); })
    },
    { field: "note", title: "备注", align: "center", sortable: true, width: 100 },
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
            url: "/ProductManage/CommissionInfo/GetListEffect?date=" + new Date(),        //请求后台的URL（*）
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
                    model: $('#model').val().trim(),
                    guide_commission_min: $('#guide_commission_min').val().trim(),
                    guide_commission_max: $('#guide_commission_max').val().trim(),
                    exclusive_commission_min: $('#exclusive_commission_min').val().trim(),
                    exclusive_commission_max: $('#exclusive_commission_max').val().trim(),
                    startTime1: $('#startDateFrom').val(),
                    startTime2: $('#startDateTo').val(),
                    endTime1: $('#endDateFrom').val(),
                    endTime2: $('#endDateTo').val(),
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
    firstText: "--请选择所属机构--",
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

//重新查询
function Export() {
    window.location.href = "/ProductManage/CommissionInfo/Export?company_id=" + $('#company_id').val()
            + "&model=" + $('#model').val().trim()
            + "&guide_commission_min=" + $('#guide_commission_min').val().trim()
            + "&guide_commission_max=" + $('#guide_commission_max').val().trim()
            + "&exclusive_commission_min=" + $('#exclusive_commission_min').val().trim()
            + "&exclusive_commission_max=" + $('#exclusive_commission_max').val().trim()
            + "&startTime1=" + $('#startDateFrom').val().trim()
            + "&startTime2=" + $('#startDateTo').val().trim();
            + "&endTime1=" + $('#endDateFrom').val().trim()
            + "&endTime2=" + $('#endDateTo').val().trim();
}