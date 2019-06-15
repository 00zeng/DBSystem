$(function () {
    $("#sidebar-nav").on("click", ".nav>li>a", function () {
        $(this).addClass("active_bg").parent().siblings().removeClass("active_bg");
        $(this).find("ul").addClass("active_ul").find("li:eq(0)").addClass("active");
    });
    var data = clients.authorizeMenu;
    var _html = "";
    $.each(data, function (i) {
        var row = data[i];
        if (row.parent_code == "0") {
            _html += '<li>';//' + row.Icon + '
            _html += '<a href="#">'
                        + '<i class="' + (!!row.icon && row.icon != "" ? row.icon : "fa fa-building-o icon") + '"><b class="btn-primary"></b></i>'
                        + '<span class="pull-right"><i class="fa fa-angle-down text"></i><i class="fa fa-angle-up text-active"></i></span>'
                        + '<span>' + row.name + '</span>'
                        + '</a>';


            var childNodes = row.ChildNodes;
            if (childNodes.length > 0) {
                _html += '<ul class="nav lt">';
                $.each(childNodes, function (i) {
                    var subrow = childNodes[i];
                    _html += '<li class="menu_2nd" data-href="' + subrow.url + '" data-code="' + subrow.encode + '"><a href="javascript:;"><i></i><span>' + subrow.name + '</span> </a></li>';
                });
                _html += '</ul>';
            }
            _html += '</li>';
        }
    });
    /*$("#sidebar-nav ul").prepend(_html);
    $(".menu_2nd").on("click", function () {
        var $this = $(this);
        $(".menu_2nd.active_bg").removeClass("active_bg");
        $this.addClass("active_bg");
        $("#myFrameId").off("load");
        //$.loading(true);
        $("#myFrameId").data("code", $this.data("code"));
        $("#myFrameId").attr("src", $this.data("href")).on("load", function () {
            //$.loading(false);
        })
        return false;
    });*/
    $("#sidebar-nav ul").prepend(_html);
    $(".menu_2nd").on("click", function () {
        var $this = $(this);
        var href = $this.data("href");
        var code = $this.data("code");
        $(".menu_2nd.active_bg").removeClass("active_bg");
        $this.addClass("active_bg");
        JumpUrl(code, href);
        $("#navUrl").data("href", href).data("code", code);
        $("#navTitle").text($this.find("span").text());
        $("#navUrl").find("i").attr("class", $this.parent().prev().find("i").attr("class") + " fa_margin");
        return false;
    });
    /*屏幕改变时iframe高度也要重置*/
    $(window).resize(function () {
        calculate_h();
    })
    calculate_h();
    $("#btnCancel").on("click", function () {
        layer.confirm("确认退出系统？", {
            icon: "fa-exclamation-circle",
            title: "系统提示",
            btn: ['确认', '取消'],
            btnclass: ['btn btn-primary', 'btn btn-danger'],
        }, function () {
            $.ajax({
                url: "/Account/OutLogin",
                type: "post",
                dataType: "json",
                success: function (data) {
                    if (data.state == "success") {
                        window.location.href = "/Account/Login";
                    }
                }
            })
        }, function () {
            $("#ajaxModal").css("display", "none");
            $(".modal-backdrop").remove();
        });
    });

    $("#homePage").on("click", function () {
        JumpUrl("", "/Home/HomePage");
        $("#navTitle").text($(this).text());
        $("#navUrl").find("i").attr("class", $(this).find("i").attr("class") + " fa_margin");
    });
})

/*计算iframe的高度*/
function calculate_h() {
    var h = $(window).height() - $("#top").height() - $("#top2").height()-20;
    //console.log($(window).height(), $("#top").height(), $("#top2").height())
    $("#myFrameId").height(h);
}


//loading
$.loading = function (bool, text) {
    var $loadingpage = $("#loadingPage");
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

function PictureShow(photoJson,anim)
{
    layer.photos({
        photos: {
            "title": "相册", //相册标题
            "id": "", //相册id
            "start": 0, //初始显示的图片序号，默认0
            "data": photoJson
        },
        anim: !anim ? 1 : anim//动画效果 0-6
        , success: function (layero, index) {
            $(".layui-layer-shade:last").append("<i class='fa fa-close'style='font-size:80px;color:#fff;z-index:29891018;position:fixed; right:0px; top:0px;' id='closePicture'></i>");
        }
    });
}
//更改密码
function OpenForm(url, title) {
    $.modalOpen({
        id: "Form",
        title: title,
        url: url,
        width: "600px",
        height: "500px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}
$("#changePasswore").on("click", function () {
    var user_id = $("#userId").text();
    OpenForm('/Account/UpdatePassword', '修改密码');
});

function JumpUrl(code, href) {
    $("#myFrameId").off("load");
    //$.loading(true);
    $("#myFrameId").data("code", code)
    $("#myFrameId").attr("src", href).on("load", function () {
        //$.loading(false);
    })
}
