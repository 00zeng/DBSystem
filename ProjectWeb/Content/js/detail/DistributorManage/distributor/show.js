var id = $.request("id");
var account_inactive = "";
var name = "";
$(function () {
    $.ajax({
        url: "/DistributorManage/DistributorManage/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            if (!!data) {
                $("#form1").formSerializeShow(data.mainInfo);
                $("#account").text(data.accountInfo.account);
                name = data.mainInfo.name;
                if (data.accountInfo.inactive) {
                    $("#account_state").text("已停用");                 
                    account_inactive = true;
                }
                else {
                    $("#account_state").text("已启用");                   
                    account_inactive = false;
                }
            
                if (!(data.mainInfo.inactive))
                    $("#btnClose").show(); // TODO 状态显示
                else {
                    $("#btnEdit").css("display", "none");
                    $("#btnActive").css("display", "none"); 
                    $("#account_tr1").css("display", "none");
                    $("#account_tr2").css("display", "none");
                    $("#inactive_close_tr").css("display", "");
                }
                
                $.each(data.imageList,function (i,v) {
                    var index = getIndex(v.type);
                    if ($('div[name="row"]:eq(' + index + ') .item').length == 0) {
                        $('div[name="picbox"]:eq(' + index + ')').show();
                        $('div[name="row"]:eq(' + index + ')').append('<div show="open" class="col-sm-4"  style="text-align: center;" name="open_div">'
                                         + '<div class="thumbnail" style="height:100px">'
                                         + '<img style="height:90px" show="open" src="' + v.url_path + '" />'
                                         + '</div>'
                                         + '</div>');
                    } else {
                        $('div[name="row"]:eq(' + index + ')').append('<div show="open" class="col-sm-4"  style="text-align: center;" name="open_div">'
                                         + '<div class="thumbnail" style="height:100px">'
                                         + '<img style="height:90px" show="open" src="' + v.url_path + '" />'
                                         + '</div>'
                                         + '</div>');
                    }
                })
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });

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
})


function getIndex(type) {
    var index;
    switch (type) {
        case 1:
            index = 1;
            break;
        case 2:
            index = 2;
            break;
        case 3:
            index = 0;
            break;
        case 12:
            index = 3;
            break;
        case 99:
            index = 4;
            break;
    }
    return index;
}

//经销商信息修改页面
function EditDistributorInfo() {
    window.location.href = "EditDistributorInfo?id=" + id;
}
//经销商账户信息修改页面
function EditAccountInfo() {
    window.location.href = "EditAccountInfo?id=" + id;
}
//经销商图片信息修改页面
function EditPictureInfo() {
    window.location.href = "EditPictureInfo?id=" + id;
}

function SetClose() {
   
    var hint = "确认要将经销商【" + name + "】设为停业？注意此操作不可撤消！"
    top.layer.confirm(hint, function (index) {
        var data = { id: id };
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        $.submitForm({
            url: "/DistributorManage/DistributorManage/Closed?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    top.layer.closeAll();
                    setTimeout("location.reload();", 1000)
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}


/*删除数据*/
function Delete() {
    top.layer.confirm("确认要删除 " + name, function (index) {
        var data = {};
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }

        data["id"] = id;
        $.ajax({
            url: "/DistributorManage/DistributorManage/Delete?date=" + new Date(),
            type: "post",
            data: data,
            success: function (data) {
                top.layer.closeAll();
                window.location.href = "/DistributorManage/DistributorManage/Index";
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}
