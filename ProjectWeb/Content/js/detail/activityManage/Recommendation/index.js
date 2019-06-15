//达量奖励>活动列表
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
    {
        field: "", title: "操作", align: "center", sortable: true, width: 80,
        formatter: function (value, row) {
            if (row.activity_status == -2 || row.activity_status == -1 || row.activity_status == 0)
                return "<a class='btn btn-primary btn-xs' href='/ActivityManage/Recommendation/Show?id=" + row.id + " ' title='查看活动详情'><i class='fa fa-search'></i></a>";
            else if (row.activity_status == 1 || row.activity_status == 2)
                return "<a class='btn btn-primary btn-xs' href='/ActivityManage/Recommendation/ShowActivity?id=" + row.id + " ' title='查看活动详情'><i class='fa fa-search'></i></a>";
        }
    },
     {
         field: "name", title: "活动名称", align: "center", sortable: true, width: 150,        
     },
    {
        field: "end_date", title: "活动时期", align: "center", width: 200, sortable: true,
        formatter: function (value, row, index) {
            var startDate = new Date(row.start_date);
            var endDate = new Date(value);
            return startDate.pattern("yyyy-MM-dd") + ' 至 ' + endDate.pattern("yyyy-MM-dd");

        }
     },
    { field: "company_name", title: "分公司", align: "center", width: 150, },
    {
        field: "activity_status", title: "活动状态", align: "center", sortable: true, width: 100,
        formatter: function (value,row) {
            switch (value) {
                case -2: return "<span class='label label-info'>审核未结束</span>";
                case -1: return "<span class='label label-info'>未开始</span>";
                case 1: return "<span class='label label-success'>进行中</span>";
                case 2: return "<span class='label label-danger'>已结束</span>";
                default: return "<span class='label label-warning'>未知状态</span>";
            }
        }
    },
    {
        field: "approve_status", title: "审批状态", align: "center", sortable: true, width: 100,
        formatter: function (value, row) {
            if (value == 0) {
                return "<span class='label label-warning'>未审批</span>";
            } else if (value == 100) {
                return "<span class='label label-success'>通过</span>";
            } else if (value < 0) {
                return "<span class='label label-danger'>未通过</span>";
            } else {
                return "<span class='label label-info'>审批中</span>";
            }
        }
    },

    {
        field: "emp_count", title: "参与人数", align: "center", width: 100, sortable: true,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "product_model", title: "活动机型", align: "center", width: 150,
        formatter: function (cellvalue, options, rowObject) {
            if (cellvalue == 1) {
                return "全部机型";
            } else if (cellvalue == 2) {
                return "指定单一型号";
            } else {
                return "指定多个型号";
            }
        }
    },
     {
         field: "activity_target", title: "目标销量(台)", align: "center", sortable: true, width: 120,
         formatter: function (value) {
             if (value == 0) {
                 return "-";
             } else
                 return ToThousandsStr(value);
         }
     },
     {
         field: "target_content", title: "计量方式", align: "center", sortable: true, width: 100,
         formatter: function (value) {
             if (value == 1) {
                 return "按实销量";
             } else if (value == 2) {
                 return "按下货量";
             }
         }
     },
    {
        field: "target_mode", title: "返利模式", align: "center", sortable: true, width: 120,
        formatter: function (value) {
            if (value == 1) {
                return "按完成率";
            } else if (value == 2) {
                return "按台数";
            } else if (value == 3) {
                return "按零售价";
            } else if (value == 4) {
                return "按型号";
            }
        }
    },
    {
        field: "rebate_mode", title: "奖励金额", align: "center", sortable: true, width: 120,
        formatter: function (value) {
            if (value == 1) {
                return "每台固定金额";
            } else if (value == 2) {
                return "每台批发价比例";
            } else if (value == 3) {
                return "每台零售价比例";
            } else if (value == 4) {
                return "固定总金额";
            }
        }
    },
    { field: "creator_name", title: "发起人", align: "center", width: 100, sortable: true },
    { field: "create_time", title: "发起时间", align: "center", width: 150, sortable: true },
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
            url: "/ActivityManage/Recommendation/GetList?date=" + new Date(),         //请求后台的URL（*）
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
                    name: $('#name').val().trim(),
                    activity_status: $('#activity_status').val(),
                    company_id: $('#company_id').val(),
                    approve_status: $('#approve_status').val(),
                    startTime1: $('#startTime1').val(),
                    startTime2: $('#startTime2').val(),
                    endTime1: $('#endTime1').val(),
                    endTime2: $('#endTime2').val(),
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
