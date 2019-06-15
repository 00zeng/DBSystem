var id = $.request("id");
var positionSalary;
$(function () {
    $("#form1").queryFormValidate();
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
//获取岗位工资
$(function () {
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetPOSSalary?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            positionSalary = data;
            $("#subsidySalary").text(positionSalary);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});
//补助后工资计算
function getSum() {
    var subsidy = $("#subsidy").val();
    if (subsidy == "" || subsidy.length == 0 || isNaN(subsidy)) {
        $("#subsidySalary").text(ToThousandsStr(positionSalary));
    } else {
        var subsidySalary = Number(positionSalary) + Number(subsidy);
        $("#subsidySalary").text(ToThousandsStr(subsidySalary));
    }
}
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
    data["effect_now"] = $("#effect_now").prop("checked");
    data["emp_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/EmployeeSalary/AddSubsidy?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/EmployeeSalary/Index" ;

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}