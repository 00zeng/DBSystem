var $table = $('#gridList');
var h = $(window).height() - 30;
var show_inactive = false;    // 初始不显示已注销
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
        field: "", title: "操作", align: "center",
        formatter: (function (value, row) {
            if (row.area_l2_id == 0) {
                return '<a class="btn btn-info btn-xs" href="/SubordinateManage/Assignment/SalesAdd?id=' + row.id + '&name=' + encodeURI(encodeURI(row.name)) + '">指派</a>';
            } else {
                return '<a class="btn btn-info btn-xs" href="/SubordinateManage/Assignment/SalesAdd?id=' + row.id + '&name=' + encodeURI(encodeURI(row.name)) + '">指派</a> '
                + '<a class="btn btn-danger btn-xs"   data-id="' + row.id + '"  data-name="' + row.name + '" onclick="Remove(this)">解绑</a>';
            };
        })
    },
    { field: "name", title: "姓名", align: "center" },
    { field: "name_v2", title: "v2姓名", align: "center" },
    { field: "area_l2_name", title: "业务片区", align: "center" },
    { field: "area_l1_name", title: "经理片区", align: "center" },
    { field: "company_linkname", title: "所属机构", align: "center" },
]


//解绑操作
function Remove(obj) {
    var data = {};
    var id = $(obj).data('id');
    var name = $(obj).data('name');
    top.layer.confirm("您确认要解绑业务员：“" + name + "”吗？", function (index) {
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["emp_id"] = id;
        $.submitForm({
            url: "/SubordinateManage/Assignment/RemoveSale?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    ReSearch();
                    top.layer.closeAll();
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}

function initTable(isAllGuide) {
    //1.初始化Table
    var oTable = new TableInit(isAllGuide);
    oTable.Init();
}
var TableInit = function (isAllGuide) {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            height: h,
            url: "/SubordinateManage/Assignment/SalesList?date=" + new Date(),        //请求后台的URL（*）
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
                    isAllSales: $("input[id='ShowAll']").prop("checked"),
                };
                return param;
            },
            //onClickRow: function (row) {
            //    location.href = "/DistributorManage/Image/Show?id=" + row.id + "&name=" + row.distributor_name;
            //}
        });
    };
    return oTableInit;
};
//显示全部员工
$("input[id='ShowAll']").on("change", function () {
    querySubmit();
})

function querySubmit() {
    $table.bootstrapTable({ offset: 0 }); // 第一页
    $table.bootstrapTable('removeAll');
    $table.bootstrapTable('refresh');

}
//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}