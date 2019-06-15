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
    ActionBinding();
    BindCompany();
    GetAreaL1List();

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
function CallBackPictures(data) {
    var obj = $("button[add='upload']").eq(index);
    top.layer.msg("上传成功");
    top.layer.closeAll();
    picJson2.push(data);
    $('div[name="picbox"]:eq(' + index + ')').show();
    $('div[name="row"]:eq(' + index + ')').append('<div show="open" class="col-sm-3"  style="text-align: center;" name="open_idv">'
                                 + '<div class="thumbnail" style="height:100px">'
                                 + '<img style="height:76px" show="open" src="' + data.url_path + '" />'
                                 + '<i class="fa fa-trash trash" del="del"></i>'
                                 + '</div>'
                                 + '</div>');

}
// 绑定标签事件
function ActionBinding() {
    $("#company_id").on("change", function (e) {
        GetAreaL1List();
    });
    $("#area_l1_id").on("change", function (e) {
        GetAreaL2List();
    });
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
//检查经销商名称格式
function checkName(obj) {
    distributorName = $("#name").val();
    if (!distributorName || distributorName.trim().length < 4) {
        $(obj).parents('.formValue').addClass('has-error');
        $(obj).parents('.has-error').find('div.tooltip').remove();
        $(obj).parents('.has-error').find('i.error').remove();
        $(obj).parents('.has-error').append('<i class="form-control-feedbacks fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="经销商名称格式错误，前4位或前6位为快捷编码"></i>');
        $("[data-toggle='tooltip']").tooltip();
        if ($(obj).parents('.input-group').hasClass('input-group')) {
            $(obj).parents('.has-error').find('i.error').addClass('has-error-pullright');
        }
        return;
    }
    else {
        distributorName = distributorName.trim();
        if (distributorName.charAt(4) == "-") {
            if (distributorName.length >= 6)
                code = distributorName.substring(0, 6);
            else
                code = distributorName.substring(0, 4);
        }
        else {
            code = distributorName.substring(0, 4);
        }
        $(obj).parents('.has-error').find('div.tooltip').remove();
        $(obj).parents('.has-error').find('i.error').remove();
        $(obj).parents().removeClass('has-error');
        $("#code").val(code);
    }
}
function BindCompany() {
    $("#company_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Company/GetIdNameCategoryList",
        param: { category: "分公司" },
        search: true,
    });
}


//经理片区
function GetAreaL1List() {
    var curCompany = $("#company_id").val();
    $("#area_l1_id").empty();
    if (curCompany > 0) {
        $("#area_l1_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Area/GetManagerAreaIdNameList",
            param: { company_id: curCompany },
            firstText: "--请选择经理片区--",
        });
    }
}

//业务片区
function GetAreaL2List() {
    var areaL1Id = $("#area_l1_id").val();
    $("#area_l2_id").empty();
    if (areaL1Id > 0) {
        $("#area_l2_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Area/GetSalesAreaIdNameList",
            param: { id: areaL1Id },
            firstText: "--请选择业务片区--",
        });
    }
}

function submitForm() {
    if (!$("#main-form").formValid())
        return false;
    var data = $("#main-form").formSerialize();
    data["inactive"] = $("#inactive").prop("checked");//是否启用
    data["potential_salepoint"] = $("input[name='potential_salepoint']:checked").val();//潜在售点
    data["distributor_attribute"] = $("input[name='distributor_attribute']:checked").val();//经销商属性
    data["area_l1_name"] = $("#area_l1_id option:selected").text();
    data["area_l2_name"] = $("#area_l2_id option:selected").text();
    //var addrCity = $("#addrCity .select-item");
    //data["city_code"] = $(addrCity).length > 0 ? $(addrCity).last().data("code") : "";
    data["image_list"] = picJson2;

    $.submitForm({
        url: "/DistributorManage/DistributorManage/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/DistributorManage/DistributorManage/Index" ;

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
