var id = $.request("id");
var cFullList = []; // 所有分公司
var cSelList = [];  // 已选分公司
var companyTree = [];
var company_linkname = decodeURI($.request("company_linkname"));
var name = decodeURI($.request("name"));
var company_id = $.request("company_id");
$("#company_linkname").text(company_linkname);
$("#name").text(name);

var isTemplate = $("#is_template").val();
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

var $duallistCompany = $('#duallistCompany').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入名称、区域关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个分公司",
    infoTextEmpty: "共0个分公司",
    infoTextFiltered: "",
    filterTextClear: "清空"
});

$("input[name='perf_mode']").on('change', function () {
    var value = $(this).val();
    if (value == 1) {
        $("#amounts").css('display', '');

    } else if (value == 2) {
        $("#amounts").css('display', 'none');
        $("#perf_target").val('');
    }
});

//获取id
var myCompanyInfo = top.clients.loginInfo.companyInfo;
var companyList = {};    // 上级机构列表
$(function () {
    BindCompany();
    SelectCompany();
    ActionBind();
})

function BindCompany() {
    if ($("#is_template").val() == 3) {
        $("#selectCompanyDiv").css("display", "");
    }
    BindTemplate();
}

function SelectCompany() {
    $.ajax({
        url: "/OrganizationManage/Company/GetIdNameList",
        async: false,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            cFullList = data;
            $.each(data, function (index, value) {
                //value.main_id = guid;
                $duallistCompany.append("<option value='" + value.id + "'>" + value.name + "</option>");
            })
            $duallistCompany.bootstrapDualListbox("refresh");
        },
        error: function (data) {
            $.modalAlert("系统出错，请稍候重试", "error")
            $("#emp_count").text(0);
        }
    })
}


function ActionBind() {
    $("input[name='base_salary']").on('change', function () {
        var value = $(this).val();
        if (value == 4) {
            $("#salary").css('display', '');

        } else {
            $("#salary").css('display', 'none');
        }
    });
    // Dual List Box START
    $("#modalCompany").on('hide.bs.modal', function (e) {
        cSelList.splice(0, cSelList.length);    // 清空数组
        var selIds = $duallistCompany.val();
        $.each(cFullList, function (i, item) {
            if ($.inArray(item['id'], selIds) > -1)
                cSelList.push(item);
        });
        $("#emp_count").text(cSelList.length);
        ShowCompany();
    })

}

function ShowCompany() {
    $("#ShowCompany").css('height', '100%');
    $("#ShowCompany").empty();//清空
    $.each(cSelList, function (index, item) {
        $("#ShowCompany").append("<button type='button' class='btn btn-primary btn-xs' style='margin:2px'>" + item.name + "</button>");
    })
    if (cSelList.length < 1) {
        $("#ShowCompany").css('height', '34px');
    }
}


//返利模式
$('input[name="target_mode"]').on("change", function () {
    checkTargetVal = $('input[name="target_mode"]:checked').val();//返利模式
    if (checkTargetVal != 1)
        $("#forTargetMode1").hide();
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
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 3) {
        // 按零售价
        $("span[name='target_mode_change']").html("零售价（元/台）");
        $("input[name='normal_rebate_mode'][value='4']").attr("disabled", "disabled");
        $("input[name='buyout_rebate_mode'][value='4']").attr("disabled", "disabled");

        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 5) {
        // 批发价
        $("span[name='target_mode_change']").html("批发价（元/台）");
        $("input[name='normal_rebate_mode'][value='4']").attr("disabled", "disabled");
        $("input[name='buyout_rebate_mode'][value='4']").attr("disabled", "disabled");
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


//是否设为公版 
function BindTemplate() {
    $("#is_template").on("change", function () {
        document.getElementById("assessment_setting").checked = true;
        document.getElementById("assessment_setting2").checked = true;
        isTemplate = $("#is_template").val();
        if (isTemplate == 1) {
            document.getElementById("assessment_setting").disabled = true;
            document.getElementById("assessment_setting2").disabled = true;
            $("#selectCompanyDiv").css("display", "none");
        }
        else if (isTemplate == 3) {
            document.getElementById("assessment_setting").disabled = false;
            document.getElementById("assessment_setting2").disabled = false;
            $("#selectCompanyDiv").css("display", "");
        } else {
            document.getElementById("assessment_setting").disabled = false;
            document.getElementById("assessment_setting2").disabled = false;
            $("#selectCompanyDiv").css("display", "none");
        }
    })
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
    var subList = [];//列表信息
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data['reset_all_perf'] = $("#reset_all_perf").prop("checked");
    data['reset_all'] = $("#reset_all").prop("checked");
    //职位类型
    data['category'] = $("#emp_category").val();


    // 类型：1-完成率；2-按销量
    //完成率
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

        if ($(maxInputList[i]).val() == '' || $(normalRateList[i]).val() == '' || $(buyoutRateList[i]).val() == '') {
            $.modalAlert("月度考核信息不完整");
            return;
        }

        subList.push(Rate);
    }
    data['subList'] = subList;
    //公版
    if (isTemplate == 3) {
        data["companyList"] = cSelList;
        if (!cSelList || cSelList.length < 1) {
            $.modalAlert("请选择分公司");
            return;
        }
    } 
    data["company_id"] = myCompanyInfo.id;
    //生效日期
    if ($("#effect_date").val() == '' && $("#effect_now").prop("checked") == false) {
        $.modalAlert("请选择生效时间");
        return;
    }
    data["effect_now"] = $("#effect_now").prop("checked");
    $.submitForm({
        url: "/FinancialAccounting/PositionSalary/SetSales?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/PositionSalary/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
