// Submit Form START
//$.submitForm = function (options) {
//    var defaults = {
//        url: "",
//        param: [],
//        success: null,
//        close: false
//    };
//    var options = $.extend(defaults, options);
//    if ($('[name=__RequestVerificationToken]').length > 0) {
//        options.param["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
//    }
//    $.ajax({
//        url: options.url,
//        data: options.param,
//        type: "post",
//        dataType: "json",
//        success: function (data) {
//            if (data.state == "success") {
//                options.success(data);
//                $.modalMsg(data.message, data.state);
//                if (options.close) {
//                    $.modalClose();
//                }
//            } else {
//                $.modalMsg(data.message, data.state);
//            }
//        },
//        error: function (XMLHttpResponse, textStatus, errorThrown) {
//            $.loading(false);
//            $.modalMsg(errorThrown, "error");
//        },
//        beforeSend: function () {
//            $.loading(true);
//        },
//        complete: function () {
//            try {
//                $.loading(false);
//                //top.layer.close(index);
//            }
//            catch ($) { }
//        }
//    });
//}

//validate

// Submit Form END