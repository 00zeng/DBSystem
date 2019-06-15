var id = $.request("id");
var position_type;
var empKPIList;
$(function () {
    $("#form1").queryFormValidate();
    $.ajax({
        url: "/FinancialAccounting/trainerKPI/GetInfo?date=" + new Date(),
        type: "get",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            
            $("#form1").formSerializeShow(data.empInfo); 
            $("#form1").formSerializeShow(data.mainInfo);
            $("#kpi_total").text(ToThousandsStr(data.mainInfo.kpi_total));

            //   $("#form1").formSerializeShow();
            $("#form1").formSerializeShow(data.positionKPI);

            var company_linkname = decodeURI($.request("company_linkname"));
            $("#company_linkname").text(company_linkname);

            position_type = data.empInfo.position_type;
            empKPIList = data.empKPIList;
          
            if (position_type == 12) {//12-培训师 
                $("#position_type_name").text("培训师");
                $("#image_efficiency_advice").text(ToThousandsStr(data.positionKPI.image_efficiency_advice));
                $("#image_efficiency_standard").text(ToThousandsStr(data.positionKPI.image_efficiency_standard));
                $("#image_fine_advice").text(ToThousandsStr(data.positionKPI.image_fine_advice));
                $("#image_fine_number").text(ToThousandsStr(data.positionKPI.image_fine_number));
                $("#image_fine_standard").text(ToThousandsStr(data.positionKPI.image_fine_standard));
                $("#manager_scoring_advice").text(ToThousandsStr(data.positionKPI.manager_scoring_advice));
                $("#manager_scoring_standard").text(ToThousandsStr(data.positionKPI.manager_scoring_standard));
                $("#product_expensive_advice").text(ToThousandsStr(data.positionKPI.product_expensive_advice));
                $("#product_expensive_assess").text(ToThousandsStr(data.positionKPI.product_expensive_assess));
                $("#product_expensive_standard").text(ToThousandsStr(data.positionKPI.product_expensive_standard));
                $("#product_train_advice").text(ToThousandsStr(data.positionKPI.product_train_advice));
                $("#product_train_assess").text(ToThousandsStr(data.positionKPI.product_train_assess));
                $("#product_train_standard").text(ToThousandsStr(data.positionKPI.product_train_standard));
                $("#shopguide_average_advice").text(ToThousandsStr(data.positionKPI.shopguide_average_advice));
                $("#shopguide_average_assess").text(ToThousandsStr(data.positionKPI.shopguide_average_assess));
                $("#shopguide_average_standard").text(ToThousandsStr(data.positionKPI.shopguide_average_standard));
                $("#shopguide_resign_advice").text(ToThousandsStr(data.positionKPI.shopguide_resign_advice));
                $("#shopguide_resign_assess").text(ToThousandsStr(data.positionKPI.shopguide_resign_assess));
                $("#shopguide_resign_standard").text(ToThousandsStr(data.positionKPI.shopguide_resign_standard));
                $("#snowball_number_advice").text(ToThousandsStr(data.positionKPI.snowball_number_advice));
                $("#snowball_number_assess").text(ToThousandsStr(data.positionKPI.snowball_number_assess));
                $("#snowball_number_standard").text(ToThousandsStr(data.positionKPI.snowball_number_standard));
                $("#snowball_ratio_advice").text(ToThousandsStr(data.positionKPI.snowball_ratio_advice));
                $("#snowball_ratio_assess").text(ToThousandsStr(data.positionKPI.snowball_ratio_assess));
                $("#snowball_ratio_standard").text(ToThousandsStr(data.positionKPI.snowball_ratio_standard));
                $(".wrap9").show();
                $.each(empKPIList, function (index, item) {
                        $(".wrap" + (item.kpi_type)).find("span[name = area_l1_name]").text("(" + item.area_l1_name + ")");
                })
                $.each(data.detailList, function (index, item) {
                    $(".wrap" + (item.kpi_type)).show();
                    if (item.kpi_type == 1) {
                        $("#snowball_number_result").text(ToThousandsStr(item.kpi_score));
                        $("#snowball_number_KPI").text(ToThousandsStr(item.kpi_result));
                    }
                    if (item.kpi_type == 2) {
                        $("#snowball_ratio_result").text(ToThousandsStr(item.kpi_score));
                        $("#snowball_ratio_KPI").text(ToThousandsStr(item.kpi_result));
                    }
                    if (item.kpi_type == 3) {
                        $("#shopguide_average_result1").text(ToThousandsStr(item.guide_amount));
                        $("#shopguide_average_result").text(ToThousandsStr(item.guide_count));
                        $("#shopguide_average_KPI").text(ToThousandsStr(item.kpi_result));
                    }
                    if (item.kpi_type == 4) {
                        $("#shopguide_resign_result1").text(ToThousandsStr(item.resign_emp_count));
                        $("#shopguide_resign_result").text(ToThousandsStr(item.emp_count));
                        $("#shopguide_resign_KPI").text(ToThousandsStr(item.kpi_result));
                    }
                    if (item.kpi_type == 5) {
                        $("#product_expensive_result").text(ToThousandsStr(item.kpi_score));
                        $("#product_expensive_KPI").text(ToThousandsStr(item.kpi_result));
                    }
                    if (item.kpi_type == 6) {
                        $("#product_train_result").text(ToThousandsStr(item.kpi_score));
                        $("#product_train_KPI").text(ToThousandsStr(item.kpi_result));
                    }
                    if (item.kpi_type == 7) {
                        $("#image_efficiency_result").text(ToThousandsStr(item.kpi_score));
                        $("#image_efficiency_KPI").text(ToThousandsStr(item.kpi_result));
                    }
                    if (item.kpi_type == 8) {
                        $("#image_fine_result").text(ToThousandsStr(item.kpi_score));
                        $("#image_fine_KPI").text(ToThousandsStr(item.kpi_result));
                    }
                    if (item.kpi_type == 9) {
                        $("#manager_scoring_result1").text(ToThousandsStr(item.emp_count));
                        $("#manager_scoring_result").text(ToThousandsStr(item.resign_emp_count));
                        $("#manager_scoring_KPI").text(ToThousandsStr(item.kpi_result));
                    }
                })
            }
            else {
                //11 - 培训主管
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

                $.each(empKPIList, function (index, item) {
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
                $.each(data.detailList, function (index, item) {
                    if (item.kpi_type == 51) {
                        $("#shopguide_average_result1").text(ToThousandsStr(item.guide_amount));
                        $("#shopguide_average_result").text(ToThousandsStr(item.guide_count));
                        $("#shopguide_average_KPI").text(ToThousandsStr(item.kpi_result));
                    }
                    if (item.kpi_type == 52) {
                        $("#shopguide_resign_result1").text(ToThousandsStr(item.resign_emp_count));
                        $("#shopguide_resign_result").text(ToThousandsStr(item.emp_count));
                        $("#shopguide_resign_KPI").text(ToThousandsStr(item.kpi_result));
                    }
                    if (item.kpi_type == 53) {
                        $("#product_expensive_result").text(ToThousandsStr(item.kpi_score));
                        $("#product_expensive_KPI").text(ToThousandsStr(item.kpi_result));
                    }
                })
            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
});
