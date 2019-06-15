var id = $.request("id");
$(function () {
    $.ajax({
        url: "/SaleManage/BuyoutInfo/GetBuyoutInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            
            //$("#form1").formSerializeShow(data.mainInfo);
            //买断列表
            if (data.mainInfo.category == 1) {
                $("#distributor_name1").text(data.mainInfo.distributor_name);
                $("#sales_name1").html(data.mainInfo.sales_name);
                $("#category").text("仓库买断");
                $("#warehouse").show();
                $.each(data.subInfoList, function (index, item) {
                    var tr;
                    tr = '<tr><th>型号</th><td>' + item.model + '</td>' + '<th>颜色</th><td>' + item.color + '</td>' + '<th>买断价</th><td>' + item.price_buyout + '</td>' + '<th>数量</th><td>' + item.buyout_count + '</td></tr>';
                    $("#warehouse").append(tr);
                })
            } else if (data.mainInfo.category == 2) {
                $("#distributor_name").text(data.mainInfo.distributor_name);
                $("#sales_name").text(data.mainInfo.sales_name);
                $("#category").text("门店买断");
                $("#store").show();
                $.each(data.buyoutInfoList, function (index, item) {
                    var tr;
                    tr = '<tr><th>串码</th><td>' + item.phone_sn + '</td>' + '<th>型号</th><td>' + item.model + '</td>' + '<th>买断价</th><td>' + item.price_buyout + '</td></tr>';
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