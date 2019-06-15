var myCompanyInfo = top.clients.loginInfo.companyInfo;
var eFullList = []; // 所有导购员
var eSelList = [];  // 已选导购员
var $tableEmp = $('#targetEmp');
var guid = "";
var guideTree = [];
var areaFullList = [];
//导购员
var $duallistGuide = $('#duallistGuide').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入名称、区域关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个导购员",
    infoTextEmpty: "共0个导购员",
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
    ActionBind();
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
$("#pos_category").on("change", function () {
    var pos_category = $("#pos_category").val();
    if (pos_category == 1) {
        $("#deputyPos").css("display", "");
        $("#annualBonus").css("display", "");
        $("#salary_base").css("display", "none");
        if ($("input[name='guide_base_type']:checked").val() == 1) {
            $("#salary_commission").css('display', '');
        }
    } else {
        $("#annualBonus").css("display", "none");
        $("#deputyPos").css("display", "none");
        $("#salary_base").css("display", "");
        $("#salary_commission").css('display', 'none');
    }


    $.ajax({
        url: "/OrganizationManage/Company/GetIdNameList?pos_category=" + $("#pos_category").val(),
        async: false,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
           data
        },
        error: function (data) {
            $.modalAlert("系统出错，请稍候重试", "error");
        }
    })

})

function BindCompany() {
    if (myCompanyInfo.category == "分公司") {
        $("#company").append("<option value='" + myCompanyInfo.id + "' selected>" + myCompanyInfo.name + "</option>");
    } else if (myCompanyInfo.category == "事业部") {
        $.ajax({
            url: "/OrganizationManage/Company/GetIdNameList",
            async: false,
            type: "get",
            dataType: "json",
            contentType: "application/json",
            success: function (data) {
                $.each(data, function (index, value) {
                    $("#company").append("<option value='" + value.id + "'>" + value.name + "</option>");
                })
            },
            error: function (data) {
                $.modalAlert("系统出错，请稍候重试", "error");
            }
        })
    } else {
        $.modalAlert("经销商政策须由分公司或事业部录入", "error");
        window.history.go(-1);
        return;
    }
    $("#company").on("change", function () {
        areaFullList.splice(0, areaFullList.length);
        eFullList.splice(0, eFullList.length);  // 清空数组
        eSelList.splice(0, eSelList.length);  // 清空数组
        $duallistGuide.empty();
        $duallistGuide.bootstrapDualListbox("refresh");
      
        GetguideTree();
    })
    GetguideTree();
}

function GetguideTree() {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetEmpTree",
        data: { company_id: $("#company").val(),emp_type:3 },
        async: false,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            guideTree.splice(0, guideTree.length, data);
            if (data.length > 0) {
                $.each(data, function (index, value) {
                    areaFullList.push(value);
                    if (value.key_list.length > 0) {
                        $.each(value.key_list, function (index1, value1) {
                            eFullList.push(value1);
                            eSelList.push(value1);
                            $duallistGuide.append("<option value='" + value1.id + "'>" + value1.display_info + "</option>");
                             
                        });
                    }
                })
            }
            $duallistGuide.bootstrapDualListbox("refresh");
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

// Dual List Box START
$("#modalGuide").on('hide.bs.modal', function (e) {
    eSelList.splice(0, eSelList.length);    // 清空数组
    var selIds = $duallistGuide.val();
    $.each(eFullList, function (i, item) {
        if ($.inArray(item['id'], selIds) > -1)
            eSelList.push(item);
    });
    $tableEmp.bootstrapTable('load', eSelList);
    $("#emp_count").text(eSelList.length);
})

//$(function () {
//    $.ajax({
//        url: "/HumanResource/EmployeeManage/GetInfo?date=" + new Date(),
//        type: "get",
//        dataType: "json",
//        data: { id: id },
//        success: function (data) {
//            $("#form1").formSerializeShow(data.jobInfo);
//        },
//error: function (data) {
//    $.modalAlert(data.responseText, 'error');
//}
//    })
//});

// Table START
function TablesInit() {
    initTable($tableEmp, tableGuideHeader);
    $tableEmp.bootstrapTable('load', eFullList);
    $("#emp_count").text(eFullList.length);
}
var tableGuideHeader = [
    { field: "id", visible: false, },
    { field: "name", title: "名称", align: "center" },
    { field: "area_l2_name", title: "业务片区", align: "center" },
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


function ActionBind() {
    $("input[name='guide_base_type']").on('change', function () {
        var value = $(this).val();
        if (value == 1) {
            $("#salary_commission").css('display', '');
        } else {
            $("#salary_commission").css('display', 'none');
            $("#guide_standard_salary").val('');
            $("#guide_standard_commission").val('');
        }
    });
}
//立即生效 
function Effective(checkbox) {
    if (checkbox.checked == true) {
        $("#effect_date").val('');
        $("#effect_date").attr("disabled", true);
    } else {
        $("#effect_date").removeAttr("disabled");
    }
}


function submitForm() {
    if (!$("#form1").formValid())
        return false;

    var data = $("#form1").formSerialize();
    if (!eSelList || eSelList.length < 1) {
        $.modalAlert("请选择导购员");
        return;
    }else
    data['empList'] = eSelList;
    //生效时间
    if ($("#effect_date").val() == '' && $("#effect_now").prop("checked") == false) {
        $.modalAlert("请选择生效时间");
        return;
    }
    data["effect_now"] = $("#effect_now").prop("checked");
    data["guide_base_type"] = $("input[name=guide_base_type]:checked").val();
    data["guide_annualbonus_type"] = $("input[name=guide_annualbonus_type]:checked").val();//年终奖类型
    $.submitForm({
        url: "/FinancialAccounting/EmployeeSalary/AddGuide?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/EmployeeSalary/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}