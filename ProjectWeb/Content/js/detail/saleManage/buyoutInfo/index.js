//买断信息
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
})
var tableHeader = [
    { field: "distributor_name", title: "经销商", align: "center", sortable: true, width: 130 },
    { field: "area_name", title: "区域", align: "center", sortable: true, width: 130 },
    {
        field: "category", title: "买断类型", align: "center", sortable: true, width: 130 ,
        formatter: function (cellvalue) {
            if (cellvalue == 1)
                return "仓库买断";
            else
                return "门店买断";
        }
    },
    { field: "phone_sn", title: "串码", align: "center", sortable: true, width: 150 },
    { field: "model", title: "型号", align: "center", sortable: true, width: 130 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 130 },
    {
        field: "price_wholesale", title: "批发价", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_buyout", title: "买断价", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_retail", title: "零售价", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    { field: "salesmanager_name", title: "区域经理", align: "center", sortable: true, width: 130 },
    { field: "sales_name", title: "业务员", align: "center", sortable: true, width: 130 },
    { field: "guide_name", title: "促销员", align: "center", sortable: true, width: 130 },
    {
        field: "buyout_refund", title: "买断补差", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "guide_commission", title: "促销提成", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "sales_commission", title: "业务提成", align: "center", sortable: true, width: 130,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    { field: "create_time", title: "申请时间", align: "center", sortable: true, width: 150 },
    { field: "create_time", title: "导入时间", align: "center", sortable: true, width: 150 },
]

//查看操作
function Show(obj) {
    var id = $(obj).data('id');
    $('#phone_sn').val($(obj).data('phone_sn'));
    querySubmit();
}
//删除操作
function Del(obj) {
    var data = {};
    var id = $(obj).data('id');
    var name = $(obj).data('name');
    var phone_sn = $(obj).data('phone_sn');
    top.layer.confirm("您确认要删除经销商：“" + name + "”串码：“" + phone_sn + "”吗？", function (index) {
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["idList"] = id;
        data["status"] = 2;
        $.submitForm({
            url: "/SaleManage/BuyoutInfo/UpdateCheckStatus?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    $.currentWindow().ReSearch();
                    top.layer.closeAll();
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}
//处理操作
function Check(obj) {
    var data = {};
    var id = $(obj).data('id');
    var name = $(obj).data('name');
    var phone_sn = $(obj).data('phone_sn');
    top.layer.confirm("您确认已处理经销商：“" + name + "”串码：“" + phone_sn + "”了吗？", function (index) {
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["idList"] = id;
        data["status"] = 1;
        $.submitForm({
            url: "/SaleManage/BuyoutInfo/UpdateCheckStatus?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    $.currentWindow().ReSearch();
                    top.layer.closeAll();
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}

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
            url: "/SaleManage/BuyoutInfo/GetListAll?date=" + new Date(),        //请求后台的URL（*）
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
                    phone_sn: $('#phone_sn').val().trim(),
                    model: $('#model').val().trim(),
                    distributor_name: $('#distributor_name').val().trim(),
                    sales_name: $('#sales_name').val().trim(),
                    category: $('#category').val(),
                    guide_name: $('#guide_name').val().trim(),
                };
                return param;
            },
            onClickRow: function (row) {
                location.href = "/SaleManage/BuyoutInfo/Show?id="+row.main_id;
            }
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