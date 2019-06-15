
$(function () {
    GetCompanyList();
    $("#select2-company_id-container").width("220px"); // 改变所属机构显示的宽度，原select2插件生成的显示宽度仅150px。
})

function GetCompanyList() {
    $.ajax({
        url: "/OrganizationManage/Company/GetIdNameCategoryList",
        dataType: "json",
        async: false,
        success: function (data) {
            var optionStr = "";
            $.each(data, function (i) {
                optionStr += "<option value='" + data[i]["id"] + "' category='"
                            + data[i]["category"] + "'>" + data[i]["name"] + "</option>";
            });
            $("#company_id").append(optionStr);
            $("#company_id").select2({
                minimumResultsForSearch: 0, tags: false
            });
            BindDepartment();
            $("#company_id").on("change", function (e) {
                $("#select2-" + $("#company_id").attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
                BindDepartment();
            });
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

function BindDepartment() {
    var curCompany = $("#company_id").val();
    if (curCompany > 0) {
        $("#dept_id").empty();
        $("#dept_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Department/GetIdNameList",
            param: { company_id: curCompany },
            firstText: "--无所属部门--",
        });
    }
    BindGradeSalary();
}
var curCategory = "";
function BindGradeSalary() {
    var category = $("#company_id").find('option:selected').attr("category");
    if (category == curCategory)
        return;
    curCategory = category;
    $("#grade_salary_category").empty();
    if (category == "分公司") {
        $("#grade_salary_category").append("<option value='1'>行政管理</option>"
                        + "<option value='2'>市场销售</option>"
                        + "<option value='3'>终端管理</option>"
                        + "<option value='4'>运营商中心</option>");
    }
    else {
        $("#grade_salary_category").append("<option value='1'>行政管理</option>"
                        + "<option value='4'>运营商中心</option>");
    }

    $("#position_type").empty();
    if (category == "分公司") {
        $("#position_type").append("<option value='7'>行政普通员工</option>"
                        + "<option value='3'>分公司总经理</option>"
                        + "<option value='4'>分公司助理</option>"
                        + "<option value='5'>部门经理</option>"
                        + "<option value='6'>部门主管</option>"
                        + "<option value='11'>培训经理</option>"
                        + "<option value='12'>培训师</option>"
                        + "<option value='21'>业务经理</option>"
                        + "<option value='22'>业务员</option>"
                        + "<option value='31'>导购员</option>");

    

    }
    else if (category == "事业部") {
        $("#position_type").append("<option value='7'>行政普通员工</option>"
                        + "<option value='1'>事业部总经理</option>"
                        + "<option value='2'>事业部助理</option>"
                        + "<option value='5'>部门经理</option>"
                        + "<option value='6'>部门主管</option>");
    }
    else {
        $("#position_type").append("<option value='0'>普通员工</option>");
    }
}

$("#position_type").on("change", function () {
        if ($("#position_type").val() == 31) {//导购员
            $("#position_change").text("导购薪资")
        } else if ($("#position_type").val() == 21 || $("#position_type").val() == 22) {//业务
            $("#position_change").text("业务薪资")
        } else if ($("#position_type").val() == 11 || $("#position_type").val() == 12) {//培训
            $("#position_change").text("培训薪资")
        } else {//行政
            $("#position_change").text("行政薪资")
        }
})

function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var data = $("#form1").formSerialize();
    if (!data["name"]) {
        alert("请输入职位名称");
        return;
    }
    data["grade_level_display"] = $("#grade_level").find("option:selected").text();
    $.submitForm({
        url: "/OrganizationManage/Position/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                //$.currentWindow().ReSearch();//更新
                //top.layer.closeAll();
                window.location.href = "/OrganizationManage/Position/Index";

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
