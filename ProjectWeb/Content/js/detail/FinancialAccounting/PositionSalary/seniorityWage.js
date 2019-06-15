var id = $.request("id");
var myCompanyInfo = top.clients.loginInfo.companyInfo;
$("#company_name").text(myCompanyInfo.name);

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
            if (parseInt($('#seniorityWage_table tr:eq(' + i + ') td:eq(0) input:eq(0)').val()) >= parseInt($('#seniorityWage_table tr:eq(' + i + ') td:eq(0) input:eq(1)').val())) {
                $.modalAlert("该销量值应大于" + $('#seniorityWage_table tr:eq(' + i + ') td:eq(0) input:eq(0)').val() + "！");
                $('#seniorityWage_table tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
        var max = $('#seniorityWage_table tr:eq(' + (i - 1) + ') td:eq(0) input:eq(1)').val();
        if (max != '') {
            $('#seniorityWage_table tr:eq(' + i + ') td:eq(0) input:eq(0)').val(parseInt(max) + 1);
            if (parseInt($('#seniorityWage_table tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(0)').val()) >= parseInt($('#seniorityWage_table tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(1)').val())) {
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
//提交
function submitForm() {
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
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
        info_list.push(salary);
    }
    data['info_list'] = info_list;
    data["effect_now"] = $("#effect_now").prop("checked");
    data["id"] = guid;
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
