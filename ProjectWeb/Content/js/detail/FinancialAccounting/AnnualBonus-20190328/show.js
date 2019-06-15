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
            $("#amount").text(ToThousandsStr(data.annualbonusInfo.amount));

            var date = data.annualbonusInfo.month.substring(0, 7);
            $("#month").text(date);
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
           
            
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});