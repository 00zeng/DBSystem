var empName = top.clients.loginInfo.empName;
var empId = top.clients.loginInfo.empId;
$("#empName").text(empName);
$("#empName1").text(empName);
//获取id
var myCompanyInfo = top.clients.loginInfo.companyInfo;
var companyList = {};    // 上级机构列表
var guid = "";
var editData = [];
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

$(function () {
    $.ajax({
        url: "/SaleManage/buyoutInfo/GetSalesInfo?empId=" + empId,
        type: "get",
        dataType: "json",
        success: function (data) {
            $("#sales_name").text(data.sales_name);
            $("#distributor_name").text(data.name);
            var opt = '';
            if (data.guideList.length == 0) {
                opt += "<option value='" + 0 + "'>" + "--无导购员--" + "</option>";
            } else {
                $.each(data.guideList, function (index, item) {
                    if (index >= 0) {
                        opt += "<option value='" + item.id + "'>" + item.name + "</option>";
                    }
                })
            }
            $("#guide_id").html(opt);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})
//门店
function setPhoneSN(obj) {
    var index = $(obj).closest("tr").index();
    $.ajax({
        url: "/SaleManage/buyoutInfo/GetModelByPhoneSn",
        data: { "phone_sn": $(obj).val() },
        type: "get",
        dataType: "json",
        success: function (data) {
            
            if (data.length == 0) {
                $.modalAlert("请检查输入的串码是否正确！", "error");
                return false;
            } else {
                $('#store tr:eq(' + index + ') td:eq(1)').html(data[0].model);
                $('#store tr:eq(' + index + ') input:eq(1)').val(data[0].price_buyout);
            }

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}
var productList = [];
var productListSub = [];
$(function () {
    setModel();
    SetColor();
})
function setModel() {
    $("select[name='model']").empty();
    $.ajax({
        url: "/SaleManage/buyoutInfo/GetModelByModel",
        dataType: "json",
        async: false,
        success: function (data) {
            if (!!data || data != "") {
                productList = data;
                var $element = $("select[name='model']");
                $.each(productList, function (i) {
                    $element.append($("<option></option>").val(i).html(productList[i]["name"]));
                });

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

function SetPrice(obj) {
    var index = $(obj).closest("tr").index();
    $("#warehouse tr:eq(" + index + ") input[name='price_buyout']").val($("#warehouse tr:eq(" + index + ") select[name='color'] option:selected").attr("tag1"));
}

function SetColor(obj) {
    var index = $(obj).closest("tr").index();
    $("#warehouse tr:eq(" + index + ") select[name='color']").empty();
    var curModel = $("#warehouse tr:eq(" + index + ") select[name='model']").val();
    var $element = $("#warehouse tr:eq(" + index + ") select[name='color']");
    productListSub = productList[curModel].product_list;
    $.each(productListSub, function (i) {
        $("#warehouse tr:eq(" + index + ") select[name='color']").append($("<option tag1='" + productListSub[i]["price_buyout"] + "'></option>").val(productListSub[i]["id"]).html(productListSub[i]["color"]));
    });
    $("#warehouse tr:eq(" + index + ") input[name='price_buyout']").val(productListSub[0]["price_buyout"]);
}

//选择类型：1-仓库买断  2-门店买断
$("#category1").on("change", function () {
    if ($("#category1 option:selected").val() == 2) {
        $("#store").show();
        $("#warehouse").hide();
        $("#warehouse").val('');
    } else {
        $("#warehouse").show();
        $("#store").hide();
        $("#store").val('');
    }
})
//门店买断 增加行数
function StoreAddRow() {
    var Tr = "<tr><td>"
        + "<input class='form-control text-center'  onchange='setPhoneSN(this)'/>"
        + "</td>"
        + "<td><input class='form-control  text-center ' /></td>"
        + "<td><input class='form-control  text-center ' /></td>"
        + "<td>"
        + "<i class='glyphicon glyphicon-plus text-blue margin' onclick='StoreAddRow(this)'></i>"
        + "<i class='glyphicon glyphicon-minus text-red margin' onclick='deleteRow(this)' ></i>"
        + "</td>"
        + "</tr>";
    $("#store").append(Tr);
}

//仓库买断 增加行数
function WarehouseAddRow(e) {
    var htmlStr = $(e).parents('tr')[0].innerHTML;
    var td = (htmlStr.split('<i class="glyphicon glyphicon-plus text-blue margin" onclick="WarehouseAddRow(this)"></i>'))[0];
    td += "<i class='glyphicon glyphicon-plus text-blue margin' onclick='WarehouseAddRow(this)'></i>"
        + "<i class='glyphicon glyphicon-minus text-red margin' onclick='deleteRow(this)' ></i>"
        + "</td>"
        + "</tr>";
    $("#warehouse").append("<tr>" + td + "</tr>");
    $("select[name='model']:last").val(0).trigger("change");
}
function deleteRow(e) {
    $(e).parents('tr').remove();
}

$('#btnReturn').click(function () {
    if (!!editData && editData.length > 0) {
        $.modalConfirm("确定放弃当前编辑？", function (result) {
            if (result)
                window.history.go(-1);
            top.layer.closeAll();
        });
    }
    else
        window.history.go(-1);
})

function submitForm() {
    //if (!$("#form1").formValid())
    //    return false;

    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    //门店
    if ($("#category1 option:selected").val() == 2) {
        var buyoutList = [];
        for (var i = 1; i < $("#store tr").length; i++) {
            var $phone_sn = $('#store tr:eq(' + i + ') td:eq(0) input').val();
            var $model = $('#store tr:eq(' + i + ') td:eq(1) ').html();
            var $price_buyout = $('#store tr:eq(' + i + ') td:eq(2) input').val();
            var json = { "main_id": guid, "phone_sn": $phone_sn, "model": $model, "price_buyout": $price_buyout };
            if ($phone_sn == "" || $phone_sn == null) {
                $.modalAlert("串码不能为空！", "error");
                return false;
            } else {
                buyoutList.push(json);
            }
        }
        
        if ($("#guide_id").val() == 0) {
            data["guide_name"] = "无导购员";
            data["guide_id"] = $("#guide_id").val();
        } else {
            data["guide_id"] = $("#guide_id").val();
            data["guide_name"] = $("#guide_id").text();
        }
          
        data["buyoutList"] = buyoutList;
        data["id"] = guid;
        data["empId"] = empId;
        $.submitForm({
            url: "/SaleManage/BuyoutInfo/StoreAdd?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    window.location.href = "/SaleManage/BuyoutInfo/RequestIndex";
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    } else if ($("#category1 option:selected").val() == 1) {//仓库
        var buyoutList = [];
        for (var i = 1; i < $("#warehouse tr").length; i++) {
            var $model = $('#warehouse tr:eq(' + i + ') td:eq(0) select option:selected').text();
            var $color = $('#warehouse tr:eq(' + i + ') td:eq(1) select option:selected').text();
            var $price_buyout = $('#warehouse tr:eq(' + i + ') td:eq(2) input').val();
            var $buyout_count = $('#warehouse tr:eq(' + i + ') td:eq(3) input').val();
            var json = { "main_id": guid, "model": $model, "color": $color, "price_buyout": $price_buyout, "buyout_count": $buyout_count };
            if ($model == "" || $model == null) {
                $.modalAlert("型号不能为空！", "error");
                return false;
            } else {
                buyoutList.push(json);
            }
        }
        data["guide_id"] = $("#guides_name").val();
        data["guide_name"] = $("#guides_name").text();
        data["buyoutList"] = buyoutList;
        data["id"] = guid;
        data["empId"] = empId;
        $.submitForm({
            url: "/SaleManage/BuyoutInfo/StorageAdd?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    window.location.href = "/SaleManage/BuyoutInfo/RequestIndex";
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    } else {
        $.modalAlert("请检查申请类型！", "error");
        return false;
    }

}
