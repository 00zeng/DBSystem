var $table = $('#tb_table');
var importData = [];
var tableHeader =
    {
        "门店名": "distributor_name",
        "业务大区": "company_name",
        "三级地区": "area_l2_name",
        "型号": "model",
        "串码": "phone_sn",
        "调价日期": "accur_time",
        "原批发价": "orig_price",
        "新批发价": "new_price",
        "补差金额": "diff_price",
    };

var guid = "";
var distributorList = [];
var myCompanyInfo = top.clients.loginInfo.companyInfo;
$(function () {
    initTable();
    GetGUID();
    GetListForMatching();
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
function GetListForMatching() {
    $.ajax({
        url: "/SystemManage/CommonApi/GetListForMatching",
        data: { guide: false, sales: false, distributor: true, company: false, area: false },
        type: "get",
        async: false,
        dataType: "json",
        success: function (data) {
            distributorList = data.distributorList;
            //guideList = data.guideList;
            //salesList = data.salesList;
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}
//function GetDistributorTree() {
//    $.ajax({
//        url: "/DistributorManage/DistributorManage/GetDistributorTree",
//        type: "get",
//        async: false,
//        dataType: "json",
//        success: function (data) {
//            distributorTree = data;
//        },
//        error: function (data) {
//            $.modalAlert(data.responseText, 'error');
//        }
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
            uniqueId: "",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: columns,
        });
    };
    return oTableInit;
};

function inittitle(gtitle) {
    var columns = []; //; [{ field: "id", title: "", align: "center", edit: false, formatter: function (value, row, index) { return index; }}]

    for (var a in gtitle) {
        var colObj = {
            field: gtitle[a],
            title: a,
            align: "center",
            width: 150,
            sortable: true,
        };
        if (gtitle[a] == "orig_price" || gtitle[a] == "new_price" || gtitle[a] == "diff_price" ) {
            colObj.formatter = (function (value) { return ToThousandsStr(value); });
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
            var companyName = "";
            var area_l2_name = "";
            var salesDate = "";
            var distributorName = "";
            var obj = {};
            for (var index in config) {
                var key = list[one][index];
                if (!key || key == "") {
                    if (index == "调价日期") {
                        resoveFail("第" + num + "行：调价日期有误，请核查后重新导入！");
                        return;
                    } else if (index == "原批发价" || index == "新批发价" || index == "补差金额") {
                        obj[config[index]] = 0;
                    }
                    else
                        obj[config[index]] = "";
                    continue;
                }
                key = key.trim();
                if (index == "调价日期") {
                    if (key.indexOf("年") > 0 || key.indexOf("月") > 0 || key.indexOf("日") > 0) { // 只做简单判断
                        key = key.replace('年', '-');
                        key = key.replace('月', '-');
                        key = key.replace('日', '');
                    }
                    var date = new Date(key);
                    salesDate = date.pattern("yyyy-MM-dd");
                    obj[config[index]] = salesDate;
                }
                else {
                    obj[config[index]] = key;
                    if (index == "业务大区")
                        companyName = key;
                    else if (index == "三级地区")
                        area_l2_name = key;
                    else if (index == "门店名")
                        distributorName = key;
                }

            }
            if (companyName == "")
            {
                resoveFail("第" + num + "行：分公司（业务大区）名称为空，请核查后重新导入！");
                return ;
            }
            else if (area_l2_name == "")
            {
                resoveFail("第" + num + "行：业务片区为空，请核查后重新导入！");
                return;
            }
            else if (distributorName == "") {
                resoveFail("第" + num + "行：经销商名称为空，请核查后重新导入！");
                return;
            }
            //经销商
            if (distributorName != null && distributorName != "") {
                var sLen = 0;
                for (var a in distributorList) {
                    if (distributorName == distributorList[a].name && salesDate >= distributorList[a].effect_date && (distributorList[a].inactive == false || salesDate <= distributorList[a].inactive_time)) {
                        obj.distributor_id = distributorList[a].main_id;
                        obj.company_id = distributorList[a].company_id;
                        obj.company_id_parent = distributorList[a].company_id_parent;
                        obj.company_linkname = distributorList[a].company_linkname;
                        break;
                    }
                    sLen++;
                }
                if (sLen == distributorList.length) {
                    //没有匹配到经销商
                    resoveFail("第" + num + "行：系统无法匹配到经销商【" + distributorName + "】的信息，请核查后重新导入！");
                    return;
                }
            } else {
                resoveFail("第" + num + "行：经销商名称不能为空，请核查后重新导入！");
                return;
            }
            //if (companyName == "" || area_l2_name == "" || distributorName == "") {
            //    resoveFail("第" + num + "行：业务大区/三级地区/门店有误，请核查后重新导入！");
            //    return;
            //}
            //var cLen = 0;
            //for (var c in distributorTree) {
            //    if (distributorTree[c].company_name != companyName) {
            //        cLen++; continue;
            //    }
            //    var area_l2_list = distributorTree[c].area_l2_list;
            //    var aLen = 0;
            //    for (var a in area_l2_list) {
            //        if (area_l2_list[a].area_l2_name != area_l2_name) {
            //            aLen++; continue;
            //        }
            //        var keyList = area_l2_list[a].key_list;
            //        var kLen = 0;
            //        for (var k in keyList) {
            //            if (keyList[k].name != distributorName) {
            //                kLen++; continue;
            //            }
            //            obj.distributor_id = keyList[k].id;
            //            obj.company_id = distributorTree[c].company_id;
            //            obj.company_linkname = distributorTree[c].company_linkname; // LinkName
            //            obj.area_l2_id = area_l2_list[a].area_l2_id;
            //            obj.area_l2_name = area_l2_list[a].area_l2_name;
            //            obj.area_l1_id = keyList[k].area_l1_id;
            //            obj.area_l1_name = keyList[k].area_l1_name;
            //            break;
            //        }
            //        if (kLen == keyList.length) {
            //            resoveFail("第" + num + "行：系统中不存在该经销商“" + distributorName + "”，请核查后重新导入！");
            //            return;
            //        }
            //        break;
            //    }
            //    if (aLen == area_l2_list.length) {
            //        resoveFail("第" + num + "行：系统中不存在业务片区“" + area_l2_name + "”，请核查后重新导入！");
            //        return;
            //    }
            //    break;
            //}
            //if (cLen == distributorTree.length) {
            //    resoveFail("第" + num + "行：系统中不存在分公司（业务大区）“" + companyName + "”，请核查后重新导入！");
            //    return;
            //}
            obj.company_id_parent = myCompanyInfo.id;
            obj.check_status = 1;
            obj.import_file_id = guid;
            importData.push(obj);
            num++;
        }
        $table.bootstrapTable('load', importData);
    }
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
    data["id"] = guid;
    data["importListStr"] = JSON.stringify(importData);

    $.submitForm({
        url: "/SaleManage/refund/Import?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/SaleManage/refund/HistoryIndex";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
