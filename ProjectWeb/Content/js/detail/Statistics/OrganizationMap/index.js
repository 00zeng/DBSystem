var orgTree;
var myChart1 = echarts.init(document.getElementById('chart1'));
$(function () {
    myChart1.showLoading();
    $.ajax({
        url: "/OrganizationManage/Company/GetOrgTree",
        async: false,
        type: "get",        
        dataType: "json",
        success: function (data) {
            orgTree = data;
            myChart1.setOption(option = {
                title: {
                    text: '道本组织架构图',
                    x: 'left'
                },
                tooltip: {
                    trigger: 'item',
                    triggerOn: 'mousemove'
                },
                //legend: {
                //    top: '10%',
                //    left: '3%',
                //    orient: 'vertical',
                //    //data: [{
                //    //    name: '组织架构',
                //    //    icon: 'circle',                       
                //    //},
                //    //{
                //    //    name: '部门',
                //    //    icon: 'triangle'
                //    //}
                //    //],
                //    color: '#3c8dbc',
                //    borderColor: '#c23531'
                //},
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
                        name: '',
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
                        initialTreeDepth: 4,         //默认：2，树图初始展开的层级（深度）。根节点是第 0 层，然后是第 1 层、第 2 层...
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

            myChart1.hideLoading();
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    })
})


window.onresize = function () {
    myChart1.resize();

}
