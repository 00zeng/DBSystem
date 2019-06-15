var id = $.request("id");
var position_name = decodeURI($.request("position_name"));
var name = decodeURI($.request("name"));
var grade = decodeURI($.request("grade"));
$("#position_name").text(position_name);
$("#name").text(name);
$("#grade").text(grade);

function Month() {
    $.ajax({
        url: "/FinancialAccounting/OfficeKPI/GetOrigInfo",
        type: "get",
        dataType: "json",
        data: { emp_id: id, month: $("#month").val() },
        success: function (data) {
            $("#kpi").val(data.kpi);
            $("#note").val(data.note);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })

}

$(function () {
    $.ajax({
        url: "/FinancialAccounting/OfficeKPI/GetSettingInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.empInfo);

            $("#month").val(data.month.substring(0, 7));
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});

function submitForm() {
    // 提交验证
    if (!$("#form1").formValid())
        return false;
    var data = $("#form1").formSerialize();
    data["emp_id"] = id;
    data["emp_name"] = name;

    if ($("#kpi").val() > 100) {
        $.modalAlert("KPI分数应小于100！");
        return;
    }

    if ($("#month").val() == '' && $("#month").prop("checked") == false) {
        $.modalAlert("请选择考核周期");
        return;
    }

    $.submitForm({
        url: "/FinancialAccounting/OfficeKPI/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/OfficeKPI/EmpIndex?id=" + id;

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}