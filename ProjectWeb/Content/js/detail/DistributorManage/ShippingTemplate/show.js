var myCompanyInfo = top.clients.loginInfo.companyInfo;
var category = myCompanyInfo.category;
if (category == "事业部") {
    $("#editIcon").show();
} else {
    $("#editIcon").hide();
}
$(function () {
    $.ajax({
        url: "/DistributorManage/ShippingTemplate/GetEffectInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        success: function (data) {
            $("#form1").formSerializeShow(data.mainInfo);
            $("#minimum_fee").text(ToThousandsStr(data.mainInfo.minimum_fee));

            if (data.feeList == null && data.mainInfo == null) {
                $("#Newdiv").css("display", "");
                $("#showdiv").css("display", "none");
                $("#teble_div").css("display", "none");
            } else {
                $("#Newdiv").css("display", "none");
                $("#showdiv").css("display","");
                $("#teble_div").css("display", "");
                if (data.mainInfo.effect_status == -1) {
                    $("#effect_status").text("未生效");
                } else if (data.mainInfo.effect_status == 1) {
                    $("#effect_status").text("有效");
                } else {
                    $("#effect_status").text("已失效");

                }
            }


           
            ////运费为空  显示 new 
            //if (data.mainInfo.minimum_fee == "" || data.mainInfo.minimum_fee.length == 0) {
               
            //} else {
               
            //}

            var tr = '';
            $.each(data.feeList, function (index, item) {
                if (item.count_max == -1) {
                    item.count_max = "以上";
                }
                tr += "<tr><td><div class='input-group'><input  class='form-control text-center' value='" + ToThousandsStr(item.count_min) + "' disabled><span class='input-group-addon no-border'>-</span><input  class='form-control text-center'value='" + ToThousandsStr(item.count_max) + "' disabled><td>" + ToThousandsStr(item.shipping_fee) + "</td></tr>";
            })
            $("#shipping_tr").append(tr);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});