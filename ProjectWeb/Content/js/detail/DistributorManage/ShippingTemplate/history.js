var id = $.request("id");
var effect_status = decodeURI($.request("effect_status"));
$("#effect_status1").text(effect_status); 
$(function () {
    $.ajax({
        url: "/DistributorManage/ShippingTemplate/GetInfoById?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.mainInfo);
            var tr;
            $.each(data.feeList, function (index, item) {
                if (index == 0) {
                    tr = '<tr><th rowspan="100">按台计费</th><td>' + ToThousandsStr(item.count_min) + '台' + '-' + ToThousandsStr(item.count_max) + '台' + '</td>' + '<td>' + ToThousandsStr(item.shipping_fee) + '元/台' + '</td></tr>';
                } else {
                    if (item.count_max == -1) {
                        tr = '<tr><td>' + ToThousandsStr(item.count_min) + '台及以上' + '</td>' + '<td>' + ToThousandsStr(item.shipping_fee) + '元/台' + '</td></tr>';
                    } else {
                        tr = '<tr><td>' + ToThousandsStr(item.count_min) + '台' + '-' + ToThousandsStr(item.count_max) + '台' + '</td>' + '<td>' + ToThousandsStr(item.shipping_fee) + '元/台' + '</td></tr>';
                    }
                }
                $("#shipping_tr").append(tr);
            })

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});