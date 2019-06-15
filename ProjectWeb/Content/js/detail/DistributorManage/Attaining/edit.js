var id = $.request("id");
var start_date = $.request("start_date");
var end_date = $.request("end_date");
var startDate = (new Date(start_date)).pattern("yyyy-MM-dd");
var endDate = (new Date(end_date)).pattern("yyyy-MM-dd");
$("#start_date").val(startDate);
$("#end_date").val(endDate);


function submitForm() {
    if (!$("#form1").formValid())
        return false;

    var alterDate = $("#alterDate").val();
    if (!alterDate) {
        $.modalAlert("设置结束时间不能为空！", "error");
        return false;
    } else {
        if (alterDate == endDate) {
            $.modalAlert("修改结束时间与活动结束时间一致！", "error");
            return false;
        }
    }
    var data = { id: id, alterDate: alterDate };
    if ($('[name=__RequestVerificationToken]').length > 0) {
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.submitForm({
        url: "/DistributorManage/Attaining/Alter?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                //$.currentWindow().location.reload();
                top.layer.closeAll();
                
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}
