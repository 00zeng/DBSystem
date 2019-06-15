var id = 0;
var origDate;
$(function () {
    $.ajax({
        url: "/SalaryCalculate/PayrollSetting/GetPayrollMonth?date=" + new Date(),
        type: "get",
        dataType: "json",
        success: function (data) {
            if (!!data) {
                id = data.id;
                origDate = (new Date(data.end_date)).pattern("yyyy-MM-dd");
                if (data.company_id > 0) {
                    var msg = "";
                    if (data.approve_status > 0)
                        msg = "本月结算日已设置过（已审批），是否重新设置？";
                    else if (data.approve_status < 0)
                        msg = "本月结算日已设置过（审批不通过），是否重新设置？";
                    else
                        msg = "本月结算日已设置过（未审批），是否重新设置？";
                    $.modalConfirm(msg, function (result) {
                        if (!result)
                            top.layer.closeAll();
                        else {
                            var index = top.layer.getFrameIndex(window.name);
                            top.layer.close(Number(index) + 1); // window.name 为Edit的弹出层，index+1为下一个弹出层
                        }
                        return;
                    });
                }
                $("#end_date_orig").val(origDate);
                $("#note").val(data.note);
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
});
function submitForm() {
    if (!$("#form1").formValid())
        return false;

    var end_date = $("#end_date").val();
    if (!end_date) {
        $.modalAlert("结算日期不能为空！", "error");
        return false;
    } else {
        if (end_date == origDate) {
            $.modalAlert("设置结算日与默认结算日一致！", "error");
            return false;
        }
    }
    var note = $("#note").val().trim();
    var data = { id: id, end_date: end_date, note: note };
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/SalaryCalculate/PayrollSetting/Edit?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                top.layer.closeAll();
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
