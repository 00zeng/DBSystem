var $table = $('#gridList');
var h = $(window).height() - 30;

var show_inactive = false;    // 初始不显示已注销
$(function () {
    //1. 初始化Table
    initTable();
    $("#querySubmit").click(querySubmit);
    $('#name').bind('input propertychange', function (event) {
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
    { field: "status", title: "状态", align: "center", sortable: true },
    { field: "name", title: "姓名", align: "center", sortable: true },
    { field: "phone", title: "手机号", align: "center", sortable: true },
    { field: "wechat", title: "微信号", align: "center", sortable: true },   
    { field: "distributor_name", title: "经销商名称", align: "center", sortable: true },
    { field: "address", title: "详细地址", align: "center", sortable: true, },
    { field: "guide_name", title: "挂钩导购员", align: "center", sortable: true },
    { field: "entry_date", title: "入职时间", align: "center", sortable: true },
    { field: "", title: "离职时间", align: "center", sortable: true },
    { field: "account", title: "账户名", align: "center", sortable: true },
    { field: "account_status", title: "账户状态", align: "center", sortable: true },
    { field: "creator_name", title: "创建人", align: "center", sortable: true },
    {
        field: "create_time", title: "创建时间", align: "center", sortable: true,
        formatter: (function (value) {
            var date = new Date(value);
            return date.pattern("yyyy-MM-dd");
        })
    },
     {
         field: "button", title: '操作', align: 'center',
         formatter: function (value, row, index) {
             var btnString = "";
             btnString = "<a name='show' class='btn bg-purple btn-xs auth'  href='/DistributorManage/DistributorManage/Show?id=" + row.id + "'>查看</a> "
             + "<a name='show' class='btn bg-purple btn-xs auth'  href='/DistributorManage/DistributorManage/Edit?id=" + row.id + "'>修改</a> "
             return btnString;
         }
     },
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
            url: "//DistributorManage/ClerkManage/GetGridJson?date=" + new Date(),        //请求后台的URL（*）
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
                    name: $('#name').val(),
                    //sales_name: $('#sales_name').val(),
                    //company_id: $('#company_id').val(),
                    //area_id: $('#area_id').val(),
                };
                return param;
            },
        });
    };
    return oTableInit;
};



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

//删除
function Delete(id, name) {
    top.layer.confirm("确认要删除: “" + name + "”？", function (index) {
        var data = { id: id };
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }

        $.submitForm({
            url: "/SystemManage/MsAccount/Delete?date=" + new Date(),
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

//重新查询
function ReSearch() {
    $table.bootstrapTable('refresh');
}