var id = $.request("id");
$(function () {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetTrainer?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.infoMain);
            $("#image_efficiency_advice").text(ToThousandsStr(data.infoMain.image_efficiency_advice));
            $("#image_efficiency_standard").text(ToThousandsStr(data.infoMain.image_efficiency_standard));
            $("#image_fine_advice").text(ToThousandsStr(data.infoMain.image_fine_advice));
            $("#image_fine_number").text(ToThousandsStr(data.infoMain.image_fine_number));
            $("#image_fine_standard").text(ToThousandsStr(data.infoMain.image_fine_standard));
            $("#manager_scoring_advice").text(ToThousandsStr(data.infoMain.manager_scoring_advice));
            $("#manager_scoring_standard").text(ToThousandsStr(data.infoMain.manager_scoring_standard));
            $("#product_expensive_advice").text(ToThousandsStr(data.infoMain.product_expensive_advice));
            $("#product_expensive_assess").text(ToThousandsStr(data.infoMain.product_expensive_assess));
            $("#product_expensive_standard").text(ToThousandsStr(data.infoMain.product_expensive_standard));
            $("#product_train_advice").text(ToThousandsStr(data.infoMain.product_train_advice));
            $("#product_train_assess").text(ToThousandsStr(data.infoMain.product_train_assess));
            $("#product_train_standard").text(ToThousandsStr(data.infoMain.product_train_standard));
            $("#shopguide_average_advice").text(ToThousandsStr(data.infoMain.shopguide_average_advice));
            $("#shopguide_average_assess").text(ToThousandsStr(data.infoMain.shopguide_average_assess));
            $("#shopguide_average_standard").text(ToThousandsStr(data.infoMain.shopguide_average_standard));
            $("#shopguide_resign_advice").text(ToThousandsStr(data.infoMain.shopguide_resign_advice));
            $("#shopguide_resign_assess").text(ToThousandsStr(data.infoMain.shopguide_resign_assess));
            $("#shopguide_resign_standard").text(ToThousandsStr(data.infoMain.shopguide_resign_standard));
            $("#snowball_number_advice").text(ToThousandsStr(data.infoMain.snowball_number_advice));
            $("#snowball_number_assess").text(ToThousandsStr(data.infoMain.snowball_number_assess));
            $("#snowball_number_standard").text(ToThousandsStr(data.infoMain.snowball_number_standard));
            $("#snowball_ratio_advice").text(ToThousandsStr(data.infoMain.snowball_ratio_advice));
            $("#snowball_ratio_assess").text(ToThousandsStr(data.infoMain.snowball_ratio_assess));
            $("#snowball_ratio_standard").text(ToThousandsStr(data.infoMain.snowball_ratio_standard));

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
