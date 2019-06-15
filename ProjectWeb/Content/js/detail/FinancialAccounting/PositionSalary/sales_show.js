var id = $.request("id");
//返利政策
var str1 = "<tr><td><div class='input-group'><input  class='form-control text-center' value='";
var str2 = "'disabled><span class='input-group-addon no-border'><= X < </span><input  class='form-control text-center'value='";
var str3 = "";
var str4 = "";
var str6 = "";
var str8 = "";
var category="";
$(function () {
    $.ajax({
        url: "/FinancialAccounting/PositionSalary/GetSales?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            console.log(data);
            $("#form1").formSerializeShow(data.getInfo);
            $("#form1").formSerializeShow(data.trafficInfo);
            $("#traffic_subsidy").text(ToThousandsStr(data.trafficInfo.traffic_subsidy));
            $("#form1").formSerializeShow(data.infoList[0]);
            //if (data.getinfo.effect_date == '' && data.getinfo.effect_now == true) {
            //    $("#effect_date").text("立即生效");
            //}
            category = data.getInfo.category;
            if (category == 22) {
                $("#category").text("业务员")
            } else if (category == 21) {
                $("#category").text("业务经理")
            }
            var date = data.getInfo.effect_date.substring(0, 7);
            $("#effect_date").text(date);

            var checkNormalVal = data.infoList[0].normal_rebate_mode; //奖励金额
            var checkBuyoutVal = data.infoList[0].buyout_rebate_mode; //奖励金额
            var checkTargetVal = data.infoList[0].target_mode;//返利模式
            //返利政策 改变th
            if (checkTargetVal == 1)
                $("#forTargetMode1").hide();

            if (checkTargetVal == 1) {
                $("span[name='target_mode_change']").html("完成率（%）");

            } else if (checkTargetVal == 2) {
                // 按台数
                $("span[name='target_mode_change']").html("销量（台）");
            } else if (checkTargetVal == 3) {
                // 按零售价
                $("span[name='target_mode_change']").html("零售价（元/台）");
            } else if (checkTargetVal == 5) {
                // 按批发价
                $("span[name='target_mode_change']").html("批发价（元/台）");
            } 

            //奖励金额 改变th
            if (checkNormalVal == 1) {
                $("span[name='rebate_mode_change']").html("（元/台）");
                str4 = "";
                str3 = "";
            } else if (checkNormalVal == 2) {
                $("span[name='rebate_mode_change']").html("（元/台）");
                str4 = "*批发价";
                str3 = "%";
            } else if (checkNormalVal == 3) {
                $("span[name='rebate_mode_change']").html("（元/台）");
                str4 = "*零售价 ";
                str3 = "%";
            } else if (checkNormalVal == 4) {
                $("span[name='rebate_mode_change']").html("（元/月）");
                str4 = "";
                str3 = "";
            }
            $("span[name='rebate_mode_input1']").html(str3);
            $("span[name='rebate_mode_input2']").html(str4);


            //奖励金额 改变th
            if (checkBuyoutVal == 1) {
                $("span[name='rebate_mode_change2']").html("（元/台）");
                str8 = "";
                str6 = "";
            } else if (checkBuyoutVal == 2) {
                $("span[name='rebate_mode_change2']").html("（元/台）");
                str8 = "*批发价";
                str6 = "%";
            } else if (checkBuyoutVal == 3) {
                $("span[name='rebate_mode_change2']").html("（元/台）");
                str8 = "*买断价 ";
                str6 = "%";
            } else if (checkBuyoutVal == 4) {
                $("span[name='rebate_mode_change2']").html("（元/月）");
                str8 = "";
                str6 = "";
            }
            //$("span[name='rebate_mode_input1']").html(str6);
            //$("span[name='rebate_mode_input2']").html(str8);


            var tr;
            $.each(data.subList, function (index, item) {
                if (item.target_max == -1) {
                    tr = str1 + ToThousandsStr(item.target_min) + "'disabled><span class='input-group-addon no-border'><= X &nbsp;&nbsp;</span><input  class='form-control text-center'value='" + '以上' + "' style='visibility: hidden;'></div></td>"
                         + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;'><span>" + item.sale_rebate + "</span><span>" + str3 + "</span>&nbsp;&nbsp;<span>" + str4 + "</span></td>"
                         + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;'><span>" + item.buyout_rebate + "</span><span>" + str6 + "</span>&nbsp;&nbsp;<span>" + str8 + "</span></td>";
                         } else {
                    tr = str1 + ToThousandsStr(item.target_min) + str2 + ToThousandsStr(item.target_max) + "' disabled></div></td>"
                        + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;'><span>" + item.sale_rebate + "</span><span>" + str3 + "</span>&nbsp;&nbsp;<span>" + str4 + "</span></td>"
                        + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;'><span>" + item.buyout_rebate + "</span><span>" + str6 + "</span>&nbsp;&nbsp;<span>" + str8 + "</span></td>";
                }
                $("#rebateMode1").append(tr);
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