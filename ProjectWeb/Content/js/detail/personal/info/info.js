var id;
var gender;
var political;
var birthday_type;
var marriage;
var resign_type;

var jobInfo = {};

$(function () {
    $.ajax({
        url: "/PersonalManage/Info/ShowInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        success: function (data) {
            id = data.employeeInfo.id;
            jobInfo["id"] = id;
            $("#job_number").text(data.posInfo.job_number);
            $("#name").text(data.employeeInfo.name);
            $("#phone").text(data.employeeInfo.phone);
            $("#wechat").text(data.employeeInfo.wechat);
            $("#emergency_contact").text(data.employeeInfo.emergency_contact);
            $("#emergency_contact_phone").text(data.employeeInfo.emergency_contact_phone);
            gender = data.employeeInfo.gender;
            if (gender == 0) {
                $("#gender").text("女");
            } else if (gender == 1) {
                $("#gender").text("男");
            } else {
                $("#gender").text("未指定");
            }
            political = data.employeeInfo.political;
            if (political == 1) {
                $("#political").text("中共党员");
            } else if (political == 2) {
                $("#political").text("团员");
            } else if (political == 3) {
                $("#political").text("群众");
            } else if (political == 4) {
                $("#political").text("民主党派");
            } else if (political == 5) {
                $("#political").text("无党派人士");
            } else if (political == 6) {
                $("#political").text("其他");
            } 
            $("#birthdate").text(data.employeeInfo.birthdate);
            $("#age").text(data.employeeInfo.age);
            $("#birthday").text(data.employeeInfo.birthday);
           
            birthday_type = data.employeeInfo.birthday_type;
            if (birthday_type == 1) {
                $("#birthday_type").text("农历");
            } else {
                $("#birthday_type").text("阳历");
            }
          
            marriage = data.employeeInfo.marriage;
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
            $("#number_children").text(data.employeeInfo.number_children);
            $("#native").text(data.employeeInfo.native);
            $("#nation").text(data.employeeInfo.nation);
            $("#country").text(data.employeeInfo.country);
            $("#address").text(data.employeeInfo.address);
            $("#idtype").text(data.employeeInfo.idtype);
            $("#identity").text(data.employeeInfo.identity);
            $("#idaddress").text(data.employeeInfo.idaddress);
            $("#id_begin").text(data.employeeInfo.id_begin);
            $("#id_end").text(data.employeeInfo.id_end);
            $("#idIssued").text(data.employeeInfo.idIssued);
            $("#health_begin").text(data.employeeInfo.health_begin);
            $("#health_end").text(data.employeeInfo.health_end);
            $("#education").text(data.employeeInfo.education);
            $("#profession").text(data.employeeInfo.profession);
            $("#graduation_school").text(data.employeeInfo.graduation_school);
            $("#graduation_day").text(data.employeeInfo.graduation_day);
            $("#submitter").text(data.employeeInfo.submitter);
            $("#submit_day").text(data.employeeInfo.submit_day);
            $("#note").text(data.employeeInfo.note);
            //银行信息
            $("#bank_type").text(data.employeeInfo.bank_type);
            $("#bank_name").text(data.employeeInfo.bank_name);
            $("#bank_account").text(data.employeeInfo.bank_account);
            //家庭信息
            $("#parents_card").text(data.employeeInfo.parents_card);
            $("#parents_card_number").text(data.employeeInfo.parents_card_number);
            $("#parents_phone").text(data.employeeInfo.parents_phone);
            //职务信息
            $("#position_name").text(data.posInfo.position_name);
            $("#organization_name").text(data.posInfo.organization_name);
            $("#department_name").text(data.posInfo.department_name);
            $("#supervisor_name").text(data.posInfo.supervisor_name);
            $("#emp_category").text(data.posInfo.emp_category);
            $("#salary").text(data.posInfo.salary);
            $("#working_place").text(data.posInfo.working_place);
            $("#taxable_unit").text(data.posInfo.taxable_unit);
            $("#social_security").text(data.posInfo.social_security);
            $("#entry_time").text(data.posInfo.entry_time);
            $("#working_years").text(data.posInfo.working_years);
            $("#contract_signing_date").text(data.posInfo.contract_signing_date);
            $("#contract_expiration_date").text(data.posInfo.contract_expiration_date);
            $("#selling_point_area").text(data.posInfo.selling_point_area);
            $("#selling_point").text(data.posInfo.selling_point);
            $("#shopping_guide_category").text(data.posInfo.shopping_guide_category);
            $("#salesman").text(data.posInfo.salesman);
            $("#trainer").text(data.posInfo.trainer);
            $("#deposit_leave").text(data.posInfo.deposit_leave);
            //状态
            emp_status = data.employeeInfo.emp_status;
            if (emp_status == 0) {
                $("#emp_status").text("在职");
            } else if (emp_status == 1) {
                $("#emp_status").text("离职");
            } else if (emp_status == 2) {
                $("#emp_status").text("休假");
            }
            $("#rest_time").text(data.posInfo.rest_time);
            //离职方式
            resign_type = data.employeeInfo.resign_type;
            if (resign_type == 1) {
                $("#resign_type").text("正常离职");
            } else if (resign_type == 2) {
                $("#resign_type").text("辞退");
            } else if (resign_type == 3) {
                $("#resign_type").text("自离");
            } 
            $("#content").text(data.posInfo.content);
            $("#report_person").text(data.posInfo.report_person);
            $("#deductible_insurance").text(data.posInfo.deductible_insurance);
            $("#others_insurance").text(data.posInfo.others_insurance);
            $("#deposit_return").text(data.posInfo.deposit_return);
            //系统账户
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }

    });
});

function updateForm() {
    window.location.href = "/PersonalManage/Info/PersonalEdit?jobInfo=" + jobInfo;
}



function startForm() {
    if (inactive == 0) {
        alert("该经销商属于已启动状态");
        return "已启动";
    }
    var data = {};
    var hint = "确认将" + id + "启动？";
    top.layer.confirm(hint, function (index) {
        var data = {};
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        data["id"] = id;
        $.submitForm({
            url: "/PersonalManage/Info/SubmitActive?date=" + new Date(),
            param: data,
            success: function (data) {
                if (data.state == "success") {
                    $.currentWindow().location.reload();

                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })

}




/*删除数据*/
function DelForm() {

    top.layer.confirm("确认要删除经销商: " + distributor_name, function (index) {
        var data = {};
        if ($('[name=__RequestVerificationToken]').length > 0) {
            data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }

        data["id"] = id;
        $.ajax({
            url: "/PersonalManage/Info/Delete?date=" + new Date(),
            type: "post",
            data: data,
            success: function (data) {
                window.location.href = "/PersonalManage/Info/Index";
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        })
    })
}