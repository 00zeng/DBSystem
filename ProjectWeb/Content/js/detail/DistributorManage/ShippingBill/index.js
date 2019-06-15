//出库信息
var $table = $('#gridList');
var h = $(window).height() - 30;

$(function () {
    $("#queryForm").queryFormValidate();
    //1. 初始化Table
    initTable();

    $("#querySubmit").click(querySubmit);
    $("#exportExcel").click(Export);
    $('#distributor_name').bind('input propertychange', function (event) {
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
    { field: "shipping_bill", title: "单号", align: "center", sortable: true, width: 170 },
    { field: "company_linkname", title: "所属机构", align: "center", sortable: true, width: 180 },
    { field: "distributor_name", title: "经销商", align: "center", sortable: true, width: 150 },
    {
        field: "quantity", title: "数量", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "amount", title: "金额", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    { field: "product_detail", title: "明细", align: "center", sortable: true, width: 450 },
    { field: "bill_date", title: "制单时间", align: "center", sortable: true, width: 170 },
    { field: "is_received", title: "是否收货", align: "center", sortable: true, width: 100 },
    { field: "note", title: "备注", align: "center", sortable: true, width: 180 },
    { field: "operator_name", title: "经手人", align: "center", sortable: true, width: 100 },
    { field: "is_printed", title: "是否打印", align: "center", sortable: true, width: 100 },
    { field: "extend_note", title: "扩展备注", align: "center", sortable: true, width: 180 },
    { field: "schedule_note", title: "计划单备注", align: "center", sortable: true, width: 180 },
    { field: "schedule_bill", title: "计划单自定义单号", align: "center", sortable: true, width: 150 },
    { field: "import_file", title: "导入文件名", align: "center", sortable: true, width: 200 },
    { field: "creator_name", title: "导入人", align: "center", sortable: true, width: 150 },
    { field: "create_time", title: "导入时间", align: "center", sortable: true, width: 170 },
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
            url: "/DistributorManage/ShippingBill/GetEffectList?date=" + new Date(),         //请求后台的URL（*）
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
                    shipping_bill: $('#shipping_bill').val().trim(),
                    distributor_name: $('#distributor_name').val().trim(),
                    total_count_min: $('#total_count_min').val().trim(),
                    total_count_max: $('#total_count_max').val().trim(),
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
    url: "/OrganizationManage/Company/GetIdNameList",
    search: true,
    firstText: "--请选择分公司--",
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
    window.location.href = "/DistributorManage/ShippingBill/Export?company_id=" + $('#company_id').val()
            + "&shipping_bill=" + $('#shipping_bill').val().trim()
            + "&distributor_name=" + $('#distributor_name').val().trim()
            + "&total_count_min=" + $('#total_count_min').val().trim()
            + "&total_count_max=" + $('#total_count_max').val().trim()
            + "&startTime1=" + $('#startTime1').val().trim()
            + "&startTime2=" + $('#startTime2').val().trim();
}