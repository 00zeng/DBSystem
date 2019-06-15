var GuideInfo = [];
$(function () {
    ActionBinding();
})
// 绑定标签事件
function ActionBinding() {
    $("#company_id").on("change", function (e) {
        GetAreaL1List();
    });
    $("#area_l1_id").on("change", function (e) {
        GetAreaL2List();
    });
    $("#area_l2_id").on("change", function (e) {
        GetEmpSalaryList();
    });
}
$("#company_id").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameList",
    search: true,
    firstText: "--请选择机构--",
});
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
//$('#trainerDetail').attr('data-href', "/FinancialAccounting/EmployeeSalary/TrainerCheck?id=" + id);
function GetEmpSalaryList() {
    $.ajax({
        url: "/FinancialAccounting/AnnualBonus/GuideBonus?area_l2_id=" + $("#area_l2_id").val(),
        //url: "/FinancialAccounting/AnnualBonus/GetGridJson3?dept_id=" + $("#dept_id").val(),
        type: "get",
        dataType: "json",
        success: function (data) {
            var boxClone = "";
            GuideInfo = data.rows;
            var GuideLen = GuideInfo.length;
            if (GuideLen <= 0) {
                $("#orgBox").css("dispaly", "");
                $("#warningMsg").text("该部门无员工，请重新选择！");
            } else {
                $("#orgBox").css("dispaly", "none");
                for (var i = 0; i < GuideLen ; i++) {
                    boxClone = '<div class="box box-widget">'
                      + '<div class="box-header">'
                      + '<span style="margin-left:28px;">' + GuideInfo[i].name + '</span>'
                      + '<span style="margin-left:36px;">' + GuideInfo[i].position_name + '</span>'
                      + '<span style="margin-left:36px;">' + GuideInfo[i].grade + '</span>'
                      + '<span style="margin-left:36px;">入职时间：' + GuideInfo[i].entry_date.substring(0, 10) + '</span>'
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
                      + '</div></div>'
                      + '<div class="col-xs-2 col-sm-2 col-lg-2">'
                      + '<a class="label label-primary btnDetailTag"   data-href="/FinancialAccounting/AnnualBonus/GuideBonusDetail?id="' + GuideInfo[i].id + '  data-btndetail="导购员年终奖核对">导购员年终奖核对</a>'
                      + '</div></div></div>';
                    $("#bigBox").append(boxClone);
                }
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, "error");
        }
    });
}