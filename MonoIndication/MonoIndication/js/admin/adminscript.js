
$(function() {

    // состояние базы
    $("#DbSettingLink").click(
        function (e) {
            showLoading(true);
            $.get('././DbStatus', {},
                function(data) {
                    $("#AdminContainer").html(data);
                })
            .error(function (jqXHR, textStatus, errorThrown) {
                $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка! Подробные сведения можно помотреть в консоли браузера</h4>");
                console.log(jqXHR.responseText);
            }).complete(function () {
                showLoading(false);
            });

            e.preventDefault();

        }
    );
    
    // добавление пользователя
    $("#UserCreateLink").click(
        function (e) {
            showLoading(true);
            $.get('././CreateNewUser', {},
                function (data) {
                    $("#AdminContainer").html(data);
                })
            .error(function (jqXHR, textStatus, errorThrown) {
                $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка! Подробные сведения можно помотреть в консоли браузера</h4>");
                console.log(jqXHR.responseText);
            }).complete(function () {
                showLoading(false);
            });
            e.preventDefault();
        }
    );
    
    // просмотр пользователей
    $("#UserEditLink").click(
        function (e) {
            showLoading(true);
            $.get('././ViewUsers', {},
                function (data) {
                    $("#AdminContainer").html(data);
                })
            .error(function (jqXHR, textStatus, errorThrown) {
                $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка! Подробные сведения можно помотреть в консоли браузера</h4>");
                console.log(jqXHR.responseText);
            }).complete(function () {
                showLoading(false);
            });
            e.preventDefault();
        }
    );
    
    $("#LogViewLink").click(
        function (e) {
            showLoading(true);
            $.get('././ViewLog', {},
                function (data) {
                    $("#AdminContainer").html(data);
                })
            .error(function (jqXHR, textStatus, errorThrown) {
                $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка! Подробные сведения можно помотреть в консоли браузера</h4>");
                console.log(jqXHR.responseText);
            }).complete(function () {
                showLoading(false);
            });
            e.preventDefault();
        }
    );

    $("#DbEditLink").click(
        function (e) {
            showLoading(true);
            $.get('././DbEditor', {},
                function (data) {
                    $("#AdminContainer").html(data);
                })
            .error(function (jqXHR, textStatus, errorThrown) {
                $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка! Подробные сведения можно помотреть в консоли браузера</h4>");
                console.log(jqXHR.responseText);
            }).complete(function () {
                showLoading(false);
            });

            e.preventDefault();
            //return false;
        }
    );


    $("#PhoneEditLink").click(function (e) {
        showLoading(true);
        $.get('././PhoneEdit', {},
            function (data) {
                $("#AdminContainer").html(data);
            })
        .error(function (jqXHR, textStatus, errorThrown) {
            $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка! Подробные сведения можно помотреть в консоли браузера</h4>");
            console.log(jqXHR.responseText);
        }).complete(function () {
            showLoading(false);
        });
        e.preventDefault();
    });
    

    $("#AdminPasswordChange").click(
    function (e) {
        showLoading(true);
        $.get('././ChangeAdminPassword', {},
            function (data) {
                $("#AdminContainer").html(data);
            })
        .error(function (jqXHR, textStatus, errorThrown) {
            $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка! Подробные сведения можно помотреть в консоли браузера</h4>");
            console.log(jqXHR.responseText);
        }).complete(function () {
            showLoading(false);
        });
        e.preventDefault();
    }
);
    
});