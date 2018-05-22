
$(function() {

    // состояние базы
    $("#DbSettingLink").click(
        function (e) {
            showLoading(true);
            $.ajax({
                url: '././DbStatus',
                type: "GET",
                
                success: function (data) {
                    $("#AdminContainer").html(data);
                    showLoading(false);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка! </h4>");
                    showLoading(false);
                }
            });

            e.preventDefault();

        }
    );
    
    // добавление пользователя
    $("#UserCreateLink").click(
        function (e) {
            showLoading(true);
            $.ajax(
                {
                    url: '././CreateNewUser',
                    type: "GET",
                    success: function (data) {
                        $("#AdminContainer").html(data);
                        showLoading(false);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка! </h4>");

                        showLoading(false);
                    }
                });
            e.preventDefault();
        }
    );
    
    // просмотр пользователей
    $("#UserEditLink").click(
        function (e) {
            showLoading(true);
            $.ajax(
                {
                    url: '././ViewUsers', type: "GET",
                    success: function (data) {
                        $("#AdminContainer").html(data);
                        showLoading(false);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка! </h4>");

                        showLoading(false);
                    }
                });
            e.preventDefault();
        }
    );
    
    $("#LogViewLink").click(
        function (e) {
            showLoading(true);
            $.ajax(
                {
                    url: '././ViewLog',
                    success: function (data) {
                        $("#AdminContainer").html(data);
                        showLoading(false);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка!</h4>");

                        showLoading(false);
                    }
                });
            e.preventDefault();
        }
    );

    $("#DbEditLink").click(
        function (e) {
            showLoading(true);
            $.ajax({
                url: '././DbEditor',
                success: function (data) {
                    $("#AdminContainer").html(data);
                    showLoading(false);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка! </h4>");

                    showLoading(false);
                }
            });

            e.preventDefault();
            //return false;
        }
    );


    $("#PhoneEditLink").click(function (e) {
        showLoading(true);
        $.ajax({
                    url: '././PhoneEdit',
                    success: function (data) {
                        $("#AdminContainer").html(data);
                        showLoading(false);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка! </h4>");

                        showLoading(false);
                    }
                });
        e.preventDefault();
    });
    

    $("#AdminPasswordChange").click(
    function (e) {
        showLoading(true);
        $.ajax({
            url: '././ChangeAdminPassword',
            success: function (data) {
                $("#AdminContainer").html(data);
                showLoading(false);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("#AdminContainer").html("<h4 class='text-danger'>Возникла ошибка!</h4>");

                showLoading(false);
            }
        });
        e.preventDefault();
    }
);
    
});