var myCompanyInfo = top.clients.loginInfo.companyInfo;
var hasIntern = false;
var eFullList = []; // 所有业务员
var eSelList = [];  // 已选业务员
var $tableEmp = $('#targetEmp');
var emp_type = $('#emp_category').val();//员工类型
var guid = "";
var salesTree = [];
var areaFullList = [];
var checkTargetVal = $('input[name="target_mode"]:checked').val();//奖励模式
var checkNormalVal = $('input[name="normal_rebate_mode"]:checked').val();//正常机提成
var checkBuyoutVal = $('input[name="buyout_rebate_mode"]:checked').val();//买断机提成
var index;//行号
var str1 = '<tr><td class="col-sm-4">'
         + '<div class="input-group"><input class="form-control text-center" name="target_min" disabled>'
         + '<span class="input-group-addon no-border"><= X < </span><input class="form-control text-center required number" name="target_max" onchange="setNum()" ></div></td>'
         + '<td class="col-sm-3"><div class="input-group formValue"><input type="text" class="form-control text-center required number" name="sale_rebate">'
         + '<span name="rebate_mode_input1" class="input-group-addon no-border">';
var str2 = "";
var str3 = '</span> <span name="rebate_mode_input2" class="input-group-addon no-border no-padding">';
var str4 = "";
var str5 = '</span></div></td><td class="col-sm-3"><div class="input-group formValue">'
+ '<input type="text" class="form-control text-center required number" name="buyout_rebate"> <span name="rebate_mode_input3" class="input-group-addon no-border">'

var str6 = "";
var str7 = '</span><span name="rebate_mode_input4" class="input-group-addon no-border no-padding">';
var str8 = "";
var str9 = '</span> </div></td><td class="col-sm-2"><i onclick="addRow(this)" class="fa fa-plus-circle fa-lg margin text-blue "></i><i onclick="deleteRow(this)" class="fa fa-minus-circle fa-lg margin text-red "></i></td></tr>';

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
//业务经理
var $duallistSalesManage = $('#duallistSalesManage').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入名称、区域关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个业务经理",
    infoTextEmpty: "共0个业务经理",
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
    TablesInit();
});
$('#emp_category').on("change", function () {
    emp_type = $('#emp_category option:selected').val();
    if (emp_type == 20) {
        $('#selectGuide').css("display", "none");
        $('#selectSales').css("display", "");
        $('#selectSalesManage').css("display", "none");
        $tableEmp.bootstrapTable('showColumn', 'area_l2_name');
        $tableEmp.bootstrapTable('hideColumn', 'area_l1_name');

    } else if (emp_type == 21) {//业务经理
        $('#selectGuide').css("display", "none");
        $('#selectSales').css("display", "none");
        $('#selectSalesManage').css("display", "");
        $tableEmp.bootstrapTable('showColumn', 'area_l1_name');
        $tableEmp.bootstrapTable('hideColumn', 'area_l2_name');

    }
    eFullList.splice(0, eFullList.length);  // 清空数组
    eSelList.splice(0, eSelList.length);  // 清空数组
    $duallistSales.empty();
    $duallistSalesManage.empty();
    GetSalesTree();
})

$("input[name='intern_salary_type']").on('change', function () {
    $intern_salary_type = $(this).val();
    if ($intern_salary_type == 1) {
        $("#salary_fix").css('display', 'none');
        $("#salary_ratio").css('display', '');
        $("#intern_fix_salary").val('');
    } else if ($intern_salary_type == 2) {
        $("#salary_ratio").css('display', 'none');
        $("#salary_fix").css('display', '');
        $("#intern_ratio_salary").val('');
    }
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
        $duallistSales.empty();
        $duallistSales.bootstrapDualListbox("refresh");

        $duallistSalesManage.empty();
        $duallistSalesManage.bootstrapDualListbox("refresh");

        GetSalesTree();
    })
    GetSalesTree();
}

function GetSalesTree() {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetEmpTree",
        data: { company_id: $("#company").val(), emp_type: emp_type },
        async: false,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            salesTree.splice(0, salesTree.length, data);
            if (data.length > 0) {
                $.each(data, function (index, value) {
                    areaFullList.push(value);
                    if (value.key_list.length > 0) {
                        hasIntern = false;
                        $.each(value.key_list, function (index1, value1) {
                            eFullList.push(value1);
                            eSelList.push(value1);
                            if (emp_type == 20) {
                                $duallistSales.append("<option value='" + value1.id + "'>" + value1.display_info + "</option>");
                            } else if (emp_type == 21) {
                                $duallistSalesManage.append("<option value='" + value1.id + "'>" + value1.display_info + "</option>");
                            }
                            if (value1.emp_category == "实习生") {
                                hasIntern = true;
                            }
                        });
                        if (hasIntern) {
                            $("#EmpCategory").css("display", "");
                        } else {
                            $("#EmpCategory").css("display", "none");
                        }
                    }
                })
            }
            $duallistSales.bootstrapDualListbox("refresh");
            $duallistSalesManage.bootstrapDualListbox("refresh");
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
$("#modalSales").on('hide.bs.modal', function (e) {
    eSelList.splice(0, eSelList.length);    // 清空数组
    var selIds = $duallistSales.val();
    hasIntern = false;
    $.each(eFullList, function (i, item) {
        if ($.inArray(item['id'], selIds) > -1) {
            eSelList.push(item);
            if (item.emp_category == "实习生")
                hasIntern = true;
        }
    });
    if (hasIntern) {
        $("#EmpCategory").css("display", "");
    } else {
        $("#EmpCategory").css("display", "none");
    }
    $tableEmp.bootstrapTable('load', eSelList);
    $("#emp_count").text(eSelList.length);
})
$("#modalSalesManage").on('hide.bs.modal', function (e) {
    eSelList.splice(0, eSelList.length);    // 清空数组
    var selIds = $duallistSalesManage.val();
    hasIntern = false;
    $.each(eFullList, function (i, item) {
        if ($.inArray(item['id'], selIds) > -1) {
            eSelList.push(item);
            if (item.emp_category == "实习生")
                hasIntern = true;
        }
    });
    if (hasIntern) {
        $("#EmpCategory").css("display", "");
    } else {
        $("#EmpCategory").css("display", "none");
    }
    $tableEmp.bootstrapTable('load', eSelList);
    $("#emp_count").text(eSelList.length);
})


//返利模式
$('input[name="target_mode"]').on("change", function () {
    checkTargetVal = $('input[name="target_mode"]:checked').val();//返利模式

    if (checkTargetVal == 1) {
        $("span[name='target_mode_change']").html("完成率（%）");
        // 按完成率
        $("input[name='buyout_rebate_mode'][value='4']").removeAttr("disabled");
        $("input[name='normal_rebate_mode'][value='4']").removeAttr("disabled");
        $("#forTargetMode1").show();
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 2) {
        // 按台数
        $("span[name='target_mode_change']").html("销量（台）");
        $("input[name='normal_rebate_mode'][value='4']").removeAttr("disabled");
        $("input[name='buyout_rebate_mode'][value='4']").removeAttr("disabled");
        $("#forTargetMode1").hide();
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 3) {
        // 按零售价
        $("span[name='target_mode_change']").html("零售价（元/台）");
        $("input[name='normal_rebate_mode'][value='4']").attr("disabled", "disabled");
        $("input[name='buyout_rebate_mode'][value='4']").attr("disabled", "disabled");
        $("#forTargetMode1").hide();
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 5) {
        // 批发价
        $("span[name='target_mode_change']").html("批发价（元/台）");
        $("input[name='buyout_rebate_mode'][value='4']").attr("disabled", "disabled");
        $("input[name='normal_rebate_mode'][value='4']").attr("disabled", "disabled");
        $("#forTargetMode1").hide();
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    }
})

//奖励金额方式
$('input[name="normal_rebate_mode"]').on("change", function () {
    checkNormalVal = $('input[name="normal_rebate_mode"]:checked').val();//正常机
    if (checkNormalVal == 1) {
        $("span[name='rebate_mode_change']").html("(元/台)");
        str4 = "";
        str2 = "";
    } else if (checkNormalVal == 2) {
        $("span[name='rebate_mode_change']").html("（元/台）");
        str4 = "* 批发价";
        str2 = "%";
    } else if (checkNormalVal == 3) {
        $("span[name='rebate_mode_change']").html("（元/台）");
        str4 = "* 零售价";
        str2 = "%";
    } else if (checkNormalVal == 4) {
        $("input[name='target_mode'][value='3']").attr("disabled", "disabled");
        $("input[name='target_mode'][value='5']").attr("disabled", "disabled");
        $("span[name='rebate_mode_change']").html("（元/月）");
        str4 = "";
        str2 = "";
    }
    if (checkNormalVal != 4) {
        if (checkBuyoutVal != 4) {
            $("input[name='target_mode'][value='3']").removeAttr("disabled");
            $("input[name='target_mode'][value='5']").removeAttr("disabled");
        }
    }
    $("span[name='rebate_mode_input1']").html(str2);
    $("span[name='rebate_mode_input2']").html(str4);
})

//奖励金额方式
$('input[name="buyout_rebate_mode"]').on("change", function () {
    checkBuyoutVal = $('input[name="buyout_rebate_mode"]:checked').val();//买断机
    if (checkBuyoutVal == 1) {
        $("span[name='rebate_mode_change2']").html("(元/台)");
        str8 = "";
        str6 = "";
    } else if (checkBuyoutVal == 2) {
        $("span[name='rebate_mode_change2']").html("（元/台）");
        str8 = "* 批发价";
        str6 = "%";
    } else if (checkBuyoutVal == 3) {
        $("span[name='rebate_mode_change2']").html("（元/台）");
        str8 = "* 买断价";
        str6 = "%";
    } else if (checkBuyoutVal == 4) {
        $("input[name='target_mode'][value='5']").attr("disabled", "disabled");
        $("input[name='target_mode'][value='3']").attr("disabled", "disabled");
        $("span[name='rebate_mode_change2']").html("（元/月）");
        str8 = "";
        str6 = "";
    }
    if (checkBuyoutVal != 4) {
        if (checkNormalVal != 4) {
            $("input[name='target_mode'][value='3']").removeAttr("disabled");
            $("input[name='target_mode'][value='5']").removeAttr("disabled");
        }
    }
    $("span[name='rebate_mode_input3']").html(str6);
    $("span[name='rebate_mode_input4']").html(str8);
})

//新增一行
function addRow(e) {
    $(e).parents('tr').after(str1 + str2 + str3 + str4 + str5 + str6 + str7 + str8 + str9);
    index = $(e).closest('tr')[0].rowIndex;
    setNum();
    emptyNum();
}


//删除
function deleteRow(e) {
    $(e).parents('tr').remove();
    setNum();
}
//input输入值 1.小数不行
function setNum() {
    for (var i = 2; i < $("#rebateMode1 tr").length; i++) {
        if (i != $("#rebateMode1 tr").length - 1) {
            if (parseInt($('#rebateMode1 tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(0)').val()) >= parseInt($('#rebateMode1 tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(1)').val())) {
                $.modalAlert("该值应大于" + $('#rebateMode1 tr:eq(' + i + ') td:eq(0) input:eq(0)').val() + "！");
                $('#rebateMode1 tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
        var max = $('#rebateMode1 tr:eq(' + (i - 1) + ') td:eq(0) input:eq(1)').val();
        if (max != '') {
            $('#rebateMode1 tr:eq(' + i + ') td:eq(0) input:eq(0)').val(parseInt(max));
            if (parseInt($('#rebateMode1 tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(0)').val()) >= parseInt($('#rebateMode1 tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(1)').val())) {
                $('#rebateMode1 tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
    }
}

function emptyNum() {
    for (var j = index + 2; j < $("#rebateMode1 tr").length; j++) {
        $('#rebateMode1 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
        $('#rebateMode1 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("");
        if (j == $("#rebateMode1 tr").length - 1) {
            $('#rebateMode1 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
            $('#rebateMode1 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("以上");
        }
    }
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
    { field: "grade", title: "职等", align: "center" },
    {
        field: "emp_category", title: "类别", align: "center",
    },
    { field: "area_l2_name", title: "业务片区", align: "center" },
    { field: "area_l1_name", title: "经理片区", align: "center", visible: false },
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
    //提交业务员
    if (!eSelList || eSelList.length < 1) {
        if (emp_type == 20) {
            $.modalAlert("请选择业务员");
        } else if (emp_type == 21) {
            $.modalAlert("请选择业务经理");
        }
        return;
    }
    data['empList'] = eSelList;

    var subList = [];
    // 考核类型
    var minInputList = $("input[name='target_min']");
    var maxInputList = $("input[name='target_max']");
    var normalRateList = $("input[name='sale_rebate']");
    var buyoutRateList = $("input[name='buyout_rebate']");
    var len1 = minInputList.length;
    for (var i = 0; i < len1; i++) {
        var Rate = {};
        Rate.category = 1;
        Rate.target_min = $(minInputList[i]).val();
        if (i == len1 - 1)
            Rate.target_max = -1; //以上
        else
            Rate.target_max = $(maxInputList[i]).val();
        Rate.sale_rebate = $(normalRateList[i]).val();
        Rate.buyout_rebate = $(buyoutRateList[i]).val();
        if ($(buyoutRateList[i]).val() == '' || $(normalRateList[i]).val() == '' || $(maxInputList[i]).val() == '') {
            $.modalAlert("月度考核信息填写不完整！");
            return;
        }
        subList.push(Rate);
    }
    data['subList'] = subList;

    //生效时间
    if ($("#effect_date").val() == '' && $("#effect_now").prop("checked") == false) {
        $.modalAlert("请选择生效时间");
        return;
    }

    //目标销量
    checkTargetVal = $('input[name="target_mode"]:checked').val();//返利模式
    if (checkTargetVal == 1 && $("#activity_target").val() == 0) {
        $.modalAlert("目标销量不能为0");
        return;
    }

    data["effect_now"] = $("#effect_now").prop("checked");
    $.submitForm({
        url: "/FinancialAccounting/EmployeeSalary/AddSales?date=" + new Date(),
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