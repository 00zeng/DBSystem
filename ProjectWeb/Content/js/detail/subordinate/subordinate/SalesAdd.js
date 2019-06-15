var id = $.request("id");
var company_id = $.request("company_id");
$(function () {
    $("#area_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Area/GetIdNameList",
        param: { company_id: company_id },
        firstText: "--请选择区域--",
        search: true,
        change: function () {
            GetDis();
        }
    });
    $("#select2-company_id-container").width("210px"); // 改变所属机构显示的宽度，原select2插件生成的显示宽度仅150px。
})
function GetDis() {
    var curArea = $("#area_id").val();
    if (curArea > 0) {
        $.ajax({
            url: "/SubordinateManage/MySubordinate/GetDistributorList?date=" + new Date(),
            type: "get",
            data: { area_id: curArea },
            dataType: "json",
            success: function (data) {
                var columns = inittitle(tableHeader);
                //1.初始化Table
                var oTable = new TableInit(data, columns);
                oTable.Init();
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        });

    }
}
var $table = $('#tb_table');
var importData = [];
var tableHeader =
    {   
        "":"ck",
        "名称": "name",
        "地址": "address",
        "现有业务员": "sales",
        "现有导购员": "guides",
    };
function initTable() {
    var columns = inittitle(tableHeader);
    //1.初始化Table
    var oTable = new TableInit([], columns);
    oTable.Init();
}
var TableInit = function (data, columns) {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            url: '',         //请求后台的URL（*）
            data: data,
            method: 'get',                      //请求方式（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "asc",                   //排序方式
            queryParams: '',                    //传递参数（*）
            sidePagination: "client",           //分页方式：client客户端分页，server服务端分页（*）
            pageNumber: 1,                       //初始化加载第一页，默认第一页
            pageSize: 10,                       //每页的记录行数（*）
            pageList: [10, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            strictSearch: true,
            // showColumns: true,                  //是否显示所有的列 
            uniqueId: "no",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: columns,
        });
    };
    return oTableInit;
};
function inittitle(gtitle) {
    var columns = []; //; [{ field: "id", title: "", align: "center", edit: false, formatter: function (value, row, index) { return index; }}]
    for (var a in gtitle) {
        var colObj = {
            field: gtitle[a],
            title: a,
            align: "center",
            width: 100,
        };
        if (gtitle[a] == "ck") {
            colObj.radio = true;
            colObj.width = 50;
        }
        columns.push(colObj);
    }
    return columns;
}

function submitForm() {
    var data = $("#form1").formSerialize();
    data["emp_id"]=id;
    var table_info = $("#tb_table").bootstrapTable('getSelections');
    $.each(table_info, function (index, item) {
        data["id"] = item.id;
    })
    $.submitForm({
        url: "/SubordinateManage/MySubordinate/SalesAdd?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                top.layer.closeAll();
                $.currentWindow().ReSearch();
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}