var id = $.request("id");
$(function () {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetDept?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.infoMain);
            $("#form1").formSerializeShow(data.getInfo);
            $("#form1").formSerializeShow(data.trafficInfo);
            //if (data.getInfo.effect_date == null && data.getInfo.effect_now == true) {
            //    $("#effect_date").text("审批通过后立即生效");
            //}
            var date = data.getInfo.effect_date.substring(0, 7);
            $("#effect_date").text(date);
            $("#traffic_subsidy").text(ToThousandsStr(data.trafficInfo.traffic_subsidy));
            if (data.trafficInfo.reset_all == true) {
                document.getElementById("assessment_setting3").checked = true;
            }
            else if (data.trafficInfo.reset_all == false) {
                document.getElementById("assessment_setting3").checked = false;
            }
            var tr;
            $.each(data.infoList, function (index, item) {
                if (item.grade_category == 1) {
                    item.grade_category_display = "行政管理";
                }
                else if (item.grade_category == 2) {
                    item.grade_category_display = "市场销售";
                }
                else if (item.grade_category == 3) {
                    item.grade_category_display = "终端管理";
                }
                else if (item.grade_category == 4) {
                    item.grade_category_display = "运营商中心";
                }
                if (item.grade_level == 1) {
                    item.grade_level_display = "公司层面";
                }
                else if (item.grade_level == 2) {
                    item.grade_level_display = "部门层面";
                }
                if (!item.kpi_advice || item.kpi_advice == "null")
                    item.kpi_advice = "-";
                tr += '<tr style="display: table;width: 100%;table-layout: fixed;"><td width="10%"><span name="grade_category_display" class="form-control no-border">'
                        + item.grade_category_display + '<span></td>' + '<td width="10%"><span name="grade_level_display" class="form-control no-border">'
                        + item.grade_level_display + '<span></td>' + '<td width="10%"><span name="grade" class="form-control no-border">'
                        + item.grade + '<span></td>' + '<td width="30%"><span name="kpi_advice" class="form-control no-border">'
                        + item.kpi_advice + '<span></td>' + '<td width="15%"><span name="kpi_standard" class="form-control no-border">'
                        + ToThousandsStr(item.kpi_standard) + '<span></td>' + '<td width="25%"><span class="form-control no-border">打分分值/100*标准金额<span></td></tr>';
            })
            $("#deptKpi").append('<tbody style="display: block;height:400px;overflow-y:scroll;">' + tr + '</tbody>');
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

