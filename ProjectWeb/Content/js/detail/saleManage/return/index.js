//退库信息
var phone_sn = $.request("phone_sn");
var $table = $('#gridList');
var h = $(window).height() - 30;

$(function () {
    $("#queryForm").queryFormValidate();
    //1. 初始化Table
    initTable();

    $("#querySubmit").click(querySubmit);
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
    if (phone_sn != "" || phone_sn != null) {
        $('#phone_sn').val(phone_sn);
        querySubmit();
    }
})
var tableHeader = [
    { field: "bill_name", title: "单号", align: "center", sortable: true },
    { field: "accur_time", title: "日期", align: "center", sortable: true, formatter: (function (value) { var date = new Date(value); return date.pattern("yyyy-MM-dd hh:mm"); }) },
    { field: "distributor_name", title: "门店", align: "center", sortable: true },
    { field: "model", title: "型号", align: "center", sortable: true },
    { field: "color", title: "颜色", align: "center", sortable: true },
    { field: "phone_sn", title: "串码", align: "center", sortable: true },
    { field: "import_file", title: "导入文件名", align: "center", sortable: true },
    { field: "creator_name", title: "导入人", align: "center", sortable: true },
    { field: "create_time", title: "导入时间", align: "center", sortable: true },

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
            url: "/SaleManage/Return/GetGridJson?date=" + new Date(),        //请求后台的URL（*）
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
                    bill_name: $('#bill_name').val().trim(),
                    phone_sn: $('#phone_sn').val().trim(),
                    distributor_name: $('#distributor_name').val().trim(),
                    model: $('#model').val().trim(),
                    startTime1: $('#startDate').val(),
                    startTime2: $('#endDate').val(),
                    import_file_id: $('#import_file_id').val().trim(),
                };
                return param;
            },
        });
      
    };
    return oTableInit;
};

function querySubmit() {
    $table.bootstrapTable({ offset: 0 }); // 第一页
    $table.bootstrapTable('removeAll');
    $table.bootstrapTable('refresh');
    $('#modalQuery').modal('hide')
}

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

//清空筛选
$("#clear").click(function () {
    $(".modal-body input").each(function () {
        $(this).val('');
    });
    $(".modal-body select").each(function () {
        $(this).val('').trigger("change");
    });
})

//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}