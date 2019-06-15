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

//是否为实习生
var emp_category = decodeURI($.request("emp_category"));
if (emp_category == "实习生") {
    $("#EmpCategory").css("display", "");
    $("#position_name").text(position_name + "(" + emp_category + ")");
}

$("input[name='intern_salary_type']").on('change', function () {
    $intern_salary_type = $(this).val();
    if ($intern_salary_type == 1) {
        $("#salary_fix").css('display', 'none');
        $("#salary_ratio").css('display', '');
        $("#intern_fix_salary").val('');
    } else if ($intern_salary_type == 2) {
        $("#salary_ratio").css('display', 'none');
        $("#salary_fix").css('display', '');
        $("#intern_ratio_salary").val('');
    }
});
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
    data["intern_salary_type"] = $("input[name=intern_salary_type]:checked").val();
    data["emp_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/EmployeeSalary/AddGeneral?date=" + new Date(),
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