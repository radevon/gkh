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