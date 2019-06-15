var id = $.request("id");
var empList;
var eFullList = []; // 所有员工
var eSelList = [];  // 已选员工
var empTree = [];
var cur_company = $("#company_id").val();

//获取id
var myCompanyInfo = top.clients.loginInfo.companyInfo;
var companyList = {};    // 上级机构列表
var guid = "";
$(function () {
    $("#form1").queryFormValidate();

    if (myCompanyInfo.category == "分公司") {
        $("#branch").css("display", "");
        $("#company_name").text(myCompanyInfo.linkName);
    }
    else {
        $("#division").css("display", "");
        BindCompany();
        $('#company_id').on("change", function () { //切换机构后 数据清空  ① 双向列表里面的checkbox取消选中
            cur_company = $("#company_id").val();
            eFullList.splice(0, eFullList.length);  // 清空数组
            eSelList.splice(0, eSelList.length);  // 清空数组
            $duallistEmp.empty();
            $duallistEmp.bootstrapDualListbox("refresh");
            GetEmpTree(cur_company);
        })
    }
    GetGUID();
    TablesInit();
    GetEmpTree(cur_company);
})
function GetGUID() {
    $.ajax({
        url: "/ClientsData/GetGUID",
        async: false,
        type: "get",
        dataType: "json",
        success: function (data) {
            guid = data.guid;
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

function BindCompany() {
    $.ajax({
        url: "/OrganizationManage/Company/GetIdNameCategoryList",
        dataType: "json",
        async: false,
        success: function (data) {
            if (!data || data.length < 1)
                return;
            var optionStr = "";
            $.each(data, function (i) {
                optionStr += "<option value='" + data[i]["id"] + "' category='"
                    + data[i]["category"] + "'>" + data[i]["name"] + "</option>";
            });
            $("#company_id").append(optionStr);
            $("#company_id").select2({
                minimumResultsForSearch: 0, tags: false
            });
            cur_company = $("#company_id").val();
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}


var $tableEmp = $('#targetEmp');
var $duallistEmp = $('#duallistEmp').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入名称、区域关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个员工",
    infoTextEmpty: "共0个员工",
    infoTextFiltered: "",
    filterTextClear: "清空"
});

function GetEmpTree(cur_company) {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetEmpListByCompanyId",
        data: { company_id: cur_company},
        async: false,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            empTree.splice(0, empTree.length, data);
            $.each(data, function (index, value) {
                var e = value;
                e.main_id = guid;
                e.emp_id = e.id;
                e.emp_name = e.name;
                eFullList.push(e);
                eSelList.push(e);
                $duallistEmp.append("<option value='" + value.id + "'>" + value.display_info + "</option>");
            })
            $duallistEmp.bootstrapDualListbox("refresh");
            $tableEmp.bootstrapTable('load', eFullList);
            $("#emp_count").text(eFullList.length);
        },
        error: function (data) {
            $.modalAlert("系统出错，请稍候重试", "error")
            $tableEmp.bootstrapTable('load', eFullList);
            $("#emp_count").text(0);
        }
    })
}

// Dual List Box START
$("#modalEmp").on('hide.bs.modal', function (e) {
    eSelList.splice(0, eSelList.length);    // 清空数组
    var selIds = $duallistEmp.val();
    $.each(eFullList, function (i, item) {
        if ($.inArray(item['id'], selIds) > -1)
            eSelList.push(item);
    });
    $tableEmp.bootstrapTable('load', eSelList);
    $("#emp_count").text(eSelList.length);
})

function test() { }
function change_by_date() {
    eFullList.splice(0, eFullList.length);  // 清空数组
    eSelList.splice(0, eSelList.length);  // 清空数组
    $duallistEmp.empty();
    $duallistEmp.bootstrapDualListbox("refresh");
    GetEmpTree(cur_company);
}

// Table START
function TablesInit() {
    initTable($tableEmp, tableEmpHeader);
    $tableEmp.bootstrapTable('load', eFullList);
    $("#emp_count").text(eFullList.length);
}

var tableEmpHeader = [
    { field: "id", visible: false, },
    { field: "name", title: "名称", align: "center" },
    { field: "position_name", title: "职位", align: "center", },
    { field: "dept_name", title: "所属部门", align: "center", },
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

function getSum() {
    var num1 = $("#company_amount").val();
    var num2 = $("#emp_amount").val();
    if (isNaN(num1) || num1 == "" || num1.length == 0) {
        num1 = 0;
    }
    if (isNaN(num2) || num2 == "" || num2.length == 0) {
        num2 = 0;
    }
    var amount = Number(num1) + Number(num2);
    $("#amount").val(amount);
}

function submitForm() {
    if (!$("#form1").formValid())
        return false;

    var data = $("#form1").formSerialize();
    var listEmp = [];
    if (myCompanyInfo.category == "分公司") {

    } else if (myCompanyInfo.category == "事业部") {
        data["company_id"] = cur_company;
    }

    if (!eSelList || eSelList.length < 1) {
        $.modalAlert("请选择员工");
        return;
    }
    if ($("#month").val() == '' && $("#month").prop("checked") == false) {
        $.modalAlert("请选择发放月份");
        return;
    }
    data["listEmp"] = eSelList;
    data["count"] = $("#emp_count").text();
    data["id"] = guid;//addInfo
    $.submitForm({
                url: "/FinancialAccounting/StaySubsidy/Add?date=" + new Date(),
                param: data,
                success: function (data) {
                    if (data.state == "success") {
                        window.location.href = "/FinancialAccounting/StaySubsidy/Index?id=" + id;

                    }
                },
                error: function (data) {
                    $.modalAlert(data.responseText, 'error');
                }
            })
}