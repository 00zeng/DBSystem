var id = $.request("id");
$(function () {
    setInterval(function () { $('#currentTime').text((new Date()).pattern("yyyy-MM-dd HH:mm:ss")) }, 1000);
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetGuide?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.infoMain);
            $("#form1").formSerializeShow(data.getInfo);
            if (data.getInfo.effect_date == null && data.getInfo.effect_now == true) {
                $("#effect_date").text("审批通过后立即生效");
            }
            var tr;
            $.each(data.infoList, function (index, item) {
                switch (item.category) {
                    case 1:
                        tr = '<tr><td>' + item.level + '</td>' + '<td>' + item.amount + '</td></tr>';
                        $("#baseStarlevel_table").append(tr);
                        break;
                    case 2:
                        if (item.target_max == -1) {
                            item.target_max = "以上";
                        }
                        tr = '<tr><td><div class="input-group"><input class="form-control text-center" value="' + item.target_min + '" disabled>'
                           + '<span class="input-group-addon">-</span><input class="form-control text-center" value="' + item.target_max + '" disabled></div></td>'
                           + '<td><span class="form-control text-center no-border">' + item.amount + '</td></tr>';
                        $("#float_salary_tr").append(tr);
                        break;
                    case 3:
                        if (item.target_max == -1) {
                            item.target_max = "以上";
                        }
                        tr = '<tr><td><div class="input-group"><input class="form-control text-center" value="' + item.target_min + '" disabled>'
                           + '<span class="input-group-addon">-</span><input class="form-control text-center" value="' + item.target_max + '" disabled></div></td>'
                           + '<td><span class="form-control text-center no-border">' + item.amount + '</td></tr>';
                        $("#sales_volume_tr").append(tr);
                        break;
                    case 4:
                        tr = '<tr><td>' + item.level + '</td>' + '<td>' + item.amount + '</td></tr>';
                        $("#yearStarlevel_table").append(tr);
                        break;
                    default: break;
                }
            })
            //查看审批
            $("#creator_position_name").text(data.creator_position_name);
            $("#approve_name").text(top.clients.loginInfo.empName);
            $("#approve_position_name").text(top.clients.loginInfo.positionInfo.name);

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


function submitForm() {
    var data = {};
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["status"] = $("input[name='status']:checked").val();
    data["approve_note"] = $("#approve_note").val();
    data["salary_position_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/PositionSalary/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/PositionSalary/ApproveIndex";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}