//离职信息
var $gridList = $("#gridList");
var $gridList2 = $("#gridList2");
var $gridList3 = $("#gridList3");
var w = $("#list_area").width();
var h = $(window).height() - 50 - 33 - $(".topPanel").height() - $(".panel-heading").height() - 8;
$(function () {
    LoadGridList();
    $(".panel-heading").authorizeButton();
    $(window).resize(function () {
        w = $("#list_area").width();
        h = $(window).height() - 50 - 33 - $(".topPanel").height() - $(".panel-heading").height() - 8;
        $(".topPanel").width(w);
        $("#gridList").setGridWidth(w);
        $("#gridList2").setGridWidth(w);
        $("#gridList3").setGridWidth(w);
        $(".ui-jqgrid").find(".ui-jqgrid-bdiv").height(h);
    })
    // 筛选按钮1
    $("#btnSearch").on("click", function () {
        ReSearch();
    });
    //$("#area_name").bindNormalSelect({
    //    text: "name",
    //    url: "/CompanyManage/Area/GetSelectJson2",
    //    firstText: "--请选择所属区域--",
    //});
    //$("#dept_name").bindNormalSelect({
    //    text: "name",
    //    url: "/CompanyManage/Department/GetSelectJson2",
    //    firstText: "--请选择所属部门--",
    //});
    // 筛选按钮2
    $("#btnSearch2").on("click", function () {
        ReSearch2();
    });
    // 筛选按钮3
    $("#btnSearch3").on("click", function () {
        $gridList3.jqGrid('setGridParam', {
            postData: { name: $("#name").val() }, page: 1
        }).trigger('reloadGrid');
    });
})
//查看所有
function LoadGridList() {
    $gridList.dataGrid({
        url: "/HumanResource/Resign/GetListAll?date=" + new Date(),
        height: h,
        colModel: [
            { label: '主键', name: 'id', hidden: true, key: true },
            { label: '姓名', name: 'emp_name', width: 80, align: 'center' },
            { label: '工号', name: 'work_number', width: 80, align: 'center' },
            {
                label: '审批状态', name: 'approve_status', width: 80, align: 'center',
                formatter: function (cellvalue) {
                    if (cellvalue == 0) {
                        return "未审批"
                    } else if (cellvalue == 100) {
                        return "通过"
                    } else if (cellvalue > 0) {
                        return "审批中"
                    } else
                        return "未通过"
                }
            },
            {
                label: '类型', name: 'type', width: 80, align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    if (cellvalue == 1) {
                        return "正常离职";
                    } else if (cellvalue == 2) {
                        return "辞退";
                    } else if (cellvalue == 3) {
                        return "自离";
                    }
                }
            },
            { label: '机构', name: 'company_name', width: 100, align: 'center' },
            { label: '区域', name: 'area_name', width: 100, align: 'center' },
            { label: '部门', name: 'dept_name', width: 100, align: 'center' },
            { label: '职位', name: 'position_name', width: 100, align: 'center' },
            { label: '入职日期', name: 'entry_date', width: 100, align: 'center' },
            { label: '离职日期', name: 'request_date', width: 100, align: 'center' },
            { label: '工作年限', name: 'area_name_new', width: 80, align: 'center' ,
                formatter: function (gender, options, rowObject) {
                    return (new Date(rowObject.request_date)).getFullYear() - (new Date(rowObject.entry_date)).getFullYear();
                }
            },
            {
                label: '性别', name: 'gender', width: 80, align: 'center',
                formatter: function (gender, options, rowObject) {
                    if (gender == 1) {
                        return "男";
                    } else if (gender == 0) {
                        return "女";
                    }else{
                        return "未指定";
                    }
                }
            },
            { label: '学历', name: 'edu', width: 80, align: 'center' },
            { label: '出生日期', name: 'birthdate', width: 100, align: 'center' },
            { label: '年龄', name: 'age', width: 80, align: 'center' ,
                formatter: function (gender, options, rowObject) {
                    return (new Date()).getFullYear() - (new Date(rowObject.birthdate)).getFullYear();
                }},
            { label: '离职原因', name: 'content_reason', width: 100, align: 'center' },
        ],
        //sortname: 'entry_time',
        sortorder: 'desc',
        rowNum: 20,
        pager: "#gridPager",
        viewrecords: true,
        onSelectRow: function (id) {
            window.location.href = "/HumanResource/Resign/Show?id=" + id
        },
    });
    $("#gridList").setGridWidth(w);
}

//我的审批
function LoadGridList2() {
    $gridList2.dataGrid({
        url: "/HumanResource/Resign/GetListApprove?date=" + new Date(),
        height: h,
        colModel: [
            { label: '主键', name: 'id', hidden: true, key: true },
            { label: '姓名', name: 'emp_name', width: 80, align: 'center' },
            { label: '工号', name: 'work_number', width: 80, align: 'center' },
            {
                label: '审批状态', name: 'approve_status', width: 80, align: 'center',
                formatter: function (cellvalue) {
                    if (cellvalue == 0) {
                        return "未审批"
                    } else if (cellvalue == 100) {
                        return "通过"
                    } else if (cellvalue > 0) {
                        return "审批中"
                    } else
                        return "未通过"
                }
            },
            {
                label: '类型', name: 'type', width: 80, align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    if (cellvalue == 1) {
                        return "正常离职";
                    } else if (cellvalue == 2) {
                        return "辞退";
                    } else if (cellvalue == 3) {
                        return "自离";
                    }
                }
            },
            { label: '机构', name: 'company_name', width: 100, align: 'center' },
            { label: '区域', name: 'area_name', width: 100, align: 'center' },
            { label: '部门', name: 'dept_name', width: 100, align: 'center' },
            { label: '职位', name: 'position_name', width: 100, align: 'center' },
            { label: '入职日期', name: 'entry_date', width: 100, align: 'center' },
            { label: '离职日期', name: 'request_date', width: 100, align: 'center' },
            { label: '工作年限', name: 'area_name_new', width: 80, align: 'center' ,
                formatter: function (gender, options, rowObject) {
                    return (new Date(rowObject.request_date)).getFullYear() - (new Date(rowObject.entry_date)).getFullYear();
                }
                },
            {
                label: '性别', name: 'gender', width: 80, align: 'center',
                formatter: function (gender, options, rowObject) {
                    if (gender == 1) {
                        return "男";
                    } else if (gender == 0) {
                        return "女";
                    }else{
                        return "未指定";
                    }
                }
            },
            { label: '学历', name: 'edu', width: 80, align: 'center' },
            { label: '出生日期', name: 'birthdate', width: 100, align: 'center' },
            { label: '年龄', name: 'age', width: 80, align: 'center' ,
                formatter: function (gender, options, rowObject) {
                    return (new Date()).getFullYear() - (new Date(rowObject.birthdate)).getFullYear();
                }},
            { label: '离职原因', name: 'content_reason', width: 100, align: 'center' },
        ],
        sortname: 'create_time',
        sortorder: 'desc',
        rowNum: 20,
        pager: "#gridPager2",
        viewrecords: true,
        onSelectRow: function (id) {
            window.location.href = "/HumanResource/Resign/Approve?id=" + id
        },
    });
    $("#gridList2").setGridWidth(w);
}

// 点击tab加载列表
var cur_grid = 1;
function LoadGrid(grid_index) {
    if (grid_index == cur_grid)
        return;
    if (grid_index == 1)
        LoadGridList();
    else if (grid_index == 2)
        LoadGridList2();
    cur_grid = grid_index;
}


$("#btn_fold").click(function () {
    $("#fold_block_wrap").slideToggle();
});
$("#btn_fold2").click(function () {
    $("#fold_block_wrap2").slideToggle();
});


function ReSearch() {
    $gridList.jqGrid('setGridParam', {
        postData: {
            name: $("#name").val(),
            approve_status: $("#approve_status option:selected").val(),
        }, page: 1
    }).trigger('reloadGrid');
}

function ReSearch2() {
    $gridList.jqGrid('setGridParam', {
        postData: {
            name: $("#name2").val(),
            approve_status: $("#approve_status2 option:selected").val(),
        }, page: 1
    }).trigger('reloadGrid');
}



