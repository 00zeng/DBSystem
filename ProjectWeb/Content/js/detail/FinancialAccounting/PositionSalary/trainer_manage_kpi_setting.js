var id = $.request("id");
var company_linkname = decodeURI($.request("company_linkname"));
var name = decodeURI($.request("name"));
var company_id = $.request("company_id");
$("#company_linkname").text(company_linkname);
$("#name").text(name);
$(function () {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetCurrentTrainer?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { company_id: id },
        success: function (data) {
            if(data ==null)
                alert("空");
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
    var data = $("#form1").formSerialize();
    //var info_main = {};//培训经理KPI
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    //主表传值内容
    if ($("#effect_date").val() == '' && $("#effect_now").prop("checked") == false) {
        $.modalAlert("请选择生效时间");
        return;
    }
    data["effect_now"] = $("#effect_now").prop("checked");
    data["company_name"] = company_linkname;
    data["company_id"] = company_id;
    data["position_name"] = name;
    data["position_id"] = id;
    //data["info_main"] = info_main;
    $.submitForm({
        url: "/FinancialAccounting/PositionSalary/SetTrainerManager?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/PositionSalary/Index?id=" + id;

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
