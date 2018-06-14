function showLoading(param) {
    var wall = document.getElementById('backgrounding');
    if(param==true)
        wall.style.display = "block";
    else {
        wall.style.display = "none";
    }
    
}

function sendRequestLog(url,phone) {
    $.ajax({
        url: url,
        data: {'phone':phone},
        dataType: 'json',
        method: 'POST',
        success:function(data){},
        error:function(error){}
    });
}

function trackEvents(url, seconds_ago) {
    $.ajax({
        url: url,
        data: { 'seconds': seconds_ago },
        dataType: 'json',
        method: 'GET',
        success: function (data) {
            
            if (data.length > 0) {
                var msg = 'Сработка датчика проникновения: ';
                for (var i = 0; i < data.length; i++) {
                    var dt = null;
                    try{
                        dt = new Date(parseInt(data[i].EvtTime.substr(6)));
                    }catch(e){

                    }
                    msg += '<a href="Itp/Index?id=' + data[i].MarkerId + '" title="'+dt+'">' + data[i].Address + '</a>';
                    switch (data[i].EventStatus) {
                        case 0: msg += '- <span style="color:green; background-color:#fff">закрыто</span>, '; break;
                        case 1: msg += '- <span style="color:red; background-color:#fff">открыто</span>, '; break;
                        default: msg += '- <span style="color:gray; background-color:#fff">неизвестн</span>, ';
                    }
                    // - <span class="label label-success">'+data[i].EventStatus+'</span>, ';
                }

                customAlert(msg, 'dangerAlert', 5);
            }
            
        },
        error: function (error) { }
    });
}

function openweathermap(fn) {
    
        var url = 'http://api.openweathermap.org/data/2.5/weather?q=Chojniki&appid=f62c1e09b0f0ac4468daf91c3fd39089&units=metric';
        $.getJSON(url + '&callback=?', {  }, function (data) {
            
            fn(data);
        });
   
}

function customAlert(message, clas,delay) {
    var aler = $("<div class='alert fixedAlert " + clas + " alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>" + message + "</div>");
    $("body").append(aler);
    $(aler).delay(delay*1000).fadeOut("slow", function () { $(this).remove(); });

}

var RedBlueColorGenerator = function (minValue, maxValue) {

    if (minValue === undefined || maxValue === undefined)
        throw "Диаппазон изменения переменной не задан!";

    if (minValue === maxValue)
        throw "Диаппазон значений заданн не корректно. Границы равны!";

    if(+minValue>+maxValue){
        this.minValue = +maxValue;
        this.maxValue = +minValue;
    }else
    {
        this.minValue = +minValue;
        this.maxValue = +maxValue;
    }
    

    this.delta = 255 / (this.maxValue - this.minValue);

    this.R = function (T) {
        return this.delta * (T - this.minValue);
    }

    this.G = function (T) {
        return 130;
    }

    this.B = function (T) {
        return this.delta * (this.maxValue-T);
    }

    this.RGB = function (T) {
        if (T < this.minValue)
            T=this.minValue;
        if (T > this.maxValue)
            T = this.maxValue;
            
        return 'rgb('+this.R(T) + ',' + this.G(T) + ',' + this.B(T)+')';
    }
}

function toExcel(tables,name) {

    if ("ActiveXObject" in window) {
        alert("This is Internet Explorer! Not Supported...");
    } else {

        var cache = {};
        this.tmpl = function tmpl(str, data) {
            var fn = !/\W/.test(str) ? cache[str] = cache[str] || tmpl(document.getElementById(str).innerHTML) :
            new Function("obj",
                         "var p=[],print=function(){p.push.apply(p,arguments);};" +
                         "with(obj){p.push('" +
                         str.replace(/[\r\t\n]/g, " ")
                         .split("{{").join("\t")
                         .replace(/((^|}})[^\t]*)'/g, "$1\r")
                         .replace(/\t=(.*?)}}/g, "',$1,'")
                         .split("\t").join("');")
                         .split("}}").join("p.push('")
                         .split("\r").join("\\'") + "');}return p.join('');");
            return data ? fn(data) : fn;
        };
        var tableToExcel = (function () {
            var uri = 'data:application/vnd.ms-excel;base64,',
                template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{{=worksheet}}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body>{{for(var i=0; i<tables.length;i++){ }}<table>{{=tables[i]}}</table>{{ } }}</body></html>',
                base64 = function (s) {
                    return window.btoa(unescape(encodeURIComponent(s)));
                },
                format = function (s, c) {
                    return s.replace(/{(\w+)}/g, function (m, p) {
                        return c[p];
                    });
                };
            return function (tableList, name) {
                if (!tableList.length > 0 && !tableList[0].nodeType) table = document.getElementById(table);
                var tables = [];
                for (var i = 0; i < tableList.length; i++) {
                    tables.push(tableList[i].innerHTML);
                }
                var ctx = {
                    worksheet: name || 'Worksheet',
                    tables: tables
                };
                window.location.href = uri + base64(tmpl(template, ctx));
            };
        })();
        tableToExcel(tables, name);
    }
}
