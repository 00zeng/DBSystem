
var idtype;//证件类别
var gender;//性别
var political;//政治面貌
var birthday_type;//生日类型
var marriage;//婚姻状况
var resign_type;//离职方式
var myInfo = top.clients.loginInfo;
id = myInfo.empId;
$(function () {
    $.ajax({
        url: "/HumanResource/EmployeeManage/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            
            if (data.empJobInfo.entry_date == null || data.empJobInfo.entry_date == "") {
                if (window.confirm('您是新员工，请先完善个人信息')) {
                    window.location.href = "/PersonalManage/EmpInfo/edit?id=" + id;
                }
            }
            $("#form1").formSerializeShow(data.empJobInfo);
            $("#work_number").text(data.empJobInfo.work_number);// 不在“jobInfo”表单内
            $("#name").text(data.empJobInfo.name);  // 不在“jobInfo”表单内
            $("#account").text(data.accountInfo.account);
            $("#role_name").text(data.accountInfo.role_name);
            if (!!(data.empInfo)) {
                $("#form1").formSerializeShow(data.empInfo);
                //CalculateAge($("#birthdate"));
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
                if (data.empJobInfo.status == 0)
                    $("#emp_status").text("正常");
                else
                    $("#emp_status").text("休假");
            }
            //是否购买社保
            var insurance_paid = data.empJobInfo.insurance_paid;
            if (insurance_paid == true) {
                $("#insurance_paid").text("是");
            } else {
                $("#insurance_paid").text("否");
            }
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
            $.each(data.imageList, function (index, item) {
                $divId = getTypeName(item.type);
                $("#" + $divId).append('<div show="open" class="col-sm-6" id="my_license">'
                                + '<div class="thumbnail">'
                                + '<img show="open" class="img-responsive" src="' + item.url_path + '" />'
                                + '</div>'
                                + '</div>')
            })
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }

    });


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

//function CalculateAge(obj) {
//    var birthdate = $(obj).val().trim();
//    if (!birthdate || birthdate == "")
//        return;
//    var curYear = (new Date()).getFullYear();
//    var birthYear = (new Date(birthdate)).getFullYear();
//    $("#age").val(curYear - birthYear + 1);
//}


function OpenForm(url, title) {
    $.modalOpen({
        id: "Form",
        title: '修改个人信息',
        url: '/PersonalManage/EmpInfo/Edit?id='+id,
        width: "600px",
        height: "340px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}

//重新查询
function ReSearch() {
    window.location.reload();
}