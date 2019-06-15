var dataNav1 = '';
var dataNav2 = '';
var dataNav3 = '';
(function ($) {
    $.dinftab = {
        requestFullScreen: function () {
            var de = document.documentElement;
            if (de.requestFullscreen) {
                de.requestFullscreen();
            } else if (de.mozRequestFullScreen) {
                de.mozRequestFullScreen();
            } else if (de.webkitRequestFullScreen) {
                de.webkitRequestFullScreen();
            }
        },
        exitFullscreen: function () {
            var de = document;
            if (de.exitFullscreen) {
                de.exitFullscreen();
            } else if (de.mozCancelFullScreen) {
                de.mozCancelFullScreen();
            } else if (de.webkitCancelFullScreen) {
                de.webkitCancelFullScreen();
            }
        },
        refreshTab: function () {
            var currentId = $('.page-tabs-content').find('.active').attr('data-id');
            var target = $('.Daoben_iframe[data-id="' + currentId + '"]');
            var url = target.attr('src');
            $.loading(true);
            target.attr('src', url).on("load", function () {
                $.loading(false);
            });
        },
        activeTab: function () {
            var currentId = $(this).data('id');
            var curNav1 = $(this).data('nav1');
            var curNav2 = $(this).data('nav2');
            var curNav3 = $(this).data('nav3');
            if (!$(this).hasClass('active')) {
                var dataId = $(this).attr('data-cookie');//权限id
                //if (dataId != "") {
                //    top.$.cookie('dinf_currentmoduleid', dataId, { path: "/" });
                //}
                $('.mainContent .Daoben_iframe').each(function () {
                    if ($(this).data('id') == currentId) {
                        $(this).show().siblings('.Daoben_iframe').hide();
                        return false;
                    }
                });

                if (curNav1 == '') {
                    $("#nav1stTitle").text("首页");
                    $("#nav2ndTitle").text("");
                    $("#nav3rdTitle").text("");
                } else {
                    $("#nav1stTitle").text(curNav1);
                    $("#nav2ndTitle").text(">\xa0\xa0\xa0" + curNav2);
                    if (curNav3 == "") {
                        $("#nav3rdTitle").text(curNav3);
                    } else {
                        $("#nav3rdTitle").text(">\xa0\xa0\xa0" + curNav3);
                    }
                }
                
                $(this).addClass('active').siblings('.menuTab').removeClass('active');
                $.dinftab.scrollToTab(this);
            }
        },
        closeOtherTabs: function () {
            $('.page-tabs-content').children("[data-id]").find('.fa-remove').parents('a').not(".active").each(function () {
                $('.Daoben_iframe[data-id="' + $(this).data('id') + '"]').remove();
                $(this).remove();
            });
            $('.page-tabs-content').css("margin-left", "0");
        },
        changeMenuTitle: function (menuName) {
            var operateTab = $(".page-tabs-content").find(".active");
            var cTab = operateTab.find("i").detach();
            $(".page-tabs-content").find(".active").text(menuName).append(cTab);
        },
        closeCurrentTab: function () {
            $(".page-tabs-content").find(".active").find("i.fa-remove").trigger("click");
        },
        closeTab: function () {
            var closeTabId = $(this).parents('.menuTab').data('id');
            var currentWidth = $(this).parents('.menuTab').width();
            if ($(this).parents('.menuTab').hasClass('active')) {
                if ($(this).parents('.menuTab').next('.menuTab').length > 0) {
                    var activeId = $(this).parents('.menuTab').next('.menuTab:eq(0)').data('id');
                    var obj = $(this).parents('.menuTab').next('.menuTab:eq(0)');
                    var dataId = obj.attr('data-cookie');//权限id
                    //if (dataId != "") {
                    //    top.$.cookie('dinf_currentmoduleid', dataId, { path: "/" });
                    //}
                    obj.addClass('active');
                    $('.mainContent').find(".Daoben_iframe").each(function () {
                        if ($(this).data('id') == activeId) {
                            $(this).show().siblings('.Daoben_iframe').hide();
                            return false;
                        }
                    });
                    var marginLeftVal = parseInt($('.page-tabs-content').css('margin-left'));
                    if (marginLeftVal < 0) {
                        $('.page-tabs-content').animate({
                            marginLeft: (marginLeftVal + currentWidth) + 'px'
                        }, "fast");
                    }
                    $(this).parents('.menuTab').remove();
                    $('.mainContent').find(".Daoben_iframe").each(function () {
                        if ($(this).data('id') == closeTabId) {
                            $(this).remove();
                            return false;
                        }
                    });
                }
                if ($(this).parents('.menuTab').prev('.menuTab').length > 0) {
                    var activeId = $(this).parents('.menuTab').prev('.menuTab:last').data('id');
                    //$(this).parents('.menuTab').prev('.menuTab:last').addClass('active');
                    var obj = $(this).parents('.menuTab').prev('.menuTab:last');
                    var dataId = obj.attr('data-cookie');//权限id
                    //if (dataId != "") {
                    //    top.$.cookie('dinf_currentmoduleid', dataId, { path: "/" });
                    //}
                    obj.addClass('active');
                    $('.mainContent').find(".Daoben_iframe").each(function () {
                        if ($(this).data('id') == activeId) {
                            $(this).show().siblings('.Daoben_iframe').hide();
                            return false;
                        }
                    });
                    $(this).parents('.menuTab').remove();
                    $('.mainContent').find(".Daoben_iframe").each(function () {
                        if ($(this).data('id') == closeTabId) {
                            $(this).remove();
                            return false;
                        }
                    });
                }
            }
            else {
                $(this).parents('.menuTab').remove();
                $('.mainContent').find(".Daoben_iframe").each(function () {
                    if ($(this).data('id') == closeTabId) {
                        $(this).remove();
                        return false;
                    }
                });
                $.dinftab.scrollToTab($('.menuTab.active'));
            }
            return false;
        },
        addTab: function () {
             dataNav1='';
             dataNav2='';
             dataNav3='';
            if ($(this).hasClass('menu_3rd')) {
                dataNav1 = $(this).parent().parent().parent().parent().find("span").first().text();
                dataNav2 =$(this).parent().parent().find("span").first().text();
                dataNav3 =  $(this).find("span").text();
                $("#nav1stTitle").text(dataNav1);
                $("#nav2ndTitle").text(">\xa0\xa0\xa0" + dataNav2);
                $("#nav3rdTitle").text(">\xa0\xa0\xa0" + dataNav3);

            } else if ($(this).hasClass('menu_2nd')) {
                //if ($(this).attr('data-href') == "") {
                //    return false;
                //} else {

                //}
                dataNav1 = $(this).parent().parent().find("span").first().text();
                dataNav2 = $(this).find("span").first().text();
                dataNav3 = '';
                $("#nav1stTitle").text(dataNav1);
                $("#nav2ndTitle").text(">\xa0\xa0\xa0" + dataNav2);
                $("#nav3rdTitle").text(dataNav3);
            }
            $("#header-nav>ul>li.open").removeClass("open");
            var dataId = $(this).attr('data-code');
            //if (dataId != "") {
            //    top.$.cookie('nfine_currentmoduleid', dataId, { path: "/" });
            //}
            var dataUrl = $(this).attr('data-href');
            var menuName = $.trim($(this).text());
            
            var flag = true;
            if (dataUrl == undefined || $.trim(dataUrl).length == 0) {
                return false;
            }
            $('.menuTab').each(function () {
                if ($(this).data('id') == dataUrl) {
                    if (!$(this).hasClass('active')) {
                        $(this).addClass('active').siblings('.menuTab').removeClass('active');
                        $.dinftab.scrollToTab(this);
                        $.dinftab.refreshTab(this);
                        $('.mainContent .Daoben_iframe').each(function () {
                            if ($(this).data('id') == dataUrl) {
                                $(this).show().siblings('.Daoben_iframe').hide();
                                return false;
                            }
                        });
                    }
                    else
                        $.dinftab.refreshTab(this);

                    flag = false;
                    return false;
                }
            });
            if (flag) {
                var str = '<a href="javascript:;" class="active menuTab" data-cookie="' + dataId + '" data-id="' + dataUrl + '" data-nav1="' + dataNav1 + '" data-nav2="' + dataNav2 + '" data-nav3="' + dataNav3 + '">' + menuName + ' <i class="fa fa-remove"></i></a>';
                $('.menuTab').removeClass('active');
                var str1 = '<iframe class="Daoben_iframe" id="iframe' + dataId + '" name="iframe' + dataId + '"  width="100%" height="100%" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '"  data-nav1="' + dataNav1 + '" data-nav2="' + dataNav2 + '" data-nav3="' + dataNav3 + '" seamless></iframe>';
                $('.mainContent').find('iframe.Daoben_iframe').hide();
                $('.mainContent').append(str1);
                $.loading(true);
                $('.mainContent iframe:visible').on('load', function () {
                    $.loading(false);
                });
                $('.menuTabs .page-tabs-content').append(str);
                $.dinftab.scrollToTab($('.menuTab.active'));
            }
            return false;
        },
        addChildTab: function (obj, fun) {//iframe窗体子页面跳转显示选项卡
            $("#header-nav>ul>li.open").removeClass("open");
            dataNav1 = '';
            dataNav2 = '';
            dataNav3 = '';
            dataNav1 = top.dataNav1;
            dataNav2 = top.dataNav2;
            dataNav3 = top.dataNav3;
            $("#nav1stTitle").text(dataNav1);
            $("#nav2ndTitle").text(">\xa0\xa0\xa0" + dataNav2);
            $("#nav3rdTitle").text(">\xa0\xa0\xa0" + dataNav3);
            var dataId = $(this).data('code');
            //if (dataId != "") {
            //    top.$.cookie('dinf_currentmoduleid', dataId, { path: "/" });
            //}
            var dataUrl = $(this).data('href');
            var menuName = $.trim($(this).data('btndetail'));
            var flag = true;
            if (dataUrl == undefined || $.trim(dataUrl).length == 0) {
                return false;
            }
            top.$('.menuTab').each(function () {
                if ($(this).data('id') == dataUrl) {
                    if (!$(this).hasClass('active')) {
                        $(this).addClass('active').siblings('.menuTab').removeClass('active');
                        $.dinftab.scrollToTab(this);
                        top.$('.mainContent .Daoben_iframe').each(function () {
                            if ($(this).data('id') == dataUrl) {
                                $(this).show().siblings('.Daoben_iframe').hide();
                                return false;
                            }
                        });
                    }

                    flag = false;
                    return false;
                }
            });
            if (flag) {
                var str = '<a href="javascript:;" class="active menuTab" data-cookie="' + dataId + '" data-id="' + dataUrl + '"  data-nav1="' + dataNav1 + '" data-nav2="' + dataNav2 + '" data-nav3="' + dataNav3 + '">' + menuName + ' <i class="fa fa-remove"></i></a>';
                top.$('.menuTab').removeClass('active');
                var str1 = '<iframe class="Daoben_iframe" id="iframe' + dataId + '" name="iframe' + dataId + '"  width="100%" height="100%" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '"  data-nav1="' + dataNav1 + '" data-nav2="' + dataNav2 + '" data-nav3="' + dataNav3 + '" seamless></iframe>';
                top.$('.mainContent').find('iframe.Daoben_iframe').hide();
                top.$('.mainContent').append(str1);
                $.loading(true);
                top.$('.mainContent iframe:visible').on("load", function () {
                    $.loading(false);
                    if (fun != undefined) {
                        fun.Transfer();
                    }
                });
                top.$('.menuTabs .page-tabs-content').append(str);
                $.dinftab.scrollToTab($('.menuTab.active'));
            }
            return false;
        },
        scrollTabRight: function () {
            var marginLeftVal = Math.abs(parseInt($('.page-tabs-content').css('margin-left')));
            var tabOuterWidth = $.dinftab.calSumWidth($(".content-tabs").children().not(".menuTabs"));
            var visibleWidth = $(".content-tabs").outerWidth(true) - tabOuterWidth;
            var scrollVal = 0;
            if ($(".page-tabs-content").width() < visibleWidth) {
                return false;
            } else {
                var tabElement = $(".menuTab:first");
                var offsetVal = 0;
                while ((offsetVal + $(tabElement).outerWidth(true)) <= marginLeftVal) {
                    offsetVal += $(tabElement).outerWidth(true);
                    tabElement = $(tabElement).next();
                }
                offsetVal = 0;
                while ((offsetVal + $(tabElement).outerWidth(true)) < (visibleWidth) && tabElement.length > 0) {
                    offsetVal += $(tabElement).outerWidth(true);
                    tabElement = $(tabElement).next();
                }
                scrollVal = $.dinftab.calSumWidth($(tabElement).prevAll());
                if (scrollVal > 0) {
                    $('.page-tabs-content').animate({
                        marginLeft: 0 - scrollVal + 'px'
                    }, "fast");
                }
            }
        },
        scrollTabLeft: function () {
            var marginLeftVal = Math.abs(parseInt($('.page-tabs-content').css('margin-left')));
            var tabOuterWidth = $.dinftab.calSumWidth($(".content-tabs").children().not(".menuTabs"));
            var visibleWidth = $(".content-tabs").outerWidth(true) - tabOuterWidth;
            var scrollVal = 0;
            if ($(".page-tabs-content").width() < visibleWidth) {
                return false;
            } else {
                var tabElement = $(".menuTab:first");
                var offsetVal = 0;
                while ((offsetVal + $(tabElement).outerWidth(true)) <= marginLeftVal) {
                    offsetVal += $(tabElement).outerWidth(true);
                    tabElement = $(tabElement).next();
                }
                offsetVal = 0;
                if ($.dinftab.calSumWidth($(tabElement).prevAll()) > visibleWidth) {
                    while ((offsetVal + $(tabElement).outerWidth(true)) < (visibleWidth) && tabElement.length > 0) {
                        offsetVal += $(tabElement).outerWidth(true);
                        tabElement = $(tabElement).prev();
                    }
                    scrollVal = $.dinftab.calSumWidth($(tabElement).prevAll());
                }
            }
            $('.page-tabs-content').animate({
                marginLeft: 0 - scrollVal + 'px'
            }, "fast");
        },
        scrollToTab: function (element) {
            //$.dinftab.refreshTab(this);
            var marginLeftVal = $.dinftab.calSumWidth($(element).prevAll()), marginRightVal = $.dinftab.calSumWidth($(element).nextAll());
            var tabOuterWidth = $.dinftab.calSumWidth($(".content-tabs").children().not(".menuTabs"));
            var visibleWidth = $(".content-tabs").outerWidth(true) - tabOuterWidth;
            var scrollVal = 0;
            if ($(".page-tabs-content").outerWidth() < visibleWidth) {
                scrollVal = 0;
            } else if (marginRightVal <= (visibleWidth - $(element).outerWidth(true) - $(element).next().outerWidth(true))) {
                if ((visibleWidth - $(element).next().outerWidth(true)) > marginRightVal) {
                    scrollVal = marginLeftVal;
                    var tabElement = element;
                    while ((scrollVal - $(tabElement).outerWidth()) > ($(".page-tabs-content").outerWidth() - visibleWidth)) {
                        scrollVal -= $(tabElement).prev().outerWidth();
                        tabElement = $(tabElement).prev();
                    }
                }
            } else if (marginLeftVal > (visibleWidth - $(element).outerWidth(true) - $(element).prev().outerWidth(true))) {
                scrollVal = marginLeftVal - $(element).prev().outerWidth(true);
            }
            $('.page-tabs-content').animate({
                marginLeft: 0 - scrollVal + 'px'
            }, "fast");
        },
        calSumWidth: function (element) {
            var width = 0;
            $(element).each(function () {
                width += $(this).outerWidth(true);
            });
            return width;
        },
        init: function () {
            $(document).on("click", ".menuTag", $.dinftab.addTab);
            $(document).on("click", ".btnDetailTag", $.dinftab.addChildTab);
            //$('.menuItem').on('click', $.dinftab.addTab);
            $('.menuTabs').on('click', '.menuTab i', $.dinftab.closeTab);
            $('.menuTabs').on('click', '.menuTab', $.dinftab.activeTab);
            $('.tabLeft').on('click', $.dinftab.scrollTabLeft);
            $('.tabRight').on('click', $.dinftab.scrollTabRight);
            $('.tabReload').on('click', $.dinftab.refreshTab);
            $('.tabCloseCurrent').on('click', function () {
                $('.page-tabs-content').find('.active i').trigger("click");
            });
            $('.tabCloseAll').on('click', function () {
                $('.page-tabs-content').children("[data-id]").find('.fa-remove').each(function () {
                    $('.Daoben_iframe[data-id="' + $(this).data('id') + '"]').remove();
                    $(this).parents('a').remove();
                });
                $('.page-tabs-content').children("[data-id]:first").each(function () {
                    $('.Daoben_iframe[data-id="' + $(this).data('id') + '"]').show();
                    $(this).addClass("active");
                });
                $('.page-tabs-content').css("margin-left", "0");
            });
            $('.tabCloseOther').on('click', $.dinftab.closeOtherTabs);
            $('.fullscreen').on('click', function () {
                if (!$(this).attr('fullscreen')) {
                    $(this).attr('fullscreen', 'true');
                    $.dinftab.requestFullScreen();
                } else {
                    $(this).removeAttr('fullscreen')
                    $.dinftab.exitFullscreen();
                }
            });
        }
    };
    $(function () {
        $.dinftab.init();
        var de = document.documentElement;
        if (de.requestFullscreen == undefined && de.mozRequestFullScreen == undefined && de.webkitRequestFullScreen == undefined) {
            $(".fullscreen").remove();
            $(".btn-group").addClass("btn-group-no-full");
            $(".tabRight").addClass("tabRightNoFull");
        }
    });
})(jQuery);
//系统消息
//$(function () {
//    $.getJSON("/SecondHand/Message/GetSysMessage", {}, function (data) {
//        if (data.state == "success") {
//            var message = data.data.SysMessage;
//            if (!!message && message.length > 0) {
//                var messageObj = $(".modal-body");
//                for (var item in message) {
//                    var html = "<fieldset class='message_field'>";
//                    html += "<legend style='margin-bottom:5px;'>" + message[item].title + "</legend>";
//                    html += message[item].content;
//                    html += "</fieldset>";
//                    messageObj.append(html);
//                }
//                $("#myModal").modal("show");
//            }
//            else
//                $("#myModal").remove();
//            var messageCount = data.data.MessageCount;
//            if (!!messageCount && messageCount > 0) {
//                messageCount = messageCount > 99 ? "99+" : messageCount;
//                $("#dyj_message").text(messageCount);
//                $("#dyjMessageCount").text(messageCount);
//            }
//            else
//                $("#dyj_message").hide();
//        }
//    });

//    $.getJSON("/Home/CanLoginCMS", {}, function (data) {
//        //cms
//        if (data.state == "success") {
//            $(".divider").before('<li><a onclick="return true;" href="http://test.dyjapp.com:2017/Login/DirectLogin?username=' + data.data.username + '&pwd=' + data.data.pwd + '" target="_blank"><i class="fa fa-user"></i>CMS</a></li>');
//        }
//    })
//})