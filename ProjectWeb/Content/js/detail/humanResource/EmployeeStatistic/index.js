//统计分析

$("#company_id").bindSelect({
    text: "name",
    url: "/OrganizationManage/Company/GetIdNameList",
    search: true,
    firstText: "--所有分公司--",
});

$("#company_id").on("change", function () {
    $("#branchcompany_id").empty();
    $("#area_l1_id").empty();
    $("#area_l1_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Area/GetIdNameList",
        param: { company_id: $("#company_id").val() },
        firstText: "--所有经理片区--",
    });
})

$("#area_l1_id").on("change", function () {
    $("#branchcompany_id").empty();
    $("#area_l2_id").empty();
    $("#area_l2_id").bindSelect({
        text: "name",
        url: "/OrganizationManage/Area/GetSalesAreaIdNameList",
        param: { id: $("#area_l1_id").val() },
        firstText: "--所有业务片区--",
    });
})

$("#area_l2_id").on("change", function () {
    $("#branchcompany_id").empty();
    $("#distributor_id").empty();
    $("#distributor_id").bindSelect({
        text: "name",
        url: "/DistributorManage/DistributorManage/GetIdNameList?area_l2_id=" + area_l2_id,
        param: { id: $("#area_l2_id").val() },
        firstText: "--所有经销商--",
    });
})

var sales_chart = echarts.init(document.getElementById('sales-chart'));
sales_chartOption = {
    tooltip: {
        trigger: 'axis'
    },
    toolbox: {
        show: true,
        feature: {
            mark: { show: true },
            dataView: { show: true, readOnly: false },
            magicType: { show: true, type: ['line', 'bar'] },
            restore: { show: true },
            //saveAsImage: { show: false }
        }
    },
    calculable: true,
    legend: {
        data: ['出库', '实销', '批发价总额']
    },
    xAxis: [
        {
            type: 'category',
            data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
        }
    ],
    yAxis: [
        {
            type: 'value',
            name: '数量',
            axisLabel: {
                formatter: '{value} 台'
            }
        },
        {
            type: 'value',
            name: '金额',
            axisLabel: {
                formatter: '{value} 元'
            }
        }
    ],

    series: [

        {
            name: '出库',
            type: 'bar',
            data: [2.0, 4.9, 7.0, 23.2, 25.6, 76.7, 135.6, 162.2, 32.6, 20.0, 6.4, 3.3]
        },
        {
            name: '实销',
            type: 'bar',
            data: [2.6, 5.9, 9.0, 26.4, 28.7, 70.7, 175.6, 182.2, 48.7, 18.8, 6.0, 2.3]
        },
        {
            name: '批发价总额',
            type: 'line',
            yAxisIndex: 1,
            data: [2.0, 2.2, 3.3, 4.5, 6.3, 10.2, 20.3, 23.4, 23.0, 16.5, 12.0, 6.2]
        }
    ]
};
$.ajax({
    url: "",
    async: false,
    cache: false,
//    success:functon(data){
//    sales_chartoption = {
//    series:[{name:"出库",data:data.data}]
//};
//},
});
sales_chart.setOption(sales_chartOption);




var buyout_chart = echarts.init(document.getElementById('buyout-chart'));
buyout_chartOption = {
    tooltip: {
        trigger: 'axis'
    },
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
    legend: {
        data: ['买断', '批发价总额']
    },
    xAxis: [
        {
            type: 'category',
            data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
        }
    ],
    yAxis: [
        {
            type: 'value',
            name: '数量',
            axisLabel: {
                formatter: '{value} 台'
            }
        },
        {
            type: 'value',
            name: '金额',
            axisLabel: {
                formatter: '{value} 元'
            }
        }
    ],
    series: [

        {
            name: '买断',
            type: 'bar',
            data: [2.0, 4.9, 7.0, 23.2, 25.6, 76.7, 135.6, 162.2, 32.6, 20.0, 6.4, 3.3]
        },
        {
            name: '批发价总额',
            type: 'line',
            yAxisIndex: 1,
            data: [2.0, 2.2, 3.3, 4.5, 6.3, 10.2, 20.3, 23.4, 23.0, 16.5, 12.0, 6.2]
        }
    ]
};
buyout_chart.setOption(buyout_chartOption);

var refund_chart = echarts.init(document.getElementById('refund-chart'));
refund_chartOption = {
    tooltip: {
        trigger: 'axis'
    },
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
    legend: {
        data: ['补差', '批发价总额']
    },
    xAxis: [
        {
            type: 'category',
            data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
        }
    ],
    yAxis: [
        {
            type: 'value',
            name: '数量',
            axisLabel: {
                formatter: '{value} 台'
            }
        },
        {
            type: 'value',
            name: '金额',
            axisLabel: {
                formatter: '{value} 元'
            }
        }
    ],
    series: [

        {
            name: '补差',
            type: 'bar',
            data: [2.0, 4.9, 7.0, 23.2, 25.6, 76.7, 135.6, 162.2, 32.6, 20.0, 6.4, 3.3]
        },
        {
            name: '批发价总额',
            type: 'line',
            yAxisIndex: 1,
            data: [2.0, 2.2, 3.3, 4.5, 6.3, 10.2, 20.3, 23.4, 23.0, 16.5, 12.0, 6.2]
        }
    ]
};
refund_chart.setOption(refund_chartOption);


var return_chart = echarts.init(document.getElementById('return-chart'));
return_chartOption = {
    tooltip: {
        trigger: 'axis'
    },
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
    legend: {
        data: ['退库', '批发价总额']
    },
    xAxis: [
        {
            type: 'category',
            data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
        }
    ],
    yAxis: [
        {
            type: 'value',
            name: '数量',
            axisLabel: {
                formatter: '{value} 台'
            }
        },
        {
            type: 'value',
            name: '金额',
            axisLabel: {
                formatter: '{value} 元'
            }
        }
    ],
    series: [

        {
            name: '退库',
            type: 'bar',
            data: [2.0, 4.9, 7.0, 23.2, 25.6, 76.7, 135.6, 162.2, 32.6, 20.0, 6.4, 3.3]
        },
        {
            name: '批发价总额',
            type: 'line',
            yAxisIndex: 1,
            data: [2.0, 2.2, 3.3, 4.5, 6.3, 10.2, 20.3, 23.4, 23.0, 16.5, 12.0, 6.2]
        }
    ]
};
return_chart.setOption(return_chartOption);


//人员分布搜素按钮
$("#btnSearch").on("click", function () {
    ReSearch();
});
function ReSearch() {
    $gridList.jqGrid('setGridParam', {
        postData: {
            status: $("#status option:selected").val(),
            position_name: $("#position_name").val(),
        }, page: 1
    }).trigger('reloadGrid');
}