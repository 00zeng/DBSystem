var parentCompanyList = {};    // 上级机构列表
var myCompanyInfo = top.clients.loginInfo.companyInfo;

$(function () {
    // 初始化机构类型
    if (myCompanyInfo.category == "分公司") {
        $.modalAlert("分公司人员无权限添加机构", "error");
        window.location.href = "/OrganizationManage/Company/Index";
    }
    else if (myCompanyInfo.category == "事业部") {
        $("#category").append($("<option></option>").val("分公司").html("分公司"));
        $("#gradeMarketing,#gradeTerminal,#typeWrap").show();
    }
    else if (myCompanyInfo.category == "董事会") {
        $("#category").append($("<option></option>").val("事业部").html("事业部"));
        $("#category").append($("<option></option>").val("分公司").html("分公司"));
        $("#gradeMarketing,#gradeTerminal,#typeWrap").hide();
    }
    else {
        $("#category").append($("<option></option>").val("董事会").html("董事会"));
        $("#category").append($("<option></option>").val("事业部").html("事业部"));
        $("#category").append($("<option></option>").val("分公司").html("分公司"));
        $("#gradeMarketing,#gradeTerminal,#typeWrap").hide();
    }

    BindParentCompany();
    //机构类型改变
    $("#category").on("change", function () {
        BindParentCompany();
        if ($("#category").val() == "分公司") {
            $("#gradeMarketing,#gradeTerminal,#typeWrap").show();
        }
        else {
            $("#gradeMarketing,#gradeTerminal,#typeWrap").hide();
            $("#type").val(1);
        }
    })

    //添加等级
    $(".addGrade").on('click', function () {
        $(this).parent().parent().find(".gradeListWrap").append(" <div class='col-xs-1 gradeWrap'><input type='text' class='form-control' />"
                + "<i class='fa fa-times pull-right deleteGrade'></i>"
                + "</div>");
    });
    //等级删除按钮
    $("body").on("mouseover", ".gradeWrap", function () {
        $(this).find(".deleteGrade").show();
    });
    $("body").on("mouseleave", ".gradeWrap", function () {
        $(this).find(".deleteGrade").hide();
    });
    $("body").on("click", ".deleteGrade", function () {
        $(this).parent().remove();
    });
})

function GetParentCompanyList(cur_category) {
    $.ajax({
        url: "/OrganizationManage/Company/GetIdNameList",
        data: { category: cur_category, param_type: 1 },
        dataType: "json",
        async: false,
        success: function (data) {
            parentCompanyList[cur_category] = data;
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}
//上级机构
function BindParentCompany() {
    var cur_category = $("#category").val();
    if (!parentCompanyList[cur_category])
        GetParentCompanyList(cur_category);

    $("#parent_id").empty();
    $("#parent_id").bindSelectLocal({
        data: parentCompanyList[cur_category],
        text: "name",
    });
}

function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var data = $("#form1").formSerialize();
    if (!data["name"]) {
        alert("请输入机构名称");
        return;
    }

    if ($("#category").val() == "分公司") {
        data["type"] == $("#type").val();
    } else {
        data["type"] == "";
    }
    var addrCity = $("#addrCity .select-item");
    data["city_code"] = $(addrCity).length > 0 ? $(addrCity).last().data("code") : "";

    var gradeList = [];
    $("#grade11").find("input").each(function (i) {
        var gradeInfo = {};
        gradeInfo["grade"] = $(this).val().trim();
        if (!!gradeInfo["grade"]) {
            gradeInfo["grade_level"] = 1;
            gradeInfo["grade_level_display"] = "公司层面";
            gradeInfo["grade_category"] = 1;
            gradeInfo["grade_category_display"] = "行政管理";
            gradeList.push(gradeInfo);
        }
    });
    $("#grade12").find("input").each(function (i) {
        var gradeInfo = {};
        gradeInfo["grade"] = $(this).val().trim();
        if (!!gradeInfo["grade"]) {
            gradeInfo["grade_level"] = 2;
            gradeInfo["grade_level_display"] = "部门层面";
            gradeInfo["grade_category"] = 1;
            gradeInfo["grade_category_display"] = "行政管理";
            gradeList.push(gradeInfo);
        }
    });
    $("#grade41").find("input").each(function (i) {
        var gradeInfo = {};
        gradeInfo["grade"] = $(this).val().trim();
        if (!!gradeInfo["grade"]) {
            gradeInfo["grade_level"] = 1;
            gradeInfo["grade_level_display"] = "公司层面";
            gradeInfo["grade_category"] = 4;
            gradeInfo["grade_category_display"] = "运营商中心";
            gradeList.push(gradeInfo);
        }
    });
    $("#grade42").find("input").each(function (i) {
        var gradeInfo = {};
        gradeInfo["grade"] = $(this).val().trim();
        if (!!gradeInfo["grade"]) {
            gradeInfo["grade_level"] = 2;
            gradeInfo["grade_level_display"] = "部门层面";
            gradeInfo["grade_category"] = 4;
            gradeInfo["grade_category_display"] = "运营商中心";
            gradeList.push(gradeInfo);
        }
    });
    if (data["category"] == "分公司") {
        $("#grade21").find("input").each(function (i) {
            var gradeInfo = {};
            gradeInfo["grade"] = $(this).val().trim();
            if (!!gradeInfo["grade"]) {
                gradeInfo["grade_level"] = 2;
                gradeInfo["grade_level_display"] = "部门层面";
                gradeInfo["grade_category"] = 2;
                gradeInfo["grade_category_display"] = "市场销售";
                gradeList.push(gradeInfo);
            }
        });
        $("#grade31").find("input").each(function (i) {
            var gradeInfo = {};
            gradeInfo["grade"] = $(this).val().trim();
            if (!!gradeInfo["grade"]) {
                gradeInfo["grade_level"] = 2;
                gradeInfo["grade_level_display"] = "部门层面";
                gradeInfo["grade_category"] = 3;
                gradeInfo["grade_category_display"] = "终端管理";
                gradeList.push(gradeInfo);
            }
        });
    }
    data["grade_list"] = gradeList;
    $.submitForm({
        url: "/OrganizationManage/Company/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/OrganizationManage/Company/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
