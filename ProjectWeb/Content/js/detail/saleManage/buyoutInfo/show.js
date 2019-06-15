var id = $.request("id");
var category;
var role_id = top.clients.loginInfo.roleId;//3:经销商  5:业务员
var main_id;
var guide_name;
var warehouse_main_id;
$(function () {
    $.ajax({
        url: "/SaleManage/BuyoutInfo/GetBuyoutInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#form1").formSerializeShow(data.mainInfo);
            category = data.mainInfo.category;
          
            //买断列表
            if (category == 1) {
                $("#category").text("仓库买断");
                $("#warehouse").show();
                $.each(data.subInfoList, function (index, item) {
                    warehouse_main_id = item.main_id;
                    var tr;
                    tr = '<tr><td>' + item.model + '</td>' + '<td>' + item.color + '</td>' + '<td>' + item.price_buyout + '</td>' + '<td>' + item.buyout_count + '</td>' + '</td></tr>';
                    $("#warehouse").append(tr);
                })
            } else if (category == 2) {
                $("#category").text("门店买断");
                $("#store").show();
                $.each(data.buyoutInfoList, function (index, item) {
                    main_id = item.main_id;
                    var tr;
                    if (role_id == 3)//经销商 
                        tr = '<tr><td>' + item.phone_sn + '</td>' + '<td>' + item.model + '</td>' + '<td>' + item.price_buyout + '</td></tr>';
                    else if (role_id == 5) {//业务员可以看导购员提成
                        $("#guide_commission").css("display", "");
                        tr = '<tr><td>' + item.phone_sn + '</td>' + '<td>' + item.model + '</td>' + '<td>' + item.price_buyout + '<td>' + item.guide_commission + '</td>' + '</td></tr>';
                    } else {//
                        $("#guide_commission").css("display", "");
                        $("#sales_commission").css("display", "");
                        tr = '<tr><td>' + item.phone_sn + '</td>' + '<td>' + item.model + '</td>' + '<td>' + item.price_buyout + '</td>' + '<td>' + item.guide_commission + '</td>' + '<td>' + item.sales_commission + '</td></tr>';
                    }
                    //if (data.mainInfo.approve_status == 1) {
                    //    tr = '<tr><td>' + item.phone_sn + '</td>' + '<td>' + item.model + '</td>' + '<td>' + item.price_buyout + '</td>' + '<td>' + item.guide_commission + '</td>' + '<td><input  tag1="' + item.id + '"/></td></td></tr>';
                    //} else {
                    //    tr = '<tr><td>' + item.phone_sn + '</td>' + '<td>' + item.model + '</td>' + '<td>' + item.price_buyout + '</td>' + '<td>' + item.guide_commission + '</td>' + '<td>' + item.sales_commission + '</td></tr>';

                    //}
                    $("#store").append(tr);
                })
            }
            //审批
            if (data.creator_position_name == null ||data.creator_position_name == "") {
                $("#creator_position_name").text("-");
            } else {
                $("#creator_position_name").text(data.creator_position_name);
            }
            
            var tr = '';
            $.each(data.approveInfoList, function (index, item) {
                if (item.approve_note == null || item.approve_note == '') {
                    approve_note_null = "-";
                } else
                    approve_note_null = item.approve_note;
                if (item.status == 1 || item.status == -1)
                    approve_grade = ("一级审批");
                else if (item.status == 2 || item.status == -2)
                    approve_grade = ("二级审批");
                else if (item.status == 3 || item.status == -3)
                    approve_grade = ("三级审批");
                else if (item.status == 4 || item.status == -4)
                    approve_grade = ("四级审批");
                else if (item.status == 100 || item.status == -100)
                    approve_grade = ("终审");
                if (item.status > 0) {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                } else {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);
            
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});