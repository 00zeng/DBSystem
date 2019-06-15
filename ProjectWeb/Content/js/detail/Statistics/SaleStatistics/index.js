//初始值
var endDate = new Date();
var xLen = endDate.getDate();   //dateInterval(endDate, startDate);
var xIntervalStr = "d";
var xFormatStr = "MM-dd";
var startDate = endDate.dateAdd(xIntervalStr, 1 - xLen);

var companyL1Id = 0, companyL2Id = 0, areaL1Id = 0, areaL2Id = 0, dateType = 0;
var distributorId = "";
var queryParam = {};

var sale_chart = echarts.init($('#sale-chart')[0]);
var buyout_chart = echarts.init($('#buyout-chart')[0]);

var saleItemStyle = { normal: { color: '#3c8dbc', barBorderColor: '#3c8dbc', barBorderWidth: 2, barBorderRadius: 0, } };
var buyoutItemStyle = { normal: { color: '#a2d3ef', barBorderColor: '#3c8dbc', barBorderWidth: 2, barBorderRadius: 0, } };

$(function () {
    $('#start_date').val(startDate.pattern("yyyy-MM-dd"));
    $('#end_date').val(endDate.pattern("yyyy-MM-dd"));
    ActionBinding();
    BindCompany();
    //ChartsInit();

    queryParam = {
        companyL1Id: 0, companyL2Id: companyL2Id,
        areaL1Id: areaL1Id, areaL2Id: areaL2Id, distributorId: distributorId,
        startDate: $('#start_date').val(), endDate: $('#end_date').val(), dateType: dateType
    };
    LoadStatistics();
})

function ActionBinding() {
    $("#company_l2_id").on("change", function () {
        var curL2Company = $("#company_l2_id").val();
        $("#area_l1_id").empty();
        $("#area_l1_id").append($("<option></option>").val(0).html("--所有经理片区--"));
        if (curL2Company > 0) {
            $("#area_l1_id").bindSelect({
                text: "name",
                url: "/OrganizationManage/Area/GetManagerAreaIdNameList",
                param: { company_id: curL2Company },
                search: true,
            });
        }
        else 
            $("#area_l1_id").val(0).trigger("change");
    });

    $("#area_l1_id").on("change", function () {
        var areaL1Id = $("#area_l1_id").val();
        $("#area_l2_id").empty();
        $("#area_l2_id").append($("<option></option>").val(0).html("--所有业务片区--"));
        if (areaL1Id > 0) {
            $("#area_l2_id").bindSelect({
                text: "name",
                url: "/OrganizationManage/Area/GetSalesAreaIdNameList",
                param: { id: areaL1Id },
                search: true,
            });
        }
        else
            $("#area_l2_id").val(0).trigger("change");
    });

    $("#area_l2_id").on("change", function () {
        $("#distributor_id").empty();
        $("#distributor_id").append($("<option></option>").val(0).html("--所有经销商--"));
        $("#distributor_id").bindSelect({
            text: "name",
            url: "/DistributorManage/DistributorManage/GetIdNameList",
            param: { area_l2_id: $("#area_l2_id").val(), },
            search: true,
        });
    })

    $("#querySubmit").on('click', QuerySubmit);

    $("#date_type").change(function () {
        switch (this.value) {
            case "0":
                $("#start_date").val('');
                $("#end_date").val('');
                document.getElementById('start_date').onfocus = function () {
                    WdatePicker({readOnly:true, maxDate: '#F{$dp.$D(\'end_date\')|| \'%y-%M-%d\'}', minDate: '#F{$dp.$D(\'end_date\',{M:-3})}' });
                }
                document.getElementById('end_date').onfocus = function () {
                    WdatePicker({readOnly:true, maxDate: '#F{$dp.$D(\'start_date\',{M:3}) && \'%y-%M-%d\' }', minDate: '#F{$dp.$D(\'start_date\')}' });
                }
                xIntervalStr = "d";
                xFormatStr = "MM-dd";
                break;
            case "1":
                $("#start_date").val('');
                $("#end_date").val('');
                document.getElementById('start_date').onfocus = function () {
                    WdatePicker({ readOnly: true, dateFmt: 'yyyy-MM', maxDate: '#F{$dp.$D(\'end_date\')||  \'%y-%M\'}' });
                }
                document.getElementById('end_date').onfocus = function () {
                    WdatePicker({readOnly:true, dateFmt: 'yyyy-MM', minDate: '#F{$dp.$D(\'start_date\')}', maxDate: '%y-%M' });
                }
                xIntervalStr = "m";
                xFormatStr = "yyyy-MM";
                break;
            case "2":
                $("#start_date").val('');
                $("#end_date").val('');
                document.getElementById('start_date').onfocus = function () {
                    WdatePicker({readOnly:true, dateFmt: 'yyyy-QM', disabledDates: ['....-0[5-9]-..', '....-1[0-2]-..'], maxDate: '#F{$dp.$D(\'end_date\')}' });
                }
                document.getElementById('end_date').onfocus = function () {
                    WdatePicker({readOnly:true, dateFmt: 'yyyy-QM', disabledDates: ['....-0[5-9]-..', '....-1[0-2]-..'], minDate: '#F{$dp.$D(\'start_date\')}' });
                }
                xIntervalStr = "q";
                break;
            case "3":
                $("#start_date").val('');
                $("#end_date").val('');
                document.getElementById('start_date').onfocus = function () {
                    WdatePicker({readOnly:true, dateFmt: 'yyyy', maxDate: '#F{$dp.$D(\'end_date\')||  \'%y\'}', });
                }
                document.getElementById('end_date').onfocus = function () {
                    WdatePicker({readOnly:true, dateFmt: 'yyyy', minDate: '#F{$dp.$D(\'start_date\')}', maxDate: '%y' });
                }
                xIntervalStr = "y";
                xFormatStr = "yyyy";
                break;
        }
    })

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        sale_chart.resize();
        buyout_chart.resize();
    });
    //改变窗口大小时达到自适应效果
    window.onresize = function () {
        sale_chart.resize();
        buyout_chart.resize();
    }
}

function BindCompany() {
    $("#company_l2_id").empty();
    $("#company_l2_id").append($("<option></option>").val(0).html("--所有分公司--"));
    $("#company_l2_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Company/GetIdNameCategoryList",
        param: { category: "分公司" },
        search: true,
    });
}

function ChartsInit() {
    var commonOpt = {
        toolbox: {
            show: true,
            feature: {
                mark: { show: true },
                dataView: { show: true, readOnly: false },
                magicType: { show: true, type: ['line', 'bar'] },
                restore: { show: true },
                //saveAsImage: { show: true }
            }
        },
        calculable: true,
        xAxis: [{ type: 'category', name: '时间' }],
        yAxis: [{ type: 'value' }],    
    }
    var quantityOpt = {
        tooltip: {
            trigger: 'axis',
            formatter: function (params) {
                return params[0].name + '<br/>'
                       + params[0].seriesName + ' : ' + params[0].value + ' 台<br/>'
                       + params[1].seriesName + ' : ' + (params[2].value + params[1].value) + ' 台<br/>'
                       + params[2].seriesName + ' : ' + params[2].value + ' 台<br/>'
                       + params[3].seriesName + ' : ' + params[3].value + ' 台<br/>';
            }
        },
        yAxis: [{ name: '数量（台）' }],
        legend: { data: ['出库', '总实销', '买断', '退库'] }
    };
    var amountOpt = {
        tooltip: {
            trigger: 'axis',
            formatter: function (params) {
                return params[0].name + '<br/>'
                       + params[0].seriesName + ' : ' + params[0].value + ' 元<br/>'
                       + params[1].seriesName + ' : ' + (params[2].value + params[1].value) + ' 元<br/>'
                       + params[2].seriesName + ' : ' + params[2].value + ' 元<br/>'
                       + params[3].seriesName + ' : ' + params[3].value + ' 元';
            }
        },
        yAxis: [{ name: '金额（元）' }],
        legend: { data: ['出库', '总实销', '买断', '补差', '退库'] }
    };
    sale_chart.setOption(commonOpt);
    sale_chart.setOption(quantityOpt);

    buyout_chart.setOption(commonOpt);
    buyout_chart.setOption(amountOpt);
}

function LoadStatistics() {
    var saleX = [];
    var outQuantity = [], outAmount = [];
    var saleQuantity = [], saleAmount = [];
    var buyoutQuantity = [], buyoutAmount = [];
    var woBuyoutQuantity = [], woBuyoutAmount = [];  // with out buyout, 不含买断的实销，用于堆叠显示 
    var refundQuantity = [], refundAmount = [];
    var returnQuantity = [], returnAmount = [];
    $.loading(true);
    $.ajax({
        url: "/Statistics/SaleStatistics/SaleStatistics",
        data: queryParam,
        dataType: 'json',
        success: function (data) {
            if (!!data) {
                var outList = data.outList;
                var oLen = outList.length, oIndex = 0;
                var saleList = data.saleList;
                var sLen = saleList.length, sIndex = 0;
                var buyoutList = data.buyoutList;
                var bLen = buyoutList.length, bIndex = 0;
                var refundList = data.refundList;
                var rdLen = refundList.length, rdIndex = 0;
                var returnList = data.returnList;
                var rnLen = returnList.length, rnIndex = 0;
                var xStr = "";
                var j = 0;
                for (var i = 0; i < xLen; i++) {
                    xStr = endDate.dateAdd(xIntervalStr, 1 + i - xLen).pattern(xFormatStr);
                    saleX.push(xStr);
                    if (oIndex >= oLen || xStr != outList[oIndex].t_interval) { // 出库
                        outQuantity.push(0);
                        outAmount.push(0);
                    } else {
                        outQuantity.push(outList[oIndex].quantity);
                        outAmount.push(outList[oIndex].amount);
                        oIndex++;
                    }
                    if (sIndex >= sLen || xStr != saleList[sIndex].t_interval) {    // 实销
                        saleQuantity.push(0);
                        saleAmount.push(0);
                    } else {
                        saleQuantity.push(saleList[sIndex].quantity);
                        saleAmount.push(saleList[sIndex].amount);
                        sIndex++;
                    }
                    if (bIndex >= bLen || xStr != buyoutList[bIndex].t_interval) {  // 买断
                        buyoutQuantity.push(0);
                        buyoutAmount.push(0);
                    } else {
                        buyoutQuantity.push(buyoutList[bIndex].quantity);
                        buyoutAmount.push(buyoutList[bIndex].amount);
                        bIndex++;
                    }
                    // with out buyout, 不含买断的实销，用于堆叠显示 
                    woBuyoutQuantity.push(saleQuantity[i] - buyoutQuantity[i]);
                    woBuyoutAmount.push(saleAmount[i] - buyoutAmount[i]);

                    if (rdIndex >= rdLen || xStr != refundList[rdIndex].t_interval) {   // 补差
                        refundQuantity.push(0);
                        refundAmount.push(0);
                    } else {
                        refundQuantity.push(refundList[rdIndex].quantity);
                        refundAmount.push(refundList[rdIndex].amount);
                        rdIndex++;
                    }
                    if (rnIndex >= rnLen || xStr != returnList[rnIndex].t_interval) {   // 退库
                        returnQuantity.push(0);
                        returnAmount.push(0);
                    } else {
                        returnQuantity.push(returnList[rnIndex].quantity);
                        returnAmount.push(returnList[rnIndex].amount);
                        rnIndex++;
                    }
                }
            }
            else {
                for (var i = 0; i < xLen; i++) {
                    saleX.push(endDate.dateAdd(xIntervalStr, 1 + i - xLen).pattern(xFormatStr));
                    outQuantity.push(0);
                    outAmount.push(0);
                    saleQuantity.push(0);
                    saleAmount.push(0);
                    buyoutQuantity.push(0);
                    buyoutAmount.push(0);
                    woBuyoutQuantity.push(0);
                    woBuyoutAmount.push(0);
                    refundQuantity.push(0);
                    refundAmount.push(0);
                    returnQuantity.push(0);
                    returnAmount.push(0);
                }
            }
            // 图表显示 
            sale_chart.clear();
            buyout_chart.clear();
            ChartsInit()
            sale_chart.setOption({
                xAxis: [{ data: saleX, axisLabel: { interval: (xLen / 15) | 0 } }],
                series: [{ name: '出库', type: 'bar', data: outQuantity },
                        { name: '总实销', type: 'bar', itemStyle: saleItemStyle, stack: 'buyout', data: woBuyoutQuantity },
                        { name: '买断', type: 'bar', itemStyle: buyoutItemStyle, stack: 'buyout', data: buyoutQuantity },
                        { name: '退库', type: 'bar', data: returnQuantity }, ]
            });
            buyout_chart.setOption({
                xAxis: [{ data: saleX, axisLabel: { interval: (xLen / 15) | 0 } }],
                series: [{ name: '出库', type: 'bar', data: outAmount },
                        { name: '总实销', type: 'bar', itemStyle: saleItemStyle, stack: 'buyout', data: woBuyoutAmount },
                        { name: '买断', type: 'bar', itemStyle: buyoutItemStyle, stack: 'buyout', data: buyoutAmount },
                        { name: '补差', type: 'bar', data: refundAmount },
                        { name: '退库', type: 'bar', data: returnAmount } ]
            });
            $.loading(false);
        },
        error: function () {
            $.modalAlert("系统错误，请稍等重试");
            $.loading(false);

        }
    });
}


function QuerySubmit() {
    var startStr = $("#start_date").val();
    var endStr = $("#end_date").val();
    if (!startStr || startStr == null | startStr == '' || !endStr || endStr == null | endStr == '') {
        $.modalAlert("请选择统计时间", "error");
        return;
    }
    
    endDate = new Date(endStr);
    startDate = new Date(startStr);
    xLen = dateInterval(xIntervalStr, startDate, endDate);
    queryParam = {
        companyL1Id: 0, companyL2Id: $("#company_l2_id").val(),
        areaL1Id: $("#area_l1_id").val(), areaL2Id: $("#area_l2_id").val(), distributorId: $("#distributor_id").val(),
        startDate: startStr, endDate: endStr, dateType: $("#date_type").val()
    };
    
    LoadStatistics();
}