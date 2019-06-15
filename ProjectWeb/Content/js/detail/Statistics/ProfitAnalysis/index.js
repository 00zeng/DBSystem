

var myChart1 = echarts.init(document.getElementById('chart1'));
var myChart2 = echarts.init(document.getElementById('chart2'));
var myChart3 = echarts.init(document.getElementById('chart3'));



var option1 = {
    title: {
        text: '各机型利润/利润率汇总',
        x: 'left'
    },
    tooltip: {
        trigger: 'axis'
    },
    toolbox: {
        show: true,
        feature: {
            mark: { show: false },
            dataView: { show: true, readOnly: true },
            magicType: { show: false, type: ['line', 'bar'] },
            restore: { show: true },
            saveAsImage: { show: true }
        }
    },
    calculable: true,
    legend: {
        data: ['各型号利润', '各型号利润率']
    },
    xAxis: [
        {
            type: 'category',
            data: ['型号1', '型号2', '型号3', '型号4', '型号5', '型号6']
        }
    ],
    yAxis: [
        {
            type: 'value',
            name: '利润',
            axisLabel: {
                formatter: '{value} 万元'
            }
        },
        {
            type: 'value',
            name: '利润率',
            axisLabel: {
                formatter: '{value} %'
            }
        }
    ],
    series: [
        {
            name: '利润',
            type: 'bar',
            barwidth:5,
            data: [10, 19, 17, 23.2, 25.6, 6.7]
        },

        {
            name: '利润率',
            type: 'line',
            yAxisIndex: 1,
            data: [5, 3, 2, 6, 8, 10]
        }
    ]
};
myChart1.setOption(option1);

var option2 = {
    title: {
        text: '各机型利润占比',
        x: 'left'
    },
    tooltip: {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c}万元 ({d}%)"
    },
    toolbox: {
        show: true,
        feature: {
            mark: { show: false },
            dataView: { show: true, readOnly: true },
            restore: { show: true },
            saveAsImage: { show: true }
        }
    },
    legend: {
        //orient: 'vertical',
        //x: 'left',
        data: ['机型1', '机型2', '机型3', '机型4', '机型5', '机型6']
    },
    calculable: true,
    series: [
        {
            name: '利润',
            type: 'pie',
            radius: '50%',
            center: ['50%', 225],
            data: [
                { value: 10, name: '机型1' },
                { value: 19, name: '机型2' },
                { value: 17, name: '机型3' },
                { value: 23.2, name: '机型4' },
                { value: 25.6, name: '机型5' },
                { value: 6.7, name: '机型6' }
           
            ]
        }
    ]
};
myChart2.setOption(option2);


var option3 = {
    title: {
        text: '各分公司各机型利润/利润率统计',
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
        data: ['机型1', '机型2', '机型3', '机型4', '机型5', '机型6']
    },
    toolbox: {
        show: true,
        //orient: 'vertical',
        //y: 'center',
        feature: {
            mark: { show: false },
            magicType: { show: true, type: ['line', 'bar','stack', 'tiled'] },
            restore: { show: true },
            saveAsImage: { show: true },
        }
    },
    calculable: true,
    xAxis: [
        {
            type: 'category',
            data: ['分公司一', '分公司二', '分公司三', '分公司四', '分公司五', '分公司六', '分公司七']
        }
    ],
    yAxis: [
       {
           type: 'value',
           name: '利润',
           axisLabel: {
               formatter: '{value} 万元'
           }
       },
       {
           type: 'value',
           name: '利润率',
           axisLabel: {
               formatter: '{value} %'
           }
       }
    ],
   
    series: [
        {
            name: '机型1',
            type: 'bar',
            //stack: '利润',
            data: [320, 332, 301, 334, 390, 330, 320]
        },
        {
            name: '机型2',
            type: 'bar',
            //stack: '利润',
            data: [120, 132, 101, 134, 90, 230, 210]
        },
        {
            name: '机型3',
            type: 'bar',
            //stack: '利润',
            data: [220, 182, 191, 234, 290, 330, 310]
        },
        {
            name: '机型4',
            type: 'bar',
            //stack: '利润',
            data: [150, 232, 201, 154, 190, 330, 410]
        },
        {
            name: '机型5',
            type: 'bar',
            //stack: '利润',
            data: [820, 932, 901, 934, 1290, 1330, 1320]
        },
        {
            name: '机型6',
            type: 'bar',
            //stack: '利润',
            data: [620, 732, 701, 934, 1090, 1030, 120]
        },
        {
            name: '机型1',
            type: 'line',
            stack: '利润率',
            yAxisIndex: 1,
            data: [1, 2, 1, 5, 6, 4, 2]
        },
        {
            name: '机型2',
            type: 'line',
            stack: '利润率',
            yAxisIndex: 1,
            data: [4, 2, 0, 5, 7, 4, 2]
        },
        {
            name: '机型3',
            type: 'line',
            stack: '利润率',
            yAxisIndex: 1,
            data: [2, 4, 1, 8, -1, 0, 9]
        },
        {
            name: '机型4',
            type: 'line',
            stack: '利润率',
            yAxisIndex: 1,
            data: [-1, 2,3, 5, 6, 4, 2]
        },
        {
            name: '机型5',
            type: 'line',
            stack: '利润率',
            yAxisIndex: 1,
            data: [6, 2, 3, 7, -6, 4, 2]
        },
        {
            name: '机型6',
            type: 'line',
            stack: '利润率',
            yAxisIndex: 1,
            data: [8, 5, -1, 5, 6, 3, 7]
        }
    ]
};
myChart3.setOption(option3);



window.onresize = function () {
    myChart1.resize();
    myChart2.resize();  
    myChart3.resize();
    
}