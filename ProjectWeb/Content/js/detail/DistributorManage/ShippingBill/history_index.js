//运费导入记录 
var $table = $('#gridList');
var h = $(window).height() - 30;

$(function () {
    $("#queryForm").queryFormValidate();
    //1. 初始化Table
    initTable();
    
    $("#querySubmit").click(querySubmit);
    $('#queryName').bind('input propertychange', function (event) {
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
        formatter: function (value, row) {
            console.log(row);
            return "<a class='btn btn-primary btn-xs' href='/DistributorManage/ShippingBill/Show?id=" + row.id + "'title='查看文件详情'><i class='fa fa-search'></i></a>";
        }
    },
    {
        field: "import_file", title: "导入文件名", align: "center", sortable: true,
    },
    {
        field: "status", title: "审批状态", align: "center", sortable: true,
        formatter: function (value, row, index) {
            if (value < 0)
                return "<span class='label label-danger'>不通过</span>";
            else if (value == 100)
                return "<span class='label label-success'>通过</span>";
            else if (value == 0)
                return "<span class='label label-warning'>未审批</span>";
            else
                return "<span class='label label-info'>审批中</span>";
        }
    },
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
            url: "/DistributorManage/ShippingBill/GetAllList?date=" + new Date(),        //请求后台的URL（*）
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
                    startTime1: $('#startDate').val(),
                    startTime2: $('#endDate').val(),
                    queryName: $('#queryName').val().trim(),
                    
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