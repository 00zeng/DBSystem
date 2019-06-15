var myCompanyInfo = top.clients.loginInfo.companyInfo;
var checkTargetVal = $('input[name="target_mode"]:checked').val();//返利模式
var checkRebateVal = $('input[name="rebate_mode"]:checked').val();//金额计算模式
var checkProductVal = 2;//活动机型
var guid = "";
var distributorTree = [];
var areaFullList = [];
var dFullList = []; // 所有经销商
var dSelList = [];  // 已选经销商
var pFullList = [];   // 所有机型
var pSelList = [];    // 已选机型
var $tableDistributor = $('#targetDistributor');
var $tableProduct = $('#targetProduct');
var $tableRebateProduct = $('#rebateProduct');  // 按型号返利
var $pCountWrap = $("#productCountWrap");
var $pTableWrap = $("#targetProductWrap");
var index;//行号
var str1 = "<tr style='display: table;width: 100%;table-layout: fixed;'>"
        + "<td class='col-sm-6'><div class='input-group'><input class='form-control text-center' name='target_min' disabled>"
        + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center' name='target_max' onchange='setNum()'>"
        + "<td class='col-sm-4'><div class='input-group'><input class='form-control text-center' name='rebate'></div></td>"
        + "<td class='col-sm-2'><div class='row'><i onclick='addRow(this)' class='fa fa-plus-circle fa-lg  text-blue '></i> <i onclick='deleteRow(this)' class='fa fa-minus-circle fa-lg text-red'></i></div></td>"
        + "</tr>";
//型号的th
var str6 = '<tr><th class="col-sm-3">型号</th><th class="col-sm-3">颜色</th><th class="col-sm-3">批发价（元/台）</th><th class="col-sm-3"><span name="rebate_mode_change">返利金额</span></th></tr>';
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
var $duallistProduct = $('#duallistProduct').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入型号、颜色关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个机型",
    infoTextEmpty: "共0个机型",
    infoTextFiltered: "",
    filterTextClear: "清空"
});


$(function () {
    $(window).resize(function () {
        $tableDistributor.bootstrapTable('resetView', {
            height: 300
        });
        $tableProduct.bootstrapTable('resetView', {
            height: 300
        });
    });
    $pTableWrap.hide();
    $pCountWrap.hide();
    // $tableRebateProduct.hide();
    TablesInit();
    GetGUID();
    BindCompany();
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
                            d.main_id = guid;
                            d.distributor_id = d.id;
                            d.distributor_name = d.name;
                            dFullList.push(d);
                            dSelList.push(d);
                            $duallistDistributor.append("<option value='" + value1.id + "'>" + value1.display_info + "</option>");
                        });
                    }
                })
            }
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
            $.each(data, function (index, value) {
                value.main_id = guid;
                $duallistProduct.append("<option value='" + value.id + "'>" + value.model + "(" + value.color + ")" + "</option>");
            })

            $duallistProduct.bootstrapDualListbox("refresh");
            $tableProduct.bootstrapTable('load', pSelList);
            $("#product_count").text(pSelList.length);

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}


//返利模式
$('input[name="target_mode"]').on("change", function () {
    checkTargetVal = $('input[name="target_mode"]:checked').val();//返利模式
    if (checkTargetVal != 1)
        $("#forTargetMode1").hide();
    if (checkTargetVal == 1) {
        $("span[name='target_mode_change']").html("完成率（%）");
        // 按完成率
        $("input[name='rebate_mode'][value='4']").removeAttr("disabled");
        $("#forTargetMode1").show();
        //$("#rebateMode1").val("");
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 2) {
        // 按台数
        $("span[name='target_mode_change']").html("销量（台）");
        $("input[name='rebate_mode'][value='4']").removeAttr("disabled");
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 3) {
        // 按零售价
        $("span[name='target_mode_change']").html("零售价（元/台）");
        $("input[name='rebate_mode'][value='4']").attr("disabled", "disabled");
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 5) {
        // 按零售价
        $("span[name='target_mode_change']").html("批发价（元/台）");
        $("input[name='rebate_mode'][value='4']").attr("disabled", "disabled");
        $("#rebateMode1").show();
        $("#rebateMode4").hide();

    } else if (checkTargetVal == 4) {
        // 按型号
        $("input[name='rebate_mode'][value='1']").prop("checked", "checked");
        $("input[name='rebate_mode'][value='4']").attr("disabled", "disabled");
        $("#rebateMode4").show();
        $("#rebateMode1").hide();
        ProductScope();
    }
})


//新增一行
function addRow(e) {
    $(e).parents('tr').after(str1);
    index = $(e).closest('tr')[0].rowIndex;
    setNum();
    emptyNum();
}


//删除
function deleteRow(e) {
    $(e).parents('tr').remove();
    setNum();
}
//input输入值 1.小数不行
function setNum() {
    for (var i = 2; i < $("#rebateMode1 tr").length; i++) {
        if (i != $("#rebateMode1 tr").length - 1) {
            if (parseInt($('#rebateMode1 tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(0)').val()) >= parseInt($('#rebateMode1 tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(1)').val())) {
                $.modalAlert("该值应大于" + $('#rebateMode1 tr:eq(' + i + ') td:eq(0) input:eq(0)').val() + "！");
                $('#rebateMode1 tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
        var max = $('#rebateMode1 tr:eq(' + (i - 1) + ') td:eq(0) input:eq(1)').val();
        if (max != '') {
            $('#rebateMode1 tr:eq(' + i + ') td:eq(0) input:eq(0)').val(parseInt(max) + 1);
            if (parseInt($('#rebateMode1 tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(0)').val()) >= parseInt($('#rebateMode1 tr:eq(' + i + ') td:eq(0) div:eq(0) input:eq(1)').val())) {
                $('#rebateMode1 tr:eq(' + i + ') td:eq(0) input:eq(1)').val('');
            }
        }
    }
}

function emptyNum() {
    for (var j = index + 2; j < $("#rebateMode1 tr").length; j++) {
        $('#rebateMode1 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
        $('#rebateMode1 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("");
        if (j == $("#rebateMode1 tr").length - 1) {
            $('#rebateMode1 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(0)').val("");
            $('#rebateMode1 tr:eq(' + j + ') td:eq(0) div:eq(0) input:eq(1)').val("以上");
        }
    }
}

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
})
$("#modalProduct").on('hide.bs.modal', function (e) {
    var count = 0;
    pSelList.splice(0, pSelList.length);
    var selIds = $duallistProduct.val();
    $.each(pFullList, function (i, item) {
        if ($.inArray(item['id'] + "", selIds) > -1)
            pSelList.push(item);
    });
    $pTableWrap.show();
    $pCountWrap.show();
    $tableProduct.bootstrapTable('load', pSelList);
    $("#product_count").text(pSelList.length);

    $("#productCountWrap").show();
})
// Dual List Box END


// Table START
function TablesInit() {
    initTable($tableDistributor, tableDistributorHeader);
    $tableDistributor.bootstrapTable('load', dFullList);
    $("#distributor_count").text(dFullList.length);

    initTable($tableProduct, tableProductHeader);
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
    {
        field: "price_wholesale", title: "批发价（元/台）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); }),
    },
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
            height: 300,
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
    data['id'] = guid;
    data['name'] = $("#name").val();
    data['company_id'] = $("#company").val();
    data['company_name'] = $("#company option:selected").text();
    data['start_date'] = $("#start_date").val();
    data['end_date'] = $("#end_date").val();
    data['distributor_count'] = $("#distributor_count").html();//经销商家数
    data['product_scope'] = 2;//指定机型
    data['target_content'] = $('input[name="target_content"]:checked').val();
    data['target_mode'] = checkTargetVal;
    data['rebate_mode'] = checkRebateVal;
    if (checkTargetVal == 1)
        data['activity_target'] = $("#activity_target").val();
    data['note'] = $("#note").val();

    if (checkTargetVal != 4)    // 非按机型返利
    {
        var minInputList = $("input[name='target_min']");
        var maxInputList = $("input[name='target_max']");
        var rebInputList = $("input[name='rebate']");
        var len = minInputList.length;
        var rebateList = [];
        for (var i = 0; i < len; i++) {
            var rebate = {};
            rebate.main_id = guid;
            rebate.target_min = $(minInputList[i]).val();
            if (i == len - 1)
                rebate.target_max = -1; //以上
            else
                rebate.target_max = $(maxInputList[i]).val();
            rebate.rebate = $(rebInputList[i]).val();

            if ($(maxInputList[i]).val() == '' || $(rebInputList[i]).val() == '') {
                $.modalAlert("返利信息填写不完整！");
                return;
            }
            rebateList.push(rebate);
        }
        data['rebateList'] = rebateList;
    }
    else {
        var rebInputList = $("input[name='rebate_p']");
        var len = pSelList.length;
        for (var i = 0; i < len; i++) {
            pSelList[i].rebate = $(rebInputList[i]).val();
            if ($(rebInputList[i]).val() == '') {
                $.modalAlert("返利信息填写不完整！");
                return;
            }
        }
    }
    data['distributorList'] = dSelList;

    if (!dSelList || dSelList.length < 1) {
        $.modalAlert("请选择经销商");
        return;
    }
    if (!pSelList || pSelList.length < 1) {
        $.modalAlert("请选择型号");
        return;
    }
    data['productList'] = pSelList;
    $.submitForm({
        url: "/DistributorManage/Recommendation/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/DistributorManage/Recommendation/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

