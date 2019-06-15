var myCompanyInfo = top.clients.loginInfo.companyInfo;
var emp_type = $('#emp_category').val();//员工类型
var checkTargetVal = $('input[name="target_mode"]:checked').val();//奖励模式
var checkRebateVal = $('input[name="rebate_mode"]:checked').val();//金额计算模式
var checkProductVal = $('input[name="product_scope"]:checked').val();//活动机型
var guid = "";
var guideTree = [];
var areaFullList = [];
var eFullList = []; // 所有经销商
var eSelList = [];  // 已选经销商
var pFullList = [];   // 所有机型
var pSelList = [];    // 已选机型
var $tableProduct = $('#targetProduct');
var $tableRebateProduct = $('#rebateProduct');  // 按型号返利
var $pCountWrap = $("#productCountWrap");
var $pTableWrap = $("#targetProductWrap");
var activity_target = 0;//目标销量
var index;//行号
var str1 = "<tr style='display: table;width: 100%;table-layout: fixed;'>"
        + "<td class='col-sm-5'><div class='input-group'><input class='form-control text-center' name='target_min' disabled>"
        + "<span class='input-group-addon no-border'>-</span><div class='formValue'><input type='text' class='form-control text-center required number' name='target_max' onchange='setNum()'></div>"
        + "<td class='col-sm-5'><div class='input-group formValue'><div class='input-group-btn'><button type='button' class='btn btn-primary'  name='rewards'  onclick='Change(this)'>奖</button></div><input type='text' class='form-control text-center required number' name='rebate'>"
        + "<span name='rebate_mode_input1' class='input-group-addon no-border'>";
var str2 = "元/台";
var str3 = "</span><span name='rebate_mode_input2' class='input-group-addon no-border no-padding'>";
var str4 = "";
var str5 = "</span></div></td><td class='col-sm-2'><div class='row'><i onclick='addRow(this)' class='fa fa-plus-circle fa-lg  text-blue '></i> <i onclick='deleteRow(this)' class='fa fa-minus-circle fa-lg text-red'></i></div></td>"
        + "</tr>";
//型号的th
var str6 = '<tr><th style="text-align:center;vertical-align:middle;" class="col-sm-3">型号</th><th style="text-align:center;vertical-align:middle;" class="col-sm-2">颜色</th><th style="text-align:center;vertical-align:middle;" class="col-sm-2">批发价(元/台)</th><th style="text-align:center;vertical-align:middle;" class="col-sm-5"><span name="rebate_mode_change">奖励金额</span></th></tr>';

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


$('#emp_category').on("change", function () {
    emp_type = $('#emp_category option:selected').val();
    $("td[id^=target]").empty();
    $("input[name^=activity_target]").val("");
    if (emp_type == 3) {
        $("#onlyGuide").css("display", "");
    } else {
        $("#onlyGuide").css("display", "none");
    }
    $("#EmpTable tbody").empty();
    GetguideTree();
})


$('#guide_category').on("change", function () {
    emp_type = $('#emp_category').val();
    if (emp_type == 3) {
        GetguideTree();
    }
})
$(function () {
    $("#form1").queryFormValidate();

    $(window).resize(function () {
        $tableProduct.bootstrapTable('resetView', {
            height: 300
        });
    });
    $pTableWrap.hide();
    $pCountWrap.hide();
    GetGUID();
    BindCompany();
    TablesInit();
    checkChange();
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
        $("#EmpTable tbody").empty();

        areaFullList.splice(0, areaFullList.length);
        eFullList.splice(0, eFullList.length);  // 清空数组
        eSelList.splice(0, eSelList.length);  // 清空数组

        pFullList.splice(0, pFullList.length);
        $duallistProduct.empty();
        $duallistProduct.bootstrapDualListbox("refresh");
        $pTableWrap.hide();
        $pCountWrap.hide();

        $("#all_product").prop("checked", "checked");

        GetguideTree();
        getProduct();
    })
    GetguideTree();
    getProduct();
}

function GetguideTree() {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetEmpTree?emp_type=" + emp_type,
        data: { company_id: $("#company").val(), guide_category: $("#guide_category").val() },
        async: false,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            $("#EmpTable tbody").empty();
            var sum = 0;
            guideTree.splice(0, guideTree.length, data);
            if (data.length > 0) {
                $.each(data, function (index, value) {
                    areaFullList.push(value);
                    if (value.key_list.length > 0) {
                        $.each(value.key_list, function (index1, value1) {
                            var d = value1;
                            d.main_id = guid;
                            d.emp_id = d.id;
                            d.emp_name = d.name;
                            eFullList.push(d);
                            eSelList.push(d);
                            $("#EmpTable tbody").append("<tr><td  style='width:385px;' id='draggable" + index1 + "' class='ui-draggable ui-draggable-handle'  data-emp_id='" + value1.emp_id + "' data-emp_name='" + value1.emp_name + "' data-area_l2_id='" + value1.area_l2_id + "' data-area_l2_name='" + value1.area_l2_name + "'> " + value1.emp_name + "(" + value1.area_l2_name + ")" + " </td> </tr>");
                            Move();
                        });
                    }
                    sum += data[index].key_list.length;
                })
            }
            $("#emp_count").text(sum);
        },
        error: function (data) {
            $.modalAlert("系统出错，请稍候重试", "error")
            $("#emp_count").text(0);
        }
    })
}

//function GetEmpTree() {//guide
//    $.ajax({
//        url: "/HumanResource/EmployeeManage/GetEmpTree",//emp_type = 3, int company_id = 0, string guide_category = null
//        data: { company_id: $("#company").val(), emp_type: 3, guide_category: $("#guide_category").val() },
//        async: false,
//        type: "get",
//        dataType: "json",
//        contentType: "application/json",
//        success: function (data) {
//            console.log(data);
//        },
//        error: function (data) {

//        }
//    })
//}
//查询条件
$('#findinput').bind("input propertychange", function (event) {
    var keyword = $("#findinput").val();
    if (keyword != null) {
        $('#EmpTable tbody tr').hide()//将原有的内容隐藏
        $("td").filter(":Contains(" + keyword + ")").parents('tr').fadeIn(500);
    } else {
        $('#EmpTable tbody tr').show();//将原有的内容显示
    }
})
var times = 1;
//新增比赛行
function addItem() {
    times++;
    var tr = ' <tr style="height:50px"><td class="col-sm-2" name="left" id="target51' + times + '"></td> <td class="col-sm-2"><div class="input-group formValue">'
           + ' <input type="text"  class="form-control  text-center"  name="activity_target_left"><span class="input-group-addon">台</span></div></td>'
           + ' <td class="col-sm-4"> <div class="progress-group"><div class="progress sm"><div class="progress-bar progress-bar-blue"  style="width: 50%" ></div>'
           + ' </div></div></td><td class="col-sm-2"  name="right" id="target52' + times + '"></td> <td class="col-sm-2"><div class="input-group formValue"><input type="text"  class="form-control  text-center"  name="activity_target_right">'
           + ' <span class="input-group-addon">台</span></div></td></tr>';
    $("#race").append(tr);
    Move();
}
//统一目标销量
function UnifySum() {
    if ($("#activity_target").val() == '') {
        $.modalAlert("请输入目标销量！");
        return;
    }
    activity_target = $("#activity_target").val();
    $("input[name=activity_target_left]").val(activity_target);
    $("input[name=activity_target_right]").val(activity_target);
}
$('#win_lose').on("change", function () {
    $("#lose_win").val($("#win_lose").val());
})
//奖罚方式 input 有值时 checkbox 勾选
$('#win_company').on("change", function () {
    if ($(this).val().trim()) {
        $("input[name='win_company']").prop("checked", true);
    } else
        $("input[name='win_company']").prop("checked", false);
})
function checkChange() {
    if (!$("input[name='win_company']").is(':checked')) {//未选中
        $('#win_company').val("0");
    }
    if (!$("input[name='win_penalty']").is(':checked')) {//未选中
        $('#win_penalty').val("0");
    }
    if (!$("input[name='lose_penalty']").is(':checked')) {//未选中
        $('#lose_penalty').val("0");
    }
}
$('#win_penalty').on("change", function () {
    if ($(this).val().trim()) {
        $("input[name='win_penalty']").prop("checked", true);
    } else
        $("input[name='win_penalty']").prop("checked", false);
})
$('#lose_penalty').on("change", function () {
    if ($(this).val().trim()) {
        $("input[name='lose_penalty']").prop("checked", true);
    } else
        $("input[name='lose_penalty']").prop("checked", false);
})


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

//活动机型 
$('input[name="product_scope"]').on("change", function () {
    checkProductVal = $('input[name="product_scope"]:checked').val();
    if (checkProductVal == 1) {
        $pTableWrap.hide();
        $pCountWrap.hide();
        $("#rebateMode1").show();
        $("#rebateMode4").hide();
    } else {
        $pTableWrap.show();
        $pCountWrap.show();
    }
})

function Move() {
    //在页面加载完之后加载jquery
    $().ready(function (e) {
        //拖拽复制体
        $('td[id^="draggable"]').draggable({
            helper: "clone",
            cursor: "move",

        });
        //释放后
        $('td[id^="target"]').droppable({
            drop: function (event, ui) {
                $(this).children().remove();
                var source = ui.draggable.clone();// <i class="glyphicon glyphicon-remove text-red"></i>
                var source1 = ui.draggable.clone();// <i class="glyphicon glyphicon-remove text-red"></i>
                $('<img />', {
                    src: '/Content/img/icon/del.png',
                    style: 'display:none',
                    click: function () {
                        $("#EmpTable tbody").prepend('<tr>' + $(source1)[0].outerHTML + '</tr>');
                        Move();
                        source.remove();
                    }
                }).appendTo(source);

                source.mouseenter(function () {
                    $(this).find("img").show();
                });

                source.mouseleave(function () {
                    $(this).find("img").hide();
                });

                $(this).append(source);
                ui.draggable.remove();
            }
        });
    });
}

$("#modalProduct").on('hide.bs.modal', function (e) {
    var count = 0;
    pSelList.splice(0, pSelList.length);
    var selIds = $duallistProduct.val();
    $.each(pFullList, function (i, item) {
        if ($.inArray(item['id'] + "", selIds) > -1)
            pSelList.push(item);
    });
    $tableProduct.bootstrapTable('load', pSelList);
    $("#product_count").text(pSelList.length);
})
// Dual List Box END

// Table START
function TablesInit() {
    initTable($tableProduct, tableProductHeader);
}
var tableProductHeader = [
    { field: "id", visible: false, },
    { field: "model", title: "型号", align: "center" },
    { field: "color", title: "颜色", align: "center", },
    {
        field: "price_wholesale", title: "批发价（元/台）", align: "center",
        formatter: (function (value) { return ToThousandsStr(value); })
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
    var data = $("#form1").formSerialize();
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data['name'] = $("#name").val();
    data['company_id'] = $("#company").val();
    data['emp_category'] = $("#emp_category").val();
    data['company_name'] = $("#company option:selected").text();
    data['start_date'] = $("#start_date").val();
    data['end_date'] = $("#end_date").val();
    data['emp_count'] = $("#emp_count").html();//经销商家数
    data['product_scope'] = checkProductVal;
    data['target_content'] = $('input[name="target_content"]:checked').val();
    data['target_mode'] = checkTargetVal;
    data['rebate_mode'] = checkRebateVal;
    if (checkTargetVal == 1)
        data['activity_target'] = $("#activity_target").val();
    data['note'] = $("#note").val();

    var raceTr = $("#race").find("tr");

    var trLen = raceTr.length;
    var left_sum = 0;
    var right_sum = 0;
    var equal = true;
    var empList = [];
    var leftSum = $("input[name=activity_target_left]");
    var rightSum = $("input[name=activity_target_right]");
    for (var i = 0; i < trLen; i++) {
        var leftEle = $($(raceTr).find("td[name=left]")).children();
        var rightEle = $($(raceTr).find("td[name=right]")).children();
        if ($(leftEle[i]).data('emp_id')) {
            left_sum++;
        }
        if ($(rightEle[i]).data('emp_id')) {
            right_sum++;
        }
        if (left_sum == 0 && right_sum == 0) {
            $.modalAlert("请选择PK人员！");
            return false;
        }
        if ((!$(leftEle[i]).data('emp_id') && $(rightEle[i]).data('emp_id')) || ($(leftEle[i]).data('emp_id') && !$(rightEle[i]).data('emp_id'))) {
            $.modalAlert("选择比赛的员工不成对，请正确选择！");
            equal = false;
        }
        if (($(leftEle[i]).data('emp_id') && $(leftSum[i]).val() == "") || ($(rightEle[i]).data('emp_id') && $(rightSum[i]).val() == "")) {
            $.modalAlert("请输入目标销量！");
            equal = false;
        }

        if (!equal) {
            return false;
        }
        if ($(leftEle[i]).data('emp_id')) {
            var left_params = {};
            left_params.group_number = left_sum;
            left_params.emp_id = $(leftEle[i]).data('emp_id');
            left_params.emp_name = $(leftEle[i]).data('emp_name');
            left_params.area_l2_id = $(leftEle[i]).data('area_l2_id');
            left_params.area_l2_name = $(leftEle[i]).data('area_l2_name');
            left_params.activity_target = $(leftSum[i]).val();
            empList.push(left_params);
        }
        if ($(rightEle[i]).data('emp_id')) {
            var right_params = {};
            right_params.group_number = right_sum;
            right_params.emp_id = $(rightEle[i]).data('emp_id');
            right_params.emp_name = $(rightEle[i]).data('emp_name');
            right_params.area_l2_id = $(rightEle[i]).data('area_l2_id');
            right_params.area_l2_name = $(rightEle[i]).data('area_l2_name');
            right_params.activity_target = $(rightSum[i]).val();
            empList.push(right_params);
        }
    }
    data['empList'] = empList;
    if (checkProductVal == 2) {
        if (!pSelList || pSelList.length < 1) {
            $.modalAlert("请选择型号");
            return;
        }
    }
    data['productList'] = pSelList;
    $.submitForm({
        url: "/ActivityManage/PK/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/ActivityManage/PK/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
