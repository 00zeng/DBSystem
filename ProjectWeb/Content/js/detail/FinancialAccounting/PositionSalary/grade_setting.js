var cFullList = []; // 所有分公司
var cSelList = [];  // 已选分公司
var companyTree = [];
var $table = $('#tb_table');
var isTemplate = $("#is_template").val();
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

var importData = [];
var tableHeader =
    {
        "类型": "grade_category_display",
        "职层": "grade_level_display",
        "职等": "grade",
        "职位": "position_display",
        "基本工资": "base_salary",
        "岗位工资": "position_salary",
        "住房补助": "house_subsidy",
        "全勤奖": "attendanceReward",
        "工龄工资": "seniority_wage",
        "车费补贴": "traffic_subsidy_display",
        "KPI": "kpi",
        "合计": "total",
    };

var myCompanyInfo = top.clients.loginInfo.companyInfo;
var companyList = {};    // 上级机构列表
var guid = "";
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
    initTable();
    GetGUID();
    SelectCompany();
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
//上级机构
function BindCompany() {
    var cur_category = $("#category").val();
    if (cur_category == "分公司") {
        $("#company").css("display", "none");
        $("#template").css("display", "");
        if ($("#is_template").val() == 3) {
            $("#selectCompanyDiv").css("display", "");
        }
        BindTemplate();
    } else if (cur_category == "事业部") {
        $("#company").css("display", "");
        $("#template").css("display", "none");
        $("#selectCompanyDiv").css("display", "none");
    }
}

function SelectCompany() {
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
    if (cSelList.length < 1) {
        $("#ShowCompany").css('height','34px');
    }
}

//立即生效 
function Effective(checkbox) {
    if (checkbox.checked == true) {
        $("#effect_date").val('');
        $("#effect_date").attr("disabled", true);
    } else {
        $("#effect_date").removeAttr("disabled");
    }
}

//是否设为公版
function BindTemplate() {
    $("#is_template").on("change", function () {
        isTemplate = $("#is_template").val();
        if (isTemplate == 3) {
            $("#selectCompanyDiv").css("display", "");
        } else {
            $("#selectCompanyDiv").css("display", "none");
        }
    })
}

function setModel() {
    if ($("#category").val() == "事业部") {
        $("#model2").css("display", "");
        $("#model1").css("display", "none");
    } else if ($("#category").val() == "分公司") {
        $("#model1").css("display", "");
        $("#model2").css("display", "none");
    }
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
    var columns = []; //; [{ field: "id", title: "", align: "center", edit: false, formatter: function (value, row, index) { return index; }}]
    //   var firstcolumns = [{ field: "check", title: "", checkbox: true, }]
    for (var a in gtitle) {
        var colObj = {
            field: gtitle[a],
            title: a,
            align: "center",
            width: 100,

        };
        if (gtitle[a] == "base_salary" || gtitle[a] == "position_salary"
            || gtitle[a] == "house_subsidy" || gtitle[a] == "attendance_reward" || gtitle[a] == "seniority_wage"
            || gtitle[a] == "traffic_subsidy_display" || gtitle[a] == "total") {
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
    var category_save = 0;
    var category_display = "";
    var level_save = 0;
    var level_display = "";
    var position_display = "";
    var attendance_reward = "";
    var traffic_subsidy_display = "";
    if (list.length > 0) {
        for (var one in list) {
            var obj = {};
            if (!list[one]["类型"] && category_save == 0)
                continue;
            for (var index in config) {
                var key = list[one][index];
                if (!key) {
                    if (index == "类型" && category_save > 0) {
                        obj['grade_category'] = category_save;
                        obj[config[index]] = category_display;
                    }
                    else if (index == "职层" && level_save > 0) {
                        obj['grade_level'] = level_save;
                        obj[config[index]] = level_display;
                    }
                    else if (index == "职位") {
                        obj["position_display"] = position_display;
                    }
                    //else if (index == "全勤奖") { 
                    //    if (key == null || key=="-" || key=="")
                    //        key = 0;
                    //    //obj['attendance_reward'] = key;
                    //}
                    else
                        obj[config[index]] = "";
                } else {
                    obj[config[index]] = key;
                    if (index == "类型") {
                        category_display = key;
                        if (key.indexOf("行政管理") > -1) {
                            category_save = 1;
                            obj['grade_category'] = 1;
                        }
                        else if (key.indexOf("市场销售") > -1) {
                            category_save = 2;
                            obj['grade_category'] = 2;
                        }
                        else if (key.indexOf("终端销售") > -1) {
                            category_save = 3;
                            obj['grade_category'] = 3;
                        }
                        else {
                            category_save = 4;
                            obj['grade_category'] = 4;
                        }
                    }
                    else if (index == "职层") {
                        level_display = key;
                        if (key.indexOf("公司") > -1) {
                            level_save = 1;
                            obj['grade_level'] = 1;
                        }
                        else {
                            level_save = 2;
                            obj['grade_level'] = 2;
                        }
                    }
                    else if (index == "职位") {
                        position_display = key;
                    }
                    else if (index == "全勤奖") {
                            if (key == null || key=="-" || key=="")
                                key = 0;
                            obj['attendance_reward'] = key;
                    }
                }
            }
            
            obj.salary_position_id = guid;
            importData.push(obj);
        }
        $table.bootstrapTable('load', importData);
    }
    $table.bootstrapTable('hideLoading');
}

$('#btnReturn').click(function () {
    if (!!importData && importData.length > 0) {
        top.layer.confirm("确定放弃将数据导入到系统中？", function (index) {
            top.layer.closeAll();
            window.history.go(-2);
        })
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
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    //公版
    if (isTemplate == 3 && $("#category").val() == "分公司") {
        data["companyList"] = cSelList;
        if (!cSelList || cSelList.length < 1) {
            $.modalAlert("请选择分公司");
            return;
        }
    }
    data["is_template"] = isTemplate;
    data["id"] = guid;
    data["file_name"] = $("#excelfile").val()
    if (!data["file_name"]) {
        alert("请选择导入文件");
        return;
    }
    if ($("#effect_date").val() == '' && $("#effect_now").prop("checked") == false) {
        $.modalAlert("请选择生效时间");
        return;
    }
    data["effect_date"] = $("#effect_date").val();
    //data["category"] = ($("#category").val() == "事业部" ? 1 : 2); //前端临时占用category字段传值

    if ($("#category").val() == "事业部") {
        data["is_template"] = 0;
        data["category"] = 1;
    } else if ($("#category").val() == "分公司") {
        data["category"] = 2;
        if (isTemplate == 3 ) {
            data["companyList"] = cSelList;
            if (!cSelList || cSelList.length < 1) {
                $.modalAlert("请选择分公司");
                return;
            }
        }
    }
    data["company_id"] = myCompanyInfo.id;
    data["effect_now"] = $("#effect_now").prop("checked");
    data["importListStr"] = JSON.stringify(importData);
    $.submitForm({
        url: "/FinancialAccounting/PositionSalary/Import?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/PositionSalary/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
