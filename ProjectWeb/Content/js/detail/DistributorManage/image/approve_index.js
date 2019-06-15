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
    {
        field: "", title: "操作", align: "center", sortable: true, width: 80,
        formatter: function (value, row) {
            return "<a class='btn btn-primary btn-xs' href='/DistributorManage/Image/Approve?id=" + row.id + " ' title='审批'><i class='fa fa-check-square-o'></i></a>";
        }
    },
     {
         field: "name", title: "活动名称", align: "center", sortable: true,        
     },
    { field: "distributor_name", title: "经销商名称", align: "center", sortable: true, },

    { field: "company_name", title: "分公司", align: "center", sortable: true },
    {
        field: "activity_status", title: "活动状态", align: "center", sortable: true,
        formatter: (function (value) {
            if (value == -2) {
                return "<span class='label label-info'>审核未结束</span>"
            } else if (value == -1) {
                return "<span class='label label-warning'>未开始</span>"
            } else if (value == 1) {
                return "<span class='label label-success'>进行中</span>"
            } else if (value == 2) {
                return "<span class='label label-danger'>已结束</span>"
            }
        })
    },
    {
        field: "approve_status", title: "审批状态", align: "center", sortable: true,
        formatter: (function (value) {
            if (value == 0) {
                return "<span class='label label-warning'>未审批</span>";
            } else if (value == 100) {
                return "<span class='label label-success'>通过</span>";
            } else if (value > 0) {
                return "<span class='label label-info'>审批中</span>";
            } else
                return "<span class='label label-danger'>未通过</span>";
        })
    },
    { field: "rebate", title: "返利金额", align: "center", sortable: true },
    {
        field: "target_mode", title: "返利模式", align: "center", sortable: true,
        formatter: (function (value) {
            if (value == 1) {
                return "按时间返利"
            } else {
                return "按销量返利"
            }
        })
    },
    {
        field: "start_date", title: "活动生效日期", align: "center", sortable: true,
        formatter: (function (value) {
            var date = new Date(value);
            return date.pattern("yyyy-MM-dd");
        })
    },
    { field: "creator_name", title: "发起人", align: "center", sortable: true },
    { field: "create_time", title: "发起时间", align: "center", sortable: true },
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
            url: "/DistributorManage/Image/GetGridJson2?date=" + new Date(),        //请求后台的URL（*）
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
                    distributor_name: $('#distributor_name').val().trim(),
                    company_id: $('#company_id').val(),
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