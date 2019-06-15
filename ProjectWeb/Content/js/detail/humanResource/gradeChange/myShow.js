//我的晋升降级详情
var id = $.request("id");
var employee_id;
var organization_id;
var department_id;
var position_id;
var job_info_id;
var account;
var gradechange_id;
var approve_result;
//调岗类型
var adjust_type;
var area_dept_type;
$(function () {
    var org_id = 1;
    var dept_id = 0;
    var area_id = 0;
    $.ajax({
        url: "/HumanResource/GradeChange/ShowInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            //area_dept_type = data.deptInfo.type;
            approve_result = data.gradechangeInfo.approve_result;
            job_info_id = data.gradechangeInfo.employee_id;
            gradechange_id = data.gradechangeInfo.id;
            $("#job_number").text(data.gradechangeInfo.job_number);
            $("#name").text(data.gradechangeInfo.name);
            $("#gender").text(data.gradechangeInfo.gender);
            $("#education").text(data.gradechangeInfo.education);
            $("#entry_time").text(data.gradechangeInfo.entry_time);
            if (area_dept_type == 0) {//type = 0 部门， type =1 区域 
                $("#department_name").text(data.gradechangeInfo.department_name);
                $("#department_name_new").text(data.gradechangeInfo.department_name_new);
            } else {
                $("#area_name").text(data.gradechangeInfo.department_name);
                $("#area_name_new").text(data.gradechangeInfo.department_name_new);
            }
            $("#position_name").text(data.gradechangeInfo.position_name);
            $("#position_name_new").text(data.gradechangeInfo.position_name_new);
            $("#orig_grade").text(data.gradechangeInfo.orig_grade);
            $("#new_grade").text(data.gradechangeInfo.new_grade);
           
            $("#summary").val(data.gradechangeInfo.summary);
            $("#plan").val(data.gradechangeInfo.plan);
            $("#expect").val(data.gradechangeInfo.expect);
            $("#adjust_time").text(data.gradechangeInfo.adjust_time);

            if (approve_result == -3||approve_result == 3) {
                $("#table_dept").css("display", "block");
                $("#table_branch").css("display", "block");
                $("#table_boss").css("display", "block");
                $("#check_grade").text(data.approveInfo1.check_grade);
                $("input:radio[name=opinion][value=" + data.approveInfo1.opinion + "]").attr("checked", true);
                $("input:radio[name=opinion2][value=" + data.approveInfo2.opinion + "]").attr("checked", true);
                $("input:radio[name=opinion3][value=" + data.approveInfo3.opinion + "]").attr("checked", true);
                $("#adjust_time").text(data.approveInfo1.adjust_time);
                $("#adjust_time2").text(data.approveInfo1.adjust_time);
                $("#check_time").text(data.approveInfo1.check_time);
                $("#charge_opinion").text(data.approveInfo1.charge_opinion);
                $("#charge_opinion2").text(data.approveInfo2.charge_opinion);
                $("#charge_opinion3").text(data.approveInfo3.charge_opinion);
                $("#approve_name").text(data.approveInfo1.name);
                $("#approve_time").text(data.approveInfo1.approve_time);
                $("#approve_name2").text(data.approveInfo2.name);
                $("#approve_time2").text(data.approveInfo2.approve_time);
                $("#approve_name3").text(data.approveInfo3.name);
                $("#approve_time3").text(data.approveInfo3.approve_time);
            } else if (approve_result == 2 || approve_result == -2) {
                $("#table_dept").css("display", "block");
                $("#table_branch").css("display", "block");
                $("#check_grade").text(data.approveInfo1.check_grade);
                $("input:radio[name=opinion][value=" + data.approveInfo1.opinion + "]").attr("checked", true);
                $("input:radio[name=opinion2][value=" + data.approveInfo2.opinion + "]").attr("checked", true);
                $("#check_time").text(data.approveInfo1.check_time);
                $("#charge_opinion").text(data.approveInfo1.charge_opinion);
                $("#charge_opinion2").text(data.approveInfo2.charge_opinion);
                $("#approve_name").text(data.approveInfo1.name);
                $("#approve_time").text(data.approveInfo1.approve_time);
                $("#approve_name2").text(data.approveInfo2.name);
                $("#approve_time2").text(data.approveInfo2.approve_time);
            } else if (approve_result == -1 || approve_result == 1) {
                $("#table_dept").css("display", "block");
                $("#check_grade").text(data.approveInfo1.check_grade);
                $("input:radio[name=opinion][value=" + data.approveInfo1.opinion + "]").attr("checked", true);
                $("#adjust_time").text(data.approveInfo1.adjust_time);
                $("#adjust_time2").text(data.approveInfo1.adjust_time);
                $("#check_time").text(data.approveInfo1.check_time);
                $("#charge_opinion").text(data.approveInfo1.charge_opinion);
                $("#approve_name").text(data.approveInfo1.name);
                $("#approve_time").text(data.approveInfo1.approve_time);
            }
           




            //switch (approve_result) {
            //    case -3:
            //    case 3:
            //        console.log(data.approveInfo3);
            //    case -2:
            //    case 2:
            //        console.log(data.approveInfo2);
            //    case -1:
            //    case 1:
            //        console.log(data.approveInfo1);
            //    default:
            //        console.log(data.gradechangeInfo);
            //        break;
            //}
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})
function updateForm() {
    window.location.href = "/HumanResource/GradeChange/CareerChangeEdit?id=" + id;
}

function backForm() {
    window.location.href = "/HumanResource/GradeChange/Index";
}

function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var data = {};
    var gradechangeInfo = {};
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    gradechangeInfo["adjust_time"] = $("#adjust_time2").val();
    gradechangeInfo["id"] = gradechange_id;
    data["gradechangeInfo"] = gradechangeInfo;
    $.submitForm({
        url: "/HumanResource/GradeChange/Check?date=" + new Date(),
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
