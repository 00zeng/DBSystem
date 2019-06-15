var id = $.request("id");
var idtype;//证件类别
var gender;//性别
var political;//政治面貌
var birthday_type;//生日类型
var marriage;//婚姻状况
var resign_type;//离职方式

var emp_name = "";//离职需要姓名
var entry_date = "";//入职时间
$(function () {
    //console.log(id);
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            console.log(data);
            if (data.empJobInfo.status == -100)
                $("#btn_edit").css("display", "none");
            else
                $("#btn_edit").css("display", "");

            $("#form1").formSerializeShow(data.empInfo);
            $("#form1").formSerializeShow(data.empJobInfo);
            $("#account").text(data.accountInfo.account);
            $("#role_name").text(data.accountInfo.role_name);
            emp_name = data.empJobInfo.name;
            //获取入职时间
            entry_date1 = data.empJobInfo.entry_date.substr(0, 10);
            
            $("#entry_date1").val(entry_date1);
            if (data.accountInfo.inactive) {
                $("#account_state").text("已停用");
                account_inactive = true;
            }
            else {
                $("#account_state").text("已启用");
                account_inactive = false;
            }
            if (data.empJobInfo.position_name == "导购员") //导购
            {
                $("#distributorWrap").show();
                $("#areaL1Wrap").show();
                $("#areaL2Wrap").show();
                $("#guideSpecificWrap").show();
                $("#introducer_name").val("");
            }
            else if (data.empJobInfo.position_name == "业务员") {//业务员
                $("#distributorWrap").hide();
                $("#areaL1Wrap").show();
                $("#areaL2Wrap").show();
                $("#guideSpecificWrap").hide();
            }
            else if (data.empJobInfo.position_name == "业务经理") {//业务经理
                $("#distributorWrap").hide();
                $("#areaL2Wrap").hide();
                $("#areaL1Wrap").show();
                $("#guideSpecificWrap").hide();
            }
            else {
                $("#distributorWrap").hide();
                $("#areaL2Wrap").hide();
                $("#areaL1Wrap").hide();
                $("#guideSpecificWrap").hide();
            }

            CalculateAge($("#birthdate"));
            if (data.empInfo.marriage != null)
                marriage = data.empInfo.marriage;
            if (marriage == 0) {
                $("#marriage").text("未婚");
            } else if (marriage == 1) {
                $("#marriage").text("已婚");
            } else if (marriage == 2) {
                $("#marriage").text("离异");
            } else if (marriage == 3) {
                $("#marriage").text("丧偶");
            } else if (marriage == 4) {
                $("#marriage").text("其他");
            }
            if (data.empInfo.gender != null)
                gender = data.empInfo.gender;
            if (gender == 0) {
                $("#gender").text("女");
            } else if (gender == 1) {
                $("#gender").text("男");
            } else {
                $("#gender").text("未指定");
            }
            if (data.empInfo.birthday_type == 1) {
                $("#birthday_type").text("农历");
            } else {
                $("#birthday_type").text("阳历");
            }
            if (data.empJobInfo.insurance_paid) {
                $("#insurance_paid").text("是");
            } else {
                $("#insurance_paid").text("否");
            }
            $("#bank_type").text(data.empInfo.bank_type);
            var birthdate = data.empInfo.birthdate.substr(0, 10);
            $("birthdate").text(birthdate);
            if (data.accountInfo.role_id == 4) // 导购
                $("#for_guide").show();

            //页面无离职状态
            //if (data.empJobInfo.status == 0)
            //    $("#emp_status").text("正常");
            //else if (data.empJobInfo.status == -100)
            //    $("#emp_status").text("已离职");
            //else if (data.empJobInfo.status == 1)
            //    $("#emp_status").text("休假");
            ////休假时间
            //$("#rest_time").text(data.posInfo.rest_time);
            ////离职方式
            //resign_type = data.employeeInfo.resign_type;
            //if (resign_type == 1) {
            //    $("#resign_type").text("正常离职");
            //} else if (resign_type == 2) {
            //    $("#resign_type").text("辞退");
            //} else if (resign_type == 3) {
            //    $("#resign_type").text("自离");
            //}
            //$("#content").text(data.posInfo.content);
            //$("#report_person").text(data.posInfo.report_person);
            //$("#deductible_insurance").text(data.posInfo.deductible_insurance);
            //$("#others_insurance").text(data.posInfo.others_insurance);
            //$("#deposit_return").text(data.posInfo.deposit_return);
            //系统账户
            $.each(data.imageList, function (i, v) {
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
});
//员工信息修改页面
function EditPersonalInfo() {
    window.location.href = "EditPersonalInfo?id=" + id;
}
//员工职务信息修改页面
function EditJobInfo() {
    window.location.href = "EditJobInfo?id=" + id;
}
//员工账户信息修改页面
function EditAccountInfo() {
    window.location.href = "EditAccountInfo?id=" + id;
}
//员工图片信息修改页面
function EditPictureInfo() {
    window.location.href = "EditpictureInfo?id=" + id;
}

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

function CalculateAge(obj) {
    var birthdate = $(obj).val().trim();
    if (!birthdate || birthdate == "")
        return;
    var curYear = (new Date()).getFullYear();
    var birthYear = (new Date(birthdate)).getFullYear();
    $("#age").val(curYear - birthYear + 1);
}

/*删除数据*/
function SetLeave() {
    top.layer.confirm("离职操作不可恢复，确定要将 " + emp_name + " 离职？",function (index){
        var data = {};
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["id"] = id;
        data["resignTime"] = $("#leave_time").val();
        $.ajax({
            url: "/HumanResource/EmployeeManage/EmpResign?date=" + new Date(),
            type: "post",
            data: data,
            success: function (data) {
                top.layer.closeAll();
                window.location.href = "/HumanResource/EmployeeManage/Index";
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}





