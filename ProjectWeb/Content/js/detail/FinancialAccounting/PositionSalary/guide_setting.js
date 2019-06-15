var id = $.request("id");
var cFullList = []; // 所有分公司
var cSelList = [];  // 已选分公司
var companyTree = [];

var isTemplate = $("#is_template").val();
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

//立即生效 
function Effective(checkbox) {
    if (checkbox.checked == true) {
        $("#effect_date").val('');
        $("#effect_date").attr("disabled", true);
    } else {
        $("#effect_date").removeAttr("disabled");
    }
}
//删除
function deleteRow(e) {
    $(e).parents('tr').remove();
    setSale();
    setYearSale();
}
//浮动底薪数值判断
function setSale() {
    for (var i = 2; i < $("#float_salary_tr tr").length; i++) {
        if (i != $("#float_salary_tr tr").length - 1) {
            if (parseInt($('#float_salary_tr tr:eq(' + i + ') td:eq(0) input:eq(0)').val()) >= parseInt($('#float_salary_tr tr:eq(' + i + ') td:eq(0) input:eq(1)').val())) {
                $.modalAlert("该销量值应大于" + $('#float_salary_tr tr:eq(' + i + ') td:eq(0) input:eq(0)').val() + "！");
                $('#float_salary_tr tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
        var max = $('#float_salary_tr tr:eq(' + (i - 1) + ') td:eq(0) input:eq(1)').val();
        if (max != '') {
            $('#float_salary_tr tr:eq(' + i + ') td:eq(0) input:eq(0)').val(parseInt(max) + 1);
            if (parseInt($('#float_salary_tr tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(0)').val()) >= parseInt($('#float_salary_tr tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(1)').val())) {
                $('#float_salary_tr tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
    }
}
//销量数值判断
function setYearSale() {
    for (var i = 2; i < $("#sales_volume_tr tr").length; i++) {
        if (i != $("#sales_volume_tr tr").length - 1) {
            if (parseInt($('#sales_volume_tr tr:eq(' + i + ') td:eq(0) input:eq(0)').val()) >= parseInt($('#sales_volume_tr tr:eq(' + i + ') td:eq(0) input:eq(1)').val())) {
                $.modalAlert("该销量值应大于" + $('#sales_volume_tr tr:eq(' + i + ') td:eq(0) input:eq(0)').val() + "！");
                $('#sales_volume_tr tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
        var max = $('#sales_volume_tr tr:eq(' + (i - 1) + ') td:eq(0) input:eq(1)').val();
        if (max != '') {
            $('#sales_volume_tr tr:eq(' + i + ') td:eq(0) input:eq(0)').val(parseInt(max) + 1);
            if (parseInt($('#sales_volume_tr tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(0)').val()) >= parseInt($('#sales_volume_tr tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(1)').val())) {
                $('#sales_volume_tr tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
    }
}
//浮动底薪添加行 
function addFloatSalary(trObj) {
    var Tr = ' <tr><td><div class="input-group"><input class="form-control text-center" name="target_min" disabled><span class="input-group-addon">-</span><div class="input-single formValue"><input class="form-control text-center required number" name="target_max" onchange="setSale()"></div></div></td>'
       + '<td><div class="input-single formValue"><input class="form-control text-center required number" name="amountFloat"></div></td><td><div class="row"><i onclick="addFloatSalary(this)" class="fa fa-plus-circle fa-lg  text-blue "></i>&nbsp;<i onclick="deleteRow(this)" class="fa fa-minus-circle fa-lg   text-red"></i></div></td> </tr>';
    $(trObj).parents('tr').after(Tr);
    index = $(trObj).closest('tr')[0].rowIndex;
    setSale();
    emptyNum();

}

function addSalesVolume(trObj) {
    var Tr = ' <tr><td><div class="input-group"><input class="form-control text-center"name="year_min" disabled><span class="input-group-addon">-</span><div class="input-single formValue"><input class="form-control text-center required number"  name="year_max" onchange="setYearSale()"></div></div></td>'
        + '<td><div class="input-single formValue"><input class="form-control text-center required number" name="amountYear"></div></td><td><div class="row"><i onclick="addFloatSalary(this)" class="fa fa-plus-circle fa-lg  text-blue "></i>&nbsp;<i onclick="deleteRow(this)" class="fa fa-minus-circle fa-lg   text-red"></i></div></td> </tr>';
    $(trObj).parents('tr').after(Tr);
    index = $(trObj).closest('tr')[0].rowIndex;
    setYearSale();
    emptyNum2();
}
function emptyNum() {
    for (var j = index + 2; j < $("#float_salary_tr tr").length; j++) {
        $('#float_salary_tr tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
        $('#float_salary_tr tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("");
        if (j == $("#float_salary_tr tr").length - 1) {
            $('#float_salary_tr tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
            $('#float_salary_tr tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("以上");
        }
    }
}
function emptyNum2() {
    for (var j = index + 2; j < $("#sales_volume_tr tr").length; j++) {
        $('#sales_volume_tr tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
        $('#sales_volume_tr tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("");
        if (j == $("#sales_volume_tr tr").length - 1) {
            $('#sales_volume_tr tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
            $('#sales_volume_tr tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("以上");
        }
    }
}


//获取id
var myCompanyInfo = top.clients.loginInfo.companyInfo;
var companyList = {};    // 上级机构列表
$(function () {
    BindCompany();
    SelectCompany();
    ActionBind();
})

//上级机构
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

//是否设为公版
function BindTemplate() {
    $("#is_template").on("change", function () {
        document.getElementById("assessment_setting").checked = true;
        document.getElementById("assessment_setting2").checked = true;
        document.getElementById("assessment_setting3").checked = true;
        isTemplate = $("#is_template").val();
        if (isTemplate == 1) {
            document.getElementById("assessment_setting").disabled = true;
            document.getElementById("assessment_setting2").disabled = true;
            document.getElementById("assessment_setting3").disabled = true;
            $("#selectCompanyDiv").css("display", "none");
        }
        else if (isTemplate == 3) {
            document.getElementById("assessment_setting").disabled = false;
            document.getElementById("assessment_setting2").disabled = false;
            document.getElementById("assessment_setting3").disabled = false;
            $("#selectCompanyDiv").css("display", "");
        } else {
            document.getElementById("assessment_setting").disabled = false;
            document.getElementById("assessment_setting2").disabled = false;
            document.getElementById("assessment_setting3").disabled = false;
            $("#selectCompanyDiv").css("display", "none");
        }
    })
}

function submitForm() {
    // 提交验证
    if (!$("#form1").formValid())
        return false;

    var data = $("#form1").formSerialize();
    var info_list = [];//列表信息
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }

    var assessment_setting1 = '';
    if ($("#assessment_setting").prop('checked') == true) {
        assessment_setting1 = "true";
    } else {
        assessment_setting1 = "false";
    }
    data["reset_all_base"] = assessment_setting1;

    var assessment_setting2 = '';
    if ($("#assessment_setting2").prop('checked') == true) {
        assessment_setting2 = "true";
    } else {
        assessment_setting2 = "false";
    }
    data["reset_all_annualbonus"] = assessment_setting2;

    var assessment_setting3 = '';
    if ($("#assessment_setting3").prop('checked') == true) {
        assessment_setting3 = "true";
    } else {
        assessment_setting3 = "false";
    }
    data["reset_all"] = assessment_setting3;

    //年终奖
    data["guide_annualbonus_type"] =  $('input[name="guide_annualbonus_type"]:checked').val();;


    // 类型：1-星级制底薪；2-浮动底薪；3-按销量年终奖；4-按星级年终奖
    //底薪星级制
    var levelList = $("span[name='level']");
    var amountList = $("input[name='amount']");
    var len = levelList.length;
    for (var i = 0; i < len; i++) {
        var Star = {};
        Star.category = 1;
        Star.level = $(levelList[i]).text();
        Star.amount = $(amountList[i]).val();

        if ($(amountList[i]).val() == '') {
            $.modalAlert("底薪星级制信息不完整");
            return;
        }

        info_list.push(Star);
    }
    //浮动底薪
    var minInputList = $("input[name='target_min']");
    var maxInputList = $("input[name='target_max']");
    var amountFloatList = $("input[name='amountFloat']");
    var len1 = minInputList.length;
    for (var i = 0; i < len1; i++) {
        var Float = {};
        Float.category = 2;
        Float.target_min = $(minInputList[i]).val();
        if (i == len1 - 1)
            Float.target_max = -1; //以上
        else
            Float.target_max = $(maxInputList[i]).val();
        Float.amount = $(amountFloatList[i]).val();

        if (!$(maxInputList[i]).val() || $(maxInputList[i]).val() == '' || $(amountFloatList[i]).val()=='') {
            $.modalAlert("浮动底薪信息不完整");
            return;
        }
        info_list.push(Float);
    }
    //按销量年终奖
    var minYearList = $("input[name='year_min']");
    var maxYearList = $("input[name='year_max']");
    var amountYearList = $("input[name='amountYear']");
    var len3 = minYearList.length;
    for (var i = 0; i < len3; i++) {
        var Year = {};
        Year.category = 3;
        Year.target_min = $(minYearList[i]).val();
        if (i == len3 - 1)
            Year.target_max = -1; //以上
        else
            Year.target_max = $(maxYearList[i]).val();
        Year.amount = $(amountYearList[i]).val();

        if ($(maxYearList[i]).val() == '' || $(amountYearList[i]).val() == '') {
            $.modalAlert("年终奖销量信息不完整");
            return;
        }

        info_list.push(Year);
    }
    //按星级年终奖
    var yearlevelList = $("span[name='yearLevel']");
    var yearamountList = $("input[name='yearAmount']");
    var len4 = yearlevelList.length;
    for (var i = 0; i < len4; i++) {
        var yearStar = {};
        yearStar.category = 4;
        yearStar.level = $(yearlevelList[i]).text();
        yearStar.amount = $(yearamountList[i]).val();

        if ($(yearamountList[i]).val() == '') {
            $.modalAlert("年终奖星级制信息不完整");
            return;
        }

        info_list.push(yearStar);
    }
    data['info_list'] = info_list;

    data["is_template"] = isTemplate;
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

    if ($("#increase_effect_time").val() == '') {
        $.modalAlert("请选择增员奖励的生效时间");
        return;
    }
    $.submitForm({
        url: "/FinancialAccounting/PositionSalary/SetGuide?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/PositionSalary/Index?id="+id;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
