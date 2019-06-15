var eFullList = []; // 所有员工
var eSelList = [];  // 已选员工
var empTree = [];
var month;
var empList;
var cur_company = $("#company_id").val();
var exclude_emp_status = false;
var exclude_entry_date =null;
var myCompanyInfo = top.clients.loginInfo.companyInfo;
//获取guid
var guid = "";
$(function () {
    if (myCompanyInfo.category == "分公司") {
        $("#branch").css("display", "");
        $("#company_name").text(myCompanyInfo.linkName);
    }
    else {
        $("#division").css("display", "");
        BindCompany();
        $('#company_id').on("change", function () { //切换机构后 数据清空  ① 双向列表里面的checkbox取消选中 ②日期值清空 
            cur_company = $("#company_id").val();
            $("input[id='exclude_emp_status']").prop("checked", false);
            $("input[id='exclude_entry_date']").prop("checked", false);
            exclude_emp_status = false;
            exclude_entry_date = null;
            $("#entry_date").val('');
            eFullList.splice(0, eFullList.length);  // 清空数组
            eSelList.splice(0, eSelList.length);  // 清空数组
            $duallistEmp.empty();
            $duallistEmp.bootstrapDualListbox("refresh");
            GetEmpTree(cur_company, exclude_emp_status, exclude_entry_date);
        })
    }
    GetGUID();
    TablesInit();
    GetEmpTree(cur_company, exclude_emp_status, exclude_entry_date);
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
var $duallistEmp= $('#duallistEmp').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入名称、区域关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个员工",
    infoTextEmpty: "共0个员工",
    infoTextFiltered: "",
    filterTextClear: "清空",
});

$(function () {
    $.ajax({
        url: "/SalaryCalculate/PayrollSetting/GetPayrollMonth",
        type: "get",
        dataType: "json",
        success: function (data) {
            month = data.end_date.substring(0, 7);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
});
function GetEmpTree(cur_company, exclude_emp_status, exclude_entry_date) {
    
    $duallistEmp.bootstrapDualListbox("setFilterOnValues", '',true);
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetEmpListByCompanyId",
        data: { company_id: cur_company, exclude_emp_status: exclude_emp_status, exclude_entry_date:exclude_entry_date },
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
            $duallistEmp.bootstrapDualListbox("refresh",true);
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


//转正
$("input[id='exclude_emp_status']").on("change", function () {
    eFullList.splice(0, eFullList.length);  // 清空数组
    eSelList.splice(0, eSelList.length);  // 清空数组
    $duallistEmp.empty();
    $duallistEmp.bootstrapDualListbox("refresh", true);
    if ($(this).is(':checked')) { //已转正
        exclude_emp_status = true;
        if ($("input[id='exclude_entry_date']").is(':checked') && $("#entry_date").val() != '') {
            exclude_entry_date = $("#entry_date").val();
        } else
            exclude_entry_date = null;
    } else if (!$(this).is(':checked')) {
        exclude_emp_status = false;
        if ($("input[id='exclude_entry_date']").is(':checked') && $("#entry_date").val() != '') {
            exclude_entry_date = $("#entry_date").val();
        } else
            exclude_entry_date = null;
    }
    GetEmpTree(cur_company,exclude_emp_status, exclude_entry_date);
})

//入职时间
$("input[id='exclude_entry_date']").on("change", function () {
    eFullList.splice(0, eFullList.length);  // 清空数组
    eSelList.splice(0, eSelList.length);  // 清空数组
    $duallistEmp.empty();
    $duallistEmp.bootstrapDualListbox("refresh", true);
    if (!$(this).is(':checked')) {
        $("#entry_date").val('');
    }

    if (!$(this).is(':checked') && $("#entry_date").val() != '') {
        exclude_entry_date = $("#entry_date").val();
        if ($("input[id='exclude_emp_status']").is(':checked')) {
            exclude_emp_status = true;
        } else {
            exclude_emp_status = false;
        }
    } else {
        exclude_entry_date = null;
        if ($("input[id='exclude_emp_status']").is(':checked')) {
            exclude_emp_status = true;
        } else {
            exclude_emp_status = false;
        }
    }
    GetEmpTree(cur_company,exclude_emp_status, exclude_entry_date);
})
//function test() {}
function change_by_date() {
    eFullList.splice(0, eFullList.length);  // 清空数组
    eSelList.splice(0, eSelList.length);  // 清空数组
    $duallistEmp.empty();
    $duallistEmp.bootstrapDualListbox("refresh", true);
    if ($("#entry_date").val() != '') {
        $("input[id='exclude_entry_date']").prop("checked", true);
    }
    if ($("input[id='exclude_emp_status']").is(':checked')) {
        exclude_emp_status = true;
    } else
        exclude_emp_status = false;

    if ($("input[id='exclude_entry_date']").is(':checked') && $("#entry_date").val() != '') {
        exclude_entry_date = $("#entry_date").val();
    } else {
        exclude_entry_date = null;
    }
    GetEmpTree(cur_company, exclude_emp_status, exclude_entry_date);
}
//发放福利 table
function addRow() {
    var Tr = ' <tr><td><div class="input-single formValue"><input  class="form-control required text-center" type="text" name="name" /></div></td>'
         + ' <td><select class="form-control" onchange="setType(this)" name="paid_type">'
         + ' <option value="2">礼品</option> <option value="1">工资</option><option value="3">现金</option><option value="4">其他形式</option></select></td>'
         + ' <td><div class="input-single formValue"><input class="form-control required number text-center" type="text"  name="amount"/></div></td>'
         + ' <td name="time1"><div class="input-single formValue"><input type="text" class="form-control required text-center" readonly="readonly" onfocus="WdatePicker();" name="paid_date1"/></div></td>'
         + ' <td name="time2" style="display:none"><input type="text" class="form-control required text-center" readonly="readonly" onfocus="WdatePicker({dateFmt:\'yyyy-MM\' });" name="paid_date2"  />  </td>'
         + ' <td><input type="text" class="form-control text-center" name="note"/> </td>'
         + ' <td><i onclick="addRow(this)" class="fa fa-plus-circle fa-lg margin text-blue "></i><i onclick="deleteRow(this)" class="fa fa-minus-circle fa-lg margin text-red "></i></td>'
         + ' </tr>';
    $("#benefit_table tbody").append(Tr);
}

//发放形式  工资时读取时间  其他选择时间
function setType(obj) {
    var index = $(obj).closest("tr").index();
    if ($("#benefit_table tr:eq(" + index + ") td:eq(1) option:selected").val() == 1) {
        $("#benefit_table tr:eq(" + index + ") td[name='time2']").show();
        $("#benefit_table tr:eq(" + index + ") td[name='time1']").hide();
        $("#benefit_table tr:eq(" + index + ") td[name='time2'] input").val(month);

    } else {
        $("#benefit_table tr:eq(" + index + ") td[name='time2']").hide();
        $("#benefit_table tr:eq(" + index + ") td[name='time1']").show();
    }
}


function deleteRow(e) {
    $(e).parents('tr').remove();
}



// Table START
function TablesInit() {
    initTable($tableEmp, tableEmpHeader);
    $tableEmp.bootstrapTable('load', eFullList );
    $("#emp_count").text(eFullList .length);
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


function submitForm() {
    // 提交验证
    if (!$("#form1").formValid())
        return false;

    //var data = $("#form1").formSerialize();
    var data = {};
    data["exclude_emp_status"] = $("#exclude_emp_status").prop("checked");
    if ($("#exclude_entry_date").prop("checked")) {
        if ($("#entry_date").val() == "" || $("#entry_date").val() == null) {
            $.modalAlert("入职时间不能为空！", "error");
            return false;
        }
        data["exclude_entry_date"] = $("#entry_date").val();
    }
    if (myCompanyInfo.category == "分公司") {

    } else if (myCompanyInfo.category == "事业部") {
        data["company_id"] = cur_company;
    }

    data['main_id'] = guid;
    data['month'] = month;
    if (!eSelList || eSelList.length < 1) {
        $.modalAlert("请选择员工");
        return;
    }

    data["empInfoList"] = eSelList;
    data["count"] = $("#emp_count").text();
    //福利
    var nameList = $("input[name='name']");
    var paidTypeList = $("select[name='paid_type']");
    var amountList = $("input[name='amount']");
    var paidDateList1 = $("input[name='paid_date1']");
    var paidDateList2 = $("input[name='paid_date2']");
    var noteList = $("input[name='note']");
    var addBenefitsList = [];
    var len = nameList.length;
    for (var i = 0; i < len; i++) {
        var benefit = {};
        benefit.main_id = guid;
        benefit.name = $(nameList[i]).val();
        benefit.paid_type = $(paidTypeList[i]).val();
        if (benefit.paid_type != 1) {
            benefit.paid_date = $(paidDateList1[i]).val();
        } else {
            benefit.paid_date = $(paidDateList2[i]).val();
        }
        benefit.amount = $(amountList[i]).val();
        benefit.note = $(noteList[i]).val();
        if (benefit.name == '' || benefit.paid_date == '' || benefit.amount == '') {
            $.modalAlert("福利信息不完整");
            return;
        } else if (isNaN(benefit.amount)) {
            $.modalAlert("请正确输入福利金额");
            return;
        }
        addBenefitsList.push(benefit);
    }
    data["addBenefitsList"] = addBenefitsList;
    data["id"] = guid;
    $.submitForm({
        url: "/FinancialAccounting/Benefits/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/Benefits/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}