$(function () {
    ActionBinding();
    $("#company_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Company/GetIdNameCategoryList",
        param: { category: "分公司" },
        search: true,
        firstText: "--请选择分公司--",
        change: function () {
            GetCompanyAddr();
        }
    });
    $("#select2-company_id-container").width("220px"); // 改变所属机构显示的宽度，原select2插件生成的显示宽度仅150px。
})

function ActionBinding() {
    $("#type").change(function () {
        if ($(this).val() == 1) {
            $("#manage").hide();
        } else if ($(this).val() == 2) {
            $("#manage").show();
        }
    });

    $("#company_id").on("change", function () {
        GetAreaL1List();
    });
}

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
    var curCompany = $("#company_id").val();
    $("#branchcompany_id").empty();
    $("#area_l1").empty();
    if (curCompany > 0) {
        $("#area_l1_id").bindSelect({
            text: "name",
            search: true,
            url: "/OrganizationManage/Area/GetManagerAreaIdNameList",
            param: { company_id: curCompany },
            firstText: "--请选择经理片区--",
        });
    }
}

function GetCompanyAddr() {
    var curCompany = $("#company_id").val();
    if (curCompany > 0) {
        $.ajax({
            url: "/OrganizationManage/Company/GetAddr?date=" + new Date(),
            type: "get",
            data: { id: curCompany },
            dataType: "json",
            success: function (data) {
                if (!!data) {
                    // Key-Value pair, key refers to city and value refers to address
                    $("#city").val(data.Key).trigger("change");
                    $("#address").val(data.Value);
                }
            },
            error: function (data) {
                $.modalAlert(data.responseText, 'error');
            }
        });
    }
}

function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var data = $("#form1").formSerialize();
    if (!(!!data["name"])) {
        alert("请输入区域名称");
        return;
    }
    var addrCity = $("#addrCity .select-item");
    data["city_code"] = $(addrCity).length > 0 ? $(addrCity).last().data("code") : "";
    var type = $("#type").val();
    if (type == 2)
        data.parent_id = $("#area_l1_id").val();
    $.submitForm({
        url: "/OrganizationManage/Area/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                if(type==1)
                    window.location.href = "/OrganizationManage/Area/AreaL1Index";
                else if (type == 2)
                    window.location.href = "/OrganizationManage/Area/AreaL2Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
