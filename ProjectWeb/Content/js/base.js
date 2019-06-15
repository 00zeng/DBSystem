//base.js
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

//set value for text.
$.fn.formSerializeShow = function (formdate) {
    var element = $(this);
    if (!!formdate) {
        for (var key in formdate) {
            var $id = element.find('#' + key);
            var value = $.trim(formdate[key]).replace(/&nbsp;/g, '');
            var type = $id.attr('type');
            switch (type) {
                case "select":
                    $id.val(value).trigger("change");
                    break;
                case "date":
                    if (value != "") {
                        if (value.length > 10)
                            $id.text(value.substring(0, 10));
                    }
                    break;
                default:
                    if ($id.hasClass("convert_datetime") && value != "") {
                        //var datetime = new Date(parseInt(value.replace("/Date(", "").replace(")/", "").split("+")[0]));
                        //value = datetime.getFullYear() + "-" + (datetime.getMonth() + 1) + "-" + datetime.getDate()
                        //        + " " + datetime.getHours() + ":" + datetime.getMinutes() + ":" + datetime.getSeconds();
                        var date = new Date(parseInt(value.replace("/Date(", "").replace(")/", "").split("+")[0]));
                        value = date.pattern("yyyy-MM-dd HH:mm:ss");
                    }else if ($id.hasClass("convert_date") && value != "") {
                        //var datetime = new Date(parseInt(value.replace("/Date(", "").replace(")/", "").split("+")[0]));
                        //value = datetime.getFullYear() + "-" + (datetime.getMonth() + 1) + "-" + datetime.getDate()
                        //        + " " + datetime.getHours() + ":" + datetime.getMinutes() + ":" + datetime.getSeconds();
                        var date = new Date(parseInt(value.replace("/Date(", "").replace(")/", "").split("+")[0]));
                        value = date.pattern("yyyy-MM-dd");
                    } else if ($id.hasClass("format_date") && value != "") {
                        var date = new Date(value);
                        value = date.pattern("yyyy-MM-dd");
                    } else if ($id.hasClass("effect_date") && value != "") {
                        var date = new Date(value);
                        value = date.pattern("yyyy-MM");
                    }
                    $id.text(value);
                    break;
            }
        };
        return false;
    }
    var postdata = {};
    element.find('span,select').each(function (r) {
        var $this = $(this);
        var id = $this.attr('id');
        var name = $this.attr('name');
        var type = $this.attr('type');
        var value = $this.text() == "" || $this.text() == null ? "" : $this.text();
        if (!$.request("keyValue")) {
            value = value.toString().replace(/&nbsp;/g, '');
        }
        postdata[id] = value;

    });
    if ($('[name=__RequestVerificationToken]').length > 0) {
        postdata["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    return postdata;
};

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

//alert
$.modalAlert = function (content, type) {
    var icon = 0;
    switch (type) {
        case "success": icon = 1; break;
        case "error": icon = 2; break;
        case "warning": icon = 3; break;
    };
    top.layer.alert(content, {
        icon: icon,
        title: "系统提示",
        btn: ['确认'],
        btnclass: ['btn btn-primary'],
    });
}
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
                    //if ($id.hasClass("Wdate") && value != "") {
                    //    var date = new Date(value);
                    //    value = date.pattern("yyyy-MM-dd");
                    //}
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

//submit form
$.submitForm = function (options) {
    var defaults = {
        url: "",
        param: [],
        loading: "正在提交数据...",
        success: null,
        close: false
    };
    var options = $.extend(defaults, options);
    if ($('[name=__RequestVerificationToken]').length > 0) {
        options.param["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.ajax({
        url: options.url,
        data: options.param,
        type: "post",
        dataType: "json",
        success: function (data) {
            if (data.state == "success") {
                options.success(data);
                $.modalMsg(data.message, data.state);
                if (options.close) {
                    $.modalClose();
                }
            } else {
                $.modalMsg(data.message, data.state);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.loading(false);
            $.modalMsg(errorThrown, "error");
        },
        beforeSend: function () {
            $.loading(true, options.loading);
        },
        complete: function () {
            try {
                $.loading(false);
                //top.layer.close(index);
            }
            catch ($) { }
        }
    });
}

//confirm
$.modalConfirm = function (content, callBack) {
    top.layer.confirm(content, {
        icon: "fa-exclamation-circle",
        title: "系统提示",
        btn: ['确认', '取消'],
        btnclass: ['btn btn-primary', 'btn btn-danger'],
    }, function () {
        callBack(true);
    }, function () {
        callBack(false)
    });
}

//layer message
$.modalMsg = function (content, type) {
    if (type != undefined) {
        var icon = 0;
        if (type == 'success') {
            icon = 1;
        }
        if (type == 'error') {
            icon = 2;
        }
        top.layer.msg(content, { icon: icon, time: 4000, shift: 5 });
    } else {
        top.layer.msg(content);
    }
}

//open iframe
$.modalOpen = function (options) {
    var defaults = {
        id: null,
        title: '系统窗口',
        width: "100px",
        height: "100px",
        url: '',
        shade: 0.3,
        btn: ['确认', '关闭'],
        btnclass: ['btn btn-primary', 'btn btn-danger'],
        callBack: null,
        canGreaterThan: false//可否大于当前页面宽高度
    };
    var options = $.extend(defaults, options);
    var _width = options.width;
    var _height = options.height;
    if (!options.canGreaterThan) {
        _width = top.$(window).width() > parseInt(options.width.replace('px', '')) ? options.width : top.$(window).width() + 'px';
        _height = top.$(window).height() > parseInt(options.height.replace('px', '')) ? options.height : top.$(window).height() + 'px';
    }
    top.layer.open({
        id: options.id,
        type: 2,
        shade: options.shade,
        title: options.title,
        fix: false,
        area: [_width, _height],
        content: options.url,
        btn: options.btn,
        btnclass: options.btnclass,
        yes: function (index) {
            if (options.callBack != null) {
                options.callBack("layui-layer-iframe" + index);
            }
            else {
                top.layer.close(index);
            }
            //options.callBack(options.id)
            //top.layer.close(index);
            //if (options.callBack != null)
            //    options.callBack("layui-layer-iframe" + index,index);
        }, cancel: function () {
            return true;
        }
    });
}
//close window
$.modalClose = function () {
    var index = top.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
    var $IsdialogClose = top.$("#layui-layer" + index).find('.layui-layer-btn').find("#IsdialogClose");
    var IsClose = $IsdialogClose.is(":checked");
    if ($IsdialogClose.length == 0) {
        IsClose = true;
    }
    if (IsClose) {
        top.layer.close(index);
    } else {
        location.reload();
    }
}
//video
$.ShowVideo = function (options) {
    var defaults = {
        SWF_ID: "KwVideo",
        SWF_NAME: "http://" + window.location.host + "/Content/swf/flvplayer.swf",

        ContainerID: "videoDiv",//容器id-播放器id
        ContainerWrapID: $("#videoDivhtmlid"),//播放器父类id

        VideoUrl: "",//视频地址

        params: {//视频需要的参数
            //play: "false",
            //loop: "true",
            allowFullscreen: "true",
            allowScriptAccess: "always",
            //wmode: "transparent",
        },
        Width: "100%",//视频宽度
        Height: "645",//视频高度
    };
    var options = $.extend(defaults, options);
    //var html = [];
    //html[html.length] = '<div id="' + options.ContainerID + '">';
    ////html[html.length] = '<a href="javascript:playflashmv(\'https://get2.adobe.com/cn/flashplayer/\');" class="butm_middle"></a>';
    //html[html.length] = '</div>';
    //$(options.ContainerWrapID).html(html.join(""));

    ////old
    //var flashvars = {
    //    url: options.VideoUrl
    //};
    var flashvars = {
        vcastr_file: options.VideoUrl,
        BufferTime: "3",
        IsAutoPlay: "1",
        IsContinue: "1",
        IsShowBar: "0"
    };

    var attributes = {
        id: options.ContainerID
    };
    swfobject.embedSWF(options.SWF_NAME, options.ContainerID, options.Width, options.Height, "10.0.0", "expressInstall.swf", flashvars, options.params, attributes);
}

//获取当前iframe
$.currentWindow = function () {
    return top.frames["myFrameId"];
}

//获取当前iframe
$.currentaOperate = function () {
    var objLayer = top.$("[id^='layui-layer-iframe']");
    if (objLayer.length == 1)
        return top.frames["myFrameId"];
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

//列表-gridview
$.fn.dataGrid = function (options) {
    var defaults = {
        datatype: "json",
        autowidth: true,
        rownumbers: true,
        shrinkToFit: false,
        gridview: true,
        loadcomplete: null
    };
    var options = $.extend(defaults, options);
    var $element = $(this);
    $element.jqGrid(options);
};

//validate
$.fn.formValid = function () {
    return $(this).valid({
        errorPlacement: function (error, element) {
            element.parents('.formValue').addClass('has-error');
            element.parents('.has-error').find('i.error').remove();
            element.parents('.has-error').append('<i class="form-control-feedbacks fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' + error + '"></i>');
            $("[data-toggle='tooltip']").tooltip();

            if (element.parents('.input-group').hasClass('input-group')) {
                element.parents('.has-error').find('i.error').css('right', '5px')
            }
        },
        success: function (element) {
            element.parents('.has-error').find('i.error').remove();
            element.parent().removeClass('has-error');
        }
    });
}

//reload
$.reload = function () {
    location.reload();
    return false;
}

//loading
$.loading = function (bool, text) {
    var $loadingpage = top.$("#loadingPage");
    var $loadingtext = $loadingpage.find('.loading-content');
    if (bool) {
        $loadingpage.show();
    } else {
        if ($loadingtext.attr('istableloading') == undefined) {
            $loadingpage.hide();
        }
    }
    if (!!text) {
        $loadingtext.html(text);
    } else {
        $loadingtext.html("数据加载中，请稍后…");
    }
    $loadingtext.css("left", (top.$('body').width() - $loadingtext.width()) / 2 - 50);
    $loadingtext.css("top", (top.$('body').height() - $loadingtext.height()) / 2);
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

$.fn.bindNormalSelect = function (options) {
    var defaults = {
        id: "id",
        text: "name",
        url: "",
        param: [],
        firstText: null,
        lastText: null,
        lastValue: null,
        change: null,//onchange事件调用
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
                if (!!options.lastText)
                    $element.append($("<option></option>").val(options.lastValue).html(options.lastText));
                if (options.change != null) {
                    $element.on("change", function () {
                        options.change();
                    });
                }
            }
        });
    }
}

//$.fn.authorizeButton = function () {
//    var moduleId = top.$("#myFrameId").data("code");
//    var dataJson = top.clients.authorizeButton[moduleId];
//    var $element = $(this);
//    $element.find('.priv').attr('privilege', 'no');
//    if (dataJson != undefined) {
//        $.each(dataJson, function (i) {
//            $element.find("[name='" + dataJson[i].encode + "']").attr('privilege', 'yes');
//        });
//    }
//    if ($element.find("[privilege=no]").length > 0)
//        $element.find("[privilege=no]").remove();
//}

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