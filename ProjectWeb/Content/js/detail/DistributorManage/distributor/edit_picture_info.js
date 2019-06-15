var addImgJson = [];//添加图片
var deleteImgJson = [];//删除图片
var id = $.request("id");
var account_inactive = "";
//var distributor_id;
var picJson2 = [];//用于提交到数据库的 json 数据

//上传图片
function OpenForm() {
    var url;
    var multiple = true;
    var type;
    switch (index) {
        case 0:
            type = 3;
            break;
        case 1:
            type = 1;
            break;
        case 2:
            type = 2;
            break;
        case 3:
            type = 12;
            break;
        case 4:
            type = 99;
            break;
    }
    url = "/File/UploadPictures?multiple=" + multiple + "&type=" + type + "&src=1&module=2";
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
    $.ajax({
        url: "/DistributorManage/DistributorManage/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            picJson2=data.imageList;
            if (!!data) {
                $.each(data.imageList, function (i, v) {
                    var index = getIndex(v.type);
                    if ($('div[name="row"]:eq(' + index + ') .item').length == 0) {
                        $('div[name="picbox"]:eq(' + index + ')').show();
                        $('div[name="row"]:eq(' + index + ')').append('<div show="open" class="col-sm-3"  style="text-align: center;" name="open_div">'
                                         + '<div class="thumbnail" style="height:100px">'
                                         + '<img style="height:76px" show="open" src="' + v.url_path + '" />'
                                         + '<i class="fa fa-trash trash" del="del"></i>'
                                         + '</div>'
                                         + '</div>');
                    } else {
                        $('div[name="row"]:eq(' + index + ')').append('<div show="open" class="col-sm-3"  style="text-align: center;" name="open_div">'
                                         + '<div class="thumbnail" style="height:100px">'
                                         + '<img style="height:76px" show="open" src="' + v.url_path + '" />'
                                         + '<i class="fa fa-trash trash" del="del"></i>'
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
})



/*图片上传成功回调函数*/
//function CallBackPictures(data) {
//    var obj = $("button[add='upload']").eq(index);
//    top.layer.msg("上传成功");
//    addImgJson.push(data);
  
//    $('div[name="picbox"]:eq(' + index + ')').show();
//    if ($('div[name="row"]:eq(' + index + ') .item').length == 0) {
//        $('div[name="row"]:eq(' + index + ')').append('<div class="item active">\n' +
//            '                                        <img show="open" src="' + data.url_path + '" style="width: 325px;height: 200px;">\n' +
//            '\n' +
//            '<a style="display: block;margin-left: 162.5px;" onclick="Del(this)" data-file_path="' + data.file_path + '"><span class="glyphicon glyphicon-trash"></span></a> ' +
//            '                                        <div class="carousel-caption">\n' +
//            '                                        </div>\n' +
//            '                                    </div>');
//    } else {
//        $('div[name="row"]:eq(' + index + ')').append('<div class="item">\n' +
//            '                                        <img show="open" src="' + data.url_path + '" style="width: 325px;height: 200px;">\n' +
//            '\n' +
//            '<a style="display: block;margin-left: 162.5px;" onclick="Del(this)" data-file_path="' + data.file_path + '"><span class="glyphicon glyphicon-trash"></span></a> ' +
//            '                                        <div class="carousel-caption">\n' +
//            '                                        </div>\n' +
//            '                                    </div>');
//    }
//}
function CallBackPictures(data) {
    var obj = $("button[add='upload']").eq(index);
    top.layer.msg("上传成功");
    picJson2.push(data);
    $('div[name="picbox"]:eq(' + index + ')').show();
    $('div[name="row"]:eq(' + index + ')').append('<div show="open" class="col-sm-3"  style="text-align: center;" name="open_idv">'
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

function submitForm() {
    data["id"] = id;
    data["add_image_list"] = addImgJson;
    data["del_image_list"] = deleteImgJson;

    $.submitForm({
        url: "/DistributorManage/DistributorManage/Edit?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/DistributorManage/DistributorManage/Index";

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
