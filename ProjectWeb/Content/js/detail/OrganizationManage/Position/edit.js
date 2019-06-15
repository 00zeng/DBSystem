var id = $.request("id");
$(function () {
    $.ajax({
        url: "/OrganizationManage/Position/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            if (!!data)
            {
                $("#form1").formSerialize(data);
                switch (data.position_type)
                {
                    case 1: $("#position_type").val("事业部总经理"); break;
                    case 2: $("#position_type").val("事业部助理"); break;
                    case 3: $("#position_type").val("分公司总经理"); break;
                    case 4: $("#position_type").val("分公司助理"); break;
                    case 5: $("#position_type").val("部门经理"); break;
                    case 6: $("#position_type").val("部门主管"); break;
                    case 7: $("#position_type").val("行政普通员工"); break;
                    case 11: $("#position_type").val("培训经理"); break;
                    case 12: $("#position_type").val("培训师"); break;
                    case 21: $("#position_type").val("业务经理"); break;
                    case 22: $("#position_type").val("业务员"); break;
                    case 31: $("#position_type").val("导购员"); break;
                    default: $("#position_type").val("行政普通员工"); break;
                }
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

function submitForm() {
    if (!$("#form1").formValid())
        return false;
    var data = {};
    data["id"] = id;
    data["name"] = $("#name").val();
    data["note"] = $("#note").val();

    $.submitForm({
        url: "/OrganizationManage/Position/Edit?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/OrganizationManage/Position/Index";
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
