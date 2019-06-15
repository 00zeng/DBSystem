var id = $.request("id");
var $tableGuideSale = $('#gridList1');
var $tableHighLevelSale = $('#gridList2');
var position_type;
//V雪球活跃人数
var snowball_number_result;
var snowball_number_KPI = 0;
var snowball_number_standard;
var snowball_number_assess;
//V雪球转化率
var snowball_ratio_result;
var snowball_ratio_KPI = 0;
var snowball_ratio_standard;
var snowball_ratio_assess;
//导购人均产值
var shopguide_average_standard = 0;
var shopguide_average_assess = 0;
var kpiRes3 = 0;
var shopguide_average_result1 = 0;
var shopguide_average_result = 0;
//导购离职率
var shopguide_resign_result = 0;//月底
var shopguide_resign_result1 = 0;//离职
var shopguide_resign_KPI = 0;
var shopguide_resign_standard;
var shopguide_resign_assess = 0;
//高端机占比
var product_expensive_result;
var kpiRes5 = 0;
var product_expensive_standard;
var product_expensive_assess;
//培训场次
var product_train_result;
var product_train_KPI = 0;
var product_train_standard;
var product_train_assess;
//形象执行效率
var image_efficiency_result;
var image_efficiency_KPI = 0;
var image_efficiency_standard;
//形象罚款
var image_fine_result;
var image_fine_KPI = 0;
var image_fine_standard;
//终端经理打分
var manager_scoring_result1;
var manager_scoring_result;
var manager_scoring_KPI = 0;
var manager_scoring_standard;
//累计
var trainer_sum_KPI = 0;
var empKPIList;
var KPIResultList = $("input[name='kpi_result']");
//培训场次的建议金额  计算KPI需用到，获取前端有千位符号影响计算 需定义原始数据（可能很多数据都这样）----by Zeng
var product_train_advice = 0;
$(function () {
    $("#form1").queryFormValidate();
    $("#querySubmit1").on('click', querySubmitGuide);
    $("#querySubmit2").on('click', querySubmitProduct);
    $.ajax({
        url: "/FinancialAccounting/trainerKPI/GetSettingInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            console.log(data);
            $("#form1").formSerializeShow(data.empInfo);
            $("#form1").formSerializeShow(data.positionKPI);

            $("#month").val(data.month.substring(0, 7));
            //if ($("#month").val() != '') {
            //    Month($("#month").val());
            //}
            position_type = data.empInfo.position_type;

            empKPIList = data.empKPIList;

            if (position_type == 12) {//12-培训师 
                $("#position_type_name").text("培训师");
                $("#image_efficiency_advice").text(ToThousandsStr(data.positionKPI.image_efficiency_advice));
                $("#image_efficiency_standard").text(ToThousandsStr(data.positionKPI.image_efficiency_standard));
                image_fine_advice = data.positionKPI.image_fine_advice;
                $("#image_fine_advice").text(ToThousandsStr(image_fine_advice));
                $("#image_fine_number").text(ToThousandsStr(data.positionKPI.image_fine_number));
                $("#image_fine_standard").text(ToThousandsStr(data.positionKPI.image_fine_standard));
                $("#manager_scoring_advice").text(ToThousandsStr(data.positionKPI.manager_scoring_advice));
                $("#manager_scoring_standard").text(ToThousandsStr(data.positionKPI.manager_scoring_standard));
                $("#product_expensive_advice").text(ToThousandsStr(data.positionKPI.product_expensive_advice));
                $("#product_expensive_assess").text(ToThousandsStr(data.positionKPI.product_expensive_assess));
                $("#product_expensive_standard").text(ToThousandsStr(data.positionKPI.product_expensive_standard));
                product_train_advice = data.positionKPI.product_train_advice;
                $("#product_train_advice").text(ToThousandsStr(product_train_advice));
                $("#product_train_assess").text(ToThousandsStr(data.positionKPI.product_train_assess));
                $("#product_train_standard").text(ToThousandsStr(data.positionKPI.product_train_standard));
                $("#shopguide_average_advice").text(ToThousandsStr(data.positionKPI.shopguide_average_advice));
                $("#shopguide_average_assess").text(ToThousandsStr(data.positionKPI.shopguide_average_assess));
                $("#shopguide_average_standard").text(ToThousandsStr(data.positionKPI.shopguide_average_standard));
                $("#shopguide_resign_advice").text(ToThousandsStr(data.positionKPI.shopguide_resign_advice));
                $("#shopguide_resign_assess").text(ToThousandsStr(data.positionKPI.shopguide_resign_assess));
                $("#shopguide_resign_standard").text(ToThousandsStr(data.positionKPI.shopguide_resign_standard));
                snowball_number_advice = data.positionKPI.snowball_number_advice;
                $("#snowball_number_advice").text(ToThousandsStr(snowball_number_advice));
                $("#snowball_number_assess").text(ToThousandsStr(data.positionKPI.snowball_number_assess));
                $("#snowball_number_standard").text(ToThousandsStr(data.positionKPI.snowball_number_standard));
                $("#snowball_ratio_advice").text(ToThousandsStr(data.positionKPI.snowball_ratio_advice));
                $("#snowball_ratio_assess").text(ToThousandsStr(data.positionKPI.snowball_ratio_assess));
                $("#snowball_ratio_standard").text(ToThousandsStr(data.positionKPI.snowball_ratio_standard));
                //V雪球活跃人数
                snowball_number_standard = data.positionKPI.snowball_number_standard;
                snowball_number_assess = data.positionKPI.snowball_number_assess;
                //V雪球转化率
                snowball_ratio_assess = data.positionKPI.snowball_ratio_assess;
                snowball_ratio_standard = data.positionKPI.snowball_ratio_standard;
                //导购人均产值
                shopguide_average_standard = data.positionKPI.shopguide_average_standard;
                shopguide_average_assess = data.positionKPI.shopguide_average_assess;
                //导购离职率
                shopguide_resign_standard = data.positionKPI.shopguide_resign_standard;
                shopguide_resign_assess = data.positionKPI.shopguide_resign_assess;
                //高端机占比
                product_expensive_standard = data.positionKPI.product_expensive_standard;
                product_expensive_assess = data.positionKPI.product_expensive_assess;
                //培训场次
                product_train_standard = data.positionKPI.product_train_standard;
                product_train_assess = data.positionKPI.product_train_assess;
                //形象执行效率
                image_efficiency_standard = data.positionKPI.image_efficiency_standard;
                //形象罚款
                image_fine_standard = data.positionKPI.image_fine_standard;
                //终端经理打分
                manager_scoring_standard = data.positionKPI.manager_scoring_standard;

                $(".wrap9").show();
                $.each(empKPIList, function (index, item) {
                    if (item.is_included) {
                        $(".wrap" + (index + 1)).show();
                        $(".wrap" + (index + 1)).find("span[name = area_l1_name]").text("(" + item.area_l1_name + ")");
                    }
                })
            }
            else {
                //11 - 培训主管
                $.each(empKPIList, function (index, item) {
                    $("#position_type_name").text("培训主管");
                    $(".advice").css("display", "none");
                    //导购人均产值
                    shopguide_average_standard = data.positionKPI.average_standard_money;
                    $("#shopguide_average_standard").text(shopguide_average_standard);
                    shopguide_average_assess = data.positionKPI.average_standard_number;
                    $("#shopguide_average_assess").text(shopguide_average_assess);

                    //导购离职率
                    shopguide_resign_standard = data.positionKPI.resign_standard_money;
                    $("#shopguide_resign_standard").text(shopguide_resign_standard);

                    shopguide_resign_assess = data.positionKPI.resign_standard_ratio;
                    $("#shopguide_resign_assess").text(shopguide_resign_assess);

                    //高端机占比
                    product_expensive_standard = data.positionKPI.product_expensive_money;
                    $("#product_expensive_standard").text(product_expensive_standard);

                    product_expensive_assess = data.positionKPI.product_expensive_ratio;
                    $("#product_expensive_assess").text(product_expensive_assess);
                    if (item.is_included) {
                        if (item.kpi_type == 51) {
                            $(".wrap3").show();
                            $(".wrap3").find("span[name = area_l1_name]").text("(" + item.area_l1_name + ")");

                            $("#shopguide_average_standard").text(ToThousandsStr(data.positionKPI.average_standard_money));
                            $("#shopguide_average_assess").text(ToThousandsStr(data.positionKPI.average_standard_number));
                        } else if (item.kpi_type == 52) {
                            $(".wrap4").show();
                            $(".wrap4").find("span[name = area_l1_name]").text("(" + item.area_l1_name + ")");

                            $("#shopguide_resign_standard").text(ToThousandsStr(data.positionKPI.resign_standard_money));
                            $("#shopguide_resign_assess").text(ToThousandsStr(data.positionKPI.resign_standard_ratio));
                        } else if (item.kpi_type == 53) {
                            $(".wrap5").show();
                            $(".wrap5").find("span[name = area_l1_name]").text("(" + item.area_l1_name + ")");

                            $("#product_expensive_assess").text(ToThousandsStr(data.positionKPI.product_expensive_ratio));
                            $("#product_expensive_standard").text(ToThousandsStr(data.positionKPI.product_expensive_money));
                        }
                    }
                })
            }
            $.each(data.detailList, function (index, item) {
                if (item.kpi_type == 3 || item.kpi_type == 51) {//导购人均产值
                    shopguide_average_result1 = item.guide_amount;
                    shopguide_average_result = item.guide_count;

                    if (item.kpi_score >= Number(shopguide_average_assess))
                        kpiRes3 = Number(shopguide_average_standard);
                    else if (item.kpi_score > 0)
                        kpiRes3 = (Number(shopguide_average_result1) / Number(shopguide_average_result)) / Number(shopguide_average_assess) * Number(shopguide_average_standard)
                    else
                        kpiRes3 = 0;
                    kpiRes3 = kpiRes3 < 0 ? 0 : kpiRes3.toFixed(2);
                    $("#shopguide_average_result1").val(shopguide_average_result1);//销量
                    $("#shopguide_average_result").val(shopguide_average_result);//人数
                    $("#shopguide_average_KPI").text(kpiRes3);
                }

                if (item.kpi_type == 5 || item.kpi_type == 53) {//高端机占比
                    product_expensive_result = item.kpi_score;
                    if (item.kpi_score >= Number(product_expensive_assess))
                        kpiRes5 = Number(product_expensive_standard);
                    else if (product_expensive_result > 0)
                        kpiRes5 = [1 - (Number(product_expensive_assess) / 100.00 - Number(product_expensive_result) / 100.00) * 5] * Number(product_expensive_standard);
                    else
                        kpiRes5 = 0;
                    kpiRes5 = kpiRes5 < 0 ? 0 : kpiRes5.toFixed(2);
                    $("#product_expensive_result").val(item.kpi_score);
                    $("#product_expensive_KPI").text(kpiRes5);
                }
            });
            getSum();
            initTable1();
            initTable2();
        },
        error: function (data) {
            if (data.responseText == "noKPISetting") {
                $('#modalMessage').modal('show');
                $('#trainerDetail').attr('data-href', "/FinancialAccounting/EmployeeSalary/TrainerCheck?id=" + id);
            } else if (data.responseText == "noPositionKPISetting") {
                $('#modalPosition').modal('show');
                $('#saleDetail2').attr('data-href', "/FinancialAccounting/PositionSalary/TrainerSetting?id=" + id);
            } else {
                $.modalAlert(data.responseText, "error");
            }
        }
    })

});
$("#modalMessage").on('hide.bs.modal', function (e) {
    window.location.href = "/FinancialAccounting/trainerKPI/EmpIndex";
})
$("#modalPosition").on('hide.bs.modal', function (e) {
    window.location.href = "/FinancialAccounting/trainerKPI/EmpIndex";
})


function Month(obj) {
    $.ajax({
        url: "/FinancialAccounting/trainerKPI/calculateRatio",
        type: "get",
        dataType: "json",
        data: { id: id, month: $("#month").val() },
        success: function (data) {
            if (!data || data == null || data.length < 1) {
                $("#shopguide_average_result1").val(0);//销量
                $("#shopguide_average_result").val(0);//人数
                $("#product_expensive_result").val(0);
                $("#shopguide_average_KPI").text(0);
                kpiRes3 = 0;
                $("#product_expensive_KPI").text(0);
                kpiRes5 = 0;
                $.modalAlert("信息不存在");
                getSum();
            }
            $.each(data, function (index, item) {
                if (item.kpi_type == 5) {//高端机
                    product_expensive_result = item.kpi_score;
                    if (item.kpi_score >= Number(product_expensive_assess))
                        kpiRes5 = Number(product_expensive_standard);
                    else if (product_expensive_result > 0)
                        kpiRes5 = [1 - (Number(product_expensive_assess) / 100.00 - Number(product_expensive_result) / 100.00) * 5] * Number(product_expensive_standard);
                    else
                        kpiRes5 = 0;
                    kpiRes5 = kpiRes5 < 0 ? 0 : kpiRes5.toFixed(2);

                    $("#product_expensive_result").val(item.kpi_score);
                    $("#product_expensive_KPI").text(kpiRes5);
                } else if (item.kpi_type == 3) {//导购人均产值
                    shopguide_average_result1 = item.guide_amount;
                    shopguide_average_result = item.guide_count;

                    if (item.kpi_score >= Number(shopguide_average_assess))
                        kpiRes3 = Number(shopguide_average_standard);
                    else if (item.kpi_score > 0)
                        kpiRes3 = (Number(shopguide_average_result1) / Number(shopguide_average_result)) / Number(shopguide_average_assess) * Number(shopguide_average_standard)
                    else
                        kpiRes3 = 0;
                    kpiRes3 = kpiRes3 < 0 ? 0 : kpiRes3.toFixed(2);

                    $("#shopguide_average_result1").val(shopguide_average_result1);//销量
                    $("#shopguide_average_result").val(shopguide_average_result);//人数
                    $("#shopguide_average_KPI").text(kpiRes3);

                }
                getSum();
            })
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })

}


//kpi计算
function SnowballNumberSum() {
    //V雪球活跃人数
    snowball_number_result = Number($("#snowball_number_result").val());
    var kpiAdv1 = Number(snowball_number_advice);
    if (snowball_number_result >= snowball_number_assess)
        snowball_number_KPI = kpiAdv1;
    else {
        snowball_number_KPI = kpiAdv1 + (snowball_number_result - snowball_number_assess) * snowball_number_standard;
    }
    snowball_number_KPI = snowball_number_KPI < 0 ? 0 : snowball_number_KPI.toFixed(2);
    $("#snowball_number_KPI").text(snowball_number_KPI);
    getSum();
}
function SnowballRatioSum() {
    //V雪球转化率
    snowball_ratio_result = Number($("#snowball_ratio_result").val());
    if (snowball_ratio_result >= snowball_ratio_assess) {
        snowball_ratio_KPI = snowball_ratio_standard;
    } else {
        snowball_ratio_KPI = [1 - (snowball_ratio_assess / 100.00 - snowball_ratio_result / 100.00) * 5] * snowball_ratio_standard;
    }
    snowball_ratio_KPI = snowball_ratio_KPI < 0 ? 0 : snowball_ratio_KPI.toFixed(2);
    $("#snowball_ratio_KPI").text(snowball_ratio_KPI);
    getSum();
}
//function ShopguideResignSum1() {
//}
function ShopguideResignSum() {
    //导购离职率
    shopguide_resign_result1 = Number($("#shopguide_resign_result1").val());//离职人数
    shopguide_resign_result = Number($("#shopguide_resign_result").val());//月底人数
    if (shopguide_resign_result1 == 0 && shopguide_resign_result == 0) {
        shopguide_resign_KPI = 0;
    } else {// 离职人数 / (月底人数 + 离职人数) - 考核标准
        var resignR = (shopguide_resign_result1 / (shopguide_resign_result + shopguide_resign_result1));
        if (resignR <= (shopguide_resign_assess / 100))
            shopguide_resign_KPI = shopguide_resign_standard;
        else
            shopguide_resign_KPI = [1 - (resignR - (shopguide_resign_assess / 100)) * 5] * Number(shopguide_resign_standard);
        shopguide_resign_KPI = shopguide_resign_KPI < 0 ? 0 : shopguide_resign_KPI.toFixed(2);
    }
    $("#shopguide_resign_KPI").text(shopguide_resign_KPI);
    getSum();
}
function ProductTrainSum() {
    //培训场次
    var kpiAdv6 = Number(product_train_advice);//建议金额
    product_train_result = Number($("#product_train_result").val());//考核结果 -- 实际场次
    if (product_train_result < 0) {
        $.modalAlert("培训场次不能小于0");
        $("#product_train_result").val(0);
        $("#product_train_KPI").text(0);
        return;
    }
    if (product_train_result >= product_train_assess) {
        product_train_KPI = kpiAdv6;
    } else {
        product_train_KPI = kpiAdv6 + (product_train_result - product_train_assess) * product_train_standard;
    }
    product_train_KPI = product_train_KPI < 0 ? 0 : product_train_KPI.toFixed(2);

    $("#product_train_KPI").text(product_train_KPI);
    getSum();
}
function ImageEfficiencySum() {
    //形象执行效率

    image_efficiency_result = Number($("#image_efficiency_result").val());
    if (image_efficiency_result >= 100) {
        $("#image_efficiency_result").val(100);
        image_efficiency_KPI = image_efficiency_standard.toFixed(2);
    }
    else if (image_efficiency_result <= 0) {
        $("#image_efficiency_result").val(0);
        image_efficiency_KPI = 0;
    }
    else
        image_efficiency_KPI = (image_efficiency_result / 100 * image_efficiency_standard).toFixed(2);
    $("#image_efficiency_KPI").text(image_efficiency_KPI);
    getSum();
}
function ImageFineSum() {
    var kpiAdv8 = Number(image_fine_advice);
    //形象罚款
    var image_fine_number = Number($("#image_fine_number").text());
    image_fine_result = Number($("#image_fine_result").val());
    if (image_fine_result <= 0)
        image_fine_KPI = kpiAdv8;
    else
        image_fine_KPI = kpiAdv8 - (image_fine_result * image_fine_standard);
    image_fine_KPI = image_fine_KPI < 0 ? 0 : image_fine_KPI.toFixed(2);
    $("#image_fine_KPI").text(image_fine_KPI);
    getSum();
}
function ManagerScoringSum() {
    //终端经理打分
    manager_scoring_result1 = Number($("#manager_scoring_result1").val());
    manager_scoring_result = Number($("#manager_scoring_result").val());
    if ((manager_scoring_result1 + manager_scoring_result) > 100) {
        $.modalAlert("态度和效率总分不能超过100");
        $("#manager_scoring_result1").val(0);
        $("#manager_scoring_result").val(0);
        $("#manager_scoring_KPI").text(0);
        return;
    }
    if (manager_scoring_result1 == 0 && manager_scoring_result == 0)
        manager_scoring_KPI = 0;
    else
        manager_scoring_KPI = ((manager_scoring_result1 + manager_scoring_result) / 100 * manager_scoring_standard).toFixed(2);
    $("#manager_scoring_KPI").text(manager_scoring_KPI);
    getSum();
}

//累计
function getSum() {
    //导购离职率-高端机占比不现实 为 0 
    if ($(".wrap3").css("display") == "none") {//
        kpiRes3 = 0;
    }
    if ($(".wrap5").css("display") == "none") {//
        kpiRes5 = 0;
    }
    trainer_sum_KPI = Number(snowball_number_KPI) + Number(snowball_ratio_KPI) + Number(shopguide_resign_KPI)
        + Number(product_train_KPI) + Number(image_efficiency_KPI) + Number(image_fine_KPI) + Number(manager_scoring_KPI) + Number(kpiRes3) + Number(kpiRes5);
    trainer_sum_KPI = trainer_sum_KPI.toFixed(2);
    $("#trainer_sum_KPI").text(trainer_sum_KPI);
}
//导购销售详情
function querySubmitGuide() {
    $tableGuideSale.bootstrapTable({ offset: 0 }); // 第一页
    $tableGuideSale.bootstrapTable('removeAll');
    $tableGuideSale.bootstrapTable('refresh');
}
var tableGuideSaleHeader = [
    { field: "name", title: "导购员", align: "center", sortable: true, width: 180 },
    { field: "phone_sn", title: "串码", align: "center", sortable: true, width: 150, },
    { field: "model", title: "型号", align: "center", sortable: true, width: 150 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 100 },
    //{
    //    field: "sale_type", title: "销售状态", align: "center", sortable: true, width: 100,
    //    formatter: (function (value) {
    //        if (value == 0) {
    //            return "已出库";
    //        } else if (value == 1) {
    //            return "正常销售";
    //        } else if (value == 2) {
    //            return "买断";
    //        } else if (value == 3) {
    //            return "包销";
    //        }
    //    })
    //},
    //{ field: "outlay", title: "提成", align: "center", sortable: true, width: 80 },
    //{
    //    field: "outlay_type", title: "类型", align: "center", sortable: true, width: 80, formatter: (function (value) {
    //        if (value == 2) return "下货";
    //        return "实销";
    //    })
    //},
    //{
    //    field: "special_offer", title: "特价机", align: "center", sortable: true, width: 80,
    //    formatter: (function (value) {
    //        if (value == 1) return "是";
    //        return "否";
    //    })
    //},
    {
        field: "price_wholesale", title: "批发价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_sale", title: "实销价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "sale_time", title: "时间", align: "center", sortable: true, width: 100,
        formatter: function (value, row) {
            var date = new Date(value);
            return date.pattern("yyyy-MM-dd");
        }
    },
]
function initTable1() {
    var oTable = new TableInit1();
    oTable.Init();
}
var TableInit1 = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $tableGuideSale.bootstrapTable({
            height: 400,
            url: "/FinancialAccounting/TrainerKPI/GetGuideSaleList?date=" + new Date(),        //请求后台的URL（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageSize: 20,                       //每页的记录行数（*）
            pageList: [20, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            strictSearch: true,
            showColumns: false,                  //是否显示所有的列 
            showRefresh: false,                  //是否显示刷新按钮
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableGuideSaleHeader,
            queryParams: function (params) {
                var param = {
                    page: (params.offset / params.limit) + 1,
                    rows: params.limit,
                    sidx: params.sort,
                    sord: params.order,
                    emp_id: id,
                    month: $('#month').val(),
                    queryName: $('#guide_name').val().trim(),
                    phone_sn: $('#phone_sn1').val().trim(),
                    model: $('#model1').val().trim(),

                };
                return param;
            },
            onLoadError: function (data) {
                console.log(data);
            }
        });
    };
    return oTableInit;
};

//高端机销售详情
function querySubmitProduct() {
    $tableHighLevelSale.bootstrapTable({ offset: 0 }); // 第一页
    $tableHighLevelSale.bootstrapTable('removeAll');
    $tableHighLevelSale.bootstrapTable('refresh');
}
var tableHighLevelSaleHeader = [    
    { field: "phone_sn", title: "串码", align: "center", sortable: true, width: 150, },
    { field: "model", title: "型号", align: "center", sortable: true, width: 150 },
    { field: "color", title: "颜色", align: "center", sortable: true, width: 100 },
    //{
    //    field: "sale_type", title: "销售状态", align: "center", sortable: true, width: 100,
    //    formatter: (function (value) {
    //        if (value == 0) {
    //            return "已出库";
    //        } else if (value == 1) {
    //            return "正常销售";
    //        } else if (value == 2) {
    //            return "买断";
    //        } else if (value == 3) {
    //            return "包销";
    //        }
    //    })
    //},    
    {
        field: "high_level", title: "高端机", align: "center", sortable: true, width: 80,
        formatter: (function (value) {
            if (value == 1) return "是";
            return "否";
        })
    },
    {
        field: "price_wholesale", title: "批发价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "price_sale", title: "实销价", align: "center", sortable: true, width: 100,
        formatter: (function (value) { return ToThousandsStr(value); })
    },
    {
        field: "sale_time", title: "时间", align: "center", sortable: true, width: 100,
        formatter: function (value, row) {
            var date = new Date(value);
            return date.pattern("yyyy-MM-dd");
        }
    },
]
function initTable2() {
    var oTable = new TableInit2();
    oTable.Init();
}
var TableInit2 = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $tableHighLevelSale.bootstrapTable({
            height: 400,
            url: "/FinancialAccounting/TrainerKPI/GetHighLevelSaleList?date=" + new Date(),        //请求后台的URL（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageSize: 20,                       //每页的记录行数（*）
            pageList: [20, 50, 100, 300, 500],        //可供选择的每页的行数（*）
            strictSearch: true,
            showColumns: false,                  //是否显示所有的列 
            showRefresh: false,                  //是否显示刷新按钮
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableHighLevelSaleHeader,
            queryParams: function (params) {
                var param = {
                    page: (params.offset / params.limit) + 1,
                    rows: params.limit,
                    sidx: params.sort,
                    sord: params.order,
                    emp_id: id,
                    month: $('#month').val(),
                    high_level: $('#high_level').val(),
                    phone_sn: $('#phone_sn2').val().trim(),
                    model: $('#model2').val().trim(),

                };
                return param;
            },
            onLoadError: function (data) {
                console.log(data);
            }
        });
    };
    return oTableInit;
};


function submitForm() {

    // 提交验证
    if (!$("#form1").formValid())
        return false;

    var data = $("#form1").formSerialize();

    if ($("#month").val() == '' && $("#month").prop("checked") == false) {
        $.modalAlert("请选择考核月份");
        return;
    }

    if (manager_scoring_result1 > 100 || manager_scoring_result > 100 || image_efficiency_result > 100) {
        $.modalAlert("分值不能超过100！");
        return;
    }



    var detailList = [];
    if (position_type == 12) {
        var empKPILen = empKPIList.length;
        for (var i = 0; i <= empKPILen - 1; i++) {
            if (!empKPIList[i].is_included) {
                continue;
            }
            if (empKPIList[i].kpi_type == 1) {
                var wrap1 = {};
                wrap1.kpi_type = 1;
                wrap1.kpi_score = snowball_number_result;
                wrap1.kpi_result = snowball_number_KPI;
                if ($("#snowball_number_result").val() == "") {
                    $.modalAlert("请填写V雪球活跃人数！");
                    return false;
                }
                detailList.push(wrap1);
            }
            else if (empKPIList[i].kpi_type == 2) {
                var wrap2 = {};
                wrap2.kpi_type = 2;
                wrap2.kpi_score = snowball_ratio_result;
                wrap2.kpi_result = snowball_ratio_KPI;
                if ($("#snowball_ratio_result").val() == "") {
                    $.modalAlert("请填写V雪球转化率！");
                    return false;
                }
                detailList.push(wrap2);

            }
            else if (empKPIList[i].kpi_type == 3) {
                var wrap3 = {};
                wrap3.kpi_type = 3;
                wrap3.guide_amount = shopguide_average_result1;
                wrap3.guide_count = shopguide_average_result;
                wrap3.kpi_result = kpiRes3;
                detailList.push(wrap3);

            }
            else if (empKPIList[i].kpi_type == 4) {
                var wrap4 = {};
                wrap4.kpi_type = 4;
                wrap4.resign_emp_count = shopguide_resign_result1;
                wrap4.emp_count = shopguide_resign_result;
                wrap4.kpi_result = shopguide_resign_KPI;
                if ($("#shopguide_resign_result1").val() == "" || $("#shopguide_resign_result").val() == "") {
                    $.modalAlert("请填写导购离职率！");
                    return false;
                }
                detailList.push(wrap4);

            }
            else if (empKPIList[i].kpi_type == 5) {
                var wrap5 = {};
                wrap5.kpi_type = 5;
                wrap5.kpi_score = product_expensive_result;
                wrap5.kpi_result = kpiRes5;
                detailList.push(wrap5);

            }
            else if (empKPIList[i].kpi_type == 6) {
                var wrap6 = {};
                wrap6.kpi_type = 6;
                wrap6.kpi_score = product_train_result;
                wrap6.kpi_result = product_train_KPI;
                if ($("#product_train_result").val() == "") {
                    $.modalAlert("请填写培训场次！");
                    return false;
                }
                detailList.push(wrap6);

            }
            else if (empKPIList[i].kpi_type == 7) {
                var wrap7 = {};
                wrap7.kpi_type = 7;
                wrap7.kpi_score = image_efficiency_result;
                wrap7.kpi_result = image_efficiency_KPI;
                if ($("#image_efficiency_result").val() == "") {
                    $.modalAlert("请填写形象执行效率！");
                    return false;
                }
                detailList.push(wrap7);
            }
            else if (empKPIList[i].kpi_type == 8) {
                var wrap8 = {};
                wrap8.kpi_type = 8;
                wrap8.kpi_score = image_fine_result;
                wrap8.kpi_result = image_fine_KPI;
                if ($("#image_fine_result").val() == "") {
                    $.modalAlert("请填写形象罚款！");
                    return false;
                }
                detailList.push(wrap8);
            }
        }
        var wrap9 = {};
        wrap9.kpi_type = 9;
        wrap9.emp_count = manager_scoring_result1;
        wrap9.resign_emp_count = manager_scoring_result;
        wrap9.kpi_result = manager_scoring_KPI;
        if ($("#manager_scoring_result1").val() == "" || $("#manager_scoring_result").val() == "") {
            $.modalAlert("请填写终端经理打分！");
            return false;
        }
        detailList.push(wrap9);
    } else if (position_type == 11) {
        var empKPILen = empKPIList.length;
        for (var i = 0; i <= empKPILen - 1; i++) {
            if (!empKPIList[i].is_included) {
                continue;
            }
            if (empKPIList[i].kpi_type == 51) {
                var wrap51 = {};
                wrap51.kpi_type = 51;
                wrap51.guide_amount = shopguide_average_result1;
                wrap51.guide_count = shopguide_average_result;
                wrap51.kpi_result = kpiRes3;
                detailList.push(wrap51);
            }
            if (empKPIList[i].kpi_type == 52) {
                var wrap52 = {};
                wrap52.kpi_type = 52;
                wrap52.resign_emp_count = shopguide_resign_result1;
                wrap52.emp_count = shopguide_resign_result;
                wrap52.kpi_result = shopguide_resign_KPI;
                if ($("#shopguide_resign_result1").val() == "" || $("#shopguide_resign_result").val() == "") {
                    $.modalAlert("请填写导购离职率！");
                    return false;
                }
                detailList.push(wrap52);

            }
            if (empKPIList[i].kpi_type == 53) {
                var wrap53 = {};
                wrap53.kpi_type = 53;
                wrap53.kpi_score = product_expensive_result;
                wrap53.kpi_result = kpiRes5;
                detailList.push(wrap53);
            }
        }
    }
    data['detailList'] = detailList;
    data['kpi_total'] = $("#trainer_sum_KPI").text();
    data["emp_id"] = id;
    $.submitForm({
        url: "/FinancialAccounting/trainerKPI/Add?date=" + new Date(),
        param: data,
        success: function (data) {
            if (data.state == "success") {
                window.location.href = "/FinancialAccounting/trainerKPI/Index?id=" + id;

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
}