﻿var myCompanyInfo = top.clients.loginInfo.companyInfo;
var emp_type = $('#emp_category').val();//员工类型
var checkTargetVal = $('input[name="target_mode"]:checked').val();//奖励模式
var checkSaleVal = $('input[name="target_sale"]:checked').val();//销量阶梯
var checkRebateVal = $('input[name="rebate_mode"]:checked').val();//金额计算模式
var checkProductVal = $('input[name="product_scope"]:checked').val();//活动机型
var mainGuid = "";
var guideTree = [];
var areaFullList = [];
var eFullList = []; // 所有经销商
var eSelList = [];  // 已选经销商
var pFullList = [];   // 所有机型
var pSelList = [];    // 已选机型
var $tableEmp = $('#targetDistributor');
var $tableProduct = $('#targetProduct');
var $tableRebateProduct = $('#rebateProduct');  // 按型号返利
var $pCountWrap = $("#productCountWrap");
var $pTableWrap = $("#targetProductWrap");
var index;//行号
var str1 = "<tr style='display: table;width: 100%;table-layout: fixed;'>"
        + "<td class='col-sm-6'><div class='input-group'><input class='form-control text-center' name='target_min' disabled>"
        + "<span class='input-group-addon no-border'>-</span><div class='formValue'><input type='text' class='form-control text-center required number' name='target_max' onchange='setNum()'></div>"
        + "<td class='col-sm-4'><div class='input-group formValue'><div class='input-group-btn'><button type='button' class='btn btn-primary'  name='rewards_no'  onclick='Change(this)'>奖</button></div><input type='text' class='form-control text-center required number' name='rebate'>"
        + "<span name='rebate_mode_input1' class='input-group-addon no-border'>";
var str2 = "元/台";
var str3 = "</span><span name='rebate_mode_input2' class='input-group-addon no-border no-padding'>";
var str4 = "";
var str5 = "</span></div></td><td class='col-sm-2'><div class='row'><i onclick='addRow(this)' class='fa fa-plus-circle fa-lg  text-blue '></i> <i onclick='deleteRow(this)' class='fa fa-minus-circle fa-lg text-red'></i></div></td>"
        + "</tr>";
//型号的th
var str6 = '<tr><th style="text-align:center;vertical-align:middle;" class="col-sm-3"><span name="product_change">完成率（%）</span></th><th style="text-align:center;vertical-align:middle;" class="col-sm-2">型号</th><th style="text-align:center;vertical-align:middle;" class="col-sm-2">颜色</th><th style="text-align:center;vertical-align:middle;" class="col-sm-4"><span name="rebate_mode_change">返利金额</span></th><th style="text-align:center;vertical-align:middle;" class="col-sm-1"></th></tr>';

var str7 = ' <tr> <td class="col-sm-3"><div class="input-group"><input class="form-control text-center" name="sma_target_min" onchange="setNumSmall(this)"><span class="input-group-addon no-border">-</span>'
            + '<div class="formValue"><input class="form-control text-center required number" name="sma_target_max"  onchange="setNumSmall(this)"></div></div></td>'
            + '<td class="col-sm-1" style="text-align:center;vertical-align:middle"><i onclick="addSmall(this)" class="fa fa-plus  text-blue "></i>&nbsp;<i onclick="delSmall(this)" class="fa fa-minus   text-red "></i>'
            + '</td><td class="col-sm-3"><div class="input-group formValue"> <div class="input-group-btn"><button type="button" class="btn btn-primary" name="rewards_normal" onclick="Change(this)">奖</button>'
            + '</div><input class="form-control text-center required number" name="nor_rebate"><span name="rebate_mode_input1" class="input-group-addon no-border">'
var str8 = '</span>'
            + '<span name="rebate_mode_input2" class="input-group-addon no-border  no-padding">';
var str9 = '</span></div></td></tr> ';

var str11 = '<tr> <td class="col-sm-3" style="text-align:center;vertical-align:middle"><div class="input-group"><input class="form-control text-center" name="nor_target_min" onchange="setNumNor(this)">'
    + '<span class="input-group-addon no-border">-</span><div class="formValue"><input class="form-control text-center required number" name="nor_target_max" onchange="setNumNor(this)">'
    + '</div></div></td><td><table class="table table-bordered text-center" name="small_table" style="margin-bottom:0px"><tr><td class="col-sm-3"><div class="input-group">'
    + '<input class="form-control text-center" name="sma_target_min" onchange="setNumSmall(this)"><span class="input-group-addon no-border">-</span><div class="formValue">'
    + '<input class="form-control text-center required number" name="sma_target_max" onchange="setNumSmall(this)"></div></div> </td><td class="col-sm-1" style="text-align:center;vertical-align:middle">'
    + '<i onclick="addSmall(this)" class="fa fa-plus text-blue "></i>&nbsp;<i onclick="delSmall(this)" class="fa fa-minus   text-red "></i></td><td class="col-sm-4"><div class="input-group formValue"><div class="input-group-btn"><button type="button" class="btn btn-primary" name="rewards_normal" onclick="Change(this)">奖</button></div><input class="form-control text-center required number" name="nor_rebate">'
    + '<span name="rebate_mode_input1" class="input-group-addon no-border">';
var str12 = '</span><span name="rebate_mode_input2" class="input-group-addon no-border  no-padding">';
var str13 = '</span></div></td></tr>'
    + '<tr><td class="col-sm-3"><div class="input-group"><input class="form-control text-center" name="sma_target_min" onchange="setNumSmall(this)"><span class="input-group-addon no-border">-</span><div class="formValue">'
    + '<input class="form-control text-center required number" name="sma_target_max"  value="以上" disabled></div></div></td><td class="col-sm-1" style="text-align:center;vertical-align:middle">'
    + '<i onclick="addSmall(this)" class="fa fa-plus text-blue "></i></td><td class="col-sm-4">'
    + '<div class="input-group formValue"> <div class="input-group-btn"><button type="button" class="btn btn-primary" name="rewards_normal" onclick="Change(this)">奖</button></div><input class="form-control text-center required number" name="nor_rebate"> <span name="rebate_mode_input1" class="input-group-addon no-border">';
var str14 = '</span>'
    + '<span name="rebate_mode_input2" class="input-group-addon no-border  no-padding">';
var str15 = '</span></div></td></tr> </table> </td>'
    + '<td class="col-sm-1" style="text-align:center;vertical-align:middle"><i onclick="addNormal(this)" class="fa fa-plus-circle fa-lg  text-blue "></i>&nbsp;<i onclick="delNormal(this)" class="fa fa-minus-circle fa-lg  text-red "></i>'
    + ' </td></tr>';

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

var $duallistProduct = $('#duallistProduct').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入型号、颜色关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个机型",
    infoTextEmpty: "共0个机型",
    infoTextFiltered: "",
    filterTextClear: "清空"
});
//奖罚 change
function Change(e) {
    var text = $(e).html();
    if (text == '奖') {
        $(e).html('罚');
        $(e).attr('class', 'btn btn-danger');
    } else {
        $(e).html('奖');
        $(e).attr('class', 'btn btn-primary');
    }
}

$('#emp_category').on("change", function () {
    emp_type = $('#emp_category option:selected').val();
    if (emp_type == 3) {
        $('#selectGuide').css("display", "");
        $('#selectSales').css("display", "none");
        $('#selectSalesManage').css("display", "none");
        $tableEmp.bootstrapTable('showColumn', 'area_l2_name');
        $tableEmp.bootstrapTable('hideColumn', 'area_l1_name');
        $("input[name='target_content'][value='2']").attr("disabled", "disabled");
        $("input[name='target_content'][value='1']").prop("checked", "checked");
    } else if (emp_type == 20) {
        $('#selectGuide').css("display", "none");
        $('#selectSales').css("display", "");
        $('#selectSalesManage').css("display", "none");
        $tableEmp.bootstrapTable('showColumn', 'area_l2_name');
        $tableEmp.bootstrapTable('hideColumn', 'area_l1_name');
        $("input[name='target_content'][value='2']").removeAttr("disabled");
    } else if (emp_type == 21) {//业务经理
        $('#selectGuide').css("display", "none");
        $('#selectSales').css("display", "none");
        $('#selectSalesManage').css("display", "");
        $tableEmp.bootstrapTable('showColumn', 'area_l1_name');
        $tableEmp.bootstrapTable('hideColumn', 'area_l2_name');
        $("input[name='target_content'][value='3']").removeAttr("disabled");
    }
    eFullList.splice(0, eFullList.length);  // 清空数组
    eSelList.splice(0, eSelList.length);  // 清空数组
    $duallistGuide.empty();
    $duallistSales.empty();
    $duallistSalesManage.empty();
    GetguideTree();
})

$('#guide_category').on("change", function () {
    //emp_type = $('#emp_category').val();
    //if (emp_type == 3) {
    //    GetguideTree();
    //}
    eFullList.splice(0, eFullList.length);  // 清空数组
    eSelList.splice(0, eSelList.length);  // 清空数组
    $duallistGuide.empty();
    $duallistGuide.bootstrapDualListbox("refresh", true);
    if (emp_type == 3) {
        GetguideTree();
    }
})

$(function () {
    $(window).resize(function () {
        $tableEmp.bootstrapTable('resetView', {
            height: 300
        });
        $tableProduct.bootstrapTable('resetView', {
            height: 300
        });
    });
    $pTableWrap.hide();
    $pCountWrap.hide();
    // $tableRebateProduct.hide();
    TablesInit();
    mainGuid = GetGUID();
    BindCompany();
    Rebate_mode();
    targetSale();

});

function GetGUID() {
    var guid = "";
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
    return guid;
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
        $duallistGuide.empty();
        $duallistGuide.bootstrapDualListbox("refresh");

        $duallistSales.empty();
        $duallistSales.bootstrapDualListbox("refresh");

        $duallistSalesManage.empty();
        $duallistSalesManage.bootstrapDualListbox("refresh");

        pFullList.splice(0, pFullList.length);
        $duallistProduct.empty();
        $duallistProduct.bootstrapDualListbox("refresh");
        $pTableWrap.hide();
        $pCountWrap.hide();

        $("#all_product").prop("checked", "checked");

        GetguideTree();
        getProduct();
    })
    GetguideTree();
    getProduct();
}

function GetguideTree() {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetEmpTree?emp_type=" + emp_type,
        data: { company_id: $("#company").val(), guide_category: $("#guide_category").val() },
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
                            var d = value1;
                            d.main_id = mainGuid;
                            d.emp_id = d.id;
                            d.emp_name = d.name;
                            eFullList.push(d);
                            eSelList.push(d);
                            if (emp_type == 3) {
                                $duallistGuide.append("<option value='" + value1.id + "'>" + value1.display_info + "</option>");
                            } else if (emp_type == 20) {
                                $duallistSales.append("<option value='" + value1.id + "'>" + value1.display_info + "</option>");
                            } else if (emp_type == 21) {
                                $duallistSalesManage.append("<option value='" + value1.id + "'>" + value1.display_info + "</option>");
                            }
                        });
                    }
                })
            }
            $duallistGuide.bootstrapDualListbox("refresh");
            $duallistSales.bootstrapDualListbox("refresh");
            $duallistSalesManage.bootstrapDualListbox("refresh");
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

function getProduct() {
    $.ajax({
        url: "/ProductManage/PriceInfo/GetEffectIdNameList",
        data: { companyId: $("#company").val() },
        type: "get",
        dataType: "json",
        success: function (data) {
            pFullList = data;
            $.each(data, function (index, value) {
                value.main_id = mainGuid;
                $duallistProduct.append("<option value='" + value.id + "'>" + value.model + "(" + value.color + ")" + "</option>");
            })
            $duallistProduct.bootstrapDualListbox("refresh");
            $tableProduct.bootstrapTable('load', pSelList);
            $("#product_count").text(pSelList.length);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}


//活动机型 
function productScopeChange() {
    checkProductVal = $('input[name="product_scope"]:checked').val();
    if (checkProductVal == 1) {
        $("#product_mode").css("display", "none");
        $pTableWrap.hide();
        $pCountWrap.hide();
        $("input[name='target_mode'][value='6']").prop("checked", "checked");
        $("input[name='target_mode'][value='4']").attr("disabled", "disabled"); // 奖励模式-按型号
        $("#rebateMode6").show();
        $("#rebateMode4").hide();
        $("#rebateMode1").hide();
        Rebate_mode();
    } else {
        if (checkRebateVal != 4) {//固定金额 为4
            $("input[name='target_mode'][value='4']").removeAttr("disabled"); // 返利模式-按型号
        }
        if (checkTargetVal == 3 || checkTargetVal == 4 || checkTargetVal == 5) {
            $("input[name='rebate_mode'][value='4']").attr("disabled", "disabled");
        }
        $("#product_mode").css("display", "");
        $pTableWrap.show();
        $pCountWrap.show();
    }
}

//按型号返利模式
function ProductScope() {

    $("#rebateMode4").empty();
    sum = pSelList.length;
    if (sum == 0)
        return;
    trProduct = "";
    var tr = "";
    var tr1 = "";
    $.each(pSelList, function (index, item) {
        if (index == 0) {
            tr += "<tr><td  class='col-sm-3' rowspan='" + sum + "' style='text-align:center;'><div class='input-group formValue'>"
                    + "<input class='form-control text-center' name='pro_target_min' onchange='setNumPro(this)'><span class='input-group-addon no-border'>-</span><input class='form-control text-center required number'value='以上' disabled name='pro_target_max' onchange='setNumPro(this)'>"
                    + "</div></td><td  class='col-sm-2' style='text-align:center;vertical-align:middle;' name='model_pro' >" + item.model + "</td><td class='col-sm-2' style='text-align:center;vertical-align:middle;' name='color_pro'>" + item.color + "</td>"
                    + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;' ><div class='input-group formValue'>"
                    + "<div class='input-group-btn'><button type='button' class='btn btn-primary' name='pro_rewards' onclick='Change(this)'>奖</button></div>"
                    + "<input class='form-control text-center' name='rebate_p'><span name='rebate_mode_input1' class='input-group-addon no-border'>" + str2 + "</span>"
                    + "<span name='rebate_mode_input2' class='input-group-addon no-border no-padding'>" + str4 + "</span></div></td><td style='text-align:center;' class='col-sm-1' rowspan='" + sum + "'> <div class='row'><i onclick='addTr(this)' class='fa fa-plus-circle fa-lg  text-blue'></i>"
                    + "</div></td></tr>";
            trProduct += "<tr><td  class='col-sm-3' rowspan='" + sum + "' style='text-align:center;'><div class='input-group formValue'>"
                    + "<input class='form-control text-center' name='pro_target_min' onchange='setNumPro(this)'><span class='input-group-addon no-border'>-</span><input class='form-control text-center required number' name='pro_target_max' onchange='setNumPro(this)'>"
                    + "</div></td><td  class='col-sm-2' style='text-align:center;vertical-align:middle;'  name='model_pro'>" + item.model + "</td><td class='col-sm-2' style='text-align:center;vertical-align:middle;' name='color_pro'>" + item.color + "</td>"
                    + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;' ><div class='input-group formValue'>"
                    + "<div class='input-group-btn'><button type='button' class='btn btn-primary' name='pro_rewards' onclick='Change(this)'>奖</button></div>"
                    + "<input class='form-control text-center' name='rebate_p'><span name='rebate_mode_input1' class='input-group-addon no-border'>" + str2 + "</span>"
                    + "<span name='rebate_mode_input2' class='input-group-addon no-border no-padding'>" + str4 + "</span></div></td><td style='text-align:center;' class='col-sm-1' rowspan='" + sum + "'> <div class='row'><i onclick='addTr(this)' class='fa fa-plus-circle fa-lg  text-blue'></i>&nbsp;"
                    + "<i onclick='deleteTr(this)' class='fa fa-minus-circle fa-lg text-red'></i></div></td></tr>";
        } else {
            tr1 += "<tr><td class='col-sm-2' style='text-align:center;vertical-align:middle;'  name='model_pro' >" + item.model + "</td><td class='col-sm-2' style='text-align:center;vertical-align:middle;' name='color_pro'>" + item.color + "</td>"
                    + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;'><div class='input-group formValue'>"
                    + "<div class='input-group-btn'><button type='button' class='btn btn-primary' name='pro_rewards' onclick='Change(this)'>奖</button></div>"
                    + "<input class='form-control text-center' name='rebate_p'><span name='rebate_mode_input1' class='input-group-addon no-border'>" + str2 + "</span>"
                    + "<span name='rebate_mode_input2' class='input-group-addon no-border no-padding'>" + str4 + "</span></div></td></tr>";
        }
    })
    trProduct += tr1;
    tr += tr1;
    $("#rebateMode4").append('<thead style="display: table;width: 99%;table-layout: fixed;">' + str6 + '</thead>' + '<tbody style="display: block;height:400px;overflow-y:scroll;overflow-x:hidden">' + tr + '</tbody>');
    targetSale();
}
//销量阶梯
function targetSale() {
    checkSaleVal = $('input[name="target_sale"]:checked').val();//返利模式
    if (checkSaleVal == 1) {
        $("span[name='sale_change']").html("完成率（%）");
        $("span[name='product_change']").html("完成率（%）");
        // 按完成率
        $("input[name='rebate_mode'][value='4']").removeAttr("disabled");
    } else if (checkSaleVal == 2) {
        // 按台数
        $("span[name='sale_change']").html("销量（台）");
        $("span[name='product_change']").html("销量（台）");
        $("input[name='rebate_mode'][value='4']").removeAttr("disabled");
    }
}
//返利模式
$('input[name="target_mode"]').on("change", function () {
    checkTargetVal = $('input[name="target_mode"]:checked').val();//返利模式
    if (checkTargetVal == 3) {
        // 按零售价
        $("span[name='target_mode_change']").html("零售价（元/台）");
        $("input[name='rebate_mode'][value='4']").attr("disabled", "disabled");
        $("#rebateMode1").show();
        $("#rebateMode4").hide();
        $("#rebateMode6").hide();
    } else if (checkTargetVal == 4) {
        //型号
        $("input[name='rebate_mode'][value='4']").attr("disabled", "disabled");
        $("#rebateMode1").hide();
        $("#rebateMode6").hide();
        $("#rebateMode4").show();
    } else if (checkTargetVal == 5) {
        // 按批发价
        $("span[name='target_mode_change']").html("批发价（元/台）");
        $("input[name='rebate_mode'][value='4']").attr("disabled", "disabled");
        $("#rebateMode1").show();
        $("#rebateMode4").hide();
        $("#rebateMode6").hide();
    } else if (checkTargetVal == 6) {
        // 无
        $("#rebateMode1").hide();
        $("#rebateMode4").hide();
        $("#rebateMode6").show();
        $("input[name='rebate_mode'][value='4']").removeAttr("disabled");
    }
})

//返利金额
function Rebate_mode() {
    checkRebateVal = $('input[name="rebate_mode"]:checked').val();//奖励金额
    if (checkRebateVal == 1) {
        $("span[name='rebate_mode_change']").html("返利金额");
        $("input[name='target_mode'][value='3']").removeAttr("disabled");
        $("input[name='target_mode'][value='5']").removeAttr("disabled");
        if (checkProductVal == 2) {
            $("input[name='target_mode'][value='4']").removeAttr("disabled"); // 奖励模式-按型号
        }
        str2 = "元/台";
        str4 = "";
    } else if (checkRebateVal == 2) {
        $("span[name='rebate_mode_change']").html("批发价比例");
        $("input[name='target_mode'][value='3']").removeAttr("disabled");
        $("input[name='target_mode'][value='5']").removeAttr("disabled");
        if (checkProductVal == 2) {
            $("input[name='target_mode'][value='4']").removeAttr("disabled"); // 奖励模式-按型号
        }
        str2 = "%";
        str4 = "*批发价";
    } else if (checkRebateVal == 3) {
        $("span[name='rebate_mode_change']").html("零售价比例");
        $("input[name='target_mode'][value='3']").removeAttr("disabled");
        $("input[name='target_mode'][value='5']").removeAttr("disabled");
        if (checkProductVal == 2) {
            $("input[name='target_mode'][value='4']").removeAttr("disabled"); // 奖励模式-按型号
        }
        str2 = "%";
        str4 = "*零售价";
    } else if (checkRebateVal == 4) {//固定金额只能选无
        $("input[name='target_mode'][value='3']").attr("disabled", "disabled");
        $("input[name='target_mode'][value='4']").attr("disabled", "disabled");
        $("input[name='target_mode'][value='5']").attr("disabled", "disabled");
        $("input[name='target_mode'][value='6']").prop("checked", "checked");
        //活动机型 

        $("span[name='rebate_mode_change']").html("固定金额");
        str2 = "元";
        str4 = "";
    }
    $("span[name='rebate_mode_input1']").html(str2);
    $("span[name='rebate_mode_input2']").html(str4);
}
//新增一行
function addRow(e) {
    $(e).parents('tr').after(str1 + str2 + str3 + str4 + str5);
    index = $(e).closest('tr')[0].rowIndex;
    setNum();
    emptyNum();
}


//删除
function deleteRow(e) {
    $(e).parents('tr').remove();
    setNum();
}


function addSmall(e) {//一般 子表格
    //$(e).closest('tbody').prepend(str7 + str2 + str8 + str4 + str9);
    $($(e).closest('tbody')[0]).children('tr:last').before(str7 + str2 + str8 + str4 + str9);
}
function addNormal(e) {//一般 父表格
    //$(e).parents('tbody').prepend(str11 + str2 + str12 + str4 + str13 + str2 + str14 + str4 + str15);
    $($($('#rebateMode1').children('tbody')[0]).children('tr:last')[0]).before(str11 + str2 + str12 + str4 + str13 + str2 + str14 + str4 + str15);
}

function delSmall(e) {//删除一般
    $(e).closest('tr').remove();
}
function delNormal(e) {//删除一般
    $(e).parents('tr').remove();
}
//型号新增一个tr
function addTr(e) {
    $($(e).parents('tbody').find('tr:eq(' + (-sum) + ')')).before(trProduct);//固定添加到倒数第二行
    //$(e).parents('tbody').prepend(trProduct);
    Rebate_mode();
}
//删除Tr
function deleteTr(e) {
    var del_index = $(e).closest('tr')[0].rowIndex;
    for (var i = del_index + sum - 1; i >= del_index ; i--) {
        $(e).parents('tbody').find('tr:eq(' + (i - 1) + ')').remove();
    }
}
function setNumNor(e) {
    if ($(e)[0].name == "nor_target_min") {//点击的是min
        var i = $("input[name='nor_target_min']").index(e);//获取该行行号
        if (i != 0) {
            if (Number($($("input[name='nor_target_min']")[i]).val()) <= Number($($("input[name='nor_target_max']")[i - 1]).val())) {//输入的值大于max时
                $.modalAlert("该值应大于" + $($("input[name='nor_target_max']")[i - 1]).val() + "！");
                $($("input[name='nor_target_min']")[i]).val('');
                return;
            }
        }
        if ($($("input[name='nor_target_max']")[i]).val() != '') {//当前行max不为空时
            if (Number($($("input[name='nor_target_min']")[i]).val()) >= Number($($("input[name='nor_target_max']")[i]).val())) {//输入的值大于max时
                $.modalAlert("该值应小于" + $($("input[name='nor_target_max']")[i]).val() + "！");
                $($("input[name='nor_target_min']")[i]).val('');
                return;
            }
        }
    } else if ($(e)[0].name == "nor_target_max") {//点击的是max
        var i = $("input[name='nor_target_max']").index(e);

        if (Number($($("input[name='nor_target_max']")[i]).val()) > Number($($("input[name='nor_target_min']")[i]).val())) {
            $($("input[name='nor_target_min']")[i + 1]).val(Number($($("input[name='nor_target_max']")[i]).val()) + Number(1));
        } else {
            $($("input[name='nor_target_max']")[i]).val('');
            $.modalAlert("该值应大于" + $($("input[name='nor_target_min']")[i]).val() + "！");
        }

        if ($($("input[name='nor_target_max']")[i + 1]).val() != '') {
            if (Number($($("input[name='nor_target_max']")[i]).val()) > Number($($("input[name='nor_target_max']")[i + 1]).val() - 2)) {
                $($("input[name='nor_target_max']")[i + 1]).val('');
                $.modalAlert("该值应大于" + (Number($($("input[name='nor_target_max']")[i]).val()) + Number(1)) + "！");
            }
        }
    }
    //var len = $(e).parents('tbody').children().length;
    //for (var i = 0; i < len; i++) {
    //    var min = $($("input[name='nor_target_min']")[i]).val();
    //    var max = $($("input[name='nor_target_max']")[i]).val();
    //    if (i != len - 1 && max != '') {
    //        if (Number(max) <= Number(min)) {
    //            $.modalAlert("该值应大于" + min + "！");
    //            $($("input[name='nor_target_max']")[i]).val("");
    //            return;
    //        }
    //    }
    //    if (max != '') {
    //        $($("input[name='nor_target_min']")[i + 1]).val(Number($($("input[name='nor_target_max']")[i]).val()) + Number(1));
    //        if (Number(min) >= Number(max)) {
    //            $($("input[name='nor_target_max']")[i]).val('');
    //        }
    //    }
    //}
}

function setNumPro(e) {
    if ($(e)[0].name == "pro_target_min") {//点击的是min
        var i = $("input[name='pro_target_min']").index(e);//获取该行行号
        if (i != 0) {
            if (Number($($("input[name='pro_target_min']")[i]).val()) <= Number($($("input[name='pro_target_max']")[i - 1]).val())) {//输入的值大于max时
                $.modalAlert("该值应大于" + $($("input[name='pro_target_max']")[i - 1]).val() + "！");
                $($("input[name='pro_target_min']")[i]).val('');
                return;
            }
        }
        if ($($("input[name='pro_target_max']")[i]).val() != '') {//当前行max不为空时
            if (Number($($("input[name='pro_target_min']")[i]).val()) >= Number($($("input[name='pro_target_max']")[i]).val())) {//输入的值大于max时
                $.modalAlert("该值应小于" + $($("input[name='pro_target_max']")[i]).val() + "！");
                $($("input[name='pro_target_min']")[i]).val('');
                return;
            }
        }
    } else if ($(e)[0].name == "pro_target_max") {//点击的是max
        var i = $("input[name='pro_target_max']").index(e);

        if (Number($($("input[name='pro_target_max']")[i]).val()) > Number($($("input[name='pro_target_min']")[i]).val())) {
            $($("input[name='pro_target_min']")[i + 1]).val(Number($($("input[name='pro_target_max']")[i]).val()) + Number(1));
        } else {
            $($("input[name='pro_target_max']")[i]).val('');
            $.modalAlert("该值应大于" + $($("input[name='pro_target_min']")[i]).val() + "！");
        }

        if ($($("input[name='pro_target_max']")[i + 1]).val() != '') {
            if (Number($($("input[name='pro_target_max']")[i]).val()) > Number($($("input[name='pro_target_max']")[i + 1]).val() - 2)) {
                $($("input[name='pro_target_max']")[i + 1]).val('');
                $.modalAlert("该值应大于" + (Number($($("input[name='pro_target_max']")[i]).val()) + Number(1)) + "！");
            }
        }
    }
    //var len = $(e).parents('tbody').children().length;
    //for (var i = 0; i < len; i++) {
    //    var min = $($("input[name='pro_target_min']")[i]).val();
    //    var max = $($("input[name='pro_target_max']")[i]).val();
    //    if (i != len - 1 && max != '') {
    //        if (Number(max) <= Number(min)) {
    //            $.modalAlert("该值应大于" + min + "！");
    //            $($("input[name='pro_target_max']")[i]).val("");
    //            return;
    //        }
    //    }
    //    if (max != '') {
    //        $($("input[name='pro_target_min']")[i + 1]).val(Number($($("input[name='pro_target_max']")[i]).val()) + Number(1));
    //        if (Number(min) >= Number(max)) {
    //            $($("input[name='pro_target_max']")[i]).val('');
    //        }
    //    }
    //}
}


function setNumSmall(e) {
    if ($(e)[0].name == "sma_target_min") {//点击的是min
        var i = $("input[name='sma_target_min']").index(e);//获取该行行号
        if (i != 0) {
            if (Number($($("input[name='sma_target_min']")[i]).val()) <= Number($($("input[name='sma_target_max']")[i - 1]).val())) {//输入的值大于max时
                $.modalAlert("该值应大于" + $($("input[name='sma_target_max']")[i - 1]).val() + "！");
                $($("input[name='sma_target_min']")[i]).val('');
                return;
            }
        }
        if ($($("input[name='sma_target_max']")[i]).val() != '') {//当前行max不为空时
            if (Number($($("input[name='sma_target_min']")[i]).val()) >= Number($($("input[name='sma_target_max']")[i]).val())) {//输入的值大于max时
                $.modalAlert("该值应小于" + $($("input[name='sma_target_max']")[i]).val() + "！");
                $($("input[name='sma_target_min']")[i]).val('');
                return;
            }
        }
    } else if ($(e)[0].name == "sma_target_max") {//点击的是max
        var i = $("input[name='sma_target_max']").index(e);

        if (Number($($("input[name='sma_target_max']")[i]).val()) > Number($($("input[name='sma_target_min']")[i]).val())) {
            $($("input[name='sma_target_min']")[i + 1]).val(Number($($("input[name='sma_target_max']")[i]).val()) + Number(1));
        } else {
            $($("input[name='sma_target_max']")[i]).val('');
            $.modalAlert("该值应大于" + $($("input[name='sma_target_min']")[i]).val() + "！");
        }

        if ($($("input[name='sma_target_max']")[i + 1]).val() != '') {
            if (Number($($("input[name='sma_target_max']")[i]).val()) > Number($($("input[name='sma_target_max']")[i + 1]).val() - 2)) {
                $($("input[name='sma_target_max']")[i + 1]).val('');
                $.modalAlert("该值应大于" + (Number($($("input[name='sma_target_max']")[i]).val()) + Number(1)) + "！");
            }
        }
    }
}

//input输入值 1.小数不行
function setNum() {
    for (var i = 2; i < $("#rebateMode6 tr").length; i++) {
        if (i != $("#rebateMode6 tr").length - 1) {
            if (parseInt($('#rebateMode6 tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(0)').val()) >= parseInt($('#rebateMode6 tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(1)').val())) {
                $.modalAlert("该值应大于" + $('#rebateMode6 tr:eq(' + i + ') td:eq(0) input:eq(0)').val() + "！");
                $('#rebateMode6 tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
        var max = $('#rebateMode6 tr:eq(' + (i - 1) + ') td:eq(0) input:eq(1)').val();
        if (max != '') {
            $('#rebateMode6 tr:eq(' + i + ') td:eq(0) input:eq(0)').val(parseInt(max) + 1);
            if (parseInt($('#rebateMode6 tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(0)').val()) >= parseInt($('#rebateMode6 tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(1)').val())) {
                $('#rebateMode6 tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
    }
}

function emptyNum() {
    for (var j = index + 2; j < $("#rebateMode6 tr").length; j++) {
        $('#rebateMode6 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
        $('#rebateMode6 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("");
        if (j == $("#rebateMode6 tr").length - 1) {
            $('#rebateMode6 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
            $('#rebateMode6 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("以上");
        }
    }
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
$("#modalSalesManage").on('hide.bs.modal', function (e) {
    eSelList.splice(0, eSelList.length);    // 清空数组
    var selIds = $duallistSalesManage.val();
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
    ProductScope();
    productScopeChange();
    $("#productCountWrap").show();
})
// Dual List Box END

// Table START
function TablesInit() {
    initTable($tableEmp, tableGuideHeader);
    $tableEmp.bootstrapTable('load', eFullList);
    $("#emp_count").text(eFullList.length);

    initTable($tableProduct, tableProductHeader);
}
var tableGuideHeader = [
    { field: "id", visible: false, },
    { field: "name", title: "名称", align: "center" },
    { field: "area_l2_name", title: "业务片区", align: "center" },
    { field: "area_l1_name", title: "经理片区", align: "center", visible: false, },
    { field: "company_name", title: "分公司", align: "center", },
]
var tableProductHeader = [
    { field: "id", visible: false, },
    { field: "model", title: "型号", align: "center" },
    { field: "color", title: "颜色", align: "center", },
    {
        field: "price_wholesale", title: "批发价（元/台）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); })
    },
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
    data['id'] = mainGuid;
    data['name'] = $("#name").val();
    data['company_id'] = $("#company").val();
    data['emp_category'] = $("#emp_category").val();
    data['company_name'] = $("#company option:selected").text();
    data['start_date'] = $("#start_date").val();
    data['end_date'] = $("#end_date").val();
    data['emp_count'] = $("#emp_count").html();//经销商家数
    data['product_scope'] = checkProductVal;
    data['target_content'] = $('input[name="target_content"]:checked').val();
    data['target_mode'] = checkTargetVal;
    data['rebate_mode'] = checkRebateVal;
    data['target_sale'] = checkSaleVal;
    data['activity_target'] = $("#activity_target").val();
    data['money_included'] = $("#money_included").prop("checked") == true ? 0 : 1;
    data['note'] = $("#note").val();
    var rewardList = [];
    var proRebateList = [];
    if (checkTargetVal == 3 || checkTargetVal == 5)    // 一般
    {
        //var minInputList = $("#rebateMode1 tbody").find('tr:eq(0) td:eq(0) input[name=nor_target_min]').val();
        //var maxInputList = $("#rebateMode1 tbody").find('tr:eq(0) td:eq(0) input[name=nor_target_max]').val();


        var minInputList = $("input[name='nor_target_min']");
        var maxInputList = $("input[name='nor_target_max']");

        var minSmallList = $("input[name='sma_target_min']");
        var maxSmallList = $("input[name='sma_target_max']");
        var rebInputList = $("input[name='nor_rebate']");
        var proNormalList = $("button[name='rewards_normal']");

        var smallTable = $("table[name='small_table']");

        var len = minInputList.length;
        var index = 0;
        for (var i = 0; i < len; i++) {
            var rebate = {};
            rebate.id = GetGUID();
            rebate.main_id = mainGuid;
            rebate.target_min = $(minInputList[i]).val();
            rebate.target_max = $(maxInputList[i]).val() == '以上' ? "-1" : $(maxInputList[i]).val();
            rewardList.push(rebate);

            for (var j = 0; j < $(smallTable[i]).find('tr').length; j++) {
                var sma_rebate = {};
                sma_rebate.reward_id = rebate.id;
                sma_rebate.target_min = $(minSmallList[index]).val();
                sma_rebate.target_max = $(maxSmallList[index]).val() == '以上' ? "-1" : $(maxSmallList[index]).val();
                if ($(proNormalList[index]).html() == '奖') {
                    sma_rebate.reward = $(rebInputList[index]).val();
                } else {
                    sma_rebate.reward = 0 - $(rebInputList[index]).val();
                }
                if ($(minInputList[i]).val() == '' || $(maxInputList[i]).val() == '' || $(minSmallList[index]).val() == '' || $(maxSmallList[index]).val() == '' || $(rebInputList[index]).val() == '') {
                    $.modalAlert("返利信息填写不完整！");
                    return;
                }
                proRebateList.push(sma_rebate);
                index++;
               
            }
           
        }
        data['rewardList'] = rewardList;
        data['proRewardList'] = proRebateList;
    }
    else if (checkTargetVal == 4)    // 型号
    {
        var minProductList = $("input[name='pro_target_min']");
        var maxProductList = $("input[name='pro_target_max']");
        var proRewardsList = $("button[name='pro_rewards']");
        var rebProductList = $("input[name='rebate_p']");

        var model_pro = $("td[name='model_pro']");
        var color_pro = $("td[name='color_pro']");
        var len = minProductList.length;
        var index = 0;
        for (var i = 0; i < len; i++) {
            var product = {};
            product.id = GetGUID();
            product.main_id = mainGuid;
            product.target_min = $(minProductList[i]).val();
            product.target_max = $(maxProductList[i]).val() == '以上' ? "-1" : $(maxProductList[i]).val();
            rewardList.push(product);

            for (var j = 0; j < sum; j++) {
                var sma_rebate = {};
                sma_rebate.reward_id = product.id;
                sma_rebate.model = $(model_pro[index]).html();
                sma_rebate.color = $(color_pro[index]).html();
                if ($(proRewardsList[index]).html() == '奖') {
                    sma_rebate.reward = $(rebProductList[index]).val();
                } else {
                    sma_rebate.reward = 0 - $(rebProductList[index]).val();
                }
                if ($(minProductList[i]).val() == '' || $(maxProductList[i]).val() == '' || $(rebProductList[index]).val() == '') {
                    $.modalAlert("返利信息填写不完整！");
                    return;
                }
                proRebateList.push(sma_rebate);
                index++;
            }
        }

        data['rewardList'] = rewardList;
        data['proRewardList'] = proRebateList;
    }
    else if (checkTargetVal == 6) { //无
        var minInputList = $("input[name='target_min']");
        var maxInputList = $("input[name='target_max']");
        var rewardsList = $("button[name='rewards_no']");
        var rebInputList = $("input[name='rebate']");
        var len = minInputList.length;

        for (var i = 0; i < len; i++) {
            var rebateNo = {};
            rebateNo.id = GetGUID();
            rebateNo.main_id = mainGuid;
            rebateNo.target_min = $(minInputList[i]).val();
            if (i == len - 1)
                rebateNo.target_max = -1; //以上
            else
                rebateNo.target_max = $(maxInputList[i]).val();

            var rebateValue = {};
            rebateValue.reward_id = rebateNo.id;
            if ($(rewardsList[i]).html() == '奖') {
                rebateValue.reward = $(rebInputList[i]).val();
            } else {
                rebateValue.reward = 0 - $(rebInputList[i]).val();
            }
            if ($(maxInputList[i]).val() == '' || $(rebInputList[i]).val() == '') {
                $.modalAlert("返利信息填写不完整！");
                return;
            }
            rewardList.push(rebateNo);
            proRebateList.push(rebateValue);
        }
        data['rewardList'] = rewardList;
        data['proRewardList'] = proRebateList;
    }
    if (checkProductVal == 2) {
        if (!pSelList || pSelList.length < 1) {
            $.modalAlert("请选择型号");
            return;
        }
    }
    if (!eSelList || eSelList.length < 1) {
        if (emp_type == 3) {
            $.modalAlert("请选择导购员");
        } else if (emp_type == 20) {
            $.modalAlert("请选择业务员");
        } else if (emp_type == 21) {
            $.modalAlert("请选择业务经理");
        }
        return;
    }
    data['empList'] = eSelList;
    data['productList'] = pSelList;
    $.submitForm({
        url: "/ActivityManage/Attaining/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/ActivityManage/Attaining/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

