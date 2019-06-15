var id = $.request("id");
var job_info_id;
var main_id;
var myCompanyInfo = top.clients.loginInfo;

//获取数据
$(function () {
    $.ajax({
        url: "/HumanResource/Resign/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            main_id  = data.mainInfo.id;
            if(data.mainInfo.date_approve != data.mainInfo.request_date && data.approveInfoList.length == 1){
                $("#btnConfirm").show();
            }
            if(data.approveInfoList.length == 0 && myCompanyInfo.empId == data.jobInfo.id){
                $("#btnDel").show();
            }
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
            $("#date_approve").val(data.mainInfo.request_date);

            
            
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
            $("#approve_tb").append(tr);
            
           

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
});

function confirm() {
    var data = {};
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["id"] = id;
    $.ajax({
        url: "/HumanResource/Resign/Confirm",
        type: "post",
        data: data,
        success: function (data) {
            top.layer.msg("确认成功");
            window.location.href = "javascript: history.go(-1)";

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

//撤回
function delForm() {
    top.layer.confirm("确认要撤回? ", function (index) {
       
        var data = {};
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["id"] = id;
        $.ajax({
            url: "/HumanResource/Resign/Delete?id=" + id,
            type: "post",
            data: data,
            success: function (data) {
                top.layer.msg("撤回成功");
                window.location.href = "javascript: history.go(-1)";

            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}