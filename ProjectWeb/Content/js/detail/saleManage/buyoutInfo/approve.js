var id = $.request("id");
var category;
var main_id;
var warehouse_main_id;
var approve_note_null;
var approve_num;
//登录人为分公司助理   
var MypositionInfo = top.clients.loginInfo.positionInfo;
var positionType = MypositionInfo.positionType;
$(function () {
    $.ajax({
        url: "/SaleManage/BuyoutInfo/GetBuyoutInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            
            $("#form1").formSerializeShow(data.mainInfo);
            category = data.mainInfo.category;
            var guide_name = data.mainInfo.guide_name;
            var guide_id = data.mainInfo.guide_id;
            if (guide_name == null || guide_id == null)
                $("#guide_name").text("无导购员");
            else
                $("#guide_name").text(guide_name);
            //买断列表
            if (category == 1) {
                $("#distributor_name1").text(data.mainInfo.distributor_name);
                $("#sales_name1").html(data.mainInfo.sales_name);
                $("#category").text("仓库买断");
                $("#approve").show();
                $("#warehouse").show();
                $.each(data.subInfoList, function (index, item) {
                    warehouse_main_id = item.main_id;
                    var tr;
                    tr = '<tr><td>' + item.model + '</td>' + '<td>' + item.color + '</td>' + '<td>' + item.price_buyout + '</td>' + '<td>' + item.buyout_count + '</td>' + '</td></tr>';
                    $("#warehouse").append(tr);
                })
                //审批
                var tr = '';
                
                $.each(data.approveInfoList, function (index, item) {
                    if (item.approve_note == null || item.approve_note == '') {
                        approve_note_null="--";
                    } else
                        approve_note_null = item.approve_note;
                    if (item.status > 0) {
                        tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                    } else {
                        tr += '<tr><td>' + item.approve_position_name + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                    }
                    

                })
                $("#approve tr:eq(1)").after(tr);
                
                    
            } else if (category == 2) {
                $("#distributor_name").text(data.mainInfo.distributor_name);
                $("#sales_name").text(data.mainInfo.sales_name);
                $("#category").text("门店买断");
                $("#store").show();
                $.each(data.buyoutInfoList, function (index, item) {
                    main_id = item.main_id;
                    var tr;
                    if (data.mainInfo.approve_status == 1) {
                        tr = '<tr><td>' + item.phone_sn + '</td>' + '<td>' + item.model + '</td>' + '<td>' + item.price_buyout + '</td>' + '<td>' + item.guide_commission + '</td>' + '<td><input class="form-control text-center" tag1="' + item.id + '"/></td></td></tr>';
                    } else {
                        tr = '<tr><td>' + item.phone_sn + '</td>' + '<td>' + item.model + '</td>' + '<td>' + item.price_buyout + '</td>' + '<td>' + item.guide_commission + '</td>' + '<td>' + item.sales_commission + '</td></tr>';
                    }
                    $("#store").append(tr);
                })
                if (positionType == 4) {//状态是分公司助理
                    $("#approve1").show();
                } else {
                    $("#approve").show();
                    //审批
                    var tr = '';
                    $.each(data.approveInfoList, function (index, item) {
                        if (item.approve_note == null || item.approve_note == '') {
                            approve_note_null = "--";
                        } else
                            approve_note_null = item.approve_note;
                        if (index == 0)
                            approve_num = ("一级");
                        else if (index == 1)
                            approve_num = ("二级");
                        else if (index == 2)
                            approve_num = ("三级");
                        else if (index == 3)
                            approve_num = ("四级");
                        if (item.status > 0) {
                            tr += '<tr><td>' + approve_num + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                        } else {
                            tr += '<tr><td>' + approve_num + '审批' + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                        }
                    })
                    $("#approve tr:eq(1)").after(tr);
                }
            }
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
        data["status"] = $("input[name='status']:checked").val();
        data["main_id"] = warehouse_main_id;
        data["approve_note"] = $("#approve_note").val();
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
        if (positionType == 4) {//分公司助理
            data["status"] = $("input[name='status1']:checked").val();
            data["approve_note"] = $("#approve_note1").val();
            data["gm2_approve"] = $("#gm2_approve").prop("checked");
            data["gm1_approve"] = $("#gm1_approve").prop("checked");
            for (var i = 1; i < $("#store tr").length; i++) {
                var $id = $('#store tr:eq(' + i + ') td:eq(4) input').attr("tag1");
                var $sales_commission = $('#store tr:eq(' + i + ') td:eq(4) input').val();
                var $guide_commission = $('#store tr:eq(' + i + ') td:eq(3) ').html();
                var json = { "id": $id, "main_id": main_id, "sales_commission": $sales_commission, "guide_commission": $guide_commission };
                buyoutList.push(json);
            }
            data["buyoutList"] = buyoutList;
        } else {
            data["status"] = $("input[name='status']:checked").val();
            data["approve_note"] = $("#approve_note").val();
        }
        data["main_id"] = main_id;
        $.submitForm({
            url: "/SaleManage/BuyoutInfo/StoreApprove?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    $.currentWindow().ReSearch();//更新
                    window.location.href = "/SaleManage/BuyoutInfo/ApproveIndex";
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    }
}

function ReSearch() {
    location.reload(force);
}