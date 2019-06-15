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
                    _html += '<li class="menu_2nd" data-href="' + subrow.url + '" data-code="' + subrow.encode + '">';
                    _html += '<a  href="javascript:;"><i class="fa fa-circle-o"></i>' + subrow.name + '</a>';
                    _html += '</li>';
                });
                _html += '</ul>';
            }
            _html += '</li>';
        }
    });
    $("#sidebar-menu").prepend(_html);

    $(".menu_2nd").on("click", function () {
        //var $this = $(this);
        //var href = $this.data("href");
        //var code = $this.data("code");
        //$(".menu_2nd.active").removeClass("active");
        //$this.addClass("active");
        //JumpUrl(code, href);
        //$("#navUrl").data("href", href).data("code", code);
        //$("#navTitle").text($this.find("span").text());
        //$("#navUrl").find("i").attr("class", $this.parent().prev().find("i").attr("class") + " fa_margin");
        //return false;
    });
    /*��Ļ�ı�ʱiframe�߶�ҲҪ����*/
    $(window).resize(function () {
        calculate_h();
    })
    calculate_h();
    $("#btnCancel").on("click", function () {
        layer.confirm("ȷ���˳�ϵͳ��", {
            icon: "fa-exclamation-circle",
            title: "ϵͳ��ʾ",
            btn: ['ȷ��', 'ȡ��'],
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
})

/*����iframe�ĸ߶�*/
function calculate_h() {
    var h = $(window).height() - $("#top").height() - $("#top2").height() - 20;
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
        $loadingtext.html("���ݼ����У����Ժ�");
    }
    $loadingtext.css("left", (top.$('body').width() - $loadingtext.width()) / 2 - 50);
    $loadingtext.css("top", (top.$('body').height() - $loadingtext.height()) / 2);
}

function PictureShow(photoJson, anim) {
    layer.photos({
        photos: {
            "title": "���", //������
            "id": "", //���id
            "start": 0, //��ʼ��ʾ��ͼƬ��ţ�Ĭ��0
            "data": photoJson
        },
        anim: !anim ? 1 : anim//����Ч�� 0-6
        , success: function (layero, index) {
            $(".layui-layer-shade:last").append("<i class='fa fa-close'style='font-size:80px;color:#fff;z-index:29891018;position:fixed; right:0px; top:0px;' id='closePicture'></i>");
        }
    });
}
//��������
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
    OpenForm('/Account/UpdatePassword', '�޸�����');
});

function JumpUrl(code, href) {
    $("#myFrameId").off("load");
    //$.loading(true);
    $("#myFrameId").data("code", code)
    $("#myFrameId").attr("src", href).on("load", function () {
        //$.loading(false);
    })
}
