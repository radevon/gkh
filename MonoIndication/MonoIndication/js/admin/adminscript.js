
$(function() {

    // состояние базы
    $("#DbSettingLink").click(
        function () {
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

        }
    );
    
    // добавление пользователя
    $("#UserCreateLink").click(
        function () {
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

        }
    );
    
    // просмотр пользователей
    $("#UserEditLink").click(
        function () {
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

        }
    );
    
    $("#LogViewLink").click(
        function () {
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
        }
    );

    $("#DbEditLink").click(
        function () {
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
        }
    );


    $("#PhoneEditLink").click(function () {
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
    });
    

    $("#AdminPasswordChange").click(
    function () {
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
    }
);
    
});