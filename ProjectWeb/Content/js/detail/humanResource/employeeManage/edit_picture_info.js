var id = $.request("id");
var addImgJson = [];//添加图片
var deleteImgJson = [];//删除图片

//上传图片
function OpenForm() {
    var url;
    var multiple = true;
    var type;
    switch (index) {
        case 0:
            type = 12;
            break;
        case 1:
            type = 11;
            break;
        case 2:
            type = 2;
            break;
        case 3:
            type = 1;
            multiple = false;
            break;
        case 4:
            type = 3;
            break;
        case 5:
            type = 4;
            break;
        case 6:
            type = 5;
            break;
    }
    url = "/File/UploadPictures?multiple=" + multiple + "&type=" + type + "&src=1";
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
    ActionBinding();
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        //async:false,
        success: function (data) {
            $("#name").text(data.empJobInfo.name);
            $("#work_number").text(data.empJobInfo.work_number);

            $.each(data.imageList, function (index, item) {
                $divId = getTypeName(item.type);
                $("#" + $divId).append('<div show="open" class="col-sm-6" id="my_license">'
                                + '<div class="thumbnail">'
                                + '<img show="open" class="img-responsive" src="' + item.url_path + '" />'
                                + '<i class="fa fa-trash trash" del="del" data-id="' + item.id + '"></i>'
                                + '</div>'
                                + '</div>')
            })

            $.each(data.imageList,function (i,v) {
                var index = getIndex(v.type);
                console.log(index);
                if ($('div[name="row"]:eq(' + index + ') .item').length == 0) {
                    $('div[name="picbox"]:eq(' + index + ')').show();
                    $('div[name="row"]:eq(' + index + ')').append('<div class="item active">\n' +
                        '                                        <img show="open" src="' + v.url_path + '" style="width: 325px;height: 200px;">\n' +
                        '\n' +
                        '<a style="display: block;margin-left: 162.5px;" onclick="Del(this)" data-id="'+v.id+'"><span class="glyphicon glyphicon-trash"></span></a> '+
                        '                                        <div class="carousel-caption">\n' +
                        '                                        </div>\n' +
                        '                                    </div>');
                } else {
                    $('div[name="row"]:eq(' + index + ')').append('<div class="item">\n' +
                        '                                        <img show="open" src="' + v.url_path + '" style="width: 325px;height: 200px;">\n' +
                        '\n' +
                        '<a style="display: block;margin-left: 162.5px;" onclick="Del(this)" data-id="'+v.id+'"><span class="glyphicon glyphicon-trash"></span></a> '+
                        '                                        <div class="carousel-caption">\n' +
                        '                                        </div>\n' +
                        '                                    </div>');
                }
            })
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

    $("body").on("click", "i[del='del']", function () { Del(this) })
    //查看图片
    $("body").on("click", "img[show='open']", function () {
        var obj = "<img class='img-responsive' src='" + $(this).attr("src") + "' style='widows: 500px;height: 500px;'/>";
        var h = "500px";
        var w = "500px";
        top.layer.open({
            type: 1,
            title: false,
            area: [w, h],
            closeBtn: 0,
            shadeClose: true,
            content: obj,
        })
    })
});

function getIndex(type) {
    var index;
    switch (type) {
        case 1:
            index = 3;
            break;
        case 2:
            index = 2;
            break;
        case 3:
            index = 4;
            break;
        case 4:
            index = 5;
            break;
        case 5:
            index = 6;
            break;
        case 11:
            index = 1;
            break;
        case 12:
            index = 0;
            break;
    }
    return index;
}
// 绑定标签事件
function ActionBinding() {
    $("button[add='upload']").on("click", function () {
        index = $("button[add='upload']").index(this);
        OpenForm();
    })
    $("body").on("click", "i[del='del']", function () {
        Del(this)
    })
    //查看图片
    $("body").on("click", "img[show='open']", function () {
        var obj = "<img class='img-responsive' src='" + $(this).attr("src") + "' style='widows: 500px;height: 500px;'/>";
        var h = "500px";
        var w = "500px";
        top.layer.open({
            type: 1,
            title: false,
            area: [w, h],
            closeBtn: 0,
            shadeClose: true,
            content: obj,
        })
    })
}

/*图片上传成功回调函数*/
function CallBackPictures(data) {
    var obj = $("button[add='upload']").eq(index);
    //top.layer.closeAll();
    top.layer.msg("上传成功");
    // if (index != 3) {
    addImgJson.push(data);
    // } else {
    //     for (var i = 0; i < picJson2.length; i++) {
    //         if (picJson2[i].type == 1) {
    //             picJson2.splice(i, 1);
    //         }
    //     }
    //     picJson2.push(data);
    //     $(obj).siblings(".row").find("#my_license").remove();
    //     //$(obj).attr("disabled", true);//按钮禁用
    //     $(obj).siblings("[name='src']").val(data.file_path + data.ext);//图片地址
    //     $(obj).siblings("[name='full-src']").val(data.server_path + data.file_path + data.ext);//图片显示所需地址
    // }
    $('div[name="picbox"]:eq(' + index + ')').show();

    if ($('div[name="row"]:eq(' + index + ') .item').length == 0) {
        $('div[name="row"]:eq(' + index + ')').append('<div class="item active">\n' +
            '                                        <img show="open" src="' + data.url_path + '" style="width: 325px;height: 200px;">\n' +
            '\n' +
            '<a style="display: block;margin-left: 162.5px;" onclick="Del(this)" data-file_path="'+data.file_path+'"><span class="glyphicon glyphicon-trash"></span></a> '+
            '                                        <div class="carousel-caption">\n' +
            '                                        </div>\n' +
            '                                    </div>');
    } else {
        $('div[name="row"]:eq(' + index + ')').append('<div class="item">\n' +
            '                                        <img show="open" src="' + data.url_path + '" style="width:325px;height: 200px;">\n' +
            '\n' +
            '<a style="display: block;margin-left: 162.5px;" onclick="Del(this)" data-file_path="'+data.file_path+'"><span class="glyphicon glyphicon-trash"></span></a> '+
            '                                        <div class="carousel-caption">\n' +
            '                                        </div>\n' +
            '                                    </div>');
    }
}

function getTypeName(type) {
    var name;
    switch (type) {
        case 1:
            name = "picture";
            break;
        case 2:
            name = "IDcard";
            break;
        case 3:
            name = "resume";
            break;
        case 4:
            name = "Diploma";
            break;
        case 5:
            name = "graduate";
            break;
        case 11:
            name = "entry";
            break;
        case 12:
            name = "contract";
            break;
    }
    return name;
}

/*上传错误删除按钮事件*/
function Del(obj) {
    //获取最外层父元素
    var img_index = $(obj).parent().index();
    var parent_index = $(obj).parent().parent().index('div[name="row"]');
    console.log(img_index);
    console.log(parent_index);
    $(obj).parent().remove();

    $('div[name="row"]:eq(' + parent_index + ')').children().eq(0).prop('class', 'item active');

    if ($('div[name="row"]:eq(' + parent_index + ') .item').length == 0) {
        $('div[name="picbox"]:eq(' + parent_index + ')').hide();
    }
    
    
    if ($(obj).data('id')) {
        deleteImgJson.push($(obj).data('id'));
    } else {
        for (var i = 0; i < addImgJson.length; i++) {
            if (addImgJson[i].file_path == $(obj).data('file_path')) {
                addImgJson.splice(i, 1);
                $.ajax({
                    type: "post",
                    async: "false",
                    url: "/File/DeleteLocalFile",
                    data: "file_path=" + $(obj).data('file_path'),
                    success: function (data) {
                        console.log("图片删除成功");
                    },
                    error: function (data) {
                        $.modalAlert(data.responseText, 'error');
                    }
                })
            }
        }
    }
}


function submitForm() {
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }   
    data["id"] = id;
    data["add_image_list"] = addImgJson;
    data["del_image_list"] = deleteImgJson;
    $.submitForm({

        url: "/HumanResource/EmployeeManage/Edit?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/EmployeeManage/Index?id=" + id;

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
