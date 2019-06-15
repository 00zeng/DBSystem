//岗位薪资
var $table = $('#gridList');
var h = $(window).height() - 30;

var show_inactive = false;    // 初始不显示已注销
$(function () {

    //1. 初始化Table
    initTable();
    $("#querySubmit").click(querySubmit);

    $('#category').change(querySubmit);
    $(window).resize(function () {
        $table.bootstrapTable('resetView', {
            height: $(window).height() - 30
        });
    });
})
var tableHeader = [
     {
         field: "", title: "操作", align: "center", sortable: true, width: 80,
         formatter: (function (value, row) {
             var category = row.category;
             // 1-导购薪资，2-业务提成考核，3-工龄工资，4-职等薪资导入，11-培训师KPI，12-培训经理KPI，13-部门职等KPI
             switch (category) {
                 case 1:
                     return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/GuideShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
                 case 21:
                     return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/SalesShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
                 case 22:
                     return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/SalesShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
                 case 3:
                     return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/OtherShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
                 case 4:
                     return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/GradeShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
                 case 11:
                     return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/TrainerShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
                 case 12:
                     return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/TrainerManageShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
                 case 13:
                     return "<a class='btn btn-primary btn-xs' href='/FinancialAccounting/PositionSalary/DeptShow?id=" + row.id + " ' title='查看详情'><i class='fa fa-search'></i></a>";
                 default:
                     break;
             }
         })
     },

    {
        field: "category_display", title: "薪资类型", align: "center", sortable: true,       
    },
    { field: "company_name", title: "机构", align: "center", sortable: true },
    { field: "dept_name", title: "部门", align: "center", sortable: true },
    {
        field: "is_template", title: "是否公版", align: "center", sortable: true,
        formatter: function (value) {
            if (value == 1 || value == 2) {
                return "是";
            }
            else 
                return "否";
        }
    },
    {
        field: "effect_date", title: "生效时间", align: "center", sortable: true,
        formatter: function (value) {
            if(!!value && value.length > 7)
                return value.substring(0, 7);
            return value;
        }
    }
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
            url: "/FinancialAccounting/PositionSalary/GetListCurrent?date=" + new Date(),        //请求后台的URL（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "asc",                   //排序方式
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageSize: 20,                       //每页的记录行数（*）
            pageList: [20, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            strictSearch: false,
            showColumns: true,                  //是否显示所有的列 
            showRefresh: true,                  //是否显示刷新按钮
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableHeader,
            queryParams: function (params) {
                var pageInfo = {
                    rows: params.limit,                              //页面大小
                    page: (params.offset / params.limit) + 1,        //页码
                    sidx: params.sort,
                    sord: params.order,
                    category: $('#category').val(),
                    company_id: $('#company_id').val(),
                    is_template: $('#is_template').val(),
                    startTime1: $('#startTime1').val(),
                    startTime2: $('#startTime2').val(),
                };
                return pageInfo;
            },
           //todo：返回null时的处理
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

$("#company_id").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameCategoryList",
    search: true,
    firstText: "--请选择机构--",
});

//清空筛选
$("#clear").click(function () {
    $(".modal-body input").each(function () {
        $(this).val('');
    });
    $(".modal-body select").each(function () {
        $(this).val('').trigger("change");
    });
})

//初始化页面上面的按钮事件
var ButtonInit = function () {
    var oInit = new Object();
    var postdata = {};

    oInit.Init = function () {
        $('#btn_add').click(addcolumns);
        $('#btn_edit').click(editcolumns);
        $('#btn_delete').click(deletecolumns);
    };
    return oInit;
};

function addcolumns() {
    var table = $table.bootstrapTable('getData'),
        length = table.length;;
    var type = $('#identitys input:radio:checked').val();
    var empty = cloneObj(configjson[type].data);
    empty.id = length + 1;

    $table.bootstrapTable('load', table);
    $table.bootstrapTable('selectPage', 1); //Jump to the first page
    $table.bootstrapTable('prepend', empty);

}


function deletecolumns() {
    var obj = $table.bootstrapTable('getSelections');
    var ids = $.map(obj, function (row) {
        return row.id;
    });
    if (ids.length > 0) {
        $table.bootstrapTable('remove', { field: 'id', values: ids });
    } else {
        alert("请至少选择一行删除")
    }
}

function editcolumns() {
    $table.find('.editable').editable('toggleDisabled');
}

function removeData(index) {
    $table.bootstrapTable('remove', { field: 'id', values: [index] });
}

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

function SetState(id, name) {
    var hint = "确定要" + (show_inactive ? "启用“" : "注销“") + name + "”？";
    top.layer.confirm(hint, function (index) {
        var data = { id: id };
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        $.submitForm({
            url: "/SystemManage/MsAccount/Active?date=" + new Date(),
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
    $gridList.jqGrid('setGridParam', {
        postData: { employee_name: $("#employee_name").val(), account: $("#account").val(), inactive: show_inactive }, page: 1
    }).trigger('reloadGrid');
}