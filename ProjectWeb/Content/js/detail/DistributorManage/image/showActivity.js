var id = $.request("id");
var name = decodeURI($.request("name"));
$("#name").text(name);
var $tableDetail = $('#gridList');
var start_date;
var end_date;
var fileList = [];
$(function () {
    $("#querySubmit").on('click', querySubmit);
    $.ajax({
        url: "/DistributorManage/Image/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#form1").formSerializeShow(data.mainInfo);
            start_date = data.mainInfo.start_date;
            end_date = data.mainInfo.end_date;
            if (data.mainInfo.activity_status == 2 || data.mainInfo.approve_status != 100) {
                $("#btn_edit").css("display", "none");
            }
            $("#rebate").text(ToThousandsStr(data.mainInfo.rebate));
            $("#activity_target").text(ToThousandsStr(data.mainInfo.activity_target));

            var tr1 = '';
            var rebate_sum = 0;
            var total_sale_count_sum = 0;
            var total_sale_amount_sum = 0;

            $.each(data.resList, function (index, item) {

                tr1 += '<tr><td>' + item.start_date.substring(0, 10) + '至' + item.end_date.substring(0, 10) + '</td>' + '<td>' + ToThousandsStr(item.rebate) + '</td>' + '<td>' + ToThousandsStr(item.total_sale_count) + '</td>' + '<td>' + ToThousandsStr(item.total_sale_amount) + '</td></tr>';
                rebate_sum += item.rebate;
                total_sale_count_sum += item.total_sale_count;
                total_sale_amount_sum += item.total_sale_amount;

            })
            $("#gridResult tr:eq(0)").after(tr1);
            $("#rebate_sum").text(ToThousandsStr(rebate_sum));
            $("#total_sale_count_sum").text(ToThousandsStr(total_sale_count_sum));
            $("#total_sale_amount_sum").text(ToThousandsStr(total_sale_amount_sum));

            //返利发放
            if (data.mainInfo.pay_mode == 1) {
                // 一次性
                $("span[name='activity_unit']").html("元");
            }
            else if (data.mainInfo.pay_mode == 2) {
                //按月 
                $("span[name='activity_unit']").html("元/月");
            }


            //var calculateInfo = data.calculateInfo;
            //$("#form1").formSerializeShow(calculateInfo[0]);
            //$("#quantity").text(ToThousandsStr(data.calculateInfo[0].quantity));
            //$("#amount_price_wholesale").text(ToThousandsStr(data.calculateInfo[0].amount_price_wholesale));
            //$("#amount_price_retail").text(ToThousandsStr(data.calculateInfo[0].amount_price_retail));

            activity_status = data.mainInfo.activity_status;
            if (activity_status == 1) {
                $("#activity_status").text("进行中");
            } else if (activity_status == -1) {
                $("#activity_status").text("未开始");
            } else if (activity_status == 2) {
                $("#activity_status").text("已结束");
            }
            ////剩余时间
            //CalculateDay();
            ////统计时间
            if (data.mainInfo.target_mode == 2) { //销量
                $("#saleDetail").css("display", "");
                initTable();
                $("#mode3").css("display", "");
                $("#mode4").css("display", "");

            } else if (data.mainInfo.target_mode == 1) {//时间
                $("#saleDetail").css("display", "none");
                $("#mode3").css("display", "none");
                $("#mode4").css("display", "none");
            };
            //查看审批
            var tr = '';
            $("#creator_position_name").text(data.mainInfo.creator_position_name);
            $.each(data.approveList, function (index, item) {
                if (item.approve_note == null || item.approve_note == '') {
                    approve_note_null = "--";
                } else
                    approve_note_null = item.approve_note;
                if (item.status == 1 || item.status == -1)
                    approve_grade = ("一级审批");
                else if (item.status == 2 || item.status == -2)
                    approve_grade = ("二级审批");
                else if (item.status == 3 || item.status == -3)
                    approve_grade = ("三级审批");
                else if (item.status == 4 || item.status == -4)
                    approve_grade = ("四级审批");
                else if (item.status == 100 || item.status == -100)
                    approve_grade = ("终审");
                if (item.status > 0) {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                } else {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);

            $.each(data.fileList, function (i, v) {
                var index = getIndex(v.type);
                if ($('div[name="row"]:eq(' + index + ') .item').length == 0) {
                    $('div[name="picbox"]:eq(' + index + ')').show();
                    $('div[name="row"]:eq(' + index + ')').append('<div show="open" class="col-sm-6"  style="text-align: center;" name="open_div">'
                                     + '<div class="thumbnail" style="height:100px">'
                                     + '<img style="height:90px" show="open" src="' + v.url_path + '" />'
                                     + '</div>'
                                     + '</div>');
                } else {
                    $('div[name="row"]:eq(' + index + ')').append('<div show="open" class="col-sm-6"  style="text-align: center;" name="open_div">'
                                     + '<div class="thumbnail" style="height:100px">'
                                     + '<img style="height:90px" show="open" src="' + v.url_path + '" />'
                                     + '</div>'
                                     + '</div>');
                }
            })
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
    $("body").on("click", "img[show='open']", function () {
        var obj = "<img class='img-responsive' src='" + $(this).attr("src") + "'/>";
        var w_no = $(obj).prop("width");
        var h_no = $(obj).prop("height");
        if (w_no > 800) {
            var rate = w_no / 800;
            w_no = 800;
            h_no = h_no / rate;
        }
        var h = h_no + "px";
        var w = w_no + "px";
        top.layer.open({
            type: 1,
            title: false,
            area: [w, h],
            closeBtn: 0,
            skin: 'layui-layer-nobg',
            shadeClose: true,
            content: obj,
        })
    });
})
function getIndex(type) {
    var index;
    switch (type) {
        case 1:
            index = 0;
            break;
        case 99:
            index = 1;
            break;
    }
    return index;
}
//function checkTime(i) {
//    if (i < 10) {
//        i = "0" + i;
//    }
//    return i;
//}
//function CalculateDay() {
//    var nowDate = new Date();
//    var yyyy = nowDate.getFullYear();//通过日期对象的getFullYear()方法返回年
//    var MM = nowDate.getMonth() + 1;//通过日期对象的getMonth()方法返回月
//    var dd = nowDate.getDate();//通过日期对象的getDate()方法返回日
//    MM = checkTime(MM);
//    dd = checkTime(dd);
//    var now_date = yyyy + "-" + MM + "-" + dd ;
//    if (end_date == "" || now_date == "") {
//        $("#countDown").text("0天");
//    } else {
//        $("#countDown").text(DateDiff(now_date ,end_date.substring(0,10)));
//    }
//}

//function DateDiff(sDate1, sDate2) {  //sDate1和sDate2是yyyy-MM-dd格式
//    var aDate, oDate1, oDate2, oDate3, iDays;
//    aDate = sDate1.split("-");
//    oDate1 = new Date(aDate[0], aDate[1], aDate[2]); 
//    aDate = sDate2.split("-");
//    oDate2 = new Date(aDate[0], aDate[1], aDate[2]);
//    oDate1 = oDate1.valueOf();
//    oDate2 = oDate2.valueOf();
//    var oDate3 = oDate2 - oDate1;
//    oDate3 = new Date(oDate3);   
//    if (oDate3 < 0) {
//        return "0";
//    }
//    else if ((oDate3.getFullYear() - 1970) == 0 && (oDate3.getMonth()) == 0) {
//        iDays = (oDate3.getDate() - 1) + '天';
//    } else
//        iDays = oDate3.getFullYear() - 1970 + '年' + (oDate3.getMonth()) + '个月' + (oDate3.getDate() - 1) + '天';
//    return iDays;  //返回相差天数
//}

function querySubmit() {
    $tableDetail.bootstrapTable({ offset: 0 }); // 第一页
    $tableDetail.bootstrapTable('removeAll');
    $tableDetail.bootstrapTable('refresh');
}


var tableDetailHeader = [
    { field: "phone_sn", title: "串码", align: "center", sortable: true, width: 150, },
    { field: "model", title: "型号", align: "center", sortable: true, width: 150 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 100 },
    {
        field: "price_wholesale", title: "批发价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_retail", title: "零售价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
     {
         field: "time", title: "时间", align: "center", sortable: true, width: 100,
         formatter: function (value, row) {
             var date = new Date(value);
             return date.pattern("yyyy-MM-dd");
         }
     },
]

function initTable() {
    var oTable = new TableInit();
    oTable.Init();
}

var TableInit = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $tableDetail.bootstrapTable({
            height: 400,
            url: "/DistributorManage/Image/GetSaleList?date=" + new Date(),        //请求后台的URL（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageSize: 20,                       //每页的记录行数（*）
            pageList: [20, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            strictSearch: true,
            showColumns: false,                  //是否显示所有的列 
            showRefresh: false,                  //是否显示刷新按钮
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableDetailHeader,
            queryParams: function (params) {
                var param = {
                    page: (params.offset / params.limit) + 1,
                    rows: params.limit,
                    sidx: params.sort,
                    sord: params.order,
                    id: id,
                    start_date: $('#start_date').text(),
                    end_date: $('#end_date').text(),
                    startTime1: $('#startTime1').val(),
                    endTime1: $('#endTime1').val(),
                    target_content: $('#target_content').val(),
                    phone_sn: $('#phone_sn').val().trim(),
                    model: $('#model').val().trim(),

                };
                return param;
            },
            onLoadError: function (data) {
                console.log(data);
            }
        });
    };
    return oTableInit;
};

function OpenForm(url, title) {
    //if (title == "修改结束时间")
    url += "?id=" + id + "&start_date=" + start_date + "&end_date=" + end_date;
    $.modalOpen({
        id: "Form",
        title: title,
        url: url,
        width: "600px",
        height: "300px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}