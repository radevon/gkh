var eventApp = angular.module("eventApp",[]);


eventApp.controller("evtctrl", function ($scope, $interval, dataServise) {

    $scope.objects = [];


    $scope.init = function () {
        $scope.loadData();
        $interval(function () { $scope.loadData(); }, 10000);
    }

    $scope.loadData = function () {
        dataServise.getobjects()
            .success(function (data) {

                for (var i = 0; i < data.length; i++) {

                    // ищу в массиве элемент по id
                    var item = $scope.objects.find(function (elem, index, array) {
                        return elem.id === data[i].MarkerId;
                    });

                    if (item === undefined) {
                        $scope.objects.push(dataServise.mapTo(data[i]));
                    } else {
                        var indx = $scope.objects.indexOf(item);

                        $scope.objects[indx].addres = data[i].Address;
                        $scope.objects[indx].phone = data[i].Phone;
                        $scope.objects[indx].descr = data[i].Description;
                        $scope.objects[indx].evtTime = new Date(parseInt(data[i].EvtTime.substr(6)));
                        $scope.objects[indx].evtStat = parseInt(data[i].EventStatus);
                    }

                }
                

            })
            .error(function (error) { console.log(error); });
    }

    $scope.prettyDate = function (date) {
        if (date != undefined) {
            if (date > new Date(2000, 1, 1)) {
                //return date.toLocaleString('ru-RU', { day: 'numeric', month: 'short', year: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric', timeZone: 'Europe/Minsk' });
                return moment(date).format("DD MMM YYYY, HH:mm:ss");
            }
        }
        return '';
    };

});


// получить данные
eventApp.service("dataServise", function ($http) {

    this.getobjects = function () {
        return $http({
            url: './././GetObjectsEvents',
            method: 'GET',
            dataType: 'json',
            cache: false
        });
    };

   
    this.mapTo = function (item) {
        return {
            id: item.MarkerId,
            addres: item.Address,
            phone: item.Phone,
            descr: item.Description,
            evtTime: new Date(parseInt(item.EvtTime.substr(6))),
            evtStat: parseInt(item.EventStatus)
        };
    };



});
