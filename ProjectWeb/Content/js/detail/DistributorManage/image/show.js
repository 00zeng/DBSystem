var id = $.request("id");
var name = decodeURI($.request("name"));
$("#name").text(name);
var start_date;
var end_date;
$(function () {
    $.ajax({
        url: "/DistributorManage/Image/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#form1").formSerializeShow(data.mainInfo);
            var activity_status = data.mainInfo.activity_status;
            if (activity_status == 1) {
                $("#activity_status").text("进行中");
            } else if (activity_status == -1) {
                $("#activity_status").text("未开始");
            } else if (activity_status == 2) {
                $("#activity_status").text("已结束");
            }

            $("#rebate").text(ToThousandsStr(data.mainInfo.rebate));
            $("#activity_target").text(ToThousandsStr(data.mainInfo.activity_target));
            start_date = data.mainInfo.start_date;
            end_date = data.mainInfo.end_date;
            if (data.mainInfo.activity_status == 2 || data.mainInfo.approve_status != 100) {
                $("#btn_edit").css("display", "none");
            }
            //返利发放
            if (data.mainInfo.pay_mode == 1) {
                // 一次性
                $("span[name='activity_unit']").html("元");

            } else if (data.mainInfo.pay_mode == 2) {
                //按月 
                $("span[name='activity_unit']").html("元/月");

            }
            $("#rebate").text(ToThousandsStr(data.mainInfo.rebate));

            if (data.mainInfo.target_mode == 2) {
                $("#mode1").css("display", "none");
                $("#mode2").css("display", "none");
                $("#mode3").css("display", "");
                $("#mode4").css("display", "");
            } else if (data.mainInfo.target_mode == 1) {
                $("#mode1").css("display", "");
                $("#mode2").css("display", "");
                $("#mode3").css("display", "none");
                $("#mode4").css("display", "none");
            };
            //查看审批
            var tr = '';
            $("#creator_position_name").text(data.creator_position_name);
            $.each(data.approveList, function (index, item) {
                if (item.approve_note == null || item.approve_note == '') {
                    approve_note_null = "--";
                } else
                    approve_note_null = item.approve_note;
                if (item.status == 1 || item.status == -1)
                    approve_grade = ("一级审批");
                else if (item.status == 2 || item.status == -2)
                    approve_grade = ("二级审批");
                else if (item.status == 3 || item.status == -3)
                    approve_grade = ("三级审批");
                else if (item.status == 4 || item.status == -4)
                    approve_grade = ("四级审批");
                else if (item.status == 100 || item.status == -100)
                    approve_grade = ("终审");
                if (item.status > 0) {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                } else {
                    tr += '<tr><td>' + approve_grade + '</td>' + '<td>' + item.approve_name + '</td>' + '<td>' + item.approve_position_name + '</td>' + '<td>' + item.approve_time + '</td>' + '<td>不通过</td>' + '<td>' + approve_note_null + '</td></tr>';
                }
            })
            $("#approve tr:eq(1)").after(tr);

            $.each(data.fileList, function (i, v) {
                var index = getIndex(v.type);
                if ($('div[name="row"]:eq(' + index + ') .item').length == 0) {
                    $('div[name="picbox"]:eq(' + index + ')').show();
                    $('div[name="row"]:eq(' + index + ')').append('<div show="open" class="col-sm-6"  style="text-align: center;" name="open_div">'
                                     + '<div class="thumbnail" style="height:100px">'
                                     + '<img style="height:90px" show="open" src="' + v.url_path + '" />'
                                     + '</div>'
                                     + '</div>');
                } else {
                    $('div[name="row"]:eq(' + index + ')').append('<div show="open" class="col-sm-6"  style="text-align: center;" name="open_div">'
                                     + '<div class="thumbnail" style="height:100px">'
                                     + '<img style="height:90px" show="open" src="' + v.url_path + '" />'
                                     + '</div>'
                                     + '</div>');
                }
            })

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
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
            index = 0;
            break;
        case 99:
            index = 1;
            break;
    }
    return index;
}
function OpenForm(url, title) {
    //if (title == "修改结束时间")
        url += "?id=" + id + "&start_date=" + start_date + "&end_date=" + end_date;
    $.modalOpen({
        id: "Form",
        title: title,
        url: url,
        width: "600px",
        height: "300px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}