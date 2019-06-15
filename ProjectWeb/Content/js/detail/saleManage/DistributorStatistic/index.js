//统计分析
var sex = echarts.init(document.getElementById('sex'));
sexOption = {
    title : {
        text: '性别分布',
        x:'left',
        textStyle: {
            color:'white'
        }
    },
    tooltip : {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c} ({d}%)"
    },
    series : [
        {
            name: '比例',
            type: 'pie',
            radius : '55%',
            center: ['50%', '50%'],
            data:[
                {value:40, name:'男',itemStyle: {
                        color:'green'
                    }},
                {value:60, name:'女',itemStyle: {
                        color:'yellow'
                    }},
            ],
            itemStyle: {
                emphasis: {
                    shadowBlur: 10,
                    shadowOffsetX: 0,
                    shadowColor: 'rgba(0, 0, 0, 0.5)'
                }
            }
        }
    ]
};
sex.setOption(sexOption);




var old = echarts.init(document.getElementById('old'));
oldOption = {
    title : {
        text: '年龄分布',
        x:'left',
        textStyle: {
            color:'white'
        }
    },
    tooltip : {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c} ({d}%)"
    },
    series : [
        {
            name: '比例',
            type: 'pie',
            radius : '55%',
            center: ['50%', '50%'],
            data:[
                {value:20, name:'人事部',itemStyle: {
                        color:'green'
                    }},
                {value:20, name:'财务部',itemStyle: {
                        color:'yellow'
                    }},
                {value:60, name:'业务部',itemStyle: {
                        color:'pink'
                    }},
            ],
            itemStyle: {
                emphasis: {
                    shadowBlur: 10,
                    shadowOffsetX: 0,
                    shadowColor: 'rgba(0, 0, 0, 0.5)'
                }
            }
        }
    ]
};
old.setOption(oldOption);



var workYear = echarts.init(document.getElementById('workYear'));
workYearOption = {
    title : {
        text: '工龄分布',
        x:'left',
        textStyle: {
            color:'white'
        }
    },
    tooltip : {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c} ({d}%)"
    },
    series : [
        {
            name: '比例',
            type: 'pie',
            radius : '55%',
            center: ['50%', '50%'],
            data:[
                {value:20, name:'人事部',itemStyle: {
                        color:'green'
                    }},
                {value:20, name:'财务部',itemStyle: {
                        color:'yellow'
                    }},
                {value:60, name:'业务部',itemStyle: {
                        color:'pink'
                    }},
            ],
            itemStyle: {
                emphasis: {
                    shadowBlur: 10,
                    shadowOffsetX: 0,
                    shadowColor: 'rgba(0, 0, 0, 0.5)'
                }
            }
        }
    ]
};
workYear.setOption(workYearOption);


var job = echarts.init(document.getElementById('job'));
jobOption = {
    title : {
        text: '职务分布',
        x:'left',
        textStyle: {
            color:'white'
        }
    },
    tooltip : {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c} ({d}%)"
    },
    series : [
        {
            name: '比例',
            type: 'pie',
            radius : '55%',
            center: ['50%', '50%'],
            data:[
                {value:20, name:'人事部',itemStyle: {
                        color:'green'
                    }},
                {value:20, name:'财务部',itemStyle: {
                        color:'yellow'
                    }},
                {value:60, name:'业务部',itemStyle: {
                        color:'pink'
                    }},
            ],
            itemStyle: {
                emphasis: {
                    shadowBlur: 10,
                    shadowOffsetX: 0,
                    shadowColor: 'rgba(0, 0, 0, 0.5)'
                }
            }
        }
    ]
};
job.setOption(jobOption);




var company = echarts.init(document.getElementById('company'));
companyOption = {
    title : {
        text: '机构分布',
        x:'left',
        textStyle: {
            color:'white'
        }
    },
    tooltip : {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c} ({d}%)"
    },
    series : [
        {
            name: '比例',
            type: 'pie',
            radius : '55%',
            center: ['50%', '50%'],
            data:[
                {value:20, name:'人事部',itemStyle: {
                        color:'green'
                    }},
                {value:20, name:'财务部',itemStyle: {
                        color:'yellow'
                    }},
                {value:60, name:'业务部',itemStyle: {
                        color:'pink'
                    }},
            ],
            itemStyle: {
                emphasis: {
                    shadowBlur: 10,
                    shadowOffsetX: 0,
                    shadowColor: 'rgba(0, 0, 0, 0.5)'
                }
            }
        }
    ]
};
company.setOption(companyOption);



var area = echarts.init(document.getElementById('area'));
areaOption = {
    title : {
        text: '区域分布',
        x:'left',
        textStyle: {
            color:'white'
        }
    },
    tooltip : {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c} ({d}%)"
    },
    series : [
        {
            name: '比例',
            type: 'pie',
            radius : '55%',
            center: ['50%', '50%'],
            data:[
                {value:20, name:'人事部',itemStyle: {
                        color:'green'
                    }},
                {value:20, name:'财务部',itemStyle: {
                        color:'yellow'
                    }},
                {value:60, name:'业务部',itemStyle: {
                        color:'pink'
                    }},
            ],
            itemStyle: {
                emphasis: {
                    shadowBlur: 10,
                    shadowOffsetX: 0,
                    shadowColor: 'rgba(0, 0, 0, 0.5)'
                }
            }
        }
    ]
};
area.setOption(areaOption);


var department = echarts.init(document.getElementById('department'));
departmentOption = {
    title : {
        text: '部门分布',
        x:'left',
        textStyle: {
            color:'white'
        }
    },
    tooltip : {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c} ({d}%)"
    },
    series : [
        {
            name: '比例',
            type: 'pie',
            radius : '55%',
            center: ['50%', '50%'],
            data:[
                {value:20, name:'人事部',itemStyle: {
                        color:'green'
                    }},
                {value:20, name:'财务部',itemStyle: {
                        color:'yellow'
                    }},
                {value:60, name:'业务部',itemStyle: {
                        color:'pink'
                    }},
            ],
            itemStyle: {
                emphasis: {
                    shadowBlur: 10,
                    shadowOffsetX: 0,
                    shadowColor: 'rgba(0, 0, 0, 0.5)'
                }
            }
        }
    ]
};
department.setOption(departmentOption);

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