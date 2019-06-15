//价格信息
var $table = $('#gridList');
var h = $(window).height() - 30;
var re = /(?=(?!(\b))(\d{3})+$)/g;

$(function () {
    $("#queryForm").queryFormValidate();
    //1. 初始化Table
    initTable();

    $("#querySubmit").click(querySubmit);
    $("#exportExcel").click(Export);
    $('#model').bind('input propertychange', function (event) {
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
    { field: "company_linkname", title: "所属机构", align: "center", sortable: true, width: 180 },
    { field: "model", title: "型号", align: "center", sortable: true, width: 170 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 120 },
    {
        field: "expire_date", title: "生效时间", align: "center", sortable: true, width: 200,
        formatter: function (value, row, index) {
            var effectDate = new Date(row.effect_date);
            var expireDate = new Date(value);
            return effectDate.pattern("yyyy-MM-dd") + ' 至 ' + expireDate.pattern("yyyy-MM-dd");
        }
    },
    {
        field: "special_offer", title: "特价机", align: "center", sortable: true, width: 90,
        formatter: (function (value) { return (value ? "是" : "否"); })
    },
    {
        field: "high_level", title: "高端机", align: "center", sortable: true, width: 90,
        formatter: (function (value) { return (value ? "是" : "否"); })
    },
    {
        field: "price_wholesale", title: "批发价", align: "center", sortable: true, width: 90,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_retail", title: "零售价", align: "center", sortable: true, width: 90,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_l2", title: "二级价", align: "center", sortable: true, width: 90,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_l3", title: "事业部价格", align: "center", sortable: true, width: 110,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_l4", title: "代理价", align: "center", sortable: true, width: 90,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    { field: "ad_fee_show", title: "广告费", align: "center", sortable: true, width: 90 },
    {
        field: "price_internal", title: "内购价", align: "center", sortable: true, width: 90,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_buyout", title: "最低买断价", align: "center", sortable: true, width: 110,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    { field: "product_type", title: "属性", align: "center", sortable: true, width: 120 },
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
            url: "/ProductManage/PriceInfo/GetListEffect?date=" + new Date(),        //请求后台的URL（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "asc",                   //排序方式
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
                    model: $('#model').val().trim(),
                    color: $('#color').val().trim(),
                    company_id: $('#company_id').val(),
                    price_wholesale_min: $('#price_wholesale_min').val().trim(),
                    price_wholesale_max: $('#price_wholesale_max').val().trim(),
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

//重新查询  对应查询参数
function Export() {
    window.location.href = "/ProductManage/PriceInfo/Export?company_id=" + $('#company_id').val()
            + "&model=" + $('#model').val().trim()
            + "&price_wholesale_min=" + $('#price_wholesale_min').val().trim()
            + "&price_wholesale_max=" + $('#price_wholesale_max').val().trim()
}
