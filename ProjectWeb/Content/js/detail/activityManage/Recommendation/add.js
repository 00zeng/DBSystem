var myCompanyInfo = top.clients.loginInfo.companyInfo;
var emp_type = $('#emp_category').val();//员工类型
var checkTargetVal = $('input[name="target_mode"]:checked').val();//返利模式
var checkRebateVal = $('input[name="rebate_mode"]:checked').val();//金额计算模式
var checkProductVal = $('input[name="product_scope"]:checked').val();//活动机型
var guid = "";
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
var str1 = '<tr style="display: table;width: 100%;table-layout: fixed;">'
        + '<td class="col-sm-5"><div class="input-group"><input class="form-control text-center " name="target_min" disabled>'
        + '<span class="input-group-addon no-border">-</span><div class="formValue"><input type="text" class="form-control text-center required number" name="target_max" onchange="setNum()"></div>'
        + '<td class="col-sm-5"><div class="input-group formValue"><div class="input-group-btn"><button type="button" class="btn btn-primary"  name="rewards"  onclick="Change(this)">奖</button></div><input type="text" class="form-control text-center required number" name="rebate">'
        + '<span name="rebate_mode_input1" class="input-group-addon no-border">';
var str2 = '元/台';
var str3 = '</span><span name="rebate_mode_input2" class="input-group-addon no-border no-padding">';
var str4 = '';
var str5 = '</span></div></td><td class="col-sm-2"><div class="row"><i onclick="addRow(this)" class="fa fa-plus-circle fa-lg  text-blue "></i> <i onclick="deleteRow(this)" class="fa fa-minus-circle fa-lg text-red"></i></div></td>'
        + '</tr>';
//型号的th
var str6 = '<tr><th style="text-align:center;vertical-align:middle;" class="col-sm-3">型号</th><th style="text-align:center;vertical-align:middle;" class="col-sm-2">颜色</th><th style="text-align:center;vertical-align:middle;" class="col-sm-2">批发价(元/台)</th><th class="col-sm-5" style="text-align:center;vertical-align:middle;"><span name="rebate_mode_change">奖励金额</span></th></tr>';
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
        $("input[name='target_content'][value='2']").removeAttr("disabled");

    }
    eFullList.splice(0, eFullList.length);  // 清空数组
    eSelList.splice(0, eSelList.length);  // 清空数组
    $duallistGuide.empty();
    $duallistSales.empty();
    $duallistSalesManage.empty();
    GetguideTree();



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
    GetGUID();
    BindCompany();
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
                            d.main_id = guid;
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
                value.main_id = guid;
                $duallistProduct.append("<option value='" + value.id + "'>" + value.model + "(" + value.color + ")" + "</option>");
            })
            $duallistProduct.bootstrapDualListbox("refresh");
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}


//活动机型 
$('input[name="product_scope"]').on("change", function () {
    checkProductVal = $('input[name="product_scope"]:checked').val();
    if (checkProductVal == 1) {
        $pTableWrap.hide();
        $pCountWrap.hide();
        $("input[name='target_mode'][value='1']").prop("checked", "checked");
        $("input[name='target_mode'][value='4']").attr("disabled", "disabled"); // 返利模式-按型号
        $("#rebateMode1").show();
        $("#rebateMode4").hide();
    } else {
        $pTableWrap.show();
        $pCountWrap.show();
        $("input[name='target_mode'][value='4']").removeAttr("disabled"); // 返利模式-按型号
    }
})

function ProductScope() {
    $("#rebateMode4").empty()
    var tr = "";
    $.each(pSelList, function (index, item) {
        tr += "<tr style='display: table;width: 100%;table-layout: fixed;'><td class='col-sm-3' style='text-align:center;vertical-align:middle;'>" + item.model + "</td><td class='col-sm-2' style='text-align:center;vertical-align:middle;'>" + item.color + "</td><td style='text-align:center;vertical-align:middle;' class='col-sm-2'>" + item.price_wholesale + "</td>"
        + "<td style='text-align:center;vertical-align:middle;' class='col-sm-5'><div class='input-group formValue'> <div class='input-group-btn'><button type='button' class='btn btn-primary' name='rewards_p' onclick='Change(this)'>奖</button></div>"
        + "<input class='form-control text-center required number' name='reward_p'><span name='rebate_mode_input1' class='input-group-addon no-border no-padding'>元/台</span>"
        + "<span name='rebate_mode_input2' class='input-group-addon no-border no-padding'></span> </div></td></tr>"
    })
    $("#rebateMode4").append('<thead style="display: table;width: 99%;table-layout: fixed;">' + str6 + '</thead>' + '<tbody style="display: block;height:300px;overflow-y:scroll;">' + tr + '</tbody>');
}


//返利模式
$('input[name="target_mode"]').on("change", function () {
    checkTargetVal = $('input[name="target_mode"]:checked').val();//返利模式
    if (checkTargetVal != 1)
        $("#forTargetMode1").hide();
    if (checkTargetVal == 1) {
        $("span[name='target_mode_change']").html("完成率（%）");
        // 按完成率
        $("input[name='rebate_mode'][value='4']").removeAttr("disabled");
        $("#forTargetMode1").show();
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 2) {
        // 按台数
        $("span[name='target_mode_change']").html("销量（台）");
        $("input[name='rebate_mode'][value='4']").removeAttr("disabled");
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 3) {
        // 按零售价
        $("span[name='target_mode_change']").html("零售价（元/台）");
        $("input[name='rebate_mode'][value='4']").attr("disabled", "disabled");
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 5) {
        // 按零售价
        $("span[name='target_mode_change']").html("批发价（元/台）");
        $("input[name='rebate_mode'][value='4']").attr("disabled", "disabled");
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 4) {
        // 按型号
        $("input[name='rebate_mode'][value='1']").prop("checked", "checked");
        $("input[name='rebate_mode'][value='4']").attr("disabled", "disabled");
        $("#rebateMode4").show();
        $("#rebateMode1").hide();
        ProductScope();
    }
})

//奖励金额
$('input[name="rebate_mode"]').on("change", function () {
    checkRebateVal = $('input[name="rebate_mode"]:checked').val();//奖励金额
    if (checkRebateVal == 1) {
        $("input[name='target_mode'][value='3']").removeAttr("disabled");
        $("span[name='rebate_mode_change']").html("奖励金额");
        str4 = "";
        str2 = "元/台";
    } else if (checkRebateVal == 2) {
        $("input[name='target_mode'][value='3']").removeAttr("disabled");
        $("span[name='rebate_mode_change']").html("批发价比例");
        str4 = "* 批发价";
        str2 = "%";
    } else if (checkRebateVal == 3) {
        $("input[name='target_mode'][value='3']").removeAttr("disabled");
        $("span[name='rebate_mode_change']").html("零售价比例");
        str4 = "* 零售价";
        str2 = "%";
    } else if (checkRebateVal == 4) {
        $("span[name='rebate_mode_change']").html("固定金额");
        $("input[name='target_mode'][value='3']").attr("disabled", "disabled");

        str4 = "";
        str2 = "元";
    }
    $("span[name='rebate_mode_input1']").html(str2);
    $("span[name='rebate_mode_input2']").html(str4);
})
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
            $('#rebateMode1 tr:eq(' + i + ') td:eq(0) input:eq(0)').val(parseInt(max) + 1);
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
    $tableProduct.show();
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
    { field: "area_l1_name", title: "经理片区", align: "center", visible: false },
    { field: "company_name", title: "分公司", align: "center", },
]
var tableProductHeader = [
    { field: "id", visible: false, },
    { field: "model", title: "型号", align: "center" },
    { field: "color", title: "颜色", align: "center", },
    { field: "price_wholesale", title: "批发价（元/台）", align: "center", },
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
    data['id'] = guid;
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
    if (checkTargetVal == 1)
        data['activity_target'] = $("#activity_target").val();
    data['note'] = $("#note").val();
    if (checkTargetVal != 4)    // 非按机型返利
    {
        var rewardList = [];
        var minInputList = $("input[name='target_min']");
        var maxInputList = $("input[name='target_max']");
        var rewardsList = $("button[name='rewards']");
        var rebInputList = $("input[name='rebate']");
        var len = minInputList.length;

        for (var i = 0; i < len; i++) {
            var reward = {};
            reward.main_id = guid;
            reward.target_min = $(minInputList[i]).val();
            if (i == len - 1)
                reward.target_max = -1; //以上
            else
                reward.target_max = $(maxInputList[i]).val();
            if ($(rewardsList[i]).html() == '奖') {
                reward.reward = $(rebInputList[i]).val();
            } else {
                reward.reward = 0 - $(rebInputList[i]).val();
            }
            if ($(maxInputList[i]).val() == '' || $(rebInputList[i]).val() == '') {
                $.modalAlert("奖励信息填写不完整！");
                return;
            }
            rewardList.push(reward);
        }
        data['rewardList'] = rewardList;
    }
    else {
        var rebInputList = $("input[name='reward_p']");
        var rewardsPList = $("button[name='rewards_p']");
        var len = pSelList.length;
        for (var i = 0; i < len; i++) {
            if ($(rewardsPList[i]).html() == '奖') {
                pSelList[i].reward = $(rebInputList[i]).val();
            } else {
                pSelList[i].reward = 0 - $(rebInputList[i]).val();
            }
            if ($(rebInputList[i]).val() == '') {
                $.modalAlert("奖励信息填写不完整！");
                return;
            }
        }
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
        url: "/ActivityManage/Recommendation/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/ActivityManage/Recommendation/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

