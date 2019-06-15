var id = $.request("id");
var gender;
var political;

var account_id;
var role_id;
var position_id;
var job_info_id;
var personal_info_id;

var birthday_type;
var marriage;
$(function () {
    $("#organization_id").bindSelect({
        text: "name",
        url: "/CompanyManage/Organization/GetSelectJson4",
        firstText: "--请选择所属机构--",
    });
    $.ajax({
        url: "/PersonalManage/Info/ShowInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {

            account_id = data.accountInfo.id;
            role_id = data.accountInfo.role_id;
            position_id = data.posInfo.position_id;
            job_info_id = data.posInfo.id;
            personal_info_id = data.employeeInfo.id;

            $("#job_number").text(data.posInfo.job_number);
            $("#name").val(data.employeeInfo.name);
            $("#phone").val(data.employeeInfo.phone);
            $("#wechat").val(data.employeeInfo.wechat);
            $("#emergency_contact").val(data.employeeInfo.emergency_contact);
            $("#emergency_contact_phone").val(data.employeeInfo.emergency_contact_phone);
            $("#idtype").val(data.employeeInfo.idtype);
            $("#identity").val(data.employeeInfo.identity);
            $("input:radio[name=SexList][value=" + data.employeeInfo.gender + "]").attr("checked", true);
            $("#political").val(data.employeeInfo.political);
            $("#birthdate").val(data.employeeInfo.birthdate);
            $("#age").val(data.employeeInfo.age);
            $("#birthday").val(data.employeeInfo.birthday);
            $("#birthday_type").val(data.employeeInfo.birthday_type);
            $("#marriage").val(data.employeeInfo.marriage);
            $("#number_children").val(data.employeeInfo.number_children);
            $("#native").val(data.employeeInfo.native);
            $("#nation").val(data.employeeInfo.nation);
            $("#country").val(data.employeeInfo.country);
            $("#address").val(data.employeeInfo.address);
            $("#idaddress").val(data.employeeInfo.idaddress);
            $("#id_begin").val(data.employeeInfo.id_begin);
            $("#id_end").val(data.employeeInfo.id_end);
            $("#idIssued").val(data.employeeInfo.idIssued);
            $("#health_begin").val(data.employeeInfo.health_begin);
            $("#health_end").val(data.employeeInfo.health_end);
            $("#education").val(data.employeeInfo.education);
            $("#profession").val(data.employeeInfo.profession);
            $("#graduation_school").val(data.employeeInfo.graduation_school);
            $("#graduation_day").val(data.employeeInfo.graduation_day);
            $("#submitter").val(data.employeeInfo.submitter);
            $("#submit_day").val(data.employeeInfo.submit_day);
            $("#note").val(data.employeeInfo.note);
            //银行信息
            $("#bank_type").val(data.employeeInfo.bank_type);
            $("#bank_name").val(data.employeeInfo.bank_name);
            $("#bank_account").val(data.employeeInfo.bank_account);
            //家庭信息
            $("#parents_card").val(data.employeeInfo.parents_card);
            $("#parents_card_number").val(data.employeeInfo.parents_card_number);
            $("#parents_phone").val(data.employeeInfo.parents_phone);
            //职务信息
            $("#position_name").val(data.posInfo.position_name);
            $("#organization_id").val(data.posInfo.organization_id).trigger("change");
            $("#department_id").val(data.posInfo.department_id).trigger("change");
            $("#supervisor_name").val(data.posInfo.supervisor_name);
            $("#emp_category").val(data.posInfo.emp_category);
            $("#salary").val(data.posInfo.salary);
            $("#working_place").val(data.posInfo.working_place);
            $("#taxable_unit").val(data.posInfo.taxable_unit);
            $("input:radio[name=social_security][value=" + data.posInfo.social_security + "]").attr("checked", true);
            $("#entry_time").val(data.posInfo.entry_time);
            $("#working_years").val(data.posInfo.working_years);
            $("#contract_signing_date").val(data.posInfo.contract_signing_date);
            $("#contract_expiration_date").val(data.posInfo.contract_expiration_date);
            $("#selling_point_area").val(data.posInfo.selling_point_area);
            $("#selling_point").val(data.posInfo.selling_point);
            $("#shopping_guide_category").val(data.posInfo.shopping_guide_category);
            $("#salesman").val(data.posInfo.salesman);
            $("#trainer").val(data.posInfo.trainer);
            $("#deposit_leave").val(data.posInfo.deposit_leave);
            $("#emp_status").val(data.posInfo.emp_status);
            $("#rest_time").val(data.posInfo.rest_time);
            $("#resign_type").val(data.posInfo.resign_type);
            $("#content").val(data.posInfo.content);
            $("#report_person").val(data.posInfo.report_person);
            $("#deductible_insurance").val(data.posInfo.deductible_insurance);
            $("#others_insurance").val(data.posInfo.others_insurance);
            $("#deposit_return").val(data.posInfo.deposit_return);
            //系统账户
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
    $("#organization_id").on("change", function () {
        $("#department_id").empty();
        $("#department_id").bindSelect({
            text: "name",
            url: "/CompanyManage/Department/GetSelectJson",
            param: { organization_id: $("#organization_id").val() },
            firstText: "--请选择所属部门--",
        });
    })
});
function backForm() {
    window.location.href = "/PersonalManage/Info/Index";
}



function checkIDCard(obj) {
    if ($(obj).val() == "")
        return;
    var checkFlag = new clsIDCard($(obj).val());
    if (!checkFlag.IsValid(obj)) {
        $(obj).focus();
        return false;
        //$.modalAlert("输入的身份证号码无效,请输入有效的身份证号码！！", "error");
    }
    $("#birthdate").val(checkFlag.GetBirthDate());
    var sex = checkFlag.GetSex();
    var sexBool = new Boolean(sex);
    $("#gender").val(sexBool);
    $("[name='SexList']").filter("[value='" + sexBool + "']").prop("checked", "checked");
}
function submitForm() {
    //if (!$("#form1").formValid())
    //    return false;
    var data = {};
    var empInfo = {};
    var jobInfo = {};
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }


    jobInfo["job_number"] = $("#job_number").text();
    empInfo["name"] = $("#name").val();
    empInfo["phone"] = $("#phone").val();
    empInfo["wechat"] = $("#wechat").val();
    empInfo["emergency_contact"] = $("#emergency_contact").val();
    empInfo["emergency_contact_phone"] = $("#emergency_contact_phone").val();
    empInfo["gender"] = $("#gender").val();
    empInfo["political"] = $("#political").val();
    empInfo["customer_level"] = $("#customer_level").val();
    empInfo["birthdate"] = $("#birthdate").val();
    empInfo["age"] = $("#age").val();
    empInfo["birthday"] = $("#birthday").val();
    empInfo["birthday_type"] = $("#birthday_type").val();
    empInfo["marriage"] = $("#marriage").val();
    empInfo["number_children"] = $("#number_children").val();
    empInfo["native"] = $("#native").val();
    empInfo["nation"] = $("#nation").val();
    empInfo["country"] = $("#country").val();
    empInfo["address"] = $("#address").val();
    empInfo["idtype"] = $("#idtype").val();
    empInfo["identity"] = $("#identity").val();
    empInfo["idaddress"] = $("#idaddress").val();
    empInfo["id_begin"] = $("#id_begin").val();
    empInfo["id_end"] = $("#id_end").val();
    empInfo["idIssued"] = $("#idIssued").val();
    empInfo["health_begin"] = $("#health_begin").val();
    empInfo["health_end"] = $("#health_end").val();
    empInfo["education"] = $("#education").val();
    empInfo["profession"] = $("#profession").val();
    empInfo["graduation_school"] = $("#graduation_school").val();
    empInfo["graduation_day"] = $("#graduation_day").val();
    empInfo["submitter"] = $("#submitter").val();
    empInfo["submit_day"] = $("#submit_day").val();
    empInfo["note"] = $("#note").val();
    //银行信息
    empInfo["bank_type"] = $("#bank_type").val();
    empInfo["bank_name"] = $("#bank_name").val();
    empInfo["bank_account"] = $("#bank_account").val();
    //职位
    //jobInfo["position_id"] = data.posInfo.id;
    jobInfo["position_name"] = $("#position_name").val();
    jobInfo["department_name"] = $("#department_name").val();
    jobInfo["organization_name"] = $("#organization_name").val();
    jobInfo["supervisor_name"] = $("#supervisor_name").val();
    jobInfo["emp_category"] = $("#emp_category").val();
    jobInfo["salary"] = $("#salary").val();
    jobInfo["working_place"] = $("#working_place").val();
    jobInfo["taxable_unit"] = $("#taxable_unit").val();
    jobInfo["social_security"] = $("#social_security").val();
    jobInfo["entry_time"] = $("#entry_time").val();
    jobInfo["working_years"] = $("#working_years").val();
    jobInfo["contract_signing_date"] = $("#contract_signing_date").val();
    jobInfo["contract_expiration_date"] = $("#contract_expiration_date").val();
    jobInfo["selling_point_area"] = $("#selling_point_area").val();
    jobInfo["selling_point"] = $("#selling_point").val();
    jobInfo["shopping_guide_category"] = $("#shopping_guide_category").val();
    jobInfo["salesman"] = $("#salesman").val();
    jobInfo["trainer"] = $("#trainer").val();
    jobInfo["deposit_leave"] = $("#deposit_leave").val();
    jobInfo["rest_time"] = $("#rest_time").val();
    jobInfo["resign_type"] = $("#resign_type").val();
    jobInfo["content"] = $("#content").val();
    jobInfo["report_person"] = $("#report_person").val();
    jobInfo["deductible_insurance"] = $("#deductible_insurance").val();
    jobInfo["others_insurance"] = $("#others_insurance").val();
    jobInfo["deposit_return"] = $("#deposit_return").val();

    jobInfo["role_id"] = role_id;
    jobInfo["position_id"] = position_id;
    jobInfo["account_id"] = account_id;
    jobInfo["id"] = job_info_id;
    jobInfo["info_id"] = personal_info_id;
    empInfo["id"] = personal_info_id;
    empInfo["job_info_id"] = job_info_id;
    empInfo["ms_account_id"] = account_id;
    data["empInfo"] = empInfo;
    data["jobInfo"] = jobInfo;
    $.submitForm({
        url: "/PersonalManage/Info/UpdateInfo?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/PersonalManage/Info/Index?id=" + id;

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
