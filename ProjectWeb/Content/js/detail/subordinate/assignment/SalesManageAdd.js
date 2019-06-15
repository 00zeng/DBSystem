var id = $.request("id");
var name = decodeURI($.request("name"));
$("#name").text(name);
var company_id;
//查看当前
$(function () {
    $.ajax({
        url: "/SubordinateManage/Assignment/GetSalesInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            var date = data.effect_date.substring(0, 10);
            $("#effect_date").val(date);
            company_id = data.company_id;
            if (!!data) {
                $("#company_name").text(data.company_name);
                $("#company_name1").text(data.company_name);
                if (data.area_l1_name == null || data.area_l1_name == "" || data.area_l1_name == "-")
                    $("#area_l1_name").text("--无--");
                else
                    $("#area_l1_name").text(data.area_l1_name);
            }

            $(function () {
                ActionBinding();
                BindCompany();
                GetAreaL1List();
            })
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

// 绑定标签事件
function ActionBinding() {
        GetAreaL1List();
}
//分公司
function BindCompany() {
    $("#company_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Company/GetIdNameCategoryList",
        param: { category: "分公司" },
        search: true,
    });
}

//经理片区
function GetAreaL1List() {
    var curCompany = company_id;
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
function submitForm() {
    var data = {};

    var area_l1_id = $('#area_l1_id').val();
    if (!area_l1_id || area_l1_id.length < 1) {
        $.modalAlert("请选择经理片区");
        return;
    }

    var start_date = $('#start_date').val();
    if (!start_date || start_date.length < 1) {
        $.modalAlert("请选择生效时间");
        return;
    }

    data["area_l1_id"] = $("#area_l1_id").val();
    data["effect_date"] = $("#start_date").val();
    data["emp_id"] = id;
    $.submitForm({
        url: "/SubordinateManage/Assignment/SalesManageAdd?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/SubordinateManage/Assignment/SalesManagerIndex";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}



