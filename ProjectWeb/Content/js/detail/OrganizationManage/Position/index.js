//职位管理
var $table = $('#gridList');
var h = $(window).height() - 30;

$(function () {
    $("#queryForm").queryFormValidate();
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
        field: "button", title: '操作', align: 'center',width:100,
        formatter: function (value, row, index) {
            var name = row.name;
            var id = row.id;
            var btnString = "";
            btnString = "<a name='show' class='btn btn-primary btn-xs auth'  href='/OrganizationManage/Position/Edit?id=" + row.id + "'>修改</a> "
            if (row.delible) {
                btnString += "<a name='Del' class='btn btn-danger btn-xs auth'  href='javascript:;' onclick='Delete(\""
                                        + id + "\", \"" + name + "\")'>删除</a> "
            }
            return btnString;
        }
    },
    { field: "name", title: "职位名称", align: "center", sortable: true, width: 100 },
    { field: "dept_name", title: "所属部门", align: "center", sortable: true, width: 100 },
    { field: "company_linkname", title: "所属机构", align: "center", sortable: true, width: 180 },
    {
        field: "position_type", title: "职位类型", align: "center", sortable: true, width: 100,
        formatter: function (cellvalue, options, rowObject) {
            switch (cellvalue) {
                case 1: return "事业部总经理";
                case 2: return "事业部助理";
                case 3: return "分公司总经理";
                case 4: return "分公司助理";
                case 5: return "部门经理";
                case 6: return "部门主管";
                case 7: return "行政普通员工";
                case 11: return "培训经理";
                case 12: return "培训师";
                case 21: return "业务经理";
                case 22: return "业务员";
                case 31: return "导购员";
                default: return "行政普通员工";
            }
        }
    },
    { field: "note", title: "备注说明", align: "center", sortable: true, width: 100 },
    { field: "creator_name", title: "创建人", align: "center", sortable: true, width: 100 },
    {
        field: "create_time", title: "创建时间", align: "center", sortable: true, width: 100,
        formatter: (function (value) {
            var date = new Date(value);
            return date.pattern("yyyy-MM-dd");
        })
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
            url: "/OrganizationManage/Position/GetGridJson?date=" + new Date(),         //请求后台的URL（*）
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
                    company_id: $('#company_id').val(),
                    dept_id: $('#dept_id').val(),
                };
                return param;
            },
        });
    };
    return oTableInit;
};

$("#company_id").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameCategoryList",
    search: true,
    firstText: "--请选择所属机构--",
});

$("#company_id").on("change", function () {
    $("#branchcompany_id").empty();
    $("#dept_id").empty();
    $("#dept_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Department/GetIdNameList",
        param: { company_id: $("#company_id").val() },
        firstText: "--请选择所属部门--",
    });
})

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
        height: "340px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}

/*删除数据*/
function Delete(id, name) {
    top.layer.confirm("确认要删除: " + name, function (index) {
        var data = {};
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["id"] = id;

        $.ajax({
            url: "/OrganizationManage/Position/Delete?date=" + new Date(),
            type: "post",
            data: data,
            success: function (data) {
                $.currentWindow().location.reload();
                top.layer.msg("删除成功");
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