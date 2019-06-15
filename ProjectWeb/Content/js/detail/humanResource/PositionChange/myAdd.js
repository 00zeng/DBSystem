var empId=top.clients.loginInfo.empId;
$(function () {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: empId },
        //async:false,
        success: function (data) {
            
            $("#form1").formSerializeShow(data.empJobInfo);
            $("#form1").formSerializeShow(data.empInfo);
            if (data.empInfo.gender == 0) {
                $("#gender").text("女");
            } else if (data.empInfo.gender == 1) {
                $("#gender").text("男");
            } else
                $("#gender").text("未指定");
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
})

$("select#type").on("change", function () {
    if ($(this).val() == 1) {//转区域

    } else if ($(this).val() == 2) {//转部门

    } else if ($(this).val() == 3) {//转机构

    }
})
        
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
            $("#company_id_new").append(optionStr);
            $("#company_id_new").select2({
                minimumResultsForSearch: 0, tags: false
            });
            BindDepartment();
            $("#company_id_new").on("change", function (e) {
                $("#select2-" + $("#company_id_new").attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
                BindDepartment();
            });
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

function BindDepartment() {
    cur_company = $("#company_id_new").val();
    $("#dept_id_new").empty();
    $.ajax({
        url: "/OrganizationManage/Department/GetDeptAddrList?id=" + cur_company,
        dataType: "json",
        async: false,
        success: function (data) {
            var optionStr = "";
            $.each(data, function (i) {
                optionStr += "<option value='" + data[i]["id"] + "' grade='"
                    + data[i]["grade_category"] + "' addr='" + data[i]["addr_str"] + "'>" + data[i]["name"] + "</option>";
            });
            $("#dept_id_new").append(optionStr);
            $("#dept_id_new").select2({
                minimumResultsForSearch: 0, tags: false
            });
            BindPosition();
            $("#dept_id_new").on("change", function (e) {
                $("#select2-" + $("#dept_id_new").attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
                BindPosition();
            });
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

//function BindArea() {
//    cur_company = $("#company_id_new").val();
//    $("#area_id_new").empty();
//    $.ajax({
//        url: "/OrganizationManage/Area/GetIdNameList?id=" + cur_company,
//        dataType: "json",
//        async: false,
//        success: function (data) {
//            var optionStr = "";
//            $.each(data, function (i) {
//                optionStr += "<option value='" + data[i]["id"] + "' grade='"
//                    + data[i]["grade_category"] + "' addr='" + data[i]["addr_str"] + "'>" + data[i]["name"] + "</option>";
//            });
//            $("#area_id_new").append(optionStr);
//            $("#area_id_new").select2({
//                minimumResultsForSearch: 0, tags: false
//            });
//            BindPosition();
//            $("#area_id_new").on("change", function (e) {
//                $("#select2-" + $("#area_id_new").attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
//                BindPosition();
//            });
//        }
//    });
//}

function BindPosition() {
    var curDepartment = $("#dept_id_new").val();
    if (curDepartment > 0) {
        $("#position_id_new").empty();
        $("#position_id_new").bindSelect({
            text: "name",
            url: "/OrganizationManage/Position/GetIdNameList",
            param: { dept_id: curDepartment },
            firstText: "--请选择职位--",
            change: function () {
                GetGradeList();
            }
        });
        var addr = $("#dept_id_new").find('option:selected').attr("addr");
        $("#work_addr").val(addr);
        grade_category = $("#dept_id_new").find('option:selected').attr("grade");
    }
}
function GetGradeList() {
    var position_id = $("#position_id_new").val();
    $("#grade_new").empty();
    $.ajax({
        url: "/OrganizationManage/Position/GetGradeList",
        data: { position_id: position_id, grade_category: grade_category },
        dataType: "json",
        async: false,
        success: function (data) {
            $("#grade_new").bindSelectLocal({
                data: data.gradeList,
                id: "name",
                text: "name",
            });
            if (data.positionInfo.salary_category == 3) //导购
            {
                $("#GuideSpecific").show();
                $("#role_id").val(4).trigger("change");
                ;//导购角色
                $("#role_id").attr("disabled", true);
                $("#introducer_name").val("");
            }
            else {
                $("#role_id").val("").trigger("change");
                ;
                $("#role_id").attr("disabled", false);
                $("#GuideSpecific").hide();
            }

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}
function submitForm() {
    //if (!$("#form1").formValid())
    //    return false;
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["emp_id"] = empId;
    $.submitForm({
        url: "/HumanResource/PositionChange/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/PositionChange/Index" ;

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}