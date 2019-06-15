var id = $.request("id");
var picJson2 = [];//用于提交到数据库的 json 数据
var identityCheck = true;
var grade_category = 0;
var cur_company = 0;
$("#role_id").bindSelect({
    text: "name",
    url: "/SystemManage/MsRole/GetSelectJson",
    firstText: "--请选择所属角色--",
    search: true,
});
$(function () {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        //async:false,
        success: function (data) {
            $("#name").text(data.empJobInfo.name);
            $("#work_number").text(data.empJobInfo.work_number);
            //入职时间
            if (!!data.empJobInfo.entry_date || data.empJobInfo.entry_date != '') {
                $("#entry_date").val(data.empJobInfo.entry_date.substring(0, 10));
            }
            if (!!(data.empInfo)) {
                $("#form1").formSerialize(data.empInfo);
                //出生日期
                if (!!data.empInfo.birthdate || data.empInfo.birthdate != '') {
                    $("#birthdate").val(data.empInfo.birthdate.substring(0, 10));
                }
                //生日
                if (!!data.empInfo.birthday || data.empInfo.birthday != '') {
                    $("#birthday").val(data.empInfo.birthday.substring(0, 10));
                }
                //健康证有效期
                if (!!data.empInfo.health_start || data.empInfo.health_start != '') {
                    $("#health_start").val(data.empInfo.health_start.substring(0, 10));
                }
                if (!!data.empInfo.health_expire || data.empInfo.health_expire != '') {
                    $("#health_expire").val(data.empInfo.health_expire.substring(0, 10));
                }
                //证件有效起止日
                if (!!data.empInfo.identity_effect || data.empInfo.identity_effect != '') {
                    $("#identity_effect").val(data.empInfo.identity_effect.substring(0, 10));
                }
                if (!!data.empInfo.identity_expire || data.empInfo.identity_expire != '') {
                    $("#identity_expire").val(data.empInfo.identity_expire.substring(0, 10));
                }
                //毕业日期
                if (!!data.empInfo.graduation_date || data.empInfo.graduation_date != '') {
                    $("#graduation_date").val(data.empInfo.graduation_date.substring(0, 10));
                }
                CalculateAge($("#birthdate"));
                $("input:radio[name=gender][value=" + data.empInfo.gender + "]").attr("checked", true);

                $.each(data.imageList, function (index, item) {
                    $divId = getTypeName(item.type);
                    $("#" + $divId).append('<div show="open" class="col-sm-6" id="my_license">'
                                    + '<div class="thumbnail">'
                                    + '<img show="open" class="img-responsive" src="' + item.url_path + '" />'
                                    + '<i class="fa fa-trash trash" del="del" data-id="' + item.id + '"></i>'
                                    + '</div>'
                                    + '</div>')
                })
            }
            if (data.accountInfo.role_id == 20) {
                $("#startDate").val(data.empJobInfo.entry_date);
                $("#startDate").off().on("focus", function () {
                    var startDate = data.empJobInfo.entry_date;
                    WdatePicker({readOnly:true, startDate: startDate });
                });
            } else {
                $("#startDate").val(data.empJobInfo.create_time);
                $("#startDate").off().on("focus", function () {
                    var startDate = data.empJobInfo.create_time;
                    WdatePicker({readOnly:true, startDate: startDate });
                });
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
})

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
    url = "/File/UploadPictures?multiple=" + multiple + "&type=" + type;
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
        url: "/OrganizationManage/Company/GetIdNameCategoryList",
        dataType: "json",
        async: false,
        success: function (data) {
            var optionStr = "";
            $.each(data, function (i) {
                optionStr += "<option value='" + data[i]["id"] + "' category='"
                            + data[i]["category"] + "'>" + data[i]["name"] + "</option>";
            });
            $("#company_id").append(optionStr);
            $("#company_id").select2({
                minimumResultsForSearch: 0, tags: false
            });
            BindDepartment();
            BindEmpCategory();
            $("#company_id").on("change", function (e) {
                $("#select2-" + $("#company_id").attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
                BindDepartment();
                BindEmpCategory();
            });
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });

    $("#identity_type").on("change", function () {
        var obj = $("#identity");
        obj.val("");
        if ($("#identity_type").val() == "居民身份证")
            identityCheck = true;
        else {
            $(obj).parents('.has-error').find('div.tooltip').remove();
            $(obj).parents('.has-error').find('i.error').remove();
            $(obj).parents().removeClass('has-error');
            identityCheck = false;
        }
    })

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
})

/*图片上传成功回调函数*/
function CallBackPictures(data) {
    var obj = $("button[add='upload']").eq(index);
    //top.layer.closeAll();
    top.layer.msg("上传成功");
    if (index != 3) {
        picJson2.push(data);
    } else {
        for (var i = 0; i < picJson2.length; i++) {
            if (picJson2[i].type == 1) {
                picJson2.splice(i, 1);
            }
        }
        picJson2.push(data);
        $(obj).siblings(".row").find("#my_license").remove();
        //$(obj).attr("disabled", true);//按钮禁用
        $(obj).siblings("[name='src']").val(data.file_path + data.ext);//图片地址
        $(obj).siblings("[name='full-src']").val(data.server_path + data.file_path + data.ext);//图片显示所需地址
    }
    $(obj).siblings(".row").append('<div show="open" class="col-sm-6" id="my_license">'
        + '<div class="thumbnail">'
        + '<img show="open" class="img-responsive" src="' + data.url_path + '" />'
        + '<i class="fa fa-trash trash" del="del" data-file_path="' + data.file_path + '"></i>'
        + '</div>'
        + '</div>');
}

/*上传错误删除按钮事件*/
function Del(obj) {
    //获取最外层父元素
    var parent = $(obj).parent().parent().parent();
    index = $("form .row").index(parent);
    //获取下标
    var img_index = $(parent).find("i").index(obj);
    for (var i = 0; i < picJson2.length; i++) {
        if (picJson2[i].file_path == $(obj).data('file_path')) {
            picJson2.splice(i, 1);
        }
    }
    $(parent).children().eq(img_index).remove();
    $.ajax({
        type: "post",
        async: "false",
        url: "/File/DeleteLocalFile",
        data: "file_path=" + $(obj).data('file_path'),
        success: function (data) {
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}

function BindDepartment() {
    cur_company = $("#company_id").val();
    $("#dept_id").empty();
    $.ajax({
        url: "/OrganizationManage/Department/GetDeptAddrList?id=" + cur_company,
        dataType: "json",
        async: false,
        success: function (data) {
            var optionStr = "";
            $.each(data, function (i) {
                optionStr += "<option value='" + data[i]["id"] + "' grade='"
                    + data[i]["grade_category"] + "' addr='" + data[i]["addr_str"] + "'>" + data[i]["name"] + "</option>";
            });
            $("#dept_id").append(optionStr);
            $("#dept_id").select2({
                minimumResultsForSearch: 0, tags: false
            });
            BindPosition();
            $("#dept_id").on("change", function (e) {
                $("#select2-" + $("#dept_id").attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
                BindPosition();
            });
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}
var curCategory = "";
function BindEmpCategory() {
    var category = $("#company_id").find('option:selected').attr("category");
    if (category == curCategory)
        return;
    curCategory = category;
    $("#emp_category").empty();
    if (category == "分公司") {
        $("#emp_category").append("<option value='实习生'>实习生</option>"
                        + "<option value='员工'>员工</option>"
                        + "<option value='劳务工'>劳务工</option>");
    }
    else {
        $("#emp_category").append("<option value='实习生'>实习生</option>"
                        + "<option value='职员'>职员</option>"
                        + "<option value='劳务工'>劳务工</option>");
    }
}

function BindPosition() {
    var curDepartment = $("#dept_id").val();
    if (curDepartment > 0) {
        $("#position_id").empty();
        $("#position_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Position/GetIdNameList",
            param: { dept_id: curDepartment },
            firstText: "--请选择职位--",
            change: function () {
                GetGradeList();
            }
        });
        var addr = $("#dept_id").find('option:selected').attr("addr");
        $("#work_addr").val(addr);
        grade_category = $("#dept_id").find('option:selected').attr("grade");
    }
}

function GetGradeList() {
    var position_id = $("#position_id").val();
    $("#grade").empty();
    $.ajax({
        url: "/OrganizationManage/Position/GetGradeList",
        data: { position_id: position_id, grade_category: grade_category },
        dataType: "json",
        async: false,
        success: function (data) {
            $("#grade").bindSelectLocal({
                data: data.gradeList,
                id: "name",
                text: "name",
            });
            if (data.positionInfo.salary_category == 3) //导购
            {
                $("#GuideSpecific").show();
                $("#role_id").val(4).trigger("change");
                ;//导购角色
                $("#role_id").attr("disabled", true);
                $("#introducer_name").val("");
            }
            else {
                $("#role_id").val("").trigger("change");
                ;
                $("#role_id").attr("disabled", false);
                $("#GuideSpecific").hide();
            }

        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

function CheckIDCard(obj) {
    if (!identityCheck || $(obj).val() == "")
        return;
    var checkFlag = new clsIDCard($(obj).val());
    if (!checkFlag.IsValid(obj)) {
        return;
    }
    $("#birthdate").val(checkFlag.GetBirthDate());
    $("#birthday").val(checkFlag.GetBirthDate());
    CalculateAge($("#birthday"));
    var sex = checkFlag.GetSex();
    $("[name='gender']").filter("[value='" + sex + "']").prop("checked", "checked");

}

function CalculateAge(obj) {
    var birthdate = $(obj).val().trim();
    if (!birthdate || birthdate == "")
        return;
    var curYear = (new Date()).getFullYear();
    var birthYear = (new Date(birthdate)).getFullYear();
    $("#age").val(curYear - birthYear + 1);
}

var supervisor_id = "";
var introducer_id = "";

function CheckNameExist(obj) {
    var name = $(obj).val().trim();
    if (!name || name == "")
        return;
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetEmpByName",
        data: { name: name },
        dataType: "json",
        async: false,
        success: function (data) {
            if (!data || !data.id || data.id == "") {
                if ($(obj).attr('id') == "supervisor_name")
                    supervisor_id = "";
                else
                    introducer_id = "";
                $.modalAlert("系统中不存在“" + name + "”，请确认", "warning");
            }
            else {
                if ($(obj).attr('id') == "supervisor_name")
                    supervisor_id = data.id;
                else
                    introducer_id = data.id;
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var data = $("#form1").formSerialize();
    data["image_list"] = picJson2;
    data["id"] = id;
    
    $.submitForm({
        url: "/HumanResource/EmployeeManage/Edit?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/HumanResource/EmployeeManage/Index";

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
