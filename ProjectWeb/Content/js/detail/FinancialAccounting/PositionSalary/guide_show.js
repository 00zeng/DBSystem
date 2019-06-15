var id = $.request("id");
$(function () {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetGuide?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.infoMain);
            $("#increase_salary").text(ToThousandsStr(data.infoMain.increase_salary));
            $("#increase_commission").text(ToThousandsStr(data.infoMain.increase_commission));
            $("#standard_commission").text(ToThousandsStr(data.infoMain.standard_commission));
            $("#standard_salary").text(ToThousandsStr(data.infoMain.standard_salary));
            $("#guide_salary_base").text(ToThousandsStr(data.infoMain.guide_salary_base));
            $("#form1").formSerializeShow(data.getInfo);
            //交通补贴
            if (data.trafficInfo.reset_all == true) {
                document.getElementById("assessment_setting3").checked = true;
            }
            else if (data.trafficInfo.reset_all == false) {
                document.getElementById("assessment_setting3").checked = false;
            }
            //年终奖
            if (data.infoMain.reset_all_annualbonus == true) {
                document.getElementById("assessment_setting2").checked = true;
            }
            else if (data.infoMain.reset_all_annualbonus == false) {
                document.getElementById("assessment_setting2").checked = false;
            }
            //底薪
            if (data.infoMain.reset_all_base == true) {
                document.getElementById("assessment_setting").checked = true;
            }
            else if (data.infoMain.reset_all_base == false) {
                document.getElementById("assessment_setting").checked = false;
            }
            
            $("#traffic_subsidy").text(data.trafficInfo.traffic_subsidy);
            //if (data.getInfo.effect_date == null && data.getInfo.effect_now == true) {
            //    $("#effect_date").text("审批通过后立即生效");
            //}
            var date = data.getInfo.effect_date.substring(0, 7);
            $("#effect_date").text(date);
            var date = data.infoMain.increase_effect_time.substring(0, 7);
            $("#increase_effect_time").text(date);
            var BaseValue =data.infoMain.guide_base_type;
                if (BaseValue == 4) {
                    $("#salary_base").css('display', '');
                    $("#guide_salary_base").text(ToThousandsStr(data.infoMain.guide_salary_base));
                } else if (BaseValue == 1) {//达标
                    $("#salary_commission").css('display', '');
                    $("#guide_salary_commission").text(ToThousandsStr(data.infoMain.guide_salary_commission));
                    $("#standard_commission").text(ToThousandsStr(data.infoMain.standard_commission));
                }

            var tr;
            $.each(data.infoList, function (index, item) {
                switch (item.category) {
                    case 1:
                        tr = '<tr><td>' + item.level + '</td>' + '<td>' + ToThousandsStr(item.amount) + '</td></tr>';
                        $("#baseStarlevel_table").append(tr);
                        break;
                    case 2:
                        if (item.target_max == -1) {
                            item.target_max = "以上";
                        }
                        tr = '<tr><td class="col-sm-7"><div class="input-group"><input class="form-control text-center" value="' + ToThousandsStr(item.target_min) + '" disabled>'
                           + '<span class="input-group-addon">-</span><input class="form-control text-center" value="' + ToThousandsStr(item.target_max) + '" disabled></div></td>'
                           + '<td class="col-sm-5"><span class="form-control text-center no-border">' + ToThousandsStr(item.amount) + '</td></tr>';
                        $("#float_salary_tr").append(tr);
                        break;
                    case 3:
                        if (item.target_max == -1) {
                            item.target_max = "以上";
                        }
                        tr = '<tr><td class="col-sm-7"><div class="input-group"><input class="form-control text-center" value="' + ToThousandsStr(item.target_min) + '" disabled>'
                           + '<span class="input-group-addon">-</span><input class="form-control text-center" value="' + ToThousandsStr(item.target_max) + '" disabled></div></td>'
                           + '<td class="col-sm-5"><span class="form-control text-center no-border">' + ToThousandsStr(item.amount) + '</td></tr>';
                        $("#sales_volume_tr").append(tr);
                        break;
                    case 4:
                        tr = '<tr><td >' + item.level + '</td>' + '<td>' + ToThousandsStr(item.amount) + '</td></tr>';
                        $("#yearStarlevel_table").append(tr);
                        break;
                    default: break;
                }
            })
            //查看审批
            $("#creator_position_name").text(data.creator_position_name);
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

