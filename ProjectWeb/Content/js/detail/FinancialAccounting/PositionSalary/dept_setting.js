var CurDept = $("#dept_id").val();
var cur_company = 0;

var str = '<tr><th width="10%">类型</th><th width="10%">职层</th><th width="10%">职等</th><th width="30%">建议金额（元）</th> <th width="15%">标准金额（元）</th>  <th width="25%">KPI（元）</th></tr>';
$(function () {
    ActionBinding();
    BindCompany();
})

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
            BindDepartment();
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}


function BindDepartment() {
    $("#dept_id").empty();
    $.ajax({
        url: "/OrganizationManage/Department/GetDeptAddrList?id=" + cur_company,
        dataType: "json",
        async: false,
        success: function (data) {
            var optionStr = "<option value='0'>--无所属部门--</option>";
            $.each(data, function (i) {
                optionStr += "<option value='" + data[i]["id"] + "' grade='"
                    + data[i]["grade_category"] + "' addr='" + data[i]["addr_str"] + "'>" + data[i]["name"] + "</option>";
            });
            $("#dept_id").append(optionStr);
            $("#dept_id").select2({
                minimumResultsForSearch: 0, tags: false
            });
            GetDeptGrade();
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

// 绑定标签事件
function ActionBinding() {
    $("#company_id").on("change", function (e) {
        $("#select2-" + $("#company_id").attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
        cur_company = $("#company_id").val();
        $("#deptKpi").empty();
        BindDepartment();
    });
    $("#dept_id").on("change", function (e) {
        $("#deptKpi").empty();
        $("#select2-" + $("#dept_id").attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
        CurDept = $("#dept_id").val();
        GetDeptGrade();
    });
}

function GetDeptGrade() {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetCurrentDept",
        type: "get",
        data: { dept_id: CurDept, company_id: cur_company },
        dataType: "json",
        success: function (data) {
            if (data.err_msg != null && data.err_msg != '') {
                $("#err-message").text(data.err_msg);
                $("#err-message").show();
            } else {
                $("#err-message").hide();
            }
            //$("#form1").formSerialize(data);
            var tr;
            $.each(data.adviceInfoList, function (index, item) {//item.grade
                if (!item.kpi_advice || item.kpi_advice == "null")
                    item.kpi_advice = "-";
                tr += '<tr><td><span name="grade_category"  data-grade_category="' + item.grade_category + '" class="form-control no-border">' + item.grade_category_display + '<span></td><td><span  name="grade_level" data-grade_level="' + item.grade_level + '" class="form-control no-border">' + item.grade_level_display + '<span></td><td><span name="grade" class="form-control no-border">' + item.grade + '<span></td>' + '<td><span name="kpi_advice" class="form-control no-border">' + item.kpi_advice + '<span></td>' + '<td>' + '<input class="form-control text-center" name="kpi_standard" value="0"/>' + '</td>' + '<td><span class="form-control no-border">打分分值/100*标准金额<span></td></tr>';
            })
            $("#deptKpi").append('<thead>' + str + '</thead>' + '<tbody>' + tr + '</tbody>');


        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

//获取id
var myCompanyInfo = top.clients.loginInfo.companyInfo;
var companyList = {};    // 上级机构列表
var guid = "";
$(function () {
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
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    var info_list = [];

    var gradeCategoryList = $("span[name='grade_category']");
    var gradeLevelList = $("span[name='grade_level']");
    var gradeList = $("span[name='grade']");
    var KpiAdviceList = $("span[name='kpi_advice']");
    var kpiStandardList = $("input[name='kpi_standard']");
    var len = gradeList.length;
    for (var i = 0; i < len; i++) {
        var Grade = {};
        Grade.grade_category = $(gradeCategoryList[i]).data('grade_category');
        Grade.grade_level = $(gradeLevelList[i]).data('grade_level');
        Grade.grade = $(gradeList[i]).text();
        Grade.kpi_advice = $(KpiAdviceList[i]).text();
        Grade.kpi_standard = $(kpiStandardList[i]).val();

        if ($(kpiStandardList[i]).val() == '') {
            $.modalAlert("标准金额不能为空");
            return;
        }

        info_list.push(Grade);
    }
    data["info_list"] = info_list;
    if ($("#effect_date").val() == '' && $("#effect_now").prop("checked") == false) {
        $.modalAlert("请选择生效时间");
        return;
    }
    var assessment_setting = '';
    if ($("#assessment_setting3").prop('checked') == true) {
        assessment_setting = "true";
    } else {
        assessment_setting = "false";
    }
    data["effect_now"] = $("#effect_now").prop("checked");
    data["reset_all"] = assessment_setting;
    data["company_id"] = $("#company_id").val();
    data["dept_id"] = $("#dept_id").val();
    $.submitForm({
        url: "/FinancialAccounting/PositionSalary/SetDept?date=" + new Date(),
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

