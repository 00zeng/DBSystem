var id = $.request("id");
var position_name = decodeURI($.request("position_name"));
var company_name = decodeURI($.request("company_name"));
var emp_name = decodeURI($.request("emp_name"));
var grade = decodeURI($.request("grade"));
$("#position_name").text(position_name);
$("#company_name").text(company_name);
$("#name").text(emp_name);
$("#grade").text(grade);

$(function () {
    $.ajax({
        url: "/FinancialAccounting/AnnualBonus/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.annualbonusInfo);
            //审批
            var tr = '';
            $.each(data.listApproveInfo, function (index, item) {
                if (item.status > 0) {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                } else {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);
            if (data.annualbonusInfo.company_id_parent == 1) {
                if (data.listApproveInfo.length == 0) {
                    $("#next_approve").html('事业部总经理审批');
                } else if (data.listApproveInfo.length == 1) {
                    $("#next_approve").html('财务经理审批');
                }
            } else {
                if (data.listApproveInfo.length == 0) {
                    $("#next_approve").html('分公司经理审批');
                } else if (data.listApproveInfo.length == 1) {
                    $("#next_approve").html('事业部总经理审批');
                } else if (data.listApproveInfo.length == 2) {
                    $("#next_approve").html('财务经理审批');
                }
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});
function submitForm() {
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["status"] = $("input[name='status']:checked").val();
    data["main_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/AnnualBonus/Approve?date=" + new Date(),
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