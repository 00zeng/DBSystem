var id = $.request("id");
$(function () {
    $.ajax({
        url: "/HumanResource/attendance/GetInfoMain?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            
            $("#import_file").html(data.import_file);
            $("#month").val(data.month);
            //console.log(data.listPriceInfo);
            //console.log(data.listPriceInfo[0]);
            //console.log(data.listPriceInfo.length);

            //for (var i = 0; i < data.listPriceInfo.length; i++) {
            //    console.log(data.listPriceInfo[i]);
            //    $("tb_table").bootstrapTable('load', data.listPriceInfo[i]);
            //}
            var columns = inittitle(tableHeader);
            //1.初始化Table
            var oTable = new TableInit(data.listInfo, columns);
            oTable.Init();
            //resoveresult(tableHeader, data.listPriceInfo);

            $("#creator_name").text(data.creator_name);
            $("#creator_position_name").text(data.creator_position_name);
            $("#create_time").text(data.create_time);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})
var $table = $('#tb_table');
var importData = [];
var tableHeader =
    {
        "姓名": "emp_name",
        "应出勤(天)": "work_days",
        "实际出勤(天)": "attendance",
        "出勤率": "attendance_rate",
    };

//$(function () {
//    initTable();
//});

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
            url: '/HumanResource/attendance/GetInfoPage',         //请求后台的URL（*）
            //data: data,
            method: 'get',                      //请求方式（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "asc",                   //排序方式
            //queryParams: '',                    //传递参数（*）
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageNumber: 1,                       //初始化加载第一页，默认第一页
            pageSize: 10,                       //每页的记录行数（*）
            pageList: [10, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            strictSearch: true,
            //showColumns: true,                  //是否显示所有的列 
            //uniqueId: "no",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: columns,
            queryParams: function (params) {
                var pageInfo = {
                    id: id,
                    rows: params.limit,                              //页面大小
                    page: (params.offset / params.limit) + 1,        //页码
                };
                return pageInfo;
            },
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
            width: 140,
        };
        if (gtitle[a] == "attendance_rate") {
            colObj.width = 180;
            colObj.formatter = (function (value) { return value+"%"; });
        }
        columns.push(colObj);
    }
    return columns;
}


//function importfile(file) {//导入
//    var f = file.files[0];
//    $("#excelfile").val(f.name);
//    var wb;//读取完成的数据
//    var rABS = false; //是否将文件读取为二进制字符串
//    var ie = IEVersion();
//    if (ie != -1 && ie != 'edge') {
//        if (ie < 10) {
//            return;
//        } else {
//            rABS = true;
//        }
//    }

//    if (checkfilename(file)) {
//        var reader = new FileReader();
//        reader.onload = function (e) {
//            var data = e.target.result;
//            if (rABS) {
//                wb = XLSX.read(btoa(fixdata(data)), {//手动转化
//                    type: 'base64'
//                });
//            } else {
//                wb = XLSX.read(data, {
//                    type: 'binary'
//                });
//            }
//            var result = XLSX.utils.sheet_to_json(wb.Sheets[wb.SheetNames[0]]);

//            resoveresult(tableHeader, result);
//        };

//        if (rABS) {
//            reader.readAsArrayBuffer(f);
//        } else {
//            reader.readAsBinaryString(f);
//        }
//    }
//}

function resoveresult(config, list) {
    $table.bootstrapTable('showLoading');
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
            obj.id = Number(one);
            importData.push(obj);
        }
        $table.bootstrapTable('load', importData);
    }
    $table.bootstrapTable('hideLoading');
}
