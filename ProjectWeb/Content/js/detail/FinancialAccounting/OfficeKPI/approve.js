var id = $.request("id");
var position_name = decodeURI($.request("position_name"));
var company_name = decodeURI($.request("company_name"));
var emp_name = decodeURI($.request("emp_name"));
var grade = decodeURI($.request("grade"));
$("#position_name").text(position_name);
$("#company_name").text(company_name);
$("#name").text(emp_name);
$("#grade").text(grade);
var myPositionInfo = top.clients.loginInfo.positionInfo;
$(function () {
    $.ajax({
        url: "/FinancialAccounting/OfficeKPI/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            console.log(data);
            $("#form1").formSerializeShow(data.ApprovelistInfo);
            $("#form1").formSerializeShow(data.kpiscoreInfo);
            //审批
            var tr = '';
            $.each(data.ApprovelistInfo, function (index, item) {
                if (item.status > 0) {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                } else {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);
            //0-普通职位；1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门/区域经理
            if (myPositionInfo.positionType == 1 || myPositionInfo.positionType == 2) {
                $("#next_approve").html('事业部总经理审批');

            } else if (myPositionInfo.positionType == 3 || myPositionInfo.positionType == 4) {
                $("#next_approve").html('分公司总经理审批');

            } else {
                $("#next_approve").html('财务经理审批');

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
        url: "/FinancialAccounting/OfficeKPI/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/OfficeKPI/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}