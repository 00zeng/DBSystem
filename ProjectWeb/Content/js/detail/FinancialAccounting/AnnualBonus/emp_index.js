var EmpInfo = [];
$(function () {
    ActionBinding();
})
// 绑定标签事件
function ActionBinding() {
    $("#company_id").on("change", function (e) {
        GetDeptList();
    });
    $("#dept_id").on("change", function (e) {
        $("#bigBox").empty();
        GetEmpSalaryList();
    });
}
$("#company_id").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameList",
    search: true,
    firstText: "--请选择机构--",
});
//经理片区--调区
function GetDeptList() {
    var curCompany = $("#company_id").val();
    $("#dept_id").empty();
    if (curCompany > 0) {
        $("#dept_id").bindSelect({
            text: "name",
            url: "/OrganizationManage/Department/GetIdNameList",
            param: { company_id: curCompany },
            firstText: "--请选择部门--",
        });
    }
}
function GetEmpSalaryList() {
    $.ajax({
        url: "/FinancialAccounting/AnnualBonus/GetGridJson3?dept_id=" + $("#dept_id").val(),
        type: "get",
        dataType: "json",
        success: function (data) {
            console.log(data);
            var boxClone = "";
            EmpInfo = data.rows;
            var EmpLen = EmpInfo.length;
            if (EmpLen <= 0) {
                $("#orgBox").css("dispaly", "");
                $("#warningMsg").text("该部门无员工，请重新选择！");
            } else {
                $("#orgBox").css("dispaly", "none");
                for (var i = 0; i < EmpLen ; i++) {
                    boxClone = '<div class="box box-widget">'
                      + '<div class="box-header">'
                      + '<span style="margin-left:28px;">' + EmpInfo[i].name + '</span>'
                      + '<span style="margin-left:36px;">' + EmpInfo[i].position_name + '</span>'
                      + '<span style="margin-left:36px;">' + EmpInfo[i].grade + '</span>'
                      + '<span style="margin-left:36px;">入职时间：' + EmpInfo[i].entry_date.substring(0,10) + '</span>'
                      + '</div>'
                      + '<div class="box-body">'
                      + '<div class="col-xs-12 col-sm-6 col-lg-2">'
                      + '<div class="input-group margin-bottom formValue">'
                      + '<span class="input-group-addon no-border">年终奖金</span>'
                      + '<input type="text" name="amount" class="form-control" />'
                      + '<span class="input-group-addon">元</span>'
                      + '</div></div>'
                      + '<div class="col-xs-12 col-sm-6 col-lg-6">'
                      + '<div class="input-group margin-bottom formValue">'
                      + '<span class="input-group-addon no-border">奖金说明</span>'
                      + '<input type="text" name="note" class="form-control required">'
                      + '</div></div></div></div>';
                    $("#bigBox").append(boxClone);
                }
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, "error");
        }
    });
}