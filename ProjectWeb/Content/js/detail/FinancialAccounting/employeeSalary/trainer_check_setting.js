var id = $.request("id");
var company_name = decodeURI($.request("company_name"));
$("#company_name").text(company_name);
var position_name = decodeURI($.request("position_name"));
$("#position_name").text(position_name);
var name = decodeURI($.request("name"));
$("#name").text(name);
var entry_date = $.request("entry_date");
$("#entry_date").text(entry_date.substring(0, 10));
var grade = decodeURI($.request("grade"));
$("#grade").text(grade);
var position_type = $.request("position_type");

////是否为培训师
//if (position_name != "培训师") {
//    $("#snowball_number_div").css('display', 'none');
//    $("#snowball_ratio_div").css('display', 'none');
//    $("#product_train_div").css('display', 'none');
//    $("#image_efficiency_div").css('display', 'none');
//    $("#image_fine_div").css('display', 'none');
//} 

if (position_type != 12) {
    $("#box2").css("display", "");
} else {
    $("#box1").css("display", "");
    $("#box2").css("display", "");
    $("#box3").css("display", "");

}
//绑定经理片区
var areaidList = $("select[name='area_l1_id']");
for (var i = 0; i < areaidList.length; i++) {
    $(areaidList[i]).bindSelect({
        text: "name",
        url: "/OrganizationManage/Area/GetManagerAreaIdNameList",
        search: true,
        firstText: "--请选择经理片区--",
    });   
}
   

//不考核设置
    function setDisabled() {
        var uncheckList = $("input[name='uncheck']");
        var areaList = $("select[name='area_l1_id']");
        for (var i = 0; i < uncheckList.length; i++) {
            if ($(uncheckList[i]).prop("checked") == true) {
                $(areaList[i]).attr("disabled", true);
                $(areaList[i]).val('').trigger("change");
            } else {
                $(areaList[i]).removeAttr("disabled");
            }
        }      
    }

//立即生效 
function Effective(checkbox) {
    if (checkbox.checked == true) {
        $("#effect_date").val('');
        $("#effect_date").attr("disabled", true);
    } else {
        $("#effect_date").removeAttr("disabled");
    }
}

function submitForm() {
    if (!$("#form1").formValid())
        return false;

    var data = $("#form1").formSerialize();
    if ($("#effect_date").val() == '' && $("#effect_now").prop("checked") == false) {
        $.modalAlert("请选择生效时间");
        return;
    }
    data["effect_now"] = $("#effect_now").prop("checked");

    var subList = [];
    var uncheckList = $("input[name='uncheck']");
    var areaList = $("select[name='area_l1_id']");
    if (position_type == 12) {
        var trainerno = 1;
        for (var i = 0; i < uncheckList.length; i++) {
            var checkValue = {};
            checkValue.kpi_type = trainerno;
            var check = $(uncheckList[i]).prop("checked");
            if (check == false) {
                checkValue.is_included = true;
            } else {
                checkValue.is_included = false;
            }
            if ($(areaList[i]).val() == 0 && $(uncheckList[i]).prop("checked")==false) {
                $.modalAlert("请选择不考核的经理片区");
                return;
            }
            checkValue.area_l1_id = $(areaList[i]).val();
            checkValue.area_l1_name = $(areaList[i]).find("option:selected").text();
            subList.push(checkValue);
            trainerno++;
        }
    } else {
        var managerno = 51;
        for (var i = 0; i < uncheckList.length; i++) {
            var checkValue = {};
            if (i == 2 || i == 3 || i == 4) {
                checkValue.kpi_type = managerno;
                var check = $(uncheckList[i]).prop("checked");
                if (check == false) {
                    checkValue.is_included = true;
                } else {
                    checkValue.is_included = false;
                }
                if ($(areaList[i]).val() == 0 && $(uncheckList[i]).prop("checked") == false) {
                    $.modalAlert("请选择不考核的经理片区");
                    return;
                }
                checkValue.area_l1_id = $(areaList[i]).val();
                checkValue.area_l1_name = $(areaList[i]).find("option:selected").text();
                subList.push(checkValue);
                managerno++;
            }
        }
    }

    data['subList'] = subList;
    data["emp_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/EmployeeSalary/AddTrainer?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/EmployeeSalary/Index?id=" + id;

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}