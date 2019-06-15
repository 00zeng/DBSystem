// 后加载代码，必须放页面前面加载
//get param
$.request = function (name) {
    var search = location.search.slice(1);
    var arr = search.split("&");
    for (var i = 0; i < arr.length; i++) {
        var ar = arr[i].split("=");
        if (ar[0] == name) {
            if (unescape(ar[1]) == 'undefined') {
                return "";
            } else {
                return unescape(ar[1]);
            }
        }
    }
    return "";
}




// DateTime START
$.ChangeTimeStamp = function (timeStamp, transform) {
    if (timeStamp == undefined || timeStamp == null || timeStamp == "")
        return "";
    s = timeStamp;
    s.replace(/Date\([\d+]+\)/, function (a) { eval('d = new ' + a) });
    return d.pattern(transform == undefined ? "yyyy-MM-dd" : transform);
}

Date.prototype.pattern = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份        
        "d+": this.getDate(), //日        
        "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12, //小时        
        "H+": this.getHours(), //小时        
        "m+": this.getMinutes(), //分        
        "s+": this.getSeconds(), //秒        
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度        
        "S": this.getMilliseconds() //毫秒        
    };
    var week = {
        "0": "\u65e5",
        "1": "\u4e00",
        "2": "\u4e8c",
        "3": "\u4e09",
        "4": "\u56db",
        "5": "\u4e94",
        "6": "\u516d"
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    if (/(E+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, ((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "\u661f\u671f" : "\u5468") : "") + week[this.getDay() + ""]);
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        }
    }
    return fmt;
}

/**
Date add by interval
@param interval y  Year,m  Month,d  Day,w  Week
@param number

*/
Date.prototype.dateAdd = function (interval, number) {
    switch (interval) {
        case "y":
            return new Date(this.getFullYear() + number, this.getMonth(), this.getDate());
            break;
        case "m":
            return new Date(this.getFullYear(), this.getMonth() + number, checkDate(this.getFullYear(), this.getMonth() + number, this.getDate()));
            break;
        case "d":
            return new Date(this.getFullYear(), this.getMonth(), this.getDate() + number);
            break;
        case "w":
            return new Date(this.getFullYear(), this.getMonth(), 7 * number + this.getDate());
            break;
    }
}

function checkDate(year, month, date) {
    var enddate = ["31", "28", "31", "30", "31", "30", "31", "31", "30", "31", "30", "31"];
    var returnDate = "";
    if (year % 4 == 0) {
        enddate[1] = "29";
    }
    if (date > enddate[month]) {
        returnDate = enddate[month];
    } else {
        returnDate = date;
    }
    return returnDate;
}

/**
 * 两个日期相隔长度（加1）
 */
function dateInterval(interval, sDate1, sDate2) {
    if (sDate1 == sDate2) {
        return 0;
    }
    var date1 = new Date(sDate1);
    var date2 = new Date(sDate2);
    var yLen = date2.getFullYear() - date1.getFullYear();
    var mLen = date2.getMonth() - date1.getMonth();
    switch (interval) {
        case "y":
            return yLen + 1;
            break;
        case "m":
            return yLen * 12 + mLen + 1;;
            break;
        case "d":
            return (parseInt(Math.abs(date2 - date1) / 1000 / 60 / 60 / 24)) + 1;
            break;
        //case "q":
        //    return new Date(this.getFullYear(), this.getMonth(), 7 * number + this.getDate());
        //    break;
    }
    //if (!sDate1) {
    //    var date = new Date();
    //    var year = date.getFullYear();
    //    var month = date.getMonth() + 1;
    //    var day = date.getDate();
    //    sDate1 = year + "-" + month + "-" + day;
    //}
    //var aDate, oDate1, oDate2, iDays;
    //aDate = sDate1.split("-");
    //oDate1 = new Date(aDate[1] + '-' + aDate[2] + '-' + aDate[0]);//转换为Mm-dd-yyyy格式,这种date的构造方式在苹果手机会报错，见解释
    //aDate = sDate2.split("-");
    //oDate2 = new Date(aDate[1] + '-' + aDate[2] + '-' + aDate[0]);
    //iDays = parseInt(Math.abs(oDate1 - oDate2) / 1000 / 60 / 60 / 24);    //把相差的毫秒数转换为天数  
    //return iDays;
}

// Datetime END
// 千位分隔符 START
function ToThousandsStr(data) {
    if (data == undefined || data == null)
        return "";
    var source = String(data).split(".");//按小数点分成2部分
    source[0] = source[0].replace(new RegExp('(\\d)(?=(\\d{3})+$)', 'ig'), "$1,");//只将整数部分进行都好分割
    return source.join(".");//再将小数部分合并进来
}
// 千位分隔符 END
// BindSelect START

//var str = "js实现用{two}自符串替换占位符{two} {three}  {one} ".format({one: "I",two: "LOVE",three: "YOU"});
//var str2 = "js实现用{1}自符串替换占位符{1} {2}  {0} ".format("I","LOVE","YOU");
String.prototype.format = function () {
    if (arguments.length == 0) return this;
    var param = arguments[0];
    var s = this;
    if (typeof (param) == 'object') {
        for (var key in param)
            s = s.replace(new RegExp("\\{" + key + "\\}", "g"), param[key]);
        return s;
    } else {
        for (var i = 0; i < arguments.length; i++)
            s = s.replace(new RegExp("\\{" + i + "\\}", "g"), arguments[i]);
        return s;
    }
}


$.fn.bindSelectLocal = function (options) {
    var defaults = {
        id: "id",
        text: "text",
        search: false,
        data: [],
        change: null,
        tags: false,
        firstText: null,
    };
    var options = $.extend(defaults, options);
    var $element = $(this);
    var data = options.data;
    if (!!options.firstText)
        $element.append($("<option></option>").val("").html(options.firstText));
    $.each(data, function (i) {
        $element.append($("<option></option>").val(data[i][options.id]).html(data[i][options.text]));
    });
    $element.select2({
        minimumResultsForSearch: options.search == true ? 0 : -1, tags: options.tags
    });
    $element.on("change", function (e) {
        if (options.change != null) {
            options.change(data[$(this).find("option:selected").index()]);
        }
        $("#select2-" + $element.attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
    });
}

$.fn.bindSelect = function (options) {
    var defaults = {
        id: "id",
        text: "text",
        search: false,
        url: "",
        param: [],
        change: null,
        tags: false,
        firstText: null,
    };
    var options = $.extend(defaults, options);
    var $element = $(this);
    if (options.url != "") {
        $.ajax({
            url: options.url,
            data: options.param,
            dataType: "json",
            async: false,
            success: function (data) {
                if (!!options.firstText)
                    $element.append($("<option></option>").val("").html(options.firstText));
                $.each(data, function (i) {
                    $element.append($("<option></option>").val(data[i][options.id]).html(data[i][options.text]));
                });
                $element.select2({
                    minimumResultsForSearch: options.search == true ? 0 : -1, tags: options.tags
                });
                $element.on("change", function (e) {
                    if (options.change != null) {
                        options.change(data[$(this).find("option:selected").index()]);
                    }
                    $("#select2-" + $element.attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
                });
            }
        });
    } else {
        $element.select2({
            minimumResultsForSearch: -1
        });
    }
}
// BindSelect END

$.fn.authorizeButton = function () {
    var moduleId = top.$("#myFrameId").data("code");
    var dataJson = top.clients.authorizeButton[moduleId];
    var d_len = (dataJson == undefined ? 0 : dataJson.length);
    var $element = $(this).find('.auth');
    var e_len = $element.length;
    var remove = true;
   // $element.attr('privilege', 'no');
    if (e_len < 1 || d_len < 1)
        return;

    $.each($element, function (e) {
        var name = $($element[e]).attr("name");
        $.each(dataJson, function (d) {
            if (dataJson[d].encode == name) {
                $($element[e]).show();
                remove = false;
                return;
            }
        });
        if (remove)
            $element[e].remove();
        else
            remove = true;
    });
}
//set value for text.
// 注：radio组 - 需在其中一个radio加"id=KEY"属性
$.fn.formSerializeShow = function (formdate) {
    var element = $(this);
    if (!!formdate) {
        for (var key in formdate) {
            var $id = element.find('#' + key);
            //if (!$id || $id.length < 1)
            //    $id = element.find("name['" + key + "']");
            if (!$id || $id.length < 1)
                continue;
            var value = $.trim(formdate[key]).replace(/&nbsp;/g, '');
            // 时间格式化
            if ($id.hasClass("convert_datetime") && value != "") {
                value = (new Date(parseInt(value.replace("/Date(", "").replace(")/", "").split("+")[0]))).pattern("yyyy-MM-dd HH:mm:ss");
            } else if ($id.hasClass("convert_date") && value != "") {
                value = (new Date(parseInt(value.replace("/Date(", "").replace(")/", "").split("+")[0]))).pattern("yyyy-MM-dd");
            } else if ($id.hasClass("format_date") && value != "") {
                value = (new Date(value)).pattern("yyyy-MM-dd");
            } else if ($id.hasClass("format_date_ym") && value != "") {
                value = (new Date(value)).pattern("yyyy-MM");
            } else if ($id.hasClass("format_date_md") && value != "") {
                value = (new Date(value)).pattern("MM-dd");
            }

            if ($id[0].tagName.toLowerCase() == "select") {
                $id.val(value).trigger("change");
                continue;
            }
            if ($id[0].tagName.toLowerCase() != "input") {
                $id.text(value);
                continue;
            }
            var type = $id.attr('type');
            switch (type) {
                case "checkbox":
                    if (value == "true") {
                        $id.attr("checked", 'checked');
                    } else {
                        if (value == "false")
                            $id.removeAttr("checked");
                        else {
                            var str = value.split('|');
                            for (var s in str) {
                                if (str[s] != "")
                                    $("input[name='" + key + "']").filter("[value='" + str[s] + "']").attr("checked", "checked");
                            }
                        }
                    }
                    break;
                case "radio":
                    $("[name='" + key + "']").filter("[value='" + value + "']").attr("checked", "checked");
                    break;
                case "date":
                    if (value != "") {
                        if (value.length > 10)
                            $id.text(value.substring(0, 10));
                    }
                    break;
                case "text":
                    // 有误
                    //if ($id.hasClass("Wdate") && value != "") {
                    //    var date = new Date(value);
                    //    value = date.pattern("yyyy-MM-dd");
                    //}
                    $id.val(value);
                    break;
                default:
                    $id.text(value);
                    break;
            }
        };
        return false;
    }
};
//set or get value for input
$.fn.formSerialize = function (formdate) {
    var element = $(this);
    if (!!formdate) {
        for (var key in formdate) {
            var $id = element.find('#' + key);
            var value = $.trim(formdate[key]).replace(/&nbsp;/g, '');
            var type = $id.attr('type');
            if ($id.hasClass("select2-hidden-accessible")) {
                type = "select";
            }
            switch (type) {
                case "checkbox":
                    if (value == "true") {
                        $id.attr("checked", 'checked');
                    } else {
                        if (value == "false")
                            $id.removeAttr("checked");
                        else {
                            var str = value.split('|');
                            for (var s in str) {
                                if (str[s] != "")
                                    $("input[name='" + key + "']").filter("[value='" + str[s] + "']").attr("checked", "checked");
                            }
                        }
                    }
                    break;
                case "radio":
                    $("[name='" + key + "']").filter("[value='" + value + "']").attr("checked", "checked");
                    break;
                case "select":
                    $id.val(value).trigger("change");
                    break;
                case "date":
                    if (value != "") {
                        if (value.length > 10)
                            $id.val(value.substring(0, 10));
                    }
                    break;
                default:
                    if ($id.hasClass("Wdate") && value != "") {
                        var date = new Date(value);
                        value = date.pattern("yyyy-MM-dd");
                    }
                    $id.val(value);
                    break;
            }
        };
        return false;
    }
    var postdata = {};
    element.find('input,select,textarea').each(function (r) {
        var $this = $(this);
        var id = $this.attr('id');
        var name = $this.attr('name');
        var type = $this.attr('type');
        switch (type) {
            case "checkbox":
                var len = $("[name='" + name + "']").length;
                if (len == 1) {
                    postdata[id] = $this.is(":checked");
                }
                else {
                    postdata[name] = postdata[name] == undefined ? "" : postdata[name];
                    postdata[name] += $this.is(":checked") ? ("|" + $this.val()) : "";
                }
                break;
            case "radio":
                var len = $("[name='" + name + "']:checked").length;
                if (len > 0)
                    postdata[name] = $("[name='" + name + "']:checked").val();
                else
                    postdata[name] = "";
                break;
            default:
                var value = $this.val() == "" || $this.val() == null ? "" : $this.val();
                if (!$.request("keyValue")) {
                    value = value.toString().replace(/&nbsp;/g, '');
                }
                postdata[id] = value;
                break;
        }
    });
    if ($('[name=__RequestVerificationToken]').length > 0) {
        postdata["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    return postdata;
};

//loading
$.loading = function (bool, text) {
    var $loadingpage = top.$("#loadingPage");
    if (bool) {
        $loadingpage.show();
    } else {
        $loadingpage.hide();
    }
}

//获取当前iframe
$.currentWindow = function () {
    var iframeId = "myFrameId";
    return top.frames["myFrameId"];
}


//获取当前iframe
$.currentaOperate = function () {
    
    var objLayer = top.$("[id^='layui-layer-iframe']");
    var iframeShow = top.$(".page-tabs-content").find(".active");//获取当前显示的页面 的data-cookie -- by Zeng 
    var iframeName = iframeShow.attr("data-cookie")
    if (objLayer.length == 1)
        return top.frames["iframe" + iframeName];
        //return parent.window.document.getElementById("iframe" + iframeName);

    else {
        var obj = null;
        objLayer.each(function () {
            if ($(this).attr("id") != window.name) {
                obj = top.frames[$(this).attr("id")];
                return false;
            }
        });
        return obj;
    }
}


