var id = $.request("id");


$(function () {
    $.ajax({
        url: "/ActivityManage/SalesPerformance/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#approve_position_id").val(data.perfInfo.emp_id);
            $("#creator_name").html(data.perfInfo.creator_name);
            $("#creator_position_name").html(data.perfInfo.creator_position_name);
            $("#create_time").html(data.perfInfo.create_time);
            var tr = '';
            $.each(data.approveList, function (index, item) {
                if (item.status > 0) {
                    tr += '<tr><td>' + item.approve_position_name+'审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                } else {
                    tr += '<tr><td>' + item.approve_position_name+'审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }

            })
            $("#approve tr:eq(1)").after(tr);
            if(data.approveList.length == 1){
                $("#next_approve").html('总经理审批');
            }else if(data.approveList.length == 2){
                $("#next_approve").html('财务经理审批');
            }
            $("#emp_name").html(data.perfInfo.emp_name);
            $("#position_name").html(data.jobInfo.position_name);
            if (data.perfInfo.category == 1) {
                $("#category").html("月度考核");
            } else if (data.perfInfo.category == 2) {
                $("#category").html("销量考核");
            } else {
                $("#category").html("导购人数考核");
            }
            if (data.perfInfo.end_date) {
                $("#date").html(data.perfInfo.start_date.substring(0, 10) + "  " + data.perfInfo.end_date.substring(0, 10));
            } else {
                $("#date").html(data.perfInfo.start_date.substring(0, 10));
            }
            $("#content_" + data.perfInfo.category).show();
            $("input[name='perf_content1'][value=" + data.perfInfo.perf_content + "]").attr("checked", true);
            $("input[name='perf_mode1'][value=" + data.perfInfo.perf_mode + "]").attr("checked", true);
            $("input[name='perf_content2'][value=" + data.perfInfo.perf_content + "]").attr("checked", true);
            $("input[name='perf_product2'][value=" + data.perfInfo.perf_product + "]").attr("checked", true);
            $("#product_model2").html(data.perfInfo.product_model);
            $("#perf_target1").html(data.perfInfo.perf_target);
            $("#perf_target2").html(data.perfInfo.perf_target);
            $("#perf_target3").html(data.perfInfo.perf_target);
            $("input[name='perf_mode2'][value=" + data.perfInfo.perf_mode + "]").attr("checked", true);
            $("input[name='perf_mode3'][value=" + data.perfInfo.perf_mode + "]").attr("checked", true);
            if (data.perfInfo.category == 2) {
                if (data.perfInfo.perf_mode == 2) {
                    $("#reward_first_text2").text('台数');
                    $("#penalty_first_text2").text('台数');
                    $("#reward_second_text2").text('元/台');
                    $("#penalty_second_text2").text('元/台');
                } else {
                    $("#reward_first_text2").text('比例');
                    $("#penalty_first_text2").text('比例');
                    $("#reward_second_text2").text('元');
                    $("#penalty_second_text2").text('元');
                }
                if (data.perfInfo.reward > 0) {
                    $("input[name='reward2']").attr("checked", true);
                    $("#reward2").html(data.perfInfo.reward);
                } else {
                    $("#reward2").html(0);
                }
                if (data.perfInfo.penalty > 0) {
                    $("input[name='penalty2']").attr("checked", true);
                    $("#penalty2").html(data.perfInfo.penalty);
                } else {
                    $("#penalty2").html(0);
                }
            } else if (data.perfInfo.category == 3) {
                if (data.perfInfo.perf_mode == 2) {
                    $("#reward_first_text3").text('人数');
                    $("#penalty_first_text3").text('人数');
                    $("#reward_second_text3").text('元/人');
                    $("#penalty_second_text3").text('元/人');
                } else {
                    $("#reward_first_text3").text('比例');
                    $("#penalty_first_text3").text('比例');
                    $("#reward_second_text3").text('元');
                    $("#penalty_second_text3").text('元');
                }
                if (data.perfInfo.reward > 0) {
                    $("input[name='reward3']").attr("checked", true);
                    $("#reward3").html(data.perfInfo.reward);
                } else {
                    $("#reward3").html(0);
                }
                if (data.perfInfo.penalty > 0) {
                    $("input[name='penalty3']").attr("checked", true);
                    $("#penalty3").html(data.perfInfo.penalty);
                } else {
                    $("#penalty3").html(0);
                }
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
})


function submitForm() {
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["main_id"] = id;
    data["status"] = $("input[name='status']:checked").val();
    data["approve_note"] = $("#approve_note").val();
    $.submitForm({
        url: "/ActivityManage/SalesPerformance/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/ActivityManage/SalesPerformance/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}