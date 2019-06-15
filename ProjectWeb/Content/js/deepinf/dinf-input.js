//是否是INT类型
function KeyIsIntNumber(KeyCode, key) {//如果输入的字符是在0-9之间，或者是backspace、DEL键
    if (KeyCode == 13) { event.keyCode = 9; return true; }
    if (KeyCode == 9) { event.keyCode = 9; return true; }
    if (((KeyCode > 47) && (KeyCode < 58)) || ((KeyCode > 95) && (KeyCode < 106)) || (KeyCode == 8) || (KeyCode == 46) || (KeyCode > 36 && KeyCode < 41) || (KeyCode == 189)) { return true; } else { return false; }
}
//是否是double类型
function KeyIsDoubleNumber(KeyCode, evalue) {
    if (KeyCode == 13) { event.keyCode = 9; return true; }
    if (KeyCode == 9) { event.keyCode = 9; return true; }
    if ((KeyCode == 190 || KeyCode == 110)) {
        if (evalue.indexOf(".") > -1)
            return false;
        else if (evalue == "")
            return false;
    }
    if (KeyCode == 13 || KeyCode == 46 || KeyCode == 8 || KeyCode == 110 || KeyCode == 190 || KeyCode == 189 || (KeyCode > 36 && KeyCode < 41))
        return true;
    if (KeyCode < 37 || KeyCode > 40 && KeyCode < 48 || (KeyCode > 57 && KeyCode < 96) || KeyCode > 105)
        return false;
    else
        return true;
}


//数字转中文数字(n为数字,suffix为后缀，如果要表示钱大写，只需把文字改一改)
function DX(n, suffix, options) {
    var defaults = {
        unit: "千百十亿千百十万千百十元角分",
        strDegit: "零一二三四五六七八九",
        ten: "十"
    };
    options = $.extend(defaults, options);
    if (!/^(0|[1-9]\d*)(\.\d+)?$/.test(n))
        return "数据非法";
    //var unit = "千百" + ten + "亿千百" + ten + "万千百" + ten + "元角分";
    var str = "";
    n += "00";
    var p = n.indexOf('.');
    if (p >= 0)
        n = n.substring(0, p) + n.substr(p + 1, 2);
    options.unit = options.unit.substr(options.unit.length - n.length);
    for (var i = 0; i < n.length; i++)
        str += options.strDegit.charAt(n.charAt(i)) + options.unit.charAt(i);
    return str.replace(/零(千|百|十|角)/g, "零").replace(/(零)+/g, "零").replace(/零(万|亿|元)/g, "$1").replace(/(亿)万|一(十)/g, "$1$2").replace(/^元零?|零分/g, "").replace(/元$/g, suffix);
}


function DXNew(n,suffix) {
    suffix = !!suffix ? suffix : "";
    if (!/^(0|[1-9]\d*)(\.\d+)?$/.test(n))
        return "数据非法";
    var unit = "千百十亿千百十万千百十元角分", str = "";
    n += "00";
    var p = n.indexOf('.');
    if (p >= 0)
        n = n.substring(0, p) + n.substr(p + 1, 2);
    unit = unit.substr(unit.length - n.length);
    for (var i = 0; i < n.length; i++)
        str += '零一二三四五六七八九'.charAt(n.charAt(i)) + unit.charAt(i);
    var digit = str.replace(/零(千|百|十|角)/g, "零").replace(/(零)+/g, "零").replace(/零(万|亿|元)/g, "$1").replace(/(亿)万|一(十)/g, "$1$2").replace(/^元零?|零分/g, "").replace(/元$/g, suffix);
    return digit == "分"&&digit.length==1 ? "零" + suffix : digit;
}

//数字转中文数字(n为数字,suffix为后缀，如果要表示钱大写，只需把文字改一改)
function DXUpper(n, suffix, options) {
    suffix = !!suffix ? suffix : "";
    var defaults = {
        unit: "仟佰拾亿仟佰拾万仟佰拾圆角分",
        strDegit: "零壹贰叁肆伍陆柒捌玖",
        ten: "拾"
    };
    options = $.extend(defaults, options);
    if (!/^(0|[1-9]\d*)(\.\d+)?$/.test(n))
        return "数据非法";
    //var unit = "千百" + ten + "亿千百" + ten + "万千百" + ten + "元角分";
    var str = "";
    n += "00";
    var p = n.indexOf('.');
    if (p >= 0)
        n = n.substring(0, p) + n.substr(p + 1, 2);
    options.unit = options.unit.substr(options.unit.length - n.length);
    for (var i = 0; i < n.length; i++)
        str += options.strDegit.charAt(n.charAt(i)) + options.unit.charAt(i);
    var digit = str.replace(/零(仟|佰|拾|角)/g, "零").replace(/(零)+/g, "零").replace(/零(万|亿|圆)/g, "$1").replace(/(亿)万|壹(拾)/g, "$1$2").replace(/^圆零?|零分/g, "").replace(/圆$/g, suffix);
    return digit == "分" && digit.length == 1 ? "零" + suffix : digit;
}


//四舍五入v参数，e长度
function Round(v, e) {
    var t = 1;
    for (; e > 0; t *= 10, e--);
    for (; e < 0; t /= 10, e++);
    return Math.round(v * t) / t;
}
//----------------------------身份证验证模块-------------------
// 构造函数，变量为15位或者18位的身份证号码
function clsIDCard(CardNo) {
    this.Valid = false;
    this.ID15 = '';
    this.ID18 = '';
    this.Local = '';
    if (CardNo != null) this.SetCardNo(CardNo);
}

// 设置身份证号码，15位或者18位
clsIDCard.prototype.SetCardNo = function (CardNo) {
    this.ID15 = '';
    this.ID18 = '';
    this.Local = '';
    CardNo = CardNo.replace(" ", "");
    var strCardNo;
    if (CardNo.length == 18) {
        pattern = /^\d{17}(\d|x|X)$/;
        if (pattern.exec(CardNo) == null) return;
        strCardNo = CardNo.toUpperCase();
    } else {
        pattern = /^\d{15}$/;
        if (pattern.exec(CardNo) == null) return;
        strCardNo = CardNo.substr(0, 6) + '19' + CardNo.substr(6, 9)
        strCardNo += this.GetVCode(strCardNo);
    }
    this.Valid = this.CheckValid(strCardNo);
}

// 校验身份证有效性
clsIDCard.prototype.IsValid = function (obj) {
    if (!this.Valid) {
        $(obj).parents('.formValue').addClass('has-error');    
        $(obj).parents('.has-error').find('div.tooltip').remove();
        $(obj).parents('.has-error').find('i.error').remove();
        $(obj).parents('.has-error').append('<i class="form-control-feedbacks fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="输入的身份证号无效,请输入有效的身份证号！"></i>');
        $("[data-toggle='tooltip']").tooltip();
        if ($(obj).parents('.input-group').hasClass('input-group')) {
            $(obj).parents('.has-error').find('i.error').addClass('has-error-pullright');
            
        }
    } else {
        $(obj).parents('.has-error').find('div.tooltip').remove();
        $(obj).parents('.has-error').find('i.error').remove();
        $(obj).parents().removeClass('has-error');
    }
    return this.Valid;
}

// 返回出生日期字符串，格式如下，1981-10-10
clsIDCard.prototype.GetBirthDate = function () {
    var BirthDate = '';
    if (this.Valid) BirthDate = this.GetBirthYear() + '-' + this.GetBirthMonth() + '-' + this.GetBirthDay();
    return BirthDate;
}

// 返回生日字符串，格式如下，10-10
clsIDCard.prototype.GetBirthday = function () {
    var BirthDate = '';
    if (this.Valid) BirthDate = this.GetBirthMonth() + '-' + this.GetBirthDay();
    return BirthDate;
}

// 返回生日中的年，格式如下，1981
clsIDCard.prototype.GetBirthYear = function () {
    var BirthYear = '';
    if (this.Valid) BirthYear = this.ID18.substr(6, 4);
    return BirthYear;
}

// 返回生日中的月，格式如下，10
clsIDCard.prototype.GetBirthMonth = function () {
    var BirthMonth = '';
    if (this.Valid) BirthMonth = this.ID18.substr(10, 2);
    if (BirthMonth.charAt(0) == '0') BirthMonth = BirthMonth.charAt(1);
    return BirthMonth;
}

// 返回生日中的日，格式如下，10
clsIDCard.prototype.GetBirthDay = function () {
    var BirthDay = '';
    if (this.Valid) BirthDay = this.ID18.substr(12, 2);
    return BirthDay;
}

// 返回性别，1：男，0：女
clsIDCard.prototype.GetSex = function () {
    var Sex = '';
    if (this.Valid) Sex = this.ID18.charAt(16) % 2;
    return Sex;
}

// 返回15位身份证号码
clsIDCard.prototype.Get15 = function () {
    var ID15 = '';
    if (this.Valid) ID15 = this.ID15;
    return ID15;
}

// 返回18位身份证号码
clsIDCard.prototype.Get18 = function () {
    var ID18 = '';
    if (this.Valid) ID18 = this.ID18;
    return ID18;
}

// 返回所在省，例如：上海市、浙江省
clsIDCard.prototype.GetLocal = function () {
    var Local = '';
    if (this.Valid) Local = this.Local;
    return Local;
}

clsIDCard.prototype.GetVCode = function (CardNo17) {
    var Wi = new Array(7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1);
    var Ai = new Array('1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2');
    var cardNoSum = 0;
    for (var i = 0; i < CardNo17.length; i++) cardNoSum += CardNo17.charAt(i) * Wi[i];
    var seq = cardNoSum % 11;
    return Ai[seq];
}

clsIDCard.prototype.CheckValid = function (CardNo18) {
    if (this.GetVCode(CardNo18.substr(0, 17)) != CardNo18.charAt(17)) return false;
    if (!this.IsDate(CardNo18.substr(6, 8))) return false;
    var aCity = { 11: "北京", 12: "天津", 13: "河北", 14: "山西", 15: "内蒙古", 21: "辽宁", 22: "吉林", 23: "黑龙江 ", 31: "上海", 32: "江苏", 33: "浙江", 34: "安徽", 35: "福建", 36: "江西", 37: "山东", 41: "河南", 42: "湖北 ", 43: "湖南", 44: "广东", 45: "广西", 46: "海南", 50: "重庆", 51: "四川", 52: "贵州", 53: "云南", 54: "西藏 ", 61: "陕西", 62: "甘肃", 63: "青海", 64: "宁夏", 65: "新疆", 71: "台湾", 81: "香港", 82: "澳门", 91: "国外" };
    if (aCity[parseInt(CardNo18.substr(0, 2))] == null) return false;
    this.ID18 = CardNo18;
    this.ID15 = CardNo18.substr(0, 6) + CardNo18.substr(8, 9);
    this.Local = aCity[parseInt(CardNo18.substr(0, 2))];
    return true;
}

clsIDCard.prototype.IsDate = function (strDate) {
    var r = strDate.match(/^(\d{1,4})(\d{1,2})(\d{1,2})$/);
    if (r == null) return false;
    var d = new Date(r[1], r[2] - 1, r[3]);
    return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[2] && d.getDate() == r[3]);
}
//------------------------------------身份证验证模块结束---------------------------------------------------

//手机号码验证
function clsPhone(obj) {
    var reg = /^1[0-9]{10}$/;
    if (!reg.test($(obj).val())) {
        $(obj).parents('.formValue').addClass('has-error');
        $(obj).parents('.has-error').find('i.error').remove();
        $(obj).parents('.has-error').append('<i class="form-control-feedbacks fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="输入的手机号码无效,请输入有效的手机号码！"></i>');
        $("[data-toggle='tooltip']").tooltip();
        if ($(obj).parents('.input-group').hasClass('input-group')) {
            $(obj).parents('.has-error').find('i.error').addClass('has-error-pullright');
        }
        return false;
    } else {
        $(obj).parents('.has-error').find('i.error').remove();
        $(obj).parents().removeClass('has-error');
        return true;
    }

}

//银行卡号号码验证
function clsBank(obj) {
    var reg = /^\d{16}|\d{19}$/;// 银行卡有可能是16位也有可能是19位
    // var reg = /^\d{4}(?:\s\d{4}){3}$/;// 如果你只要带空格的16位
    if (!reg.test($(obj).val().replace(/\s/g, ""))) {
        $(obj).parents('.formValue').addClass('has-error');
        $(obj).parents('.has-error').find('i.error').remove();
        $(obj).parents('.has-error').append('<i class="form-control-feedbacks fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="输入的银行卡号无效,请输入银行卡号！"></i>');
        $("[data-toggle='tooltip']").tooltip();
        if ($(obj).parents('.input-group').hasClass('input-group')) {
            $(obj).parents('.has-error').find('i.error').addClass('has-error-pullright');
        }
        return false;
    } else {
        $(obj).parents('.has-error').find('i.error').remove();
        $(obj).parents().removeClass('has-error');
        return true;
    }
}

//银行卡号 输入4位自动补空格
function fillEmptyBank(obj, keyCode) {
    if (keyCode != 8) {                                      //判断是否为Backspace键，若不是执行函数；
        var b = $(obj).val();          //定义变量input  value值；
        var maxValue = 23;                                       //限制输入框的最大值；
        b = b.replace(/[^\d\s]/g, "");                           //正则表达式：如果输入框中输入的不是数字或者空格，将不会显示；
        $(obj).val(b);               //把新得到得value值赋值给输入框；
        for (n = 1; n <= 4; n++) {
            if (b.length <= 5 * n - 2 || b.length > 5 * n - 1) { //判断是否是该加空格的时候，若不会，还是原来的值；
                b = b;
            } else {
                b = b + " ";                                     //给value添加一个空格；
                $(obj).val(b);          //赋值给输入框新的value值；
            }
        }
    }
    //encode
    function compile(code) {
        var c = String.fromCharCode(code.charCodeAt(0) + code.length);
        for (var i = 1; i < code.length; i++) {
            c += String.fromCharCode(code.charCodeAt(i) + code.charCodeAt(i - 1));
        }
        //alert(escape(c));
        return escape(c);
    }
    //compile('alert("黑客防线");');

    //decode
    function uncompile(code) {
        code = unescape(code);
        var c = String.fromCharCode(code.charCodeAt(0) - code.length);
        for (var i = 1; i < code.length; i++) {
            c += String.fromCharCode(code.charCodeAt(i) - c.charCodeAt(i - 1));
        }
        return c;
    }
}

