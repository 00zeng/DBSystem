var id = $.request("id");
$(function () {
    setInterval(function () { $('#currentTime').text((new Date()).pattern("yyyy-MM-dd HH:mm:ss")) }, 1000);
    $.ajax({
        url: "/ProductManage/PriceInfo/GetInfoMain?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            if (!data || data == null || data.length < 1) {
                $.modalAlert("信息不存在");
                window.history.go(-1);
                return;
            }
            $(".form").formSerializeShow(data[0]);
            initTable();

            $("#approve_status").text("未审批");
            $("#approve_status").addClass("label-warning");
            $("#approve_name").text(top.clients.loginInfo.empName);
            $("#approve_position_name").text(top.clients.loginInfo.positionInfo.name);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

var $table = $('#tb_table');
var importData = [];
var tableHeader =
    {
        "型号": "model",
        "颜色": "color",
        "特价机": "special_offer",
        "高端机": "high_level",
        "二级价": "price_l2",
        "事业部价格": "price_l3",
        "代理价": "price_l4",
        "广告费": "ad_fee_show",
        "内购价": "price_internal",
        "最低买断价": "price_buyout",
        "批发价": "price_wholesale",
        "零售价": "price_retail",
        "属性": "product_type",
        "开始时间": "effect_date",
        "结束时间": "expire_date",
    };


function initTable() {
    var columns = inittitle(tableHeader);
    //1.初始化Table
    var oTable = new TableInit([], columns);
    oTable.Init();
    $('.fixed-table-toolbar').append('<div class="pull-right search"><input id="queryStr" class="form-control" type="text" placeholder="型号"></div>');
    $('#queryStr').bind('keydown', function (event) {
        if (event.keyCode == "13") {
            $("#queryStr").val($("#queryStr").val().trim());
            $table.bootstrapTable({ offset: 0 }); // 第一页
            $table.bootstrapTable('removeAll');
            $table.bootstrapTable('refresh');

        }
    });
}
var TableInit = function (data, columns) {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            url: '/ProductManage/priceInfo/GetInfoPage',         //请求后台的URL（*）
            method: 'get',                      //请求方式（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "asc",                   //排序方式          
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageNumber: 1,                       //初始化加载第一页，默认第一页
            pageSize: 10,                       //每页的记录行数（*）
            pageList: [10, 50, 100, 300, 500],        //可供选择的每页的行数（*）          
            strictSearch: false,
            showColumns: true,                  //是否显示所有的列 
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: columns,
            queryParams: function (params) {
                var pageInfo = {
                    id: id,
                    rows: params.limit,                              //页面大小
                    page: (params.offset / params.limit) + 1,        //页码
                    sidx: params.sort,
                    sord: params.order,
                    queryStr: $("#queryStr").val(), //搜索框
                };
                return pageInfo;
            },
        });
    };
    return oTableInit;
};

function inittitle(gtitle) {
    var columns = [];
    for (var a in gtitle) {
        var colObj = {
            field: gtitle[a],
            title: a,
            align: "center",
            width: 140,
            sortable: true,
        };
        if (gtitle[a] == "price_l3" || gtitle[a] == "price_buyout") {
            colObj.width = 180;
            colObj.formatter = (function (value) {
                return ToThousandsStr(value);
            });
        }
        else if (gtitle[a] == "model") {
            colObj.width = 200;
        }
        else if (gtitle[a] == "special_offer") {
            colObj.formatter = (function (value) { return (value ? "是" : "否"); });
        }
        else if (gtitle[a] == "high_level") {
            colObj.formatter = (function (value) { return (value ? "是" : "否"); });
        }
        else if (gtitle[a] == "effect_date" || gtitle[a] == "expire_date") {
            colObj.width = 160;
            colObj.formatter = (function (value) {
                if (!value || value == "") return;
                return (new Date(value)).pattern("yyyy-MM-dd");
            });
        }
        else if (gtitle[a] == "price_l2" || gtitle[a] == "price_l4" || gtitle[a] == "price_internal" || gtitle[a] == "price_wholesale" || gtitle[a] == "price_retail") {
            colObj.formatter = (function (value) {
                return ToThousandsStr(value);
            });
        }
        columns.push(colObj);
    }
    return columns;
}

function submitForm() {
    var data = {};
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["id"] = id;
    data["status"] = $("input[name='status']:checked").val();
    data["approve_note"] = $("#approve_note").val();
    $.submitForm({
        url: "/ProductManage/PriceInfo/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/ProductManage/PriceInfo/ApproveIndex";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
