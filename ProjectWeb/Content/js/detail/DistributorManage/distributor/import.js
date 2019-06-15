var $table = $('#tb_table');
var importData = [];
var tableHeader =
    {
        "经销商名称": "name",
        "V2报量名称": "name_v2",
        "快捷编码": "code",
        "经销商ID": "id",
        "所属代理商": "agent",
        "连锁体系": "system_chain",
        "TOP层级客户": "top_customers",
        "体系调拨": "system_allocation",
        "地区属性": "locale_attribute",
        "客户类别": "customer_category",
        "客户级别": "customer_level",
        "经销商属性": "distributor_attribute",
        "运营商属性": "sp_attribute",
        "潜在售点": "potential_salepoint",
        "业务片区": "sales_name",
        "业务大区": "company_name",
        "所属地区": "area_l2_name",
        "经营品牌": "brand",
        "年销售规模": "annual_sales_volume",
        "直派促销员": "salesman",
        "电话": "phone",
        "传真": "fax",
        "地址": "address",
        "联系人": "contact_name",
        "联系人电话": "contact_phone",
        "备注": "note",
        "创建时间": "create_time",
        "经营模式": "operation_mode",

    };

var guid = "";
var areaTree = "";
var distriID = "";
var myCompanyInfo = top.clients.loginInfo.companyInfo;
$(function () {
    initTable();
    GetGUID();
    GetAreaTree();
    GetAllDistriId();
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

function GetAreaTree() {
    $.ajax({
        url: "/DistributorManage/DistributorManage/GetAreaTree",
        type: "get",
        async: false,
        dataType: "json",
        success: function (data) {
            areaTree = data;
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

function GetAllDistriId() {
    $.ajax({
        url: "/DistributorManage/DistributorManage/GetAllDistriId",
        type: "get",
        async: false,
        dataType: "json",
        success: function (data) {
            distriID = data;
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
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
    var columns = [];

    for (var a in gtitle) {
        var colObj = {
            field: gtitle[a],
            title: a,
            align: "center",
            sortable: true,
            width: 180,
        };
        if (gtitle[a] == "phone") {
            colObj.width = 230;
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
            //var distributor_number = list[one]["经销商ID"];
            //var length = list.length;
            //for (var l = 1; l < list.length-one; l++) {
            //    if (distributor_number == list[Number(one + l)]["经销商ID"]) {
            //        resoveFail("第" + num + "行经销商ID【" + distributor_number + "】与导入文件中第" + Number(Number(one + l) + 1) + "行经销商ID重复，请核查后重新导入！");
            //        return;
            //    }
            //}
            var distributorName = list[one]["经销商名称"];
            //var code = "";
            //if (!distributorName || distributorName.trim().length < 4) {
            //    resoveFail("第" + num + "行：经销商名称有误，请核查后重新导入！");
            //    return;
            //}            
            //else {
            //    distributorName = distributorName.trim();
            //    if (distributorName.charAt(4) == "-") {
            //        if (distributorName.length >= 6)
            //            code = distributorName.substring(0, 6);
            //        else
            //            code = distributorName.substring(0, 4);
            //    }
            //    else {
            //        code = distributorName.substring(0, 4);
            //    }
            //}

            var companyName = "";
            //var v2Name = "";
            //var areaName = list[one]["所属地区"];
            var area_l2_name = "";
            var distributor_id = "";
            var distributorID = list[one]["经销商ID"];
            var obj = {};

            for (var index in config) {
                var key = list[one][index];
                if (!key || key == "") {
                    obj[config[index]] = "";
                    continue;
                }
                key = key.trim();
                obj[config[index]] = key;
                if (index == "业务大区")
                    companyName = key;
                //else if (index == "V2报量名称")
                //    v2Name = key;
                else if (index == "所属地区")
                    area_l2_name = key;
                else if (index == "经销商ID")
                    distributor_id = key;
                else if (index == "经销商属性")
                    obj.type = (key == "自营") ? 1 : 2;
            }
            if (companyName == "") {
                resoveFail("第" + num + "行：分公司（业务大区）名称为空，请核查后重新导入！");
                return;
            }
            //else if (v2Name == "") {
            //    resoveFail("第" + num + "行：V2报量名称为空，请核查后重新导入！");
            //    return;
            //}
            else if (area_l2_name == "") {
                resoveFail("第" + num + "行：业务片区为空，请核查后重新导入！");
                return;
            }

            else if (distributorName == "" || distributorID == "") {
                resoveFail("第" + num + "行：经销商名称/经销商ID为空，请核查后重新导入！");
                return;
            }
            //if (companyName == "" || area_l2_name == "" || distributorName == "" || distributorID == "") {
            //    resoveFail("第" + num + "行：业务大区/所属地区/经销商/经销商ID不能为空，请核查后重新导入！");
            //    return;
            //}

            for (var d in distriID) {
                if (distriID[d] == distributor_id) {
                    resoveFail("第" + num + "行经销商ID【" + distributor_id + "】与系统中已有经销商ID重复，请核查后重新导入！");
                    return;
                }
            }



            var cLen = 0;
            for (var c in areaTree) {
                if (areaTree[c].company_name != companyName) {
                    cLen++; continue;
                }
                var area_l1_list = areaTree[c].area_l1_list;
                var aLen = 0;
                for (var a in area_l1_list) {                    
                    var area_l2_list = area_l1_list[a].area_l2_list;
                    var kLen = 0;
                    for (var k in area_l2_list) {
                        if (area_l2_list[k].area_l2_name != area_l2_name) {
                            kLen++; continue;
                        }
                        obj.company_id = area_l2_list[k].company_id;
                        obj.company_name = area_l2_list[k].company_name;
                        obj.company_linkname = area_l2_list[k].company_linkname; // LinkName
                        obj.company_id_parent = area_l2_list[k].company_id_parent
                        obj.area_l2_id = area_l2_list[k].area_l2_id;
                        obj.area_l2_name = area_l2_list[k].area_l2_name;
                        obj.area_l1_id = area_l2_list[k].area_l1_id;
                        obj.area_l1_name = area_l2_list[k].area_l1_name;

                        break;
                    }
                    aLen++;
                    if (kLen == area_l2_list.length && aLen == area_l1_list.length) {
                        resoveFail("第" + num + "行：系统中不存在该所属地区（业务片区）“" + area_l2_name + "”，请核查后重新导入！");
                        return;
                    }
                    else if (kLen != area_l2_list.length)   // 已匹配
                        break;
                }
                //if (false) {
                //    resoveFail("第" + num + "行：系统中不存在所属地区（小区域）“" + areaName + "”，请核查后重新导入！");
                //    return;
                //}
                break;
            }
            if (cLen == areaTree.length) {
                resoveFail("第" + num + "行：系统中不存在分公司（业务大区）“" + companyName + "”，请核查后重新导入！");
                return;
            }

            //obj.code = code;
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
    data["distributorListStr"] = JSON.stringify(importData);

    $.submitForm({
        url: "/DistributorManage/DistributorManage/Import?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/DistributorManage/DistributorManage/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
