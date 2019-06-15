//获取id
var myCompanyInfo = top.clients.loginInfo.companyInfo;
var companyList = {};    // 上级机构列表
var guid = "";
$(function () {
    if (myCompanyInfo.category == "分公司") {
        $("#category").append($("<option></option>").val("分公司").html("分公司"));
        $("#company_id").append($("<option></option>").val(myCompanyInfo.id).html(myCompanyInfo.name));
    }
    else {
        $("#category").append($("<option></option>").val("事业部").html("事业部"));
        $("#category").append($("<option></option>").val("分公司").html("分公司"));
        companyList["事业部"] = [{ id: myCompanyInfo.id, name: myCompanyInfo.name }];
        $("#company_id").append($("<option></option>").val(myCompanyInfo.id).html(myCompanyInfo.name));
        $("#category").on("change", function () {
            BindCompany();
        })
    }
    GetGUID();
})
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
//立即生效  js
function Effective(checkbox) {
    if (checkbox.checked == true) {
        $("#effect_date").val('');
        $("#effect_date").attr("disabled", true);
    } else {
        $("#effect_date").removeAttr("disabled");
    }
}
function addRow(trObj) {
    var Tr = ' <tr><td><div class="input-group"><input class="form-control text-center" disabled><span class="input-group-addon">-</span><input class="form-control text-center" onchange="setShipping()"></div></td>'
        + '<td><div class="input-group" ><input class="form-control text-center"></div></td><td><div class="row"><i onclick="addRow(this)" class="fa fa-plus-circle fa-lg  text-blue "></i>&nbsp;<i onclick="deleteRow(this)" class="fa fa-minus-circle fa-lg   text-red"></i></div></td> </tr>';
    $(trObj).parents('tr').after(Tr);
    index = $(trObj).closest('tr')[0].rowIndex;
    setShipping();
    emptyNum();

}
function deleteRow(e) {
    $(e).parents('tr').remove();
    setShipping();
}
function setShipping() {
    for (var i = 2; i < $("#shipping_tr tr").length; i++) {
        if (i != $("#shipping_tr tr").length - 1) {
            if (parseInt($('#shipping_tr tr:eq(' + i + ') td:eq(0) input:eq(0)').val()) >= parseInt($('#shipping_tr tr:eq(' + i + ') td:eq(0) input:eq(1)').val())) {
                $.modalAlert("该销量值应大于" + $('#shipping_tr tr:eq(' + i + ') td:eq(0) input:eq(0)').val() + "！");
                $('#shipping_tr tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
        var max = $('#shipping_tr tr:eq(' + (i - 1) + ') td:eq(0) input:eq(1)').val();
        if (max != '') {
            $('#shipping_tr tr:eq(' + i + ') td:eq(0) input:eq(0)').val(parseInt(max) + 1);
            if (parseInt($('#shipping_tr tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(0)').val()) >= parseInt($('#shipping_tr tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(1)').val())) {
                $('#shipping_tr tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
    }
}
function emptyNum() {
    for (var j = index + 2; j < $("#shipping_tr tr").length; j++) {
        $('#shipping_tr tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
        $('#shipping_tr tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("");
        if (j == $("#shipping_tr tr").length - 1) {
            $('#shipping_tr tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
            $('#shipping_tr tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("以上");
        }
    }
}
function submitForm() {
    var data = $("#form1").formSerialize();
    var importList = [];
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    for (var i = 1; i < $("#shipping_tr tr").length; i++) {
        var $count_min;
        var $count_max;
        if (i == 1 ) {
            $count_max = $('#shipping_tr tr:eq(' + i + ') td:eq(0) input:eq(1)').val();
            $count_min = 0;
        } else if (i == $("#shipping_tr tr").length - 1) {
            $count_min = $('#shipping_tr tr:eq(' + i + ') td:eq(0) input').val();
            $count_max = -1;
        } else {
            $count_min = $('#shipping_tr tr:eq(' + i + ') td:eq(0) input:eq(0)').val();
            $count_max = $('#shipping_tr tr:eq(' + i + ') td:eq(0) input:eq(1)').val();
        }
        var $shipping_fee = $('#shipping_tr tr:eq(' + i + ') td:eq(1) input').val();
        var json = { "main_id": guid, "count_min": $count_min, "count_max": $count_max, "shipping_fee": $shipping_fee };
        if ($shipping_fee == 0 && i == $("#shipping_tr tr").length - 1) {
            data["free_count"] = $count_min;
        }
        importList.push(json);
    }
    data["importList"] = importList;
    data["id"] = guid;
    //生效时间 
    if ($("#effect_date").val() == '' && $("#effect_now").prop("checked") == false) {
        $.modalAlert("请选择生效时间");
        return;
    }
    data["effect_now"] = $("#effect_now").prop("checked");
    $.submitForm({
        url: "/DistributorManage/ShippingTemplate/Import?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/DistributorManage/ShippingTemplate/Show";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}