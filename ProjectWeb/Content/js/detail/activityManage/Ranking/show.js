var id = $.request("id");
var $tableEmp = $('#targetEmp');
var $tableProduct = $('#targetProduct');
var $pTableWrap = $("#targetProductWrap");
var eFullList = [];  // 员工
var pSelList = [];  // 机型
var emp_category;
var start_date;
var end_date;
$(function () {
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
            //Table 
            TablesInit();
            eFullList = data.empList;
            $tableEmp.bootstrapTable('load', eFullList);

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

function TablesInit() {
    initTable($tableEmp, tableDistributorHeader);
    $tableEmp.bootstrapTable('load', eFullList);
}

var tableDistributorHeader = [
    { field: "id", visible: false, },
    { field: "emp_name", title: "名称", align: "center" },
    { field: "area_l1_name", title: "经理片区", align: "center", },
    { field: "area_l2_name", title: "业务片区", align: "center", },
]

function initTable($table, tableHeader) {
    //1.初始化Table
    var oTable = new TableInit($table, tableHeader);
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
// Table END

function OpenForm(url, title) {
    //if (title == "修改结束时间")
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