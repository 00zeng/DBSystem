//包销信息
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
    {
        field: "check_status", title: "数据状态", align: "center", sortable: true, width: 100,
        formatter: (function (value, row) {
            switch (value) {
                case 0: return "<span class='label label-info'>待检查</span>";
                case 1: return "<span class='label label-success'>正常</span>";
                case 11: return '<div class="dropdown"><button class="btn btn-danger btn-xs" data-toggle="dropdown"> 异常 </button>'
                    + ' <div class="dropdown-menu dropdown-menu-left" style="width:300px;"><div class="modal-content"><div class="modal-header"><h4 class="modal-title">异常信息</h4></div>'
                    + ' <div class="modal-body"> <p>存在相同串码、型号、颜色、经销商等重复数据</p> </div>'
                    + ' <div class="modal-footer"> <button type="button" data-id="' + row.id + '" data-name="' + row.distributor_name + '" data-phone_sn="' + row.phone_sn + '" title="确认保留该异常数据" class="btn btn-success pull-left" onclick="Keeping(this)" >保留</button>'
                    + ' <button type="button"  data-id="' + row.id + '" data-phone_sn="' + row.phone_sn + '" title="查看对比相同串码" class="btn btn-primary"  onclick="Show(this)">查看</button>'
                    + ' </div> </div> </div> </div>';
                case 12: return "<span class='label label-danger' data-toggle='tooltip' data-placement='top' title='存在相同串码、型号、颜色、经销商等重复数据'>异常</span>"
                        + "<span class='label label-success' data-toggle='tooltip' data-placement='top' title='保留该异常数据'>已保留</span>";
                case 13: return "<span class='label label-danger' data-toggle='tooltip' data-placement='top' title='存在相同串码、型号、颜色、经销商等重复数据'>异常</span>"
                        + "<span class='label label-success'>已删除</span>";
                case 21: return '<div class="dropdown"><button class="btn btn-danger btn-xs" data-toggle="dropdown"> 异常 </button>'
                    + ' <div class="dropdown-menu dropdown-menu-left" style="width:300px;"><div class="modal-content"><div class="modal-header"><h4 class="modal-title">异常信息</h4></div>'
                    + ' <div class="modal-body"> <p>存在相同串码，型号、颜色、经销商等信息不一致</p> </div>'
                    + ' <div class="modal-footer"> <button type="button" data-id="' + row.id + '" data-name="' + row.distributor_name + '" data-phone_sn="' + row.phone_sn + '" title="确认保留该异常数据" class="btn btn-success pull-left" onclick="Keeping(this)" >保留</button>'
                    + ' <button type="button"  data-id="' + row.id + '" data-phone_sn="' + row.phone_sn + '" title="查看对比相同串码" class="btn btn-primary"  onclick="Show(this)">查看</button>'
                    + ' </div> </div> </div> </div>';
                case 22: return "<span class='label label-danger' data-toggle='tooltip' data-placement='top' title='存在相同串码，型号、颜色、经销商等信息不一致'>异常</span>"
                        + "<span class='label label-success' data-toggle='tooltip' data-placement='top' title='保留该异常数据'>已保留</span>";
                case 23: return "<span class='label label-danger' data-toggle='tooltip' data-placement='top' title='存在相同串码，型号、颜色、经销商等信息不一致'>异常</span>"
                        + "<span class='label label-success'>已删除</span>";
                case -101: return "<span class='label label-primary'>已退库</span>";
                default: return "<span class='label label-warning'>未知状态</span>";
            }
        })
    },
    { field: "distributor_name", title: "经销商", align: "center", sortable: true, width: 250 },
    { field: "model", title: "型号", align: "center", sortable: true, width: 150 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 100 },
    { field: "phone_sn", title: "串码", align: "center", sortable: true, width: 150 },
    {
        field: "price_wholesale", title: "批发价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_exclusive", title: "包销价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "guide_commission", title: "促销提成", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    { field: "area_l2_name", title: "三级地区", align: "center", sortable: true, width: 180 },
    { field: "sales_name", title: "业务员", align: "center", sortable: true, width: 100 },
    { field: "accur_time", title: "最后操作日期", align: "center", sortable: true, width: 150, formatter: (function (value) { var date = new Date(value); return date.pattern("yyyy-MM-dd  hh:mm"); }) },
    { field: "report_rule", title: "上报规则", align: "center", sortable: true, width: 180 },
    { field: "note", title: "备注", align: "center", sortable: true, width: 220 },
    { field: "import_file", title: "导入文件名", align: "center", sortable: true, width: 200 },
    { field: "creator_name", title: "导入人", align: "center", sortable: true, width: 150 },
    { field: "create_time", title: "导入时间", align: "center", sortable: true, width: 150 },
]

//function Show(obj) {
//    var curSn = $('#phone_sn').val();
//    // $table.bootstrapTable({ offset }); // 第一页
//    var curPage = $table.bootStrapTable('getOptions').offset;
//    console.log(test1, curPage);
//    $("#BackIcon").attr("data-sn", curSn);
//    $("#BackIcon").attr("data-page", curPage);

//    var id = $(obj).data('id');
//    $('#phone_sn').val($(obj).data('phone_sn'));
    
//    querySubmit();
//    $("#BackIcon").css("display","");
//}

//查看操作
function Show(obj) {
    var id = $(obj).data('id');
    $('#phone_sn').val($(obj).data('phone_sn'));
    querySubmit();
}
////返回按钮
//$("#BackIcon").click(function () {


//})

//删除操作
function Del(obj) {
    var data = {};
    var id = $(obj).data('id');
    var name = $(obj).data('name');
    var phone_sn = $(obj).data('phone_sn');
    top.layer.confirm("您确认要删除经销商：“" + name + "”，串码：“" + phone_sn + "”的包销信息吗？", function (index) {
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["idList"] = id;
        data["status"] = 2;
        $.submitForm({
            url: "/SaleManage/Exclusive/UpdateCheckStatus?date=" + new Date(),
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

//保留操作
function Keeping(obj) {
    var data = {};
    var id = $(obj).data('id');
    var name = $(obj).data('name');
    var phone_sn = $(obj).data('phone_sn');
    top.layer.confirm("您确认要保留经销商：“" + name + "”，串码：“" + phone_sn + "”的包销信息，并删除其他重复串码的信息吗？", function (index) {
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["id"] = id;
        $.submitForm({
            url: "/SaleManage/Exclusive/Keeping?date=" + new Date(),
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
            url: "/SaleManage/Exclusive/GetAllList?date=" + new Date(),        //请求后台的URL（*）
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
                    //company_id: $('#company_id').val(),
                    //area_l2_id: $('#area_id').val(),
                    startTime1: $('#startDate').val(),
                    startTime2: $('#endDate').val(),
                    main_id: $('#import_file_id').val().trim(),
                };
                return param;
            },
        })
    };
    return oTableInit;
};

//$("#company_id").bindSelect({
//    text: "name",
//    url: "/OrganizationManage/Company/GetIdNameList",
//    search: true,
//    firstText: "--请选择机构--",
//});

//$("#company_id").on("change", function () {
    //$("#branchcompany_id").empty();
    //$("#area_id").empty();
    //$("#area_id").bindSelect({
    //    text: "name",
    //    url: "/OrganizationManage/Area/GetIdNameList",
    //    param: { company_id: $("#company_id").val() },
    //    firstText: "--请选择区域--",
    //});
//})

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
