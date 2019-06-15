var id = $.request("id");
var category;
var main_id;
var warehouse_main_id;
var guide_name;
$(function () {
    $.ajax({
        url: "/SaleManage/BuyoutInfo/GetBuyoutInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            
            //$("#form1").formSerializeShow(data.mainInfo);
            $("#distributor_name").text(data.mainInfo.distributor_name);
            $("#sales_name").html(data.mainInfo.sales_name);
            guide_name = data.mainInfo.guide_name;
            var guide_id = data.mainInfo.guide_id;
            if (guide_name == null || guide_id == null)
                $("#guide_name").text("无导购员");
            else
                $("#guide_name").text(guide_name);

            category = data.mainInfo.category;
            //买断列表
            if (category == 1) {
                $("#category").text("仓库买断");
                $("#warehouse").show();
                $.each(data.subInfoList, function (index, item) {
                    warehouse_main_id = item.main_id;
                    var tr;
                    tr = '<tr><td>' + item.model + '</td>' + '<td>' + item.color + '</td>' + '<td>' + item.price_buyout + '</td>' + '<td>' + item.buyout_count +  '</td>' + '</td></tr>';
                    $("#warehouse").append(tr);
                })
            } else if (category == 2) {
                $("#category").text("门店买断");
                $("#store").show();
                $.each(data.buyoutInfoList, function (index, item) {
                    main_id = item.main_id;
                    var tr;
                    tr = '<tr><td>' + item.phone_sn + '</td>' + '<td>' + item.model + '</td>' + '<td>' + item.price_buyout + '</td>' + '<td><input class="form-control text-center" tag1="' + item.id + '"/></td></td></tr>';
                    $("#store").append(tr);
                })
            }
            //审批
            var tr = '';
            $.each(data.approveListInfo, function (index, item) {
                if (item.status > 0) {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                } else {
                    tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + item.approve_note + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});
function submitForm() {
    var data = {};
    var buyoutList = [];
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    if (category == 1) {//仓库买断 warehouse
        data["main_id"] = warehouse_main_id;
        data["status"] = 1;
        $.submitForm({
            url: "/SaleManage/BuyoutInfo/StorageApprove?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    window.location.href = "/SaleManage/BuyoutInfo/Index";
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })

    } else if (category == 2) {//门店买断 store
        data["status"] = 1;
        data["main_id"] = main_id;
        var $id = $('#store tr:eq(' + i + ') td:eq(3) input').attr("tag1");
        for (var i = 1; i < $("#store tr").length; i++) {
            var $id = $('#store tr:eq(' + i + ') td:eq(3) input').attr("tag1");
            var $guide_commission = $('#store tr:eq(' + i + ') td:eq(3) input').val();
            var json = { "id": $id, "main_id": main_id, "guide_commission": $guide_commission };
            buyoutList.push(json);
        }
        data["buyoutList"] = buyoutList;
        $.submitForm({
            url: "/SaleManage/BuyoutInfo/StoreApprove?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    window.location.href = "/SaleManage/BuyoutInfo/Index";
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    }

}
//信息错误
function errorForm() {
    var data = {};
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    if (category == 1) {//仓库买断 warehouse
        data["main_id"] = warehouse_main_id;
        data["status"] = -1;
        $.submitForm({
            url: "/SaleManage/BuyoutInfo/StorageApprove?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    window.location.href = "/SaleManage/BuyoutInfo/ConfirmIndex";
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })

    } else if (category == 2) {//门店买断 store
        data["status"] = -1;
        data["main_id"] = main_id;
        $.submitForm({
            url: "/SaleManage/BuyoutInfo/StoreApprove?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    window.location.href = "/SaleManage/BuyoutInfo/ConfirmIndex";
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    }

}
