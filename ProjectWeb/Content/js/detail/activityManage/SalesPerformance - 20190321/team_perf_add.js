var myCompanyInfo = top.clients.loginInfo.companyInfo;
var emp_type = 20;//员工类型

var guid = "";
var employeeTree = [];
var areaFullList = [];
var eFullList = []; // 所有员工
var eSelList = [];  // 已选员工
var $tableEmp = $('#targetEmployee');
var index;//行号
var targetModeValue;

//业务员
var $duallistSales = $('#duallistSales').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入名称、区域关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个业务员",
    infoTextEmpty: "共0个业务员",
    infoTextFiltered: "",
    filterTextClear: "清空"
});


$(function () {
    $(window).resize(function () {
        $tableEmp.bootstrapTable('resetView', {
            height: 300
        });
        
    });
    
    GetGUID();
    BindCompany();
    BindAction();
    TablesInit();
});

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
    if (myCompanyInfo.category == "分公司") {
        $("#company_id").append("<option value='" + myCompanyInfo.id + "' selected>" + myCompanyInfo.name + "</option>");
    } else if (myCompanyInfo.category == "事业部") {
        $.ajax({
            url: "/OrganizationManage/Company/GetIdNameList",
            async: false,
            type: "get",
            dataType: "json",
            contentType: "application/json",
            success: function (data) {
                $.each(data, function (index, value) {
                    $("#company_id").append("<option value='" + value.id + "'>" + value.name + "</option>");
                })
            },
            error: function (data) {
                $.modalAlert("系统出错，请稍候重试", "error");
            }
        })
    } else {
        $.modalAlert("业务考核须由分公司或事业部录入", "error");
        window.history.go(-1);
        return;
    }
    $("#company_id").on("change", function () {
        areaFullList.splice(0, areaFullList.length);
        eFullList.splice(0, eFullList.length);  // 清空数组
        eSelList.splice(0, eSelList.length);  // 清空数组
        $duallistSales.empty();
        $duallistSales.bootstrapDualListbox("refresh");     
        GetEmployeeTree();       
    })
    GetEmployeeTree();
  
}

function GetEmployeeTree() {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetEmpTree?emp_type=" + emp_type,
        data: { company_id: $("#company_id").val() },
        async: false,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            employeeTree.splice(0, employeeTree.length, data);
            if (data.length > 0) {
                $.each(data, function (index, value) {
                    areaFullList.push(value);
                    if (value.key_list.length > 0) {
                        $.each(value.key_list, function (index1, value1) {
                            var d = value1;
                            d.main_id = guid;
                            d.emp_id = d.id;
                            d.emp_name = d.name;
                            eFullList.push(d);
                            eSelList.push(d);                         
                            $duallistSales.append("<option value='" + value1.id + "'>" + value1.display_info + "</option>");                           
                        });
                    }
                })
            }
            $duallistSales.bootstrapDualListbox("refresh");           
            $tableEmp.bootstrapDualListbox("refresh");
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


function BindAction()
{
    //考核模式
    $('input[name="target_mode"]').on("change", function () {
        targetModeValue = $(this).val();                          
        if (targetModeValue == 1) {  //按比例       
            $("span[name='rebate_mode_input1']").html("完成比例*");
            $("span[name='rebate_mode_input2']").html("未完成比例*");
            $("span[name='rebate_mode_input']").html("元");

        } else if (targetModeValue == 2) {  //按人数       
            $("span[name='rebate_mode_input1']").html("完成人数*");
            $("span[name='rebate_mode_input2']").html("未完成人数*");
            $("span[name='rebate_mode_input']").html("元/人");

        }
    })

    //Dual List Box START
    $("#modalSales").on('hide.bs.modal', function (e) {
        eSelList.splice(0, eSelList.length);    // 清空数组
        var selIds = $duallistSales.val();
        $.each(eFullList, function (i, item) {
            if ($.inArray(item['id'], selIds) > -1)
                eSelList.push(item);
        });
        $tableEmp.bootstrapTable('load', eSelList);
        $("#emp_count").text(eSelList.length);
    })
    $("#modalProduct").on('hide.bs.modal', function (e) {
        var count = 0;
        pSelList.splice(0, pSelList.length);
        var selIds = $duallistProduct.val();
        $.each(pFullList, function (i, item) {
            if ($.inArray(item['id'] + "", selIds) > -1)
                pSelList.push(item);
        });
        $tableProduct.bootstrapTable('load', pSelList);
        $("#product_count").text(pSelList.length);
    })
    // Dual List Box END

}

// Table START
function TablesInit() {
    initTable($tableEmp, tableSalesHeader);
    $tableEmp.bootstrapTable('load', eFullList);
    $("#emp_count").text(eFullList.length);
  
}
var tableSalesHeader = [
    { field: "id", visible: false, },
    { field: "name", title: "姓名", align: "center" },
    { field: "area_l2_name", title: "业务片区", align: "center"},
    { field: "area_l1_name", title: "经理片区", align: "center",visible:false },
    { field: "company_name", title: "分公司", align: "center", },
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
    var data = {};
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
   
    data= $("#form1").formSerialize();
    data['category'] = 3;  
    data['empList'] = eSelList;
    data['emp_category'] = emp_type;
    $.submitForm({
        url: "/ActivityManage/SalesPerformance/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/ActivityManage/SalesPerformance/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

