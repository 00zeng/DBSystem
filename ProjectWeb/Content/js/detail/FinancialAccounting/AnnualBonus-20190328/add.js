var id = $.request("id");
var position_name = decodeURI($.request("position_name"));
var name = decodeURI($.request("name"));
var grade = decodeURI($.request("grade"));
$("#position_name").text(position_name);
$("#name").text(name);
$("#grade").text(grade);

function submitForm() {
    if (!$("#form1").formValid())
        return false;  
    var data = $("#form1").formSerialize();
    if ($("#month").val() == '' && $("#month").prop("checked") == false) {
        $.modalAlert("请选择发放月份");
        return;
    }
    data["emp_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/AnnualBonus/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/AnnualBonus/Index";

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}