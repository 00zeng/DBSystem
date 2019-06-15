// 后加载代码，必须放页面最后加载

// Modal START

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
// Modal END

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

// Validation START
$.fn.queryFormValidate = function () {
    $(this).validate({
        onfocusout: function (element) { $(element).valid() },
        onkeyup: false,
        errorPlacement: function (error, element) {
            element.parents('.formValue').addClass('has-error');
            element.parents('.has-error').find('i.error').remove();
            if (element[0].tagName.toLowerCase() == "select")
                element.parents('.has-error').find('.select2').after('<i class="form-control-feedbacks fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' + error + '"></i>');
            else
                element.parents('.has-error').find('.error').after('<i class="form-control-feedbacks fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' + error + '"></i>');

            $("[data-toggle='tooltip']").tooltip();
            if (element.parents('.input-group').hasClass('input-group')) {
                element.parents('.has-error').find('i.error').addClass('has-error-pullright');

            }
        },
        success: function (element) {
            element.parents('.has-error').find('i.error').remove();
            element.parent().removeClass('has-error');
        }
    });
}


$.fn.formValid = function () {
    var collapsedBox = $(this).find('.collapsed-box');
    if (collapsedBox.length > 0) {
        collapsedBox.each(function (index, value) {
            $(value).find('[data-widget="collapse"]').trigger('click');
        })
    }

    return $(this).valid({
        errorPlacement: function (error, element) {
            element.parents('.formValue').addClass('has-error');
            element.parents('.has-error').find('i.error').remove();
            if (element[0].tagName.toLowerCase() == "select")
                element.parents('.has-error').find('.select2').after('<i class="form-control-feedbacks fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' + error + '"></i>');
            else
                element.parents('.has-error').find('.error').after('<i class="form-control-feedbacks fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' + error + '"></i>');

            $("[data-toggle='tooltip']").tooltip();
            if (element.parents('.input-group').hasClass('input-group') || element.parents('.input-single').hasClass('input-single')) {
                // input-single: temp solutions -- JIANG
                element.parents('.has-error').find('i.error').addClass('has-error-pullright');

            }
        },
        success: function (element) {
            element.parents('.has-error').find('i.error').remove();
            element.parent().removeClass('has-error');
        }
    });
}

function CheckValid(obj) {
    if ($(obj).val() == "") {
        $(obj).parents('.formValue').addClass('has-error');
        $(obj).parents('.has-error').find('div.tooltip').remove();
        $(obj).parents('.has-error').find('i.error').remove();
        $(obj).parents('.has-error').find('.error').after('<i class="form-control-feedbacks fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="输入的身份证号无效,请输入有效的身份证号！"></i>');
        $("[data-toggle='tooltip']").tooltip();
        if ($(obj).parents('.input-group').hasClass('input-group')) {
            $(obj).parents('.has-error').find('i.error').addClass('has-error-pullright');
        }
        return;
    }
    $(obj).parents('.has-error').find('div.tooltip').remove();
    $(obj).parents('.has-error').find('i.error').remove();
    $(obj).parents().removeClass('has-error');
}

function checkValidForSelect2(obj) {
    if ($(obj).val() == "") {
        $(obj).parents('.formValue').addClass('has-error');
        $(obj).parents('.has-error').find('div.tooltip').remove();
        $(obj).parents('.has-error').find('i.error').remove();
        $(obj).parents('.has-error').find('.select2').after('<i class="form-control-feedbacks fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' + error + '"></i>');
        $("[data-toggle='tooltip']").tooltip();
        if ($(obj).parents('.input-group').hasClass('input-group')) {
            $(obj).parents('.has-error').find('i.error').addClass('has-error-pullright');
        }
        return;
    }
    $(obj).parents('.has-error').find('div.tooltip').remove();
    $(obj).parents('.has-error').find('i.error').remove();
    $(obj).parents().removeClass('has-error');
}
// Validation END
