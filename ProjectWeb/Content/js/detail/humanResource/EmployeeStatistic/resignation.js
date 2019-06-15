//离职分析
var line = echarts.init(document.getElementById('line'));
lineOption = {
    title: {
        text: '不同区域的离职比率',
        x: 'right',
    },
    tooltip: {
        trigger: 'axis',
        axisPointer: {
            type: 'shadow'
        }
    },
    grid: {
        left: '3%',
        right: '4%',
        bottom: '3%',
        containLabel: true
    },
    xAxis: {
        type: 'value',
        boundaryGap: [0, 0.01]
    },
    yAxis: {
        type: 'category',
        data: ['长虎一区', '长虎二区', '长虎三区', '长虎四区', '长虎五区', '运营商部', '长安总部'],
    },
    series: [
        {
            type: 'bar',
            data: [0.02, 0.01, 0.14, 0.07, 0.01, 0.04, 0.00]
        },
    ]
};
line.setOption(lineOption);


var first = echarts.init(document.getElementById('first'));
firstOption = {
    title: {
        text: '不同区域的离职人数占比',
        x: 'right',
    },
    tooltip: {
        trigger: 'item',
        formatter: "{a} <br/>{b}: {c} ({d}%)"
    },
    legend: {
        orient: 'vertical',
        x: 'left',
        data: ['长虎一区', '长虎二区', '长虎三区', '长虎四区', '长虎五区', '运营商部', '长安总部'],
    },
    series: [
        {
            name: '人数占比',
            type: 'pie',
            radius: ['40%', '70%'],
            center: ['50%', '60%'],
            avoidLabelOverlap: false,
            label: {
                normal: {
                    show: false,
                    position: 'center'
                },
                emphasis: {
                    show: true,
                    textStyle: {
                        fontSize: '30',
                        fontWeight: 'bold'
                    }
                }
            },
            labelLine: {
                normal: {
                    show: false
                }
            },
            data: [
                {value: 3, name: '长虎一区'},
                {value: 2, name: '长虎二区'},
                {value: 5, name: '长虎三区'},
                {value: 4, name: '长虎四区'},
                {value: 1, name: '长虎五区'},
                {value: 2, name: '运营商部'},
                {value: 0, name: '长安总部'},
            ]
        }
    ]
};
first.setOption(firstOption);

//离职分析搜素按钮
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