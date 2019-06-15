var id = $.request("id");
var $tableDistributor = $('#targetDistributor');
var $tableProduct = $('#targetProduct');
var $pTableWrap = $("#targetProductWrap");
var dFullList = [];  // 经销商
var pFullList = [];  // 机型
var proRebateList = [];

var productSecList = [];//机型 -- 指定型号
var productList = [];//型号列表
var rebateList = [];//返利列表
var rebateProList = [];//细
var timeSecList = [];//时间
var resList = [];//暂不知
var pSelListArr = new Array();    // 已选机型
pSelListArr.push([]);
var product_sec_i = 1;//按机型
var time_sec_i = 1;//按时间
var start_date = '';
var end_date = '';

//返利政策
var str2 = "";
var str4 = "";

$(function () {
    $(window).resize(function () {
        $tableDistributor.bootstrapTable('resetView', {
            height: 451
        });
        $tableProduct.bootstrapTable('resetView', {
            height: 451
        });
    });
    $.ajax({
        url: "/DistributorManage/Attaining/GetInfo?date=" + new Date(),
        type: "get",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            var target_sale = data.mainInfo.target_sale;
            start_date = data.mainInfo.start_date;
            end_date = data.mainInfo.end_date;
            $("#sale_avg_before").text(data.mainInfo.sale_avg_before);//前三个月
            if (data.mainInfo.activity_status == 2 || data.mainInfo.approve_status != 100) {
                $("#btn_edit").css("display", "none");
            }
            $("#creator_position_name").text(data.creator_position_name);
            $("#form1").formSerializeShow(data.mainInfo);
            if (data.mainInfo.money_included == 0) {
                $("#money_included").prop("checked", "checked");//特价机算量不算钱
            }
            var activity_status = data.mainInfo.activity_status;
            if (activity_status == 1) {
                $("#activity_status").text("进行中");
            } else if (activity_status == -1) {
                $("#activity_status").text("未开始");
            } else if (activity_status == 2) {
                $("#activity_status").text("已结束");
            }
            //返利类型
            var category = data.mainInfo.category;
            if (category == 2) {
                $("#category").text("主推返利");
            } else if (category == 3) {
                $("#category").text("订货返利");
            } else if (category == 4) {
                $("#category").text("无促保卡返利");
            }
            else
                $("#category").text("达量返利");
           
            $("#activity_target").text(ToThousandsStr(data.mainInfo.activity_target));

            //Table 
            TablesInit();
            //经销商tbale
            dFullList = data.distributorList;
            //pFullList = data.productList;
            $tableDistributor.bootstrapTable('load', dFullList);
            $("#distributor_count").text(dFullList.length);

            productSecList = data.productSecList;//机型分段
            productSecLen = productSecList.length;//机型分段长度
            productList = data.productList;//型号列表
            productLen = productList.length;

            rebateList = data.rebateList;//返利列表
            rebateLen = rebateList.length;//返利列表
            rebateProList = data.rebateProList;//细
            rebateProLen = rebateProList.length;//细
            timeSecList = data.timeSecList;//时间分段
            timeSecLen = timeSecList.length;//时间分段长度
            resList = data.resList;//暂不知

            for (var i = 0; i < productSecLen; i++) {
                var productCloneTable = "";
                product_sec_i = productSecList[i].product_sec_i;
                var proSum = 0;
                if (productSecList[i].product_scope == 2) {
                    productCloneTable += '<div id="targetProductWrap' + product_sec_i + '" ><table id="targetProduct' + product_sec_i + '" class="table table-bordered text-center"  style="display: table;">'
                  + '<thead style="display:table;width:99%;table-layout:fixed;">'
                  + '<tr><th class="col-sm-5"><span>型号</span></th>'
                  + '<th class="col-sm-4"><span>颜色</span></th>'
                  + '<th class="col-sm-3"><span>批发价（元/台）</span></th></tr></thead>'
                  + '<tbody style="display:block;height:391px;overflow-y:scroll;overflow-x:hidden">';
                    $.each(productList, function (pIndex, pItem) {
                        if (pItem.product_sec_i == product_sec_i) {
                            proSum++;
                            productCloneTable += '<tr style="display:table;width:100%;table-layout:fixed;"><td class="col-sm-5" style="text-align:center;vertical-align:middle;">' + pItem.model + '</td>'
                             + '<td class="col-sm-4" style="text-align:center;vertical-align:middle;">' + pItem.color + '</td>'
                             + '<td class="col-sm-3" style="text-align:center;vertical-align:middle;">' + pItem.price_wholesale + '</td> </tr>';
                        }
                    })
                    productCloneTable += '</tbody></table></div>';
                }


                var productClone = '<div class="col-xs-12 col-sm-12 col-lg-12" id="proBox' + product_sec_i + '" data-index="' + product_sec_i + '">'
                    + '<div class="box box-primary"><div class="box-header with-border" >   <h3 class="box-title">机型分段</h3>'
                    + '<div class="box-tools pull-right"><button type="button" class="btn btn-box-tool" data-widget="collapse">'
                    + '<i class="fa fa-minus"></i></button></div></div>'
                    + '<div class="box-body" style="background-color:#ecf0f5" id="box1">'
                    + '<div class="col-xs-12 col-sm-12 col-lg-6"><div class="col-xs-12 col-sm-12 col-lg-12 no-padding">'
                    + '<div class="box box-widget"><div class="box-header with-border">'
                    + '<h3 class="box-title">活动机型</h3>'
                    + '<h6 id="productCountWrap' + product_sec_i + '" class="inline margin" style="display:none">共<span class="margin" id="product_count' + product_sec_i + '">' + proSum + '</span>款</h6>'
                    + '<div class="box-tools pull-right">'
                    + '<button type="button" class="btn btn-box-tool" data-widget="collapse">'
                    + '<i class="fa fa-minus"></i></button></div></div>'
                    + '<div class="box-body"><div style="height:40px"><div class="radio col-sm-6">'
                    + '<label><input name="product_scope' + product_sec_i + '"  type="radio" value="1"  data-index="' + product_sec_i + '" disabled>全部机型</label></div>'
                    + '<div class="radio col-sm-6"><label><input name="product_scope' + product_sec_i + '" type="radio" value="2" data-index="' + product_sec_i + '" disabled>指定机型'
                    + '</label></div></div>';

                productClone += productCloneTable;

                productClone += '</div></div></div></div>';
                for (var j = 0; j < timeSecLen; j++) {
                    time_sec_i = timeSecList[j].time_sec_i;
                    if (timeSecList[j].product_sec_i == product_sec_i) {
                        productClone += '<div class="col-xs-12 col-sm-12 col-lg-6" id="copy_' + product_sec_i + '_' + time_sec_i + '"  data-index="1">'
                            + '<div class="col-xs-12 col-sm-12 col-lg-12 no-padding"><div class="box box-widget">'
                            + '<div class="box-header with-border"><div class="col-lg-12"><div class="col-lg-6">'
                            + '<div class="input-group col-lg-12"><input class="form-control" id="start_time_' + product_sec_i + '_' + time_sec_i + '" disabled value="' + timeSecList[j].start_date.substring(0, 10) + '" >'
                            + '<span class="input-group-addon no-border"> 至 </span>'
                            + '<input class="form-control" id="end_time_' + product_sec_i + '_' + time_sec_i + '" value="' + timeSecList[j].end_date.substring(0, 10) + '"disabled ></div></div></div>'
                            + '<div class="box-tools pull-right" >'
                            + '<button type="button" class="btn btn-box-tool" data-widget="collapse">'
                            + '<i class="fa fa-minus"></i></button></div></div>'
                            + '<div class="box-body"><div class="col-md-12 col-lg-12 margin-bottom">'
                            + '<div class="row"><div class="form-group col-sm-12"><label>返利模式</label>'
                            + '<div><div class="radio col-sm-3">'
                            + '<label><input name="target_mode_' + product_sec_i + '_' + time_sec_i + '" data-index="1" type="radio" value="3" disabled>按零售价</label></div>'
                            + '<div class="radio col-sm-3">'
                            + '<label><input name="target_mode_' + product_sec_i + '_' + time_sec_i + '" type="radio" data-index="1" value="5" disabled>按批发价</label></div>'
                            + '<div class="radio col-sm-3">'
                            + '<label><input name="target_mode_' + product_sec_i + '_' + time_sec_i + '" type="radio" data-index="1" value="4" disabled>按型号</label></div>'
                            + '<div class="radio col-sm-3">'
                            + '<label><input name="target_mode_' + product_sec_i + '_' + time_sec_i + '" type="radio" data-index="1" value="6" disabled>无'
                            + '</label></div></div></div></div>'
                            + '<div class="form-group"><label>返利金额</label>'
                            + '<div><div class="radio col-sm-3 col-lg-3">'
                            + '<label><input name="rebate_mode_' + product_sec_i + '_' + time_sec_i + '" data-index="1" type="radio" value="1" disabled >每台固定金额</label></div>'
                            + '<div class="radio col-sm-3 col-lg-3">'
                            + '<label><input name="rebate_mode_' + product_sec_i + '_' + time_sec_i + '" type="radio" data-index="1" value="2" disabled>每台批发价比例</label></div>'
                            + '<div class="radio col-sm-3 col-lg-3">'
                            + '<label><input name="rebate_mode_' + product_sec_i + '_' + time_sec_i + '" type="radio" data-index="1" value="3" disabled>每台零售价比例</label></div>'
                            + '<div class="radio col-sm-3 col-lg-3">'
                            + '<label><input name="rebate_mode_' + product_sec_i + '_' + time_sec_i + '" type="radio" data-index="1" value="4" disabled> 固定总金额'
                            + '</label></div></div></div></div><br />';

                        if (timeSecList[j].target_mode == 3 || timeSecList[j].target_mode == 5) {//3-零售价 5-批发价

                            var rebateMax = "";
                            var tr = '';
                            for (var k = 0; k < rebateLen; k++) {
                                if (rebateList[k].time_sec_i == time_sec_i) {
                                    var rebate_i = rebateList[k].rebate_i;
                                    var trProR = "";
                                    var rowSpan = 0;
                                    for (var l = 0; l < rebateProLen; l++) {
                                        rebate_i_pro = rebateProList[l].rebate_i;
                                        if (rebate_i_pro == rebate_i) {
                                            rowSpan++;
                                            if (rebateProList[l].target_max == -1)
                                                proRebateMax = "以上";
                                            else
                                                proRebateMax = rebateProList[l].target_max;

                                            if (rowSpan > 1)
                                                trProR += "<tr>";
                                            trProR += "<td  class='col-sm-4'  style='text-align:center;vertical-align:middle;'><div class='input-group formValue'>"
                                                    + "<input class='form-control text-center' name='pro_target_min' disabled value='" + ToThousandsStr(rebateProList[l].target_min) + "'>"
                                                    + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center  'disabled name='pro_target_max'  value='" + ToThousandsStr(proRebateMax) + "'>"
                                                    + "</div></td>"
                                                    + "<td class='col-sm-4' style='text-align:center;vertical-align:middle;' ><span>" + ToThousandsStr(rebateProList[l].rebate) + "</span>&nbsp;<span name='rebate_mode_input1_" + product_sec_i + "_" + time_sec_i + "' >" + str2 + "</span>&nbsp;"
                                                    + "<span name='rebate_mode_input2_" + product_sec_i + "_" + time_sec_i + "'>" + str4 + "</span></td></tr>";
                                        }
                                    }
                                    if (time_sec_i = rebateList[k].time_sec_i) {
                                        if (rebateList[k].target_max == -1) {
                                            rebateMax = "以上";
                                        } else {
                                            rebateMax = rebateList[k].target_max;
                                        }
                                        var trR = "<tr><td  class='col-sm-4' rowspan='" + rowSpan + "' style='text-align:center;'><div class='input-group formValue'>"
                                        + "<input class='form-control text-center' name='pro_target_min' disabled value='" + ToThousandsStr(rebateList[k].target_min) + "'>"
                                        + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center required number'disabled name='pro_target_max'  value='" + ToThousandsStr(rebateMax) + "'>"
                                        + "</div></td>";
                                    }
                                    tr += trR + trProR;
                                }
                            }
                            productClone += '<table id="rebateMode1_' + product_sec_i + '_' + time_sec_i + '" class="table table-bordered text-center">'
                            + '<thead style="display:table;width:99%;table-layout:fixed;">'
                            + '<tr><th class="col-sm-4"><span name="sale_change">完成率（%）</span></th>'
                            + '<th class="col-sm-4"><span name="target_mode_change_' + product_sec_i + '_' + time_sec_i + '">零售价（元）</span></th>'
                            + '<th class="col-sm-4"><span name="rebate_mode_change_' + product_sec_i + '_' + time_sec_i + '">返利金额</span></th>'
                            + '</tr></thead>'
                            + '<tbody style="display:block;height:273px;overflow-y:scroll;overflow-x:hidden">'
                            + tr
                            + '</tbody></table>';

                        } else if (timeSecList[j].target_mode == 4) {//型号

                            var rebateMax = "";
                            var tr = '';
                            for (var k = 0; k < rebateLen; k++) {
                                if (rebateList[k].time_sec_i == time_sec_i) {
                                    var rebate_i = rebateList[k].rebate_i;
                                    var trProR = "";
                                    var rowSpan = 0;
                                    for (var l = 0; l < rebateProLen; l++) {
                                        rebate_i_pro = rebateProList[l].rebate_i;
                                        if (rebate_i_pro == rebate_i) {
                                            rowSpan++;
                                            if (rebateProList[l].target_max == -1)
                                                proRebateMax = "以上";
                                            else
                                                proRebateMax = rebateProList[l].target_max;

                                            if (rowSpan > 1)
                                                trProR += "<tr>";
                                            trProR += "<td  class='col-sm-3' style='text-align:center;vertical-align:middle;' name='model_pro' >" + rebateProList[l].model + "</td><td class='col-sm-3' style='text-align:center;vertical-align:middle;' name='color_pro'>" + rebateProList[l].color + "</td>"
                                    + "<td class='col-sm-3' style='text-align:center;vertical-align:middle;' ><span>" + ToThousandsStr(rebateProList[l].rebate) + "</span>&nbsp;<span name='rebate_mode_input1_" + product_sec_i + "_" + time_sec_i + "'>" + str2 + "</span>&nbsp;"
                                    + "<span name='rebate_mode_input2_" + product_sec_i + "_" + time_sec_i + "' >" + str4 + "</span></td></tr>";
                                        }
                                    }
                                    if (time_sec_i = rebateList[k].time_sec_i) {
                                        if (rebateList[k].target_max == -1) {
                                            rebateMax = "以上";
                                        } else {
                                            rebateMax = rebateList[k].target_max;
                                        }
                                        var trR = "<tr><td  class='col-sm-3' rowspan='" + rowSpan + "' style='text-align:center;'><div class='input-group formValue'>"
                                    + "<input class='form-control text-center' name='pro_target_min' disabled value='" + ToThousandsStr(rebateList[k].target_min) + "'>"
                                    + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center required number'disabled name='pro_target_max'  value='" + ToThousandsStr(rebateMax) + "'>"
                                    + "</div></td>";
                                    }
                                    tr += trR + trProR;
                                }
                            }
                            productClone += '<table id="rebateMode4_' + product_sec_i + '_' + time_sec_i + '" data-index="1" class="table table-bordered text-center" >'
                                + ' <thead style="display: table;width: 99%;table-layout: fixed;"><tr>'
                                + ' <th style="text-align:center;vertical-align:middle;" class="col-sm-3"><span name="product_change">完成率（%）</span> </th>'
                                + ' <th style="text-align:center;vertical-align:middle;" class="col-sm-3">型号</th>'
                                + ' <th style="text-align:center;vertical-align:middle;" class="col-sm-3">颜色</th>'
                                + ' <th style="text-align:center;vertical-align:middle;" class="col-sm-3"><span name="rebate_mode_change_' + product_sec_i + '_' + time_sec_i + '">返利金额</span></th>'
                                + ' </tr></thead><tbody style="display: block;height:273px;overflow-y:scroll;overflow-x:hidden">'
                                + tr
                                + '</tbody></table>';

                        }
                        else if (timeSecList[j].target_mode == 6) {//无
                            var rebateMax = "";
                            var tr = '';
                            for (var k = 0; k < rebateLen; k++) {
                                if (rebateList[k].time_sec_i == time_sec_i) {
                                    var rebate_i = rebateList[k].rebate_i;
                                    var trProR = "";
                                    trProR += "<td class='col-sm-6' style='text-align:center;vertical-align:middle;' >"
                                            + "<span>" + ToThousandsStr(rebateList[k].rebate) + "</span>&nbsp;<span name='rebate_mode_input1_" + product_sec_i + "_" + time_sec_i + "'>" + str2 + "</span>&nbsp;"
                                            + "<span name='rebate_mode_input2_" + product_sec_i + "_" + time_sec_i + "' >" + str4 + "</span></td></tr>"
                                    if (time_sec_i = rebateList[k].time_sec_i) {
                                        if (rebateList[k].target_max == -1) {
                                            rebateMax = "以上";
                                        } else {
                                            rebateMax = rebateList[k].target_max;
                                        }
                                        var trR = "<tr><td  class='col-sm-6'  style='text-align:center;vertical-align:middle;'><div class='input-group formValue'>"
                                        + "<input class='form-control text-center' name='pro_target_min_" + product_sec_i + "_" + time_sec_i + "' disabled value='" + ToThousandsStr(rebateList[k].target_min) + "'>"
                                        + "<span class='input-group-addon no-border'>-</span><input class='form-control text-center required number'disabled name='pro_target_max_" + product_sec_i + "_" + time_sec_i + "'  value='" + ToThousandsStr(rebateMax) + "'>"
                                        + "</div></td>";;
                                    }
                                    tr += trR + trProR;
                                }
                            }
                            productClone += '<table id="rebateMode6_' + product_sec_i + '_' + time_sec_i + '" class="table table-bordered text-center" >'
                          + '<thead style="display:table;width:99%;table-layout:fixed;">'
                          + '<tr><th class="col-sm-6"><span name="sale_change">完成率（%）</span></th>'
                          + '<th class="col-sm-6"><span name="rebate_mode_change_' + product_sec_i + '_' + time_sec_i + '">返利金额</span></th></tr></thead>'
                          + '<tbody style="display:block;height:273px;overflow-y:scroll;overflow-x:hidden">'
                          + tr
                          + '</tbody></table>';
                        }
                        productClone += '</div></div></div></div>'
                    }
                }
                productClone += '</div></div></div>';
                $("#bigBox").append(productClone);
                if (productSecList[i].product_scope == 2) {
                    $("input[name = 'product_scope" + product_sec_i + "'][value=2]").prop("checked", "checked");
                    $("#targetProductWrap" + product_sec_i).css("display", "");
                    $("#productCountWrap" + product_sec_i).css("display", "");
                } else {
                    $("input[name = 'product_scope" + product_sec_i + "'][value=1]").prop("checked", "checked");
                }
            }
            for (var m = 0; m < timeSecLen; m++) {
                var index = timeSecList[m].product_sec_i;
                var times = timeSecList[m].time_sec_i;
                $("input[name = 'target_mode_" + index + "_" + times + "'][value=" + timeSecList[m].target_mode + "]").prop("checked", "checked");
                $("input[name = 'rebate_mode_" + index + "_" + times + "'][value=" + timeSecList[m].rebate_mode + "]").prop("checked", "checked");
                if (timeSecList[m].target_mode == 3) {
                    $("span[name='target_mode_change_" + index + "_" + times + "']").html("零售价（元/台）");
                } else if (timeSecList[m].target_mode == 5) {
                    $("span[name='target_mode_change_" + index + "_" + times + "']").html("批发价（元/台）");
                }

                if (timeSecList[m].rebate_mode == 1) {
                    $("span[name='rebate_mode_change_" + index + "_" + times + "']").html("返利金额");
                    $("span[name='rebate_mode_input1_" + index + "_" + times + "']").html("元/台");
                    $("span[name='rebate_mode_input2_" + index + "_" + times + "']").html("");
                } else if (timeSecList[m].rebate_mode == 2) {
                    $("span[name='rebate_mode_change_" + index + "_" + times + "']").html("批发价比例");
                    $("span[name='rebate_mode_input1_" + index + "_" + times + "']").html("%");
                    $("span[name='rebate_mode_input2_" + index + "_" + times + "']").html("*批发价");

                } else if (timeSecList[m].rebate_mode == 3) {
                    $("span[name='rebate_mode_change_" + index + "_" + times + "']").html("零售价比例");
                    $("span[name='rebate_mode_input1_" + index + "_" + times + "']").html("%");
                    $("span[name='rebate_mode_input2_" + index + "_" + times + "']").html("*零售价");

                } else if (timeSecList[m].rebate_mode == 4) {
                    $("span[name='rebate_mode_change_" + index + "_" + times + "']").html("固定金额");
                    $("span[name='rebate_mode_input1_" + index + "_" + times + "']").html("元");
                    $("span[name='rebate_mode_input2_" + index + "_" + times + "']").html("");
                }
            }
            if (target_sale == 1) {
                $("#act_show").css("display", "none");
                $("span[name='sale_change']").html("完成率（%）");
                $("span[name='product_change']").html("完成率（%）");

            } else if (target_sale == 2) {
                $("#act_show").css("display", "");
                $("span[name='sale_change']").html("销量（台）");
                $("span[name='product_change']").html("销量（台）");

            }
        },
        error: function (data) {
            $.modalAlert(data.responseText, 'error');
        }
    });
})

function TablesInit() {
    initTable($tableDistributor, tableDistributorHeader);
    $tableDistributor.bootstrapTable('load', dFullList);
    $("#distributor_count").text(dFullList.length);
    $tableProduct.bootstrapTable('load', pFullList);
    $("#product_count").text(pFullList.length);
}

var tableDistributorHeader = [
    { field: "id", visible: false, },
    { field: "distributor_name", title: "经销商", align: "center" },
    { field: "area_l1_name", title: "经理片区", align: "center", },
]

function initTable($table, tableHeader) {
    //1.初始化Table
    var oTable = new TableInit($table, tableHeader);
    oTable.Init();
}
var TableInit = function ($table, tableHeader) {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $table.bootstrapTable({
            height: 451,
            url: "",        //请求后台的URL（*）
            striped: false,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: false,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            sidePagination: "client",           //分页方式：client客户端分页，server服务端分页（*）
            uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: tableHeader,
        });
    };
    return oTableInit;
};
// Table END
function OpenForm(url, title) {
    //if (title == "修改结束时间")
    url += "?id=" + id + "&start_date=" + start_date + "&end_date=" + end_date;
    $.modalOpen({
        id: "Form",
        title: title,
        url: url,
        width: "600px",
        height: "300px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}

function EditFull() {
    window.location.href = "/DistributorManage/Attaining/EditFull?id=" + id;
}