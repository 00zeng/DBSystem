var id = $.request("id");

$(function () {
    $.ajax({
        url: "/SubordinateManage/MySubordinate/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            $("#jobInfo").formSerializeShow(data.empJobInfo);
            if (data.posInfo.salary_category != 3 && data.posInfo.salary_category != '') {
                $("#Guide_tr").css("display", "");
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
})