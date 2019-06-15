var picJson2 = [];//用于提交到数据库的 json 数据
var identityCheck = true;
var grade_category = 0;
var cur_company = 0;
var position_type = 0;

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
    $('.select2').select2();
    ActionBinding();
    BindCompany();
    BindRole();
})

//v2姓名获取姓名的值
function getname() {
    var name = document.getElementById('name').value; //获取第一个输入框的值
    document.getElementById('name_v2').value = name;
}

// 绑定标签事件
function ActionBinding() {
    $("#company_id").on("change", function (e) {
        $("#select2-" + $("#company_id").attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
        cur_company = $("#company_id").val();
        BindDepartment();
        BindEmpCategory();
    });
    $("#dept_id").on("change", function (e) {
        grade_category = $("#dept_id").find('option:selected').attr("grade");
        $("#select2-" + $("#dept_id").attr('id') + "-container").html($(this).find("option:selected").text().replace(/　　/g, ''));
        BindPosition();
    });
    $("#position_id").on("change", function (e) {
        GetGradeList();
    });
    $("#area_l1_id").on("change", function (e) {
        GetAreaL2List();
        if (position_type == 31)    // 导购员
            GetDistributorList();
    });
    $("#area_l2_id").on("change", function (e) {
        if (position_type == 31)    // 导购员
            GetDistributorList();
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
}

/*图片上传成功回调函数*/
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
    //var img_index = $("form .row.mg-t").find("i").index(obj);//获取下标
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
function BindCompany() {
    $.ajax({
        url: "/OrganizationManage/Company/GetIdNameCategoryList",
        dataType: "json",
        async: false,
        success: function (data) {
            if (!data || data.length < 1)
                return;
            var optionStr = "";
            $.each(data, function (i) {
                optionStr += "<option value='" + data[i]["id"] + "' category='"
                    + data[i]["category"] + "'>" + data[i]["name"] + "</option>";
            });
            $("#company_id").append(optionStr);
            $("#company_id").select2({
                minimumResultsForSearch: 0, tags: false
            });
            cur_company = $("#company_id").val();
            BindDepartment();
            BindEmpCategory();
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

function BindRole() {
    $("#role_id").bindSelect({
        text: "name",
        url: "/SystemManage/MsRole/GetSelectJson",
        firstText: "--请选择所属角色--",
        search: true,
    });
}

function BindDepartment() {
    $("#dept_id").empty();
    $.ajax({
        url: "/OrganizationManage/Department/GetDeptAddrList?id=" + cur_company,
        dataType: "json",
        async: false,
        success: function (data) {
            var optionStr = "<option value='0'>--无所属部门--</option>";
            $.each(data, function (i) {
                optionStr += "<option value='" + data[i]["id"] + "' grade='"
                    + data[i]["grade_category"] + "' addr='" + data[i]["addr_str"] + "'>" + data[i]["name"] + "</option>";
            });
            $("#dept_id").append(optionStr);
            $("#dept_id").select2({
                minimumResultsForSearch: 0, tags: false
            });
            BindPosition();
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
            param: { dept_id: curDepartment, company_id: cur_company, },
            firstText: "--请选择职位--",
        });
        var addr = $("#dept_id").find('option:selected').attr("addr");
        $("#work_addr").val(addr);
    } else if (curDepartment == 0) { //无所属部门时
        $("#position_id").empty();
        $("#position_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Position/GetIdNameList",
            param: { company_id: cur_company, dept_id: 0 },
            firstText: "--请选择职位--",
        });
        var addr = $("#dept_id").find('option:selected').attr("addr");
        $("#work_addr").val(addr);
    }
}

//经理片区
function GetAreaL1List() {
    $("#area_l1_id").empty();
    $("#area_l1_id").empty();
    if (cur_company > 0) {
        $("#area_l1_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Area/GetManagerAreaIdNameList",
            param: { company_id: cur_company },
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

//经销商
// TODO 只选经理片区时，跨业务片区情况的处理
function GetDistributorList() {
    $("#distributor_id").empty();
    $("#distributor_id").bindSelect({
        text: "name",
        url: "/DistributorManage/DistributorManage/GetIdNameList",
        param: { company_id: cur_company, area_l1_id: $("#area_l1_id").val(), area_l2_id: $("#area_l2_id").val(), },
        firstText: "--请选择经销商--",
    });
}

// 1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门经理；6-部门主管；7-行政普通员工；11-培训经理；12-培训师；21-业务经理；22-业务员；31-导购员
function GetGradeList() {
    var position_id = $("#position_id").val();
    if (position_id < 1)
        return;
    $("#grade").empty();
    $.ajax({
        url: "/OrganizationManage/Position/GetGradeList",
        data: { position_id: position_id, grade_category: grade_category },
        dataType: "json",
        async: false,
        success: function (data) {
            position_type = data.positionInfo.position_type;
            $("#grade").bindSelectLocal({
                data: data.gradeList,
                id: "name",
                text: "name",
            });
            if (position_type == 31) //导购
            {
                $("#distributorWrap").show();
                $("#areaL1Wrap").show();
                $("#areaL2Wrap").show();
                $("#guideSpecificWrap").show();

                $("#role_id").val(4).trigger("change");
                //导购角色
                $("#role_id").attr("disabled", true);
                $("#introducer_name").val("");
                GetAreaL1List();
            } else if (position_type == 22) {//业务员
                $("#distributorWrap").hide();
                $("#areaL1Wrap").show();
                $("#areaL2Wrap").show();
                $("#role_id").val("").trigger("change");
                $("#role_id").attr("disabled", false);
                $("#guideSpecificWrap").hide();
                GetAreaL1List();
            } else if (position_type == 21) {//业务经理
                $("#distributorWrap").hide();
                $("#areaL2Wrap").hide();
                $("#areaL1Wrap").show();
                $("#role_id").val("").trigger("change");
                $("#role_id").attr("disabled", false);
                $("#guideSpecificWrap").hide();
                GetAreaL1List()
            }
            else {
                $("#role_id").val("").trigger("change");
                $("#role_id").attr("disabled", false);
                $("#guideSpecificWrap").hide();
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
}

var empFill = false;
$("#emp_fill").on("click", function () {
    empFill = $("#emp_fill").prop('checked');
    if (empFill == true) {
        $("#main-form").hide();
        $("#image_list").hide();
        $("#main-form2").show();
    } else {
        $("#main-form").show();
        $("#image_list").show();
        $("#main-form2").hide();
    }
});

function CheckIDCard(obj) {
    if (!identityCheck || $(obj).val() == "")
        return;
    var checkFlag = new clsIDCard($(obj).val());
    if (!checkFlag.IsValid(obj)) {
        return;
    }
    $("#birthdate").val(checkFlag.GetBirthDate());
    $("#birthdate").parents('.has-error').find('div.tooltip').remove();
    $("#birthdate").parents('.has-error').find('i.error').remove();
    $("#birthdate").parents().removeClass('has-error');

    $("#birthday").val(checkFlag.GetBirthday());
    $("#birthday").parents('.has-error').find('div.tooltip').remove();
    $("#birthday").parents('.has-error').find('i.error').remove();
    $("#birthday").parents().removeClass('has-error');

    CalculateAge($("#birthdate"));

    var sex = checkFlag.GetSex();
    $("[name='gender']").filter("[value='" + sex + "']").prop("checked", "checked");
}

function CalculateAge(obj) {
    var birthdate = $(obj).val().trim();
    if (!birthdate || birthdate == "")
        return;
    var curYear = (new Date()).getFullYear();
    var birthYear = (new Date(birthdate)).getFullYear();
    $("#age").val(curYear - birthYear);
    $("#age").parents('.has-error').find('div.tooltip').remove();
    $("#age").parents('.has-error').find('i.error').remove();
    $("#age").parents().removeClass('has-error');
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



function backForm() {
    window.location.href = "/HumanResource/EmployeeManage/Index";
}

//$(document).ready(function () {
//    $("#test-upload").fileinput({
//        'theme': 'fas',
//        'showPreview': false,
//        'allowedFileExtensions': ['jpg', 'png', 'gif'],
//        'elErrorContainer': '#errorBlock'
//    });
   
//    /*
//     $("#test-upload").on('fileloaded', function(event, file, previewId, index) {
//     alert('i = ' + index + ', id = ' + previewId + ', file = ' + file.name);
//     });
//     */
//});

//$("#file-1").fileinput({
//    theme: 'fas',
//    uploadUrl: "/HumanResource/EmployeeManage/Add?date=" + new Date(), // you must set a valid URL here else you will get an error
//    allowedFileExtensions: ['jpg', 'png', 'gif'],
//    overwriteInitial: false,
//    maxFileSize: 1000,
//    maxFilesNum: 9,
//    //allowedFileTypes: ['image', 'video', 'flash'],
//    slugCallback: function (filename) {
//        return filename.replace('(', '_').replace(']', '_');
//    }
//});

function submitForm() {
    var data = {};
    if (empFill) {
        if (!$("#main-form2").formValid())
            return false;
        data = $("#main-form2").formSerialize();
    }
    else {
        if (!$("#main-form").formValid())
            return false;
        if (identityCheck) {
            var checkFlag = new clsIDCard($("#identity").val());
            if (!checkFlag.IsValid($("#identity"))) {
                return;
            }
        }
        data = $("#main-form").formSerialize();
        data["image_list"] = picJson2;
        data.supervisor_id = supervisor_id;
        data.introducer_id = introducer_id;
        data["area_l1_name"] = $("#area_l1_id option:selected").text();
        data["area_l2_name"] = $("#area_l2_id option:selected").text();

        var distributorSelList = $('#distributor_id').find("option:selected");
        if (!!distributorSelList && distributorSelList.length > 0) {
            data.distributorList = [];
            $.each(distributorSelList, function (index, value) {
                var distributorInfo = {};
                distributorInfo.id = $(value).val();
                distributorInfo.name = $(value).text();
                data.distributorList.push(distributorInfo);
            });
        }
    }
    $.submitForm({
        url: "/HumanResource/EmployeeManage/Add?date=" + new Date(),
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
