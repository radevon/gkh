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
        $.getJSON(url + '&callback=?', {}, function (data) {
            
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