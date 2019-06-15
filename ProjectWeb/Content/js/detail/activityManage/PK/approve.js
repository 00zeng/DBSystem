var id = $.request("id");

var sum = 0;

$(function () {
    $.ajax({
        url: "/ActivityManage/PK/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: {id: id},
        success: function (data) {
            $("#form1").formSerialize(data.mainInfo);
            $("#form1").formSerializeShow(data.mainInfo);
            
            $.each(data.empList, function (index, value) {
                if (sum % 2 == 0) {
                    $("div[class='box']:eq(0)").append("<div class='left'>" + value.emp_name + "(" + value.area_name + ")</div>");
                } else {
                    $("div[class='box']:eq(2)").append("<div class='right'>" + value.emp_name + "(" + value.area_name + ")</div>");
                }
                sum++;
            })
            
            
            if(data.mainInfo.win_company !=0){
                $("input[name='win_company']").attr("checked",true);
            }else{
                $("input[name='win_company']").attr("checked",false);
            }

            if(data.mainInfo.win_penalty !=0){
                $("input[name='win_penalty']").attr("checked",true);
            }else{
                $("input[name='win_penalty']").attr("checked",false);
            }

            if(data.mainInfo.lose_penalty !=0){
                $("input[name='lose_penalty']").attr("checked",true);
            }else{
                $("input[name='lose_penalty']").attr("checked",false);
            }
            

            if (data.mainInfo.product_scope == 1) {
                $("#rank_tb tr:eq(1)").show();
            } else if (data.mainInfo.product_scope == 2) {
                $("#rank_tb tr:eq(2)").show();
                $.each(data.rewardList, function (index, value) {
                    $("select[name='product_type']").append("<option>" + value.model + "</option>");
                })
            } else {
                $("#rank_tb tr:eq(3)").show();
                $.each(data.rewardList, function (index, value) {
                    $("select[name='selectProducts']").append("<option>" + value.model + "</option>");
                })
            }

            
            
            var tr = '';
            $.each(data.approveList, function (index, item) {
                if (item.status > 0) {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                } else {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }

            })
            $("#approve tr:eq(1)").after(tr);
            if (data.approveList.length == 1) {
                $("#next_approve").html('总经理审批');
            } else if (data.approveList.length == 2) {
                $("#next_approve").html('财务经理审批');
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
        url: "/ActivityManage/PK/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/ActivityManage/PK/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}