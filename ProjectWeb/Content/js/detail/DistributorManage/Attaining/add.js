var myCompanyInfo = top.clients.loginInfo.companyInfo;
var checkTypeVal = $('input[name="pay_mode"]:checked').val();//返利发放
var checkTargetVal = $('input[name="target_mode"]:checked').val();//返利模式
var checkSaleVal = $('input[name="target_sale"]:checked').val();//销量阶梯
var checkRebateVal = $('input[name^="rebate_mode"]:checked').val();//返利金额
var checkProductVal = $('input[name="product_scope1"]:checked').val();//活动机型
var mainGuid = "";
var distributorTree = [];
var areaFullList = [];
var dFullList = []; // 所有经销商
var dSelList = [];  // 已选经销商
var pFullList = [];   // 所有机型
var pRestList = [];
var pSelList = [];
var timesArr = new Array();
timesArr.push(1);
var pSelListArr = new Array();    // 已选机型
pSelListArr.push([]);
var $tableDistributor = $('#targetDistributor');
var $tableProduct = $('table[id^=targetProduct]');// 可用于页面大小改变时
var $tableRebateProduct = $('#rebateProduct');  // 按型号返利
var $pCountWrap = $("#productCountWrap1");
var $pTableWrap = $("#targetProductWrap1");
var index;//行号
var sum = 0;
var trProduct = '';
var nowIndex = 0;
var ProNowIndex = 0;
var ProIndex = 0;
var pIndex = 0;

var str1 = "<tr style='display: table;width: 100%;table-layout: fixed;'>"
        + "<td class='col-sm-6'><div class='input-group'><input class='form-control text-center' name='target_min_{0}'  onchange='setNum(this)'>"
        + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center required number' name='target_max_{0}' onchange='setNum(this)'>"
        + "<td class='col-sm-4'><div class='input-group'><input class='form-control text-center' name='rebate_{0}'>"
        + "<span name='rebate_mode_input1_{0}' class='input-group-addon no-border'>";
var str2 = "元/台";
var str3 = "</span><span name='rebate_mode_input2_{0}' class='input-group-addon no-border no-padding'>";
var str4 = "";
var str5 = "</span></div></td>"
        + "<td class='col-sm-2'><div class='row'><i onclick='addRow(this)' data-index='{1}' name='addRow_{0}' class='fa fa-plus-circle fa-lg  text-blue '></i> <i onclick='deleteRow(this)' class='fa fa-minus-circle fa-lg text-red'></i></div></td>"
        + "</tr>";
//型号的th
var str6 = '<tr><th style="text-align:center;vertical-align:middle;" class="col-sm-3"><span name="product_change">完成率（%）</span></th><th style="text-align:center;vertical-align:middle;" class="col-sm-2">型号</th><th style="text-align:center;vertical-align:middle;" class="col-sm-2">颜色</th><th style="text-align:center;vertical-align:middle;" class="col-sm-3"><span name="rebate_mode_change_{0}">';
var strUint = "返利金额";
var str06 = '</span></th><th style="text-align:center;vertical-align:middle;" class="col-sm-2"></th></tr>';

var str7 = ' <tr> <td class="col-sm-3"><div class="input-group"><input class="form-control text-center" name="sma_target_min_{0}"  onchange="setNumSmall(this)"><span class="input-group-addon no-border">-</span>'
            + '<div class="formValue"><input class="form-control text-center required number" name="sma_target_max_{0}"  onchange="setNumSmall(this)"></div></div></td>'
            + '<td class="col-sm-1" style="text-align:center;vertical-align:middle"><i onclick="addSmall(this)" name="addSmall_{0}" data-index="{1}" class="fa fa-plus  text-blue "></i>&nbsp;<i onclick="delSmall(this)" class="fa fa-minus   text-red "></i>'
            + '</td><td class="col-sm-3"><div class="input-group formValue"><input class="form-control text-center required number" name="nor_rebate_{0}"><span name="rebate_mode_input1_{0}" class="input-group-addon no-border">'
var str8 = '</span>'
            + '<span name="rebate_mode_input2_{0}" class="input-group-addon no-border  no-padding">';
var str9 = '</span></div></td></tr> ';

var str11 = '<tr> <td class="col-sm-3" style="text-align:center;vertical-align:middle"><div class="input-group"><input class="form-control text-center" name="nor_target_min_{0}"  onchange="setNumNor(this)" >'//
    + '<span class="input-group-addon no-border">-</span><div class="formValue"><input class="form-control text-center required number" name="nor_target_max_{0}" onchange="setNumNor(this)">'
    + '</div></div></td><td><table class="table table-bordered text-center" name="small_table_{0}" style="margin-bottom:0px"><tr><td class="col-sm-3"><div class="input-group">'
    + '<input class="form-control text-center" name="sma_target_min_{0}" onchange="setNumSmall(this)"><span class="input-group-addon no-border">-</span><div class="formValue">'
    + '<input class="form-control text-center required number" name="sma_target_max_{0}" onchange="setNumSmall(this)"></div></div> </td><td class="col-sm-1" style="text-align:center;vertical-align:middle">'
    + '<i onclick="addSmall(this)" name="addSmall_{0}" data-index="{1}" class="fa fa-plus text-blue "></i>&nbsp;<i onclick="delSmall(this)" class="fa fa-minus   text-red "></i></td><td class="col-sm-3"><div class="input-group formValue"><input class="form-control text-center required number" name="nor_rebate_{0}">'
    + '<span name="rebate_mode_input1_{0}" class="input-group-addon no-border">';
var str12 = '</span><span name="rebate_mode_input2_{0}" class="input-group-addon no-border  no-padding">';
var str13 = '</span></div></td></tr>'
    + '<tr><td class="col-sm-3"><div class="input-group"><input class="form-control text-center" name="sma_target_min_{0}" onchange="setNumSmall(this)"><span class="input-group-addon no-border">-</span><div class="formValue">'
    + '<input class="form-control text-center required number" name="sma_target_max_{0}"  value="以上" disabled></div></div></td><td class="col-sm-1" style="text-align:center;vertical-align:middle">'
    + '<i onclick="addSmall(this)" name="addSmall_{0}" data-index="{1}" class="fa fa-plus text-blue "></i></td><td class="col-sm-3">'
    + '<div class="input-group formValue"><input class="form-control text-center required number" name="nor_rebate_{0}"> <span name="rebate_mode_input1_{0}" class="input-group-addon no-border">';
var str14 = '</span>'
    + '<span name="rebate_mode_input2_{0}" class="input-group-addon no-border  no-padding">';
var str15 = '</span></div></td></tr> </table> </td>'
    + '<td class="col-sm-2" style="text-align:center;vertical-align:middle"><i onclick="addNormal(this)"  data-index="{1}" name="addNormal_{0}" class="fa fa-plus-circle fa-lg  text-blue "></i>&nbsp;<i onclick="delNormal(this)" class="fa fa-minus-circle fa-lg  text-red "></i>'
    + ' </td></tr>';


var $duallistDistributor = $('#duallistDistributor').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入名称、区域关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}家经销商",
    infoTextEmpty: "共0家经销商",
    infoTextFiltered: "",
    filterTextClear: "清空"
});

var bootstrapProductOpt = {
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入型号、颜色关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个机型",
    infoTextEmpty: "共0个机型",
    infoTextFiltered: "",
    filterTextClear: "清空"
};

var $duallistProduct = $('#duallistProduct1').bootstrapDualListbox(bootstrapProductOpt);
//时间
function DateSelect(e) {
    var ProIndex = $(e).parents("div[id^=proBox]").data("index");
    var now_index = $(e).attr("data-index");//data-index
    var index = $("input[id^=end_time_" + ProIndex + "_]").index(e);//当前点击的行数
    var nowDate = new Date($(e)[0].value);//格式化日期
    var xIntervalStr = "d";//天数
    var stratData = nowDate.dateAdd(xIntervalStr, 1);//日期增加一天
    var curEndInput=$("input[id^=end_time_" + ProIndex + "_]:eq(" + (index) + ")");//当前结束时间
    var nextStartInput=$("input[id^=start_time_" + ProIndex + "_]:eq(" + (index + 1) + ")");//下一行开始时间
    var nextEndInput=$("input[id^=end_time_" + ProIndex + "_]:eq(" + (index + 1) + ")");//下一行结束时间
    if (curEndInput.val() != "") {//当前结束时间不为空时
        nextStartInput.val(stratData.pattern("yyyy-MM-dd"));
        if (nextEndInput.val() != "") {//下一行结束时间不为空时
            if (nextStartInput.val() > nextEndInput.val()) {
                curEndInput.val("");
                nextStartInput.val("");
                $.modalAlert("开始时间不能大于结束时间!");
                return false;
            }
        } else {//下一行结束时间为空时
            if (nextStartInput.val() > enddate) {
                curEndInput.val("");
                nextStartInput.val("");
                $.modalAlert("时间分段开始时间不能大于活动结束时间!");
                return false;
            }
        }
    }
    //时间判断 开始时间不能大于结束时间
    //下一行的开始时间比上一行结束时间 多一天
}
var proBox = "";
function DateShow() {

    proBox = $("div[id^=proBox]");
    var proBoxNum = proBox.length;//机型分段
    for (var i = 0; i < proBoxNum; i++) {
        $(proBox[i]).find("input[id^=start_time_]:first").val(startdate);
        $(proBox[i]).find("input[id^=end_time_]:last").val(enddate);
        $(proBox[i]).find("input[id^=end_time_]:last").attr("disabled","disabled");
    }
}
var startdate = '';
var enddate = '';
function Month() {
    startdate = $("#start_date").val();
    enddate = $("#end_date").val();
    DateShow();
    if (startdate.substring(0, 7) == enddate.substring(0, 7)) {
        $("input[name='pay_mode'][value='2']").attr("disabled", "disabled");
    } else
        $("input[name='pay_mode'][value='2']").removeAttr("disabled");
}
$('input[name="pay_mode"]').on("change", function () {
    checkTargetVal = $('input[name="pay_mode"]:checked').val();
    if (checkTargetVal == 2) {
        document.getElementById('start_date').onfocus = function () {
            var date1 = limitMonthDate(1);
            WdatePicker({ readOnly: true, maxDate: date1 });
        }
        document.getElementById('end_date').onfocus = function () {
            var date2 = limitMonthDate(2);
            WdatePicker({ readOnly: true, minDate: date2 });
        }
    } else {
        document.getElementById('start_date').onfocus = function () {
            WdatePicker({ readOnly: true, maxDate: '#F{$dp.$D(\'end_date\')}' });
        }
        document.getElementById('end_date').onfocus = function () {
            WdatePicker({ readOnly: true, minDate: '#F{$dp.$D(\'start_date\')}' });
        }
    }
})

function limitMonthDate(e) {
    var DateString;
    if (e == 2) {
        var beginDate = $dp.$("start_date").value;
        if (beginDate != "" && beginDate != null) {
            var limitDate = new Date(beginDate);
            limitDate.setDate(new Date(limitDate.getFullYear(), limitDate
                    .getMonth() + 1, 0).getDate() + 1); //获取此月份的天数  
            DateString = limitDate.getFullYear() + '-'
                    + (limitDate.getMonth() + 1) + '-'
                    + limitDate.getDate();
            return DateString;
        }
    }
    if (e == 1) {
        var endDate = $dp.$("end_date").value;
        if (endDate != "" && endDate != null) {
            var limitDate = new Date(endDate);
            limitDate.setDate("0");
            DateString = limitDate.getFullYear() + '-'
                    + (limitDate.getMonth() + 1) + '-'
                    + limitDate.getDate();
            return DateString;
        }
    }

}

$(function () {
    $("#sale_avg_before").text(0);
    $("#form1").queryFormValidate();
    $(window).resize(function () {
        $tableDistributor.bootstrapTable('resetView', {
            height: 451
        });
        $tableProduct.bootstrapTable('resetView', {
            height: 451
        });
    });
    $pTableWrap.hide();
    $pCountWrap.hide();
    // $tableRebateProduct.hide();
    mainGuid = GetGUID();
    TablesInit();
    BindCompany();
    Rebate_mode();
    targetSale();
});


var sale_avg;
var idStr = "";

//前三个目标销量
function SaleAvgBefore() {
    $.each(dSelList, function (index, item) {
        idStr = "";
        var strId = item.id;
        var str = "|";
        idStr+= strId + str;
    })

    $.ajax({
        url: "/DistributorManage/DistributorManage/GetAvgBefore?idStr=" + idStr,
        async: false,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            sale_avg = data;
            $("#sale_avg_before").text(data);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

function GetGUID() {
    var guid = "";
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
    return guid;
}

function BindCompany() {
    if (myCompanyInfo.category == "分公司") {
        $("#company").append("<option value='" + myCompanyInfo.id + "' selected>" + myCompanyInfo.name + "</option>");
    } else if (myCompanyInfo.category == "事业部") {
        $.ajax({
            url: "/OrganizationManage/Company/GetIdNameList",
            async: false,
            type: "get",
            dataType: "json",
            contentType: "application/json",
            success: function (data) {
                $.each(data, function (index, value) {
                    $("#company").append("<option value='" + value.id + "'>" + value.name + "</option>");
                })
            },
            error: function (data) {
                $.modalAlert("系统出错，请稍候重试", "error");
            }
        })
    } else {
        $.modalAlert("经销商政策须由分公司或事业部录入", "error");
        window.history.go(-1);
        return;
    }
    $("#company").on("change", function () {
        areaFullList.splice(0, areaFullList.length);
        dFullList.splice(0, dFullList.length);  // 清空数组
        dSelList.splice(0, dSelList.length);  // 清空数组
        $duallistDistributor.empty();
        $duallistDistributor.bootstrapDualListbox("refresh");

        pFullList.splice(0, pFullList.length);
        $duallistProduct.empty();
        $duallistProduct.bootstrapDualListbox("refresh");
        $pTableWrap.hide();
        $("#all_product").prop("checked", "checked");

        GetDistributorTree();
        getProduct();
    })
    GetDistributorTree();
    getProduct();
}

function GetDistributorTree() {
    $.ajax({
        url: "/DistributorManage/DistributorManage/GetDistributorTree",
        data: { companyId: $("#company").val() },
        async: false,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            distributorTree.splice(0, distributorTree.length, data);
            if (data.length > 0) {
                $.each(data[0].area_l2_list, function (index, value) {
                    areaFullList.push(value);
                    if (value.key_list.length > 0) {
                        $.each(value.key_list, function (index1, value1) {
                            var d = value1;
                            d.main_id = mainGuid;
                            d.distributor_id = d.id;
                            d.distributor_name = d.name;
                            dFullList.push(d);
                            dSelList.push(d);
                            $duallistDistributor.append("<option value='" + value1.id + "'>" + value1.display_info + "</option>");
                        });
                    }
                })
            }
            SaleAvgBefore();
            $duallistDistributor.bootstrapDualListbox("refresh");
            $tableDistributor.bootstrapTable('load', dFullList);
            $("#distributor_count").text(dFullList.length);
        },
        error: function (data) {
            $.modalAlert("系统出错，请稍候重试", "error")
            $tableDistributor.bootstrapTable('load', dFullList);
            $("#distributor_count").text(0);
        }
    })

}

function getProduct() {
    $.ajax({
        url: "/ProductManage/PriceInfo/GetEffectIdNameList",
        data: { companyId: $("#company").val() },
        type: "get",
        dataType: "json",
        success: function (data) {
            pFullList = data;
            pRestList = data;
            $.each(pFullList, function (index, value) {
                //value.main_id = mainGuid;
                $duallistProduct.append("<option value='" + value.id + "'>" + value.model + "(" + value.color + ")" + "</option>");
            })
            $duallistProduct.bootstrapDualListbox("refresh");
            $tableProduct.bootstrapTable('load', []);
            sum = pSelList.length;
            $("#product_count").text("0");
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

function getPIndex(e) {
    ProIndex = $(e).parents("div[id^=proBox]").data("index");
    ProNowIndex = $(e).data('index');

}
  // eIndex == 1  指定机型关闭事件  2 :添加时间分段
function ProductScope(eIndex) {
    var pTable = [];
    if (eIndex == 1) {
        $("table[id^=rebateMode4_" + ProIndex + "]").empty();
        pTable = $("table[id^=rebateMode4_" + ProIndex + "]");
    } else {
        pTable = $("table[id=rebateMode4_" + ProIndex + "_" + timesArr[ProIndex - 1] + "]");
    }
    sum = pSelList.length;
    if (sum == 0)
        return;
    var tr = "";
    var tr1 = "";
    var unit2 = 0;

    $.each(pTable, function (indexCopy, itemCopy) {
        tr = "";
        unit2 = itemCopy.attributes["data-index"].value;

        GetTrProduct(unit2, false);
    });
    targetSale();

}
function GetTrProduct(tIndex, forNewRow) {
    var newTr = "", newTr1 = "", newTrProduct = "", new_str2 = "", new_str4 = "", strUint = "";
    var curPSelList = pSelListArr[ProIndex - 1];
    var trRowSpan = curPSelList.length;
    var selIndex = $('input[name="rebate_mode_' + ProIndex + '_' + tIndex + '"]:checked').val();
    if (selIndex == 1) {
        strUint = "返利金额";
        new_str2 = "元/台";
        new_str4 = "";
    } else if (selIndex == 2) {
        strUint = "批发价比例";
        new_str2 = "%";
        new_str4 = "*批发价";
    } else if (selIndex == 3) {
        strUint = "零售价比例";
        new_str2 = "%";
        new_str4 = "*零售价";
    } else if (selIndex == 4) {
        strUint = "固定金额";
        new_str2 = "元";
        new_str4 = "";
    }
    $.each(curPSelList, function (index, item) {
        if (index == 0) {//第一行 有一列完成率  // tr - trProduct 相差 删除按钮
            newTr += "<tr><td  class='col-sm-3' rowspan='" + trRowSpan + "' style='text-align:center;'><div class='input-group formValue'>"
                    + "<input class='form-control text-center' name='pro_target_min_" + ProIndex + "_" + tIndex + "' onchange='setNumPro(this)' ><span class='input-group-addon no-border'>-</span><input class='form-control text-center required number'value='以上' disabled name='pro_target_max_" + ProIndex + "_" + tIndex + "' onchange='setNumPro(this)'>"
                    + "</div></td><td  class='col-sm-2' style='text-align:center;vertical-align:middle;' name='model_pro' >" + item.model + "</td><td class='col-sm-2' style='text-align:center;vertical-align:middle;' name='color_pro'>" + item.color + "</td>"
                    + "<td class='col-sm-3' style='text-align:center;vertical-align:middle;' ><div class='input-group formValue'><input class='form-control text-center' name='rebate_p_" + ProIndex + "_" + tIndex + "'><span name='rebate_mode_input1_" + ProIndex + "_" + tIndex + "' class='input-group-addon no-border'>" + new_str2 + "</span>"
                    + "<span name='rebate_mode_input2_" + ProIndex + "_" + tIndex + "' class='input-group-addon no-border no-padding'>" + new_str4 + "</span></div></td><td style='text-align:center;' class='col-sm-2' rowspan='" + trRowSpan + "'> <div class='row'><i onclick='addTr(this)' name='addTr_" + ProIndex + "_" + tIndex + "' data-index='" + tIndex + "' class='fa fa-plus-circle fa-lg  text-blue margin'></i>"
                    + "</div></td></tr>";
            newTrProduct += "<tr><td  class='col-sm-3' rowspan='" + trRowSpan + "' style='text-align:center;'><div class='input-group formValue'>"
                    + "<input class='form-control text-center' name='pro_target_min_" + ProIndex + "_" + tIndex + "' onchange='setNumPro(this)'><span class='input-group-addon no-border'>-</span><input class='form-control text-center required number' name='pro_target_max_" + ProIndex + "_" + tIndex + "' onchange='setNumPro(this)'>"
                    + "</div></td><td  class='col-sm-2' style='text-align:center;vertical-align:middle;'  name='model_pro'>" + item.model + "</td><td class='col-sm-2' style='text-align:center;vertical-align:middle;' name='color_pro'>" + item.color + "</td>"
                    + "<td class='col-sm-3' style='text-align:center;vertical-align:middle;' ><div class='input-group formValue'><input class='form-control text-center' name='rebate_p_" + ProIndex + "_" + tIndex + "'><span name='rebate_mode_input1_" + ProIndex + "_" + tIndex + "' class='input-group-addon no-border'>" + new_str2 + "</span>"
                    + "<span name='rebate_mode_input2_" + ProIndex + "_" + tIndex + "' class='input-group-addon no-border no-padding'>" + new_str4 + "</span></div></td><td style='text-align:center;' class='col-sm-2' rowspan='" + trRowSpan + "'> <div class='row'><i onclick='addTr(this)' name='addTr_" + ProIndex + "_" + tIndex + "' data-index='" + tIndex + "' class='fa fa-plus-circle fa-lg  text-blue margin'></i>"
                    + "<i onclick='deleteTr(this)' class='fa fa-minus-circle fa-lg text-red margin'></i></div></td></tr>";
        } else {
            newTr1 += "<tr><td class='col-sm-2' style='text-align:center;vertical-align:middle;'  name='model_pro' >" + item.model + "</td><td class='col-sm-2' style='text-align:center;vertical-align:middle;' name='color_pro'>" + item.color + "</td>"
                    + "<td class='col-sm-3' style='text-align:center;vertical-align:middle;'><div class='input-group formValue'><input class='form-control text-center' name='rebate_p_" + ProIndex + "_" + tIndex + "'><span name='rebate_mode_input1_" + ProIndex + "_" + tIndex + "' class='input-group-addon no-border'>" + new_str2 + "</span>"
                    + "<span name='rebate_mode_input2_" + ProIndex + "_" + tIndex + "' class='input-group-addon no-border no-padding'>" + new_str4 + "</span></div></td></tr>";
        }
    })
    newTrProduct += newTr1;
    newTr += newTr1;
    if (forNewRow == false) {
        var str6_new = str6.replace("rebate_mode_change_1_1", "rebate_mode_change_" + ProIndex + "_" + tIndex);
        $("table[id=rebateMode4_" + ProIndex + "_" + tIndex + " ]").append('<thead style="display: table;width: 99%;table-layout: fixed;">' + str6_new + strUint + str06 + '</thead>' + '<tbody style="display: block;height:273px;overflow-y:scroll;overflow-x:hidden">' + newTr + '</tbody>');
        return null;
    }
    return newTrProduct;

}
//活动机型 
function productScopeChange() {
    //ProIndex = $(e).parents("div[id^=proBox]").data("index");
    checkProductVal = $('input[name="product_scope' + ProIndex + '"]:checked').val();
    if (checkProductVal == 1) {
        $("#targetProductWrap" + ProIndex).hide();
        $("#productCountWrap" + ProIndex).hide();
        $("input[name^='target_mode_" + ProIndex + "'][value='6']").prop("checked", "checked");//默认无table
        $("input[name^='rebate_mode_" + ProIndex + "'][value='4']").removeAttr("disabled"); //取消disable固定金额
        $("input[name^='target_mode_" + ProIndex + "'][value='4']").attr("disabled", "disabled"); // 返利模式-按型号 
        $("table[id^=rebateMode6_" + ProIndex + "]").show();
        $("table[id^=rebateMode4_" + ProIndex + "]").hide();
        $("table[id^=rebateMode1_" + ProIndex + "]").hide();
    } else {
        Rebate_mode(); //影响pIndex 赋值 

        checkRebateVal = $('input[name^="rebate_mode_' + ProIndex + '"]:checked').val()
        if (checkRebateVal != 4) {//固定金额 为4
            $("input[name^='target_mode_" + ProIndex + "'][value='4']").removeAttr("disabled"); // 返利模式-按型号
        }
        if (checkTargetVal == 3 || checkTargetVal == 4 || checkTargetVal == 5) {
            $("input[name^='rebate_mode_" + ProIndex + "'][value='4']").attr("disabled", "disabled");
        }
        $("#targetProductWrap" + ProIndex).show();
        $("#productCountWrap" + ProIndex).show();
    }
}
//销量阶梯
function targetSale() {
    checkSaleVal = $('input[name="target_sale"]:checked').val();//返利模式
    if (checkSaleVal == 1) {
        $("span[name='sale_change']").html("完成率（%）");
        $("span[name='product_change']").html("完成率（%）");
        // 按完成率
        $("#act_show").css("display", "none");
        $("input[name='rebate_mode'][value='4']").removeAttr("disabled");
    } else if (checkSaleVal == 2) {
        // 按台数
        $("#act_show").css("display", "");

        $("span[name='sale_change']").html("销量（台）");
        $("span[name='product_change']").html("销量（台）");
        $("input[name='rebate_mode'][value='4']").removeAttr("disabled");
    }
}
//返利发放
$('input[name="pay_mode"]').on("change", function () {
    checkTypeVal = $('input[name="pay_mode"]:checked').val();//返利发放
    if (checkTypeVal == 1) {
        // 一次性
        $("span[name='activity_unit']").html("台");
    } else if (checkTypeVal == 2) {
        //按月 
        $("span[name='activity_unit']").html("台/月");

    }
})
function addTimes(e) {
    ProIndex = $(e).parents("div[id^=proBox]").data("index");
    var times = timesArr[ProIndex-1];
    timesArr[ProIndex-1] = ++times;
    var c = '<div class="col-xs-12 col-sm-12 col-lg-6" id="copy_' + ProIndex + '_' + times + '" data-index="' + times + '">'
    + '<div class="col-xs-12 col-sm-12 col-lg-12 no-padding"><div class="box box-widget">'
    + '<div class="box-header with-border"><div class="col-lg-12"><div class="col-lg-6">'
    + '<div class="input-group col-lg-12"><input class="form-control" id="start_time_' + ProIndex + '_' + times + '"  data-index="' + times + '" disabled>'
    + '<span class="input-group-addon no-border"> 至 </span>'
    + '<input class="form-control" id="end_time_' + ProIndex + '_' + times + '"  data-index="' + times + '" readonly="readonly" onfocus="WdatePicker({ onpicked: DateSelect(this),minDate: \'#F{$dp.$D(\\\'start_time_' + ProIndex + '_' + times + '\\\')}\', maxDate: \'#F{$dp.$D(\\\'end_date\\\')}\' });"></div></div></div>'
    + '<div class="box-tools pull-right" name="removeIcon">'
    + '<button type="button" class="btn btn-box-tool" data-widget="collapse">'
    + '<i class="fa fa-minus"></i></button><button type="button" class="btn btn-box-tool" onclick="delDiv(this)"><i class="fa fa-remove"></i></button></div></div>'
    + '<div class="box-body"><div class="col-md-12 col-lg-12 margin-bottom">'
    + '<div class="row"><div class="form-group col-sm-12"><label>返利模式</label>'
    + '<div><div class="radio col-sm-3">'
    + '<label><input name="target_mode_' + ProIndex + '_' + times + '" data-index="' + times + '" type="radio" value="3" onchange="Target_mode(this)" checked>按零售价</label></div>'
    + '<div class="radio col-sm-3">'
    + '<label><input name="target_mode_' + ProIndex + '_' + times + '" type="radio" data-index="' + times + '" value="5" onchange="Target_mode(this)" >按批发价</label></div>'
    + '<div class="radio col-sm-3">'
    + '<label><input name="target_mode_' + ProIndex + '_' + times + '" type="radio" data-index="' + times + '" value="4" disabled onchange="Target_mode(this)">按型号</label></div>'
    + '<div class="radio col-sm-3">'
    + '<label><input name="target_mode_' + ProIndex + '_' + times + '" type="radio" data-index="' + times + '" value="6" onchange="Target_mode(this)">无'
    + '</label></div></div></div></div>'
    + '<div class="form-group"><label>返利金额</label>'
    + '<div><div class="radio col-sm-3 col-lg-3">'
    + '<label><input name="rebate_mode_' + ProIndex + '_' + times + '" data-index="' + times + '" type="radio" value="1" checked onchange="RebateModeClick(this)">每台固定金额</label></div>'
    + '<div class="radio col-sm-3 col-lg-3">'
    + '<label><input name="rebate_mode_' + ProIndex + '_' + times + '" type="radio" data-index="' + times + '" value="2" onchange="RebateModeClick(this)">每台批发价比例</label></div>'
    + '<div class="radio col-sm-3 col-lg-3">'
    + '<label><input name="rebate_mode_' + ProIndex + '_' + times + '" type="radio" data-index="' + times + '" value="3" onchange="RebateModeClick(this)">每台零售价比例</label></div>'
    + '<div class="radio col-sm-3 col-lg-3">'
    + '<label><input name="rebate_mode_' + ProIndex + '_' + times + '" type="radio" data-index="' + times + '" value="4" onchange="RebateModeClick(this)" disabled> 固定总金额'
    + '</label></div></div></div></div><br />'
    + '<table id="rebateMode1_' + ProIndex +  '_' + times + '" class="table table-bordered text-center">'
    + '<thead style="display:table;width:99%;table-layout:fixed;">'
    + '<tr><th class="col-sm-3"><span name="sale_change">完成率（%）</span></th>'
    + '<th class="col-sm-3"><span name="target_mode_change_' + ProIndex +  '_' + times + '">零售价（元）</span></th>'
    + '<th class="col-sm-1"></th>'
    + '<th class="col-sm-3"><span name="rebate_mode_change_' + ProIndex +  '_' + times + '">返利金额</span></th>'
    + '<th class="col-sm-2"></th></tr></thead>'
    + '<tbody style="display:block;height:273px;overflow-y:scroll;overflow-x:hidden">'
    + '<tr><td class="col-sm-3" style="text-align:center;vertical-align:middle">'
    + '<div class="input-group">'
    + '<input class="form-control text-center" name="nor_target_min_' + ProIndex + '_' + times + '" onchange="setNumNor(this)">'
    + '<span class="input-group-addon no-border">-</span>'
    + '<div class="formValue">'
    + '<input class="form-control text-center required number" name="nor_target_max_' + ProIndex + '_' + times + '" disabled value="以上">'
    + '</div></div></td>'
    + '<td><table class="table table-bordered text-center" name="small_table_' + ProIndex + '_' + times + '" style="margin-bottom:0px">'
    + '<tr><td class="col-sm-3"><div class="input-group">'
    + '<input class="form-control text-center" name="sma_target_min_' + ProIndex + '_' + times + '" onchange="setNumSmall(this)">'
    + '<span class="input-group-addon no-border">-</span>'
    + '<div class="formValue">'
    + '<input class="form-control text-center required number" name="sma_target_max_' + ProIndex + '_' + times + '" onchange="setNumSmall(this)">'
    + '</div></div></td>'
    + '<td class="col-sm-1" style="text-align:center;vertical-align:middle">'
    + '<i onclick="addSmall(this)" name="addSmall_' + ProIndex + '_' + times + '" data-index="' + times + '" class="fa fa-plus text-blue"></i>&nbsp;'
    + '<i onclick="delSmall(this)" class="fa fa-minus text-red"></i></td>'
    + '<td class="col-sm-3"><div class="input-group formValue">'
    + '<input class="form-control text-center required number" name="nor_rebate_' + ProIndex + '_' + times + '">'
    + '<span name="rebate_mode_input1_' + ProIndex +  '_' + times + '" class="input-group-addon no-border">元/台</span>'
    + '<span name="rebate_mode_input2_' + ProIndex +  '_' + times + '" class="input-group-addon no-border  no-padding"></span></div></td></tr>'
    + '<tr><td class="col-sm-3"><div class="input-group">'
    + '<input class="form-control text-center" name="sma_target_min_' + ProIndex + '_' + times + '" onchange="setNumSmall(this)">'
    + '<span class="input-group-addon no-border">-</span>'
    + '<div class="formValue">'
    + '<input class="form-control text-center required number" name="sma_target_max_' + ProIndex + '_' + times + '" disabled value="以上">'
    + '</div></div></td>'
    + '<td class="col-sm-1" style="text-align:center;vertical-align:middle">'
    + '<i onclick="addSmall(this)" name="addSmall_' + ProIndex + '_' + times + '" data-index="' + times + '" class="fa fa-plus text-blue"></i></td>'
    + '<td class="col-sm-3">'
    + '<div class="input-group formValue"><input class="form-control text-center required number" name="nor_rebate_' + ProIndex + '_' + times + '">'
    + '<span name="rebate_mode_input1_' + ProIndex +  '_' + times + '" class="input-group-addon no-border">元/台</span>'
    + '<span name="rebate_mode_input2_' + ProIndex +  '_' + times + '" class="input-group-addon no-border  no-padding"></span>'
    + '</div></td></tr></table></td>'
    + '<td class="col-sm-2" style="text-align:center;vertical-align:middle">'
    + '<i onclick="addNormal(this)" data-index="' + times + '" name="addNormal_' + ProIndex + '_' + times + '" class="fa fa-plus-circle fa-lg text-blue"></i>'
    + '</td></tr></tbody></table>'
    + '<table id="rebateMode4_' + ProIndex + '_' + times + '" data-index="' + times + '" class="table table-bordered text-center" hidden></table>'
    + '<table id="rebateMode6_' + ProIndex +  '_' + times + '" class="table table-bordered text-center" hidden>'
    + '<thead style="display:table;width:99%;table-layout:fixed;">'
    + '<tr><th class="col-sm-6"><span name="sale_change">完成率（%）</span></th>'
    + '<th class="col-sm-4"><span name="rebate_mode_change_' + ProIndex +  '_' + times + '">返利金额</span></th>'
    + '<th class="col-sm-2"></th></tr></thead>'
    + '<tbody style="display:block;height:273px;overflow-y:scroll;overflow-x:hidden">'
    + '<tr style="display:table;width:100%;table-layout:fixed;">'
    + '<td class="col-sm-6"><div class="input-group">'
    + '<input class="form-control text-center" name="target_min_' + ProIndex + '_' + times + '" disabled value="0" >'
    + '<span class="input-group-addon no-border">-</span><div class="formValue">'
    + '<input class="form-control text-center required number" name="target_max_' + ProIndex + '_' + times + '" onchange="setNum(this)"></div></div></td>'
    + '<td class="col-sm-4"><div class="input-group formValue">'
    + '<input class="form-control text-center required number" name="rebate_' + ProIndex + '_' + times + '">'
    + '<span name="rebate_mode_input1_' + ProIndex +  '_' + times + '" class="input-group-addon no-border">元/台</span>'
    + '<span name="rebate_mode_input2_' + ProIndex +  '_' + times + '" class="input-group-addon no-border  no-padding"></span></div></td>'
    + '<td class="col-sm-2">'
    + '<i onclick="addRow(this)" data-index="' + times + '" name="addRow_' + ProIndex + '_' + times + '" class="fa fa-plus-circle fa-lg margin text-blue"></i>'
    + '</td></tr>'
    + '<tr style="display:table;width:100%;table-layout:fixed;">'
    + '<td class="col-sm-6"><div class="input-group formValue">'
    + '<input class="form-control text-center" name="target_min_' + ProIndex + '_' + times + '" onchange="setNum(this)">'
    + '<span class="input-group-addon no-border">-</span>'
    + '<input class="form-control text-center required number" name="target_max_' + ProIndex + '_' + times + '" onchange="setNum(this)"></div></td>'
    + '<td class="col-sm-4">'
    + '<div class="input-group formValue">'
    + '<input class="form-control text-center required number" name="rebate_' + ProIndex + '_' + times + '">'
    + '<span name="rebate_mode_input1_' + ProIndex +  '_' + times + '" class="input-group-addon no-border">元/台</span>'
    + '<span name="rebate_mode_input2_' + ProIndex +  '_' + times + '" class="input-group-addon no-border  no-padding"></span></div></td>'
    + '<td class="col-sm-2"><div class="row">'
    + '<i onclick="addRow(this)" data-index="' + times + '" name="addRow_' + ProIndex + '_' + times + '" class="fa fa-plus-circle fa-lg text-blue"></i>'
    + '<i onclick="deleteRow(this)" class="fa fa-minus-circle fa-lg text-red"></i></div></td></tr>'
    + '<tr style="display:table;width:100%;table-layout:fixed;">'
    + '<td class="col-sm-6"><div class="input-group">'
    + '<input class="form-control text-center" name="target_min_' + ProIndex + '_' + times + '"  onchange="setNum(this)">'
    + '<span class="input-group-addon no-border">-</span>'
    + '<input class="form-control text-center" name="target_max_' + ProIndex + '_' + times + '" disabled value="以上"></div></td>'
    + '<td class="col-sm-4"><div class="input-group formValue">'
    + '<input class="form-control text-center required number" name="rebate_' + ProIndex + '_' + times + '">'
    + '<span name="rebate_mode_input1_' + ProIndex +  '_' + times + '" class="input-group-addon no-border">元/台</span>'
    + '<span name="rebate_mode_input2_' + ProIndex +  '_' + times + '" class="input-group-addon no-border  no-padding"></span></div></td>'
    + '<td class="col-sm-2"></td></tr></tbody></table></div></div></div></div>';

    $($(e).parents("div[id=box1]")).append(c);//id = box1
    $($(e).parents("div[id^=proBox]")).find("input[id^=end_time_]:eq(-2)").val("");
    $($(e).parents("div[id^=proBox]")).find("input[id^=end_time_]:eq(-2)").removeAttr("disabled");
    DateShow();
    //添加时间分段时，机型选中后，型号取消disable
    if ($("input[name='product_scope" + ProIndex + "']:checked").val() == 2) {
        ProductScope(2);
        $("input[name^='target_mode_" + ProIndex + "_" + times + "'][value=4]").removeAttr("disabled"); // 返利模式-按型号 
    }
}
function delDiv(e) {
    var ProIndex = $(e).parents("div[id^=proBox]").data("index");
    var now_index = $(e).parents("div[id^=copy]").data("index");

    $(e).parents("div[id^=copy]").remove();
    $("input[id^=start_time_" + ProIndex + "_]").val("");
    $("input[id^=end_time_" + ProIndex + "_]").val("");
    DateShow();
}
function delProBox(e) {
    $(e).parents("div[id^=proBox]").remove();
    DateShow();
}

var proCount = 1;
$('span[name="addProduct"]').on("click", function () {
    proCount++;

    var productClone = '<div class="col-xs-12 col-sm-12 col-lg-12" id="proBox' + proCount + '" data-index="' + proCount + '">'
    + '<div class="box box-primary"><div class="box-header with-border" style="height:55px">  <h3 class="box-title">机型分段</h3>'
    + '<div class="box-tools pull-right"><button type="button" class="btn btn-box-tool" data-widget="collapse">'
    + '<i class="fa fa-minus"></i></button><button type="button" class="btn btn-box-tool" onclick="delProBox(this)"><i class="fa fa-remove"></i></button></div></div>'
    + '<div class="box-body" style="background-color:#ecf0f5" id="box1">'
    + '<div class="col-xs-12 col-sm-12 col-lg-6"><div class="col-xs-12 col-sm-12 col-lg-12 no-padding">'
    + '<div class="box box-widget"><div class="box-header with-border">'
    + '<h3 class="box-title">活动机型</h3>'
    + '<h6 id="productCountWrap' + proCount + '" class="inline margin" >共<span class="margin" id="product_count' + proCount + '">0</span>款</h6>'
    + '<div class="box-tools pull-right">'
    + '<button type="button" class="btn btn-box-tool" data-widget="collapse">'
    + '<i class="fa fa-minus"></i></button></div></div>'
    + '<div class="box-body"><div style="height:40px"><div class="radio col-sm-6">'
    + '<label><input name="product_scope' + proCount + '" disabled type="radio" value="1" checked onchange="productScopeChange()" onclick="getPIndex(this)" data-index="' + proCount + '">全部机型</label></div>'
    + '<div class="radio col-sm-6"><label onclick="$(\'#modalProduct' + proCount + '\').modal(\'show\');">'
    + '<input name="product_scope' + proCount + '" type="radio" checked value="2" onchange="productScopeChange()" data-index="' + proCount + '" onclick="getPIndex(this)">指定机型'
    + '</label></div></div>'
    + '<div id="targetProductWrap' + proCount + '" ><table id="targetProduct' + proCount + '"></table></div></div></div></div></div>'
    + '<div class="col-xs-12 col-sm-12 col-lg-6" id="copy_' + proCount + '_1"  data-index="1">'
    + '<div class="col-xs-12 col-sm-12 col-lg-12 no-padding"><div class="box box-widget">'
    + '<div class="box-header with-border"><div class="col-lg-12"><div class="col-lg-6">'
    + '<div class="input-group col-lg-12"><input class="form-control" id="start_time_' + proCount + '_1" disabled >'
    + '<span class="input-group-addon no-border"> 至 </span>'
    + '<input class="form-control" id="end_time_' + proCount + '_1"  readonly="readonly" onfocus="WdatePicker({ onpicked: DateSelect(this),minDate: \'#F{$dp.$D(\\\'start_time_' + proCount + '_1' + '\\\')}\', maxDate: \'#F{$dp.$D(\\\'end_date\\\')}\' });"></div></div>'
    + '<div class="input-group col-lg-6" name="addTimesDiv">'
    + '<span class="btn btn-primary" name="addTimes" onclick="addTimes(this)">添加时间分段</span></div></div>'
    + '<div class="box-tools pull-right" name="removeIcon">'
    + '<button type="button" class="btn btn-box-tool" data-widget="collapse">'
    + '<i class="fa fa-minus"></i></button></div></div>'
    + '<div class="box-body"><div class="col-md-12 col-lg-12 margin-bottom">'
    + '<div class="row"><div class="form-group col-sm-12"><label>返利模式</label>'
    + '<div><div class="radio col-sm-3">'
    + '<label><input name="target_mode_' + proCount + '_1" data-index="1" type="radio" value="3" onchange="Target_mode(this)" checked>按零售价</label></div>'
    + '<div class="radio col-sm-3">'
    + '<label><input name="target_mode_' + proCount + '_1" type="radio" data-index="1" value="5" onchange="Target_mode(this)" >按批发价</label></div>'
    + '<div class="radio col-sm-3">'
    + '<label><input name="target_mode_' + proCount + '_1" type="radio" data-index="1" value="4" disabled onchange="Target_mode(this)">按型号</label></div>'
    + '<div class="radio col-sm-3">'
    + '<label><input name="target_mode_' + proCount + '_1" type="radio" data-index="1" value="6" onchange="Target_mode(this)">无'
    + '</label></div></div></div></div>'
    + '<div class="form-group"><label>返利金额</label>'
    + '<div><div class="radio col-sm-3 col-lg-3">'
    + '<label><input name="rebate_mode_' + proCount + '_1" data-index="1" type="radio" value="1" checked onchange="RebateModeClick(this)">每台固定金额</label></div>'
    + '<div class="radio col-sm-3 col-lg-3">'
    + '<label><input name="rebate_mode_' + proCount + '_1" type="radio" data-index="1" value="2" onchange="RebateModeClick(this)">每台批发价比例</label></div>'
    + '<div class="radio col-sm-3 col-lg-3">'
    + '<label><input name="rebate_mode_' + proCount + '_1" type="radio" data-index="1" value="3" onchange="RebateModeClick(this)">每台零售价比例</label></div>'
    + '<div class="radio col-sm-3 col-lg-3">'
    + '<label><input name="rebate_mode_' + proCount + '_1" type="radio" data-index="1" value="4" onchange="RebateModeClick(this)" disabled> 固定总金额'
    + '</label></div></div></div></div><br />'
    + '<table id="rebateMode1_' + proCount + '_1" class="table table-bordered text-center">'
    + '<thead style="display:table;width:99%;table-layout:fixed;">'
    + '<tr><th class="col-sm-3"><span name="sale_change">完成率（%）</span></th>'
    + '<th class="col-sm-3"><span name="target_mode_change_' + proCount + '_1">零售价（元）</span></th>'
    + '<th class="col-sm-1"></th>'
    + '<th class="col-sm-3"><span name="rebate_mode_change_' + proCount + '_1">返利金额</span></th>'
    + '<th class="col-sm-2"></th></tr></thead>'
    + '<tbody style="display:block;height:273px;overflow-y:scroll;overflow-x:hidden">'
    + '<tr><td class="col-sm-3" style="text-align:center;vertical-align:middle">'
    + '<div class="input-group">'
    + '<input class="form-control text-center" name="nor_target_min_' + proCount + '_1" onchange="setNumNor(this)">'
    + '<span class="input-group-addon no-border">-</span>'
    + '<div class="formValue">'
    + '<input class="form-control text-center required number" name="nor_target_max_' + proCount + '_1" disabled value="以上">'
    + '</div></div></td>'
    + '<td><table class="table table-bordered text-center" name="small_table_' + proCount + '_1" style="margin-bottom:0px">'
    + '<tr><td class="col-sm-3"><div class="input-group">'
    + '<input class="form-control text-center" name="sma_target_min_' + proCount + '_1" onchange="setNumSmall(this)">'
    + '<span class="input-group-addon no-border">-</span>'
    + '<div class="formValue">'
    + '<input class="form-control text-center required number" name="sma_target_max_' + proCount + '_1" onchange="setNumSmall(this)">'
    + '</div></div></td>'
    + '<td class="col-sm-1" style="text-align:center;vertical-align:middle">'
    + '<i onclick="addSmall(this)" name="addSmall_' + proCount + '_1" data-index="1" class="fa fa-plus text-blue"></i>&nbsp;'
    + '<i onclick="delSmall(this)" class="fa fa-minus text-red"></i></td>'
    + '<td class="col-sm-3"><div class="input-group formValue">'
    + '<input class="form-control text-center required number" name="nor_rebate_' + proCount + '_1">'
    + '<span name="rebate_mode_input1_' + proCount + '_1" class="input-group-addon no-border">元/台</span>'
    + '<span name="rebate_mode_input2_' + proCount + '_1" class="input-group-addon no-border  no-padding"></span></div></td></tr>'
    + '<tr><td class="col-sm-3"><div class="input-group">'
    + '<input class="form-control text-center" name="sma_target_min_' + proCount + '_1" onchange="setNumSmall(this)">'
    + '<span class="input-group-addon no-border">-</span>'
    + '<div class="formValue">'
    + '<input class="form-control text-center required number" name="sma_target_max_' + proCount + '_1" disabled value="以上">'
    + '</div></div></td>'
    + '<td class="col-sm-1" style="text-align:center;vertical-align:middle">'
    + '<i onclick="addSmall(this)" name="addSmall_' + proCount + '_1" data-index="1" class="fa fa-plus text-blue"></i></td>'
    + '<td class="col-sm-3">'
    + '<div class="input-group formValue"><input class="form-control text-center required number" name="nor_rebate_' + proCount + '_1">'
    + '<span name="rebate_mode_input1_' + proCount + '_1" class="input-group-addon no-border">元/台</span>'
    + '<span name="rebate_mode_input2_' + proCount + '_1" class="input-group-addon no-border  no-padding"></span>'
    + '</div></td></tr></table></td>'
    + '<td class="col-sm-2" style="text-align:center;vertical-align:middle">'
    + '<i onclick="addNormal(this)" data-index="1" name="addNormal_' + proCount + '_1" class="fa fa-plus-circle fa-lg text-blue"></i>'
    + '</td></tr></tbody></table>'
    + '<table id="rebateMode4_' + proCount + '_1" data-index="1" class="table table-bordered text-center" hidden></table>'
    + '<table id="rebateMode6_' + proCount + '_1" class="table table-bordered text-center" hidden>'
    + '<thead style="display:table;width:99%;table-layout:fixed;">'
    + '<tr><th class="col-sm-6"><span name="sale_change">完成率（%）</span></th>'
    + '<th class="col-sm-4"><span name="rebate_mode_change_' + proCount + '_1">返利金额</span></th>'
    + '<th class="col-sm-2"></th></tr></thead>'
    + '<tbody style="display:block;height:273px;overflow-y:scroll;overflow-x:hidden">'
    + '<tr style="display:table;width:100%;table-layout:fixed;">'
    + '<td class="col-sm-6"><div class="input-group">'
    + '<input class="form-control text-center" name="target_min_' + proCount + '_1" disabled value="0">'
    + '<span class="input-group-addon no-border">-</span><div class="formValue">'
    + '<input class="form-control text-center required number" name="target_max_' + proCount + '_1" onchange="setNum(this)"></div></div></td>'
    + '<td class="col-sm-4"><div class="input-group formValue">'
    + '<input class="form-control text-center required number" name="rebate_' + proCount + '_1">'
    + '<span name="rebate_mode_input1_' + proCount + '_1" class="input-group-addon no-border">元/台</span>'
    + '<span name="rebate_mode_input2_' + proCount + '_1" class="input-group-addon no-border  no-padding"></span></div></td>'
    + '<td class="col-sm-2">'
    + '<i onclick="addRow(this)" data-index="1" name="addRow_' + proCount + '_1" class="fa fa-plus-circle fa-lg margin text-blue"></i>'
    + '</td></tr>'
    + '<tr style="display:table;width:100%;table-layout:fixed;">'
    + '<td class="col-sm-6"><div class="input-group formValue">'
    + '<input class="form-control text-center" name="target_min_' + proCount + '_1"   onchange="setNum(this)">'
    + '<span class="input-group-addon no-border">-</span>'
    + '<input class="form-control text-center required number" name="target_max_' + proCount + '_1" onchange="setNum(this)"></div></td>'
    + '<td class="col-sm-4">'
    + '<div class="input-group formValue">'
    + '<input class="form-control text-center required number" name="rebate_' + proCount + '_1">'
    + '<span name="rebate_mode_input1_' + proCount + '_1" class="input-group-addon no-border">元/台</span>'
    + '<span name="rebate_mode_input2_' + proCount + '_1" class="input-group-addon no-border  no-padding"></span></div></td>'
    + '<td class="col-sm-2"><div class="row">'
    + '<i onclick="addRow(this)" data-index="1" name="addRow_' + proCount + '_1" class="fa fa-plus-circle fa-lg text-blue"></i>'
    + '<i onclick="deleteRow(this)" class="fa fa-minus-circle fa-lg text-red"></i></div></td></tr>'
    + '<tr style="display:table;width:100%;table-layout:fixed;">'
    + '<td class="col-sm-6"><div class="input-group">'
    + '<input class="form-control text-center" name="target_min_' + proCount + '_1"  onchange="setNum(this)" >'
    + '<span class="input-group-addon no-border">-</span>'
    + '<input class="form-control text-center" name="target_max_' + proCount + '_1" disabled value="以上"></div></td>'
    + '<td class="col-sm-4"><div class="input-group formValue">'
    + '<input class="form-control text-center required number" name="rebate_' + proCount + '_1">'
    + '<span name="rebate_mode_input1_' + proCount + '_1" class="input-group-addon no-border">元/台</span>'
    + '<span name="rebate_mode_input2_' + proCount + '_1" class="input-group-addon no-border  no-padding"></span></div></td>'
    + '<td class="col-sm-2"></td></tr></tbody></table></div></div></div></div></div></div></div>';
    $("#bigBox").append(productClone);


    var modalClone = '<div class="modal fade" id="modalProduct' + proCount + '" tabindex="-1" role="dialog">'
    + ' <div class="modal-dialog modal-lg" role="document">'
    + ' <div class="modal-content">'
    + ' <div class="modal-header">'
    + ' <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>'
    + ' <h4 class="modal-title">请从左侧选择机型</h4></div>'
    + ' <div class="modal-body">'
    + ' <select multiple="multiple" size="10" id="duallistProduct' + proCount + '"  data-index="' + proCount + '"></select></div>'
    + ' <div class="modal-footer">'
    + ' <button type="button" class="btn btn-primary" data-dismiss="modal">确定</button>'
    + ' </div></div> </div></div>';
    $("#modalProduct").after(modalClone);
    $('#duallistProduct' + proCount).bootstrapDualListbox(bootstrapProductOpt);
    initTable($('table[id=targetProduct' + proCount + ']'), tableProductHeader);
    $("#modalProduct" + proCount).on('hide.bs.modal', ModalProductHide);
    $.each(pRestList, function (index, value) {
        //value.main_id = mainGuid;
        $('#duallistProduct' + proCount).append("<option value='" + value.id + "'>" + value.model + "(" + value.color + ")" + "</option>");
    })
    $('#duallistProduct' + proCount).bootstrapDualListbox("refresh");
    pSelListArr.push([]);
    timesArr.push(1);
    DateShow();



})
//返利模式
function Target_mode(e) {
    nowIndex = $(e).data('index');
    ProIndex = $(e).parents("div[id^=proBox]").data("index");
    checkTargetVal = $('input[name="target_mode_' + ProIndex + '_' + nowIndex + '"]:checked').val();//返利模式
    if (checkTargetVal == 3) {
        // 按零售价
        $("span[name='target_mode_change_" + ProIndex + "_" + nowIndex + "']").html("零售价（元/台）");
        $("input[name='rebate_mode_" + ProIndex + "_" + nowIndex + "'][value='4']").attr("disabled", "disabled");
        $("#rebateMode1_" + ProIndex + "_" + nowIndex + "").show();
        $("#rebateMode4_" + ProIndex + "_" + nowIndex + "").hide();
        $("#rebateMode6_" + ProIndex + "_" + nowIndex + "").hide();
    } else if (checkTargetVal == 4) {
        //型号
        $("input[name='rebate_mode_" + ProIndex + "_" + nowIndex + "'][value='4']").attr("disabled", "disabled");
        $("#rebateMode1_" + ProIndex + "_" + nowIndex + "").hide();
        $("#rebateMode6_" + ProIndex + "_" + nowIndex + "").hide();
        $("#rebateMode4_" + ProIndex + "_" + nowIndex + "").show();
        Rebate_mode(e);
    } else if (checkTargetVal == 5) {
        // 按批发价
        $("span[name='target_mode_change_" + ProIndex + "_" + nowIndex + "']").html("批发价（元/台）");
        $("input[name='rebate_mode_" + ProIndex + "_" + nowIndex + "'][value='4']").attr("disabled", "disabled");
        $("#rebateMode1_" + ProIndex + "_" + nowIndex + "").show();
        $("#rebateMode4_" + ProIndex + "_" + nowIndex + "").hide();
        $("#rebateMode6_" + ProIndex + "_" + nowIndex + "").hide();
    } else if (checkTargetVal == 6) {
        // 无
        $("#rebateMode1_" + ProIndex + "_" + nowIndex + "").hide();
        $("#rebateMode4_" + ProIndex + "_" + nowIndex + "").hide();
        $("#rebateMode6_" + ProIndex + "_" + nowIndex + "").show();
        $("input[name='rebate_mode_" + ProIndex + "_" + nowIndex + "'][value='4']").removeAttr("disabled");
        Rebate_mode(e);
    }
}
//返利金额
function RebateModeClick(e) {
    nowIndex = $(e).data('index');
    ProIndex = $(e).parents("div[id^=proBox]").data("index");
    Rebate_mode(e);
}
//返利金额
function Rebate_mode(e) {
    //nowIndex = $(e).data('index');
    //pIndex = $(e).parents("div[id^=proBox]").data("index");
    checkRebateVal = $('input[name="rebate_mode_' + ProIndex + '_' + nowIndex + '"]:checked').val();//奖励金额
    if (checkRebateVal == 1) {
        $("span[name='rebate_mode_change_" + ProIndex + "_" + nowIndex + "']").html("返利金额");
        $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='3']").removeAttr("disabled");
        $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='5']").removeAttr("disabled");
        if (checkProductVal == 2) {
            $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='4']").removeAttr("disabled"); // 奖励模式-按型号
        }
        str2 = "元/台";
        str4 = "";
    } else if (checkRebateVal == 2) {
        $("span[name='rebate_mode_change_" + ProIndex + "_" + nowIndex + "']").html("批发价比例");
        $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='3']").removeAttr("disabled");
        $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='5']").removeAttr("disabled");
        if (checkProductVal == 2) {
            $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='4']").removeAttr("disabled"); // 奖励模式-按型号
        }
        str2 = "%";
        str4 = "*批发价";
    } else if (checkRebateVal == 3) {
        $("span[name='rebate_mode_change_" + ProIndex + "_" + nowIndex + "']").html("零售价比例");
        $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='3']").removeAttr("disabled");
        $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='5']").removeAttr("disabled");
        if (checkProductVal == 2) {
            $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='4']").removeAttr("disabled"); // 奖励模式-按型号
        }
        str2 = "%";
        str4 = "*零售价";
    } else if (checkRebateVal == 4) {//固定金额只能选无
        $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='3']").attr("disabled", "disabled");
        $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='4']").attr("disabled", "disabled");
        $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='5']").attr("disabled", "disabled");
        $("input[name='target_mode_" + ProIndex + "_" + nowIndex + "'][value='6']").prop("checked", "checked");
        //活动机型 

        $("span[name='rebate_mode_change_" + ProIndex + "_" + nowIndex + "']").html("固定金额");
        str2 = "元";
        str4 = "";
    }
    $("span[name='rebate_mode_input1_" + ProIndex + "_" + nowIndex + "']").html(str2);
    $("span[name='rebate_mode_input2_" + ProIndex + "_" + nowIndex + "']").html(str4);
}
//新增一行
function addRow(e) {//无
    ProIndex = $(e).parents("div[id^=proBox]").data("index");
    nowIndex = $(e).data('index');
    Rebate_mode(e);
    var str01 = str1.format(ProIndex + '_' + nowIndex);
    var str03 = str3.format(ProIndex + '_' + nowIndex);
    var str05 = str5.format(ProIndex + '_' + nowIndex, nowIndex);
    $(e).parents('tr').after(str01 + str2 + str03 + str4 + str05);
    index = $(e).closest('tr')[0].rowIndex;
    setNum(e);//添加行 取消数据的配对 因为 点击事件
    //emptyNum();
}

function addSmall(e) {//一般 子表格
    ProIndex = $(e).parents("div[id^=proBox]").data("index");
    nowIndex = $(e).data('index');
    Rebate_mode(e);
    var str07 = str7.format(ProIndex + '_' + nowIndex, nowIndex);
    var str08 = str8.format(ProIndex + '_' + nowIndex);
    $($(e).closest('tbody')[0]).children('tr:last').before(str07 + str2 + str08 + str4 + str9);
}

function addNormal(e) {//一般 父表格
    ProIndex = $(e).parents("div[id^=proBox]").data("index");
    nowIndex = $(e).data('index');
    Rebate_mode(e);
    var str011 = str11.format(ProIndex + '_' + nowIndex, nowIndex);
    var str012 = str12.format(ProIndex + '_' + nowIndex);
    var str013 = str13.format(ProIndex + '_' + nowIndex, nowIndex);
    var str014 = str14.format(ProIndex + '_' + nowIndex);
    var str015 = str15.format(ProIndex + '_' + nowIndex, nowIndex);
    $($($('#rebateMode1_' + ProIndex + '_' + nowIndex ).children('tbody')[0]).children('tr:last')[0]).before(str011 + str2 + str012 + str4 + str013 + str2 + str014 + str4 + str015);

}

function delSmall(e) {//删除一般
    $(e).closest('tr').remove();
}

function delNormal(e) {//删除一般
    $(e).parents('tr').remove();
}
//型号新增一个tr   
function addTr(e) {
    var trTemp = '';
    ProIndex = $(e).parents("div[id^=proBox]").data("index");//机型
    nowIndex = $(e).data('index');//时间
    sum = pSelListArr[ProIndex - 1].length;
    trTemp = GetTrProduct(nowIndex, true);
    var times = timesArr[ProIndex - 1];
    var s1 = new RegExp("rebate_mode_input1_" + ProIndex + "_" + times, "g");
    var s2 = new RegExp("rebate_mode_input2_" + ProIndex + "_" + times, "g");
    var s3 = new RegExp("pro_target_min_" + ProIndex + "_" + times, "g");
    var s4 = new RegExp("pro_target_max_" + ProIndex + "_" + times, "g");
    var s5 = new RegExp("rebate_p_" + ProIndex + "_" + times, "g");
    trTemp = trTemp.replace(s1, "rebate_mode_input1_" + ProIndex + "_" + nowIndex);
    trTemp = trTemp.replace(s2, "rebate_mode_input2_" + ProIndex + "_" + nowIndex);
    trTemp = trTemp.replace(s3, "pro_target_min_" + ProIndex + "_" + nowIndex);
    trTemp = trTemp.replace(s4, "pro_target_max_" + ProIndex + "_" + nowIndex);
    trTemp = trTemp.replace(s5, "rebate_p_" + ProIndex + "_" + nowIndex);
    trTemp = trTemp.replace("data-index='" + nowIndex + "'", "data-index='" + nowIndex + "'");
    trTemp = trTemp.replace("addTr_" + ProIndex + "_" + times, "addTr_" + ProIndex + "_" + nowIndex);
    $($(e).parents('tbody').find('tr:eq(' + (-sum) + ')')).before(trTemp);//固定添加到倒数第二行
    Rebate_mode(e);
}
//删除Tr
function deleteTr(e) {
    var del_index = $(e).closest('tr')[0].rowIndex;
    for (var i = del_index + sum - 1; i >= del_index ; i--) {
        $(e).parents('tbody').find('tr:eq(' + (i - 1) + ')').remove();
    }
}
//删除
function deleteRow(e) {
    ProIndex = $(e).parents("div[id^=proBox]").data("index");
    nowIndex = $(e).parents("div[id^=copy]").data("index");
    $(e).parents('tr').remove();
    setNum(e);
}
//input输入值 1.小数不行 返利模式：无
function setNum(e) {
    ProIndex = $(e).parents("div[id^=proBox]").data("index");
    nowIndex = $(e).parents("div[id^=copy]").data("index");
    if ($(e)[0].name == "target_min_" + ProIndex + "_" + nowIndex) {//点击的是min
        var i = $("input[name='target_min_" + ProIndex + "_" + nowIndex + "']").index(e);//获取该行行号
        if (i != 0) {
            if (Number($($("input[name='target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val()) <= Number($($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i - 1]).val())) {//输入的值大于max时
                $.modalAlert("该值应大于" + $($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i - 1]).val() + "！");
                $($("input[name='target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val('');
                return;
            }
        }
        if ($($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val() != '') {//当前行max不为空时
            if (Number($($("input[name='target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val()) >= Number($($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val())) {//输入的值大于max时
                $.modalAlert("该值应小于" + $($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val() + "！");
                $($("input[name='target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val('');
                return;
            }
        }
    } else if ($(e)[0].name == "target_max_" + ProIndex + "_" + nowIndex) {//点击的是max
        var i = $("input[name='target_max_" + ProIndex + "_" + nowIndex + "']").index(e);

        if (Number($($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) > Number($($("input[name='target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val())) {
            $($("input[name='target_min_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val(Number($($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) + Number(1));
        } else {
            $($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val('');
            $.modalAlert("该值应大于" + $($("input[name='target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val() + "！");
        }

        if ($($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val() != '') {
            if (Number($($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) > Number($($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val() - 2)) {
                $($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val('');
                $.modalAlert("该值应大于" + (Number($($("input[name='target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) + Number(1)) + "！");
            }
        }
    }
}
// 返利模式：按零售价 按批发价
function setNumNor(e) {
    ProIndex = $(e).parents("div[id^=proBox]").data("index");
    nowIndex = $(e).parents("div[id^=copy]").data("index");
    if ($(e)[0].name == "nor_target_min_" + ProIndex +"_"+ nowIndex) {//点击的是min
        var i = $("input[name='nor_target_min_"+ ProIndex +"_"+ nowIndex+"']").index(e);//获取该行行号
        if (i != 0) {
            if (Number($($("input[name='nor_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val()) <= Number($($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i - 1]).val())) {//输入的值大于max时
                $.modalAlert("该值应大于" + $($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i - 1]).val() + "！");
                $($("input[name='nor_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val('');
                return;
            }
        }
        if ($($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val() != '') {//当前行max不为空时
            if (Number($($("input[name='nor_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val()) >= Number($($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val())) {//输入的值大于max时
                $.modalAlert("该值应小于" + $($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val() + "！");
                $($("input[name='nor_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val('');
                return;
            }
        }
    } else if ($(e)[0].name == "nor_target_max_" + ProIndex + "_" + nowIndex) {//点击的是max
        var i = $("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']").index(e);

        if (Number($($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) > Number($($("input[name='nor_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val())) {
            $($("input[name='nor_target_min_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val(Number($($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) + Number(1));
        } else {
            $($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val('');
            $.modalAlert("该值应大于" + $($("input[name='nor_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val() + "！");
        }

        if ($($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val() != '') {
            if (Number($($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) > Number($($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val() - 2)) {
                $($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val('');
                $.modalAlert("该值应大于" + (Number($($("input[name='nor_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) + Number(1)) + "！");
            }
        }
    }
}
// 返利模式：按型号
function setNumPro(e) {
    ProIndex = $(e).parents("div[id^=proBox]").data("index");
    nowIndex = $(e).parents("div[id^=copy]").data("index");
    if ($(e)[0].name == "pro_target_min_" + ProIndex + "_" + nowIndex + "") {//点击的是min
        var i = $("input[name='pro_target_min_" + ProIndex + "_" + nowIndex + "']").index(e);//获取该行行号
        if (i != 0) {
            if (Number($($("input[name='pro_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val()) <= Number($($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i - 1]).val())) {//输入的值大于max时
                $.modalAlert("该值应大于" + $($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i - 1]).val() + "！");
                $($("input[name='pro_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val('');
                return;
            }
        }
        if ($($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val() != '') {//当前行max不为空时
            if (Number($($("input[name='pro_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val()) >= Number($($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val())) {//输入的值大于max时
                $.modalAlert("该值应小于" + $($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val() + "！");
                $($("input[name='pro_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val('');
                return;
            }
        }
    } else if ($(e)[0].name == "pro_target_max_" + ProIndex + "_" + nowIndex + "") {//点击的是max
        var i = $("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']").index(e);

        if (Number($($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) > Number($($("input[name='pro_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val())) {
            $($("input[name='pro_target_min_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val(Number($($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) + Number(1));
        } else {
            $($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val('');
            $.modalAlert("该值应大于" + $($("input[name='pro_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val() + "！");
        }

        if ($($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val() != '') {
            if (Number($($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) > Number($($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val() - 2)) {
                $($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val('');
                $.modalAlert("该值应大于" + (Number($($("input[name='pro_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) + Number(1)) + "！");
            }
        }
    }
}
// 返利模式：按零售价 按批发价 中小的
function setNumSmall(e) {
    ProIndex = $(e).parents("div[id^=proBox]").data("index");
    nowIndex = $(e).parents("div[id^=copy]").data("index");
    if ($(e)[0].name == "sma_target_min_" + ProIndex + "_" + nowIndex) {//点击的是min
        var i = $("input[name='sma_target_min_" + ProIndex + "_" + nowIndex + "']").index(e);//获取该行行号
        if (i != 0) {
            if (Number($($("input[name='sma_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val()) <= Number($($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i - 1]).val())) {//输入的值大于max时
                $.modalAlert("该值应大于" + $($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i - 1]).val() + "！");
                $($("input[name='sma_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val('');
                return;
            }
        }
        if ($($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val() != '') {//当前行max不为空时
            if (Number($($("input[name='sma_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val()) >= Number($($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val())) {//输入的值大于max时
                $.modalAlert("该值应小于" + $($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val() + "！");
                $($("input[name='sma_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val('');
                return;
            }
        }
    } else if ($(e)[0].name == "sma_target_max_" + ProIndex + "_" + nowIndex + "") {//点击的是max
        var i = $("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']").index(e);

        if (Number($($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) > Number($($("input[name='sma_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val())) {
            $($("input[name='sma_target_min_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val(Number($($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) + Number(1));
        } else {
            $($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val('');
            $.modalAlert("该值应大于" + $($("input[name='sma_target_min_" + ProIndex + "_" + nowIndex + "']")[i]).val() + "！");
        }

        if ($($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val() != '') {
            if (Number($($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) > Number($($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val() - 2)) {
                $($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i + 1]).val('');
                $.modalAlert("该值应大于" + (Number($($("input[name='sma_target_max_" + ProIndex + "_" + nowIndex + "']")[i]).val()) + Number(1)) + "！");
            }
        }
    }
}
//清空 
//function emptyNum() {
//    for (var j = index + 2; j < $("#rebateMode6 tr").length; j++) {
//        $('#rebateMode6 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
//        $('#rebateMode6 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("");
//        if (j == $("#rebateMode6 tr").length - 1) {
//            $('#rebateMode6 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
//            $('#rebateMode6 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("以上");
//        }
//    }
//}

// Dual List Box START
$("#modalDistributor").on('hide.bs.modal', function (e) {
    dSelList.splice(0, dSelList.length);    // 清空数组
    var selIds = $duallistDistributor.val();
    $.each(dFullList, function (i, item) {
        if ($.inArray(item['id'], selIds) > -1)
            dSelList.push(item);
    });
    $tableDistributor.bootstrapTable('load', dSelList);
    $("#distributor_count").text(dSelList.length);
    SaleAvgBefore();

})

function ModalProductHide() {
    
    pSelList = pSelListArr[ProIndex - 1];
    if (pSelList.length > 0) {
        pRestList = pRestList.concat(pSelList);
        pSelList.splice(0, pSelList.length);
    }
    var selIds = $('#duallistProduct' + ProIndex).val();
    var len = pRestList.length,j = 0, i = 0;
    for (; i < len ; i++) {
        if ($.inArray(pRestList[j]['id'] + "", selIds) > -1) {
            pSelList.push(pRestList[j]);
            pRestList.splice(j, 1);
        } else {
            j++;
        }
    }
    var duallistProductList = $('select[id^=duallistProduct]');
    var dua_len = duallistProductList.length;
    i = 0;
    for (; i < dua_len; i++) {
        if (duallistProductList[i].attributes["data-index"].value == ProIndex) {
            continue;
        }
        $(duallistProductList[i]).empty();
        $.each(pRestList, function (index, value) {
            //value.main_id = mainGuid;
            $(duallistProductList[i]).append("<option value='" + value.id + "'>" + value.model + "(" + value.color + ")" + "</option>");
        })
        var pOrigList = pSelListArr[i];
        $.each(pOrigList, function (index, value) {
            //value.main_id = mainGuid;
            $(duallistProductList[i]).append("<option selected value='" + value.id + "'>" + value.model + "(" + value.color + ")" + "</option>");
        })
        $(duallistProductList[i]).bootstrapDualListbox("refresh");
    }
    $('table[id=targetProduct' + ProIndex + ']').bootstrapTable('load', pSelList);
    $("#product_count" + ProIndex + "").text(pSelList.length);
    $('table[id=targetProduct' + ProIndex + ']').show();
    productScopeChange();
    ProductScope(1);
    $("#productCountWrap" + ProIndex + "").show();
}
$("div[id^=modalProduct]").on('hide.bs.modal', ModalProductHide);
// Dual List Box END

// Table START
function TablesInit() {
    initTable($tableDistributor, tableDistributorHeader);
    $tableDistributor.bootstrapTable('load', dFullList);
    $("#distributor_count").text(dFullList.length);
    initTable($tableProduct, tableProductHeader);
    //$tableProduct.bootstrapTable('load', pFullList);
    //$("#product_count").text(pFullList.length);
}

var tableDistributorHeader = [
    { field: "id", visible: false, },
    { field: "name", title: "经销商", align: "center" },
    { field: "area_l1_name", title: "经理片区", align: "center", },
    { field: "company_name", title: "分公司", align: "center", },
]

var tableProductHeader = [
    { field: "id", visible: false, },
    { field: "model", title: "型号", align: "center" },
    { field: "color", title: "颜色", align: "center", },
    { field: "price_wholesale", title: "批发价（元/台）", align: "center", },
]

function initTable($table, tableHeader) {
    //1.初始化Table
    var oTable = new TableInit($table, tableHeader);
    oTable.Init();
}

var TableInit = function ($table, tableHeader) {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            height: 451,
            url: "",        //请求后台的URL（*）
            striped: false,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: false,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "client",           //分页方式：client客户端分页，server服务端分页（*）
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableHeader,
        });
    };
    return oTableInit;
};
// Table END

function submitForm() {
    // 提交验证
    if (!$("#form1").formValid())
        return false;

    var data = {};
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data['id'] = mainGuid;
    data['name'] = $("#name").val();
    data['company_id'] = $("#company").val();
    data['company_name'] = $("#company option:selected").text();
    data['start_date'] = $("#start_date").val();
    data['end_date'] = $("#end_date").val();
    data['distributor_count'] = $("#distributor_count").html();//经销商家数
    //data['product_scope'] = checkProductVal;//活动机型
    data['target_content'] = $('input[name="target_content"]:checked').val();//计量方式
    data['pay_mode'] = checkTypeVal;//返利发放
    //data['pay_mode'] = 1;//返利发放
    data['target_sale'] = checkSaleVal;//销量阶梯
    //data['target_mode'] = checkTargetVal;//返利模式
    //data['rebate_mode'] = checkRebateVal;//返利金额
    data['activity_target'] = $("#activity_target").val();
    data['money_included'] = $("#money_included").prop("checked") == true ? 0 : 1;
    data['note'] = $("#note").val();
    data["sale_avg_before"] = sale_avg;//前三个月目标销量
    data["category"] = $("#category").val();//返利类型

    //先找到proBox
    var productSecList = [], productList = [], rebateList = [], timeSecList = [], rebateProList = [];
    var rebate_i = 1;
    var time_sec_i = 1;
    var proBoxList = $("div[id^=proBox]");
    var proBoxLen = proBoxList.length;
    for (var i = 0; i < proBoxLen ; i++) {
        var pro_index = $(proBoxList[i]).data("index");//probox的data-index
        var productSec = {};
        productSec.main_id = mainGuid;
        productSec.product_sec_i = i + 1;
        productSec.product_scope = $("input[name=product_scope" + pro_index + "]:checked").val();
        if (productSec.product_scope == 2) {//指定机型
            var pList = pSelListArr[pro_index - 1];
            if (pList.length < 1) {
                $.modalAlert("指定机型请至少选择一个机型！")
                return false;
            } 
            $.each(pList, function (pIndex, pItem) {
                pItem.main_id = mainGuid;
                pItem.product_sec_i = i + 1;
            })
            productList = productList.concat(pList);
        }
        //时间
        var copyList = $("div[id^=copy_" + pro_index + "]");
        var copyLen = copyList.length;
        for (var j = 0; j < copyLen; j++, time_sec_i++) {
            var timeSec = {};
            var copy_index = $(copyList[j]).data("index");//copy的data-index
            timeSec.rebate_i = rebate_i;
            timeSec.main_id = mainGuid;
            timeSec.product_sec_i = i + 1;
            timeSec.time_sec_i = time_sec_i;
            timeSec.target_mode = $("input[name=target_mode_" + pro_index + "_" + copy_index + "]:checked").val();
            timeSec.rebate_mode = $("input[name=rebate_mode_" + pro_index + "_" + copy_index + "]:checked").val();
            timeSec.start_date = $("#start_time_" + pro_index + "_" + copy_index).val();
            timeSec.end_date = $("#end_time_" + pro_index + "_" + copy_index).val();
            if (timeSec.start_date == "" || timeSec.end_date == "") {
                $.modalAlert("时间分段填写不完整！");
                return;
                

            }
            if (timeSec.target_mode == 3 || timeSec.target_mode == 5) {//返利模式：按零售价/按批发价 
                var minInputList = $("input[name='nor_target_min_" + pro_index + "_" + copy_index + "']");
                var maxInputList = $("input[name='nor_target_max_" + pro_index + "_" + copy_index + "']");
                var minSmallList = $("input[name='sma_target_min_" + pro_index + "_" + copy_index + "']");
                var maxSmallList = $("input[name='sma_target_max_" + pro_index + "_" + copy_index + "']");
                var rebInputList = $("input[name='nor_rebate_" + pro_index + "_" + copy_index + "']");

                var smallTable = $("table[name='small_table_" + pro_index + "_" + copy_index + "']");
                var len = minInputList.length;
                var index = 0;
                for (var k = 0; k < len; k++, rebate_i++) {
                    var rebate = {};
                    rebate.rebate_i = rebate_i;
                    rebate.main_id = mainGuid;
                    rebate.time_sec_i = time_sec_i;
                    rebate.target_min = $(minInputList[k]).val();
                    rebate.target_max = $(maxInputList[k]).val() == '以上' ? "-1" : $(maxInputList[k]).val();
                    rebateList.push(rebate);
                    var smaLen = $(smallTable[k]).find('tr').length;
                    for (var l = 0; l < smaLen ; l++) {
                        var sma_rebate = {};
                        sma_rebate.main_id = mainGuid;
                        sma_rebate.rebate_i = rebate_i;
                        sma_rebate.target_min = $(minSmallList[index]).val();
                        sma_rebate.target_max = $(maxSmallList[index]).val() == '以上' ? "-1" : $(maxSmallList[index]).val();
                        sma_rebate.rebate = $(rebInputList[index]).val();
                        if ($(minInputList[k]).val() == '' || $(maxInputList[k]).val() == '' || $(minSmallList[index]).val() == '' || $(maxSmallList[index]).val() == '' || $(rebInputList[index]).val() == '') {
                            $.modalAlert("返利信息填写不完整！");
                            return;
                        }
                        rebateProList.push(sma_rebate);
                        index++;
                    }
                }
            } else if (timeSec.target_mode == 4) {//返利模式：按型号
                var minProductList = $("input[name='pro_target_min_" + pro_index + "_" + copy_index + "']");
                var maxProductList = $("input[name='pro_target_max_" + pro_index + "_" + copy_index + "']");
                var rebProductList = $("input[name='rebate_p_" + pro_index + "_" + copy_index + "']");
                var curProList = pSelListArr[pro_index - 1];
                var curProLen = curProList.length;
                var model_pro = $("td[name='model_pro']");
                var color_pro = $("td[name='color_pro']");
                var len = minProductList.length;
                var index = 0;
                for (var k = 0; k < len; k++, rebate_i++) {
                    var product = {};
                    product.rebate_i = rebate_i;
                    product.main_id = mainGuid;
                    product.time_sec_i = time_sec_i;
                    product.target_min = $(minProductList[k]).val();
                    product.target_max = $(maxProductList[k]).val() == '以上' ? "-1" : $(maxProductList[k]).val();
                    rebateList.push(product);
                    for (var l = 0; l < curProLen; l++) {
                        var sma_rebate = {};
                        sma_rebate.main_id = mainGuid;
                        sma_rebate.rebate_i = rebate_i;
                        sma_rebate.model = curProList[l].model;
                        sma_rebate.color = curProList[l].color;
                        sma_rebate.rebate = $(rebProductList[index]).val();
                        if ($(minProductList[k]).val() == '' || $(maxProductList[k]).val() == '' || $(rebProductList[index]).val() == '') {
                            $.modalAlert("返利信息填写不完整！");
                            return;
                        }
                        rebateProList.push(sma_rebate);
                        index++;
                    }
                }
            } else if (timeSec.target_mode == 6) {//返利模式：无
                var minInputList = $("input[name='target_min_" + pro_index + "_" + copy_index + "']");
                var maxInputList = $("input[name='target_max_" + pro_index + "_" + copy_index + "']");
                var rebInputList = $("input[name='rebate_" + pro_index + "_" + copy_index + "']");
                var len = minInputList.length;

                for (var k = 0; k < len; k++, rebate_i++) {
                    var rebateNo = {};
                    rebateNo.rebate_i = rebate_i;
                    rebateNo.main_id = mainGuid;
                    rebateNo.time_sec_i = time_sec_i;
                    rebateNo.target_min = $(minInputList[k]).val();
                    rebateNo.rebate = $(rebInputList[k]).val();
                    if (k == len - 1)
                        rebateNo.target_max = -1; //以上
                    else
                        rebateNo.target_max = $(maxInputList[k]).val();
                    var rebateValue = {};
                    if ($(maxInputList[k]).val() == '' || $(rebInputList[k]).val() == '') {
                        $.modalAlert("返利信息填写不完整！");
                        return;
                    }
                    rebateList.push(rebateNo);
                }
            }
            timeSecList.push(timeSec);
            //rebate_i++;
        }
        productSecList.push(productSec);
    }
    //经销商
    if (!dSelList || dSelList.length < 1) {
        $.modalAlert("请选择经销商");
        return;
    }
    data['distributorList'] = dSelList;

    data['rebateList'] = rebateList;
    data['rebateProList'] = rebateProList;
    data['productSecList'] = productSecList;
    data['productList'] = productList;
    data['timeSecList'] = timeSecList;
      
    //if (checkProductVal == 2) {
    //    if (!pSelList || pSelList.length < 1) {
    //        $.modalAlert("请选择型号");
    //        return;
    //    }
    //}
    //data['productList'] = pSelList;
    $.submitForm({
        url: "/DistributorManage/Attaining/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/DistributorManage/Attaining/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

