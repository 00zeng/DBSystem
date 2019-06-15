var id = $.request("id");
var guid = "";
var position_name = decodeURI($.request("position_name"));
var company_name = decodeURI($.request("company_name"));
var name = decodeURI($.request("name"));
var grade = decodeURI($.request("grade"));
$("#position_name").text(position_name);
$("#company_name").text(company_name);
$("#name").text(name);
$("#grade").text(grade);

$(function () {
    GetGUID();
});

function GetGUID() {
    $.ajax({
        url: "/ClientsData/GetGUID",
        async: false,
        type: "get",
        dataType: "json",
        success: function (data) {
            guid = data.guid;
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}
function addRow() {
    var Tr = ' <tr><td><div class="input-single formValue"><input class="form-control required text-center" type="text" name="detail_name" /></div></td>'
           + ' <td><div class="input-group formValue"> <div class="input-group-btn"> <button type="button" class="btn btn-primary" name="category" onclick="Change(this)" title="切换奖/罚">奖</button></div>'
           + ' <input type="text" class="form-control text-center required number" name="amount"> </div>  </td>'
           + ' <td><input type="text"  class="form-control text-center"  name="note" /> </td>'
           + ' <td> <i onclick="addRow(this)" class="fa fa-plus-circle fa-lg margin text-blue "></i><i onclick="deleteRow(this)" class="fa fa-minus-circle fa-lg margin text-red "></i></td></tr>';
    $("#reward_table tbody").append(Tr);
}
function deleteRow(e) {
    $(e).parents('tr').remove();
}
//奖罚 change
function Change(e) {
    var text = $(e).html();
    if (text == '奖') {
        $(e).html('罚');
        $(e).attr('class', 'btn btn-danger');
    } else {
        $(e).html('奖');
        $(e).attr('class', 'btn btn-primary');
    }
}

function submitForm() {
    // 提交验证
    if (!$("#form1").formValid())
        return false;

    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    var rewardsAddList = [];
    var detailNameList = $("input[name='detail_name']");
    var categoryList = $("button[name='category']");
    var amountList = $("input[name='amount']");
    var noteList = $("input[name='note']");
    var len = detailNameList.length;
    for (var i = 0; i < len; i++) {
        var reward = {};
        reward.main_id  = guid;
        reward.detail_name = $(detailNameList[i]).val();
        reward.category = $(categoryList[i]).val();
        //reward.amount = $(amountList[i]).val();
        reward.note = $(noteList[i]).val();
        if ($(categoryList[i]).html() == '奖') {
            reward.amount = $(amountList[i]).val();
        } else {
            reward.amount =  0-$(amountList[i]).val();
        }

        if ($(detailNameList[i]).val() == '' || $(amountList[i]).val() == '') {
            $.modalAlert("奖罚信息不完整");
            return;
        } else if (isNaN($(amountList[i]).val())) {
            $.modalAlert("请正确输入奖罚金额");
            return;
        }

        rewardsAddList.push(reward);
    }
    data["rewardsAddList"] = rewardsAddList;
    data["emp_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/Rewards/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/Rewards/Index?id=" + id;

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
