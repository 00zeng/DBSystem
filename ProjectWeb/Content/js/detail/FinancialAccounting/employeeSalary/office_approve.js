var id = $.request("id");
var company_name = decodeURI($.request("company_name"));
$("#company_name").text(company_name);
var position_name = decodeURI($.request("position_name"));
$("#position_name").text(position_name);
var name = decodeURI($.request("name"));
$("#name").text(name);
var entry_date = $.request("entry_date");
$("#entry_date").text(entry_date.substring(0,10));
var grade = decodeURI($.request("grade"));
$("#grade").text(grade);
$(function () {
    setInterval(function () { $('#currentTime').text((new Date()).pattern("yyyy-MM-dd HH:mm:ss")) }, 1000);
    $.ajax({
        url: "/FinancialAccounting/EmployeeSalary/GetSalaryInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.generalInfo);
            $("#form1").formSerializeShow(data.salaryInfo);
            if (data.salaryInfo.effect_now == true) {
                $("#effect_date").text("审批通过后立即生效");
            }
            if (data.generalInfo.intern_salary_type == 1) {
                $("span[name=ratio_hidden]").css('display', '');
                $("span[name=salary_hidden]").css('display', 'none');
            }
            else {
                $("span[name=salary_hidden]").css('display', '');
                $("span[name=ratio_hidden]").css('display', 'none');
            }

           

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
    data["main_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/EmployeeSalary/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/EmployeeSalary/ApproveIndex";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
