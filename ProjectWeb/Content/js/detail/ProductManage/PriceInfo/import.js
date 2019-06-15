var cFullList = []; // 所有分公司
var cSelList = [];  // 已选分公司
var companyTree = [];
var guid = "";
var $duallistCompany = $('#duallistCompany').bootstrapDualListbox({
    preserveSelectionOnMove: 'moved',
    filterPlaceHolder: "输入名称、区域关键字",
    moveAllLabel: "全部选择",
    removeAllLabel: "全部移除",
    infoText: "共{0}个分公司",
    infoTextEmpty: "共0个分公司",
    infoTextFiltered: "",
    filterTextClear: "清空"
});

var $table = $('#tb_table');
var importData = [];
var tableHeader =
    {
        "型号": "model",
        "颜色": "color",
        "二级价": "price_l2",
        "事业部价格": "price_l3",
        "代理价": "price_l4",
        "广告费": "ad_fee_show",
        "内购价": "price_internal",
        "最低买断价": "price_buyout",
        "批发价": "price_wholesale",
        "零售价": "price_retail",
        "属性": "product_type",
        "是否特价机": "special_offer_show",
        "是否高端机": "high_level_show",
        "开始时间": "effect_date",
        "结束时间": "expire_date",
    };
var myCompanyInfo = top.clients.loginInfo.companyInfo;
$(function () {
    if (myCompanyInfo.category != "事业部") {
        $.modalAlert("价格信息须由事业部人员导入", "error")
        window.history.go(-1);
        return;
    }
    BindCompany();
    initTable();
    GetGUID();
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
    $.ajax({
        url: "/OrganizationManage/Company/GetIdNameList",
        async: false,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            cFullList = data;
            $.each(data, function (index, value) {
                value.main_id = guid;
                $duallistCompany.append("<option value='" + value.id + "'>" + value.name + "</option>");
            })
            $duallistCompany.bootstrapDualListbox("refresh");
        },
        error: function (data) {
            $.modalAlert("系统出错，请稍候重试", "error")
            $("#emp_count").text(0);
        }
    })
}
// Dual List Box START
$("#modalCompany").on('hide.bs.modal', function (e) {
    cSelList.splice(0, cSelList.length);    // 清空数组
    var selIds = $duallistCompany.val();
    $.each(cFullList, function (i, item) {
        if ($.inArray(item['id'], selIds) > -1)
            cSelList.push(item);
    });
    $("#emp_count").text(cSelList.length);
    ShowCompany();
})
function ShowCompany() {
    $("#ShowCompany").css('height', '100%');
    $("#ShowCompany").empty();//清空
    $.each(cSelList, function (index, item) {
        $("#ShowCompany").append("<button type='button' class='btn btn-primary btn-xs' style='margin:2px'>" + item.name + "</button>");
    })
}
function initTable() {
    var columns = inittitle(tableHeader);
    //1.初始化Table
    var oTable = new TableInit([], columns);
    oTable.Init();
}
var TableInit = function (data, columns) {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            url: '',         //请求后台的URL（*）
            data: data,
            method: 'get',                      //请求方式（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "asc",                   //排序方式
            queryParams: '',                    //传递参数（*）
            sidePagination: "client",           //分页方式：client客户端分页，server服务端分页（*）
            pageNumber: 1,                       //初始化加载第一页，默认第一页
            pageSize: 10,                       //每页的记录行数（*）
            pageList: [10, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            search: true,                       //是否显示表格搜索，此搜索是客户端搜索，不会进服务端，所以，个人感觉意义不大
            strictSearch: false,
            showColumns: true,                  //是否显示所有的列 
            clickToSelect: true,                //是否启用点击选中行
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: columns,
        });
    };
    return oTableInit;
};

function inittitle(gtitle) {
    var columns = [];
    for (var a in gtitle) {
        var colObj = {
            field: gtitle[a],
            title: a,
            align: "center",
            width: 200,
            sortable: true,
            search: true,
        };
        if (gtitle[a] == "price_l3" || gtitle[a] == "price_buyout") {
            colObj.width = 180;
            colObj.formatter = (function (value) {
                return ToThousandsStr(value);
            });
        } else if (gtitle[a] == "model") {
            colObj.width = 200;
        } else if (gtitle[a] == "effect_date" || gtitle[a] == "expire_date") {
            colObj.width = 160;
        }
        else if (gtitle[a] == "price_l2" || gtitle[a] == "price_l4" || gtitle[a] == "price_internal" || gtitle[a] == "price_wholesale" || gtitle[a] == "price_retail") {
            colObj.formatter = (function (value) {
                return ToThousandsStr(value);
            });
        }
        columns.push(colObj);
    }
    return columns;
}



function importfile(file) {//导入
    $table.bootstrapTable('removeAll');
    $table.bootstrapTable('showLoading');

    var f = file.files[0];
    $("#excelfile").val(f.name);
    var wb;//读取完成的数据
    var rABS = false; //是否将文件读取为二进制字符串
    var ie = IEVersion();
    if (ie != -1 && ie != 'edge') {
        if (ie < 10) {
            return;
        } else {
            rABS = true;
        }
    }

    if (checkfilename(file)) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var data = e.target.result;
            if (rABS) {
                wb = XLSX.read(btoa(fixdata(data)), {//手动转化
                    type: 'base64'
                });
            } else {
                wb = XLSX.read(data, {
                    type: 'binary'
                });
            }
            var result = XLSX.utils.sheet_to_json(wb.Sheets[wb.SheetNames[0]]);
            resoveresult(tableHeader, result);
        };

        if (rABS) {
            reader.readAsArrayBuffer(f);
        } else {
            reader.readAsBinaryString(f);
        }
    }
}

function resoveresult(config, list) {
    var num = 1;
    if (list.length > 0) {
        for (var one in list) {
            if (!(list[one]["型号"]) || list[one]["型号"] == "")
                continue; // 型号为空，跳过整行
            var obj = {};
            for (var index in config) {
                var key = list[one][index];
                if (!key || key == "") {
                    if (index == "结束时间")
                        obj[config[index]] = "2099-01-01";  // 表示长期有效
                    else if (index == "开始时间") {
                        resoveFail("第" + num + "行：开始时间不能为空");
                        return;
                    }
                    else if (index == "颜色" || index == "属性")
                        obj[config[index]] = "";
                    else
                        obj[config[index]] = 0;
                    continue;
                }
                key = key.trim();
                if (index == "开始时间" || index == "结束时间") {
                    if (key.indexOf("年") > 0 || key.indexOf("月") > 0 || key.indexOf("日") > 0) { // 只做简单判断
                        key = key.replace('年', '-');
                        key = key.replace('月', '-');
                        key = key.replace('日', '');
                    }
                    obj[config[index]] = (new Date(key)).pattern("yyyy-MM-dd");
                }
                else if (index == "广告费") {
                    if (key.charAt(key.length - 1) == "%")
                        obj.ad_fee = (key.substr(0, key.length - 1)) / 100;
                    obj[config[index]] = key;
                }
                else if (config[index] == "special_offer_show") {
                    obj.special_offer = (key == "是" ? true : false);
                    obj[config[index]] = key;
                }
                else if (config[index] == "high_level_show") {
                    obj.high_level = (key == "是" ? true : false);
                    obj[config[index]] = key;
                }
                else
                    obj[config[index]] = key;
            }
            obj.effect_satus = -2;
            importData.push(obj);
            num++;
        }
        $table.bootstrapTable('load', importData);
    }
    $table.bootstrapTable('hideLoading');
}
function resoveFail(msg) {
    $table.bootstrapTable('removeAll');
    importData = [];    // 清空上一次的数据
    $.modalAlert(msg, "error");
}
$('#btnReturn').click(function () {
    if (!!importData && importData.length > 0) {
        $.modalConfirm("确定放弃将数据导入到系统中？", function (result) {
            if (result)
                window.history.go(-1);
            top.layer.closeAll();
        });
    }
    else
        window.history.go(-1);
})

function submitForm() {
    if (!importData || importData.length < 1) {
        $.modalAlert("无数据可导入");
        return;
    }
    var data = {};
    var companyList = [];
    data.import_file = $("#excelfile").val();
    if (!data.import_file || data.import_file == "") {
        $.modalAlert("请选择导入文件");
        return;
    }
    if (!cSelList || cSelList.length < 1) {
        $.modalAlert("请选择分公司");
        return;
    }
    data["companyList"] = cSelList;
    data["importListStr"] = JSON.stringify(importData);
    $.submitForm({
        url: "/ProductManage/PriceInfo/Import?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/ProductManage/PriceInfo/ImportHistoryIndex";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

