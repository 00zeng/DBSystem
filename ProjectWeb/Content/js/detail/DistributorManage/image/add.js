var myCompanyInfo = top.clients.loginInfo.companyInfo;

var picJson2 = [];//用于提交到数据库的 json 数据
var checkTypeVal = $('input[name="pay_mode"]:checked').val();//返利发放
//上传图片
function OpenForm() {
    var url;
    var multiple = true;
    var type;
    switch (index) {
        case 0:
            type = 1;
            break;
        case 1:
            type = 99;
            break;
    }
    url = "/File/UploadPictures?multiple=" + multiple + "&type=" + type + "&src=2&module=2";
    $.modalOpen({
        id: "File",
        title: "上传图片",
        url: url,
        width: "650px",
        height: "360px",
        btn: ["关闭"]
    });
}
$(function () {
    $("#sale_avg_before").text(0); // TODO 获取前3个月月均销量
    ActionBinding();
    BindCompany();


    $("button[add='upload']").on("click", function () {
        index = $("button[add='upload']").index(this);
        OpenForm();
        //CallBackPictures();
    })

    $("body").on("click", "i[del='del']", function () {
        Del(this)
    })
    //查看图片
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
});

/*图片上传成功回调函数*/
function CallBackPictures(data) {
    var obj = $("button[add='upload']").eq(index);
    top.layer.msg("上传成功");
    picJson2.push(data);
    $('div[name="picbox"]:eq(' + index + ')').show();
    $('div[name="row"]:eq(' + index + ')').append('<div show="open" class="col-sm-6"  style="text-align: center;" name="open_idv">'
                                 + '<div class="thumbnail" style="height:100px">'
                                 + '<img style="height:76px" show="open" src="' + data.url_path + '" />'
                                 + '<i class="fa fa-trash trash" del="del"></i>'
                                 + '</div>'
                                 + '</div>');

}

/*上传错误删除按钮事件*/
function Del(obj) {
    //获取最外层父元素
    //$(e).closest('tr')[0].rowIndex
    var path = $(obj).siblings("img").attr("src");
    var p_index = $(($(obj).parent().parent().parent())[0]).children().length;//图片长度
    var img_src = $(obj).prev().attr("src");//获取图片路径
    $.each(picJson2, function (index, item) {
        if (item.url_path == img_src) {
            picJson2.splice(index, 1);
            return;
        }
    })
    if (p_index == 1) {//剩一个删除 隐藏div
        $(obj).parent().parent().parent().parent().hide();
    }
    $(obj).parent().parent().remove();
    var data = {};
    data["file_path"] = path;
    $.ajax({
        url: "/File/DeleteLocalFile?date=" + new Date(),
        data: data,
        type: "post",
        dataType: "json",
        async: "false",
        success: function (data) {
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

function Month() {
    var startdate = $("#start_date").val();
    var enddate = $("#end_date").val();
    if (startdate.substring(0, 7) == enddate.substring(0, 7)) {
        $("input[name='pay_mode'][value='2']").attr("disabled", "disabled");
    }else
        $("input[name='pay_mode'][value='2']").removeAttr("disabled");
}

$('input[name="pay_mode"]').on("change", function () {
    var checkTargetVal = $('input[name="pay_mode"]:checked').val();
     if (checkTargetVal == 2) {
         document.getElementById('start_date').onfocus = function () {
             var date1 = limitMonthDate(1);
             WdatePicker({ readOnly: true,maxDate: date1 });
         }
         document.getElementById('end_date').onfocus = function () {
             var date2 = limitMonthDate(2);
             WdatePicker({ readOnly: true, minDate: date2});
         }

     } else {
         document.getElementById('start_date').onfocus = function () {
             WdatePicker({ readOnly: true, maxDate: '#F{$dp.$D(\'end_date\')}' });
         }
         document.getElementById('end_date').onfocus = function () {
             WdatePicker({ readOnly: true, minDate: '#F{$dp.$D(\'start_date\')}' });
         }
     }
})

function limitMonthDate(e) {
    var DateString;
    if (e == 2) {
        var beginDate = $dp.$("start_date").value;
        if (beginDate != "" && beginDate != null) {
            var limitDate = new Date(beginDate);
            limitDate.setDate(new Date(limitDate.getFullYear(), limitDate
                    .getMonth() + 1, 0).getDate()+1); //获取此月份的天数  
            DateString = limitDate.getFullYear() + '-'
                    + (limitDate.getMonth() + 1) + '-'
                    + limitDate.getDate();
            return DateString;
        }
    }
    if (e == 1) {
        var endDate = $dp.$("end_date").value;
        if (endDate != "" && endDate != null) {
            var limitDate = new Date(endDate);
            limitDate.setDate("0"); 
            DateString = limitDate.getFullYear() + '-'
                    + (limitDate.getMonth() + 1) + '-'
                    + limitDate.getDate();
            return DateString;
        }
    }

}

$("#distributor_id").on("change", function () {
    GetAvgBefore();
})

var sale_avg;

//前三个目标销量
function GetAvgBefore() {
    var strId = $("#distributor_id").val();
    var str1 = "|";
    strId = strId + str1;

    $.ajax({
        url: "/DistributorManage/DistributorManage/GetAvgBefore?idStr=" + strId,
        type: "get",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            sale_avg = data;
            $("#sale_avg_before").text(data);
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

//返利发放
$('input[name="pay_mode"]').on("change", function () {
    checkTypeVal = $('input[name="pay_mode"]:checked').val();//返利发放
    if (checkTypeVal == 1) {
        // 一次性
        $("span[name='activity_unit']").html("元");

    } else if (checkTypeVal == 2) {
        //按月 
        $("span[name='activity_unit']").html("元/月");

    }
})

function ActionBinding() {
    $("#company_id").on("change", function () {
        GetDistributorList();
    })
    $("input[name=target_mode]").on("change", function () {
        if ($("input[name=target_mode]:checked").val() == 2) {
            //$("#day").text('');
            $("#mode1").css("display", "none");
            $("#mode2").css("display", "none");
            $("#mode3").css("display", "");
            $("#mode4").css("display", "");
        } else if ($("input[name=target_mode]:checked").val() == 1) {
            $("#activity_target").val('');
            //$("input[name=target_content]:checked").attr('checked', false);
            $("#mode1").css("display", "");
            $("#mode2").css("display", "");
            $("#mode3").css("display", "none");
            $("#mode4").css("display", "none");
        };
    })
}

function BindCompany() {
    if (myCompanyInfo.category == "分公司") {
        $("#company_id").append("<option value='" + myCompanyInfo.id + "' selected>" + myCompanyInfo.name + "</option>");
    } else if (myCompanyInfo.category == "事业部") {
        $.ajax({
            url: "/OrganizationManage/Company/GetIdNameList",
            async: false,
            type: "get",
            dataType: "json",
            contentType: "application/json",
            success: function (data) {
                $.each(data, function (index, value) {
                    $("#company_id").append("<option value='" + value.id + "'>" + value.name + "</option>");
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
    GetDistributorList();
}

function GetDistributorList() {
    $("#distributor_id").empty();
    $("#distributor_id").bindSelect({
        text: "name",
        url: "/DistributorManage/DistributorManage/GetIdNameList",
        search: true,
        firstText: "--请选择经销商--",
    });
   
    
}

function CalculateDay() {
    var start_date = $("#start_date").val();
    var end_date = $("#end_date").val();
    if (start_date == "" || end_date == "") {
        $("#day").text("0天");
    } else {
        //var startNum = parseInt(start_date.replace(/-/g, ''), 10);
        //var endNum = parseInt(end_date.replace(/-/g, ''), 10);
        $("#day").text(DateDiff(start_date, end_date));
    }
}

function DateDiff(sDate1, sDate2) {  //sDate1和sDate2是yyyy-MM-dd格式
    var aDate, oDate1, oDate2,oDate3, iDays;
    aDate = sDate1.split("-");
    oDate1 = new Date(aDate[0] , aDate[1], aDate[2]);  //转换为yyyy-MM-dd格式
    aDate = sDate2.split("-");
    oDate2 = new Date(aDate[0], aDate[1], aDate[2]);
    oDate1 = oDate1.valueOf();
    oDate2 = oDate2.valueOf();
    var oDate3 = oDate2 - oDate1;
    oDate3 = new Date(oDate3);
    iDays = oDate3.getFullYear() - 1970 + '年' + (oDate3.getMonth()) + '个月' + (oDate3.getDate() - 1) + '天';
    return iDays;  //返回相差天数
}

function submitForm() {
    // 提交验证
    if (!$("#form1").formValid())
        return false;
    var data = $("#form1").formSerialize();
    data["day"] = $("#day").html();
    data["sale_avg_before"] = sale_avg;
    data["image_list"] = picJson2;
    $.submitForm({
        url: "/DistributorManage/Image/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/DistributorManage/Image/Index";

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
    
