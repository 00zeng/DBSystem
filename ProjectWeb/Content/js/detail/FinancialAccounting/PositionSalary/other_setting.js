//其他设置
var id = $.request("id");
var cFullList = []; // 所有分公司
var cSelList = [];  // 已选分公司
var companyTree = [];
var myCompanyInfo = top.clients.loginInfo.companyInfo;
$("#company_name").text(myCompanyInfo.name);
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

$(function () {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetCurrentSeniority?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});

//将所有个人设置重置成此方案
function AssessmentSetting(checkbox) {
    if (checkbox.checked == true) {

    } else {

    }
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

//获取id
var myCompanyInfo = top.clients.loginInfo.companyInfo;
var companyList = {};    // 上级机构列表
var guid = "";
$(function () {
    if (myCompanyInfo.category == "分公司") {
        $("#category").append($("<option></option>").val("分公司").html("分公司"));
        $("#company_id").append($("<option></option>").val(myCompanyInfo.id).html(myCompanyInfo.name));
    }
    else {
        $("#category").append($("<option></option>").val("事业部").html("事业部"));
        $("#category").append($("<option></option>").val("分公司").html("分公司"));
        companyList["事业部"] = [{ id: myCompanyInfo.id, name: myCompanyInfo.name }];
        $("#company_id").append($("<option></option>").val(myCompanyInfo.id).html(myCompanyInfo.name));
        $("#category").on("change", function () {
            BindCompany();
        })
    }
    GetGUID();
    SelectCompany();
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

//上级机构
function BindCompany() {
    var cur_category = $("#category").val();
    if (cur_category == "分公司") {
        $("#company").css("display", "none");
        $("#template").css("display", "");
        if ($("#is_template").val() == 3) {
            $("#selectCompanyDiv").css("display", "");
        }
        BindTemplate();
    } else if (cur_category == "事业部") {
        document.getElementById("other_setting").checked = true;
        document.getElementById("other_setting").disabled = false;
        $("#company").css("display", "");
        $("#template").css("display", "none");
        $("#selectCompanyDiv").css("display", "none");
    }
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
                value.main_id = guid;
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
        document.getElementById("other_setting").checked = true;
        isTemplate = $("#is_template").val();
        if (isTemplate == 1) {
            document.getElementById("other_setting").disabled = true;
            $("#selectCompanyDiv").css("display", "none");
        }
        else if (isTemplate == 3) {
            document.getElementById("other_setting").disabled = false;
            $("#selectCompanyDiv").css("display", "");
        } else {
            document.getElementById("other_setting").disabled = false;
            $("#selectCompanyDiv").css("display", "none");
        }
    })
}

function addRow(trObj) {
    var Tr = ' <tr><td><div class="input-group"><input class="form-control text-center" disabled  name="year_min"><span class="input-group-addon">-</span><input class="form-control text-center" name="year_max" onchange="seniorityWage()"></div></td>'
        + '<td><div class="input-group" ><input class="form-control text-center" name="salary"></div></td><td><div class="row"><i onclick="addRow(this)" class="fa fa-plus-circle fa-lg  text-blue "></i>&nbsp;<i onclick="deleteRow(this)" class="fa fa-minus-circle fa-lg   text-red"></i></div></td> </tr>';
    $(trObj).parents('tr').after(Tr);
    index = $(trObj).closest('tr')[0].rowIndex;
    seniorityWage();
    emptyNum();
}
function deleteRow(e) {
    $(e).parents('tr').remove();
    seniorityWage();
}
function seniorityWage() {
    for (var i = 2; i < $("#seniorityWage_table tr").length; i++) {
        if (i != $("#seniorityWage_table tr").length - 1) {
            if (parseInt($('#seniorityWage_table tr:eq(' + i + ') td:eq(0) input:eq(0)').val()) >  parseInt($('#seniorityWage_table tr:eq(' + i + ') td:eq(0) input:eq(1)').val())) {
                $.modalAlert("该销量值应大于" + $('#seniorityWage_table tr:eq(' + i + ') td:eq(0) input:eq(0)').val() + "！");
                $('#seniorityWage_table tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
        var max = $('#seniorityWage_table tr:eq(' + (i - 1) + ') td:eq(0) input:eq(1)').val();
        if (max != '') {
            $('#seniorityWage_table tr:eq(' + i + ') td:eq(0) input:eq(0)').val(parseInt(max) + 1 );
            if (parseInt($('#seniorityWage_table tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(0)').val()) > parseInt($('#seniorityWage_table tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(1)').val())) {
                $('#seniorityWage_table tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
    }
}
function emptyNum() {
    for (var j = index + 2; j < $("#seniorityWage_table tr").length; j++) {
        $('#seniorityWage_table tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
        $('#seniorityWage_table tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("");
        if (j == $("#seniorityWage_table tr").length - 1) {
            $('#seniorityWage_table tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
            $('#seniorityWage_table tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("以上");
        }
    }
}

$("input[name='intern_salary_type']").on('change', function () {
    $intern_salary_type = $(this).val();
    if ($intern_salary_type == 1) {
        $("input[name='ratio_hidden']").css('display', '');
        $("span[name='ratio_hidden']").css('display', '');
        $("input[name='salary_hidden']").css('display', 'none');
        $("span[name='salary_hidden']").css('display', 'none');
        $("#intern_fix_salary").val('');
    } else if ($intern_salary_type == 2) {
        $("input[name='salary_hidden']").css('display', '');
        $("span[name='salary_hidden']").css('display', '');
        $("input[name='ratio_hidden']").css('display', 'none');
        $("span[name='ratio_hidden']").css('display', 'none');
        $("#intern_ratio_salary").val('');
    }
});

//提交
function submitForm() {
    if (!$("#form1").formValid())
        return false;

    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }

    if ($('input[name="intern_salary_type"]:checked').val() == 1) {//按总工资比例
        data['intern_ratio_salary'] = $("#intern_ratio_salary").val();
        if ($("#intern_ratio_salary").val() == '') {
            $.modalAlert("请输入总工资比例");
            return;
        } else if (isNaN($("#intern_ratio_salary").val()) || $("#intern_ratio_salary").val() < 0) {
            $.modalAlert("总工资比例输入有错");
            return;
        }
    }
    else if ($('input[name="intern_salary_type"]:checked').val() == 2) {//按固定金额
        data['intern_fix_salary'] = $("#intern_fix_salary").val();
        if ($("#intern_fix_salary").val() == '') {
            $.modalAlert("请输入固定金额");
            return;
        } else if (isNaN($("#intern_fix_salary").val()) || $("#intern_fix_salary").val()<0) {
            $.modalAlert("固定金额输入有错");
            return;
        }
    }

    var minInputList = $("input[name='year_min']");
    var maxInputList = $("input[name='year_max']");
    var salaryInputList = $("input[name='salary']");
    var len = minInputList.length;
    var info_list = [];//工龄工资
    for (var i = 0; i < len; i++) {
        var salary = {};
        salary.salary_position_id = guid;
        salary.year_min = $(minInputList[i]).val();
        if (i == len - 1)
            salary.year_max = -1; //以上
        else
            salary.year_max = $(maxInputList[i]).val();
        salary.salary = $(salaryInputList[i]).val();

        if ($(maxInputList[i]).val() == '' || $(salaryInputList[i]).val() == '') {
            $.modalAlert("工龄工资信息不完整");
            return;
        }

        info_list.push(salary);
    }
    //公版
    data["is_template"] = isTemplate;
    if ($("#category").val() == "事业部") {
        data["is_template"] = 0;
        data["category"] = 1;
    } else if ($("#category").val() == "分公司") {
        data["category"] = 2;
        if (isTemplate == 3) {
            data["companyList"] = cSelList;
            if (!cSelList || cSelList.length < 1) {
                $.modalAlert("请选择分公司");
                return;
            }
        }
    }
    data["company_id"] = myCompanyInfo.id;

    var others_setting = '';
    if ($("#other_setting").prop('checked') == true) {
        others_setting = "true";
    } else {
        others_setting = "false";
    }
    data['reset_all'] = others_setting;
    data['info_list'] = info_list;
    if ($("#effect_date").val() == '' && $("#effect_now").prop("checked") == false) {
        $.modalAlert("请选择生效时间");
        return;
    }
  
    data["effect_now"] = $("#effect_now").prop("checked");
    data["effect_date"] = $("#effect_date").val();
    $.submitForm({
        url: "/FinancialAccounting/PositionSalary/SetSeniority?date=" + new Date(),
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
