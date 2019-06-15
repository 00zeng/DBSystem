var id = $.request("id");
var gender;

$(function () {
    ActionBinding();
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        //async:false,
        success: function (data) {
            $("#form1").formSerializeShow(data.empInfo);
            $("#name").text(data.empJobInfo.name);
            $("#work_number").text(data.empJobInfo.work_number);

            CalculateAge($("#birthdate"));
            $("input:radio[name=gender][value=" + data.empInfo.gender + "]").attr("checked", true);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
});
// 绑定标签事件
function ActionBinding() {

    $("#identity_type").on("change", function () {
        var obj = $("#identity");
        //obj.val("");
        if ($("#identity_type").val() == "居民身份证")
            identityCheck = true;
        else {
            $(obj).parents('.has-error').find('div.tooltip').remove();
            $(obj).parents('.has-error').find('i.error').remove();
            $(obj).parents().removeClass('has-error');
            identityCheck = false;
        }
    })
}

function CheckIDCard(obj) {
    if ($(obj).val() == "")
        return;
    var checkFlag = new clsIDCard($(obj).val());
    if (!checkFlag.IsValid(obj)) {
        $(obj).focus();
        return false;
    }
    $("#birthdate").val(checkFlag.GetBirthDate());
    $("#birthday").val(checkFlag.GetBirthDate());
    var sex = checkFlag.GetSex();
    var sexBool = new Boolean(sex);
    $("#gender").val(sexBool);
    $("[name='SexList']").filter("[value='" + sexBool + "']").prop("checked", "checked");
}
function CalculateAge(obj) {
    var birthdate = $(obj).val().trim();
    if (!birthdate || birthdate == "")
        return;
    var curYear = (new Date()).getFullYear();
    var birthYear = (new Date(birthdate)).getFullYear();
    $("#age").val(curYear - birthYear + 1);
}

function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var data = $("#form1").formSerialize();
    
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }   
    data["id"] = id;

    $.submitForm({
        url: "/HumanResource/EmployeeManage/EditPersonalInfo?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") 
                window.location.href = "/HumanResource/EmployeeManage/Index?id=" + id;
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
