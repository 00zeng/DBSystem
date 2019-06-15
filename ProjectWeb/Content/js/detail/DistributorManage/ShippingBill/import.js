//运费导入
var $table = $('#tb_table');
var importData = [];
var tableHeader =
    {
        "单号": "shipping_bill",
        "分公司": "company_name",
        "经销商": "distributor_name",
        "数量": "quantity",
        "金额": "amount",
        "明细": "product_detail",
        "制单时间": "bill_date",
        "是否收货": "is_received",
        "备注": "note",
        "经手人": "operator_name",
        "是否打印": "is_printed",
        "扩展备注": "extend_note",
        "计划单备注": "schedule_note",
        "计划单自定义单号": "schedule_bill",
    };

//var guid = "";
//var distributorTree = "";
var myCompanyInfo = top.clients.loginInfo.companyInfo;
//var free_count = 0;   //包邮数量
//var minimum_fee = 0; //最低运费
//var shippingFeeList = [];  //运费规则列表
//var accur_time = null; // 发货时间
$(function () {
    initTable();
    //GetShippingFeeEffectInfo();
    //GetGUID();
    //GetDistributorTree();

});

//function FileInputClick() {
//    accur_time = $("#shippingTime").val();
//    if (!accur_time || accur_time == null || accur_time == "") {
//        $.modalAlert("请先选择发货时间");
//        return;
//    }
//    $('#FileInput')[0].click();
//}

//function GetShippingFeeEffectInfo() {
//    $.ajax({
//        url: "/DistributorManage/shippingBill/GetEffectInfo",
//        type: "get",
//        async: false,
//        dataType: "json",
//        success: function (data) {
//            if (!data || !data.mainInfo) {
//                $.modalAlert("当前没有可用的运费模板，请先添加运费模板！");
//                return;
//            }
//            free_count = data.mainInfo.free_count;   //包邮数量
//            minimum_fee = data.mainInfo.minimum_fee; //最低运费
//            shippingFeeList = data.shippingFeeList;  //运费规则列表
//        },
//error: function (data) {
//    $.modalAlert(data.responseText, 'error');
//}
//    });
//}

//function GetGUID() {
//    $.ajax({
//        url: "/ClientsData/GetGUID",
//        type: "get",
//        dataType: "json",
//        success: function (data) {
//            guid = data.guid;
//        },
//error: function (data) {
//    $.modalAlert(data.responseText, 'error');
//}
//    });
//}

//function GetDistributorTree() {
//    $.ajax({
//        url: "/DistributorManage/DistributorManage/GetDistributorTree",
//        type: "get",
//        async: false,
//        dataType: "json",
//        success: function (data) {
//            distributorTree = data;
//        },
//error: function (data) {
//    $.modalAlert(data.responseText, 'error');
//}
//    });
//}

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
            search: true,
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
            sortable: true,
            width: 150,
            search: true,

        }
        if (gtitle[a] == "quantity" || gtitle[a] == "amount") {
            colObj.formatter = (function (value) { return ToThousandsStr(value); });
        }

        if (gtitle[a] == "product_detail") {
            colObj.width = 450;
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


//function calculateShippingFee(count) {
//    var shippingFee = 0;
//    if (count >= free_count)
//        return 0;

//    for (var i = 0; i < shippingFeeList.length; i++) {
//        if (count >= shippingFeeList[i].count_min && count <= shippingFeeList[i].count_max) {
//            shippingFee = count * shippingFeeList[i].shipping_fee;
//            break;
//        }
//    }
//    shippingFee = (shippingFee > minimum_fee) ? shippingFee : minimum_fee;
//    return shippingFee;
//}

function resoveresult(config, list) {
    var num = 1;
    if (list.length > 0) {
        for (var one in list) {
            var obj = {};
            var shippingBill = "";
            var distributorName = "";
            var billDate = "";
            var companyName = "";
            for (var index in config) {
                var key = list[one][index];
                if (!key || key == "") {
                    if (index == "数量" || index == "金额")
                        obj[config[index]] = 0;
                    else
                        obj[config[index]] = "";
                    continue;
                }
                key = key.trim();
                if (index == "数量" || index == "金额")
                    key = key | 0; // 取整数（TODO 金额后台改为decimal，此为临时代码）
                
                //if (index == "数量") {
                //    obj[config[index]] = key;
                //    obj[config["运费"]] = calculateShippingFee(key);
                //}

                obj[config[index]] = key;
                if (index == "单号")
                    shippingBill = key;
                else if (index == "经销商")
                    distributorName = key;
                else if (index == "制单时间") {
                    if (key.indexOf("年") > 0 || key.indexOf("月") > 0 || key.indexOf("日") > 0) { // 只做简单判断
                        key = key.replace('年', '-');
                        key = key.replace('月', '-');
                        key = key.replace('日', '');
                    }
                    var date = new Date(key);
                    obj[config[index]] = date.pattern("yyyy-MM-dd hh:mm:ss");
                    billDate = key;
                }
                else if (index == "分公司")
                    companyName = key;
            }
            if (shippingBill == "" || shippingBill == null) {
                resoveFail("第" + num + "行：单号为空，请核查后重新导入！");
                return;
            }
            if (distributorName == "" || distributorName == null) {
                resoveFail("第" + num + "行：经销商名称为空，请核查后重新导入！");
                return;
            }
            if (billDate == "" || billDate == null) {
                resoveFail("第" + num + "行：制单时间为空，请核查后重新导入！");
                return;
            }
            if (companyName == "" || companyName == null) {
                resoveFail("第" + num + "行：分公司名称为空，请核查后重新导入！");
                return;
            }
            //else if (shippingBill == "" && distributorName == "") //跳过该行不处理；
            //    continue;
            //obj.company_id_parent = myCompanyInfo.id;
            //obj.import_file_id = guid;
            //obj.id = Number(one);
            //obj.accur_time = accur_time;
            importData.push(obj);
            num++;
        }

    }
    $table.bootstrapTable('load', importData);
    $table.bootstrapTable('hideLoading');
}

function resoveFail(msg) {
    $table.bootstrapTable('removeAll');
    $table.bootstrapTable('hideLoading');
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
        alert("无数据可导入");
        return;
    }
    var data = {};
    data["import_file"] = $("#excelfile").val()
    if (!data["import_file"]) {
        alert("请选择导入文件");
        return;
    }
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["importListStr"] = JSON.stringify(importData);

    $.submitForm({
        url: "/DistributorManage/ShippingBill/Import?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/DistributorManage/ShippingBill/HistoryIndex";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

