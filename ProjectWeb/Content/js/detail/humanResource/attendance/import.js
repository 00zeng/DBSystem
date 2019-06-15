var $table = $('#tb_table');
var importData = [];
var tableHeader =
    {
        "姓名": "emp_name",
        "应出勤\r\n(天)": "work_days",
        "实际出勤\r\n(天)": "attendance",
    };

var guid = "";
$(function () {
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
            strictSearch: true,
            // showColumns: true,                  //是否显示所有的列 
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
            width: 200,
        };
        //if (gtitle[a] == "accur_time") {
        //    colObj.width = 180;
        //    colObj.formatter = (function (value) { var date = new Date(value); return date.pattern("yyyy-MM-dd"); });
        //}
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
    var month = $("#month").val();
    if (list.length > 0) {
        for (var one in list) {
            var obj = {};
            for (var index in config) {
                var key = list[one][index];
                if (!key) {
                    obj[config[index]] = "";
                } else {
                    obj[config[index]] = key;
                }
            }
            var rate = (obj.attendance / obj.work_days) * 100;
            obj.attendance_rate = rate;           
            obj.month = month;
            obj.import_file_id = guid;
            importData.push(obj);
        }
        $table.bootstrapTable('load', importData);
    }
    $table.bootstrapTable('hideLoading');
}

//function resoveFail(msg) {
//    $table.bootstrapTable('removeAll');
//    $table.bootstrapTable('hideLoading');
//    importData = [];    // 清空上一次的数据
//    $.modalAlert(msg, "error");
//}
function MonthChange()
{
    if (!importData || importData.length < 1) 
        return;
    var month = $("#month").val();
    $.each(importData, function (i) {
        importData[i].month = month;
    });
    
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
    if ($("#month").val().trim() == "") {
        $.modalAlert("请选择考勤月份", "error");
        return;
    }
    if (!importData || importData.length < 1) {
        $.modalAlert("无数据可导入", "error");
        return;
    }
    var data = {};
    data["import_file"] = $("#excelfile").val()
    if (!data["import_file"]) {
        $.modalAlert("请选择导入文件", "error");
        return;
    }
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    data["id"] = guid;
    data["importListStr"] = JSON.stringify(importData);

    $.submitForm({
        url: "/HumanResource/attendance/Import?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/attendance/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
