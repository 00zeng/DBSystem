var id = $.request("id");
var main_id;
var mainData;
//获取数据
$(function () {
    $.ajax({
        url: "/HumanResource/Resign/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            
            mainData  = data;
            main_id  = data.mainInfo.id;
            $("#date_approve").val(data.mainInfo.request_date);
            data.mainInfo.request_date = data.mainInfo.request_date.substring(0,10);
            switch (data.mainInfo.approve_status) {
                case 0:
                    data.mainInfo.approve_status = '未审核';
                    break;
                case 100:
                    data.mainInfo.approve_status = '通过';
                    break;
                default:
                    data.mainInfo.approve_status = '审批中';
                    break;
            }
            switch (data.mainInfo.type) {
                case 1:
                    data.mainInfo.type = '正常离职';
                    break;
                case 2:
                    data.mainInfo.type = '辞退';
                    break;
                default:
                    data.mainInfo.type = '自离';
                    break;
            }
            switch (data.mainInfo.gender) {
                case 0:
                    data.mainInfo.gender = '女';
                    break;
                case 1:
                    data.mainInfo.gender = '男';
                    break;
                default:
                    data.mainInfo.gender = '未指定';
                    break;
            }
            
            $("#request_time").html(data.mainInfo.request_date);
            $("#form1").formSerializeShow(data.mainInfo);
            
            if(data.approveInfoList.length == 0){
                $("#approve_tb tr:eq(0) th:eq(0)").show();
                $("#approve_tb tr:eq(0) td:eq(0)").show();
            }
            
            var tr = '';
            $.each(data.approveInfoList, function (index,value) {
                var result = '';
                if(value.status>0){
                    result = '通过';
                }else{
                    result = '不通过';
                }
                if(index == 0){
                    tr += ' <tr style="height: 30px">\n' +
                        '                        <th>核准离职时间</th>\n' +
                        '                        <td><span>'+data.mainInfo.date_approve+'</span></td>\n' +
                        '                        <th>审批结果</th>\n' +
                        '                        <td>\n' +
                        '                            <span>'+result+'</span>' +
                        '                        </td>\n' +
                        '                        <th>审批人</th>\n' +
                        '                        <td><span>'+value.approve_name+'</span></td>\n' +
                        '                        <th>审批时间</th>\n' +
                        '                        <td><span>'+value.approve_time+'</span></td>\n' +
                        '                    </tr>';
                }else{
                    tr += ' <tr style="height: 30px">\n' +
                        '                        <th>审批结果</th>\n' +
                        '                        <td>\n' +
                        '                            <span>'+result+'</span>' +
                        '                        </td>\n' +
                        '                        <th>审批人</th>\n' +
                        '                        <td><span>'+value.approve_name+'</span></td>\n' +
                        '                        <th>审批时间</th>\n' +
                        '                        <td><span>'+value.approve_time+'</span></td>\n' +
                        '                        <th></th>\n' +
                        '                        <td></td>\n' +
                        '                    </tr>';
                }
            })
            $("#approve_tb tr:last").before(tr);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
});





//提交内容
function submitForm() {
    var data = {};
    var approveInfo = {};
    var mainInfo = {};

    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }

    // approveInfo["job_number"] = $("#job_number").text();
    // approveInfo["name"] = $("#name").text();
    // approveInfo["gender"] = $("#gender").text();
    // approveInfo["education"] = $("#education").text();
    // approveInfo["entry_time"] = $("#entry_time").text();
    //
    // approveInfo["department_id"] = $("#department_id").text();
    // approveInfo["area"] = $("#area").text();
    // approveInfo["position_id"] = $("#position_id").text();
    // approveInfo["leave_time"] = $("#leave_time").text();
    // approveInfo["dept_charge_name"] = $("#dept_charge_name").text();
    //
    // approveInfo["content"] = $("#content").val();
    // approveInfo["later_arrangement"] = $("#later_arrangement").val();
    // approveInfo["later_opinion"] = $("#later_opinion").val();


    approveInfo["status"] = $("input[name='opinion']:checked").val();
    approveInfo["main_id"] =id;
    data["approveInfo"] = approveInfo;

    if(mainData.approveInfoList.length == 0){
        mainInfo['date_approve'] = $("#date_approve").val();
        mainInfo["id"] =main_id;
        data['mainInfo'] = mainInfo;
    }
    
    

    $.submitForm({
        url: "/HumanResource/Resign/Approve?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/Resign/Index?id=" + id;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

