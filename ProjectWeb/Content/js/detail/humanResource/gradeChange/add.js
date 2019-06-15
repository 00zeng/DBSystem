var id = top.clients.loginInfo.empId;
var cur_company = top.clients.loginInfo.companyInfo.id;
var gender;
//获取数据
$(function () {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data:{id:id},
        success: function (data) {
            
            gender = data.empInfo.gender;
            data.empJobInfo.entry_date = data.empJobInfo.entry_date.substring(0,10);
            $("#form1").formSerializeShow(data.empJobInfo);
            $("#form1").formSerializeShow(data.empInfo);
            if (data.empInfo.gender == 0) {
                $("#gender").text("女");
            } else if (data.empInfo.gender == 1){
                $("#gender").text("男");
            } else {
                $("#gender").text("未指定");

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
});
$(function () {
    BindDepartment();
})
function BindDepartment() {
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
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}


function submitForm() {
    var data = $("#form1").formSerialize();
    data['emp_id'] = id;
    data['entry_date'] = $("#entry_date").html();
    data['gender'] = gender;
    data['education'] = $("#education").html();
    data['dept_name_new'] = $("#dept_id_new option:selected").text();
    data['area_name_new'] = $("#area_id_new option:selected").text();
    
    
    
    $.submitForm({
        url: "/HumanResource/GradeChange/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/GradeChange/Index?id=" + id;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
