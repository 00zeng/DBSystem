var orgTree;

var myChart1 = echarts.init(document.getElementById('chart1'));
var myChart2 = echarts.init(document.getElementById('chart2'));
//var myChart3 = echarts.init(document.getElementById('chart3'));
//var myChart4 = echarts.init(document.getElementById('chart4'));
//var myChart5 = echarts.init(document.getElementById('chart5'));
var myChart6 = echarts.init(document.getElementById('chart6'));

var data1 = {
    "name": "flare",
    "children": [
        {
            "name": "data",
            "children": [
                {
                    "name": "converters",
                    "children": [
                     { "name": "Converters", "value": 593 },
                     { "name": "DelimitedTextConverter", "value": 593 }
                    ]
                },
                {
                    "name": "DataUtil", "value": 593
                }
            ]
        },
        {
            "name": "display",
            "children": [
                { "name": "DirtySprite", "value": 593 },
                { "name": "LineSprite", "value": 593 },
                { "name": "RectSprite", "value": 593 }
            ]
        },
        {
            "name": "flex",
            "children": [
                { "name": "FlareVis", "value": 593 }
            ]
        },
        {
            "name": "query",
            "children": [
             { "name": "AggregateExpression", "value": 593 },
             { "name": "And", "value": 593 },
             { "name": "Arithmetic", "value": 593 },
             { "name": "Match", "value": 593 },
             { "name": "Maximum", "value": 593 },
             {
                 "name": "methods",
                 "children": [
                  { "name": "add", "value": 593 },
                  { "name": "and", "value": 330 },
                  { "name": "average", "value": 287 },
                  { "name": "count", "value": 277 },
                  { "name": "distinct", "value": 292 },
                 ]
             },
             { "name": "Minimum", "value": 843 },
             { "name": "Not", "value": 1554 },
             { "name": "Or", "value": 970 },
             { "name": "Query", "value": 13896 },
             { "name": "Range", "value": 1594 },
            ]
        },
        {
            "name": "scale",
            "children": [
             { "name": "IScaleMap", "value": 2105 },
             { "name": "LinearScale", "value": 1316 },
             { "name": "LogScale", "value": 3151 },
             { "name": "OrdinalScale", "value": 3770 },
             { "name": "QuantileScale", "value": 2435 },
            ]
        }
    ]
};
var data2 = {
    "name": "flare",
    "children": [
        {
            "name": "flex",
            "children": [
                {"name": "FlareVis", "value": 4116}
            ]
        },
        {
            "name": "scale",
            "children": [
                {"name": "IScaleMap", "value": 2105},
                {"name": "LinearScale", "value": 1316},
                {"name": "LogScale", "value": 3151},
                {"name": "OrdinalScale", "value": 3770},
                {"name": "QuantileScale", "value": 2435},
                {"name": "QuantitativeScale", "value": 4839},
                {"name": "RootScale", "value": 1756},
                {"name": "Scale", "value": 4268},
                {"name": "ScaleType", "value": 1821},
                {"name": "TimeScale", "value": 5833}
            ]
        },
        {
            "name": "display",
            "children": [
                {"name": "DirtySprite", "value": 8833}
            ]
        }
    ]
};
$(function () {
    myChart6.showLoading();

    $.ajax({
        url: "/OrganizationManage/Company/GetOrgTree",
        async: false,
        type: "get",        
        dataType: "json",
        success: function (data) {
            orgTree = data;
            myChart6.setOption(option = {
                title: {
                    text: '组织架构图',
                    x: 'left'
                },
                tooltip: {
                    trigger: 'item',
                    triggerOn: 'mousemove'
                },
                legend: {
                    top: '10%',
                    left: '3%',
                    orient: 'vertical',
                    data: [{
                        name: '组织架构',
                        icon: 'circle',
                        
                    },
                    //{
                    //    name: '部门',
                    //    icon: 'triangle'
                    //}
                    ],
                    color: '#3c8dbc',
                    borderColor: '#c23531'
                },
                toolbox: {
                    show: true,
                    orient: 'horizontal',
                    x: 'right',
                    feature: {                        
                        saveAsImage: { show: true }
                    }
                },
                series: [
                    {
                        type: 'tree',
                        name: '组织架构',
                        data: [orgTree],
                        orient: 'horizontal',  // vertical horizontal

                        top: '5%',       
                        left: '5%',
                        bottom: '10%',
                        right: '25%',                              

                        symbol: 'emptyCircle',
                        symbolSize: 7,                       
                        //roam: true,     //可放大缩小

                        label: {
                            normal: {
                                position: 'left',
                                verticalAlign: 'middle',
                                align: 'right',                               
                            }
                        },

                        leaves: {
                            label: {
                                normal: {
                                    position: 'right',
                                    verticalAlign: 'middle',
                                    align: 'left'
                                }
                            }
                        },
                        initialTreeDepth: 5,         //默认：2，树图初始展开的层级（深度）。根节点是第 0 层，然后是第 1 层、第 2 层，
                        expandAndCollapse: true,     //子树折叠和展开的交互，默认打开
                        animationDuration: 550,
                        animationDurationUpdate: 750
                    },
                    //{
                    //     type: 'tree',
                    //     name: '部门',
                    //     data: [data2],
                    //     orient: 'vertical',
                    //     top: '70%',
                    //     left: '15%',
                    //     bottom: '10%',
                    //     right: '15%',
                    //     symbol: 'emptyTriangle',
                    //     symbolSize: 7,
                    //     label: {
                    //         normal: {
                    //             position: 'left',
                    //             verticalAlign: 'middle',
                    //             align: 'right'
                    //         }
                    //     },
                    //     leaves: {
                    //         label: {
                    //             normal: {
                    //                 position: 'right',
                    //                 verticalAlign: 'middle',
                    //                 align: 'left'
                    //             }
                    //         }
                    //     },
                    // }
                ]
            });

            myChart6.hideLoading();
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
})



var option1 = {
    title: {
        text: '各分公司机型销量占比',
        x: 'center'
    },
    tooltip: {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c}台 ({d}%)"
    },
    legend: {
        orient: 'vertical',
        x: 'left',
        data: ['机型A', '机型B', '机型C', '机型D', '机型E']
    },
    calculable: true,
    series: [
        {
            name: '销量',
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
var option2 = {

    tooltip: {
        trigger: 'axis',
        axisPointer: {
            type: 'shadow'
        }
    },
    legend: {
        //orient: 'vertical',
        //x: 'left',
        data: ['机型A', '机型B', '机型C', '机型D', '机型E']
    },
    toolbox: {
        show: true,
        orient: 'vertical',
        y: 'center',
        feature: {
            mark: { show: true },
            magicType: { show: true, type: ['line', 'bar', 'stack', 'tiled'] },
            restore: { show: true },
            saveAsImage: { show: true }
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
            splitArea: { show: true }
        }
    ],
    grid: {
        x2: 40
    },
    series: [
        {
            name: '机型A',
            type: 'bar',
            stack: '总量',
            data: [320, 332, 301, 334, 390, 330, 320]
        },
        {
            name: '机型B',
            type: 'bar',
            stack: '总量',
            data: [120, 132, 101, 134, 90, 230, 210]
        },
        {
            name: '机型C',
            type: 'bar',
            stack: '总量',
            data: [220, 182, 191, 234, 290, 330, 310]
        },
        {
            name: '机型D',
            type: 'bar',
            stack: '总量',
            data: [150, 232, 201, 154, 190, 330, 410]
        },
        {
            name: '机型E',
            type: 'bar',
            stack: '总量',
            data: [820, 932, 901, 934, 1290, 1330, 1320]
        }
    ]
};

myChart1.setOption(option1);
myChart2.setOption(option2);


var option3 = {
    title: {
        text: '分公司1 vs 分公司2',
        subtext: '比赛数据'
    },
    tooltip: {
        trigger: 'axis'
    },
    legend: {
        data: [
            'ECharts1 - 2k数据', 'ECharts1 - 2w数据', 'ECharts1 - 20w数据', '',
            'ECharts2 - 2k数据', 'ECharts2 - 2w数据', 'ECharts2 - 20w数据'
        ]
    },
    toolbox: {
        show: true,
        feature: {
            mark: { show: true },
            dataView: { show: true, readOnly: false },
            magicType: { show: true, type: ['line', 'bar'] },
            restore: { show: true },
            saveAsImage: { show: true }
        }
    },
    calculable: true,
    grid: { y: 70, y2: 30, x2: 20 },
    xAxis: [
        {
            type: 'category',
            data: ['Line', 'Bar', 'Scatter', 'K', 'Map']
        },
        {
            type: 'category',
            axisLine: { show: false },
            axisTick: { show: false },
            axisLabel: { show: false },
            splitArea: { show: false },
            splitLine: { show: false },
            data: ['Line', 'Bar', 'Scatter', 'K', 'Map']
        }
    ],
    yAxis: [
        {
            type: 'value',
            axisLabel: { formatter: '{value} ms' }
        }
    ],
    series: [
        {
            name: 'ECharts2 - 2k数据',
            type: 'bar',
            itemStyle: { normal: { color: 'rgba(193,35,43,1)', label: { show: true } } },
            data: [40, 155, 95, 75, 0]
        },
        {
            name: 'ECharts2 - 2w数据',
            type: 'bar',
            itemStyle: { normal: { color: 'rgba(181,195,52,1)', label: { show: true, textStyle: { color: '#27727B' } } } },
            data: [100, 200, 105, 100, 156]
        },
        {
            name: 'ECharts2 - 20w数据',
            type: 'bar',
            itemStyle: { normal: { color: 'rgba(252,206,16,1)', label: { show: true, textStyle: { color: '#E87C25' } } } },
            data: [906, 911, 908, 778, 0]
        },
        {
            name: 'ECharts1 - 2k数据',
            type: 'bar',
            xAxisIndex: 1,
            itemStyle: { normal: { color: 'rgba(193,35,43,0.5)', label: { show: true, formatter: function (p) { return p.value > 0 ? (p.value + '\n') : ''; } } } },
            data: [96, 224, 164, 124, 0]
        },
        {
            name: 'ECharts1 - 2w数据',
            type: 'bar',
            xAxisIndex: 1,
            itemStyle: { normal: { color: 'rgba(181,195,52,0.5)', label: { show: true } } },
            data: [491, 2035, 389, 955, 347]
        },
        {
            name: 'ECharts1 - 20w数据',
            type: 'bar',
            xAxisIndex: 1,
            itemStyle: { normal: { color: 'rgba(252,206,16,0.5)', label: { show: true, formatter: function (p) { return p.value > 0 ? (p.value + '+') : ''; } } } },
            data: [3000, 3000, 2817, 3000, 0]
        }
    ]
};
//myChart3.setOption(option3);

//var zrColor = zrender.tool.color;        //标签式文件引入时,拥有echarts和zrender两个命名空间
//var colorList = [
//  '#ff7f50', '#87cefa', '#da70d6', '#32cd32', '#6495ed',
//  '#ff69b4', '#ba55d3', '#cd5c5c', '#ffa500', '#40e0d0'
//];
//var itemStyle = {
//    normal: {
//        color: function (params) {
//            if (params.dataIndex < 0) {
//                // for legend
//                return zrColor.lift(
//                  colorList[colorList.length - 1], params.seriesIndex * 0.1
//                );
//            }
//            else {
//                // for bar
//                return zrColor.lift(
//                  colorList[params.dataIndex], params.seriesIndex * 0.1
//                );
//            }
//        }
//    }
//};
//var option4 = {
//    title: {
//        text: '2015-2018年各区域手机销量对比（台）',
//        subtext: '数据来自XXX',
//    },
//    tooltip: {
//        trigger: 'axis',
//        backgroundColor: 'rgba(255,255,255,0.7)',
//        axisPointer: {
//            type: 'shadow'
//        },
//        formatter: function (params) {
//            // for text color
//            var color = colorList[params[0].dataIndex];
//            var res = '<div style="color:' + color + '">';
//            res += '<strong>' + params[0].name + '销量（万台）</strong>'
//            for (var i = 0, l = params.length; i < l; i++) {
//                res += '<br/>' + params[i].seriesName + ' : ' + params[i].value
//            }
//            res += '</div>';
//            return res;
//        }
//    },
//    legend: {
//        x: 'right',
//        data: ['2015', '2016', '2017', '2018']
//    },
//    toolbox: {
//        show: true,
//        orient: 'vertical',
//        y: 'center',
//        feature: {
//            mark: { show: true },
//            dataView: { show: true, readOnly: false },
//            restore: { show: true },
//            saveAsImage: { show: true }
//        }
//    },
//    calculable: true,
//    grid: {
//        y: 80,
//        y2: 40,
//        x2: 40
//    },
//    xAxis: [
//        {
//            type: 'category',
//            data: ['区域1', '区域2', '区域3', '区域4', '区域5', '区域6', '区域7', '区域8']
//        }
//    ],
//    yAxis: [
//        {
//            type: 'value'
//        }
//    ],
//    series: [
//        {
//            name: '2015',
//            type: 'bar',
//            itemStyle: itemStyle,
//            data: [4804.7, 1444.3, 1332.1, 908, 871.8, 1983.7, 1627.6, 499.2]
//        },
//        {
//            name: '2016',
//            type: 'bar',
//            itemStyle: itemStyle,
//            data: [5506.3, 1674.7, 1405, 1023.2, 969, 2149.7, 1851.7, 581.3]
//        },
//        {
//            name: '2017',
//            type: 'bar',
//            itemStyle: itemStyle,
//            data: [6040.9, 1823.4, 1484.3, 1116.1, 1063.7, 2455.5, 2033.5, 657.1]
//        },
//        {
//            name: '2018',
//            type: 'bar',
//            itemStyle: itemStyle,
//            data: [6311.9, 1902, 1745.1, 1215.1, 1118.3, 2736.9, 2294, 699.4]
//        }
//    ]
//};
//myChart4.setOption(option4);


var option5 = {
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
            saveAsImage: { show: true }
        }
    },
    calculable: true,
    legend: {
        data: ['导购员全年人均产值', '业务员全年人均产值', '人均年产值']
    },
    xAxis: [
        {
            type: 'category',
            data: ['区域1', '区域2', '区域3', '区域4', '区域5', '区域6', '区域7', '区域8', '区域9', '区域10', '区域11', '区域12']
        }
    ],
    yAxis: [
        {
            type: 'value',
            name: '销售额',
            axisLabel: {
                formatter: '{value} 万元'
            }
        },
        //{
        //    type: 'value',
        //    name: '温度',
        //    axisLabel: {
        //        formatter: '{value} °C'
        //    }
        //}
    ],
    series: [

        {
            name: '导购员全年人均产值',
            type: 'bar',
            data: [20, 19, 17, 23.2, 25.6, 26.7, 35.6, 22.2, 32.6, 20.0, 26.4, 33.3]
        },
        {
            name: '业务员全年人均产值',
            type: 'bar',
            data: [20, 19, 27, 13.2, 25.6, 16.7, 35.6, 22.2, 32.6, 30.0, 36.4, 33.3]
        },
        {
            name: '人均年产值',
            type: 'line',
            //yAxisIndex: 1,
            data: [20, 19, 22, 18.2, 25.6, 21.7, 35.6, 22.2, 32.6, 25.0, 31.4, 33.3]
        }
    ]
};
//myChart5.setOption(option5);

window.onresize = function () {
    myChart1.resize();
    myChart2.resize();
    myChart3.resize();
    myChart4.resize();
    myChart5.resize();
    myChart6.resize();

}
