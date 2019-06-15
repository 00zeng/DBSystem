$(function () {

    var data = clients.authorizeMenu;
    var _html = "";
    $.each(data, function (i) {
        var row = data[i];
        if (row.parent_code == "0") {
            _html += '<li class="treeview">';//' + row.Icon + '
            _html += '<a href="#">'
                    + '<i class="' + (!!row.icon && row.icon != "" ? row.icon : "fa fa-building-o icon") + '"></i>'
                    + '<span>' + row.name + '</span><span class="pull-right-container"><i class="fa fa-angle-left pull-right"></i></span></a>';
            var childNodes = row.ChildNodes;
            if (childNodes.length > 0) {
                _html += '<ul class="treeview-menu">';
                $.each(childNodes, function (i) {
                    var subrow = childNodes[i];
                    _html += '<li class="menu_2nd treeview  menuTag" data-href="' + subrow.url + '" data-code="' + subrow.encode + '" >'
                                + '<a  href="javascript:;"><i class="fa fa-circle-o"></i><span>' + subrow.name + '</span>';
                    var grandNodes = subrow.ChildNodes;
                    if (!grandNodes || grandNodes.length > 0) {
                        _html += '<span class="pull-right-container"><i class="fa fa-angle-left pull-right"></i></span></a>'
                                    + '<ul class="treeview-menu">';
                        $.each(grandNodes, function (i) {
                            var grandrow = grandNodes[i];
                            _html += '<li class="menu_3rd menuTag" data-href="' + grandrow.url + '" data-code="' + grandrow.encode + '" >'
                                        + '<a  href="javascript:;"><i class="fa fa-circle-o"></i><span>' + grandrow.name + '</span></a>';
                        });
                        _html += '</ul>';
                    } else
                        _html += '</a>';
                    _html += '</li>';
                });
                _html += '</ul>';
            }
            _html += '</li>';
        }
    });
    $("#sidebar-menu").prepend(_html);

    //$(".menu_2nd").on("click", function () {
    //    var $this = $(this);
    //    //$(".menu_2nd.active").removeClass("active");
    //    //$(".menu_3rd.active").removeClass("active");
    //    $this.toggleClass("active");
    //    if ($this.find("ul").length > 0)
    //        return;
    //    $(".menu_2nd.menu-open").removeClass("menu-open");
    //    var href = $this.data("href");
    //    var code = $this.data("code");
    //    JumpUrl(code, href);
    //    $("#navUrl").data("href", href).data("code", code);
    //    $("#nav1stTitle").text($this.parent().parent().find("span").first().text());
    //    $("#nav2ndTitle").text(">\xa0\xa0\xa0" + $this.find("span").text());
    //    $("#nav3rdTitle").text('');
    //    $("#navUrl").find("i").attr("class", $this.parent().prev().find("i").attr("class") + " fa_margin");
    //    return false;
    //});

    //$(".menu_3rd").on("click", function () {
    //    var $this = $(this);
    //    var href = $this.data("href");
    //    var code = $this.data("code");
    //    //$(".menu_2nd.active").removeClass("active");
    //    $(".menu_3rd.active").removeClass("active");
    //    $this.addClass("active");
    // //   JumpUrl(code, href);
    //    $("#navUrl").data("href", href).data("code", code);
    //    $("#nav1stTitle").text($this.parent().parent().parent().parent().find("span").first().text());
    //    $("#nav2ndTitle").text(">\xa0\xa0\xa0" + $this.parent().parent().find("span").first().text());
    //    $("#nav3rdTitle").text(">\xa0\xa0\xa0" + $this.find("span").text());
    //    $("#navUrl").find("i").attr("class", $this.parent().prev().find("i").attr("class") + " fa_margin");
    //    return false;
    //});
    /*屏幕改变时iframe高度也要重置*/
    $(window).resize(function () {
        calculate_h();
        calculate_nav_h();

    })
    calculate_h();
    calculate_nav_h();
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
                },
                error: function (data) {
                    $.modalAlert(data.responseText, 'error');
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

    $.ajax({
        async: false,
        type: "get",
        url: "/Home/GetNoticeTaskList?date=" + new Date(),
        dataType: "json",
        success: function (data) {
            for (var i = 0; i < data.noticeList.length; i++) {
                $("#noticeTitle").append('<li class="notice" data-id="' + data.noticeList[i].id + '" data-href="' + data.noticeList[i].main_url
                + '"><a  href="javascript:;" title="'+data.noticeList[i].title+'"><i class="fa fa-envelope-o text-blue"></i><span>' + data.noticeList[i].title + '</span></a></li>');
            }

            for (var i = 0; i < data.taskList.length; i++) {
                $("#taskTitle").append('<li class="task" data-id="' + data.taskList[i].id + '" data-href="' + data.taskList[i].main_url
                + '"><a  href="javascript:;" title="' + data.taskList[i].title + '"><i class="fa fa-envelope-o text-blue"></i><span>' + data.taskList[i].title + '</span></a></li>');
            }
            var noticenum = data.noticeList.length;
            var tasknum = data.taskList.length;
            $("#noticeLength").text(noticenum);
            $("#taskLength").text(tasknum);
            $("#noticeLength1").text(noticenum);
            $("#taskLength1").text(tasknum);

            $(".notice").on("click", function () {
                var $this = $(this);
                var href = $this.data("href");
                var id = $this.data("id");
                $(".notifications-menu.open").removeClass("open");
                if (href == "") {
                    $("#nav1stTitle").text('主页');
                    $("#nav2ndTitle").text('');
                    $("#nav3rdTitle").text('');
                } else {
                    $("#nav1stTitle").text($this.find("span").text());
                    $("#nav2ndTitle").text('');
                    $("#nav3rdTitle").text('');
                }
                if ($this.find("i").hasClass("fa-envelope-o")) {
                    noticenum = noticenum - 1;
                    $("#noticeLength").text(noticenum);
                    $("#noticeLength1").text(noticenum);
                    $this.find("i").removeClass("fa-envelope-o");
                    $this.find("i").addClass("fa-envelope-open-o");
                }
                else if ($this.find("i").hasClass("fa-envelope-open-o")) {
                    $("#noticeLength").text(noticenum);
                    $("#noticeLength1").text(noticenum);
                }

                $.ajax({
                    type: "post",
                    url: "/Home/ReadNews",
                    data: { id: id },
                    dataType: "json",
                });

                JumpUrl("", href);
                return false;
            });

            $(".task").on("click", function () {
                var $this = $(this);
                var href = $this.data("href");
                var id = $this.data("id");
                $("#nav1stTitle").text($this.find("span").text());
                $("#nav2ndTitle").text('');
                $("#nav3rdTitle").text('');
                $(".tasks-menu.open").removeClass("open");
                $this.remove();
                tasknum = tasknum - 1;
                $("#taskLength").text(tasknum);
                $("#taskLength1").text(tasknum);

                $.ajax({
                    type: "post",
                    url: "/Home/ReadTask",
                    data: { id: id },
                    dataType: "json",
                });

                JumpUrl("", href);
                return false;
            });
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
});

})

/*计算iframe的高度*/
function calculate_h() {
    var h = $(window).height() - $("#navbar-static-top").height() - $(".content-tabs").height() - 30 - 3 - 5; // 5-unknown;
    //console.log($(window).height(), $("#top").height(), $("#top2").height())
    $(".mainContent").height(h);
}



//function calculate_h() {
//    var h = $(window).height() - $("#navbar-static-top").height() - $(".content-tabs").height() - 30 - 3 - 5; // 5-unknown;
//    //console.log($(window).height(), $("#top").height(), $("#top2").height())
//    $("#myFrameId").height(h);
//}

//侧边栏
function calculate_nav_h() {
    var nav_h = $(window).height() - $("#navbar-static-top").height()- 10 ;
    //console.log($(window).height(), $("#top").height(), $("#top2").height())
    $(".sidebar").height(nav_h);
}

function PictureShow(photoJson, anim) {
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
    $("#loadingPage").show();
    $("#myFrameId").off("load");
    $("#myFrameId").data("code", code)
    $("#myFrameId").attr("src", href).on("load", function () {
        $("#loadingPage").hide();
    })
}

function showsub(li){ 
    var submenu=li.getElementsByTagName("ul")[0]; 
    submenu.style.display="block"; 
} 
function hidesub(li){ 
    var submenu=li.getElementsByTagName("ul")[0]; 
    submenu.style.display="none"; 
}