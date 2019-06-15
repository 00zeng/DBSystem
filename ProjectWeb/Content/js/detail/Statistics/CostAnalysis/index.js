
var myChart1 = echarts.init(document.getElementById('chart1'));
var myChart2 = echarts.init(document.getElementById('chart2'));
var myChart3 = echarts.init(document.getElementById('chart3'));

var option1 = {
    title: {
        text: '各型号成本比率',       
        x: 'center'
    },
    tooltip: {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c}元 ({d}%)"
    },
    toolbox: {
        show: true,
        feature: {
            mark: { show: false },
            dataView: { show: true, readOnly: false },
            magicType: { show: false, type: ['line', 'bar', 'stack', 'tiled'] },
            restore: { show: true },
            saveAsImage: { show: true }
        }
    },
    legend: {
        orient: 'vertical',
        x: 'left',
        data: ['机型A', '机型B', '机型C', '机型D', '机型E']
    },
    calculable: true,
    series: [
        {
            name: '成本',
            type: 'pie',
            radius: '50%',
            center: ['50%', 225],
            data: [
                { value: 335, name: '机型A' },
                { value: 310, name: '机型B' },
                { value: 234, name: '机型C' },
                { value: 135, name: '机型D' },
                { value: 1548, name: '机型E' }
            ]
        }
    ]
};
myChart1.setOption(option1);

var option2 = {
    title: {
        text: '各分公司成本比率',
        x: 'center'
    },
    tooltip: {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c}万元 ({d}%)"
    },
    toolbox: {
        show : true,
        feature : {
            mark : {show: false},
            dataView : {show: true, readOnly: false},
            magicType : {show: false, type: ['line', 'bar', 'stack', 'tiled']},
            restore : {show: true},
            saveAsImage : {show: true}
        }
    },
    legend: {
        orient: 'vertical',
        x: 'left',
        data: ['分公司1', '分公司2', '分公司3', '分公司4', '分公司5','分公司6', '分公司7', '分公司8']
    },
    calculable: true,
    series: [
        {
            name: '成本',
            type: 'pie',
            radius: '50%',
            center: ['50%', 225],
            data: [
                { value: 35, name: '分公司1' },
                { value: 50, name: '分公司2' },
                { value: 34, name: '分公司3' },
                { value: 35, name: '分公司4' },
                { value: 28, name: '分公司5' },
                { value: 34, name: '分公司6' },
                { value: 35, name: '分公司7' },
                { value: 48, name: '分公司8' }
            ]
        }
    ]
};
myChart2.setOption(option2);

var option3 = {
    title: {
        text: '各分公司成本对比',
        x: 'left'
    },
    tooltip: {
        trigger: 'axis',
        axisPointer: {
            type: 'shadow'
        }
    },
    legend: {
        //orient: 'vertical',
        //x: 'left',
        data: ['人力成本', '生产成本', '销售成本', '税收', '其他']
    },
    toolbox: {
        show: true,
        orient: 'vertical',
        y: 'center',
        feature: {
            mark: { show: false },
            magicType: { show: true, type: ['line', 'bar', 'stack', 'tiled'] },
            restore: { show: true },
            saveAsImage: { show: true }
        }
    },
    calculable: true,
    xAxis: [
        {
            type: 'category',
            data: ['分公司1', '分公司2', '分公司3', '分公司4', '分公司5', '分公司6', '分公司7', '分公司8']
        }
    ],
    yAxis: [
       {
           type: 'value',
           name: '利润（万元）',
           axisLabel: {
               formatter: '{value}'
           }
       }
    ],
    series: [
        {
            name: '人力成本',
            type: 'bar',
            stack: '总量',
            data: [32, 33, 30, 33, 39, 33, 32,36]
        },
        {
            name: '生产成本',
            type: 'bar',
            stack: '总量',
            data: [10, 12, 11, 14, 9, 2, 8,5]
        },
        {
            name: '销售成本',
            type: 'bar',
            stack: '总量',
            data: [22, 18, 19, 24, 29, 30, 10,20]
        },
        {
            name: '税收',
            type: 'bar',
            stack: '总量',
            data: [10, 22, 21, 14, 10, 30, 41,15]
        },
        {
            name: '其他',
            type: 'bar',
            stack: '总量',
            data: [20, 32, 1, 4, 10, 10, 20,5]
        }
    ]
};
myChart3.setOption(option3);

myChart2.connect(myChart3);
myChart3.connect(myChart2);

window.onresize = function () {
    myChart1.resize();
    myChart2.resize();
    myChart3.resize();
   
}