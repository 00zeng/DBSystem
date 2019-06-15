var $table = $('#tb_table');
var reg = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
var importData = [];
var tableHeader =
    {
        "工号": "work_number",
        "姓名": "name",
        "V2姓名": "name_v2",
        "职务": "position_name",
        "区域": "company_name",//分公司
        "售点地区": "area_l2_name",
        "售点": "distributor_name",
        "证件号码": "identity",
        "生日": "birthday",
        "微信号": "wechat",
        "介绍人": "introducer_name",
        "类别": "guide_category",
        "健康证有效期起": "health_start",
        "健康证有效期止": "health_expire",
        "工作地点": "work_addr",
        "部门": "dept_name",
        "班组": "",
        "入职日期": "entry_date",
        "证件类别": "identity_type",
        "身份证有效期起": "identity_effect",
        "身份证有效期止": "identity_expire",
        "签发机关": "identity_issue",
        "学历": "education",
        "专业": "profession",
        "毕业学校": "graduation_school",
        "毕业日期": "graduation_date",
        "雇员类别": "emp_category",
        "性别": "gender_show",
        "工龄": "",
        "职级": "grade",
        "工资": "",
        "银行类别": "bank_type",
        "开户行": "bank_name",
        "银行账号": "bank_account",
        "出生日期": "birthdate",
        "年龄": "age",
        "国家或地区": "country",
        "纳税单位": "tax_unit",
        "离职日期": "",
        "离职押金": "",
        "应扣押金": "",
        "其他": "",
        "应退押金": "",
        "离职方式": "",
        "上报人": "",
        "离职原因": "",
        "籍贯": "native",
        "家庭住址": "address",
        "电话号码": "phone",
        "婚姻状况": "marriage_show",
        "民族": "nation",
        "政治面貌": "religion",
        "紧急联系人": "emergency_contact",
        "紧急联系电话": "emergency_contact_phone",
        "上级主管姓名": "supervisor_name",
        "合同签署日期": "cur_contract_sign",
        "合同到期日期": "cur_contract_expire",
        "父母银行卡开户银行": "parents_bank",
        "父母银行卡号": "parents_bankaccount",
        "父母联系电话": "parents_phone",
        "备注": "note",
    };

var guid = "";
var posList = "";
var areaList = "";
var gradeList = "";
var empID = "";
var myCompanyInfo = top.clients.loginInfo.companyInfo;
$(function () {
    if (myCompanyInfo.category != "事业部") {
        $.modalAlert("出库信息须由事业部人员导入", "error")
        window.history.go(-1);
        return;
    }
    myCompanyInfo.name;
    initTable();
    GetPosList();
    GetAreaList();
    GetAllEmpId();
});


function GetPosList() {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetPosList",
        type: "get",
        async: false,
        dataType: "json",
        success: function (data) {
            posList = data;
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}
function GetAreaList() {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetAreaList",
        type: "get",
        async: false,
        dataType: "json",
        success: function (data) {
            areaList = data;
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}
function GetAllEmpId() {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetAllEmpId",
        type: "get",
        async: false,
        dataType: "json",
        success: function (data) {
            empID = data;
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
            width: 200,
        };
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
    var value = $('input:radio[name="company"]:checked').val();
    var num = 1;
    if (list.length > 0) {
        for (var one in list) {
            //var work_number = list[one]["工号"];
            //var length=list.length;
            //for (var l = 1; l < list.length-one; l++) {
            //    if (work_number == list[Number(one + l)]["工号"]) {
            //        resoveFail("第" + num + "行员工工号【" + work_number + "】与导入文件中第" + Number(Number(one + l) + 1 )+ "行员工工号重复，请核查后重新导入！");
            //        return;
            //    }
            //}
            var name = list[one]["姓名"];
            var name2 = list[one]["V2姓名"];
            var name_v2 = "";
            if (!(list[one]["V2姓名"]) || list[one]["V2姓名"] == "") {
                name_v2 = name;
            }
            else {
                name_v2 = name2;
            }
            var obj = {};
            var work_number = list[one]["工号"];

            var companyName = "";
            var positionName = "";
            var areaL2 = "";
            var grade = "";
            var gender = "";
            var identity = "";
            var emp_id = "";
            for (var index in config) {
                var key = list[one][index];
                if (!key || key == "") {
                    obj[config[index]] = "";
                    continue;
                }
                key = key.trim();
                if (config[index] == "health_start" || config[index] == "entry_date" || config[index] == "identity_effect"
                    || config[index] == "identity_expire" || config[index] == "graduation_date"
                    || config[index] == "cur_contract_sign" || config[index] == "cur_contract_expire") {
                    if (key.indexOf("年") > 0 || key.indexOf("月") > 0 || key.indexOf("日") > 0) { // 只做简单判断
                        key = key.replace('年', '-');
                        key = key.replace('月', '-');
                        key = key.replace('日', '');
                    }
                    var date = new Date(key);
                    obj[config[index]] = date.pattern("yyyy-MM-dd");

                }
                else if (config[index] == "birthday") {
                    var date = new Date(key);
                    obj[config[index]] = date.pattern("MM-dd");
                }
                else if (config[index] == "gender_show") {
                    if (key == "男") {
                        obj.gender = 1;
                    } else if (key == "女") {
                        obj.gender = 0;
                    }
                    obj[config[index]] = key;
                }
                else if (config[index] == "marriage_show") {
                    if (key == "未婚") {
                        obj.marriage = 0;
                    }
                    else if (key == "已婚") {
                        obj.marriage = 1;
                    }
                    else if (key == "离异") {
                        obj.marriage = 2;
                    }
                    else if (key == "丧偶") {
                        obj.marriage = 3;
                    }
                    else if (key == "其他") {
                        obj.marriage = 4;
                    }
                    obj[config[index]] = key;
                }
                else {
                    obj[config[index]] = key;
                    if (index == "职务")
                        positionName = key;
                    else if (index == "区域")
                    {
                        if (value == 1)
                            companyName = key;
                    }
                    else if (index == "售点地区")
                    {
                        if (value == 1)
                            areaL2 = key;
                    }
                    else if (index == "职级")
                        grade = key;
                    else if (index == "证件号码")
                        identity = key;
                    else if (index == "工号")
                        emp_id = key;
                }
            }

            for (var e in empID) {
                if (empID[e] == emp_id) {
                    resoveFail("第" + num + "行员工工号【" + emp_id + "】与系统中已有员工工号重复，请核查后重新导入！");
                    return;
                }
            }

            var clen = 0;
            for (var c in posList) {
                if (posList[c].name != positionName) {
                    clen++; continue;
                }
                else {
                    if (value == 1 && posList[c].company_name != companyName) {
                        clen++; continue;
                    }
                    else {
                        obj.company_category = posList[c].company_category;
                        obj.company_id = posList[c].company_id;
                        obj.company_id_parent = posList[c].company_id_parent;
                        obj.company_linkname = posList[c].company_linkname;
                        obj.company_name = posList[c].company_name;
                        obj.dept_id = posList[c].dept_id;
                        obj.dept_name = posList[c].dept_name;
                        obj.position_id = posList[c].id;
                        obj.position_name = posList[c].name;
                        obj.position_type = posList[c].position_type;
                        break;
                    }
                }
            }
            if (clen == posList.length) {
                resoveFail("第" + num + "行：无法匹配到该分公司-职务数据 【" + companyName + "  " + " --- " + positionName + "】，请核查后重新导入！");
                return;
            }


            if (value == 1) {
                var alen = 0;
                if (areaL2 != "" && areaL2 != null)
                    for (var c in areaList) {
                        if (areaL2 == areaList[c].name) {
                            obj.area_l1_name = areaList[c].parent_name;
                            obj.area_l1_id = areaList[c].parent_id;
                            obj.area_l2_name = areaL2;
                            obj.area_l2_id = areaList[c].id;
                            break;
                        } else {
                            alen++;
                            continue;
                        }
                    }
                if (alen == areaList.length) {
                    resoveFail("第" + num + "行：无法匹配到该售点地区【" + areaL2 + "】，请核查后重新导入！");
                    return;
                }
            }



            if (reg.test(identity) == false) {
                resoveFail("第" + num + "行：身份证【" + identity + "】输入格式有误！");
                return;
            }
            obj.id = work_number;
            obj.name_v2 = name_v2;
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
    data["empInfoListStr"] = JSON.stringify(importData);
    data["empJobListStr"] = JSON.stringify(importData);

    $.submitForm({
        url: "/HumanResource/EmployeeManage/Import?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/EmployeeManage/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
