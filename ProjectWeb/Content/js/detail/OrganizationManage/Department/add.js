var managerNotAdd = false;
$(function () {
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
    
    $("#select2-company_id-container").width("220px"); // 改变所属机构显示的宽度，原select2插件生成的显示宽度仅150px。  

    
    $("#managerNotAdd").on("click", function () {
        managerNotAdd = $("#managerNotAdd").prop('checked');
        if (managerNotAdd == true) 
            $("#form2").hide();
        else
            $("#form2").show();
    });
})

function BindDepartment() {
    var curCompany = $("#company_id").val();
    if (curCompany > 0)
    {
        $("#parent_id").empty();
        $("#parent_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Department/GetIdNameList",
            param: { company_id: curCompany },
            firstText: "--无上级部门--",
        });
        GetCompanyAddr(curCompany);
        BindGradeSalary();
    }
}

function GetCompanyAddr(curCompany) {
    $.ajax({
        url: "/OrganizationManage/Company/GetAddr?date=" + new Date(),
        type: "get",
        data: { id: curCompany },
        dataType: "json",
        success: function (data) {
            if (!!data) {
                // Key-Value pair, key refers to city and value refers to address
                $("#city").val(data.Key).trigger("change");
                $("#address").val(data.Value);
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

var curCategory = "";
function BindGradeSalary() {
    var category = $("#company_id").find('option:selected').attr("category");
    if (category == curCategory)
        return;
    curCategory = category;
    $("#grade_category").empty();
    if (category == "分公司") {
        $("#grade_category").append("<option value='1'>行政管理</option>"
                        + "<option value='2'>市场销售</option>"
                        + "<option value='3'>终端管理</option>"
                        + "<option value='4'>运营商中心</option>");
    }
    else {
        $("#grade_category").append("<option value='1'>行政管理</option>"
                        + "<option value='4'>运营商中心</option>");
    }

    $("#position_type").empty();
    if (category == "分公司") {
        $("#position_type").append("<option value='5'>部门经理</option>"
                        + "<option value='3'>分公司总经理</option>"
                        + "<option value='4'>分公司助理</option>");
    }
    else if (category == "事业部") {
        $("#position_type").append("<option value='5'>部门经理</option>"
                        + "<option value='1'>事业部总经理</option>"
                        + "<option value='2'>事业部助理</option>");
    }
    else {
        $("#position_type").append("<option value='5'>部门经理</option>");
    }
}

function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var data = $("#form1").formSerialize();
    if (!managerNotAdd) {
        if (!$("#form2").formValid())
            return false;
        data['position_name'] = $("#position_name").val();
        data['grade_level'] = $("#grade_level").val();
        data['position_type'] = $("#position_type").val();
        //data['salary_category'] = $("#salary_category").val();
        data["grade_category_display"] = $("#grade_category").find("option:selected").text();
        data["grade_level_display"] = $("#grade_level").find("option:selected").text();
    }
    else
        data['position_name'] = "";
    if (!data["name"]) {
        alert("请输入部门名称");
        return;
    }
    
    var addrCity = $("#addrCity .select-item");
    data["city_code"] = $(addrCity).length > 0 ? $(addrCity).last().data("code") : "";

    $.submitForm({
        url: "/OrganizationManage/Department/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/OrganizationManage/Department/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
