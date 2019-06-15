var myCompanyInfo = top.clients.loginInfo.companyInfo;
var emp_type = $('#emp_category').val();//员工类型
var checkTargetVal = $('input[name="target_mode"]:checked').val();//返利模式
var checkRebateVal = $('input[name="rebate_mode"]:checked').val();//金额计算模式
var checkProductVal = $('input[name="product_scope"]:checked').val();//活动机型
var guid = "";
var guideTree = [];
var areaFullList = [];
var eFullList = []; // 所有经销商
var eSelList = [];  // 已选经销商
var pFullList = [];   // 所有机型
var pSelList = [];    // 已选机型
var $tableEmp = $('#targetDistributor');
var index;//行号
//导购员
var $duallistGuide = $('#duallistGuide').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入名称、区域关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个导购员",
    infoTextEmpty: "共0个导购员",
    infoTextFiltered: "",
    filterTextClear: "清空"
});
//业务员
var $duallistSales = $('#duallistSales').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入名称、区域关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个业务员",
    infoTextEmpty: "共0个业务员",
    infoTextFiltered: "",
    filterTextClear: "清空"
});
//业务经理
var $duallistSalesManage = $('#duallistSalesManage').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入名称、区域关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个业务经理",
    infoTextEmpty: "共0个业务经理",
    infoTextFiltered: "",
    filterTextClear: "清空"
});

$('#emp_category').on("change", function () {
    emp_type = $('#emp_category option:selected').val();
    if (emp_type == 3) {
        $('#selectGuide').css("display", "");
        $('#selectSales').css("display", "none");
        $('#selectSalesManage').css("display", "none");
        $("input[name='ranking_content'][value='2']").attr("disabled", "disabled");
        $("input[name='ranking_content'][value='4']").attr("disabled", "disabled");
        $("input[name='ranking_content'][value='1']").prop("checked", "checked");
    } else if (emp_type == 20) {
        $('#selectGuide').css("display", "none");
        $('#selectSales').css("display", "");
        $('#selectSalesManage').css("display", "none");
        $("input[name='ranking_content'][value='2']").removeAttr("disabled");
        $("input[name='ranking_content'][value='4']").removeAttr("disabled");
    } else if (emp_type == 21) {
        $('#selectGuide').css("display", "none");
        $('#selectSales').css("display", "none");
        $('#selectSalesManage').css("display", "");
        $("input[name='ranking_content'][value='2']").removeAttr("disabled");
        $("input[name='ranking_content'][value='4']").removeAttr("disabled");
    }
    eFullList.splice(0, eFullList.length);  // 清空数组
    eSelList.splice(0, eSelList.length);  // 清空数组
    $duallistGuide.empty();
    $duallistSales.empty();
    $duallistSalesManage.empty();
    GetguideTree();
})

$(function () {
    GetGUID();
    TablesInit();
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
        eFullList.splice(0, eFullList.length);  // 清空数组
        eSelList.splice(0, eSelList.length);  // 清空数组
        $duallistGuide.empty();
        $duallistGuide.bootstrapDualListbox("refresh");

        $duallistSales.empty();
        $duallistSales.bootstrapDualListbox("refresh");

        $duallistSalesManage.empty();
        $duallistSalesManage.bootstrapDualListbox("refresh");
        GetguideTree();
    })
    GetguideTree();
}
$('#guide_category').on("change", function () {
    //emp_type = $('#emp_category').val();
    //if (emp_type == 3) {
    //    GetguideTree();
    //}
    eFullList.splice(0, eFullList.length);  // 清空数组
    eSelList.splice(0, eSelList.length);  // 清空数组
    $duallistGuide.empty();
    $duallistGuide.bootstrapDualListbox("refresh", true);
    if (emp_type == 3) {
        GetguideTree();
    }
})

function GetguideTree() {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetEmpTree?emp_type=" + emp_type,
        data: { company_id: $("#company").val(), guide_category: $("#guide_category").val() },
        async: false,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
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
                            if (emp_type == 3) {
                                $duallistGuide.append("<option value='" + value1.id + "'>" + value1.display_info + "</option>");
                            } else if (emp_type == 20) {
                                $duallistSales.append("<option value='" + value1.id + "'>" + value1.display_info + "</option>");
                            } else if (emp_type == 21) {
                                $duallistSalesManage.append("<option value='" + value1.id + "'>" + value1.display_info + "</option>");
                            }
                        });
                    }
                })
            }
            $duallistGuide.bootstrapDualListbox("refresh");
            $duallistSales.bootstrapDualListbox("refresh");
            $duallistSalesManage.bootstrapDualListbox("refresh");
            $tableEmp.bootstrapDualListbox("refresh");
            $tableEmp.bootstrapTable('load', eFullList);
            $("#emp_count").text(eFullList.length);
        },
        error: function (data) {
            $.modalAlert("系统出错，请稍候重试", "error")
            $tableEmp.bootstrapTable('load', eFullList);
            $("#emp_count").text(0);
        }
    })

}

function add_one() {
    var tr = '<tr><td><div class="input-single formValue">'
           + '<input  class="form-control text-center required number" name="place_min"/></div></td><td><div class="input-single formValue"><input  class="form-control text-center required" name="reward"/></div></td><td><i onclick="deleteRow(this)" class="fa fa-minus-circle fa-lg margin text-red "></i></td></tr>';
    $("#rank_tb").append(tr);
}

function add_two() {
    var tr = '<tr><td><div class="input-group"> <input class="form-control text-center" name="place_min"  ><span class="input-group-addon no-border">-</span> <input class="form-control text-center" name="place_max"></div></td><td><input  class="form-control text-center required number"  name="reward"/></td><td><i onclick="deleteRow(this)" class="fa fa-minus-circle fa-lg margin text-red "></i></td></tr>';
    $("#rank_tb").append(tr);
}
//删除
function deleteRow(e) {
    $(e).parents('tr').remove();
}
// Dual List Box START
$("#modalGuide").on('hide.bs.modal', function (e) {
    eSelList.splice(0, eSelList.length);    // 清空数组
    var selIds = $duallistGuide.val();
    $.each(eFullList, function (i, item) {
        if ($.inArray(item['id'], selIds) > -1)
            eSelList.push(item);
    });
    $tableEmp.bootstrapTable('load', eSelList);
    $("#emp_count").text(eSelList.length);
})
$("#modalSales").on('hide.bs.modal', function (e) {
    eSelList.splice(0, eSelList.length);    // 清空数组
    var selIds = $duallistSales.val();
    $.each(eFullList, function (i, item) {
        if ($.inArray(item['id'], selIds) > -1)
            eSelList.push(item);
    });
    $tableEmp.bootstrapTable('load', eSelList);
    $("#emp_count").text(eSelList.length);
})
$("#modalSalesManage").on('hide.bs.modal', function (e) {
    eSelList.splice(0, eSelList.length);    // 清空数组
    var selIds = $duallistSalesManage.val();
    $.each(eFullList, function (i, item) {
        if ($.inArray(item['id'], selIds) > -1)
            eSelList.push(item);
    });
    $tableEmp.bootstrapTable('load', eSelList);
    $("#emp_count").text(eSelList.length);
})

// Dual List Box END

// Table START
function TablesInit() {
    initTable($tableEmp, tableGuideHeader);
    $tableEmp.bootstrapTable('load', eFullList);
    $("#emp_count").text(eFullList.length);
}
var tableGuideHeader = [
    { field: "id", visible: false, },
    { field: "name", title: "名称", align: "center" },
    { field: "area_l2_name", title: "所属片区", align: "center", },
    { field: "company_name", title: "分公司", align: "center", },
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

//input输入值 1.小数不行
//function setNum() {
//    for (var i = 2; i < $("#rank_tb tr").length; i++) {
//        var max = $('#rank_tb tr:eq(' + i + ') td:eq(0) input:eq(1)').val();
//        var min = $('#rank_tb tr:eq(' + i + ') td:eq(0) input:eq(0)').val();
//        if (max != '') {
//            if (max <= min) {
//                $.modalAlert("该值应大于" + $('#rank_tb tr:eq(' + i + ') td:eq(0) input:eq(0)').val() + "！");
//            }
//        } else {
//            if ($('#rank_tb tr:eq(' + i + ') td:eq(0) input:eq(0)').val() < $('#rank_tb tr:eq(' + (i + 1) + ') td:eq(0) input:eq(0)').val()) {
//            }
//        }
//    }
//}

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
    data['emp_category'] = $("#emp_category").val();
    data['company_name'] = $("#company option:selected").text();
    data['start_date'] = $("#start_date").val();
    data['end_date'] = $("#end_date").val();
    data['emp_count'] = $("#emp_count").html();//参与人数
    data['ranking_content'] = $('input[name="ranking_content"]:checked').val();//考核内容
    data['note'] = $("#note").val();
    var rewardList = [];
    var minInputList = $("input[name='place_min']");
    var maxInputList = $("input[name='place_max']");
    var rewInputList = $("input[name='reward']");
    var len = minInputList.length;

    for (var i = 0; i < len; i++) {
        var reward = {};
        reward.main_id = guid;
        reward.place_min = $(minInputList[i]).val();
        reward.place_max = $('#rank_tb tr:eq(' + (i+1) + ') td:eq(0) input:eq(1)').val();
        reward.reward = $(rewInputList[i]).val();
        if ($(minInputList[i]).val() == '' || $('#rank_tb tr:eq(' + (i + 1) + ') td:eq(0) input:eq(1)').val() == '' || $(rewInputList[i]).val()==''  ) {
            $.modalAlert("奖励规则不完整！");
            return;
        }
        rewardList.push(reward);
    }

    data['rewardList'] = rewardList;
    if (!eSelList || eSelList.length < 1) {
        if (emp_type == 3) {
            $.modalAlert("请选择导购员");
        } else if (emp_type == 20) {
            $.modalAlert("请选择业务员");
        } else if (emp_type == 21) {
            $.modalAlert("请选择业务经理");
        }
        return;
    }
    data['empList'] = eSelList;
    $.submitForm({
        url: "/ActivityManage/Ranking/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/ActivityManage/Ranking/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

