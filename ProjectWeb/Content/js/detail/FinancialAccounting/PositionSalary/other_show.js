var id = $.request("id");
var approve_result;
$(function () {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetSeniority?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.getInfo);
            $("#form1").formSerializeShow(data.otherInfo);

            if (data.getInfo.is_template == 0) {
                $("#company_category").text("事业部");
            } else {
                $("#company_category").text("分公司");
            }

            if (data.otherInfo.reset_all == true) {
                document.getElementById("other_setting").checked = true;
            }
            else if (data.otherInfo.reset_all == false) {
                document.getElementById("other_setting").checked = false;
            }

            $("#intern_ratio_salary").text(ToThousandsStr(data.otherInfo.intern_ratio_salary));

            if (data.otherInfo.intern_salary_type == 2) {
                $("#intern_ratio_salary").css('display', 'none');
                $("#intern1").css('display', 'none');
                $("#intern_fix_salary").css('display', '');
                $("#intern_fix_salary").text(ToThousandsStr(data.otherInfo.intern_fix_salary));
                $("#intern2").css('display', '');
            } 

            //if (data.getInfo.effect_date == '' && data.getInfo.effect_now == true) {
            //    $("#effect_date").text("立即生效");
            //}
            var date = data.getInfo.effect_date.substring(0, 7);
            $("#effect_date").text(date);

            $.each(data.infoList, function (index, item) {
                var tr;
                if (item.year_max != -1) {
                    tr = '<tr><td>' + ToThousandsStr(item.year_min) + "-" + ToThousandsStr(item.year_max) + '</td>' + '<td>' + ToThousandsStr(item.salary) + '</td></tr>';
                } else {
                    tr = '<tr><td>' + ToThousandsStr(item.year_min) + "以上" + '</td>' + '<td>' + ToThousandsStr(item.salary) + '</td></tr>';
                }
                $("#seniorityWage_table").append(tr);
            })
            //审批
            //var positionSalary = data.positionSalary;
            //$("#form1").formSerializeShow(positionSalary);

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});