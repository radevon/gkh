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

function trackEvents(url,minutes_ago) {
    $.ajax({
        url: url,
        data: { 'minute': minutes_ago },
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

function customAlert(message, clas,delay) {
    var aler = $("<div class='alert fixedAlert " + clas + " alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>" + message + "</div>");
    $("body").append(aler);
    $(aler).delay(delay*1000).fadeOut("slow", function () { $(this).remove(); });

}