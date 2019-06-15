var id = $.request("id");

$(function () {
    ActionBinding();
    BindCompany();
    BindRole();
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        //async:false,
        success: function (data) {
           
            $("#form1").formSerializeShow(data.empJobInfo);
          
            if (data.empJobInfo.position_name == "导购员") //导购
            {
                $("#distributorWrap").show();
                $("#areaL1Wrap").show();
                $("#areaL2Wrap").show();
                $("#guideSpecificWrap").show();
                $("#introducer_name").val("");
            }
            else if (data.empJobInfo.position_name == "业务员") {//业务员
                $("#distributorWrap").hide();
                $("#areaL1Wrap").show();
                $("#areaL2Wrap").show();
                $("#guideSpecificWrap").hide();
            }
            else if (data.empJobInfo.position_name == "业务经理") {//业务经理
                $("#distributorWrap").hide();
                $("#areaL2Wrap").hide();
                $("#areaL1Wrap").show();
                $("#guideSpecificWrap").hide();
            }
            else {
                $("#guideSpecificWrap").hide();
            }
            $("#company_id").val(data.empJobInfo.company_id).trigger("change");
            $("#dept_id").val(data.empJobInfo.dept_id).trigger("change");
            $("#position_id").val(data.empJobInfo.position_id).trigger("change");
            $("#grade").val(data.empJobInfo.grade).trigger("change");          
            $("#emp_category1").val(data.empJobInfo.emp_category).trigger("change");//雇员类别
           
            if (data.accountInfo.role_id == 20) {  //新入职员工可修改入职时间
                $("#startDate").val(data.empJobInfo.entry_date);
                $("#startDate").off().on("focus", function () {
                    var startDate = data.empJobInfo.entry_date;
                    WdatePicker({readOnly:true, startDate: startDate });
                });
            } else {
                $("#startDate").val(data.empJobInfo.create_time);
                $("#startDate").off().on("focus", function () {
                    var startDate = data.empJobInfo.create_time;
                    WdatePicker({readOnly:true, startDate: startDate });
                });
            }
          
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });

});

// 绑定标签事件
function ActionBinding() {
    $("#company_id").on("change", function (e) {
        $("#select2-" + $("#company_id").attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
        cur_company = $("#company_id").val();
        BindDepartment();
        BindEmpCategory();
    });
    $("#dept_id").on("change", function (e) {
        grade_category = $("#dept_id").find('option:selected').attr("grade");
        $("#select2-" + $("#dept_id").attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
        BindPosition();
    });
    $("#position_id").on("change", function (e) {
        GetGradeList();
    });
    $("#area_l1_id").on("change", function (e) {
        GetAreaL2List();
        if (position_type == 31)    // 导购员
            GetDistributorList();
    });
    $("#area_l2_id").on("change", function (e) {
        if (position_type == 31)    // 导购员
            GetDistributorList();
    });

   
}



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
            BindEmpCategory();
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

function BindRole() {
    $("#role_id").bindSelect({
        text: "name",
        url: "/SystemManage/MsRole/GetSelectJson",
        firstText: "--请选择所属角色--",
        search: true,
    });
}

function BindDepartment() {
    $("#dept_id").empty();
    $.ajax({
        url: "/OrganizationManage/Department/GetDeptAddrList?id=" + cur_company,
        dataType: "json",
        async: false,
        success: function (data) {
            var optionStr = "<option value='0' grade='1'>--无所属部门--</option>";
            $.each(data, function (i) {
                optionStr += "<option value='" + data[i]["id"] + "' grade='"
                    + data[i]["grade_category"] + "' addr='" + data[i]["addr_str"] + "'>" + data[i]["name"] + "</option>";
            });
            $("#dept_id").append(optionStr);
            $("#dept_id").select2({
                minimumResultsForSearch: 0, tags: false
            });
            BindPosition();
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

var curCategory = "";

function BindEmpCategory() {
    var category = $("#company_id").find('option:selected').attr("category");
    if (category == curCategory)
        return;
    curCategory = category;
    $("#emp_category").empty();
    if (category == "分公司") {
        $("#emp_category").append("<option value='实习生'>实习生</option>"
            + "<option value='员工'>员工</option>"
            + "<option value='劳务工'>劳务工</option>");
    }
    else {
        $("#emp_category").append("<option value='实习生'>实习生</option>"
            + "<option value='职员'>职员</option>"
            + "<option value='劳务工'>劳务工</option>");
    }
}

function BindPosition() {
    var curDepartment = $("#dept_id").val();
    if (curDepartment > 0) {
        $("#position_id").empty();
        $("#position_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Position/GetIdNameList",
            param: { dept_id: curDepartment, company_id: cur_company, },
            firstText: "--请选择职位--",
        });
        var addr = $("#dept_id").find('option:selected').attr("addr");
        $("#work_addr").val(addr);
    } else if (curDepartment == 0) { //无所属部门时
        $("#position_id").empty();
        $("#position_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Position/GetIdNameList",
            param: { company_id: cur_company, dept_id: 0 },
            firstText: "--请选择职位--",
        });
        var addr = $("#dept_id").find('option:selected').attr("addr");
        $("#work_addr").val(addr);
    }
}

//经理片区
function GetAreaL1List() {
    $("#area_l1_id").empty();
    $("#area_l1_id").empty();
    if (cur_company > 0) {
        $("#area_l1_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Area/GetManagerAreaIdNameList",
            param: { company_id: cur_company },
            firstText: "--请选择经理片区--",
        });
    }
}

//业务片区
function GetAreaL2List() {
    var areaL1Id = $("#area_l1_id").val();
    $("#area_l2_id").empty();
    if (areaL1Id > 0) {
        $("#area_l2_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Area/GetSalesAreaIdNameList",
            param: { id: areaL1Id },
            firstText: "--请选择业务片区--",
        });
    }
}

//经销商
// TODO 只选经理片区时，跨业务片区情况的处理
function GetDistributorList() {
    $("#distributor_id").empty();
    $("#distributor_id").bindSelect({
        text: "name",
        url: "/DistributorManage/DistributorManage/GetIdNameList",
        param: { company_id: cur_company, area_l1_id: $("#area_l1_id").val(), area_l2_id: $("#area_l2_id").val(), },
        firstText: "--请选择经销商--",
    });
}

// 1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门经理；6-部门主管；7-行政普通员工；11-培训经理；12-培训师；21-业务经理；22-业务员；31-导购员
function GetGradeList() {
    var position_id = $("#position_id").val();
    if (position_id < 1)
        return;
    $("#grade").empty();
    $.ajax({
        url: "/OrganizationManage/Position/GetGradeList",
        data: { position_id: position_id, grade_category: grade_category },
        dataType: "json",
        async: false,
        success: function (data) {
            position_type = data.positionInfo.position_type;
            $("#grade").bindSelectLocal({
                data: data.gradeList,
                id: "name",
                text: "name",
            });
            if (position_type == 31) //导购
            {
                $("#distributorWrap").show();
                $("#areaL1Wrap").show();
                $("#areaL2Wrap").show();
                $("#guideSpecificWrap").show();

                $("#role_id").val(4).trigger("change");
                //导购角色
                $("#role_id").attr("disabled", true);
                $("#introducer_name").val("");
                GetAreaL1List();
            } else if (position_type == 22) {//业务员
                $("#distributorWrap").hide();
                $("#areaL1Wrap").show();
                $("#areaL2Wrap").show();
                $("#role_id").val("").trigger("change");
                $("#role_id").attr("disabled", false);
                $("#guideSpecificWrap").hide();
                GetAreaL1List();
            } else if (position_type == 21) {//业务经理
                $("#distributorWrap").hide();
                $("#areaL2Wrap").hide();
                $("#areaL1Wrap").show();
                $("#role_id").val("").trigger("change");
                $("#role_id").attr("disabled", false);
                $("#guideSpecificWrap").hide();
                GetAreaL1List()
            }
            else {
                $("#role_id").val("").trigger("change");
                $("#role_id").attr("disabled", false);
                $("#guideSpecificWrap").hide();
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

var supervisor_id = "";
var introducer_id = "";

function CheckNameExist(obj) {
    var name = $(obj).val().trim();
    if (!name || name == "")
        return;
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetEmpByName",
        data: { name: name },
        dataType: "json",
        async: false,
        success: function (data) {
            if (!data || !data.id || data.id == "") {
                if ($(obj).attr('id') == "supervisor_name")
                    supervisor_id = "";
                else
                    introducer_id = "";
                $.modalAlert("系统中不存在“" + name + "”，请确认", "warning");
            }
            else {
                if ($(obj).attr('id') == "supervisor_name")
                    supervisor_id = data.id;
                else
                    introducer_id = data.id;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

function submitForm() {
     
    if (!$("#form1").formValid())
        return false;
    var data = $("#form1").formSerialize();
    
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }   
    //data["emp_category"] = $("#emp_category1").val();
    data["id"] = id;
    $.submitForm({
        url: "/HumanResource/EmployeeManage/EditJobInfo?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/EmployeeManage/Index?id=" + id;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
