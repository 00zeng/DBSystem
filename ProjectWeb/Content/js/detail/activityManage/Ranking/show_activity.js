var id = $.request("id");
var $tableEmp = $('#targetEmp');
var $tableProduct = $('#targetProduct');
var $pTableWrap = $("#targetProductWrap");
var eFullList = [];  // 员工
var pSelList = [];  // 机型
var emp_category;
var ranking_content;  //排名方式
var start_date;
var end_date;
var $tableSale = $('#gridList');
$(function () {
    $("#querySubmit").on('click', querySubmit);
    $.ajax({
        url: "/ActivityManage/Ranking/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#creator_position_name").text(data.creator_position_name);
            $("#form1").formSerializeShow(data.mainInfo);
            start_date = data.mainInfo.start_date;
            end_date = data.mainInfo.end_date;
            if (data.mainInfo.activity_status == 2 || data.mainInfo.approve_status != 100) {
                $("#btn_edit").css("display", "none");
            }

            var activity_status = data.mainInfo.activity_status;
            if (activity_status == 1) {
                $("#activity_status").text("进行中");
            } else if (activity_status == -1) {
                $("#activity_status").text("未开始");
            } else if (activity_status == 2) {
                $("#activity_status").text("已结束");
            }

            //ranking_content
            emp_category = data.mainInfo.emp_category;
            switch (emp_category) {
                case 3:
                    $("#emp_category").html("导购员");
                    break;
                case 20:
                    $("#emp_category").html("业务员");
                    break;
                case 21:
                    $("#emp_category").html("业务经理");
                    break;
                default:
                    break;
            }           
            ranking_content = data.mainInfo.ranking_content;
            //Table 
            TablesInit();
            eFullList = data.empList;
            $tableEmp.bootstrapTable('load', eFullList);
            switch (ranking_content) {
                case 1:
                    $tableEmp.bootstrapTable('showColumn', 'counting_count');
                    break;
                case 2:
                    $tableEmp.bootstrapTable('showColumn', 'counting_count');
                    break;
                case 3:
                    $tableEmp.bootstrapTable('showColumn', 'counting_amount');
                    break;
                case 4:
                    $tableEmp.bootstrapTable('showColumn', 'counting_amount');
                    break;
                default:
                    break;
            }
            //奖励
            var tr = "";
            $.each(data.rewardList, function (index, item) {
                if (item.place_max == 0) {
                    tr += '<tr><td><span class="form-control no-border" name="place_min">' + ToThousandsStr(item.place_min) + '</span></td><td><span type="text" class="form-control text-center required  no-border" name="reward">' + ToThousandsStr(item.reward) + '</span></td></tr>';
                } else {
                    tr += '<tr><td><div class="input-group"><span class="form-control text-center no-border" >' + ToThousandsStr(item.place_min) + '</span><span class="input-group-addon no-border">-</span> <span class="form-control text-center no-border">' + ToThousandsStr(item.place_max) + '</span> </div></td><td><span type="text" class="form-control text-center required  no-border">' + ToThousandsStr(item.reward) + '</span></td></tr>';
                }
            })
            $("#rank_tb").append(tr);
            //查看审批
            var tr = '';
            $.each(data.approveList, function (index, item) {
                if (item.approve_note == null || item.approve_note == '') {
                    approve_note_null = "--";
                } else
                    approve_note_null = item.approve_note;
                if (item.status == 1 || item.status == -1)
                    approve_grade = ("一级审批");
                else if (item.status == 2 || item.status == -2)
                    approve_grade = ("二级审批");
                else if (item.status == 3 || item.status == -3)
                    approve_grade = ("三级审批");
                else if (item.status == 4 || item.status == -4)
                    approve_grade = ("四级审批");
                else if (item.status == 100 || item.status == -100)
                    approve_grade = ("终审");
                if (item.status > 0) {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                } else {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})
function querySubmit() {
    $tableSale.bootstrapTable({ offset: 0 }); // 第一页
    $tableSale.bootstrapTable('removeAll');
    $tableSale.bootstrapTable('refresh');
}
function TablesInit() {
    initTable($tableEmp, tableDistributorHeader);
    $tableEmp.bootstrapTable('load', eFullList);
    initTable2();
}

var tableDistributorHeader = [
    { field: "id", visible: false, },
    { field: "emp_name", title: "名称", align: "center" },
    { field: "area_l2_name", title: "业务片区", align: "center" },
    {
        field: "counting_count", title: "总销量", align: "center", visible: false,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "counting_amount", title: "总金额", align: "center", visible: false,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "final_place", title: "排名", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "reward", title: "奖金（元）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); })
    },
]
var tableSaleHeader = [
    { field: "name", title: "姓名", align: "center", sortable: true, width: 100, },
    { field: "phone_sn", title: "串码", align: "center", sortable: true, width: 150, },
    { field: "model", title: "型号", align: "center", sortable: true, width: 150 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 100 },
    {
        field: "price_wholesale", title: "批发价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_retail", title: "零售价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
     {
         field: "time", title: "时间", align: "center", sortable: true, width: 100,
         formatter: function (value, row) {
             var date = new Date(value);
             return date.pattern("yyyy-MM-dd");
         }
     },
]
function initTable($table, tableHeader) {
    //1.初始化Table
    var oTable = new TableInit($table, tableHeader);
    oTable.Init();
}
function initTable2() {
    var oTable = new TableInit2();
    oTable.Init();
}
var TableInit = function ($table, tableHeader) {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            height: 300,
            url: "",        //请求后台的URL（*）
            striped: false,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: false,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "client",           //分页方式：client客户端分页，server服务端分页（*）
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableHeader,
        });
    };
    return oTableInit;
};
var TableInit2 = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $tableSale.bootstrapTable({
            height: 400,
            url: "/ActivityManage/Ranking/GetSaleList?date=" + new Date(),        //请求后台的URL（*）
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
            showColumns: false,                  //是否显示所有的列 
            showRefresh: false,                  //是否显示刷新按钮
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableSaleHeader,
            queryParams: function (params) {
                var param = {
                    page: (params.offset / params.limit) + 1,
                    rows: params.limit,
                    sidx: params.sort,
                    sord: params.order,
                    id: id,
                    start_date: $('#start_date').text(),
                    end_date: $('#end_date').text(),
                    emp_category: emp_category,                   
                    ranking_content: ranking_content,
                    name: $('#emp_name').val().trim(),
                    phone_sn: $('#phone_sn').val().trim(),
                    model: $('#model').val().trim(),

                };
                return param;
            },
            onLoadError: function (data) {
                console.log(data);
            }
        });
    };
    return oTableInit;
};
// Table END
function OpenForm(url, title) {
    if (title == "修改结束时间")
        url += "?id=" + id +"&start_date=" + start_date+ "&end_date=" + end_date;
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