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
                if (data.company_name == null || data.company_name == "" || data.company_name == "-") {
                    $("#company_name").text("--无--");
                    $("#company_name1").text("--无--");
                }
                else {
                    $("#company_name").text(data.company_name);
                    $("#company_name1").text(data.company_name);
                }

                if (data.area_l1_name == null || data.area_l1_name == "" || data.area_l1_name == "-")
                    $("#area_l1_name").text("--无--");
                else
                    $("#area_l1_name").text(data.area_l1_name);

                if (data.area_l2_name == null || data.area_l2_name == "" || data.area_l1_name == "-")
                    $("#area_l2_name").text("--无--");
                else
                    $("#area_l2_name").text(data.area_l2_name);

                if (data.distributor_name == null || data.distributor_name == "" || data.area_l1_name == "-")
                    $("#distributor_name").text("--无--");
                else
                    $("#distributor_name").text(data.distributor_name);

            }
            ActionBinding();
            GetAreaL1List();
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

// 绑定标签事件
function ActionBinding() {
    GetAreaL1List();

    $("#area_l1_id").on("change", function (e) {
        $("#area_l2_id").empty();
        GetAreaL2List();
    });
}

//经理片区
function GetAreaL1List() {
    var curCompany = company_id;
    $("#area_l1_id").empty();
    $("#area_l2_id").val("").trigger("change");
    $("#area_l2_id").empty();
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
function submitForm() {
    var data = {};

    var area_l1_id = $('#area_l1_id').val();
    if (!area_l1_id || area_l1_id.length < 1) {
        $.modalAlert("请选择经理片区");
        return;
    }

    var area_l2_id = $('#area_l2_id').val();
    if (!area_l2_id || area_l2_id.length < 1) {
        $.modalAlert("请选择业务片区");
        return;
    }

    var start_date = $('#start_date').val();
    if (!start_date || start_date.length < 1) {
        $.modalAlert("请选择生效时间");
        return;
    }

    data["area_l2_id"] = $("#area_l2_id").val();
    data["effect_date"] = $("#start_date").val();
    data["emp_id"] = id;
    $.submitForm({
        url: "/SubordinateManage/Assignment/SalesAdd?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/SubordinateManage/Assignment/SalesIndex";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}



