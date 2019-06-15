//留守补助
var $table = $('#gridList');
var h = $(window).height() - 30;

$(function () {

    //1. 初始化Table
    initTable();
    $("#approve_status").change(querySubmit);

    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
    $("#company_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Company/GetIdNameCategoryList",
        search: true,
        firstText: "--请选择机构--",
    });
    $(".select2-selection--single").width("220px");
    $("#company_id").change(querySubmit);

})

var tableHeader = [
    {
        field: "", title: "操作", align: "center", sortable: true, width: 80,
        formatter: function (value, row) {
            console.log(row);
            return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/StaySubsidy/Show?id=" + row.id + "'title='查看详情'><i class='fa fa-search'></i></a>";
        }
    },
     {
         field: "company_linkname", title: "所属机构", align: "center", sortable: true, width: 100,
     },
    {
        field: "month", title: "补助月份", align: "center", sortable: true, width: 100,
        formatter: (function (value) { var date = new Date(value); return date.pattern("yyyy-MM"); })
    },
    {
        field: "approve_status", title: "审批状态", align: "center", sortable: true, width: 100,
        formatter: function (cellvalue, options, rowObject) {
            if (cellvalue == 0) {
                return "<span class='label label-warning'>未审批</span>"
            } else if (cellvalue == 100) {
                return "<span class='label label-success'>通过</span>"
            } else if (cellvalue > 0) {
                return "<span class='label label-info'>审批中</span>"
            } else
                return "<span class='label label-danger'>未通过</span>"
        }
    },
    {
        field: "count", title: "发放人数", align: "center", sortable: true, width: 100,
    },
    {
        field: "company_amount", title: "补助金额(公司)", align: "center", sortable: true, width: 150,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "emp_amount", title: "补助金额(员工)", align: "center", sortable: true, width: 150,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "amount", title: "补助总额", align: "center", sortable: true, width: 100,
        formatter: function (value, row) {
            var num = row.company_amount + row.emp_amount;
            return ToThousandsStr(num);
        }
    },    
    { field: "creator_name", title: "发起人", align: "center", sortable: true, width: 100 },
    { field: "create_time", title: "发起时间", align: "center", sortable: true, width: 150 },
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
            url: "/FinancialAccounting/StaySubsidy/GetGridJson?date=" + new Date(),        //请求后台的URL（*）
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
                    approve_status: $('#approve_status').val(),
                    company_id: $("#company_id").val(),
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