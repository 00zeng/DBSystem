var id = $.request("id");
var position_name = decodeURI($.request("position_name"));
var company_name = decodeURI($.request("company_name"));
var name = decodeURI($.request("name"));
var grade = decodeURI($.request("grade"));
var month = decodeURI($.request("month"));
$("#position_name").text(position_name);
$("#company_name").text(company_name);
$("#name").text(name);
$("#grade").text(grade);
$("#month").text(month);

$(function () {
    $.ajax({
        url: "/FinancialAccounting/Rewards/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.rewardInfo);
            //奖罚列表
            var date = data.rewardInfo.month.substring(0, 7);
            $("#month").text(date);
            $.each(data.rewardAddList, function (index, item) {
                if (item.amount >=0 ) {
                    btnClass = "primary";
                    rewards = "奖";
                    amount = item.amount;
                } else {
                    btnClass = "danger";
                    rewards = "罚";
                    amount = 0-item.amount;
                }
                var tr;
                tr = ' <tr><td><span class="form-control no-border">' + item.detail_name + '</span></td>'
                   + ' <td><div class="input-group"> <div class="input-group-btn">'
                   + ' <button type="button" class="btn btn-' + btnClass + '" >' + ToThousandsStr(rewards) + '</button></div><span  class="form-control text-center  "> ' + amount
                   + ' </span></div></td>'
                   + ' <td><span class="form-control no-border">' + item.note + '</span></td>'
                $("#reward_table tbody").append(tr);
            })
            //查看审批
            var tr = '';
            $.each(data.approveList, function (index, item) {
                if (item.approve_note == null || item.approve_note == '') {
                    approve_note_null = "--";
                } else
                    approve_note_null = item.approve_note;
                if (item.status == 1 || item.status == -1)
                    approve_grade = ("一级审批");
                else if (item.status == 2 || item.status == -2)
                    approve_grade = ("二级审批");
                else if (item.status == 3 || item.status == -3)
                    approve_grade = ("三级审批");
                else if (item.status == 4 || item.status == -4)
                    approve_grade = ("四级审批");
                else if (item.status == 100 || item.status == -100)
                    approve_grade = ("终审");
                if (item.status > 0) {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                } else {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});