var id = $.request("id");
$(function () {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.empJobInfo);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
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
function submitForm() {
    if (!$("#form1").formValid())
        return false;

    var data = $("#form1").formSerialize();
    //生效时间
    if ($("#effect_date").val() == '' && $("#effect_now").prop("checked") == false) {
        $.modalAlert("请选择生效时间");
        return;
    }
    //data["kpi_subsidy_full"] = $("#kpi_subsidy_full").prop("checked");
    data["effect_now"] = $("#effect_now").prop("checked");

    data["emp_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/EmployeeSalary/AddKpi?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/EmployeeSalary/SubsidyIndex?id=" + id;

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}