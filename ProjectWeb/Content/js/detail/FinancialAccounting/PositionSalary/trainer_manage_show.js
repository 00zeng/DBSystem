var id = $.request("id");
var approve_result;
$(function () {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetTrainerManager?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {

            $("#form1").formSerializeShow(data.infoMain);
            $("#average_standard_money").text(ToThousandsStr(data.infoMain.average_standard_money));
            $("#average_standard_number").text(ToThousandsStr(data.infoMain.average_standard_number));
            $("#product_expensive_money").text(ToThousandsStr(data.infoMain.product_expensive_money));
            $("#product_expensive_ratio").text(ToThousandsStr(data.infoMain.product_expensive_ratio));
            $("#resign_standard_money").text(ToThousandsStr(data.infoMain.resign_standard_money));
            $("#resign_standard_ratio").text(ToThousandsStr(data.infoMain.resign_standard_ratio));

            $("#form1").formSerializeShow(data.getInfo);
            $("#form1").formSerializeShow(data.trafficInfo);
            $("#traffic_subsidy").text(ToThousandsStr(data.trafficInfo.traffic_subsidy));

            var date = data.getInfo.effect_date.substring(0, 7);
            $("#effect_date").text(date);

            if (data.trafficInfo.reset_all == true) {
                document.getElementById("assessment_setting").checked = true;
            }
            else if (data.trafficInfo.reset_all == false) {
                document.getElementById("assessment_setting").checked = false;
            }

            $.each(data.approveList, function (index, item) {
                var tr;
                var approve_type;
                if (item.approve_position_name == "财务经理") {
                    approve_type = "财务经理审批";
                } else {
                    approve_type = "总经理审批";
                }
                if (item.status > 0) {
                    tr = '<tr><td>' + approve_type + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                } else {
                    tr = '<tr><td>' + approve_type + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }
                
                $("#approve").append(tr);
            })

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});