var id = $.request("id");
var cFullList = []; // 所有分公司
var cSelList = [];  // 已选分公司
var companyTree = [];
var company_linkname = decodeURI($.request("company_linkname"));
var name = decodeURI($.request("name"));
var company_id = $.request("company_id");
$("#company_linkname").text(company_linkname);
$("#name").text(name);
var emp_type = $('#emp_category').val();//员工类型

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

//是否设为公版
function BindTemplate() {
    $("#is_template").on("change", function () {
        document.getElementById("assessment_setting").checked = true;
        isTemplate = $("#is_template").val();
        if (isTemplate == 1) {
            document.getElementById("assessment_setting").disabled = true;
            $("#selectCompanyDiv").css("display", "none");
        }
        else if (isTemplate == 3) {
            document.getElementById("assessment_setting").disabled = false;
            $("#selectCompanyDiv").css("display", "");
        } else {
            document.getElementById("assessment_setting").disabled = false;
            $("#selectCompanyDiv").css("display", "none");
        }
    })
}

$('#emp_category').on("change", function () {
    emp_type = $('#emp_category option:selected').val();
    if (emp_type == 11) {
        $('#trainer').css("display", "");
        $('#trainerManage').css("display", "none");
    } else if (emp_type == 12) {
        $('#trainer').css("display", "none");
        $('#trainerManage').css("display", "");
    }
})

function submitForm() {
    if (!$("#form1").formValid())
        return false;

    var data = {};
    data = $("#form1").formSerialize();

    if ($("#effect_date").val() == '' && $("#effect_now").prop("checked") == false) {
        $.modalAlert("请选择生效时间");
        return;
    }
    data["effect_now"] = $("#effect_now").prop("checked");

    var assessment_setting1 = '';
    if ($("#assessment_setting").prop('checked') == true) {
        assessment_setting1 = "true";
    } else {
        assessment_setting1 = "false";
    }
    data["reset_all"] = assessment_setting1;

    data["company_name"] = company_linkname;
    data["company_id"] = company_id;
    data["position_name"] = name;
    data["position_id"] = id;

    //公版
    if (isTemplate == 3) {
        data["companyList"] = cSelList;
        if (!cSelList || cSelList.length < 1) {
            $.modalAlert("请选择分公司");
            return;
        }
    }
    data["company_id"] = myCompanyInfo.id;

    if (emp_type == 11) {
        $.submitForm({
            url: "/FinancialAccounting/PositionSalary/SetTrainer?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    window.location.href = "/FinancialAccounting/PositionSalary/Index?id=" + id;

                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    } else if (emp_type == 12) {
        $.submitForm({
            url: "/FinancialAccounting/PositionSalary/SetTrainerManager?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    window.location.href = "/FinancialAccounting/PositionSalary/Index?id=" + id;

                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    }
}
